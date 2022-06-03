using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_ImportDataCreateCube : MonoBehaviour
{
    public Vector3 position = new Vector3(0,0,0);
    public Vector3 rotation = new Vector3(0,0,0);
    public GameObject prefabObject;

    [Range(40.0f, 100.0f)]
    public float color_picking = 40.0f;

    private List<GameObject> _allObject_test = new List<GameObject>();
    private const float _alpha = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        DoStuffs(position, rotation);
        StartCoroutine(Loop());
    }

    // Update is called once per frame
    void Update()
    {
        DoStuffsUpdate(position, rotation);
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
                    UpdatingDataManager(gO);
                }
            }
        }
    }

    private void DoStuffs(Vector3 markerPos, Vector3 markerRot)
    {
        Debug.Log("marker position: " + markerPos);

        List<Things> thingsList = Import_FromThings.GetThingsList();
        Debug.Log("successful import data");
        Debug.Log("data count: " + thingsList.Count);

        List<GameObject> parents = new List<GameObject>();
        Debug.Log("parents count: " + parents.Count);

        foreach (var item in thingsList)
        {
            if (item.parent == "none")
            {
                Debug.Log("parent none?: " + item.parent);

                GameObject gameObject = new GameObject(item.name);
                gameObject.transform.localPosition = markerPos + item.position.GetPosition();
                gameObject.transform.Rotate(markerRot + item.rotation.GetRotation());
                gameObject.transform.localScale = item.scale.GetScale();

                // insert into parents
                if (!CheckIfParentsExists(parents, item.name))
                {
                    parents.Add(gameObject);
                    Debug.Log("new parent: " + gameObject.name + " " + gameObject.transform.position.ToString());
                    Debug.Log("successfully create new parent");
                    Debug.Log("parents count after creation: " + parents.Count);
                }

                // add into global config --> thingslist
                GlobalConfig.ThingsList.Add(gameObject);
                Debug.Log("Globalconfig thingslist count: " + GlobalConfig.ThingsList.Count);

                // put root parent
                GlobalConfig.OurWorldOrigin_Things = item;
                GlobalConfig.OurWorldOrigin_GameObject = gameObject;

                // create prefab
                CreateWorldAnchor();

                //Vector3 newOrigin_Pos = markerPos + item.position.GetPosition();
                //Quaternion newOrigin_Rot = Quaternion.Euler(item.rotation.GetRotation());

                //GetComponent<ARSessionOrigin>().MakeContentAppearAt(
                //    GetComponent<GameObject>().transform,
                //    newOrigin_Pos, newOrigin_Rot);
            }

            else 
            {
                Debug.Log("parent ???: " + item.parent);

                GameObject gameObject;
                if (item.render) { gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube); }
                else { gameObject = new GameObject();}
                gameObject.name = item.name;

                foreach (var parent in parents)
                {
                    if (parent.name == item.parent)
                    {
                        gameObject.transform.parent = parent.transform;
                        break;
                    }
                }

                gameObject.transform.localPosition = item.position.GetPosition();
                gameObject.transform.localRotation = Quaternion.identity;
                gameObject.transform.Rotate(item.rotation.GetRotation());
                gameObject.transform.localScale = item.scale.GetScale();

                // insert into parents
                if (!CheckIfParentsExists(parents, item.name))
                {
                    parents.Add(gameObject);
                    Debug.Log("new parent: " + gameObject.name + " " + gameObject.transform.position.ToString());
                    Debug.Log("successfully create new parent");
                    Debug.Log("parents count after creation: " + parents.Count);
                }

                Debug.Log("new object: " + gameObject.name + " " + gameObject.transform.position.ToString());
                Debug.Log("successfully create new object relative to " + gameObject.transform.parent);

                // add into global config --> thingslist
                GlobalConfig.ThingsList.Add(gameObject);
                Debug.Log("Globalconfig thingslist count: " + GlobalConfig.ThingsList.Count);

                // test into object array
                _allObject_test.Add(gameObject);

                if (item.render)
                {
                    // assign ColorManager
                    gameObject.AddComponent<ColorManager>();

                    // assign DataManager
                    gameObject.AddComponent<DataManager>();
                    gameObject.GetComponent<DataManager>().testingOnly = true;
                    gameObject.GetComponent<DataManager>().Test_AssignHiLoValue();
                }
            }
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

    private void CreateWorldAnchor()
    {
        if (prefabObject != null)
        {
            GameObject gameObject = Instantiate(prefabObject);
            gameObject.transform.parent = GlobalConfig.OurWorldOrigin_GameObject.transform;

            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
            //gameObject.transform.localScale = Vector3.one;

            gameObject.SetActive(true);
        }
    }

    private void UpdatingColorManager(GameObject gameObject, float value)
    {
        if (gameObject.GetComponent<ColorManager>() == null) { throw new System.Exception("No color manager"); }

        float hi = gameObject.GetComponent<DataManager>().GetHighestValue();
        float lo = gameObject.GetComponent<DataManager>().GetLowestValue();
        gameObject.GetComponent<ColorManager>().AssignHighLowAlpha(hi, lo, _alpha);
        gameObject.GetComponent<ColorManager>().UpdateColor(value);
    }

    private void UpdatingDataManager(GameObject gameObject)
    {
        if (gameObject.GetComponent<DataManager>() == null) { throw new System.Exception("No data manager"); }

        float previous_value = gameObject.GetComponent<DataManager>().GetCurrentValue();
        if (previous_value == 0) { previous_value = 60.0f; }
        float next_value = gameObject.GetComponent<DataManager>().Test_GetDataUpdate(previous_value);
        UpdatingColorManager(gameObject, next_value);
    }
}
