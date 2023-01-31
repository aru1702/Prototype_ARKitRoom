using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class NewARSceneImageTrackingCorrection : MonoBehaviour
{
    [SerializeField]
    float m_TimeIntervalPerSecond = 1.0f;

    [SerializeField]
    bool m_EnableThisFunction = true;

    [Tooltip("To get the Image Tracked Manager database")]
    [SerializeField]
    ARSessionOrigin m_ARSessionOrigin;

    ARTrackedImageManager m_ARTrackedImageManager;

    List<CustomTransform> m_ImageTrackedList = new();
    List<CustomTransform> m_ImageTrackedListWithRemove = new();

    string m_NowMarkerTracked, m_PreviousMarkerTracked = "na";      // necessary for GlobalSaveData

    List<CustomTransform> m_MarkerList = new();

    bool m_HasUpdate = false;

    // Awake is called on app starts
    void Awake()
    {
        // Debug.Log("enableImgTrck: " + m_EnableImageTrackingCorrection);
        if (!m_EnableThisFunction) return;

        m_ARTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
    }

    // OnEnable is called when script is activated
    void OnEnable()
    {
        // Debug.Log("enableImgTrck: " + m_EnableImageTrackingCorrection);
        if (!m_EnableThisFunction) return;

        m_ARTrackedImageManager.trackedImagesChanged += OnImageChanged;

        //TestCustomDataAdd();
        //m_HasUpdate = true;
    }

    // OnDisable is called when script is deactivated
    void OnDisable()
    {
        m_ARTrackedImageManager.trackedImagesChanged -= OnImageChanged;

        //m_HasUpdate = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("enableImgTrck: " + m_EnableImageTrackingCorrection);
        if (!m_EnableThisFunction) return;

        // Debug.Log("Check ar session origin: " + (m_ARSessionOrigin!=null).ToString());
        if (m_ARSessionOrigin == null) return;

        var useCorrectionFunc = GlobalConfig.UseCorrectionMethod;
        // Debug.Log("useCorrection: " + useCorrectionFunc);
        if (!useCorrectionFunc) return;

        var trackingEnable = m_ARSessionOrigin.GetComponent<ARTrackedImageManager>();
        if (!trackingEnable.enabled)
        {
            trackingEnable.enabled = true;
        }
        //m_MarkerList = new();

        // get saved marker data from local
        //string map = GlobalConfig.LOAD_MAP.ToString();
        //string fileName = MappingV2.GetMarkerCalibrationFileName(map);
        //string path = System.IO.Path.Combine(Application.persistentDataPath, fileName);

        // get marker data from local
        //MarkerImportCsv mIC = new();
        //var markers = mIC.GetMarkerLocationsSummarized(path);

        // process into gameObject form
        //SavedMarkerToGameObjectList(markers);

        // start loop process
        StartCoroutine(LoopMain());

        // UPDATE: proceed plan version 1
    }

    // OnImageChanged is called when the a marker is captured by camera
    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        // an image is tracked/changed/extended (not tracked)
        foreach (var updatedImage in args.updated)
        {
            // initialization
            bool is_new_data = true;

            // if the tracked img become LIMITED --> remove, and no update
            if (string.Equals(updatedImage.trackingState.ToString(), "Limited"))
            {
                //    // Debug.Log("name: " + updatedImage.referenceImage.name + ", status: Limited");
                //    //if (m_ImageTrackedList.Count <= 0) return;

                //foreach (var marker in m_ImageTrackedListWithRemove)
                //{
                //    if (string.Equals(updatedImage.referenceImage.name, marker.custom_name))
                //    {
                //        m_ImageTrackedListWithRemove.Remove(marker);
                //    }
                //}

                m_PreviousMarkerTracked = updatedImage.referenceImage.name;
                m_HasUpdate = false;

                return;
            }
            // UPDATE: we don't need to remove the marker from list
            // UPDATE: enable "remove marker from list" from another list

            // if the tracked img become TRACKING --> add/update, and has update
            if (string.Equals(updatedImage.trackingState.ToString(), "Tracking"))
            {
                // for each the tracked imgs are already exist --> update location
                foreach (var img in m_ImageTrackedList)
                {
                    if (string.Equals(updatedImage.referenceImage.name, img.custom_name))
                    {
                        // Debug.Log("name: " + updatedImage.referenceImage.name + ", status: Already in list");

                        img.custom_position = updatedImage.transform.position;
                        img.customer_q_rotation = updatedImage.transform.rotation;
                        img.custom_euler_rotation = updatedImage.transform.rotation.eulerAngles;
                        is_new_data = false;
                    }
                }

                //foreach (var img in m_ImageTrackedListWithRemove)
                //{
                //    if (string.Equals(updatedImage.referenceImage.name, img.custom_name))
                //    {
                //        // Debug.Log("name: " + updatedImage.referenceImage.name + ", status: Already in list");

                //        img.custom_position = updatedImage.transform.position;
                //        img.customer_q_rotation = updatedImage.transform.rotation;
                //        is_new_data = false;
                //    }
                //}

                // if the tracked img is new data
                if (is_new_data)
                {
                    CustomTransform newImgTgt = new();

                    newImgTgt.custom_name = updatedImage.referenceImage.name;
                    newImgTgt.custom_position = updatedImage.transform.position;
                    newImgTgt.customer_q_rotation = updatedImage.transform.rotation;
                    newImgTgt.custom_euler_rotation = newImgTgt.customer_q_rotation.eulerAngles;

                    m_ImageTrackedList.Add(newImgTgt);

                    //m_ImageTrackedListWithRemove.Add(newImgTgt);
                    // Debug.Log("name: " + updatedImage.referenceImage.name + ", status: Added");
                }


                // save to GlobalSaveData
                if (!m_HasUpdate)
                {
                    SavingToGlobalSaveData(new CustomTransform(
                        updatedImage.referenceImage.name,
                        updatedImage.transform.position,
                        updatedImage.transform.rotation.eulerAngles,
                        updatedImage.transform.rotation)

                        , updatedImage.transform);
                }
            }

            m_NowMarkerTracked = updatedImage.referenceImage.name;
            m_HasUpdate = true;
        }
    }

    // Call in while(true) with m_TimeIntervalPerSecond
    IEnumerator LoopMain()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_TimeIntervalPerSecond);
            // Debug.Log("Corountine on loop");

            //Main();

            //Debug.Log("update status: " + m_HasUpdate);
            //Debug.Log("marker in list: " + m_ImageTrackedList.Count);
        }
    }

    // Main function
    void Main()
    {
        if (m_ImageTrackedList.Count <= 0) return;

        // Debug.Log("Main with img tracked count: " + m_ImageTrackedList.Count);

        foreach (var imageTracked in m_ImageTrackedList)
        {
            var marker = GetCustomTransformByName(imageTracked.custom_name, m_MarkerList);
            // Debug.Log("marker info: " + marker.custom_name + ", " + marker.custom_position.ToString());

            GameObject tempGo = new();
            tempGo.transform.position = imageTracked.custom_position;
            var m44 = GlobalConfig.GetM44ByGameObjRef(tempGo, GlobalConfig.PlaySpaceOriginGO);
            var new_marker_position = GlobalConfig.GetPositionFromM44(m44);

            Debug.Log("marker saved position: " + marker.custom_position.ToString() +
                    "\nmarker current position: " + new_marker_position.ToString());

            //Vector3 diffVector = marker.custom_position - new_marker_position;
            Vector3 diffVector = new_marker_position - marker.custom_position;
            //Debug.Log("current: " + diffVector.ToString());
            //Debug.Log("previous: " + (marker.custom_position - new_marker_position).ToString());

            // change root position
            var root = GlobalConfig.PlaySpaceOriginGO;
            var root_pre_pos = root.transform.localPosition;
            root.transform.localPosition += diffVector;

            Debug.Log("root previous position: " + root_pre_pos.ToString() +
                    "\nroot now position: " + root.transform.position.ToString());

            // end the function
            return;
        }
    }

    void SavedMarkerToGameObjectList(List<MarkerImportCsv.MarkerLocation> markerList)
    {
        foreach (var marker in markerList)
        {
            CustomTransform tempObj = new();
            tempObj.custom_name = marker.name;
            tempObj.custom_position = marker.C_Position;
            tempObj.custom_euler_rotation = marker.C_EulerAngle;

            // Debug.Log("name: " + tempObj.custom_name);
            m_MarkerList.Add(tempObj);
        }
    }

    public CustomTransform GetCustomTransformByName(string name, List<CustomTransform> list)
    {
        // Debug.Log("imageTracked name: " + name);
        foreach (var item in list)
        {
            // Debug.Log("marker name: " + item.custom_name);
            if (string.Equals(name, item.custom_name)) return item;
        }

        return null;
    }

    public List<CustomTransform> GetImageTrackedList()
    {
        return m_ImageTrackedList;
    }

    public List<CustomTransform> GetImageTrackedListWithRemove()
    {
        return m_ImageTrackedListWithRemove;
    }

    public bool GetImageTargetUpdateStatus()
    {
        return m_HasUpdate;
    }

    void TestCustomDataAdd()
    {
        CustomTransform newImgTgt = new();
        newImgTgt.custom_name = "img_1";
        newImgTgt.custom_position = new(3.474f, -0.788f, -0.781f);
        newImgTgt.customer_q_rotation = new();
        newImgTgt.custom_euler_rotation = new();
        m_ImageTrackedList.Add(newImgTgt);
    }

    public void TestInputData(CustomTransform customTransform)
    {
        m_ImageTrackedList.Add(customTransform);
        m_NowMarkerTracked = customTransform.custom_name;
    }

    public void TestInputDataRemove(CustomTransform customTransform)
    {
        m_ImageTrackedListWithRemove.Add(customTransform);
    }

    public void UpdateHasUpdate(bool trigger)
    {
        m_HasUpdate = trigger;
    }

    public void UpdateInputData(string name, Vector3 vector)
    {
        foreach (var item in m_ImageTrackedList)
        {
            if(Equals(name, item.custom_name))
            {
                item.custom_position = vector;
                m_PreviousMarkerTracked = m_NowMarkerTracked;
                m_NowMarkerTracked = item.custom_name;                
            }
        }

        foreach (var item in m_ImageTrackedListWithRemove)
        {
            if (Equals(name, item.custom_name))
            {
                item.custom_position = vector;
                m_PreviousMarkerTracked = m_NowMarkerTracked;
                m_NowMarkerTracked = item.custom_name;
            }
        }
    }

    public void ResetImageTrackedListWithRemove()
    {
        m_ImageTrackedListWithRemove.Clear();
    }

    /// <summary>
    /// Get string of currently tracked image target.
    /// </summary>
    public string GetNowMarkerTracked()
    {
        return m_NowMarkerTracked;
    }

    /// <summary>
    /// Get string of previous tracked image target.
    /// Previous image target will be same as the current after camera no longer
    /// looking at the image target.
    /// </summary>
    public string GetPreviousMarkerTracked()
    {
        return m_PreviousMarkerTracked;
    }

    /// <summary>
    /// Get string of currently and previous tracked image target.
    /// Previous image target will be same as the current after camera no longer
    /// looking at the image target.
    /// </summary>
    public string[] GetNowAndPrevMarkerTracked()
    {
        string[] s = new string[2];
        s[0] = GetNowMarkerTracked();
        s[1] = GetPreviousMarkerTracked();
        return s;
    }

    /// <summary>
    /// Save into GlobalSaveData.
    /// </summary>
    void SavingToGlobalSaveData(CustomTransform marker, Transform raw_marker_t)
    {
        string[] data =
        {
            GlobalConfig.GetNowDateandTime(true),
            marker.custom_name + "_raw",
            marker.custom_position.x.ToString(),
            marker.custom_position.y.ToString(),
            marker.custom_position.z.ToString(),
            marker.custom_euler_rotation.x.ToString(),
            marker.custom_euler_rotation.y.ToString(),
            marker.custom_euler_rotation.z.ToString(),
            marker.customer_q_rotation.x.ToString(),
            marker.customer_q_rotation.y.ToString(),
            marker.customer_q_rotation.z.ToString(),
            marker.customer_q_rotation.z.ToString(),
            m_PreviousMarkerTracked
        };

        GlobalSaveData.WriteData(data);

        // another one with to world
        var new_m44 = GlobalConfig.GetM44ByGameObjRef(raw_marker_t, GlobalConfig.PlaySpaceOriginGO);
        string[] new_data =
        {
            GlobalConfig.GetNowDateandTime(true),
            marker.custom_name + "_byworld",
            GlobalConfig.GetPositionFromM44(new_m44).x.ToString(),
            GlobalConfig.GetPositionFromM44(new_m44).y.ToString(),
            GlobalConfig.GetPositionFromM44(new_m44).z.ToString(),
            GlobalConfig.GetEulerAngleFromM44(new_m44).x.ToString(),
            GlobalConfig.GetEulerAngleFromM44(new_m44).y.ToString(),
            GlobalConfig.GetEulerAngleFromM44(new_m44).z.ToString(),
            GlobalConfig.GetRotationFromM44(new_m44).x.ToString(),
            GlobalConfig.GetRotationFromM44(new_m44).y.ToString(),
            GlobalConfig.GetRotationFromM44(new_m44).z.ToString(),
            GlobalConfig.GetRotationFromM44(new_m44).z.ToString(),
            m_PreviousMarkerTracked
        };

        GlobalSaveData.WriteData(new_data);
    }
}
