using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeightFunction;

/// <summary>
/// Flows:
/// - get marker data
/// - compare marker data, both position and rotation, get differences
/// - rotate the world based on rotation difference
///    - create a gameobject on the marker place
///    - put the root of world as child of this gameobject
///    - rotate the gameobject
///    - remove root of world from child of this gameobject
///    - destroy gameobject
/// - move of each object based on correction function
/// </summary>

namespace CorrectionFunctions
{

    /// <summary>
    /// All of the Marker-To-Object position correction function is inherit from Version One.
    /// We add the rotation correction function here based on Eigen Method.
    /// Rotation correction is a weighted average linear solution.
    /// </summary>
    ///
    public class VersionOneWithRotation : MonoBehaviour
    {
        List<MarkerLocation> m_Markers = new();
        List<GameObject> m_Objects;
        List<GameObject> m_MarkersGroundTruth;
        List<Vector3> m_InitObjectsLocations;


        ObjectToMarkers OTM;
        MarkerRotation MR;
        //MarkerPosition MP;


        [SerializeField]
        [Tooltip("AR Camera")]
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
        [Tooltip("Scalar multiplier for MTO weight function.")]
        float m_MTOScalarWeight = 1.0f;

        [SerializeField]
        [Tooltip("Scalar multiplier for MR weight function.")]
        float m_MRScalarWeight = 1.0f;


        [SerializeField]
        [Tooltip("Experimental MTO MR Start Stop handler")]
        GameObject m_MtoMrHandler;

        [SerializeField]
        [Tooltip("Experimental MTO MR Start Stop -- Does MR function only once?")]
        bool m_MROnlyOnce = true;


        Quaternion static_root_initial_rotation;
        Vector3 static_root_initial_position;
        GameObject static_root_no_transformed;


        bool start_mto = true;      // true if start, false if stop
        bool start_mr = true;       // true if start, false if stop
        bool reset_r = false;       // true: reset Rt, Rr, then automatically to false
        bool reset_all = false;     // true: reset all object back to initial position

        bool enable_camera_update_mode = false;      // true: update WC every camera movement with marker, false: only when see marker


