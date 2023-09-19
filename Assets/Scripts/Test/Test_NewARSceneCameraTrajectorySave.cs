using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class Test_NewARSceneCameraTrajectorySave : MonoBehaviour
{
    [SerializeField]
    GameObject m_ARCamera;

    [SerializeField]
    float m_CreateTrailPerSecond = 1.0f;

    [SerializeField]
    bool m_EnableCameraRecording = false;

    GameObject localWorldCoordinate;

    List<string[]> m_RecordedCameraData = new();

    // This function is similar to MappingV2 camera record and save
    /// <summary>
    /// Record camera per tick (... seconds)
    /// </summary>
    void CameraRecord()
    {
        if (m_ARCamera == null) return;

        // data based to world coordinate
        var m44 = GlobalConfig.GetM44ByGameObjRef(m_ARCamera, localWorldCoordinate);
        Vector3 pos = GlobalConfig.GetPositionFromM44(m44);
        Quaternion qrot = GlobalConfig.GetRotationFromM44(m44);
        Vector3 rot = GlobalConfig.GetEulerAngleFromM44(m44);

        string[] data = new[]
        {
            GlobalConfig.GetNowDateandTime(),   // 0
            pos.x.ToString(),
            pos.y.ToString(),
            pos.z.ToString(),
            rot.x.ToString(),
            rot.y.ToString(),
            rot.z.ToString(),                   // 6
            qrot.x.ToString(),
            qrot.y.ToString(),
            qrot.z.ToString(),
            qrot.w.ToString()                   // 10
        };

        m_RecordedCameraData.Add(data);
    }

    /// <summary>
    /// Save CameraRecord into csv
    /// - date
    /// - camera pos (xyz)
    /// - camera rot (xyz)
    /// </summary>
    public void CameraSave()
    {
        // save nothing if there is no data
        if (m_RecordedCameraData.Count <= 0) return;

        string time = GlobalConfig.GetNowDateandTime();
        string map = GlobalConfig.LOAD_MAP.ToString();

        // for documentation
        string fileName = time + "_NewARScene__CamTrajectoryTest__Maps_" + map + ".csv";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, m_RecordedCameraData);
    }

    IEnumerator TickPerPeriod()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_CreateTrailPerSecond);

            if (GlobalConfig.PlaySpaceOriginGO != null)
            {
                if (localWorldCoordinate == null)
                    localWorldCoordinate = GlobalConfig.PlaySpaceOriginGO;

                if (m_EnableCameraRecording) CameraRecord();
            }
        }
    }

    private void Start()
    {
        string[] header;

        header = new[] {
            "timestamp",
            "pos_x", "pos_y", "pos_z",
            "rot_e_x", "rot_e_y", "rot_e_z",
            "q_rot_e_x", "q_rot_e_y", "q_rot_e_z", "q_rot_e_w"
        };
        m_RecordedCameraData.Add(header);

        StartCoroutine(TickPerPeriod());
    }
}
