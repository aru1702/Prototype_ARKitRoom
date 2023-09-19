using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// TODO:
/// - In next project, I want to make this fetch data from online
/// - We use Google Drive as simple remote storage
/// - If success, then online sharing is capable
public class LoadObject_CatExample_2__NewARScene : MonoBehaviour
{
    List<GameObject> _parents = new();
    List<GameObject> _objects = new();

    List<GameObject> m_ObjectsGroundTruth = new();

    [SerializeField, TextArea(2,5)]
    string m_MyOriginURL, m_MyObjectURL;

    [SerializeField]
    float _alpha = 0.3f;

    [SerializeField]
    bool createAnchor = false;

    [SerializeField]
    GameObject originPrefabInstantiate;

    [SerializeField]
    GameObject specialPrefabList;

    [SerializeField]
    bool dontUseZBuffer = false;

    [SerializeField]
    GameObject m_CorrectionFunctionManager;

    Vector3 pos, rot;
    Quaternion rotQ;

    GameObject originChild;

    [SerializeField]
    GameObject m_ShowTextAboveLocation;

    void OnEnable()
    {
        Debug.Log("LoadObj active");

        pos = GlobalConfig.ITT_VtriPos;
        rot = GlobalConfig.ITT_EAngleRot;
        rotQ = GlobalConfig.ITT_QuatRot;

        StartCoroutine(CheckInternet());
        StartCoroutine(RenderMyOriginData(pos, rot, GlobalConfig.NO_MAP));
        
    }

    // Update is called once per frame
    void Update()
    {
        pos = GlobalConfig.ITT_VtriPos;
        rot = GlobalConfig.ITT_EAngleRot;
        rotQ = GlobalConfig.ITT_QuatRot;

        UpdateWorldCoordinate(pos, rot);
    }

