using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// This script now has 3 function:
/// - record camera position (record and save)
/// - record load object based on camera positin (record and save)
/// - point cloud ulong (save)
///
/// If record data is zero
/// </summary>
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

    [SerializeField]
    GameObject m_LoadObjectManager;

    [SerializeField]
    GameObject m_MappingScannerForPointCloud;

    /// <summary>
    /// Option to enable recording camPos, loadObjPos
    /// Option to enable save the point cloud (because it has large data)
    /// </summary>
    [SerializeField]
    bool m_RecordCamPos, m_RecordLoadObjPos, m_SavePointCloud;

    List<string[]> recordedCamera_Pos = new();
    List<string[]> recordedLoadObject_Pos = new();
    bool cameraPos_hasHeader, loadObject_hasHeader = false;
    bool hasCamReference = false;
    Quaternion camRefRotation;

    /// <summary>
    /// Laps from 1, and recorded value from 0
    /// </summary>
    void Start()
    {
        m_Laps.text = "1";
        m_RecordedValue.text = "0";
    }

    /// <summary>
    /// To record camera and loadObject data
    /// </summary>
    public void RecordData()
    {
        if (m_RecordCamPos) CameraPos_Record();
        if (m_RecordLoadObjPos) LoadObject_Record();
    }

    /// <summary>
    /// To save camera, loadObject, and pointCloud data
    /// </summary>
    public void SaveData()
    {
        try
        {
            CameraPos_Save();
            LoadObject_Save();
            if (m_SavePointCloud) PointClouds_Save();
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
    /// Now we enter the camera position record and save
    /////////////////////////////////////////////////////////

    void CameraPos_Record()
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

        // because in string[] there is header, so must (-1)
        m_RecordedValue.text = (recordedCamera_Pos.Count-1).ToString();
    }

    void CameraPos_Save()
    {
        if (recordedCamera_Pos.Count <= 0) return;

        string time = GlobalConfig.GetNowDateandTime();
        string map = GlobalConfig.SAVE_INTO_MAP.ToString();
        string fileName = time + "_recordedCamera_Pos__Maps_" + map + ".csv";
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
    /// Now we enter the load object record and save
    /////////////////////////////////////////////////////////

    void LoadObject_Record()
    {
        if (!loadObject_hasHeader) LoadObject_AddHeader();

        if (!m_LoadObjectManager.activeSelf) return;

        List<GameObject> allObjects = m_LoadObjectManager
            .GetComponent<LoadObject_CatExample_2>()
            .GetMyObjects();

        if (allObjects.Count <= 0) return;

        // camera reference should directing to the same angle
        // otherwise object-camera pos will different each camera direction
        // this is very serious that affecting the data when record the position
        if (!hasCamReference)
        {
            hasCamReference = true;
            camRefRotation = m_ARCamera.transform.rotation;
        }

        // use of camRefRotation to make new gameobject as reference
        GameObject tempGo = new();
        tempGo.transform.SetPositionAndRotation(m_ARCamera.transform.position, camRefRotation);

        foreach (var obj in allObjects)
        {
            //Vector3 obj_pos_v3 = obj.transform.position;
            //Vector4 obj_pos_v4 = new(obj_pos_v3.x, obj_pos_v3.y, obj_pos_v3.z, 1);

            //GameObject newGO = new();
            //Vector4 newPos_v4 = m_ARCamera.transform.worldToLocalMatrix *
            //    obj.transform.localToWorldMatrix *
            //    obj_pos_v4;
            //newGO.transform.position = new(newPos_v4.x, newPos_v4.y, newPos_v4.z);

            //Vector3 pos = newGO.transform.position;
            //Quaternion rot = newGO.transform.rotation;

            // use tempGo as reference, not AR Camera anymore
            // by this AR Camera can freely direct to any angle
            // without affecting as camera angle reference to all myObject
            Matrix4x4 fromObjToCamera =
                tempGo.transform.worldToLocalMatrix *
                obj.transform.localToWorldMatrix;

            Vector3 newPos = fromObjToCamera.GetPosition();

            Quaternion newRot = Quaternion.LookRotation(
                fromObjToCamera.GetColumn(2),
                fromObjToCamera.GetColumn(1));

            string[] data = new[]
                {
                    GlobalConfig.GetNowDateandTime(),
                    obj.name,
                    newPos.x.ToString(),
                    newPos.y.ToString(),
                    newPos.z.ToString(),
                    newRot.x.ToString(),
                    newRot.y.ToString(),
                    newRot.z.ToString(),
                    newRot.w.ToString(),
                    m_Laps.text
                };
            recordedLoadObject_Pos.Add(data);
        }

        // because in string[] there is header, so must (-1)
        m_RecordedValue.text = (recordedLoadObject_Pos.Count - 1).ToString();
    }

    void LoadObject_Save()
    {
        if (!m_LoadObjectManager.activeSelf) return;

        if (recordedLoadObject_Pos.Count <= 0) return;

        string time = GlobalConfig.GetNowDateandTime();
        string map = GlobalConfig.SAVE_INTO_MAP.ToString();
        string fileName = time + "_recordedLoadObject_Pos__Maps_" + map + ".csv";        
        string path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, recordedLoadObject_Pos);
    }

    void LoadObject_AddHeader()
    {
        string[] header = new[] {
            "timestamp", "name",
            "pos x", "pos y", "pos z",
            "rot quat x", "rot quat y", "rot quat z", "rot quat w",
            "laps"
        };
        recordedLoadObject_Pos.Add(header);
        loadObject_hasHeader = true;
    }

    /////////////////////////////////////////////////////////
    /// Now we enter the point cloud save
    /////////////////////////////////////////////////////////

    void PointClouds_Save()
    {
        // create new list
        List<string[]> pointClouds_Pos = new();

        // insert the header
        string[] header = new[]
        {
            "identifier", "pos x", "pos y", "pos z", "status"
        };
        pointClouds_Pos.Add(header);

        // import data from MappingScanner
        List<Vector3> pointClouds = m_MappingScannerForPointCloud
            .GetComponent<MappingScanner>()
            .GetPointCloudsVector3s();
        List<ulong> pointCloudUlongs = m_MappingScannerForPointCloud
            .GetComponent<MappingScanner>()
            .GetPointCloudsUlongs();

        // insert data into list
        for (int i = 0; i < pointClouds.Count; i++)
        {
            // use try catch to prevent code stopped
            try
            {
                string[] data = new[]
                {
                    pointCloudUlongs[i].ToString(),
                    pointClouds[i].x.ToString(),
                    pointClouds[i].y.ToString(),
                    pointClouds[i].z.ToString(),
                    "success"
                };
                pointClouds_Pos.Add(data);
            }
            catch (System.Exception ex)
            {
                string[] data = new[]
                {
                    "",
                    "",
                    "",
                    "",
                    ex.ToString()
                };
                pointClouds_Pos.Add(data);
            }
        }

        // save data into csv
        string time = GlobalConfig.GetNowDateandTime();
        string map = GlobalConfig.SAVE_INTO_MAP.ToString();
        string fileName = time + "_pointClouds_Pos__Maps_" + map + ".csv";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, pointClouds_Pos);
    }
}
