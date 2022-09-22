using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Save function:
/// - get each camera trail from RecordPosition_CameraEveryFrame
/// - save into static name of each map (file name)
/// - camera trail GO name, pos, rot, maps
///
/// Load function:
/// - load worldmap
/// - load the file, for every object
/// - create empty GO
/// - create cube (X cm size) on under each object
/// - add every of them into RecordPosition_CameraEveryFrame
/// </summary>
public class RecordPosition_SaveAndLoad : MonoBehaviour
{
    [SerializeField]
    GameObject m_RecordPositionCameraEveryFrame;

    [SerializeField]
    GameObject m_LoadObjectManager;

    [SerializeField]
    bool m_ActiveSaveAndLoadCameraTrail;

    [SerializeField]
    bool m_ActiveLoadAfterLoadObject;

    /// <summary>
    /// Save into dedicated csv
    /// </summary>
    public void SaveData()
    {
        if (!m_ActiveSaveAndLoadCameraTrail) return;

        //List<GameObject> cameraTrails = m_RecordPositionCameraEveryFrame
        //    .GetComponent<RecordPosition_CameraEveryFrame>()
        //    .GetCameraTracks();

        List<CameraTrail> cameraTrails = m_RecordPositionCameraEveryFrame
            .GetComponent<RecordPosition_CameraEveryFrame>()
            .GetCameraTrails();

        string[] header = new[] {
                "timestamp",
                "pos x", "pos y", "pos z",
                "rot quat x", "rot quat y", "rot quat z", "rot quat w",
                "laps"
            };

        // the format is as same as from RecordPosition_CameraEveryFrame
        // unless the naming file is different
        // we choose dedicated name without timestamp
        // (while previously, timestamp is unique to deny overwrite file)
        List<string[]> recordedSLAMAdjusted_Pos = new();
        recordedSLAMAdjusted_Pos.Add(header);

        foreach (var trail in cameraTrails)
        {
            Vector3 pos = trail.position;
            Quaternion rot = trail.rotation;

            string[] data = new[]
            {
                trail.name,
                pos.x.ToString(),
                pos.y.ToString(),
                pos.z.ToString(),
                rot.x.ToString(),
                rot.y.ToString(),
                rot.z.ToString(),
                rot.w.ToString(),
                trail.laps
            };
            recordedSLAMAdjusted_Pos.Add(data);
        }

        string map = GlobalConfig.SAVE_INTO_MAP.ToString();
        string fileName = "RecordedSLAMAdjusted_" + map + ".csv";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, recordedSLAMAdjusted_Pos);
    }

    /// <summary>
    /// Load into CameraTrail class then add into
    /// RecordPosition_CameraEveryFrame
    /// </summary>
    public void LoadData()
    {
        if (!m_ActiveSaveAndLoadCameraTrail) return;

        // import data
        List<CameraTrail> cameraTrails = Import_FromCameraTrail.GetCameraTrailList();
        if (cameraTrails.Count <= 0)
        {
            Debug.LogError("No camera trails data");
            return;
        }

        if (!m_ActiveLoadAfterLoadObject)
        {
            LoadCameraTrailData(cameraTrails);
        }
        else
        {
            if (!m_LoadObjectManager) return;

            m_LoadObjectManager
                .GetComponent<LoadObject_CatExample_2>()
                .Test_LoadCameraTrails(cameraTrails, this);
        }
    }

    /// <summary>
    /// Affecting in LoadObject_CatExample_2
    /// Used in MappingConfiguration scene
    /// </summary>
    /// <param name="cameraTrails"></param>
    public void LoadCameraTrailData(List<CameraTrail> cameraTrails)
    {
        m_RecordPositionCameraEveryFrame
                .GetComponent<RecordPosition_CameraEveryFrame>()
                .ImportFromOutsource(cameraTrails);
    }
}
