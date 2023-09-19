using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_ImportData_MyOrigin : MonoBehaviour
{
    public Vector3 position = new(0, 0, 0);
    public Vector3 rotation = new(0, 0, 0);
    public GameObject prefabObject;

    [Range(40.0f, 100.0f)]
    public float color_picking = 40.0f;

    private List<GameObject> _allObject_test = new();
    private const float _alpha = 0.3f;

    private List<GameObject> _parents = new();

    // Start is called before the first frame update
    void Start()
    {
        // summon myOrigin
// DISABLE OLD MECHANIC
        //DoStuffs(position, rotation);

        // summon myObject
// DISABLE OLD MECHANIC
        //DoStuffsObjects();

        // do color stuffs per second
        //StartCoroutine(Loop());
    }

    // Update is called once per frame
    void Update()
    {
        // if anchor updated
        //DoStuffsUpdate(position, rotation);

        // playing with color controller
        foreach (var gameObject in _allObject_test)
        {
// DISABLE OLD MECHANIC
            //UpdatingColorManager(gameObject, color_picking);
        }
    }

    IEnumerator Loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            foreach (var gO in _allObject_test)
            {
                // set ColorManager
                //UpdatingColorManager(gO, color_picking);

                // set DataManager
                if (gO.GetComponent<DataManager>() != null)
                {
// DISABLE OLD MECHANIC
                    //UpdatingDataManager(gO);
                }
            }
        }
    }
