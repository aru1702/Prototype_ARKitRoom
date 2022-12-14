using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeightFunction;

namespace CorrectionFunctions
{
    /// <summary>
    /// This version ONE will update the marker in NewARScene.
    /// Requirements: 1) Marker ground truth in runtime, 2) Objects
    /// Enable this function by attach this script to GameObject (enable/disable as trigger)/// 
    /// </summary>
    public class VersionOne : MonoBehaviour
    {
        List<MarkerLocation> m_Markers;
        List<GameObject> m_Objects;
        List<GameObject> m_MarkersGroundTruth;
        List<Vector3> m_InitObjectsLocations;

        [SerializeField]
        [Tooltip("To import object location.")]
        GameObject m_LoadObjectManager;

        [SerializeField]
        [Tooltip("To import marker location in runtime.")]
        GameObject m_ImageTrackingManager;

        [SerializeField]
        [Tooltip("Update runtime per interval in seconds.")]
        float m_UpdateInterval = 1.0f;


        // Trigger when GameObject is enabled
        private void OnEnable()
        {
            // initialization
            m_Markers = new();
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
            if (!marker_has_update) { return; }

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
            ObjectToMarkers OTM = new();
            OTM.SetMarkers(MCT);
            OTM.SetObjects(m_Objects);
            var weights = OTM.GetAllWeights(MathFunctions.SIGMOID);

            // calculate new object location with weight
            var new_vector = StaticFunctions.CorrectedVector(m_InitObjectsLocations, weights, MED);
            for (int i = 0; i < m_Objects.Count; i++)
            {
                m_Objects[i].transform.position = new_vector[i];
            }
        }

        void AddOrUpdateMarkerRuntime(List<CustomTransform> markers)
        {
            // initialization
            var temp_obj = new GameObject();
            var origin = GlobalConfig.PlaySpaceOriginGO;
            temp_obj.transform.SetParent(origin.transform);

            Debug.Log("m_Markers: " + m_Markers.Count + ", markers: " + markers.Count);
                        
            // add new marker into list
            if (m_Markers.Count < markers.Count)
            {
                Debug.Log("add");
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

                            temp_ml.GT_Position = ExtractVector3(temp_obj.transform.position);
                            temp_ml.GT_EulerAngle = ExtractVector3(temp_obj.transform.eulerAngles);

                            //temp_obj.transform.position = markers[i].custom_position;
                            //temp_obj.transform.eulerAngles = markers[i].custom_euler_rotation;

                            temp_ml.C_Position = ExtractVector3(markers[i].custom_position);
                            temp_ml.C_EulerAngle = ExtractVector3(markers[i].custom_euler_rotation);

                            m_Markers.Add(temp_ml);
                            break;
                        }
                    }
                }
            }

            // update current marker in the list
            Debug.Log("update");
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

                        break;
                    }
                }
            }

            var s = "";
            foreach (var m in m_Markers)
            {
                s += m.Marker_name + "\n";
                s += m.GT_Position + "\n";
                s += m.GT_EulerAngle + "\n";
                s += m.C_Position + "\n";
                s += m.C_EulerAngle + "\n";
                s += "\n";
            }
            Debug.Log(s);

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
                m_InitObjectsLocations.Add(ExtractVector3(o.transform.position));
            }
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
    }
}
