using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;

public class RecordPosition : MonoBehaviour
{
    [SerializeField]
    GameObject m_ARCamera;

    [SerializeField]
    GameObject m_UIManager;

    [SerializeField]
    Text m_RecordedValue;

    [SerializeField]
    InputField m_Laps;

    List<string[]> recordedPosition = new();
    bool hasHeader = false;

    private void Start()
    {
        m_Laps.text = "1";
        m_RecordedValue.text = "0";
    }

    public void Record()
    {
        Vector3 pos = m_ARCamera.transform.position;
        Quaternion rot = m_ARCamera.transform.rotation;

        if (!hasHeader) AddHeader();

        string[] data = new[]
        {
            GlobalConfig.GetNowDateandTime(),
            pos.x.ToString(),
            pos.y.ToString(),
            pos.z.ToString(),
            rot.x.ToString(),
            rot.y.ToString(),
            rot.z.ToString(),
            rot.w.ToString(),
            m_Laps.text
        };
        recordedPosition.Add(data);

        m_RecordedValue.text = (recordedPosition.Count-1).ToString();
    }

    public void Save()
    {
        string time = GlobalConfig.GetNowDateandTime();
        string fileName = time + "_RecordedPosition.csv";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, recordedPosition);

        m_UIManager
            .GetComponent<UIManager_CatExample>()
            .MapStatus.text = "Recorded position saved successfully!";

        m_UIManager
            .GetComponent<UIManager_CatExample>()
            .OpenPanel();
    }

    void AddHeader()
    {
        string[] header = new[] {
            "timestamp",
            "pos x", "pos y", "pos z",
            "rot quat x", "rot quat y", "rot quat z", "rot quat w",
            "laps"
        };
        recordedPosition.Add(header);
        hasHeader = true;
    }
}
