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

    List<string[]> recordedCamera_Pos = new();
    bool cameraPos_hasHeader = false;

    private void Start()
    {
        m_Laps.text = "1";
        StartCoroutine(TickPerSecond());
    }

    IEnumerator TickPerSecond()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);
            CameraPos_Record();
        }
    }

    public void CameraPos_Record()
    {
        Vector3 pos = m_ARCamera.transform.position;
        Quaternion rot = m_ARCamera.transform.rotation;

        if (!cameraPos_hasHeader) CameraPos_AddHeader();

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
        recordedCamera_Pos.Add(data);
    }

    public void CameraPos_Save()
    {
        string time = GlobalConfig.GetNowDateandTime();
        string fileName = time + "_recordedCamera_PerFrameForSLAM.csv";
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
}
