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
    GameObject m_LoadObjectManager, m_RaycastManager, m_CalibrationManager;

    /// <summary>
    /// Option to enable recording 2D position, raycastPos
    /// </summary>
    [SerializeField]
    bool m_Record2DPos, m_RecordRaycastPos, m_RecordCalibration;

    List<string[]> recordedPoints_Pos = new();
    bool points_hasHeader = false;

    List<string[]> raycastObject_Pos = new();
    bool raycasts_hasHeader = false;

    List<string[]> calibrationObj_Pos = new();                      // from world
    List<string[]> calibrationObj_Pos_fromCloneOrigin = new();      // from clone
    List<string[]> calibrationObj_Pos_fromClone_OneObj = new();      // from clone one obj
    bool calibrationObj_hasHeader = false; bool calibrationFirstTime = false;

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
        if (m_RecordCalibration) Calibration_Record();
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
            Calibration_Save();

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
    /// Now we enter the raycast record and save
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

    /////////////////////////////////////////////////////////
    /// Now we enter the calibration record and save
    /////////////////////////////////////////////////////////

    /// <summary>
    /// To get the correct header for calibration csv.
    /// </summary>
    string[] Calibration_AddHeader()
    {
        string[] header = new[] {                       // total: 10 objects
            "timestamp", "name",                        // 0, 1
            "pos_x", "pos_y", "pos_z",                  // 2, 3, 4
            "rotE_x", "rotE_y", "rotE_z",               // 5, 6, 7
            "rotQ_x", "rotQ_y", "rotQ_z", "rotQ_w",     // 8, 9, 10, 11
            "count"                                     // 12    
        };
        return header;
    }

    /// <summary>
    /// This method should be triggered when "Record data" button pressed.
    /// </summary>
    void Calibration_Record()
    {
        if (GlobalConfig.WORLD_CALIBRATION_OBJ == null) return;
        if (!m_LoadObjectManager.activeSelf) return;

        int count = int.Parse(m_RecordedValue.text);
        count++;

        Calibration_Record_ByWorldOrigin(count);
        Calibration_Record_ByCloneVC(count);
        Calibration_Record_ByCloneVC_OneObj(count);

        calibrationFirstTime = true;
        calibrationObj_hasHeader = true;

        // Increament the text by 1
        m_RecordedValue.text = count.ToString();
    }

    /// <summary>
    /// In this method we calculate error difference based on origin VC. 
    /// Origin VC candidated as reference, then clone VC will be recorded over time.
    /// </summary>
    /// <param name="count"></param>
    void Calibration_Record_ByWorldOrigin(int count)
    {
        GameObject tempGo = new();

        if (!calibrationObj_hasHeader)
        {
            var calibhead = Calibration_AddHeader();
            calibrationObj_Pos.Add(calibhead);
        }

        // get original data
        /////////////////////
        if (!calibrationFirstTime)
        {
            List<GameObject> allLoadObjects = m_LoadObjectManager
                .GetComponent<LoadObject_CatExample_2__NewARScene>()
                .GetMyObjects();

            if (allLoadObjects.Count <= 0) return;

            foreach (var item in allLoadObjects)
            {
                Matrix4x4 localToMarker = GlobalConfig.GetM44ByGameObjRef(
                    item, GlobalConfig.PlaySpaceOriginGO);
                tempGo.transform.position = localToMarker.GetPosition();
                tempGo.transform.rotation = Quaternion.LookRotation(
                    localToMarker.GetColumn(2), localToMarker.GetColumn(1));

                string[] data = new[]
                {
                    GlobalConfig.GetNowDateandTime(),
                    item.name,

                    tempGo.transform.position.x.ToString(),
                    tempGo.transform.position.y.ToString(),
                    tempGo.transform.position.z.ToString(),

                    tempGo.transform.eulerAngles.x.ToString(),
                    tempGo.transform.eulerAngles.y.ToString(),
                    tempGo.transform.eulerAngles.z.ToString(),

                    tempGo.transform.rotation.x.ToString(),
                    tempGo.transform.rotation.y.ToString(),
                    tempGo.transform.rotation.z.ToString(),
                    tempGo.transform.rotation.w.ToString(),

                    count.ToString()
                };
                calibrationObj_Pos.Add(data);
            }
        }

        // get camera data
        //////////////////
        if(m_ARCamera != null)
        {
            GameObject cam = m_ARCamera.gameObject;

            Matrix4x4 localToMarker = GlobalConfig.GetM44ByGameObjRef(
                    cam, GlobalConfig.PlaySpaceOriginGO);
            tempGo.transform.position = localToMarker.GetPosition();
            tempGo.transform.rotation = Quaternion.LookRotation(
                localToMarker.GetColumn(2), localToMarker.GetColumn(1));

            string[] data = new[]
            {
                    GlobalConfig.GetNowDateandTime(),
                    cam.name,

                    tempGo.transform.position.x.ToString(),
                    tempGo.transform.position.y.ToString(),
                    tempGo.transform.position.z.ToString(),

                    tempGo.transform.eulerAngles.x.ToString(),
                    tempGo.transform.eulerAngles.y.ToString(),
                    tempGo.transform.eulerAngles.z.ToString(),

                    tempGo.transform.rotation.x.ToString(),
                    tempGo.transform.rotation.y.ToString(),
                    tempGo.transform.rotation.z.ToString(),
                    tempGo.transform.rotation.w.ToString(),

                    count.ToString()
                };

            calibrationObj_Pos.Add(data);
        }

        // get clone data
        /////////////////
        List<GameObject> allCloneObjects = m_CalibrationManager
                .GetComponent<Test_TurnOnOffWorldCalib>()
                .GetCloneObject();

        if (allCloneObjects.Count <= 0) return;

        foreach (var item in allCloneObjects)
        {
            Matrix4x4 localToMarker = GlobalConfig.GetM44ByGameObjRef(
                    item, GlobalConfig.PlaySpaceOriginGO);
            tempGo.transform.position = localToMarker.GetPosition();
            tempGo.transform.rotation = Quaternion.LookRotation(
                localToMarker.GetColumn(2), localToMarker.GetColumn(1));

            string[] data = new[]
            {
                    GlobalConfig.GetNowDateandTime(),
                    item.name,

                    tempGo.transform.position.x.ToString(),
                    tempGo.transform.position.y.ToString(),
                    tempGo.transform.position.z.ToString(),

                    tempGo.transform.eulerAngles.x.ToString(),
                    tempGo.transform.eulerAngles.y.ToString(),
                    tempGo.transform.eulerAngles.z.ToString(),

                    tempGo.transform.rotation.x.ToString(),
                    tempGo.transform.rotation.y.ToString(),
                    tempGo.transform.rotation.z.ToString(),
                    tempGo.transform.rotation.w.ToString(),

                    count.ToString()
                };

            calibrationObj_Pos.Add(data);
        }

        Destroy(tempGo);
    }

    /// <summary>
    /// In this method is the vice versa of the previous one. 
    /// Origin VC will be recorded over time, while clone VC candidated as reference.
    /// </summary>
    /// <param name="count"></param>
    void Calibration_Record_ByCloneVC(int count)
    {
        GameObject tempGo = new();

        if (!calibrationObj_hasHeader)
        {
            var calibhead = Calibration_AddHeader();
            calibrationObj_Pos_fromCloneOrigin.Add(calibhead);
        }

        List<GameObject> allLoadObjects = m_LoadObjectManager
                .GetComponent<LoadObject_CatExample_2__NewARScene>()
                .GetMyObjects();

        if (allLoadObjects.Count <= 0) return;

        // get original data
        /////////////////////
        if (!calibrationFirstTime)
        {
            foreach (var item in allLoadObjects)
            {
                // i know this is not effective
                // reference = VC clone root
                Matrix4x4 localToVCorigin = GlobalConfig.GetM44ByGameObjRef(
                    item, GlobalConfig.PlaySpaceOriginGO);
                tempGo.transform.position = localToVCorigin.GetPosition();
                tempGo.transform.rotation = Quaternion.LookRotation(
                    localToVCorigin.GetColumn(2), localToVCorigin.GetColumn(1));

                string[] data2 = new[]
                {
                    GlobalConfig.GetNowDateandTime(),
                    item.name + "(Clone)",

                    tempGo.transform.position.x.ToString(),
                    tempGo.transform.position.y.ToString(),
                    tempGo.transform.position.z.ToString(),

                    tempGo.transform.eulerAngles.x.ToString(),
                    tempGo.transform.eulerAngles.y.ToString(),
                    tempGo.transform.eulerAngles.z.ToString(),

                    tempGo.transform.rotation.x.ToString(),
                    tempGo.transform.rotation.y.ToString(),
                    tempGo.transform.rotation.z.ToString(),
                    tempGo.transform.rotation.w.ToString(),

                    count.ToString()
                };

                calibrationObj_Pos_fromCloneOrigin.Add(data2);
            }
        }

        // get camera data
        //////////////////
        if (m_ARCamera != null)
        {
            GameObject cam = m_ARCamera.gameObject;

            // i know this is not effective
            // reference = VC clone root
            Matrix4x4 localToVCorigin = GlobalConfig.GetM44ByGameObjRef(
                cam, GlobalConfig.WORLD_CALIBRATION_OBJ);
            tempGo.transform.position = localToVCorigin.GetPosition();
            tempGo.transform.rotation = Quaternion.LookRotation(
                localToVCorigin.GetColumn(2), localToVCorigin.GetColumn(1));

            string[] data2 = new[]
            {
                    GlobalConfig.GetNowDateandTime(),
                    cam.name,

                    tempGo.transform.position.x.ToString(),
                    tempGo.transform.position.y.ToString(),
                    tempGo.transform.position.z.ToString(),

                    tempGo.transform.eulerAngles.x.ToString(),
                    tempGo.transform.eulerAngles.y.ToString(),
                    tempGo.transform.eulerAngles.z.ToString(),

                    tempGo.transform.rotation.x.ToString(),
                    tempGo.transform.rotation.y.ToString(),
                    tempGo.transform.rotation.z.ToString(),
                    tempGo.transform.rotation.w.ToString(),

                    count.ToString()
                };
            calibrationObj_Pos_fromCloneOrigin.Add(data2);
        }

        // get clone data
        /////////////////
        //List<GameObject> allLoadObjects = m_LoadObjectManager
        //        .GetComponent<LoadObject_CatExample_2__NewARScene>()
        //        .GetMyObjects();

        foreach (var item in allLoadObjects)
        {
            // i know this is not effective
            // reference = VC clone root
            Matrix4x4 localToVCorigin = GlobalConfig.GetM44ByGameObjRef(
                item, GlobalConfig.WORLD_CALIBRATION_OBJ);
            tempGo.transform.position = localToVCorigin.GetPosition();
            tempGo.transform.rotation = Quaternion.LookRotation(
                localToVCorigin.GetColumn(2), localToVCorigin.GetColumn(1));

            string[] data2 = new[]
            {
                    GlobalConfig.GetNowDateandTime(),
                    item.name,

                    tempGo.transform.position.x.ToString(),
                    tempGo.transform.position.y.ToString(),
                    tempGo.transform.position.z.ToString(),

                    tempGo.transform.eulerAngles.x.ToString(),
                    tempGo.transform.eulerAngles.y.ToString(),
                    tempGo.transform.eulerAngles.z.ToString(),

                    tempGo.transform.rotation.x.ToString(),
                    tempGo.transform.rotation.y.ToString(),
                    tempGo.transform.rotation.z.ToString(),
                    tempGo.transform.rotation.w.ToString(),

                    count.ToString()
                };
            calibrationObj_Pos_fromCloneOrigin.Add(data2);
        }

        Destroy(tempGo);
    }

    /// <summary>
    /// In this method is the vice versa of the previous one. 
    /// Origin VC will be recorded over time, while clone VC candidated as reference.
    /// </summary>
    /// <param name="count"></param>
    void Calibration_Record_ByCloneVC_OneObj(int count)
    {
        GameObject tempGo = new();

        if (!calibrationObj_hasHeader)
        {
            var calibhead = Calibration_AddHeader();
            calibrationObj_Pos_fromClone_OneObj.Add(calibhead);
        }

        List<GameObject> allLoadObjects = m_LoadObjectManager
                .GetComponent<LoadObject_CatExample_2__NewARScene>()
                .GetMyObjects();

        if (allLoadObjects.Count <= 0) return;

        List<GameObject> allCloneObjects = m_CalibrationManager
                .GetComponent<Test_TurnOnOffWorldCalib>()
                .GetCloneObject();

        if (allCloneObjects.Count <= 0) return;

        GameObject nearGo = GlobalConfig.GetNearestObject(allCloneObjects, m_ARCamera.gameObject, out int i);
        GameObject worldGo = allLoadObjects[i];

        // ground truth (clone)
        Matrix4x4 localToVCorigin = GlobalConfig.GetM44ByGameObjRef(
                    worldGo, GlobalConfig.PlaySpaceOriginGO);
        tempGo.transform.position = localToVCorigin.GetPosition();
        tempGo.transform.rotation = Quaternion.LookRotation(
            localToVCorigin.GetColumn(2), localToVCorigin.GetColumn(1));

        string[] data = new[]
                {
                    GlobalConfig.GetNowDateandTime(),
                    worldGo.name + "(Clone)",

                    tempGo.transform.position.x.ToString(),
                    tempGo.transform.position.y.ToString(),
                    tempGo.transform.position.z.ToString(),

                    tempGo.transform.eulerAngles.x.ToString(),
                    tempGo.transform.eulerAngles.y.ToString(),
                    tempGo.transform.eulerAngles.z.ToString(),

                    tempGo.transform.rotation.x.ToString(),
                    tempGo.transform.rotation.y.ToString(),
                    tempGo.transform.rotation.z.ToString(),
                    tempGo.transform.rotation.w.ToString(),

                    count.ToString()
                };
        calibrationObj_Pos_fromClone_OneObj.Add(data);

        // ar camera (clone)
        localToVCorigin = GlobalConfig.GetM44ByGameObjRef(
                    m_ARCamera.gameObject, GlobalConfig.WORLD_CALIBRATION_OBJ);
        tempGo.transform.position = localToVCorigin.GetPosition();
        tempGo.transform.rotation = Quaternion.LookRotation(
            localToVCorigin.GetColumn(2), localToVCorigin.GetColumn(1));

        data = new[]
                {
                    GlobalConfig.GetNowDateandTime(),
                    m_ARCamera.gameObject.name,

                    tempGo.transform.position.x.ToString(),
                    tempGo.transform.position.y.ToString(),
                    tempGo.transform.position.z.ToString(),

                    tempGo.transform.eulerAngles.x.ToString(),
                    tempGo.transform.eulerAngles.y.ToString(),
                    tempGo.transform.eulerAngles.z.ToString(),

                    tempGo.transform.rotation.x.ToString(),
                    tempGo.transform.rotation.y.ToString(),
                    tempGo.transform.rotation.z.ToString(),
                    tempGo.transform.rotation.w.ToString(),

                    count.ToString()
                };
        calibrationObj_Pos_fromClone_OneObj.Add(data);

        // actual (drift)
        localToVCorigin = GlobalConfig.GetM44ByGameObjRef(
                    worldGo, GlobalConfig.WORLD_CALIBRATION_OBJ);
        tempGo.transform.position = localToVCorigin.GetPosition();
        tempGo.transform.rotation = Quaternion.LookRotation(
            localToVCorigin.GetColumn(2), localToVCorigin.GetColumn(1));

        data = new[]
                {
                    GlobalConfig.GetNowDateandTime(),
                    worldGo.name,

                    tempGo.transform.position.x.ToString(),
                    tempGo.transform.position.y.ToString(),
                    tempGo.transform.position.z.ToString(),

                    tempGo.transform.eulerAngles.x.ToString(),
                    tempGo.transform.eulerAngles.y.ToString(),
                    tempGo.transform.eulerAngles.z.ToString(),

                    tempGo.transform.rotation.x.ToString(),
                    tempGo.transform.rotation.y.ToString(),
                    tempGo.transform.rotation.z.ToString(),
                    tempGo.transform.rotation.w.ToString(),

                    count.ToString()
                };
        calibrationObj_Pos_fromClone_OneObj.Add(data);

        Destroy(tempGo);
    }

    /// <summary>
    /// This method is to save all recorded calibration data into csv.
    /// </summary>
    void Calibration_Save()
    {
        if (calibrationObj_Pos.Count <= 0) return;

        string time = GlobalConfig.GetNowDateandTime();
        string map = GlobalConfig.MapsSelection.ToString();
        string fileName = time + "_NewARScene_calibrationObj_Pos_fromWorldOrigin__Maps_" + map + ".csv";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, calibrationObj_Pos);

        if (calibrationObj_Pos_fromCloneOrigin.Count <= 0) return;

        time = GlobalConfig.GetNowDateandTime();
        map = GlobalConfig.MapsSelection.ToString();
        fileName = time + "_NewARScene_calibrationObj_Pos_fromCloneOrigin__Maps_" + map + ".csv";
        path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, calibrationObj_Pos_fromCloneOrigin);

        if (calibrationObj_Pos_fromClone_OneObj.Count <= 0) return;

        time = GlobalConfig.GetNowDateandTime();
        map = GlobalConfig.MapsSelection.ToString();
        fileName = time + "_NewARScene_calibrationObj_Pos_fromClone_OneObj__Maps_" + map + ".csv";
        path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, calibrationObj_Pos_fromClone_OneObj);
    }
}
