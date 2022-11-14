using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageRecognition_CatExample_2 : MonoBehaviour
{
    [SerializeField]
    string _imgSource_name;

    private ARTrackedImageManager _arTrackedImageManager;

    [SerializeField]
    ARSessionOrigin m_ARSessionOrigin;

    [SerializeField]
    GameObject CanvasCat, m_TransferSLAMOriginBtn;

    [SerializeField]
    GameObject m_OriginPrefab;

    [SerializeField]
    GameObject m_LoadObjectManager;

    [SerializeField]
    bool m_TransferSLAMOrigin = false;

    public List<CustomImgTarget> m_ImageTargetsTransform;

    /**
     * Default methods
     */
    private void Awake() { _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>(); }

    private void OnEnable() {
        _arTrackedImageManager.trackedImagesChanged += OnImageChanged;

        Debug.Log("ImgRecog active");

        // check if cat image recognition has turned on
        bool imageRecog_enable = m_ARSessionOrigin.GetComponent<ARTrackedImageManager>().enabled;
        if (!imageRecog_enable)
        {
            m_ARSessionOrigin.GetComponent<ARTrackedImageManager>().enabled = true;
        }

        CanvasCat.SetActive(true);

        m_ImageTargetsTransform = new();
    }

    private void OnDisable() { _arTrackedImageManager.trackedImagesChanged -= OnImageChanged; }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var newImage in args.added)
        {
            // Handle new event
        }

        // this method always updated per image recognition system
        foreach (var updatedImage in args.updated)
        {
            // DEBUGGING
            //Debug.Log("name: " + trackedImage.referenceImage.name);
            //Debug.Log("updatedImage name: " + updatedImage.referenceImage.name +
            //          "\nupdatedImage loc: " + updatedImage.transform.position.ToString());
            //Debug.Log("ref: " + updatedImage.referenceImage.name);

            // if the tracked img become LIMITED --> remove from array
            if (string.Equals(updatedImage.trackingState.ToString(), STATUS_LIMITED))
            {
                foreach (var marker in m_ImageTargetsTransform)
                {
                    if (string.Equals(updatedImage.referenceImage.name, marker.name))
                    {
                        m_ImageTargetsTransform.Remove(marker);
                        return;
                    }
                }
            }

            //if (updatedImage.referenceImage.name != _imgSource_name)
            //{

            // for each the tracked imgs are already exist, no action taken
            foreach (var marker in m_ImageTargetsTransform)
            {
                if (string.Equals(updatedImage.referenceImage.name, marker.name)) return;
            }

            // if the tracked img become TRACKING --> add to array
            if (string.Equals(updatedImage.trackingState.ToString(), STATUS_TRACKING))
            {
                CustomImgTarget newImgTgt = new(
                    updatedImage.referenceImage.name,
                    updatedImage.transform);

                m_ImageTargetsTransform.Add(newImgTgt);                 
            }
            
            //}
        }

        foreach (var removedImage in args.removed)
        {
            // Handle removed event
        }
    }

    // TODO:
    // - anchor and any trackables should be saved into Ar Session
    // - with save & load we will not require the trackable image again
    void Update()
    {
        // check if camera already found trackable image
        // in this case we use only 1 image but default ARFoundation use List<>

        // DEBUGGING
        //Debug.Log("count: " + _arTrackedImageManager.trackables.count);

        if (_arTrackedImageManager.trackables.count > 0)
        {
            foreach (var trackedImage in _arTrackedImageManager.trackables)
            {

                // DEBUGGING
                //Debug.Log("name: " + trackedImage.referenceImage.name);
                //Debug.Log("TrackedImg name: " + trackedImage.referenceImage.name +
                //          "\nTrackedImg loc: " + trackedImage.transform.position.ToString());

                //Debug.Log("justTransform name: " + trackedImage.referenceImage.name +
                //          "\njustTransform loc: " + transform.position.ToString());

                // only the one that the name == the imgsource_name (aka. WORLD ORIGIN REF)
                if (trackedImage.referenceImage.name != _imgSource_name) { return; }
                else
                {
                    // wait for 3 seconds
                    //HoldStillText.SetActive(true);
                    //StartCoroutine(CountDown());

                    // DEBUGGING
                    //Debug.Log("render?: " + GlobalConfig.AlreadyRender);
                    //Debug.Log("TempOriginGO: " + GlobalConfig.TempOriginGO == null);

                    // check if first time rendering already done
                    if (!GlobalConfig.AlreadyRender)
                    {
                        // ONLY DO THIS SECTION ONCE AS LONG THE APP NOT CLOSED
                        // by this, any script won't trigger LoadObject or any load
                        // because already performed by this script
                        GlobalConfig.AlreadyRender = true;

                        // I want to make a empty object with SAME ORIENTATION as trackedImage
                        // THIS IS IMPORTANT DO NOT DELETE
                        GameObject origin = Instantiate(m_OriginPrefab, trackedImage.transform);
                        GlobalConfig.TempOriginGO = origin;

                        // This value will as same as our newly made origin
                        // RIGHT ?????
                        // THIS IS IMPORTANT DO NOT DELETE
                        GlobalConfig.ITT_VtriPos = trackedImage.transform.position;
                        GlobalConfig.ITT_EAngleRot = trackedImage.transform.eulerAngles;
                        GlobalConfig.ITT_QuatRot = trackedImage.transform.rotation;

                        // deactive canvas
                        CanvasCat.SetActive(false);

                        // show btn
                        //m_TransferSLAMOriginBtn.SetActive(true);

                        // activate LoadObjectManager
                        if (m_LoadObjectManager) m_LoadObjectManager.SetActive(true);

                        break;
                    }
                    else
                    {
                        // only update world coordinate if found the trackedImage again
                        GlobalConfig.ITT_VtriPos = trackedImage.transform.position;
                        GlobalConfig.ITT_EAngleRot = trackedImage.transform.eulerAngles;
                        GlobalConfig.ITT_QuatRot = trackedImage.transform.rotation;

                        GlobalConfig.TempOriginGO.transform.SetPositionAndRotation(
                            GlobalConfig.ITT_VtriPos,
                            GlobalConfig.ITT_QuatRot);

                        //Debug.Log("TempOriginGO loc: " + GlobalConfig.TempOriginGO.transform.position.ToString());

                        break;
                    }
                }
            }
        }
    }

    public void HideCanvas()
    {
        CanvasCat.SetActive(false);
    }

    public void TransferSLAMOrigin()
    {
        // hide button
        m_TransferSLAMOriginBtn.SetActive(false);

        // transfer SLAM origin into cat
        GameObject self = gameObject;   // this means get the attached-by-script gameobject 
        self.AddComponent<ImageRecognition_CatExample_2__TransferSLAMOrigin>();
        self.GetComponent<ImageRecognition_CatExample_2__TransferSLAMOrigin>()
            .transferSLAMOrigin = m_TransferSLAMOrigin;
        self.GetComponent<ImageRecognition_CatExample_2__TransferSLAMOrigin>()
            .desireOriginGameObject = GlobalConfig.TempOriginGO;
        self.GetComponent<ImageRecognition_CatExample_2__TransferSLAMOrigin>()
            .arSessionOrigin = m_ARSessionOrigin;
        self.GetComponent<ImageRecognition_CatExample_2__TransferSLAMOrigin>()
            .TransferNow();
    }

    /// <summary>
    /// Get currently tracked Image Target transform based to world coordinate
    /// </summary>
    /// <returns></returns>
    public List<CustomImgTarget> GetAllImageTargetsTranform()
    {
        // only return the transform, not the class

        //List<Transform> imgTgtTransform = new();

        //foreach (var item in m_ImageTargetsTransform)
        //{
        //    imgTgtTransform.Add(item.transformObj);
        //}

        //return imgTgtTransform;

        return m_ImageTargetsTransform;
    }

    const string STATUS_TRACKING = "Tracking";
    const string STATUS_LIMITED = "Limited";

    public class CustomImgTarget
    {
        public string name { get; set; }
        public Transform transformObj { get; set; }

        public CustomImgTarget(string name, Transform transformObj)
        {
            this.name = name;
            this.transformObj = transformObj;
        }
    }
}
