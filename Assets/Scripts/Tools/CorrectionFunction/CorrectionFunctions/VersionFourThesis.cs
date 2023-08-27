using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionFourThesis : MonoBehaviour
{
    List<MarkerLocation> m_Markers;
    List<GameObject> m_Objects;
    List<GameObject> m_MarkersInWorld;

    List<Vector3> m_InitObjectsLocations;
    List<Quaternion> m_InitObjectsRotation;

    [SerializeField]
    [Tooltip("To import object location.")]
    GameObject m_LoadObjectManager;


    int choose;


    // Testing with dummy map
    [SerializeField]
    int MAP = -1;


    // Trigger when GameObject is enabled
    private void OnEnable()
    {
        m_Markers = new();
        m_Objects = new();
        m_MarkersInWorld = new();

        if (MAP >= 0) GlobalConfig.LOAD_MAP = MAP;

        // initialization
        ImportObjectsNewARScene();
        ImportMarkerData();

        Main();
    }

    void Main()
    {
        // rotate the world to slam
        Quaternion[] Rotation = new Quaternion[m_Markers.Count];
        for (int i = 0; i < m_Markers.Count; i++)
        {
            Quaternion rot = m_Markers[i].C_Rotation * Quaternion.Inverse(m_Markers[i].GT_Rotation);
            Rotation[i] = rot;
        }
        Quaternion Rot_avg = EigenMacHelper.EigenAvgRotations(Rotation);
        GlobalConfig.PlaySpaceOriginGO.transform.rotation *= Rot_avg;

        // pick methods
        int corr_num = GlobalConfig.CorrectionFunctionVersion;
        if (corr_num == 5) SingleMarker();
        if (corr_num == 6) MeanAverage();
        if (corr_num == 7) NearestMarker();
        if (corr_num == 8) DistanceBasedWeightedAverage();
    }

    void SingleMarker()
    {
        // choose randomly one marker
        System.Random rnd = new System.Random();
        int chosen = rnd.Next(m_Markers.Count);

        // put the weights
        List<float[]> weightes = new List<float[]>();
        for (int i = 0; i < m_Objects.Count; i++)
        {
            float[] ws = new float[m_Markers.Count];
            for (int j = 0; j < m_Markers.Count; j++)
            {
                if (j == chosen) ws[j] = 1;
                else ws[j] = 0;
            }
            weightes.Add(ws);
        }

        // transformation
        for (int i = 0; i < m_Objects.Count; i++)
        {
            Vector3 t_result = new Vector3();
            for (int j = 0; j < m_Markers.Count; j++)
            {
                Vector3 changes = m_Markers[j].C_Position - m_MarkersInWorld[j].transform.position;
                changes *= weightes[i][j];
                t_result += changes;
            }

            m_Objects[i].transform.position += t_result;
        }
    }

    void MeanAverage()
    {

    }

    void NearestMarker()
    {

    }

    void DistanceBasedWeightedAverage()
    {
        // put the weights
        List<float[]> weightes = new List<float[]>();
        for (int i = 0; i < m_Objects.Count; i++)
        {
            // find distance and avg
            float avg = 0;
            float[] dists = new float[m_Markers.Count];
            for (int j = 0; j < m_Markers.Count; j++)
            {
                float dist = Vector3.Distance(m_Objects[i].transform.position, m_MarkersInWorld[j].transform.position);
                dists[j] = dist;
                avg += dist;
            }
            avg /= m_Markers.Count;

            // find weights
            float sum = 0;
            float[] ws = new float[m_Markers.Count];
            for (int j = 0; j < m_Markers.Count; j++)
            {
                float weight = Mathf.Exp(-dists[j] * avg);
                ws[j] = weight;
                sum += weight;
            }

            // normalization
            float[] final_ws = new float[m_Markers.Count];
            for (int j = 0; j < m_Markers.Count; j++)
            {
                final_ws[j] = ws[j] / sum;
            }

            weightes.Add(final_ws);
        }

        // transformation
        for (int i = 0; i < m_Objects.Count; i++)
        {
            Vector3 t_result = new Vector3();
            for (int j = 0; j < m_Markers.Count; j++)
            {
                Vector3 changes = m_Markers[j].C_Position - m_MarkersInWorld[j].transform.position;
                changes *= weightes[i][j];
                t_result += changes;
            }

            //Debug.Log(t_result.ToString());
            if (t_result.x != float.NaN) m_Objects[i].transform.position += t_result;
        }
    }


    void ImportObjectsNewARScene()
    {
        m_Objects = m_LoadObjectManager
            .GetComponent<LoadObject_CatExample_2__NewARScene>()
            .GetMyObjects();

        m_InitObjectsLocations = new();
        m_InitObjectsRotation = new();

        foreach (var o in m_Objects)
        {
            m_InitObjectsLocations.Add(GlobalConfig.ExtractVector3(o.transform.position));
            m_InitObjectsRotation.Add(GlobalConfig.ExtractQuaternion(o.transform.rotation));
        }
    }

    void ImportMarkerData()
    {
        int map = GlobalConfig.LOAD_MAP;
        string filename = "MarkerCalibration_New__Maps_" + map + ".csv";
        string path = System.IO.Path.Combine(Application.persistentDataPath, filename);
        List<string[]> result = ImportCSV.getDataOutsource(path, true);

        foreach (var item in result)
        {
            string name = item[0];

            Vector3 gt_pos = new Vector3(float.Parse(item[1]),
                                         float.Parse(item[2]),
                                         float.Parse(item[3]));
            Quaternion gt_rot = new Quaternion(float.Parse(item[4]),
                                               float.Parse(item[5]),
                                               float.Parse(item[6]),
                                               float.Parse(item[7]));

            Vector3 rt_pos = new Vector3(float.Parse(item[8]),
                                         float.Parse(item[9]),
                                         float.Parse(item[10]));
            Quaternion rt_rot = new Quaternion(float.Parse(item[11]),
                                               float.Parse(item[12]),
                                               float.Parse(item[13]),
                                               float.Parse(item[14]));

            MarkerLocation m = new MarkerLocation
            {
                Marker_name = name,
                GT_Position = gt_pos,
                GT_Rotation = gt_rot,
                C_Position = rt_pos,
                C_Rotation = rt_rot
            };
            m_Markers.Add(m);

            GameObject marker = new GameObject();
            marker.name = name;
            marker.transform.SetParent(GlobalConfig.PlaySpaceOriginGO.transform);
            marker.transform.localPosition = gt_pos;
            marker.transform.localRotation = gt_rot;
            m_MarkersInWorld.Add(marker);
        }
    }
}