    IEnumerator CheckInternet()
    {
        string url = "https://www.google.com/";
        UnityWebRequest www = new(url) { downloadHandler = new DownloadHandlerBuffer() };
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("No internet connection, or line busy!\n\n" + www.error);
        }
        else
        {
            Debug.Log("Connection available!");
        }
        www.Dispose();
    }

    IEnumerator RenderMyOriginData(Vector3 markerPos, Vector3 markerRot, bool skip_root_search = false)
    {
        Debug.Log("enter RenderMyOriginData");

        // import MyOrigin data from csv
        //List<MyOrigin> myOrigins = Import_FromOrigin.GetMyOriginsList();

        // import fro google sheet
        // need to do this one by one, still don't know how to make it dynamic call

        //string full_url = front_url + myorigin_id + back_url;
        if (m_MyOriginURL.Length <= 0) yield return null;
        UnityWebRequest www = new(m_MyOriginURL) { downloadHandler = new DownloadHandlerBuffer() };
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) { Debug.Log(www.error); }
        else
        {
            var result = www.downloadHandler.text;
            List<MyOrigin> myOrigins = Import_FromOrigin.ConvertFromListString(
                    ImportCSV.GetDataFromRawString(result, true)
                );

            // do foreach in csv data
            foreach (var item in myOrigins)
            {
                //Debug.Log("Origin data: " + item.name + " " + item.position.ToString());

                // check if name contains of imagetarget
                string[] strSplit = item.name.Split("_");   // delimiter always "_"
                string firstStr = strSplit[0];              // contained name always in [0]

                // if it's the root
                if (firstStr == "imagetarget" && !skip_root_search)
                {
                    GameObject root = new("root");

                    // ANOTHER MECHANIC THAT DOESN'T NEED ORIGIN
                    // 2023-08-27
                    //if (GlobalConfig.TempOriginGO.transform.position != Vector3.zero)
                    //{

                        // NEW MECHANIC: 2022-06-07
                        // See also: Test_InverseImageToOrigin.cs - MyMethod()

                        // ================== //
                        // 1. create our root based on imagetarget
                        root.transform.SetParent(GlobalConfig.TempOriginGO.transform, false);


                        // ================== //
                        // 2. make dummy object to inverse the transformation
                        GameObject dummy = new();

                        // rotate with our root to image target ROTATION data
                        dummy = GlobalConfig.RotateOneByOne(dummy, item.euler_rotation);

                        // get its inverse of rotation
                        Quaternion imageTarget_rotinv = Quaternion.Inverse(dummy.transform.rotation);

                        // apply to our root
                        root.transform.localRotation = imageTarget_rotinv;


                        // ================== //
                        // 3. calculate our position with calculating the localToWorldMatrix

                        // make our dummy to use the inverse rotation too
                        dummy.transform.rotation = imageTarget_rotinv;

                        // get the M4x4 matrix of from local to world of our dummy after rotation
                        Matrix4x4 mat4 = dummy.transform.localToWorldMatrix;

                        // vector multiplication with our root to image target POSITION DATA
                        Vector3 vec3 = mat4 * item.position;

                        // apply to our root, but inverse it (-)
                        root.transform.localPosition = -vec3;


                        // ================== //
                        // 4. make our root become ROOT now
                        root.transform.SetParent(null);
                        //imageTarget.transform.SetParent(ourRoot.transform);

                        // ================== //
                        // 5. finishing

                        // destroy the dummy object
                        Destroy(dummy);
                    //}

                    // Instantiate the root to become origin child
                    originChild = Instantiate(root, GlobalConfig.TempOriginGO.transform, true);
                    originChild.name = "originChild";

                    GlobalConfig.PlaySpaceOriginGO = root;

                    // OLD MECHANIC
                    /**
                        GameObject gameObject = new(item.name);

                        Debug.Log("root 0:");
                        Debug.Log(gameObject.transform.localPosition.ToString());
                        Debug.Log(gameObject.transform.localEulerAngles.ToString());
                        Debug.Log(gameObject.transform.eulerAngles.ToString());

                        gameObject.transform.SetParent(GlobalConfig.TempOriginGO.transform, false);

                        Debug.Log("temp origin go:");
                        Debug.Log(GlobalConfig.TempOriginGO.transform.position.ToString());
                        Debug.Log(GlobalConfig.TempOriginGO.transform.eulerAngles.ToString());
                        Debug.Log(GlobalConfig.TempOriginGO.transform.rotation.ToString());

                        Debug.Log("root 1:");
                        Debug.Log(gameObject.transform.localPosition.ToString());
                        Debug.Log(gameObject.transform.localEulerAngles.ToString());
                        Debug.Log(gameObject.transform.eulerAngles.ToString());

                        //gameObject.transform.localPosition = Vector3.zero;
                        //gameObject.transform.localEulerAngles = Vector3.zero;

                        //Debug.Log("root 2:");
                        //Debug.Log(gameObject.transform.localPosition.ToString());
                        //Debug.Log(gameObject.transform.localEulerAngles.ToString());
                        //Debug.Log(gameObject.transform.eulerAngles.ToString());

                        gameObject.transform.localPosition += item.position;
                        gameObject.transform.Rotate(item.euler_rotation);

                        Debug.Log("root 3:");
                        Debug.Log(gameObject.transform.localPosition.ToString());
                        Debug.Log(gameObject.transform.localEulerAngles.ToString());
                        Debug.Log(gameObject.transform.eulerAngles.ToString());
                    */

                    // insert into parents so assigning parent will likely easier
                    if (!CheckIfParentsExists(_parents, root.name)) { _parents.Add(root); }

                    // add into global config --> MyObjectList
                    GlobalConfig.MyObjectList.Add(root);

                    // put into GlobalConfig as root parent
                    //GlobalConfig.PlaySpaceMyOrigin = item;
                    GlobalConfig.PlaySpaceOriginGO = root;

                    // create anchor prefab
                    if (createAnchor) { CreateWorldAnchor(root); }
                }

                // if it's under root
                else
                {
                    GameObject childGameObject = new(item.name);

                    // assign to each parents
                    foreach (var parent in _parents)
                    {
                        if (parent.name == item.parent)
                        {
                            childGameObject.transform.parent = parent.transform;
                            break;
                        }
                    }

                    // assign the orientation
                    childGameObject.transform.localPosition = item.position;
                    childGameObject.transform.localRotation = Quaternion.identity;
                    childGameObject.transform.Rotate(item.euler_rotation);
                    //GlobalConfig.RotateOneByOne(childGameObject, item.euler_rotation);

                    // insert into parents
                    if (!CheckIfParentsExists(_parents, item.name))
                    {
                        _parents.Add(childGameObject);
                    }

                    // add into global config --> thingslist
                    GlobalConfig.MyObjectList.Add(childGameObject);

                    // create anchor prefab
                    if (createAnchor) { CreateWorldAnchor(childGameObject); }
                }
            }
        }

        www.Dispose();
        StartCoroutine(RenderMyObjectData());
    }

    IEnumerator RenderMyObjectData()
    {
        Debug.Log("enter RenderMyObjectData");

        // import MyObject data from csv
        //List<MyObject> myObjects = Import_FromObject.GetMyObjectsList();

        // import fro google sheet
        // need to do this one by one, still don't know how to make it dynamic call

        //string full_url = front_url + myobject_id + back_url;
        if (m_MyObjectURL.Length <= 0) yield return null;
        UnityWebRequest www = new(m_MyObjectURL) { downloadHandler = new DownloadHandlerBuffer() };
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) { Debug.Log(www.error); }
        else
        {
            var result = www.downloadHandler.text;
            List<MyObject> myObjects = Import_FromObject.ConvertFromListString(
                    ImportCSV.GetDataFromRawString(result, true)
                );

            if (myObjects.Count <= 0) Debug.Log("No object data");

            // do foreach data in the csv
            foreach (var item in myObjects)
            {
                // initialize gameobject
                GameObject newGameObject;
                string prefabtype = item.virtualObject.type;

                // choose gameobject type
                if (prefabtype == MyObject.PrefabType.CUBE) { newGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube); }
                else if (prefabtype == MyObject.PrefabType.SPHERE) { newGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere); }
                else if (prefabtype == MyObject.PrefabType.CYLINDER) { newGameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder); }
                else if (prefabtype == MyObject.PrefabType.SPECIAL) { newGameObject = CreateSpecialPrefab(item.virtualObject); }
                else { newGameObject = new GameObject(); }

                // check if game object is not null
                if (newGameObject == null)
                {
                    Debug.Log("No game object created, skip the data!");
                }
                else
                {

                    // set gameobject name
                    newGameObject.name = item.name;

                    // set gameobject parent
                    foreach (var parent in _parents)
                    {
                        if (parent.name == item.coordinate_system)
                        {
                            newGameObject.transform.parent = parent.transform;
                            break;
                        }
                    }

                    // calculate myObject dimension to its given origin position
                    MyObject.MyObject_LHW tempLHW = new(item.length, item.height, item.width);
                    tempLHW = OriginCalculator.Calculate(tempLHW, item.origin.type, item.origin.descriptor);
                    Vector3 newOrigin = new(tempLHW.L, tempLHW.H, tempLHW.W);

                    // if only the object with special prefab, some abnormality
                    // we still put the correct position on item.comment
                    // we put their dimension as exact (1,1,1), not describe as 1 meter each
                    if (prefabtype == MyObject.PrefabType.SPECIAL)
                    {
                        newOrigin = OriginCalculator.CalculateAbnormalOrigin(item.virtualObject.special.position);
                    }

                    // assign orientation using the csv data transformation
                    newGameObject.transform.localPosition = newOrigin;
                    newGameObject.transform.localRotation = Quaternion.identity;
                    newGameObject.transform.localScale = new Vector3(item.length, item.height, item.width);

                    // render normal colorize if it's IoT device
                    if (item.iotDevice_true)
                    {
                        // insert into parents
                        if (!CheckIfParentsExists(_objects, item.name))
                        {
                            _objects.Add(newGameObject);
                            m_ObjectsGroundTruth.Add(newGameObject);
                        }

                        // add into global config --> thingslist
                        GlobalConfig.MyObjectList.Add(newGameObject);

                        // assign ColorManager
                        newGameObject.AddComponent<ColorManager>();

                        // assign DataManager
                        newGameObject.AddComponent<DataManager>();

                        // START PLAYING COLOR DATA
                        newGameObject.GetComponent<DataManager>().testingOnly = true;
                        newGameObject.GetComponent<DataManager>().Test_AssignHiLoValue();
                        //StartCoroutine(Test_Loop__PlayingColor(newGameObject));

                        /// TEST LOCATION ABOVE GAME
                        ///
                        //m_ShowTextAboveLocation
                        //    .GetComponent<Test_ShowLocationAboveObject>()
                        //    .AddGameObject(newGameObject.transform.parent.gameObject);
                    }
                    else
                    {
                        // testing z buffer ACTIVE/NO ACTIVE
                        if (!dontUseZBuffer)
                        {
                            // assign StaticPrefabManager
                            newGameObject.AddComponent<NonIoTDeviceManager>();

                            // if not special static
                            if (item.virtualObject.type != MyObject.PrefabType.SPECIAL)
                            {
                                newGameObject.GetComponent<NonIoTDeviceManager>().AssignMaterial();
                            }
                            else
                            {
                                // check for every part that has renderer or mesh renderer
                                SetAllChildIntoZBuffer(newGameObject);
                            }
                        }
                    }
                }
            }

            SetStartTime();
            UseCorrectionFunction();

            // SaveAllObjectOnlyDebugDontUseThisFunction();
        }

        www.Dispose();
    }

    /// <summary>
    /// Only set the time after load all object
    /// </summary>
    void SetStartTime()
    {
        GlobalConfig.AFTER_LOAD_START_TIME = Time.time;
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

    private GameObject CreateSpecialPrefab(MyObject.VirtualObject virtualObject)
    {
        // if only it is special prefab, already defined on Editor the number of custom prefab
        // that inside the array of SpecialPrefab_CatExample
        try
        {
            int prefab_number = int.Parse(virtualObject.special.parameter);
            GameObject prefab = specialPrefabList.GetComponent<SpecialPrefab_CatExample>().GetPrefab(prefab_number);
            GameObject prefab_obj = Instantiate(prefab);
            return prefab_obj;
        }
        catch (System.Exception ex)
        {
            // it's normal prefab
            // maybe with Resource path, etc.
            // ... dunno

            Debug.LogError(ex);
        }

        return gameObject;
    }

    /**
     * <summary>This only when image target is active</summary>
     */ 
    private void UpdateWorldCoordinate(Vector3 markerPos, Vector3 markerRot)
    {
        // OLD MECHANIC
        //GlobalConfig.PlaySpaceOriginGO.transform.position = markerPos + GlobalConfig.PlaySpaceMyOrigin.position;
        //GlobalConfig.PlaySpaceOriginGO.transform.rotation = Quaternion.identity;
        //GlobalConfig.PlaySpaceOriginGO.transform.Rotate(markerRot + GlobalConfig.PlaySpaceMyOrigin.euler_rotation);
        //GlobalConfig.OurWorldOrigin_MyOrigin_GameObject.transform.localScale = GlobalConfig.OurWorldOrigin_Things.scale.GetScale();

        // NEW MECHANIC
        // task:
        // - if worldmap used, nothing has change since no reference updated at this time
        //
        // - if imageTarget used, get image target position as reference point
        // - re calculate the root with NEW MECHANIC
        // - since NEW MECHANIC always create new GO (dummy object for reference)
        //   - does it really cost?
        //   - e.g., in every frames per seconds (30 fps), 30 times create and recalculate
        // - we need another technique for this
        //
        // Idea is:
        // - 1: get the image target pos, rot (which always updated when detected)
        // - 2: tell this script, or system, that there is an update of point of reference
        //
        // - why don't do the same technique?
        //   - origin object from imageTarget still active (not destroyed) (A)
        //   - Instantiate our root gameobject, put as child of origin object (B)
        //   - change pos, rot of root based on (B) localtoworldcoordinate

        // transform our root based on originChild

        // UPDATE 2022-07-08
        // - do not update root based on image position
        // - let them be by the SLAM system
        //if (!originChild) return;
        //Vector3 tempPos = originChild.transform.position;
        //Quaternion tempRot = originChild.transform.rotation;
        //GlobalConfig.PlaySpaceOriginGO.transform.SetPositionAndRotation(tempPos, tempRot);

        //Debug.Log(string.Format("In loadObject\n\nPos: {0}\nRot: {1}",
        //                    originChild.transform.parent.transform.position,
        //                    originChild.transform.parent.transform.rotation));
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

    void SetAllChildIntoZBuffer(GameObject prefab_gameObject)
    {
        // check if gameobject has renderer
        if (NonIoTDeviceManager.CheckIfHasRenderer(prefab_gameObject))
        {
            prefab_gameObject.GetComponent<NonIoTDeviceManager>().AssignMaterial();
        }

        int child = prefab_gameObject.transform.childCount;
        for (int i = 0; i < child; i++)
        {
            GameObject childGameObject = prefab_gameObject.transform.GetChild(i).gameObject;

            // do recursive call
            if (childGameObject.transform.childCount > 0)
                SetAllChildIntoZBuffer(childGameObject);

            // check if child has renderer
            if (NonIoTDeviceManager.CheckIfHasRenderer(childGameObject))
            {
                childGameObject.AddComponent<NonIoTDeviceManager>();
                childGameObject.GetComponent<NonIoTDeviceManager>().AssignMaterial();
            }
        }
    }

    public List<GameObject> GetMyObjects()
    {
        return _objects;
    }

    public List<GameObject> GetObjectsGT()
    {
        return m_ObjectsGroundTruth;
    }

    public List<GameObject> GetMyParents()
    {
        return _parents;
    }

    //////////////////////////////
    // PLAYING COLOR DATA
    IEnumerator Test_Loop__PlayingColor(GameObject gO)
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            UpdatingDataManager(gO);
        }
    }

    void UseCorrectionFunction()
    {        
        // but if the correction function is disabled, no use the rest of code
        if (m_CorrectionFunctionManager == null) return;

        /////////
        /// UPDATE ON 2023-06-11
        /////////
        ///
        int corr_num = GlobalConfig.CorrectionFunctionVersion;

        // Ver 0:  no correction
        // just no correction activated

        // Ver 1:  object to sc transform, marker on runtime
        if (corr_num == 1)
        {
            m_CorrectionFunctionManager.GetComponent<CorrectionFunctions.VersionOne>().enabled = true;
            EnableNewARSceneImageTrackingCorrection();
            return;
        }

        // Ver 1b: wc to sc transform, marker on runtime(or adapt with camera movement)
        // has var 2 where camera movement adapt
        if (corr_num == 2)
        {
            m_CorrectionFunctionManager.GetComponent<CorrectionFunctions.VersionOneWithRotation>().enabled = true;
            EnableNewARSceneImageTrackingCorrection();
            return;
        }

        // Ver 2:  object to sc transform, marker pre load from data
        if (corr_num == 3)
        {
            m_CorrectionFunctionManager.GetComponent<CorrectionFunctions.VersionTwoPreload>().enabled = true;
            return;
        }

        // Ver 3:  no map reload, active init marker, object to sc transform, marker on runtime
        if (corr_num == 4)
        {
            m_CorrectionFunctionManager.GetComponent<CorrectionFunctions.VersionThreeNoMap>().enabled = true;
            return;
        }

        if (corr_num >= 5 && corr_num <= 8)
        {
            m_CorrectionFunctionManager.GetComponent<VersionFourThesis>().enabled = true;
            return;
        }



        // VERSION 1:
        // desc:
        // - don't use previous marker result
        // - generate marker in testing phase runtime
        // how:
        // - use the NewARSceneCorrectionFunction to get and process marker data
        // - use the NewARSceneImageTrackingCorrection to get marker data

        //if (GlobalConfig.CorrectionFunctionVersion == 1)
        //    m_CorrectionFunctionManager
        //        .GetComponent<CorrectionFunctions.VersionOne>().enabled = true;

        //if (GlobalConfig.CorrectionFunctionVersion == 2)
        //    m_CorrectionFunctionManager
        //        .GetComponent<CorrectionFunctions.VersionOneWithRotation>().enabled = true;

        //if (GlobalConfig.CorrectionFunctionVersion == 2)
        //    m_CorrectionFunctionManager
        //        .GetComponent<CorrectionFunctions.VersionOneBLastMarker>().enabled = true;

        //if (GlobalConfig.CorrectionFunctionVersion == 3)
        //    m_CorrectionFunctionManager
        //        .GetComponent<CorrectionFunctions.VersionOneBAvgWMarker>().enabled = true;

        // VERSION 2:
        // desc:
        // - use marker to marker distance to get priority weight

        //if (GlobalConfig.CorrectionFunctionVersion == 4)
        //    m_CorrectionFunctionManager
        //        .GetComponent<CorrectionFunctions.VersionTwoA>().enabled = true;

        //if (GlobalConfig.CorrectionFunctionVersion == 5)
        //    m_CorrectionFunctionManager
        //        .GetComponent<CorrectionFunctions.VersionTwoB>().enabled = true;

        // VERSION 0:  result failed

        //m_CorrectionFunctionManager
        //    .GetComponent<NewARSceneCorrectionFunction>().Main();

        // save data (DISABLE THIS IF DOESNT USE)
        //var t = m_ShowTextAboveLocation
        //    .GetComponent<Test_JustAnotherScript>();
        //if (!t.enabled) return;
        //Test_CorrectionDataSave.SaveDataIntoCSV(GetMyObjects());
    }

    void SaveAllObjectOnlyDebugDontUseThisFunction()
    {
        var data = GetMyObjects();
        List<string[]> pos_list = new();

        foreach (var d in data)
        {
            var m44 = GlobalConfig.GetM44ByGameObjRef(d, GlobalConfig.PlaySpaceOriginGO);
            var pos = GlobalConfig.GetPositionFromM44(m44);
            string[] s = { d.name, pos.x.ToString(), pos.y.ToString(), pos.z.ToString() };
            pos_list.Add(s);
        }

        // get path
        var date = GlobalConfig.GetNowDateandTime();
        var map = GlobalConfig.LOAD_MAP.ToString();
        var file = date + "_AllObjPosition_" + map + ".csv";
        var path = System.IO.Path.Combine(Application.persistentDataPath, file);
        ExportCSV.exportData(path, pos_list);
    }

    public void EnableNewARSceneImageTrackingCorrection()
    {
        // marker tracking is enabled
        m_CorrectionFunctionManager.GetComponent<NewARSceneImageTrackingCorrection>().enabled = true;
    }
}