// OLD MECHANIC
/**
    private void DoStuffs(Vector3 markerPos, Vector3 markerRot)
    {
        Debug.Log("marker position: " + markerPos);

        //List<Things> thingsList = Import_FromThings.GetThingsList();
        List<MyOrigin> myOrigins = Import_FromOrigin.GetMyOriginsList();
        Debug.Log("successful import data");
        Debug.Log("data count: " + myOrigins.Count);
        Debug.Log("parents count: " + _parents.Count);

        foreach (var item in myOrigins)
        {
            if (item.parent == "none")
            {
                Debug.Log("parent none?: " + item.parent);

                GameObject gameObject = new GameObject(item.name);
                gameObject.transform.localPosition = markerPos + item.position;
                gameObject.transform.Rotate(markerRot + item.euler_rotation);
                //gameObject.transform.localScale = item.scale.GetScale();

                // insert into parents
                if (!CheckIfParentsExists(_parents, item.name))
                {
                    _parents.Add(gameObject);
                    Debug.Log("new parent: " + gameObject.name + " " + gameObject.transform.position.ToString());
                    Debug.Log("successfully create new parent");
                    Debug.Log("parents count after creation: " + _parents.Count);
                }

                // add into global config --> thingslist
                GlobalConfig.ThingsList.Add(gameObject);
                Debug.Log("Globalconfig thingslist count: " + GlobalConfig.ThingsList.Count);

                // put root parent
                GlobalConfig.PlaySpaceMyOrigin = item;
                GlobalConfig.PlaySpaceOriginGO = gameObject;

                // create prefab
                CreateWorldAnchor(gameObject);

                //Vector3 newOrigin_Pos = markerPos + item.position.GetPosition();
                //Quaternion newOrigin_Rot = Quaternion.Euler(item.rotation.GetRotation());

                //GetComponent<ARSessionOrigin>().MakeContentAppearAt(
                //    GetComponent<GameObject>().transform,
                //    newOrigin_Pos, newOrigin_Rot);
            }

            else
            {
                Debug.Log("parent ???: " + item.parent);

                GameObject gameObject = new GameObject(item.name);
                //if (item.render) { gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube); }
                //else { gameObject = new GameObject();}
                //gameObject.name = item.name;

                // assign to its parent
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
                    Debug.Log("new parent: " + gameObject.name + " " + gameObject.transform.position.ToString());
                    Debug.Log("successfully create new parent");
                    Debug.Log("parents count after creation: " + _parents.Count);
                }

                Debug.Log("new object: " + gameObject.name + " " + gameObject.transform.position.ToString());
                Debug.Log("successfully create new object relative to " + gameObject.transform.parent);

                // add into global config --> thingslist
                GlobalConfig.ThingsList.Add(gameObject);
                Debug.Log("Globalconfig thingslist count: " + GlobalConfig.ThingsList.Count);

                // test into object array
                _allObject_test.Add(gameObject);

                // test creating world anchor to see CS
                CreateWorldAnchor(gameObject);
            }
        }
    }

    private void DoStuffsObjects()
    {
        List<MyObject> myObjects = Import_FromObject.GetMyObjectsList();
        Debug.Log("successful import data");
        Debug.Log("data count: " + myObjects.Count);
        Debug.Log("parents count: " + _parents.Count);

        foreach (var item in myObjects)
        {
            Debug.Log("parent ???: " + item.parent);

            // initialize gameobject
            GameObject gameObject;
            string prefabtype = item.prefab_type;

            // choose gameobject type
            if (prefabtype == MyObject.PrefabType.CUBE) { gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube); }
            else if (prefabtype == MyObject.PrefabType.SPHERE) { gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere); }
            else if (prefabtype == MyObject.PrefabType.CYLINDER) { gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder); }
            else if (prefabtype == MyObject.PrefabType.SPECIAL) { gameObject = CreateSpecialPrefab(new GameObject(), item.prefab_special); }
            else { gameObject = new GameObject();}

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
                Debug.Log("new parent: " + gameObject.name + " " + gameObject.transform.position.ToString());
                Debug.Log("successfully create new parent");
                Debug.Log("parents count after creation: " + _parents.Count);
            }

            Debug.Log("new object: " + gameObject.name + " " + gameObject.transform.position.ToString());
            Debug.Log("successfully create new object relative to " + gameObject.transform.parent);

            // add into global config --> thingslist
            GlobalConfig.ThingsList.Add(gameObject);
            Debug.Log("Globalconfig thingslist count: " + GlobalConfig.ThingsList.Count);

            // test into object array
            _allObject_test.Add(gameObject);

            // assign ColorManager
            gameObject.AddComponent<ColorManager>();

            // assign DataManager
            gameObject.AddComponent<DataManager>();
            gameObject.GetComponent<DataManager>().testingOnly = true;
            gameObject.GetComponent<DataManager>().Test_AssignHiLoValue();
        }
    }

    private void DoStuffsUpdate(Vector3 markerPos, Vector3 markerRot)
    {
        foreach (GameObject item in GlobalConfig.ThingsList)
        {
            if (item.transform.parent == null)  // which means the root
            {
                item.transform.position = markerPos + GlobalConfig.OurWorldOrigin_Things.position.GetPosition();
                item.transform.rotation = Quaternion.identity;
                item.transform.Rotate(markerRot + GlobalConfig.OurWorldOrigin_Things.rotation.GetRotation());
                item.transform.localScale = GlobalConfig.OurWorldOrigin_Things.scale.GetScale();

                //Debug.Log("Position: " + item.transform.position.ToString() + "\n" +
                //          "Rotation Euler: " + item.transform.eulerAngles.ToString());

                break;
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
        if (prefabObject != null)
        {
            GameObject gameObject = Instantiate(prefabObject);
            gameObject.transform.parent = parent.transform;
            gameObject.name = parent.name + "_prefab";

            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f) ;

            gameObject.SetActive(true);
        }
    }

    private void UpdatingColorManager(GameObject gameObject, float value)
    {
        if (gameObject.GetComponent<ColorManager>() == null) { return; }

        float hi = gameObject.GetComponent<DataManager>().GetHighestValue();
        float lo = gameObject.GetComponent<DataManager>().GetLowestValue();
        gameObject.GetComponent<ColorManager>().AssignHighLowAlpha(hi, lo, _alpha);
        gameObject.GetComponent<ColorManager>().UpdateColor(value);
    }

    private void UpdatingDataManager(GameObject gameObject)
    {
        if (gameObject.GetComponent<DataManager>() == null) { return; }

        float previous_value = gameObject.GetComponent<DataManager>().GetCurrentValue();
        if (previous_value == 0) { previous_value = 60.0f; }
        float next_value = gameObject.GetComponent<DataManager>().Test_GetDataUpdate(previous_value);
        UpdatingColorManager(gameObject, next_value);
    }

    private GameObject CreateSpecialPrefab(GameObject gameObject, string prefab_special_path)
    {
        return gameObject;
    }
*/
}
