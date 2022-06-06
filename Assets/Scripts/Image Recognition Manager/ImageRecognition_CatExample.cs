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

    //[SerializeField]
    bool createAnchor = false;

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
                        Vector3 targetPos = -trackedImage.transform.position;
                        Quaternion targetRot = Quaternion.Inverse(trackedImage.transform.rotation);
                        //m_ARSessionOrigin.transform.position = targetPos;
                        //m_ARSessionOrigin.transform.rotation = targetRot;
                        //m_ARSessionOrigin.MakeContentAppearAt(trackedImage.transform,
                        //                                        trackedImage.transform.position,
                        //                                        trackedImage.transform.localRotation);

                        Debug.Log("trackedImage Rot before after:");
                        Debug.Log(trackedImage.transform.rotation.ToString());
                        Debug.Log(targetRot.ToString());

                        // I want to make a empty object with SAME ORIENTATION as trackedImage
                        GameObject origin = new GameObject("tempOrigin");
                        GlobalConfig.TempOriginGO = origin;
                        origin.transform.SetPositionAndRotation(trackedImage.transform.position, trackedImage.transform.rotation);

                        // This value will as same as our newly made origin
                        // RIGHT ?????
                        GlobalConfig.ITT_VtriPos = trackedImage.transform.position;
                        GlobalConfig.ITT_EAngleRot = trackedImage.transform.eulerAngles;
                        GlobalConfig.ITT_QuatRot = trackedImage.transform.rotation;

                        Debug.Log("origin transform:");
                        Debug.Log(origin.transform.position.ToString());
                        Debug.Log(origin.transform.rotation.ToString());

                        Debug.Log("Global confid transform:");
                        Debug.Log(GlobalConfig.ITT_VtriPos);
                        Debug.Log(GlobalConfig.ITT_EAngleRot);
                        Debug.Log(GlobalConfig.ITT_QuatRot);

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

                        // render MyOrigin data
                        //RenderMyOriginData(tImagePosition, tImageEulerRotation);

                        // render MyObject data
                        //RenderMyObjectData();

                        break;
                    }
                    else
                    {
                        // only update world coordinate if found the trackedImage again
                        GlobalConfig.ITT_VtriPos = trackedImage.transform.position;
                        GlobalConfig.ITT_EAngleRot = trackedImage.transform.eulerAngles;
                        GlobalConfig.ITT_QuatRot = trackedImage.transform.rotation;

                        //UpdateWorldCoordinate(tImagePosition, tImageEulerRotation);
                        //GlobalConfig.ITT_VtriPos = trackedImage.position;
                        //GlobalConfig.ITT_EAngleRot = trackedImage.eulerAngles;

                        break;
                    }
                }
            }
        }
    }

    private void RenderMyOriginData(Vector3 markerPos, Vector3 markerRot)
    {
        // import MyOrigin data from csv
        List<MyOrigin> myOrigins = Import_FromOrigin.GetMyOriginsList();

        // do foreach in csv data
        foreach (var item in myOrigins)
        {
            // if it's the root
            if (item.parent == "none")
            {
                GameObject gameObject = new(item.name);
                gameObject.transform.localPosition = markerPos + item.position;
                gameObject.transform.Rotate(markerRot + item.euler_rotation);

                // insert into parents so assigning parent will likely easier
                if (!CheckIfParentsExists(_parents, item.name)) { _parents.Add(gameObject); }

                // add into global config --> MyObjectList
                GlobalConfig.MyObjectList.Add(gameObject);

                // put into GlobalConfig as root parent
                GlobalConfig.PlaySpaceMyOrigin = item;
                GlobalConfig.PlaySpaceOriginGO = gameObject;

                // create anchor prefab
                if (createAnchor) { CreateWorldAnchor(gameObject); }
            }

            // if it's under root
            else
            {
                GameObject gameObject = new(item.name);

                // assign to each parents
                foreach (var parent in _parents)
                {
                    if (parent.name == item.parent)
                    {
                        gameObject.transform.parent = parent.transform;
                        break;
                    }
                }

                // assign the orientation
                gameObject.transform.localPosition = item.position;
                gameObject.transform.localRotation = Quaternion.identity;
                gameObject.transform.Rotate(item.euler_rotation);

                // insert into parents
                if (!CheckIfParentsExists(_parents, item.name))
                {
                    _parents.Add(gameObject);
                }

                // add into global config --> thingslist
                GlobalConfig.MyObjectList.Add(gameObject);

                // create anchor prefab
                if (createAnchor) { CreateWorldAnchor(gameObject); }
            }
        }
    }

    private void RenderMyObjectData()
    {
        // import MyObject data from csv
        List<MyObject> myObjects = Import_FromObject.GetMyObjectsList();

        // do foreach data in the csv
        foreach (var item in myObjects)
        {
            // initialize gameobject
            GameObject gameObject;
            string prefabtype = item.prefab_type;

            // choose gameobject type
            if (prefabtype == MyObject.PrefabType.CUBE) { gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube); }
            else if (prefabtype == MyObject.PrefabType.SPHERE) { gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere); }
            else if (prefabtype == MyObject.PrefabType.CYLINDER) { gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder); }
            else if (prefabtype == MyObject.PrefabType.SPECIAL) { gameObject = CreateSpecialPrefab(new GameObject(), item.prefab_special); }
            else { gameObject = new GameObject(); }

            // set gameobject name
            gameObject.name = item.name;

            // set gameobject parent
            foreach (var parent in _parents)
            {
                if (parent.name == item.parent)
                {
                    gameObject.transform.parent = parent.transform;
                    break;
                }
            }

            // calculate myObject origin
            MyObject.MyObject_LHW tempLHW = new MyObject.MyObject_LHW(item.length, item.height, item.width);
            tempLHW = OriginCalculator.Calculate(tempLHW, item.origin_type, item.origin_descriptor);
            Vector3 newOrigin = new(tempLHW.L, tempLHW.H, tempLHW.W);

            // assign orientation using the csv data transformation
            gameObject.transform.localPosition = newOrigin;
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(item.length, item.height, item.width);

            // insert into parents
            if (!CheckIfParentsExists(_parents, item.name))
            {
                _parents.Add(gameObject);
            }

            // add into global config --> thingslist
            GlobalConfig.MyObjectList.Add(gameObject);

            // assign ColorManager
            gameObject.AddComponent<ColorManager>();

            // assign DataManager
            gameObject.AddComponent<DataManager>();

            // START PLAYING COLOR DATA
            gameObject.GetComponent<DataManager>().testingOnly = true;
            gameObject.GetComponent<DataManager>().Test_AssignHiLoValue();
            StartCoroutine(Loop(gameObject));
        }
    }

    private bool CheckIfParentsExists(List<GameObject> parentsList, string parentName)
    {
        foreach (var parent in parentsList)
        {
            if (parent.name == parentName)
            {
                return true;
            }
        }

        return false;
    }

    private void CreateWorldAnchor(GameObject parent)
    {
        if (originPrefabInstantiate != null)
        {
            GameObject gameObject = Instantiate(originPrefabInstantiate);
            gameObject.transform.parent = parent.transform;
            gameObject.name = parent.name + "_prefab";

            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

            gameObject.SetActive(true);
        }
    }

    private GameObject CreateSpecialPrefab(GameObject gameObject, string prefab_special_path)
    {
        return gameObject;
    }

    private void UpdateWorldCoordinate(Vector3 markerPos, Vector3 markerRot)
    {
        GlobalConfig.PlaySpaceOriginGO.transform.position = markerPos + GlobalConfig.PlaySpaceMyOrigin.position;
        GlobalConfig.PlaySpaceOriginGO.transform.rotation = Quaternion.identity;
        GlobalConfig.PlaySpaceOriginGO.transform.Rotate(markerRot + GlobalConfig.PlaySpaceMyOrigin.euler_rotation);
        //GlobalConfig.OurWorldOrigin_MyOrigin_GameObject.transform.localScale = GlobalConfig.OurWorldOrigin_Things.scale.GetScale();
    }

    private void UpdatingColorManager(GameObject gameObject, float value)
    {
        if (gameObject.GetComponent<ColorManager>() == null) { return; }

        // get range value
        float hi = gameObject.GetComponent<DataManager>().GetHighestValue();
        float lo = gameObject.GetComponent<DataManager>().GetLowestValue();

        // apply color
        gameObject.GetComponent<ColorManager>().AssignHighLowAlpha(hi, lo, _alpha);
        gameObject.GetComponent<ColorManager>().UpdateColor(value);
    }

    private void UpdatingDataManager(GameObject gameObject)
    {
        if (gameObject.GetComponent<DataManager>() == null) { return; }

        // get previous value
        float previous_value = gameObject.GetComponent<DataManager>().GetCurrentValue();

        // THIS IS PLAY ONLY, YOU CAN CHANGE THIS REPRESENTATION
        if (previous_value == 0) { previous_value = 60.0f; }
        float next_value = gameObject.GetComponent<DataManager>().Test_GetDataUpdate(previous_value);

        // update color data
        UpdatingColorManager(gameObject, next_value);
    }

    //////////////////////////////
    // PLAYING COLOR DATA
    IEnumerator Loop(GameObject gO)
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            UpdatingDataManager(gO);
        }
    }
}
