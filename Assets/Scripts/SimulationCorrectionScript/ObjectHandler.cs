using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHandler : MonoBehaviour
{
    [SerializeField]
    GameObject m_DataImport;

    [SerializeField]
    GameObject m_SpecialPrefabCollection;

    [SerializeField]
    bool dontUseZBuffer = false;

    [SerializeField]
    bool createAnchor = false;

    [SerializeField]
    GameObject originPrefabInstantiate;


    List<GameObject> _parents = new();
    List<GameObject> _objects = new();
    List<GameObject> m_ObjectsGroundTruth = new();
    GameObject originChild;


    // Start is called before the first frame update
    void Start()
    {
        // this function is copy-paste from Load Object Manager
        // see either for Mapping Configuration or New AR Scene

        // but first, we create a dummy origin gameobject
        GlobalConfig.TempOriginGO = new();
        GlobalConfig.TempOriginGO.gameObject.name = "TempOriginGO";

        // origin data
        var my_origin_txt = m_DataImport.GetComponent<DataImport>().GetMyOrigins();
        List<MyOrigin> myOrigins = Import_FromOrigin.ConvertFromListString(my_origin_txt);

        ProcessOriginData(myOrigins);

        // object data
        var my_object_txt = m_DataImport.GetComponent<DataImport>().GetMyObjects();
        List<MyObject> myObjects = Import_FromObject.ConvertFromListString(my_object_txt);

        ProcessObjectData(myObjects);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Process the origin data from MyOrigin list
    /// </summary>
    void ProcessOriginData(List<MyOrigin> myOrigins)
    {
        // do foreach in csv data
        foreach (var item in myOrigins)
        {
            //Debug.Log("Origin data: " + item.name + " " + item.position.ToString());

            // check if name contains of imagetarget
            string[] strSplit = item.name.Split("_");   // delimiter always "_"
            string firstStr = strSplit[0];              // contained name always in [0]

            // if it's the root
            if (firstStr == "imagetarget")
            {
                // NEW MECHANIC: 2022-06-07
                // See also: Test_InverseImageToOrigin.cs - MyMethod()

                // ================== //
                // 1. create our root based on imagetarget
                GameObject root = new("root");
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

                // Instantiate the root to become origin child
                originChild = Instantiate(root, GlobalConfig.TempOriginGO.transform, true);
                originChild.name = "originChild";

                GlobalConfig.PlaySpaceOriginGO = root;

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

    /// <summary>
    /// Process the object data from MyOrigin list
    /// </summary>
    void ProcessObjectData(List<MyObject> myObjects)
    {
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
                    //newGameObject.AddComponent<ColorManager>();

                    // assign DataManager
                    //newGameObject.AddComponent<DataManager>();

                    // START PLAYING COLOR DATA
                    //newGameObject.GetComponent<DataManager>().testingOnly = true;
                    //newGameObject.GetComponent<DataManager>().Test_AssignHiLoValue();
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
    }

    bool CheckIfParentsExists(List<GameObject> parentsList, string parentName)
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

    void CreateWorldAnchor(GameObject parent)
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

    GameObject CreateSpecialPrefab(MyObject.VirtualObject virtualObject)
    {
        // if only it is special prefab, already defined on Editor the number of custom prefab
        // that inside the array of SpecialPrefab_CatExample
        try
        {
            int prefab_number = int.Parse(virtualObject.special.parameter);
            GameObject prefab = m_SpecialPrefabCollection.GetComponent<SpecialPrefab_CatExample>().GetPrefab(prefab_number);
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
}
