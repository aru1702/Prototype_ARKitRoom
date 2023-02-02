using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeightFunction;


namespace CorrectionFunctions
{
    /// <summary>
    /// This version ONE will update the marker in NewARScene.
    /// Requirements: 1) Marker ground truth in runtime, 2) Objects
    /// Enable this function by attach this script to GameObject (enable/disable as trigger)
    ///
    /// On this B version Last Marker, it also rotate the world origin by certain
    /// error rotation in Quaternion with data from the every detected marker.
    ///
    /// By using Quaternion SLERP, we will use all marker data and average them
    /// based on weight that from distance (t = w).
    /// </summary>
    public class VersionOneBAvgWMarker : MonoBehaviour
    {
        List<MarkerLocation> m_Markers;
        List<GameObject> m_Objects;
        List<GameObject> m_MarkersGroundTruth;
        List<Vector3> m_InitObjectsLocations;
        ObjectToMarkers OTM;

        [SerializeField]
        [Tooltip("To get camera game object information.")]
        GameObject m_ARCamera;

        [SerializeField]
        [Tooltip("To import object location.")]
        GameObject m_LoadObjectManager;

        [SerializeField]
        [Tooltip("To import marker location in runtime.")]
        GameObject m_ImageTrackingManager;

        [SerializeField]
        [Tooltip("Update runtime per interval in seconds.")]
        float m_UpdateInterval = 1.0f;

        [SerializeField]
        [Tooltip("Scalar multiplier for weight function.")]
        float m_ScalarWeight = 1.0f;

        [SerializeField]
        [Tooltip("Threshold of rotation angle to update initial data.")]
        float ANGLE_THRESHOLD = 0.1f;

        Quaternion previous_rotation;

        // unused for this moment
#pragma warning disable 0414
        bool only_once_rotation = false;
        string previous_marker = "na";
#pragma warning restore 0414

        // Trigger when GameObject is enabled
        private void OnEnable()
        {
            // initialization
            m_Markers = new();
            OTM = new();
            previous_rotation = new();
            GetMarkerGroundTruth();

            // please be know that this function only works with NewARScene case
            // check the called script (GetComponent) on each manager
            ImportObjectsNewARScene();
            StartCoroutine(WhereAttachedToNewARScene());
        }

        IEnumerator WhereAttachedToNewARScene()
        {
            while (true)
            {
                yield return new WaitForSeconds(m_UpdateInterval);
                Main();
            }
        }

