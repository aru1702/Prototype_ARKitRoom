using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// To get ground truth (GT) and runtime (RT)
/// To get position (Vector3) and rotation (Vector3, Quaternion)
/// To get time between marker recognition (from start, from last marker)
/// To get distance of camera trajector between marker recognition (from start, from last marker)
/// </summary>
public class CMOMMarkerData : MonoBehaviour
{
    /// <summary>
    /// CM: calibration mode, OM: observation mode
    /// </summary>
    [SerializeField]
    bool m_ActiveForCM, m_ActiveForOM;

    /// <summary>
    /// AR Session Origin to retrieve AR Foundation data
    /// </summary>
    [SerializeField]
    ARSessionOrigin m_ARSessionOrigin;

    /// <summary>
    /// AR Camera gameObject, to get trajectory
    /// </summary>
    [SerializeField]
    GameObject m_ARCamera;

    /// <summary>
    /// To get ground truth data, input of CM or OM is different
    /// </summary>
    [SerializeField]
    GameObject m_LoadObjectManager;

    /// <summary>
    /// Camera update the trajectory distance and time per period (in seconds)
    /// </summary>
    [SerializeField]
    float m_CameraUpdatePeriod = 1.0f;


    // Fields
    List<GameObject> GTMarkerData;
    string CurrentMarkerName = "na", PreviousMarkerName = "na";
    float CamDistFromStart, CamDistFromLastMarker;
    float CamTimeFromStart, CamTimeFromLastMarker;
    Hashtable MarkerTableData;
    int MarkerTableCount = 0;
    ARTrackedImageManager ArTrackedImageManager;
    string RecognitionStatus = "Limited";


    // Saved data
    List<string[]> SavedData;


    // Others
    bool Flag_LoadObjectManagerHasLoaded;
    bool Flag_MarkerInRecognitionProcess;
    const string STATUS_TRACKING = "Tracking";
    const string STATUS_LIMITED = "Limited";
    Vector3 PreviousCameraPosition;


    private void Awake() { ArTrackedImageManager = FindObjectOfType<ARTrackedImageManager>(); }

    private void OnEnable() { ArTrackedImageManager.trackedImagesChanged += OnImageChanged; }

