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

    [SerializeField]
    bool m_enableCameraRecord, m_enableCameraTrailCreate;

    List<GameObject> SLAM_Trails = new();

    List<string[]> recordedCamera_Pos = new();
    bool cameraPos_hasHeader = false;   

    /// <summary>
    /// Laps from 1, and start tracking camera position on world space per period
    /// </summary>
    void Start()
    {
        m_Laps.text = "1";
        StartCoroutine(TickPerPeriod());
    }

    /// <summary>
    /// Save all recorded data:
    /// - recorded camera position per period (s) which already into string
    /// - new gameObject resemble as camera tracks per period (s)
    /// </summary>
    public void SaveData()
    {
        CameraPos_Save();
        Trails_Save();
        // in here no UIManager to show already save
        //  because this function also being called simultanously
        //  with RecordPosition script's function
    }

    /// <summary>
    /// This is the function to record data per period automatically
    /// </summary>
    IEnumerator TickPerPeriod()
    {
        while(true)
        {
            yield return new WaitForSeconds(m_CreateTrailPerSecond);
            if (m_enableCameraRecord) CameraPos_Record();
            if (m_enableCameraTrailCreate) CreateNewTrails();
        }
    }

    /////////////////////////////////////////////////////////
    /// Now we enter the camera position record and save
    /////////////////////////////////////////////////////////

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

    void CameraPos_Save()
    {
        // save nothing if there is no data
        if (recordedCamera_Pos.Count <= 0) return;

        string time = GlobalConfig.GetNowDateandTime();
        string map = GlobalConfig.SAVE_INTO_MAP.ToString();
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

    /////////////////////////////////////////////////////////
    /// Now we enter the camera tracks creation and save
    /////////////////////////////////////////////////////////

    void CreateNewTrails()
    {
        GameObject newGO = new();
        newGO.name = GlobalConfig.GetNowDateandTime().ToString();
        newGO.transform.SetPositionAndRotation(
            m_ARCamera.transform.position,
            m_ARCamera.transform.rotation);
        SLAM_Trails.Add(newGO);
    }

    void Trails_Save()
    {
        // save nothing if there is no data
        if (SLAM_Trails.Count <= 0) return;

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
        string map = GlobalConfig.SAVE_INTO_MAP.ToString();
        string fileName = time + "_recordedSLAMAdjusted_Pos__Maps_" + map + ".csv";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, recordedSLAMAdjusted_Pos);
    }

    /////////////////////////////////////////////////////////
    /// Now this is the export and import data tools
    /////////////////////////////////////////////////////////

    /// <summary>
    /// Get list of camera trails in list of GameObject
    /// </summary>
    /// <returns>Camera trails</returns>
    public List<GameObject> GetCameraTracks()
    {
        return SLAM_Trails;
    }

    public List<CameraTrail> GetCameraTrails()
    {
        List<CameraTrail> cameraTrails = new();
        foreach (var item in SLAM_Trails)
        {
            CameraTrail ct = new(
                item.name,
                item.transform.position,
                item.transform.rotation,
                m_Laps.text);
            cameraTrails.Add(ct);
        }

        return cameraTrails;
    }

    /// <summary>
    /// Delete all recorded data then import from outsource
    /// </summary>
    /// <param name="cameraTrails">List of CameraTrail class</param>
    public void ImportFromOutsource(List<CameraTrail> cameraTrails)
    {
        // reset
        ResetAll();

        // add new one
        foreach (var item in cameraTrails)
        {
            GameObject go = new();
            go.name = item.name;
            go.transform.SetPositionAndRotation(item.position, item.rotation);

            if (m_enableCameraRecord)
            {
                Vector3 pos = go.transform.position;
                Quaternion rot = go.transform.rotation;

                if (!cameraPos_hasHeader) CameraPos_AddHeader();

                float newX = pos.x;
                float newY = pos.y;

                if (m_DeviceWidth > 0) newX = pos.x + m_DeviceWidth / 2;
                if (m_DeviceHeight > 0) newY = pos.y + m_DeviceHeight / 2;

                string[] data = new[]
                {
                    go.name,
                    newX.ToString(),
                    newY.ToString(),
                    pos.z.ToString(),
                    rot.x.ToString(),
                    rot.y.ToString(),
                    rot.z.ToString(),
                    rot.w.ToString(),
                    item.laps
                };
                recordedCamera_Pos.Add(data);
            }

            if (m_enableCameraTrailCreate)
            {
                SLAM_Trails.Add(go);
            }
        }
    }

    /// <summary>
    /// Reset all list
    /// </summary>
    void ResetAll()
    {
        // reset List<string[]>
        recordedCamera_Pos.Clear();

        // reset List<GameObject>
        foreach (var trail in SLAM_Trails)
        {
            Destroy(trail);
        }
        SLAM_Trails.Clear();
    }
}
