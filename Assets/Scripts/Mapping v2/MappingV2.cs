using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// This script should do:
/// - record camera track according to the designated world coordinate
/// - record "calibration marker" according to the designated world coordinate
/// - save both data
/// </summary>
public class MappingV2 : MonoBehaviour
{
    // this script field
    [SerializeField]
    GameObject m_ARCamera;                  // this for getting the AR Camera

    [SerializeField]
    GameObject m_ImageRecognitionManager;   // this for getting the list of calibration marker

    [SerializeField]
    GameObject m_LoadObjectManager;         // this for getting groundTruth data which saved online
    List<GameObject> m_MarkerGroundTruth = new();

    List<string[]> m_RecordedCameraData = new();
    List<string[]> m_RecordedMarkerData = new();
    GameObject localWorldCoordinate;

    // enabler (on/off) in Editor
    [SerializeField]
    bool m_EnableCameraRecording = true;

    [SerializeField]
    bool m_EnableMarkerRecording = true;

    /// <summary>
    /// Start function
    /// </summary>
    void Start()
    {
        string[] header;

        header = new[] {
            "timestamp",
            "pos_x", "pos_y", "pos_z",
            "rot_e_x", "rot_e_y", "rot_e_z"
        };
        m_RecordedCameraData.Add(header);

        header = new[] {
            "timestamp", "name",

            "gt_pos_x", "gt_pos_y", "gt_pos_z",
            "gt_rot_e_x", "gt_rot_e_y", "gt_rot_e_z",

            "c_pos_x", "c_pos_y", "c_pos_z",
            "c_rot_e_x", "c_rot_e_y", "c_rot_e_z"
        };
        m_RecordedMarkerData.Add(header);

        StartCoroutine(TickPerPeriod());
    }

    public void SaveData()
    {
        CameraSave();
        MarkerSave();

        // in here no UIManager to show already save
        //  because this function also being called simultanously
        //  with RecordPosition script's function
    }

