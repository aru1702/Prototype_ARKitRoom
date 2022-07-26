using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class RecordPosition_CameraEveryFrame : MonoBehaviour
{
    [SerializeField]
    GameObject m_ARCamera;

    [SerializeField]
    InputField m_Laps;

    [SerializeField]
    float m_DeviceWidth, m_DeviceHeight;

    [SerializeField]
    float m_CreateTrailPerSecond = 1.0f;

    List<GameObject> SLAM_Trails = new();

    List<string[]> recordedCamera_Pos = new();
    bool cameraPos_hasHeader = false;

    void Start()
    {
        m_Laps.text = "1";
        StartCoroutine(TickPerPeriod());
    }

    IEnumerator TickPerPeriod()
    {
        while(true)
        {
            yield return new WaitForSeconds(m_CreateTrailPerSecond);
            CameraPos_Record();
            CreateNewTrails();
        }
    }

    void CameraPos_Record()
    {
        Vector3 pos = m_ARCamera.transform.position;
        Quaternion rot = m_ARCamera.transform.rotation;

        if (!cameraPos_hasHeader) CameraPos_AddHeader();

        float newX = pos.x;
        float newY = pos.y;

        if (m_DeviceWidth > 0) newX = pos.x + m_DeviceWidth / 2;
        if (m_DeviceHeight > 0) newY = pos.y + m_DeviceHeight / 2;

        string[] data = new[]
        {
            GlobalConfig.GetNowDateandTime(),
            newX.ToString(),
            newY.ToString(),
            pos.z.ToString(),
            rot.x.ToString(),
            rot.y.ToString(),
            rot.z.ToString(),
            rot.w.ToString(),
            m_Laps.text
        };
        recordedCamera_Pos.Add(data);
    }

    public void CameraPos_Save()
    {
        string time = GlobalConfig.GetNowDateandTime();
        string map = GlobalConfig.MapsSelection.ToString();
        string fileName = time + "_recordedCamera_PerFrameForSLAM__Maps_" + map + ".csv";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, recordedCamera_Pos);
    }

    void CameraPos_AddHeader()
    {
        string[] header = new[] {
            "timestamp",
            "pos x", "pos y", "pos z",
            "rot quat x", "rot quat y", "rot quat z", "rot quat w",
            "laps"
        };
        recordedCamera_Pos.Add(header);
        cameraPos_hasHeader = true;
    }

    void CreateNewTrails()
    {
        GameObject newGO = new();
        newGO.name = GlobalConfig.GetNowDateandTime().ToString();
        newGO.transform.SetPositionAndRotation(
            m_ARCamera.transform.position,
            m_ARCamera.transform.rotation);
        SLAM_Trails.Add(newGO);
    }

    public void Trails_Save()
    {
        string[] header = new[] {
                "timestamp",
                "pos x", "pos y", "pos z",
                "rot quat x", "rot quat y", "rot quat z", "rot quat w",
                "laps"
            };

        List<string[]> recordedSLAMAdjusted_Pos = new();
        recordedSLAMAdjusted_Pos.Add(header);

        foreach (var trail in SLAM_Trails)
        {
            Vector3 pos = trail.transform.position;
            Quaternion rot = trail.transform.rotation;

            float newX = pos.x;
            float newY = pos.y;

            if (m_DeviceWidth > 0) newX = pos.x + m_DeviceWidth / 2;
            if (m_DeviceHeight > 0) newY = pos.y + m_DeviceHeight / 2;

            string[] data = new[]
            {
                trail.name,
                newX.ToString(),
                newY.ToString(),
                pos.z.ToString(),
                rot.x.ToString(),
                rot.y.ToString(),
                rot.z.ToString(),
                rot.w.ToString(),
                m_Laps.text
            };
            recordedSLAMAdjusted_Pos.Add(data);
        }

        string time = GlobalConfig.GetNowDateandTime();
        string map = GlobalConfig.MapsSelection.ToString();
        string fileName = time + "_recordedSLAMAdjusted_Pos__Maps_" + map + ".csv";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, recordedSLAMAdjusted_Pos);
    }
}
