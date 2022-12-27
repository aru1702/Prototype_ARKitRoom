using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeightFunction;


namespace CorrectionFunctions
{
    /// <summary>
    /// Version TWO side A will update the marker in NewARScene, and record the camera data.
    /// Camera data will records the time on travel only.
    /// There will be A and B scalar parameter as priority for each weight, with total of 1.00 (100%).
    /// This function needs marker data (GT and C), objects data, and camera data (as mentioned above).
    /// Enable this function by attach this script to GameObject (enable/disable as trigger)
    ///
    /// Additional note:
    /// - See the same file name with bak.
    /// - This file will be cleaned for good use.
    /// </summary>
    public class VersionTwoA : MonoBehaviour
    {
        List<MarkerLocation> m_Markers;
        List<CameraTrajectoryData> m_CameraTrajectoryData;
        List<GameObject> m_Objects;
        List<GameObject> m_MarkersGroundTruth;
        List<Vector3> m_InitObjectsLocations;

        ObjectToMarkers OTM;
        CameraTrajectoryAndTime CTT;


        [SerializeField]
        [Tooltip("AR camera game object.")]
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
        [Range(0, 1)]
        [Tooltip("A is object to marker distance weight priority, 0 to 1 in float")]
        float m_OTMPriority = 1.0f;

        [SerializeField]
        [Tooltip("Scalar multiplier for object to marker distance weight function.")]
        float m_OTMScalarWeight = 1.0f;

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("B is camera data time on travel distance weight priority, 0 to 1 in float")]
        float m_CTTTimePriority = 1.0f;

        [SerializeField]
        [Tooltip("Scalar multiplier for camera data time on travel distance weight function.")]
        float m_CTTScalarWeight = 1.0f;

        [Tooltip("Only active this if not want to use GlobalConfig configuration")]
        [SerializeField]
        bool m_UnityEditorMode = false;
        public void SetUnityEditorModeTrue() { m_UnityEditorMode = true; }
        
        [SerializeField]
        GameObject m_WeightDataScript;


