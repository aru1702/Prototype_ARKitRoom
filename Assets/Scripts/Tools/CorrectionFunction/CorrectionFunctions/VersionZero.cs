using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeightFunction;

namespace CorrectionFunctions
{
    /// <summary>
    /// This version should be only triggered once after map is loaded.
    /// Requirements: 1) Marker ground truth, 2) Object ground truth.
    /// Enable this function by attach this script to GameObject (enable/disable as trigger)
    /// </summary>
    public class VersionZero : MonoBehaviour
    {
        List<MarkerLocation> m_Markers;
        List<GameObject> m_Objects;

        [SerializeField]
        [Tooltip("To import object location.")]
        GameObject m_LoadObjectManager;


        // Trigger when GameObject is enabled
        private void OnEnable()
        {
            // initialization
            m_Markers = new();

            // please be know that this function only works with NewARScene case
            // check the called script (GetComponent) on each manager
            WhereAttachedToNewARScene();
        }

        void WhereAttachedToNewARScene()
        {
            // get object data from LoadObjectManager
            m_Objects = m_LoadObjectManager
                .GetComponent<LoadObject_CatExample_2__NewARScene>()
                .GetMyObjects();

            // get saved marker data from local
            string map = GlobalConfig.LOAD_MAP.ToString();
            string fileName = MappingV2.GetMarkerCalibrationFileName(map);
            string path = System.IO.Path.Combine(Application.persistentDataPath, fileName);
            MarkerImportCsv mIC = new();
            var markers = mIC.GetMarkerLocationsSummarized(path);   // old
            ExtractToMarkerLocation(markers);   // new
            TransformToAnotherOrigin(m_Markers, GlobalConfig.PlaySpaceOriginGO);

            // calculate marker error vector
            List<Vector3> MED = StaticFunctions.MarkerErrorDifference(m_Markers);

            // use ObjectToMarker function from WeightFunction to get weights
            List<CustomTransform> MCT = StaticFunctions.ExtractToCustomTransform(m_Markers);
            ObjectToMarkers OTM = new();
            OTM.SetMarkers(MCT);
            OTM.SetObjects(m_Objects);
            var weights = OTM.GetAllWeights(MathFunctions.SIGMOID);

            // convert GameObjects to Vector3s
            var vectors = FromGameObjectsToVector3s(m_Objects);

            // calculate new object location with weight
            var new_vector = StaticFunctions.CorrectedVector(vectors, weights, MED);
            for (int i = 0; i < m_Objects.Count; i++)
            {
                m_Objects[i].transform.position = new_vector[i];
            }
        }      

        void ExtractToMarkerLocation(List<MarkerImportCsv.MarkerLocation> markers)
        {
            foreach (var m in markers)
            {
                m_Markers.Add(new(
                    m.name,
                    m.GT_Position, m.GT_EulerAngle,
                    m.C_Position, m.C_EulerAngle,
                    m.before
                ));
            }
        }

        void TransformToAnotherOrigin(List<MarkerLocation> markers, GameObject new_origin)
        {
            // both marker dan camera data are based on DESIGNATED WORLD origin
            // our AR app works based on SLAM origin, which not matched
            // we have to convert them by putting on DESIGNATED WORLD, then get SLAM position
            var tObj = new GameObject();
            tObj.transform.SetParent(new_origin.transform);

            // transform each markers into new origin
            for (int i = 0; i < markers.Count; i++)
            {
                tObj.transform.localPosition = markers[i].GT_Position;
                tObj.transform.localEulerAngles = markers[i].GT_EulerAngle;
                Vector3 gt_pos = new(tObj.transform.position.x,
                                     tObj.transform.position.y,
                                     tObj.transform.position.z);
                Vector3 gt_eul = new(tObj.transform.eulerAngles.x,
                                     tObj.transform.eulerAngles.y,
                                     tObj.transform.eulerAngles.z);

                tObj.transform.localPosition = markers[i].C_Position;
                tObj.transform.localEulerAngles = markers[i].C_EulerAngle;
                Vector3 cr_pos = new(tObj.transform.position.x,
                                     tObj.transform.position.y,
                                     tObj.transform.position.z);
                Vector3 cr_eul = new(tObj.transform.eulerAngles.x,
                                     tObj.transform.eulerAngles.y,
                                     tObj.transform.eulerAngles.z);

                m_Markers[i] = new(markers[i].Marker_name,
                                   gt_pos, gt_eul,
                                   cr_pos, cr_eul,
                                   markers[i].Marker_before);
            }

            Destroy(tObj);
        }

        List<Vector3> FromGameObjectsToVector3s(List<GameObject> objects)
        {
            List<Vector3> vectors = new();
            foreach (var o in objects)
            {
                vectors.Add(new(
                        o.transform.position.x,
                        o.transform.position.y,
                        o.transform.position.z
                    ));
            }
            return vectors;
        }
    }
}