        void Main()
        {
            // check if image tracking has detected a marker
            bool marker_has_update = m_ImageTrackingManager
                .GetComponent<NewARSceneImageTrackingCorrection>()
                .GetImageTargetUpdateStatus();
            if (!marker_has_update) { only_once_rotation = false; return; }

            //// do single rotation correction
            //if (!only_once_rotation)
            //{

            // get marker on runtime
            var markers_runtime = m_ImageTrackingManager
                .GetComponent<NewARSceneImageTrackingCorrection>()
                .GetImageTrackedList();
            if (markers_runtime.Count <= 0) { return; }

            AddOrUpdateMarkerRuntime(markers_runtime);

            // calculate marker error vector
            List<Vector3> MED = StaticFunctions.MarkerErrorDifference(m_Markers);

            // use ObjectToMarker function from WeightFunction to get weights
            List<CustomTransform> MCT = StaticFunctions.ExtractToCustomTransform(m_Markers);
            OTM.SetMarkers(MCT);
            OTM.SetObjects(m_Objects);
            var weights = OTM.GetAllWeights(MathFunctions.SIGMOID, true, true, m_ScalarWeight);

            //GlobalDebugging.DebugLogListFloatArray(Test_OnlyAxisObjectGet.AxisForDec192022(weights),
            //    "Marker to Object weights on Version 1");

            // calculate new object location with weight
            var new_vector = StaticFunctions.CorrectedVector(m_InitObjectsLocations, weights, MED);
            for (int i = 0; i < m_Objects.Count; i++)
            {
                m_Objects[i].transform.position = new_vector[i];
            }

            // calculate weight based on camera-to-marker
            List<float> cam_to_mark_ws = new();
            for (int i = 0; i < m_Markers.Count; i++)
            {
                var d = Vector3.Distance(m_ARCamera.transform.position, m_Markers[i].GT_Position);
                var w = MathFunctions.Sigmoid(d, true, m_ScalarWeight);
                cam_to_mark_ws.Add(w);
            }
            cam_to_mark_ws = MathFunctions.NormalizedMany(cam_to_mark_ws);

            // calculate average weighted rotation error
            Quaternion result = Quaternion.identity;   
            for (int i = 0; i < m_Markers.Count; i++)
            {
                // get error rotation
                var error_q = Quaternion.Inverse(m_Markers[i].GT_Rotation) * m_Markers[i].C_Rotation;
                result = Quaternion.Slerp(result, error_q, cam_to_mark_ws[i]);
            }

            // get nearest marker
            var marker_name = m_ImageTrackingManager
                .GetComponent<NewARSceneImageTrackingCorrection>()
                .GetNowMarkerTracked();
            if (StaticFunctions.SearchMarkerLocationOnListByName(m_Markers, marker_name, out MarkerLocation f_marker))
            {
                // create dummy object on the marker position
                GameObject dummy_o = new();
                dummy_o.transform.position = f_marker.C_Position;
                dummy_o.transform.rotation = previous_rotation;

                // put the root under the dummy_o
                GlobalConfig.PlaySpaceOriginGO.transform.parent = dummy_o.transform;

                // rotate based on error
                dummy_o.transform.rotation = result;

                // release the root and delete dummy_o
                GlobalConfig.PlaySpaceOriginGO.transform.parent = null;
                Destroy(dummy_o);

                // if rotating is necessary based on angle threshold (not locked by gimbal)
                if (CheckAngleRotation(result, previous_rotation, ANGLE_THRESHOLD))
                    ImportObjectsNewARScene(true);

                // save the error_q into previous_rotation
                previous_rotation = result;
            }

            //    only_once_rotation = true;
            //}

            if (!only_once_rotation)
            {
                Debug.Log("Rotation: " + GlobalDebugging.LoggingQuat(previous_rotation));
                only_once_rotation = true;
            }
        }

        void AddOrUpdateMarkerRuntime(List<CustomTransform> markers)
        {
            // initialization
            var temp_obj = new GameObject();
            var origin = GlobalConfig.PlaySpaceOriginGO;
            temp_obj.transform.SetParent(origin.transform);

            // add new marker into list
            if (m_Markers.Count < markers.Count)
            {
                for (int i = m_Markers.Count; i < markers.Count; i++)
                {
                    MarkerLocation temp_ml = new();

                    foreach (var m_gt in m_MarkersGroundTruth)
                    {
                        if (Equals(markers[i].custom_name, m_gt.name))
                        {
                            temp_ml.Marker_name = markers[i].custom_name;

                            var gt_m44 = GlobalConfig.GetM44ByGameObjRef(m_gt, origin);
                            temp_obj.transform.localPosition = GlobalConfig.GetPositionFromM44(gt_m44);
                            //temp_obj.transform.localEulerAngles = GlobalConfig.GetEulerAngleFromM44(gt_m44);
                            temp_obj.transform.localRotation = GlobalConfig.GetRotationFromM44(gt_m44);

                            temp_ml.GT_Position = ExtractVector3(temp_obj.transform.position);
                            temp_ml.GT_EulerAngle = ExtractVector3(temp_obj.transform.eulerAngles);
                            temp_ml.GT_Rotation = GlobalConfig.ExtractQuaternion(temp_obj.transform.rotation);

                            //temp_obj.transform.position = markers[i].custom_position;
                            //temp_obj.transform.eulerAngles = markers[i].custom_euler_rotation;

                            temp_ml.C_Position = ExtractVector3(markers[i].custom_position);
                            temp_ml.C_EulerAngle = ExtractVector3(markers[i].custom_euler_rotation);
                            temp_ml.C_Rotation = GlobalConfig.ExtractQuaternion(markers[i].custom_q_rotation);

                            m_Markers.Add(temp_ml);
                            break;
                        }
                    }
                }
            }

            // update current marker in the list
            for (int i = 0; i < markers.Count; i++)
            {
                //temp_obj.transform.position = markers[i].custom_position;
                //temp_obj.transform.eulerAngles = markers[i].custom_euler_rotation;

                for (int j = 0; j < m_Markers.Count; j++)
                {
                    if (Equals(markers[i].custom_name, m_Markers[j].Marker_name))
                    {
                        m_Markers[j].C_Position = ExtractVector3(markers[i].custom_position);
                        m_Markers[j].C_EulerAngle = ExtractVector3(markers[i].custom_euler_rotation);
                        m_Markers[j].C_Rotation = GlobalConfig.ExtractQuaternion(markers[i].custom_q_rotation);

                        break;
                    }
                }
            }

            //var s = "";
            //foreach (var m in m_Markers)
            //{
            //    s += m.Marker_name + "\n";
            //    s += m.GT_Position + "\n";
            //    s += m.GT_EulerAngle + "\n";
            //    s += m.C_Position + "\n";
            //    s += m.C_EulerAngle + "\n";
            //    s += "\n";
            //}
            //Debug.Log(s);

            Destroy(temp_obj);
        }