        // Trigger when GameObject is enabled
        private void OnEnable()
        {
            // initialization
            m_Markers = new();
            m_CameraTrajectoryData = new();
            OTM = new();
            CTT = new();
            GetMarkerGroundTruth();

            // get global configuration
            if (!m_UnityEditorMode)
            {
                m_OTMScalarWeight = GlobalConfig.OTM_SCALAR;
                m_OTMPriority = GlobalConfig.OTM_PRIORITY;
                m_CTTScalarWeight = GlobalConfig.CTTtime_SCALAR;
                m_CTTTimePriority = GlobalConfig.CTTtime_PRIORITY;
            }
            else
            {
                GlobalConfig.OTM_SCALAR = m_OTMScalarWeight;
                GlobalConfig.OTM_PRIORITY = m_OTMPriority;
                GlobalConfig.CTTtime_SCALAR = m_CTTScalarWeight;
                GlobalConfig.CTTtime_PRIORITY = m_CTTTimePriority;
            }

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
                UpdateCameraData();
                Main();
            }
        }

        bool alreadyDebug = false;
        int iteration = 0;

        void Main()
        {
            // check if image tracking has detected a marker
            bool marker_has_update = m_ImageTrackingManager
                .GetComponent<NewARSceneImageTrackingCorrection>()
                .GetImageTargetUpdateStatus();
            if (!marker_has_update)
            {
                // if no marker update, set current marker as previous marker
                CTT.SetPrevMarkerName(CTT.GetNowMarkerName());

                alreadyDebug = false;

                return;
            }

            // get all markers on runtime
            var all_markers = m_ImageTrackingManager
                .GetComponent<NewARSceneImageTrackingCorrection>()
                .GetImageTrackedList();
            if (all_markers.Count > 0) { AddOrUpdateMarkerRuntime(all_markers); }

            // get only detected marker on runtime
            var markers_name = m_ImageTrackingManager
                .GetComponent<NewARSceneImageTrackingCorrection>()
                .GetNowAndPrevMarkerTracked();
            if (all_markers.Count > 0) { AddOrUpdateCameraData(all_markers, markers_name); }
                        
            // calculate marker error vector
            List<Vector3> MED = StaticFunctions.MarkerErrorDifference(m_Markers);
            List<CustomTransform> MCT = StaticFunctions.ExtractToCustomTransform(m_Markers);

            // get weights from ObjectToMarker function
            OTM.SetMarkers(MCT);
            OTM.SetObjects(m_Objects);
            var otm_weights = OTM.GetAllWeights(MathFunctions.SIGMOID, true, true, m_OTMScalarWeight);

            // get raw data from CameraTrajectoryAndTime function
            CTT.SetCameraTrajectoryData(m_CameraTrajectoryData);

            // i still left this here since we need for debug and weight save data
            var CTT_raw_data = CTT.GetCameraTrajectoryData();
            List<float> camera_times = new();
            foreach (var d in CTT_raw_data) { camera_times.Add(m_CTTScalarWeight * d.Camera_travel_time); }
            // which already replaced by below function

            var norm_camera_times = CTT.GetCameraTimeRaw(true, m_CTTScalarWeight);
            //var norm_camera_times = CTT.GetCameraTimeRawAsWeight(MathFunctions.SIGMOID, false, true, m_CTTScalarWeight);

            // multiply into weights from ObjectToMarker
            List<float[]> new_weights = new();
            List<float[]> norm_new_weights = new();
            for (int i = 0; i < otm_weights.Count; i++)            
            {
                List<float> new_w = new();
                for (int j = 0; j < otm_weights[i].Length; j++)
                {
                    var result = norm_camera_times[j] * otm_weights[i][j];
                    new_w.Add(result);
                }
                new_weights.Add(new_w.ToArray());

                // normalized after calculation
                var norm = MathFunctions.NormalizedMany(new_w);
                norm_new_weights.Add(norm.ToArray());
            }

            // get new vector with norm_new_weights
            // calculate new object location with weight
            var new_vector = StaticFunctions.CorrectedVector(m_InitObjectsLocations, norm_new_weights, MED);
            for (int i = 0; i < m_Objects.Count; i++)
            {
                m_Objects[i].transform.position = new_vector[i];
            }

            // debugging process
            if (!alreadyDebug)
            {
                iteration++;
                Debug.Log("Iteration number " + iteration);
                GlobalDebugging.DebugLogListFloatArray(Test_OnlyAxisObjectGet.MarkerAxisThenObject1To16ForDec212022(otm_weights), "otm_weights");
                GlobalDebugging.DebugLogListFloat(camera_times, "raw camera times");
                GlobalDebugging.DebugLogListFloat(norm_camera_times, "normalized raw camera times");
                GlobalDebugging.DebugLogListFloatArray(Test_OnlyAxisObjectGet.MarkerAxisThenObject1To16ForDec212022(new_weights), "new_weights");
                GlobalDebugging.DebugLogListFloatArray(Test_OnlyAxisObjectGet.MarkerAxisThenObject1To16ForDec212022(norm_new_weights), "norm_new_weights");

                var SWD = m_WeightDataScript.GetComponent<Test_NewARScene_ImgTracking_SaveWeightData>();
                SWD.AddIterationPart(iteration);
                SWD.AddAllDebugInputListFloatArray(Test_OnlyAxisObjectGet.MarkerAxisThenObject1To16ForDec212022(otm_weights), "otm_weights");
                SWD.AddAllDebugInputListFloat(camera_times, "raw camera times");
                SWD.AddAllDebugInputListFloat(norm_camera_times, "normalized raw camera times");
                SWD.AddAllDebugInputListFloatArray(Test_OnlyAxisObjectGet.MarkerAxisThenObject1To16ForDec212022(new_weights), "new_weights");
                SWD.AddAllDebugInputListFloatArray(Test_OnlyAxisObjectGet.MarkerAxisThenObject1To16ForDec212022(norm_new_weights), "norm_new_weights");

                alreadyDebug = true;
            }
        }

        void UpdateCameraData()
        {
            if (m_ARCamera != null)
            {
                var camera_pos = m_ARCamera.transform.position;
                CTT.SetCameraNow(camera_pos);
                CTT.UpdateCameraDistance(true);
            }

            CTT.AutomaticUpdateTotalTime();
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
                            temp_obj.transform.localEulerAngles = GlobalConfig.GetEulerAngleFromM44(gt_m44);

                            temp_ml.GT_Position = GlobalConfig.ExtractVector3(temp_obj.transform.position);
                            temp_ml.GT_EulerAngle = GlobalConfig.ExtractVector3(temp_obj.transform.eulerAngles);

                            temp_ml.C_Position = GlobalConfig.ExtractVector3(markers[i].custom_position);
                            temp_ml.C_EulerAngle = GlobalConfig.ExtractVector3(markers[i].custom_euler_rotation);

                            m_Markers.Add(temp_ml);
                            break;
                        }
                    }
                }
            }

            // update current marker in the list
            for (int i = 0; i < markers.Count; i++)
            {
                for (int j = 0; j < m_Markers.Count; j++)
                {
                    if (Equals(markers[i].custom_name, m_Markers[j].Marker_name))
                    {
                        m_Markers[j].C_Position = GlobalConfig.ExtractVector3(markers[i].custom_position);
                        m_Markers[j].C_EulerAngle = GlobalConfig.ExtractVector3(markers[i].custom_euler_rotation);

                        break;
                    }
                }
            }

            Destroy(temp_obj);
        }

        void AddOrUpdateCameraData(List<CustomTransform> markers, string[] markers_name)
        {            
            // add new camera trajectory data
            if (m_CameraTrajectoryData.Count < m_Markers.Count)
            {
                var arr = markers.Count - 1;
                CameraTrajectoryData data = new();
                data.Marker_name = markers[arr].custom_name;
                data.Camera_travel_distance = CTT.GetCameraDistance();
                data.Camera_travel_time = CTT.GetTotalTime();
                data.Previous_marker = markers_name[1];

                CTT.SetNowMarkerName(markers[arr].custom_name);
                m_CameraTrajectoryData.Add(data);

                return;
            }

            // update current marker in the list
            foreach (var m in markers)
            {
                if (Equals(m.custom_name, markers_name[0]))
                {
                    foreach (var ctd in m_CameraTrajectoryData)
                    {
                        if (Equals(m.custom_name, ctd.Marker_name))
                        {
                            ctd.Camera_travel_distance = CTT.GetCameraDistance();
                            ctd.Camera_travel_time = CTT.GetTotalTime();
                            ctd.Previous_marker = CTT.GetPrevMarkerName();

                            CTT.SetNowMarkerName(ctd.Marker_name);

                            break;
                        }
                    }
                }
            }
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
                    m_MarkersGroundTruth.Add(item);
            }
        }

        List<float> GetPriority(bool normalized, params float[] priorities)
        {
            List<float> norm_priority = new();

            // add them into List
            for (int i = 0; i < priorities.Length; i++)
            {
                norm_priority.Add(priorities[i]);
            }

            // return normalized
            if (normalized) return MathFunctions.NormalizedMany(norm_priority);
            else return norm_priority;
        }
    }
}
