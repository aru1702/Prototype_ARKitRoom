using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_TurnOnOffWorldCalib : MonoBehaviour
{
    public void TurnOnOffBtnPress()
    {
        bool status = GlobalConfig.WORLD_CALIBRATION_ONOFF;     // default false
        bool obj = GlobalConfig.WORLD_CALIBRATION_OBJ != null;  // default false, no obj

        if (obj) // if obj exists
        {
            if (status) // if turn on
            {
                // set turn off
                GlobalConfig.WORLD_CALIBRATION_OBJ.SetActive(false);
                GlobalConfig.WORLD_CALIBRATION_ONOFF = false;
            }
            else
            {
                // set turn on
                GlobalConfig.WORLD_CALIBRATION_OBJ.SetActive(true);
                GlobalConfig.WORLD_CALIBRATION_ONOFF = true;
            }
        }
        else    // if no objt
        {
            CreateSimilarObj();
        }
    }

    bool _IoTVC = true;

    public void ShowHideIoTVC()
    {
        if(_IoTVC)
        {
            _IoTVC = false;
            GlobalConfig.PlaySpaceOriginGO.SetActive(_IoTVC);
        }
        else
        {
            _IoTVC = true;
            GlobalConfig.PlaySpaceOriginGO.SetActive(_IoTVC);
        }
    }

    [SerializeField]
    GameObject m_LoadObjectManager;

    [SerializeField]
    GameObject m_DeskPrefab;

    //List<GameObject> cloneParents = new();
    List<GameObject> cloneObjects = new();

    /// <summary>
    /// To reset the whole origin and objects' location to back to original position.
    ///
    /// In correction activated condition, only objects are affected by compensation.
    /// In world calibration, we also need origin to be reset.
    /// Objects are sticked to their origin
    ///
    /// We reset the origin location first.
    /// Then we reset each object location, respectively to each that has been compensanated.
    /// </summary>
    public void ResetCalibration()
    {
        // this only change the world calibration origin
        GlobalConfig.WORLD_CALIBRATION_OBJ.transform.SetPositionAndRotation
            (GlobalConfig.PlaySpaceOriginGO.transform.position,
             GlobalConfig.PlaySpaceOriginGO.transform.rotation);

        // this change every object insider world calibration based on LoadObjectManager
        var loadObjects = m_LoadObjectManager
            .GetComponent<LoadObject_CatExample_2__NewARScene>()
            .GetMyObjects();

        for (int i = 0; i < loadObjects.Count; i++)
        {
            cloneObjects[i].transform.SetPositionAndRotation(
                loadObjects[i].transform.position,
                loadObjects[i].transform.rotation);
        }
    }

    void CreateSimilarObj()
    {
        if (m_LoadObjectManager == null) return;
        if (m_DeskPrefab == null) return;
        if (!m_LoadObjectManager.activeSelf) return;

        GameObject tempCloneWorld = new();
        GameObject tempWorld = GlobalConfig.PlaySpaceOriginGO;

        tempCloneWorld.transform.SetPositionAndRotation(
            tempWorld.transform.position,
            tempWorld.transform.rotation);

        tempCloneWorld.name = "WorldCalibration";

        GlobalConfig.WORLD_CALIBRATION_OBJ = tempCloneWorld;

        var loadObject = m_LoadObjectManager
            .GetComponent<LoadObject_CatExample_2__NewARScene>();

        //List<GameObject> basicParents = loadObject.GetMyParents();
        List<GameObject> basicObjects = loadObject.GetMyObjects();

        //foreach (var item in basicParents)
        //{
        //    var go = Instantiate(item);
        //    cloneParents.Add(go);
        //}

        foreach (var item in basicObjects)
        {
            //var go = Instantiate(m_DeskPrefab);

            var go = Instantiate(item);

            go.transform.SetPositionAndRotation(
                item.transform.position,
                item.transform.rotation);

            cloneObjects.Add(go);

            go.transform.SetParent(GlobalConfig.WORLD_CALIBRATION_OBJ.transform);

            SetAllChildIntoNewMats(go);
        }

        GlobalConfig.WORLD_CALIBRATION_OBJ.SetActive(true);
        GlobalConfig.WORLD_CALIBRATION_ONOFF = true;
    }

    public List<GameObject> GetCloneObject()
    {
        return cloneObjects;
    }


    //////////////////////////////////////
    /// COPIED FROM NonIoTDeviceManage ///
    //////////////////////////////////////

    [SerializeField]
    Material m_GreenTransparentMats;

    string _materialPath = "Materials/G_transparent";
    //bool _materialChange = false;

    public void AssignMaterial(GameObject go)
    {
        // change cube mats --> transparent
        //if (!_materialChange)
        //{
            if (m_GreenTransparentMats)
            {
                go.GetComponent<Renderer>().material = m_GreenTransparentMats;
            }
            else
            {
                Material newMats = Resources.Load<Material>(_materialPath);
                go.GetComponent<Renderer>().material = newMats;
            }
        //    _materialChange = true;
        //}
    }

    public static bool CheckIfHasRenderer(GameObject gameObject)
    {
        if (gameObject.GetComponent<Renderer>() == null) return false;
        if (gameObject.GetComponent<MeshRenderer>() == null) return false;

        return true;
    }

    ///////////////////////////////////////////////////////
    /// COPIED FROM LoadObject_CatExample_2__NewARScene ///
    ///////////////////////////////////////////////////////

    void SetAllChildIntoNewMats(GameObject prefab_gameObject)
    {
        // check if current gameobject has renderer
        if (CheckIfHasRenderer(prefab_gameObject))
        {
            AssignMaterial(prefab_gameObject);
        }

        // do for each child if have renderer
        int child = prefab_gameObject.transform.childCount;
        for (int i = 0; i < child; i++)
        {
            GameObject childGameObject = prefab_gameObject.transform.GetChild(i).gameObject;

            // do recursive call
            if (childGameObject.transform.childCount > 0)
                SetAllChildIntoNewMats(childGameObject);

            // check if child has renderer
            if (NonIoTDeviceManager.CheckIfHasRenderer(childGameObject))
            {
                AssignMaterial(childGameObject);
            }
        }
    }
}
