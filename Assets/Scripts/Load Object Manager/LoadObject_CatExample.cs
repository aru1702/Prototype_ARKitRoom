using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadObject_CatExample : MonoBehaviour
{
    List<GameObject> _parents = new();

    [SerializeField]
    float _alpha = 0.3f;

    [SerializeField]
    bool createAnchor = false;

    [SerializeField]
    GameObject originPrefabInstantiate;

    [SerializeField]
    GameObject specialPrefabList;

    Vector3 pos, rot;
    Quaternion rotQ;

    void OnEnable()
    {
        Debug.Log("LoadObj active");

        pos = GlobalConfig.ITT_VtriPos;
        rot = GlobalConfig.ITT_EAngleRot;
        rotQ = GlobalConfig.ITT_QuatRot;

        RenderMyOriginData(pos, rot);
        RenderMyObjectData();
    }

    // Update is called once per frame
    void Update()
    {
        pos = GlobalConfig.ITT_VtriPos;
        rot = GlobalConfig.ITT_EAngleRot;
        rotQ = GlobalConfig.ITT_QuatRot;

        UpdateWorldCoordinate(pos, rot);
    }

    private void RenderMyOriginData(Vector3 markerPos, Vector3 markerRot)
    {
        // create root
        GameObject root = new("root");

        // import MyOrigin data from csv
        List<MyOrigin> myOrigins = Import_FromOrigin.GetMyOriginsList();

        // do foreach in csv data
        foreach (var item in myOrigins)
        {
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

                // destroy the dummy object
                Destroy(dummy);


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
            string prefabtype = item.virtualObject.type;

            // choose gameobject type
            if (prefabtype == MyObject.PrefabType.CUBE) { gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube); }
            else if (prefabtype == MyObject.PrefabType.SPHERE) { gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere); }
            else if (prefabtype == MyObject.PrefabType.CYLINDER) { gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder); }
            else if (prefabtype == MyObject.PrefabType.SPECIAL) { gameObject = CreateSpecialPrefab(item.virtualObject); }
            else { gameObject = new GameObject(); }

            // check if game object is not null
            if (gameObject == null)
            {
                Debug.Log("No game object created, skip the data!");
            }
            else
            {

                // set gameobject name
                gameObject.name = item.name;

                // set gameobject parent
                foreach (var parent in _parents)
                {
                    if (parent.name == item.coordinate_system)
                    {
                        gameObject.transform.parent = parent.transform;
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
                gameObject.transform.localPosition = newOrigin;
                gameObject.transform.localRotation = Quaternion.identity;
                gameObject.transform.localScale = new Vector3(item.length, item.height, item.width);

                // insert into parents
                if (!CheckIfParentsExists(_parents, item.name))
                {
                    _parents.Add(gameObject);
                }

                // render normal colorize if it's IoT device
                if (item.iotDevice_true)
                {
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
                else
                {
                    // assign StaticPrefabManager
                    gameObject.AddComponent<NonIoTDeviceManager>();

                    // if not special static
                    if (item.virtualObject.type != MyObject.PrefabType.SPECIAL)
                    {
                        gameObject.GetComponent<NonIoTDeviceManager>().AssignMaterial();
                    }
                }
            }
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

    private void UpdateWorldCoordinate(Vector3 markerPos, Vector3 markerRot)
    {
        //GlobalConfig.PlaySpaceOriginGO.transform.position = markerPos + GlobalConfig.PlaySpaceMyOrigin.position;
        //GlobalConfig.PlaySpaceOriginGO.transform.rotation = Quaternion.identity;
        //GlobalConfig.PlaySpaceOriginGO.transform.Rotate(markerRot + GlobalConfig.PlaySpaceMyOrigin.euler_rotation);
        //GlobalConfig.OurWorldOrigin_MyOrigin_GameObject.transform.localScale = GlobalConfig.OurWorldOrigin_Things.scale.GetScale();

        GlobalConfig.TempOriginGO.transform.SetPositionAndRotation(pos, rotQ);
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