        void ImportObjectsNewARScene(bool update = false)
        {
            if (update)
            {
                m_InitObjectsLocations.Clear();
            }

            m_Objects = m_LoadObjectManager
                .GetComponent<LoadObject_CatExample_2__NewARScene>()
                .GetMyObjects();

            m_InitObjectsLocations = new();
            foreach (var o in m_Objects)
            {
                var m44 = GlobalConfig.GetM44ByGameObjRef(o, GlobalConfig.PlaySpaceOriginGO);
                //m_InitObjectsLocations.Add(ExtractVector3(m44.GetPosition()));
                m_InitObjectsLocations.Add(ExtractVector3(o.transform.position));
            }
        }

        bool CheckAngleRotation(Quaternion previous, Quaternion current, float angle_threshold = 5.0f)
        {
            var q_diff = Quaternion.Inverse(previous) * current;
            float angle_in_rad = Mathf.Acos(q_diff.w);
            float angle_in_degree = angle_in_rad * Mathf.Rad2Deg * 2;

            //Debug.Log(q_diff + " " + angle_in_rad + " " + angle_in_degree);

            if (angle_in_degree > angle_threshold) return true;
            return false;
        }

        void ExtractVector3(Vector3 v, out float x, out float y, out float z)
        {
            x = v.x;
            y = v.x;
            z = v.z;
        }

        Vector3 ExtractVector3(Vector3 v)
        {
            return new(v.x, v.y, v.z);
        }

        /// TODO:
        /// - Implement this in every version of GetMarkerGroundTruth function
        /// - Pass the data (Tr, RtE, RtQ) to GlobalSaveData
        void GetMarkerGroundTruth()
        {
            m_MarkersGroundTruth = new();

            var parents = m_LoadObjectManager
                .GetComponent<LoadObject_CatExample_2__NewARScene>()
                .GetMyParents();
            foreach (var item in parents)
            {
                string[] names = item.name.Split("_");
                if (names[0] == "img")
                {
                    GameObject temp = new();
                    temp.name = item.name;
                    temp.transform.SetPositionAndRotation(item.transform.position, item.transform.rotation);
                    m_MarkersGroundTruth.Add(temp);

                    // pass data to GlobalSaveData
                    SavingIntoGlobalSaveData(temp);
                }
            }
        }

        void SavingIntoGlobalSaveData(GameObject item)
        {
            string[] data =
        {
                GlobalConfig.GetNowDateandTime(true),
                "gtmarker_ver1bavg_" + item.name,
                item.transform.position.x.ToString(),
                item.transform.position.y.ToString(),
                item.transform.position.z.ToString(),
                item.transform.eulerAngles.x.ToString(),
                item.transform.eulerAngles.y.ToString(),
                item.transform.eulerAngles.z.ToString(),
                item.transform.rotation.x.ToString(),
                item.transform.rotation.y.ToString(),
                item.transform.rotation.z.ToString(),
                item.transform.rotation.z.ToString(),
            };

            GlobalSaveData.WriteData(data);
        }
    }
}
