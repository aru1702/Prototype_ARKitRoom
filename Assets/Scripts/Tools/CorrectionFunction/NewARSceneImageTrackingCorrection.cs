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

    List<CustomTransform> m_ImageTrackedList;
    List<CustomTransform> m_MarkerList;

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

        m_ImageTrackedList = new();
        m_MarkerList = new();

        // get saved marker data from local
        string map = GlobalConfig.LOAD_MAP.ToString();
        string fileName = MappingV2.GetMarkerCalibrationFileName(map);
        string path = System.IO.Path.Combine(Application.persistentDataPath, fileName);

        MarkerImportCsv mIC = new();
        var markers = mIC.GetMarkerLocationsSummarized(path);

        // Debug.Log("Marker info: " + markers.Count);
        // Debug.Log("SavedMarkerToGameObjectList");
        SavedMarkerToGameObjectList(markers);

        // Debug.Log("StartCoroutine(LoopMain()");
        StartCoroutine(LoopMain());
    }

    // OnImageChanged is called when the a marker is captured by camera
    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        // an image is tracked/changed/extended (not tracked)
        foreach (var updatedImage in args.updated)
        {
            // if the tracked img become LIMITED --> remove from array
            if (string.Equals(updatedImage.trackingState.ToString(), "Limited"))
            {
                // Debug.Log("name: " + updatedImage.referenceImage.name + ", status: Limited");
                //if (m_ImageTrackedList.Count <= 0) return;

                foreach (var marker in m_ImageTrackedList)
                {
                    if (string.Equals(updatedImage.referenceImage.name, marker.custom_name))
                    {
                        m_ImageTrackedList.Remove(marker);
                        return;
                    }
                }
            }

            // for each the tracked imgs are already exist --> update location
            foreach (var marker in m_ImageTrackedList)
            {
                if (string.Equals(updatedImage.referenceImage.name, marker.custom_name))
                {
                    // Debug.Log("name: " + updatedImage.referenceImage.name + ", status: Already in list");

                    marker.custom_position = updatedImage.transform.position;
                    marker.customer_q_rotation = updatedImage.transform.rotation;

                    return;
                }
            }

            // if the tracked img become TRACKING --> add to array
            if (string.Equals(updatedImage.trackingState.ToString(), "Tracking"))
            {
                CustomTransform newImgTgt = new();
                newImgTgt.custom_name = updatedImage.referenceImage.name;
                newImgTgt.custom_position = updatedImage.transform.position;
                newImgTgt.customer_q_rotation = updatedImage.transform.rotation;
                m_ImageTrackedList.Add(newImgTgt);

                // Debug.Log("name: " + updatedImage.referenceImage.name + ", status: Added");
            }
        }
    }

    // Call in while(true) with m_TimeIntervalPerSecond
    IEnumerator LoopMain()
    {
        while(true)
        {
            yield return new WaitForSeconds(m_TimeIntervalPerSecond);
            // Debug.Log("Corountine on loop");
            Main();
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

    // OnDisable is called when script is deactivated
    void OnDisable()
    {
        m_ARTrackedImageManager.trackedImagesChanged -= OnImageChanged;
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

    CustomTransform GetCustomTransformByName(string name, List<CustomTransform> list)
    {
        // Debug.Log("imageTracked name: " + name);
        foreach (var item in list)
        {
            // Debug.Log("marker name: " + item.custom_name);
            if (string.Equals(name, item.custom_name)) return item;
        }

        return null;
    }
}