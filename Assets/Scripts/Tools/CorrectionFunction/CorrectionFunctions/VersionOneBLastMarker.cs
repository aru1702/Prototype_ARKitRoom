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
    /// error rotation in Quaternion with data from the last detected marker.
    /// </summary>
    public class VersionOneBLastMarker : MonoBehaviour
    {
        List<MarkerLocation> m_Markers = new();
        List<GameObject> m_Objects;
        List<GameObject> m_MarkersGroundTruth;
        List<Vector3> m_InitObjectsLocations;
        ObjectToMarkers OTM;

        List<GameObject> m_InitObjectsLocationsHelper = new();
        List<GameObject> m_MarkersGroundTruthsHelper = new();
        GameObject center_helper;
        GameObject root_helper;

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
        [Tooltip("Scalar multiplier for object-to-marker weight function.")]
        float m_ScalarObjectWeight = 1.0f;

        [SerializeField]
        [Tooltip("Threshold of rotation angle to update initial data.")]
        float m_AngleThreshold = 0.1f;

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
            OTM = new();
            previous_rotation = new();

            center_helper = new("center_helper");
            root_helper = new("root_helper");
            root_helper.transform.SetPositionAndRotation(
                GlobalConfig.PlaySpaceOriginGO.transform.position,
                GlobalConfig.PlaySpaceOriginGO.transform.rotation);

            GetMarkerGroundTruth();

            if (GlobalConfig.OTM_SCALAR != 0.0f) m_ScalarObjectWeight = GlobalConfig.OTM_SCALAR;
            if (GlobalConfig.RA_ANGLE != 0.0f) m_AngleThreshold = GlobalConfig.RA_ANGLE;

            // please be know that this function only works with NewARScene case
            // check the called script (GetComponent) on each manager
            ImportObjectsNewARScene(false, true);
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


        // The order of compensation:
        // - Translation
        // - Rotation
        // - Update ground truth
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

            // reset root first
            GlobalConfig.PlaySpaceOriginGO.transform.SetPositionAndRotation(
                root_helper.transform.position, root_helper.transform.rotation);

            AddOrUpdateMarkerRuntime(markers_runtime);

            // calculate marker error vector
            List<Vector3> MED = StaticFunctions.MarkerErrorDifference(m_Markers);

            // use ObjectToMarker function from WeightFunction to get weights
            List<CustomTransform> MCT = StaticFunctions.ExtractToCustomTransform(m_Markers);
            OTM.SetMarkers(MCT);
            OTM.SetObjects(m_Objects);
            var weights = OTM.GetAllWeights(MathFunctions.SIGMOID, true, true, m_ScalarObjectWeight);

            //GlobalDebugging.DebugLogListFloatArray(Test_OnlyAxisObjectGet.AxisForDec192022(weights),
            //    "Marker to Object weights on Version 1");

            // calculate new object location with weight
            var new_vector = StaticFunctions.CorrectedVector(m_InitObjectsLocations, weights, MED);
            for (int i = 0; i < m_Objects.Count; i++)
            {
                m_Objects[i].transform.position = new_vector[i];
            }

            // apply root rotation
            var marker_name = m_ImageTrackingManager
                .GetComponent<NewARSceneImageTrackingCorrection>()
                .GetNowMarkerTracked(); // get current detected marker name
            if (StaticFunctions.SearchMarkerLocationOnListByName(m_Markers, marker_name, out MarkerLocation f_marker))
            {
                // check if detect different marker
                //if (previous_marker != marker_name)
                //    previous_rotation = f_marker.GT_Rotation;

                // get error rotation
                var error_q = Quaternion.Inverse(f_marker.GT_Rotation) * f_marker.C_Rotation;

                //// if rotating is necessary based on angle threshold (not locked by gimbal)
                //if (CheckAngleRotation(error_q, previous_rotation, m_AngleThreshold))
                //{
                    // create dummy object on the marker position
                    GameObject dummy_o = new();
                    dummy_o.transform.position = f_marker.C_Position;
                    //dummy_o.transform.rotation = previous_rotation;

                    // put the root under the dummy_o
                    GlobalConfig.PlaySpaceOriginGO.transform.parent = dummy_o.transform;
                    center_helper.transform.parent = dummy_o.transform;

                    // rotate based on error
                    dummy_o.transform.rotation = error_q;

                    // release the root and delete dummy_o
                    GlobalConfig.PlaySpaceOriginGO.transform.parent = null;
                    center_helper.transform.parent = null;

                    //ImportObjectsNewARScene(true);
                    //UpdateGroundTruthAfterRotation();

                    // save the error_q into previous_rotation
                    //previous_rotation = error_q;

                    Destroy(dummy_o);
                //}
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

            //Debug.Log("m_Markers: " + m_Markers.Count);
            //Debug.Log("markers: " + markers.Count);

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

                            temp_ml.GT_Position = GlobalConfig.ExtractVector3(temp_obj.transform.position);
                            temp_ml.GT_EulerAngle = GlobalConfig.ExtractVector3(temp_obj.transform.eulerAngles);
                            temp_ml.GT_Rotation = GlobalConfig.ExtractQuaternion(temp_obj.transform.rotation);

                            //temp_obj.transform.position = markers[i].custom_position;
                            //temp_obj.transform.eulerAngles = markers[i].custom_euler_rotation;

                            temp_ml.C_Position = GlobalConfig.ExtractVector3(markers[i].custom_position);
                            temp_ml.C_EulerAngle = GlobalConfig.ExtractVector3(markers[i].custom_euler_rotation);
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
                        m_Markers[j].C_Position = GlobalConfig.ExtractVector3(markers[i].custom_position);
                        m_Markers[j].C_EulerAngle = GlobalConfig.ExtractVector3(markers[i].custom_euler_rotation);
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

        /// <summary>
        /// To import all game object before alteration as ground truth position.
        /// It can also update the ground truth if the world coordinate system is alternated.
        /// </summary>
        /// <param name="update">Should the ground truth list need to be cleared.</param>
        /// <param name="create_new_object">Create new game object for all ground truth as helper.
        ///                                 Saved in m_InitObjectsLocationsHelper list.</param>
        void ImportObjectsNewARScene(bool update = false, bool create_new_object = false)
        {
            if (update) { m_InitObjectsLocations.Clear(); }

            m_Objects = m_LoadObjectManager
                .GetComponent<LoadObject_CatExample_2__NewARScene>()
                .GetMyObjects();

            center_helper.transform.position = GlobalConfig.PlaySpaceOriginGO.transform.position;

            m_InitObjectsLocations = new();
            foreach (var o in m_Objects)
            {
                m_InitObjectsLocations.Add(GlobalConfig.ExtractVector3(o.transform.position));

                // now this function also create new object to custom
                if (create_new_object)
                {
                    GameObject gameObject = new();
                    gameObject.name = "cno_gameObject_" + o.name;
                    gameObject.transform.position = GlobalConfig.ExtractVector3(o.transform.position);
                    gameObject.transform.rotation = GlobalConfig.ExtractQuaternion(o.transform.rotation);
                    gameObject.transform.SetParent(center_helper.transform);
                    m_InitObjectsLocationsHelper.Add(gameObject);
                }
            }
        }

        /// <summary>
        /// To update ground truth position after world coordinate system rotation (alternated):
        /// 1. Clear the current ground truth data,
        /// 2. Insert with the new one.
        /// </summary>
        void UpdateGroundTruthAfterRotation()
        {
            // update object GT
            m_InitObjectsLocations.Clear();
            foreach (var iolh in m_InitObjectsLocationsHelper)
            {
                m_InitObjectsLocations.Add(GlobalConfig.ExtractVector3(iolh.transform.position));
            }


            // update marker GT
            var marker_name = m_ImageTrackingManager
                .GetComponent<NewARSceneImageTrackingCorrection>()
                .GetNowMarkerTracked();
            GameObject temp_obj = new();
            temp_obj.transform.SetParent(GlobalConfig.PlaySpaceOriginGO.transform);
            for (int i = 0; i < m_Markers.Count; i++)
            {
                for (int j = 0; j < m_MarkersGroundTruth.Count; j++)
                {
                    if (m_Markers[i].Marker_name == m_MarkersGroundTruth[j].name
                        && m_Markers[i].Marker_name != marker_name)
                    {
                        var gt_m44 = GlobalConfig.GetM44ByGameObjRef(m_MarkersGroundTruthsHelper[j], GlobalConfig.PlaySpaceOriginGO);
                        temp_obj.transform.localPosition = GlobalConfig.GetPositionFromM44(gt_m44);
                        temp_obj.transform.localRotation = GlobalConfig.GetRotationFromM44(gt_m44);
                        //m_Markers[i].GT_Position = GlobalConfig.ExtractVector3(temp_obj.transform.position);
                        //m_Markers[i].GT_EulerAngle = GlobalConfig.ExtractVector3(temp_obj.transform.eulerAngles);
                        //m_Markers[i].GT_Rotation = GlobalConfig.ExtractQuaternion(temp_obj.transform.rotation);

                        break;  // for the m_MarkersGroundTruth for loop
                    }
                }
            }
            Destroy(temp_obj);
        }

        bool CheckAngleRotation(Quaternion previous, Quaternion current, float m_AngleThreshold = 5.0f)
        {
            var q_diff = Quaternion.Inverse(previous) * current;
            float angle_in_rad = Mathf.Acos(q_diff.w);
            float angle_in_degree = angle_in_rad * Mathf.Rad2Deg * 2;

            //Debug.Log(q_diff + " " + angle_in_rad + " " + angle_in_degree);

            if (angle_in_degree == float.NaN) return true;
            if (angle_in_degree > m_AngleThreshold) return true;
            return false;
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

                    // put to center_helper
                    var dupe = Instantiate(temp);
                    dupe.transform.SetParent(center_helper.transform);
                    dupe.name = item.name;
                    m_MarkersGroundTruthsHelper.Add(dupe);

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
                "gtmarker_ver1blst_" + item.name,
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

        public List<MarkerLocation> GetMarkerLocations()
        {
            return m_Markers;
        }
    }
}
