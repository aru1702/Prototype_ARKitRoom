using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;

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
    GameObject m_LoadObjectManager;

    List<string[]> recordedPoints_Pos = new();
    bool points_hasHeader = false;

    private void Start()
    {
        m_Laps.text = "1";
        m_RecordedValue.text = "0";
    }

    void IncreamentRecordedValue()
    {
        m_RecordedValue.text = (recordedPoints_Pos.Count - 2).ToString();
    }

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

    public void Points_Record()
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

        IncreamentRecordedValue();
    }

    public void Points_Save()
    {
        if (!m_LoadObjectManager.activeSelf) return;

        string time = GlobalConfig.GetNowDateandTime();
        string map = GlobalConfig.MapsSelection.ToString();
        string fileName = time + "_recordedPoints_Pos__Maps_" + map + ".csv";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, recordedPoints_Pos);

        m_UIManager
            .GetComponent<UIManager_CatExample>()
            .MapStatus.text = "Recorded position saved successfully!";

        m_UIManager
            .GetComponent<UIManager_CatExample>()
            .OpenPanel();
    }
}
