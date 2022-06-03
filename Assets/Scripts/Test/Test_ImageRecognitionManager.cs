using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class Test_ImageRecognitionManager : MonoBehaviour
{

    const string img1_name = "cat_example";
    //bool alreadyRender = false;
    bool initRendering = false;

    private ARTrackedImageManager _arTrackedImageManager;

    public GameObject OriginPrefabInstantiate;
    public bool createAnchor = false;
    public GameObject arSessionOrigin;

    private void Awake()
    {
        _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        _arTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    private void OnDisable()
    {
        _arTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var newImage in args.added)
        {
            //Debug.Log(trackedImage.GetComponent<GameObject>().transform.position.ToString());
            Debug.Log("OIC Name: " + newImage.referenceImage.name);
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

    private void Update()
    {
        if (_arTrackedImageManager.trackables.count > 0)
        {
            string debug_string = "";

            // debug AR camera position relative to the world origin
            foreach (var trackedImage in _arTrackedImageManager.trackables)
            {
                debug_string += trackedImage.referenceImage.name + ": " + trackedImage.transform.position.ToString() + "\n";
            }

            //Debug.Log(debug_string);
        }

        //if (!alreadyRender)
        //{
        //    // this should be dynamically if more than 1 img
        //    // but because we play with only 1 image
        //    foreach (var trackedImage in _arTrackedImageManager.trackables)
        //    {
        //        Debug.Log("Name: " + trackedImage.referenceImage.name);
        //        Debug.Log("Position: " + trackedImage.transform.position.ToString());
        //        Debug.Log("Rotation Euler: " + trackedImage.transform.eulerAngles.ToString());

        //        if (trackedImage.referenceImage.name == img1_name)
        //        {
        //            // only do once
        //            alreadyRender = true;

        //            DoStuffs(trackedImage.transform.position, trackedImage.transform.eulerAngles);
        //            break;
        //        }
        //    }
        //}

        foreach (var trackedImage in _arTrackedImageManager.trackables)
        {
            if (trackedImage.referenceImage.name == img1_name)
            {
                //Debug.Log("Name: " + trackedImage.referenceImage.name + "\n\n" +
                //          "Position: " + trackedImage.transform.position.ToString() + "\n\n" +
                //          "Rotation Euler: " + trackedImage.transform.eulerAngles.ToString() + "\n\n" +
                //          "Position to AR world: " + trackedImage.transform.localPosition);

                if (!initRendering)
                {
                    // only do once
                    initRendering = true;

                    //Debug.Log("Name: " + "world" + "\n\n" +
                    //      "Position: " + arSessionOrigin.transform.position.ToString() + "\n\n" +
                    //      "Rotation Euler: " + arSessionOrigin.transform.eulerAngles.ToString());

                    // make AR world origin has same orientation as the image origin
                    var targetPos = -trackedImage.transform.position;
                    var targetRot = Quaternion.Inverse(trackedImage.transform.rotation);
                    arSessionOrigin.transform.position = targetPos;
                    arSessionOrigin.transform.rotation = targetRot;

                    //Debug.Log("Name: " + "world 2" + "\n\n" +
                    //      "Position: " + arSessionOrigin.transform.position.ToString() + "\n\n" +
                    //      "Rotation Euler: " + arSessionOrigin.transform.eulerAngles.ToString());

                    // render the cube only once
                    DoStuffs(trackedImage.transform.position, trackedImage.transform.eulerAngles);
                    break;
                }
                else
                {
                    // update cube rendering every frame (tracked the image as reference)
                    DoStuffsUpdate(trackedImage.transform.position, trackedImage.transform.eulerAngles);
                    break;
                }
            }
        }
    }

    private void DoStuffs(Vector3 markerPos, Vector3 markerRot)
    {
        Debug.Log("marker position: " + markerPos);

        List<Things> thingsList = Import_FromThings.GetThingsList();
        //Debug.Log("successful import data");
        //Debug.Log("data count: " + thingsList.Count);

        List<GameObject> parents = new List<GameObject>();
        //Debug.Log("parents count: " + parents.Count);

        foreach (var item in thingsList)
        {
            if (item.parent == "none")
            {
                //Debug.Log("parent none?: " + item.parent);

                GameObject gameObject = new GameObject(item.name);

                gameObject.transform.position = markerPos + item.position.GetPosition();
                gameObject.transform.Rotate(markerRot + item.rotation.GetRotation());
                gameObject.transform.localScale = item.scale.GetScale();

                // insert into parents
                if (!CheckIfParentsExists(parents, item.name))
                {
                    parents.Add(gameObject);
                    //Debug.Log("new parent: " + gameObject.name + " " + gameObject.transform.position.ToString());
                    //Debug.Log("successfully create new parent");
                    //Debug.Log("parents count after creation: " + parents.Count);
                }

                GlobalConfig.OurWorldOrigin_Things = item;
                GlobalConfig.OurWorldOrigin_GameObject = gameObject;

                if (createAnchor) { CreateWorldAnchor(); }
            }

            else
            {
                //Debug.Log("parent ???: " + item.parent);

                GameObject gameObject;
                if (item.render) { gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube); }
                else { gameObject = new GameObject(); }
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
                    //Debug.Log("new parent: " + gameObject.name + " " + gameObject.transform.position.ToString());
                    //Debug.Log("successfully create new parent");
                    //Debug.Log("parents count after creation: " + parents.Count);
                }

                //Debug.Log("new object: " + gameObject.name + " " + gameObject.transform.position.ToString());
                //Debug.Log("successfully create new object relative to " + gameObject.transform.parent);

                if (item.render)
                {
                    // assign ColorManager
                    gameObject.AddComponent<ColorManager>();

                    // assign DataManager
                    gameObject.AddComponent<DataManager>();
                    gameObject.GetComponent<DataManager>().testingOnly = true;
                    gameObject.GetComponent<DataManager>().Test_AssignHiLoValue();

                    // start
                    StartCoroutine(Loop(gameObject));
                }
            }
        }
    }

    IEnumerator Loop(GameObject gO)
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            UpdatingDataManager(gO);
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

    private void DoStuffsUpdate(Vector3 markerPos, Vector3 markerRot)
    {
        GlobalConfig.OurWorldOrigin_GameObject.transform.position = markerPos + GlobalConfig.OurWorldOrigin_Things.position.GetPosition();
        GlobalConfig.OurWorldOrigin_GameObject.transform.rotation = Quaternion.identity;
        GlobalConfig.OurWorldOrigin_GameObject.transform.Rotate(markerRot + GlobalConfig.OurWorldOrigin_Things.rotation.GetRotation());
        GlobalConfig.OurWorldOrigin_GameObject.transform.localScale = GlobalConfig.OurWorldOrigin_Things.scale.GetScale();
    }

    private void CreateWorldAnchor()
    {
        if (OriginPrefabInstantiate != null)
        {
            GameObject gameObject = Instantiate(OriginPrefabInstantiate);
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
        float _alpha = 0.5f;
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
