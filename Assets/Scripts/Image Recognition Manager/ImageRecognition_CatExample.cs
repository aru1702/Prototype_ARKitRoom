using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageRecognition_CatExample : MonoBehaviour
{
    private const string _imgSource_name = "cat_example";
    private ARTrackedImageManager _arTrackedImageManager;
    private List<GameObject> _parents = new();
    private const float _alpha = 0.3f;

    //[SerializeField]
    GameObject originPrefabInstantiate;

    [SerializeField]
    ARSessionOrigin m_ARSessionOrigin;

    [SerializeField]
    GameObject LoadObjectManager;

    [SerializeField]
    GameObject WorldMapManager;

    [SerializeField]
    GameObject CanvasCat, HoldStillText;

    /**
     * Default methods
     */
    private void Awake() { _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>(); }

    private void OnEnable() {
        _arTrackedImageManager.trackedImagesChanged += OnImageChanged;

        Debug.Log("ImgRecog active");

        // if this is active then disable world map manager first
        // long cast name, because namespace, no question
        WorldMapManager
            .GetComponent<UnityEngine.XR.ARFoundation.Samples.WorldMap_CatExample>()
            .enabled = false;

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

                        //// set our origin into GlobalConfig
                        //GlobalConfig.ITT_VtriPos = trackedImage.position;
                        //GlobalConfig.ITT_EAngleRot = trackedImage.eulerAngles;
                        //GlobalConfig.ITT_QuatRot = trackedImage.rotation;

                        // change ARSessionOrigin position to trackedImage origin
                        //Vector3 targetPos = -trackedImage.transform.position;
                        //Quaternion targetRot = Quaternion.Inverse(trackedImage.transform.rotation);

                        //m_ARSessionOrigin.transform.position = targetPos;
                        //m_ARSessionOrigin.transform.rotation = targetRot;
                        //m_ARSessionOrigin.MakeContentAppearAt(trackedImage.transform,
                        //                                        trackedImage.transform.position,
                        //                                        trackedImage.transform.localRotation);

                        //Debug.Log("trackedImage Rot before after:");
                        //Debug.Log(trackedImage.transform.rotation.ToString());
                        //Debug.Log(targetRot.ToString());

                        // I want to make a empty object with SAME ORIENTATION as trackedImage
                        // THIS IS IMPORTANT DO NOT DELETE
                        GameObject origin = new GameObject("tempOrigin");
                        GlobalConfig.TempOriginGO = origin;
                        origin.transform.SetPositionAndRotation(trackedImage.transform.position, trackedImage.transform.rotation);

                        // This value will as same as our newly made origin
                        // RIGHT ?????
                        // THIS IS IMPORTANT DO NOT DELETE
                        GlobalConfig.ITT_VtriPos = trackedImage.transform.position;
                        GlobalConfig.ITT_EAngleRot = trackedImage.transform.eulerAngles;
                        GlobalConfig.ITT_QuatRot = trackedImage.transform.rotation;

                        //Debug.Log("origin transform:");
                        //Debug.Log(origin.transform.position.ToString());
                        //Debug.Log(origin.transform.rotation.ToString());

                        //Debug.Log("Global confid transform:");
                        //Debug.Log(GlobalConfig.ITT_VtriPos);
                        //Debug.Log(GlobalConfig.ITT_EAngleRot);
                        //Debug.Log(GlobalConfig.ITT_QuatRot);

                        // active the cat script on LoadObjectManager
                        LoadObjectManager
                            .GetComponent<LoadObject_CatExample>()
                            .enabled = true;

                        // active again the world map script
                        WorldMapManager
                            .GetComponent<UnityEngine.XR.ARFoundation.Samples.WorldMap_CatExample>()
                            .enabled = true;

                        // deactive canvas
                        CanvasCat.SetActive(false);

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

                        Debug.Log(string.Format("In imageRecog\n\nPos: {0}\nRot: {1}",
                            GlobalConfig.ITT_VtriPos,
                            GlobalConfig.ITT_QuatRot));

                        break;
                    }
                }
            }
        }
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(3);
    }
}
