using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// This script enable to record and save several things:
/// - 2D position of screen frame from 3D space position for nearest object
/// - raycast position to the nearest object (both position)
/// </summary>
public class RecordPosition_NewARScene : MonoBehaviour
{
    [SerializeField]
    Camera m_ARCamera;

    [SerializeField]
    GameObject m_UIManager;

    [SerializeField]
    Text m_RecordedValue;

    [SerializeField]
    InputField m_Laps;

    [SerializeField]
    GameObject m_LoadObjectManager, m_RaycastManager;

    /// <summary>
    /// Option to enable recording 2D position, raycastPos
    /// </summary>
    [SerializeField]
    bool m_Record2DPos, m_RecordRaycastPos;

    List<string[]> recordedPoints_Pos = new();
    bool points_hasHeader = false;

    List<string[]> raycastObject_Pos = new();
    bool raycasts_hasHeader = false;

    /// <summary>
    /// Laps from 1, and recorded value from 0
    /// </summary>
    void Start()
    {
        m_Laps.text = "1";
        m_RecordedValue.text = "0";
    }

    /// <summary>
    /// Record 2D position and raycast data
    /// </summary>
    public void RecordData()
    {
        if (m_Record2DPos) Points_Record();
        if (m_RecordRaycastPos) Raycasts_Record();
    }

    /// <summary>
    /// Save recorded 2D position and raycast data
    /// </summary>
    public void SaveData()
    {
        try
        {
            Points_Save();
            Raycasts_Save();
            SaveSuccess(true);
        }
        catch (System.Exception ex)
        {
            SaveSuccess(false, ex.ToString());
        }
    }

    void SaveSuccess(bool value, string args = "")
    {
        string text;
        if (value) text = "Recorded position saved successfully!";
        else text = "Failed to save recorded position! Reason: " + args;

        m_UIManager
            .GetComponent<UIManager_CatExample>()
            .MapStatus.text = text;

        m_UIManager
            .GetComponent<UIManager_CatExample>()
            .OpenPanel();
    }

    /////////////////////////////////////////////////////////
    /// Now we enter the 2D object position record and save
    /////////////////////////////////////////////////////////

    void Points_AddHeader()
    {
        string[] header = new[] {           // total: 6 objects
            "timestamp", "name",            // 0, 1
            "pos x", "pos y", "pos z",      // 2, 3, 4
            "laps"                          // 5
        };
        recordedPoints_Pos.Add(header);
        points_hasHeader = true;
    }

    void Points_AddCenterReference()
    {
        int x = m_ARCamera.pixelWidth / 2;
        int y = m_ARCamera.pixelHeight / 2;

        string[] data = new[]
                {
                    GlobalConfig.GetNowDateandTime(),
                    "center reference",
                    x.ToString(),
                    y.ToString(),
                    "0",
                    m_Laps.text
                };
        recordedPoints_Pos.Add(data);
    }

    void Points_Record()
    {
        if (!points_hasHeader)
        {
            Points_AddHeader();
            Points_AddCenterReference();
            points_hasHeader = true;
        }

        if (!m_LoadObjectManager.activeSelf) return;

        List<GameObject> allObjects = m_LoadObjectManager
            .GetComponent<LoadObject_CatExample_2__NewARScene>()
            .GetMyObjects();

        if (allObjects.Count <= 0) return;

        float nearestZ = 1000.0f;
        GameObject nearestGameObject = new();

        foreach (var obj in allObjects)
        {
            string[] strSplt = obj.name.Split("_");
            if (strSplt[0] == "sample")
            {
                Vector3 screenPos = m_ARCamera.WorldToScreenPoint(obj.transform.position);
                float z = System.Math.Abs(screenPos.z);
                if (z < nearestZ)
                {
                    nearestGameObject = obj;
                    nearestZ = z;
                }
            }
        }

        Vector3 objInScreen_Pos = m_ARCamera.WorldToScreenPoint(nearestGameObject.transform.position);
        string[] data = new[]
                {
                    GlobalConfig.GetNowDateandTime(),
                    nearestGameObject.name,
                    objInScreen_Pos.x.ToString(),
                    objInScreen_Pos.y.ToString(),
                    objInScreen_Pos.z.ToString(),
                    m_Laps.text
                };
        recordedPoints_Pos.Add(data);

        // Increament the text minus by 2, due to header (1) and frame center position (2)
        m_RecordedValue.text = (recordedPoints_Pos.Count - 2).ToString();
    }

    void Points_Save()
    {
        if (recordedPoints_Pos.Count <= 0) return;

        string time = GlobalConfig.GetNowDateandTime();
        string map = GlobalConfig.MapsSelection.ToString();
        string fileName = time + "_NewARScene_recordedPoints_Pos__Maps_" + map + ".csv";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, recordedPoints_Pos);
    }

    /////////////////////////////////////////////////////////
    /// Now we enter the 2D object position record and save
    /////////////////////////////////////////////////////////

    void Raycasts_AddHeader()
    {
        string[] header = new[] {                       // total: 10 objects
            "timestamp", "name",                        // 0, 1
            "near pos x", "near pos y", "near pos z",   // 2, 3, 4
            "ray pos x", "ray pos y", "ray pos z",      // 5, 6, 7
            "distance", "laps"                          // 8, 9
        };
        raycastObject_Pos.Add(header);
        raycasts_hasHeader = true;
    }

    void Raycasts_Record()
    {
        if (!raycasts_hasHeader)
        {
            Raycasts_AddHeader();
            raycasts_hasHeader = true;
        }

        GameObject nearestObj = m_RaycastManager
            .GetComponent<RaycastManager_NewARScene>()
            .GetNearestObject();

        GameObject raycastObj = m_RaycastManager
            .GetComponent<RaycastManager_NewARScene>()
            .GetRaycastObject();

        float dist = m_RaycastManager
            .GetComponent<RaycastManager_NewARScene>()
            .GetRaycastToNearestDist();

        string[] data = new[]
                {
                    GlobalConfig.GetNowDateandTime(),
                    nearestObj.name,

                    nearestObj.transform.position.x.ToString(),
                    nearestObj.transform.position.y.ToString(),
                    nearestObj.transform.position.z.ToString(),

                    raycastObj.transform.position.x.ToString(),
                    raycastObj.transform.position.y.ToString(),
                    raycastObj.transform.position.z.ToString(),

                    dist.ToString(),
                    m_Laps.text
                };
        raycastObject_Pos.Add(data);

        // Increament the text minus by 1, due to header (1)
        m_RecordedValue.text = (raycastObject_Pos.Count - 1).ToString();
    }

    void Raycasts_Save()
    {
        if (raycastObject_Pos.Count <= 0) return;

        string time = GlobalConfig.GetNowDateandTime();
        string map = GlobalConfig.MapsSelection.ToString();
        string fileName = time + "_NewARScene_raycastObject_Pos__Maps_" + map + ".csv";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, raycastObject_Pos);
    }
}