    private void OnDisable() { ArTrackedImageManager.trackedImagesChanged -= OnImageChanged; }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var newImage in args.added)
        {
            // Handle new event
        }

        // this method always updated per image recognition system
        foreach (var updatedImage in args.updated)
        {
            // if the tracked img become LIMITED --> remove from array
            if (string.Equals(updatedImage.trackingState.ToString(), STATUS_LIMITED))
            {
                RecognitionStatus = STATUS_LIMITED;
                PreviousMarkerName = CurrentMarkerName;
                ResetCameraBehavior();
            }

            // if the tracked img become TRACKING --> add to array
            if (string.Equals(updatedImage.trackingState.ToString(), STATUS_TRACKING))
            {
                //if (RecognitionStatus == STATUS_LIMITED)
                //{

                //}

                RecognitionStatus = STATUS_TRACKING;
                CurrentMarkerName = updatedImage.referenceImage.name;
                UpdateGTMarkerData(CurrentMarkerName);

                int index = (int)MarkerTableData[CurrentMarkerName];
                //Debug.Log("index: " + index);

                var GTData = GTMarkerData[index];

                // current data with world coordinate ref
                var RTMat = GlobalConfig.GetM44ByGameObjRef(updatedImage.transform, GlobalConfig.PlaySpaceOriginGO);
                var RTPos = GlobalConfig.GetPositionFromM44(RTMat);
                var RTRot = GlobalConfig.GetRotationFromM44(RTMat);

                SaveData(GTData, RTPos, RTRot, CurrentMarkerName, PreviousMarkerName);
            }
        }

        foreach (var removedImage in args.removed)
        {
            // Handle removed event
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GTMarkerData = new List<GameObject>();
        SavedData = new List<string[]>();
        MarkerTableData = new Hashtable();

        if (m_ActiveForOM) m_ARSessionOrigin.gameObject.GetComponent<ARTrackedImageManager>().enabled = true;

        SaveDataHeader();
        StartCoroutine(LoopMain());
    }

    IEnumerator LoopMain()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_CameraUpdatePeriod);
            UpdateCameraBehaviorDistance();
            UpdateCameraBehaviorTime(true);

            //Debug.Log("current marker count: " + MarkerTableCount);
            //Debug.Log("current gt data count: " + GTMarkerData.Count);
        }
    }

    /// <summary>
    /// Flow:
    /// - Check if loadObjectManager count is not zero, trigger bool flag
    /// - Check if the recognition marker is already in list, using Hashtable
    ///   - If not, get from loadObjectManager, add to list
    ///   - If yes, check in Hashtable then get the id, get from list
    /// - Count camera trajectory data (time, distance) per period
    /// </summary>

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateGTMarkerData(string cur_marker_name)
    {
        //Debug.Log("UpdateGTMarkerData");
        // empty or no related data
        if (!MarkerTableData.ContainsKey(cur_marker_name))
        {
            //Debug.Log("no marker data");

            // for calibration mode
            if (m_ActiveForCM)
            {
                //Debug.Log("CM data");
                var load_obj = m_LoadObjectManager.GetComponent<LoadObject_CatExample_2>();
                var all_objs = load_obj.GetMyParents();

                // find the same name
                foreach (var o in all_objs)
                {
                    if (o.name == cur_marker_name)
                    {
                        //Debug.Log("found: " + o.name);

                        // add recognition data to hashtable
                        MarkerTableData.Add(cur_marker_name, MarkerTableCount);
                        MarkerTableCount++;

                        // add object data to list
                        GTMarkerData.Add(o);

                        break;
                    }
                }
            }

            // for observation mode
            if (m_ActiveForOM)
            {
                //Debug.Log("OM data");
                var load_obj = m_LoadObjectManager.GetComponent<LoadObject_CatExample_2__NewARScene>();
                var all_objs = load_obj.GetMyParents();

                // find the same name
                foreach (var o in all_objs)
                {
                    if (o.name == cur_marker_name)
                    {
                        //Debug.Log("found: " + o.name);

                        // add recognition data to hashtable
                        MarkerTableData.Add(cur_marker_name, MarkerTableCount);
                        MarkerTableCount++;

                        // add object data to list
                        GTMarkerData.Add(o);

                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// This function can be called per any period
    /// </summary>
    void UpdateCameraBehaviorDistance()
    {
        var distance = Vector3.Distance(PreviousCameraPosition, m_ARCamera.transform.position);

        CamDistFromStart += distance;
        CamDistFromLastMarker += distance;
    }

    /// <summary>
    /// This function to update time per camera movement with two methods.
    /// If use_delta_time is true, called it per frame, e.g., inside Update()
    /// If use_delta_time is false, you can call it in any period
    /// </summary>
    /// <param name="use_delta_time">Use per delta time or per any period</param>
    void UpdateCameraBehaviorTime(bool use_delta_time = true)
    {
        if (use_delta_time)
        {
            var time = Time.deltaTime;

            CamTimeFromStart += time;
            CamTimeFromLastMarker += time;
        }

        else
        {
            var current_time = Time.time;
            var spend = Mathf.Abs(current_time - previous_time);

            CamTimeFromStart += spend;
            CamTimeFromLastMarker += spend;

            previous_time = current_time;
        }
    }

    float previous_time;


    void ResetCameraBehavior()
    {
        CamDistFromLastMarker = 0;
        CamTimeFromLastMarker = 0;
    }

    void SaveDataHeader()
    {
        string[] data =
        {
            "timestamp",

            "marker_name",

            "gt_pos_x",
            "gt_pos_y",
            "gt_pos_z",

            "gt_eul_x",
            "gt_eul_y",
            "gt_eul_z",

            "gt_rot_x",
            "gt_rot_y",
            "gt_rot_z",
            "gt_rot_w",

            "rt_pos_x",
            "rt_pos_y",
            "rt_pos_z",

            "rt_eul_x",
            "rt_eul_y",
            "rt_eul_z",

            "rt_rot_x",
            "rt_rot_y",
            "rt_rot_z",
            "rt_rot_w",

            "pre_marker_name",

            "cam_dist_start",
            "cam_dist_last_marker",
            "cam_time_start",
            "cam_time_last_marker",
        };

        SavedData.Add(data);
    }

    void SaveData(GameObject gt, Vector3 rt_pos, Quaternion rt_rot, string cur_name, string pre_name)
    {
        var gt_mat = GlobalConfig.GetM44ByGameObjRef(gt, GlobalConfig.PlaySpaceOriginGO);
        var gt_pos = GlobalConfig.GetPositionFromM44(gt_mat);
        var gt_rot = GlobalConfig.GetRotationFromM44(gt_mat);
        var gt_eul = gt_rot.eulerAngles;
        var rt_eul = rt_rot.eulerAngles;

        string[] data =
        {
            GlobalConfig.GetNowDateandTime(true),

            cur_name,

            gt_pos.x.ToString(),
            gt_pos.y.ToString(),
            gt_pos.z.ToString(),

            gt_eul.x.ToString(),
            gt_eul.y.ToString(),
            gt_eul.z.ToString(),

            gt_rot.x.ToString(),
            gt_rot.y.ToString(),
            gt_rot.z.ToString(),
            gt_rot.w.ToString(),

            rt_pos.x.ToString(),
            rt_pos.y.ToString(),
            rt_pos.z.ToString(),

            rt_eul.x.ToString(),
            rt_eul.y.ToString(),
            rt_eul.z.ToString(),

            rt_rot.x.ToString(),
            rt_rot.y.ToString(),
            rt_rot.z.ToString(),
            rt_rot.w.ToString(),

            pre_name,

            CamDistFromStart.ToString(),
            CamDistFromLastMarker.ToString(),
            CamTimeFromStart.ToString(),
            CamTimeFromLastMarker.ToString(),
        };

        SavedData.Add(data);
    }

    public void SaveDataCSV()
    {
        // save nothing if there is no data
        if (SavedData.Count <= 0) return;

        string time = GlobalConfig.GetNowDateandTime(true);
        string map = GlobalConfig.SAVE_INTO_MAP.ToString();

        // for data process
        string fileName = time + "_";
        if (m_ActiveForCM) fileName += "CM_MarkerData_";
        if (m_ActiveForOM) fileName += "OM_MarkerData_";
        fileName += "map_" + map + ".csv";

        string path = System.IO.Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, SavedData);
    }
}
