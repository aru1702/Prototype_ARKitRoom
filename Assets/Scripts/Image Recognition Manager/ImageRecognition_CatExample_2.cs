using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageRecognition_CatExample_2 : MonoBehaviour
{
    private const string _imgSource_name = "cat_example";
    private ARTrackedImageManager _arTrackedImageManager;

    [SerializeField]
    ARSessionOrigin m_ARSessionOrigin;

    [SerializeField]
    GameObject CanvasCat;

    [SerializeField]
    GameObject m_OriginPrefab;

    [SerializeField]
    GameObject m_LoadObjectManager;

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
    }

    private void OnDisable() { _arTrackedImageManager.trackedImagesChanged -= OnImageChanged; }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var newImage in args.added)
        {
            // Handle new event
        }

        foreach (var updatedImage in args.updated)
        {
            // Handle updated event
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
        if (_arTrackedImageManager.trackables.count > 0)
        {
            foreach (var trackedImage in _arTrackedImageManager.trackables)
            {
                // check if trackedImage name is same
                // otherwise it is useless
                if (trackedImage.referenceImage.name != _imgSource_name) { return; }
                else
                {
                    // wait for 3 seconds
                    //HoldStillText.SetActive(true);
                    //StartCoroutine(CountDown());

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
}