    /// <summary>
    /// This is the function to record data per period automatically
    /// </summary>
    [SerializeField]
    float m_CreateTrailPerSecond = 1.0f;
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
                if (m_EnableMarkerRecording) MarkerRecord();
            }
        }
    }

    /// <summary>
    /// Record camera per tick (... seconds)
    /// </summary>
    void CameraRecord()
    {
        if (m_ARCamera == null) return;

        // data based to world coordinate
        var m44 = GlobalConfig.GetM44ByGameObjRef(m_ARCamera, localWorldCoordinate);
        Vector3 pos = GlobalConfig.GetPositionFromM44(m44);
        Vector3 rot = GlobalConfig.GetEulerAngleFromM44(m44);

        string[] data = new[]
        {
            GlobalConfig.GetNowDateandTime(),   // 0
            pos.x.ToString(),
            pos.y.ToString(),
            pos.z.ToString(),
            rot.x.ToString(),
            rot.y.ToString(),
            rot.z.ToString()                    // 6
        };

        m_RecordedCameraData.Add(data);
    }

    /// <summary>
    /// Save CameraRecord into csv
    /// - date
    /// - camera pos (xyz)
    /// - camera rot (xyz)
    /// </summary>
    void CameraSave()
    {
        // save nothing if there is no data
        if (m_RecordedCameraData.Count <= 0) return;

        string time = GlobalConfig.GetNowDateandTime();
        string map = GlobalConfig.SAVE_INTO_MAP.ToString();

        // for data process
        string fileName = GetCameraTrajectoryFileName(map);
        string path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, m_RecordedCameraData);

        // for documentation
        fileName = time + "_RecordedCameraDataV2__Maps_" + map + ".csv";
        path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, m_RecordedCameraData);
    }

    /// <summary>
    /// Get calibration marker groundTruth from internet source file
    /// </summary>
    void GetMarkerGroundTruth()
    {
        if (m_LoadObjectManager == null) return;

        var scriptL = m_LoadObjectManager.GetComponent<LoadObject_CatExample_2>();
        foreach (var item in scriptL.GetMyParents())
        {
            //Debug.Log("name: " + item.name);

            string[] names = item.name.Split("_");
            if (names[0] == "img")
                m_MarkerGroundTruth.Add(item);
        }

        //Debug.Log("how many GT: " + m_MarkerGroundTruth.Count);
    }

    /// <summary>
    /// Record calibration marker per tick (... seconds)
    /// Marker in recorded only that in "Tracking" status 
    /// </summary>
    void MarkerRecord()
    {
        if (m_ImageRecognitionManager == null) return;

        var scriptI = m_ImageRecognitionManager
            .GetComponent<ImageRecognition_CatExample_2>();

        var imageTrackedList = scriptI.GetAllImageTargetsTranform();

        //Debug.Log("how many IT: " + transforms.Count);

        // check if no data
        if (imageTrackedList.Count <= 0) return;

        foreach (var imageTracked in imageTrackedList)
        {
            //Debug.Log("item name: " + item.name);

            // data based on online
            if (m_MarkerGroundTruth.Count <= 0) GetMarkerGroundTruth();

            //GameObject gT = new();
            //Vector3 gT_pos = new();
            //Vector3 gT_rot = new();

            foreach (var mGT in m_MarkerGroundTruth)
            {
                // DEBUGGIN ONLY
                //Debug.Log("how many GT again: " + m_MarkerGroundTruth.Count);
                //Debug.Log("GT name: " + mGT.name);

                // only save data if exist by DATABASE
                // for those not assigned, NO SAVE
                if (string.Equals(mGT.name, imageTracked.name))
                {
                    //var gT_pos = mGT.transform.localPosition;
                    //var gT_rot = mGT.transform.localEulerAngles;

                    // ground truth data with world coordinate ref
                    var gT_m44 = GlobalConfig.GetM44ByGameObjRef(mGT, localWorldCoordinate);
                    var gT_pos = GlobalConfig.GetPositionFromM44(gT_m44);
                    var gT_rot = GlobalConfig.GetEulerAngleFromM44(gT_m44);

                    // current data with world coordinate ref
                    var cR_m44 = GlobalConfig.GetM44ByGameObjRef(imageTracked.transformObj, localWorldCoordinate);
                    var c_pos = GlobalConfig.GetPositionFromM44(cR_m44);
                    var c_rot = GlobalConfig.GetEulerAngleFromM44(cR_m44);

                    string[] data = new[]
                    {
                        GlobalConfig.GetNowDateandTime(),   // 0
                        imageTracked.name,

                        gT_pos.x.ToString(),                // 2
                        gT_pos.y.ToString(),
                        gT_pos.z.ToString(),
                        gT_rot.x.ToString(),
                        gT_rot.y.ToString(),
                        gT_rot.z.ToString(),                // 7

                        c_pos.x.ToString(),                 // 8
                        c_pos.y.ToString(),
                        c_pos.z.ToString(),
                        c_rot.x.ToString(),
                        c_rot.y.ToString(),
                        c_rot.z.ToString()                  // 13
                    };

                    m_RecordedMarkerData.Add(data);

                    //Destroy(gT);

                    // DEBUGGING ONLY, CHECK NAME AND TRANSFORM INFORMATION
                    //Debug.Log("ImageTarget found: \n" +
                    //          "  name: " + mGT.name + "\n" +
                    //          "  GT pos: " + gT_pos.ToString() + "\n" +
                    //          "  GT rot: " + gT_rot.ToString() + "\n" +
                    //          "  Cr pos: " + c_pos.ToString() + "\n" +
                    //          "  Cr rot: " + c_rot.ToString());

                    //RelocateARCamera(mGT, imageTracked.transform);
                }
            }
        }
    }

    /// <summary>
    /// Save MarkerRecord into csv
    /// - date
    /// - name
    /// - groundtruth pos (xyz)
    /// - groundtruth rot (xyz)
    /// - current pos
    /// - current rot
    /// </summary>
    void MarkerSave()
    {
        // save nothing if there is no data
        if (m_RecordedMarkerData.Count <= 0) return;

        string time = GlobalConfig.GetNowDateandTime();
        string map = GlobalConfig.SAVE_INTO_MAP.ToString();

        // for data process
        string fileName = GetMarkerCalibrationFileName(map);
        string path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, m_RecordedMarkerData);

        // for documentation
        fileName = time + "_RecordedMarkerDataV2__Maps_" + map + ".csv";
        path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, m_RecordedMarkerData);
    }

    public static string GetCameraTrajectoryFileName(string map)
    {
        return "CameraTrajectory__Maps_" + map + ".csv";
    }

    public static string GetMarkerCalibrationFileName(string map)
    {
        return "MarkerCalibration__Maps_" + map + ".csv";
    }

    public void SaveMarkerOnly()
    {
        MarkerSave();
    }


    ////////////////////////////////////////////////
    /// AR camera relocalization when see marker ///
    ////////////////////////////////////////////////

    [SerializeField]
    GameObject m_MappingConfigurationUI;

    [SerializeField]
    UnityEngine.XR.ARFoundation.ARSessionOrigin m_ARSessionOrigin;

    bool isRelocateARCameraEnable;

    void GetRelocateARCameraCondition()
    {
        isRelocateARCameraEnable = m_MappingConfigurationUI
            .GetComponent<MappingConfigurationUI_CatExample>()
            .GetToggleRelocateARCamera();
    }

    void RelocateARCamera(GameObject groundtruthMarker, Transform trackedMarker)
    {
        GetRelocateARCameraCondition();
        if (!isRelocateARCameraEnable) return;

        // try relocate camera position only
        var gt_pos = groundtruthMarker.transform.position;
        var cr_pos = trackedMarker.position;
        var drift = gt_pos - cr_pos;

        var arCam = m_ARSessionOrigin.camera;
        var cam_pos = arCam.transform.position;

        var alt_cam_pos = cam_pos + drift;
        m_ARSessionOrigin.camera.transform.position = alt_cam_pos;

        Debug.Log("gt_pos  : " + gt_pos.ToString() + "\n" +
                  "cr_pos  : " + cr_pos.ToString() + "\n" +
                  "drift   : " + drift.ToString() + "\n" +
                  "cam_pos : " + cam_pos.ToString() + "\n" +
                  "alt_cam : " + alt_cam_pos.ToString() + "\n" +
                  "now_cam : " + m_ARSessionOrigin.camera.transform.position.ToString()
            );

        // turn off the camera relocating after found one
        m_MappingConfigurationUI
            .GetComponent<MappingConfigurationUI_CatExample>()
            .ToggleRelocateARCamera();
    }
}