        // Trigger when GameObject is enabled
        private void OnEnable()
        {
            // initialization
            OTM = new ObjectToMarkers();
            MR = new MarkerRotation();
            //MP = new MarkerPosition();

            GetMarkerGroundTruth();
            GetAllGroundTruth();

            static_root_initial_rotation = GlobalConfig.ExtractQuaternion(GlobalConfig.PlaySpaceOriginGO.transform.rotation);
            static_root_initial_position = GlobalConfig.ExtractVector3(GlobalConfig.PlaySpaceOriginGO.transform.position);
            static_root_no_transformed = new GameObject("root_no_transformed");

            if (GlobalConfig.OTM_SCALAR > 0.0f) m_MTOScalarWeight = GlobalConfig.OTM_SCALAR;
            if (GlobalConfig.CTM_SCALAR > 0.0f) m_MRScalarWeight = GlobalConfig.CTM_SCALAR;

            m_MtoMrHandler.SetActive(true);

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
            //if (reset_r)
            //{

            //    GlobalConfig.PlaySpaceOriginGO.transform.SetPositionAndRotation
            //        (static_root_initial_position, static_root_initial_rotation);
            //    reset_r = false;

            //}

            //if (reset_all)
            //{

            //    for (int i = 0; i < m_AllObjectGroundTruth.Count; i++)
            //    {
            //        m_AllObjectGroundTruth[i].transform.position = m_AllObjectGroundTruthPos[i];
            //    }
            //    reset_all = false;

            //}

            // check if image tracking has detected a marker
            bool marker_has_update = m_ImageTrackingManager
                .GetComponent<NewARSceneImageTrackingCorrection>()
                .GetImageTargetUpdateStatus();
            if (!marker_has_update && !enable_camera_update_mode) { return; }
                                                                        // NOW IF THE FUNCTION ALWAYS RUN PER PERIOD

            // get marker on runtime
            List<CustomTransform> markers_runtime = m_ImageTrackingManager
                .GetComponent<NewARSceneImageTrackingCorrection>()
                .GetImageTrackedList();
            if (markers_runtime.Count <= 0) { return; }                 // STILL CHECKING IF MARKER IS ZERO

            // get marker on tracked
            CustomTransform marker_on_tracked = m_ImageTrackingManager
                .GetComponent<NewARSceneImageTrackingCorrection>()
                .GetMarkerOnTracked();                                  // FOR CENTER OF TRANSFORMATION

            // BUT IF NO DETECTED USE CAMERA
            if (enable_camera_update_mode && marker_on_tracked.custom_name == "none")
            {
                CustomTransform ct = new();
                ct.custom_name = m_ARCamera.name;
                ct.custom_position = m_ARCamera.transform.position;
                marker_on_tracked = ct;
            }

            //Debugging("marker in recog", markers_runtime.Count.ToString());
            //Debugging("marker on tracked pos", marker_on_tracked.custom_position.ToString());

            // update the marker in runtime
            AddOrUpdateMarkerRuntime(markers_runtime);

                    Debugging("static pos and rot", static_root_initial_position.ToString() + ", " + static_root_initial_rotation.ToString());

            // reset the static root
            static_root_no_transformed.transform.SetPositionAndRotation(static_root_initial_position, static_root_initial_rotation);

            // calculate difference and weights
            List<Vector3> vector_diffs = new List<Vector3>();
            List<Quaternion> quat_diffs = new List<Quaternion>();
            List<float> weights = new List<float>();

            foreach (var m in m_Markers)
            {
                Vector3 v_diff = m.C_Position - m.GT_Position;
                vector_diffs.Add(v_diff);

                Quaternion q_diff = Quaternion.Inverse(m.GT_Rotation) * m.C_Rotation;
                quat_diffs.Add(q_diff);

                float cam_to_m = Vector3.Distance(m_ARCamera.transform.position, m.C_Position);
                float w = Mathf.Exp(-cam_to_m * m_MRScalarWeight);
                weights.Add(w);
            }
            weights = MathFunctions.NormalizedMany(weights);

                    for (int i = 0; i < vector_diffs.Count; i++)
                    {
                        Debugging("pos rot m diff", m_Markers[i].Marker_name + ": "
                                                    + vector_diffs[i].ToString() + ", "
                                                    + quat_diffs[i].ToString() + ", "
                                                    + weights[i] + ", "
                                                    + marker_on_tracked.custom_name);
                    }

            // center position of transformation
            Vector3 co_Trans = marker_on_tracked.custom_position;

            // create tempM obj and apply parent-child relationship
            GameObject temp_m = new GameObject();
            temp_m.transform.position = co_Trans;
            static_root_no_transformed.transform.parent = temp_m.transform;

            // calculate the weighted average
            Vector3 nwa_pos = Vector3.zero;
            List<EigenMacHelper.QuaternionWeighted> qws = new List<EigenMacHelper.QuaternionWeighted>();
            for (int i = 0; i < vector_diffs.Count; i++)
            {
                nwa_pos += weights[i] * vector_diffs[i];

                EigenMacHelper.QuaternionWeighted qw = new EigenMacHelper.QuaternionWeighted(quat_diffs[i], weights[i]);
                qws.Add(qw);
            }
            Quaternion nwa_rot = EigenMacHelper.EigenWeightedAvgMultiRotations(qws.ToArray());

                    Debugging("nwa result", nwa_pos.ToString() + ", " + nwa_rot.ToString());

            // apply the transformation
            temp_m.transform.position += nwa_pos;
            temp_m.transform.rotation *= nwa_rot;
            static_root_no_transformed.transform.parent = null;
            Destroy(temp_m);

                    Debugging("new pos and rot", static_root_no_transformed.transform.position.ToString() + ", " + static_root_no_transformed.transform.rotation.ToString());

            // apply the new transformation
            GlobalConfig.PlaySpaceOriginGO.transform.SetPositionAndRotation
                (static_root_no_transformed.transform.position,
                 static_root_no_transformed.transform.rotation);

            //if (start_mto)
            //{

            //    // calculate marker error vector
            //    List<Vector3> MED = StaticFunctions.MarkerErrorDifference(m_Markers);

            //    // use ObjectToMarker function from WeightFunction to get weights
            //    List<CustomTransform> MCT = StaticFunctions.ExtractToCustomTransform(m_Markers);
            //    OTM.SetMarkers(MCT);
            //    OTM.SetObjects(m_Objects);
            //    var weights = OTM.GetAllWeights(MathFunctions.SIGMOID, true, true, m_MTOScalarWeight);

            //    // calculate new object location with weight
            //    var new_vector = StaticFunctions.CorrectedVector(m_InitObjectsLocations, weights, MED);
            //    for (int i = 0; i < m_Objects.Count; i++)
            //    {
            //        m_Objects[i].transform.position = new_vector[i];
            //    }

            //}

            //if (start_mr)
            //{

            //    // use MarkerRotation function to rotate the world
            //    MR.SetCamera(m_ARCamera);
            //    MR.SetRoot(GlobalConfig.PlaySpaceOriginGO);
            //    MR.ResetRootRotationToInitial(static_root_initial_rotation);
            //    MR.SetMarkers(m_Markers);
            //    MR.SetCurrentMarker(marker_on_tracked.custom_position);
            //    MR.SetMRWeight(m_MRScalarWeight);
            //    MR.MarkerRotationStart();

            //    if (m_MROnlyOnce) start_mr = false;   // because only once

            //}
        }

        void AddOrUpdateMarkerRuntime(List<CustomTransform> markers)
        {
            // initialization
            var temp_obj = new GameObject();
            var origin = static_root_no_transformed;
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
                            temp_obj.transform.localPosition = gt_m44.GetPosition();
                            temp_obj.transform.localRotation = gt_m44.rotation;

                            temp_ml.GT_Position = GlobalConfig.ExtractVector3(temp_obj.transform.position);
                            temp_ml.GT_EulerAngle = GlobalConfig.ExtractVector3(temp_obj.transform.eulerAngles);
                            temp_ml.GT_Rotation = GlobalConfig.ExtractQuaternion(temp_obj.transform.rotation);

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
            // for each marker in current list
            for (int i = 0; i < markers.Count; i++)
            {
                // and for each marker in GT list
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

            Destroy(temp_obj);
        }

        void ImportObjectsNewARScene()
        {
            m_Objects = m_LoadObjectManager
                .GetComponent<LoadObject_CatExample_2__NewARScene>()
                .GetMyObjects();

            m_InitObjectsLocations = new();
            foreach (var o in m_Objects)
            {
                m_InitObjectsLocations.Add(GlobalConfig.ExtractVector3(o.transform.position));
            }
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
                    m_MarkersGroundTruth.Add(item);

                    // pass data to GlobalSaveData
                    //SavingIntoGlobalSaveData(item);
                }
            }
        }

        List<Vector3> m_AllObjectGroundTruthPos;
        List<GameObject> m_AllObjectGroundTruth;
        void GetAllGroundTruth()
        {
            m_AllObjectGroundTruthPos = new List<Vector3>();
            m_AllObjectGroundTruth = new List<GameObject>();
            var parents = m_LoadObjectManager
                .GetComponent<LoadObject_CatExample_2__NewARScene>()
                .GetMyParents();
            foreach (var item in parents)
            {
                m_AllObjectGroundTruth.Add(item);
                m_AllObjectGroundTruthPos.Add(GlobalConfig.ExtractVector3(item.transform.position));
            }
        }

        // at this moment we are not using this function

        //void SavingIntoGlobalSaveData(GameObject item)
        //{
        //    string[] data =
        //{
        //        GlobalConfig.GetNowDateandTime(true),
        //        "gtmarker_" + item.name,
        //        item.transform.position.x.ToString(),
        //        item.transform.position.y.ToString(),
        //        item.transform.position.z.ToString(),
        //        item.transform.eulerAngles.x.ToString(),
        //        item.transform.eulerAngles.y.ToString(),
        //        item.transform.eulerAngles.z.ToString(),
        //        item.transform.rotation.x.ToString(),
        //        item.transform.rotation.y.ToString(),
        //        item.transform.rotation.z.ToString(),
        //        item.transform.rotation.z.ToString(),
        //    };

        //    GlobalSaveData.WriteData(data);
        //}

        public List<MarkerLocation> GetMarkerLocations()
        {
            return m_Markers;
        }

        void Debugging(string context, string data) { Debug.Log(GlobalConfig.GetNowDateandTime(true, false, true) + " -- " + context + ": " + data); }
        

        public void SetStartMTO(bool t) { start_mto = t; Debug.Log("MTO: " + t); }

        public void SetStartMR(bool t) { start_mr = t; Debug.Log("MR: " + t); }

        public void ResetR() { reset_r = true; Debug.Log("Reset R"); }

        public void ResetAll() { reset_all = true; Debug.Log("Reset all"); }

        public void EnableCameraUpdateMode(bool t) { enable_camera_update_mode = t; Debug.Log("Enable camera: " + t); }
    }
}
