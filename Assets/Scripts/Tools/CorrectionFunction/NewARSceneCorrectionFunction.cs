using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NewARSceneCorrectionFunction : MonoBehaviour
{
    [SerializeField]
    GameObject m_LoadObjectManager;

    [SerializeField]
    GameObject m_ImageTargetCorrection;

    [SerializeField]
    bool m_UseCorrectionFunction = true, m_UseCameraTrajectoryData = false;

    [SerializeField]
    bool m_TestKeepOriginalData = false;

    [SerializeField]
    float m_UpdateCorrectionInterval = 1.0f;

    [SerializeField]
    [Tooltip("This value can be changed depend on math unit used.")]
    float m_AdjustedWeightCorrectionValue = 1.0f;

    GameObject m_Replica;
    List<GameObject> m_MarkerGroundTruth = new();
    List<Test_ImportTrueObjPos.DataObj> dataObjs = new();
    List<GameObject> oriObjList = new();
    List<Vector3> replicaOriObjListPos = new();

    [SerializeField]
    bool m_UseResetObjLocation = false;

    bool m_MarkerBeingUpdate;

    void Start()
    {
        if (m_LoadObjectManager == null) return;
        if (m_ImageTargetCorrection == null) return;

        m_UseCorrectionFunction = GlobalConfig.UseCorrectionMethod;

        Debug.Log("useCorrection: " + m_UseCorrectionFunction);
        if (!m_UseCorrectionFunction) return;

        Debug.Log("camTraUsed: " + m_UseCameraTrajectoryData);
        Debug.Log("keepOriData: " + m_TestKeepOriginalData);

        // duplicate if want to keep previous data
        if (m_TestKeepOriginalData)
        {
            m_Replica = Instantiate(GlobalConfig.PlaySpaceOriginGO);
            m_Replica.name = "Replica_" + GlobalConfig.PlaySpaceOriginGO.name;
            m_Replica.SetActive(false);
        }

        StartCoroutine(LoopMain());
    }

    IEnumerator LoopMain()
    {
        while(true)
        {
            yield return new WaitForSeconds(m_UpdateCorrectionInterval);
            Main_Version_One();
        }
    }

    public void Main()
    {
        //if (!m_UseCorrectionFunction) return;
        if (m_LoadObjectManager == null) return;

        m_UseCorrectionFunction = GlobalConfig.UseCorrectionMethod;

        Debug.Log("useCorrection: " + m_UseCorrectionFunction);

        if (!m_UseCorrectionFunction) return;

        Debug.Log("camTraUsed: " + m_UseCameraTrajectoryData);
        Debug.Log("keepOriData: " + m_TestKeepOriginalData);

        // get gameObj list from loadManager
        var objList = m_LoadObjectManager
            .GetComponent<LoadObject_CatExample_2__NewARScene>()
            .GetMyObjects();

        // duplicate if want to keep previous data
        if (m_TestKeepOriginalData)
        {
            var replica = Instantiate(GlobalConfig.PlaySpaceOriginGO);
            replica.name = "Replica_" + GlobalConfig.PlaySpaceOriginGO.name;
            replica.SetActive(false);
        }

        // convert them into DataObj model, then put them into new list
        List<Test_ImportTrueObjPos.DataObj> dataObjs = new();
        foreach (var obj in objList)
        {
            Test_ImportTrueObjPos.DataObj tempDataObj = new(obj.transform.position);
            dataObjs.Add(tempDataObj);
        }

        // get saved marker data from local
        string map = GlobalConfig.LOAD_MAP.ToString();
        string fileName = MappingV2.GetMarkerCalibrationFileName(map);
        string path = Path.Combine(Application.persistentDataPath, fileName);
        MarkerImportCsv mIC = new();
        var markers = mIC.GetMarkerLocationsSummarized(path);

        // get saved camera trajectory data from local      
        fileName = MappingV2.GetCameraTrajectoryFileName(map);
        path = Path.Combine(Application.persistentDataPath, fileName);
        CameraTrajectoryImportCsv cTIC = new();
        var cameras = cTIC.GetCameraTrajectories(path);

        // both marker dan camera data are based on DESIGNATED WORLD origin
        // our AR app works based on SLAM origin, which not matched
        // we have to convert them by putting on DESIGNATED WORLD, then get SLAM position
        var tObj = new GameObject();
        tObj.transform.SetParent(GlobalConfig.PlaySpaceOriginGO.transform);

        // for markers
        for (int i = 0; i < markers.Count; i++)
        {
            tObj.transform.localPosition = markers[i].GT_Position;
            tObj.transform.localEulerAngles = markers[i].GT_EulerAngle;
            Vector3 gt_pos = new(tObj.transform.position.x,
                                 tObj.transform.position.y,
                                 tObj.transform.position.z);
            Vector3 gt_eul = new(tObj.transform.eulerAngles.x,
                                 tObj.transform.eulerAngles.y,
                                 tObj.transform.eulerAngles.z);

            tObj.transform.localPosition = markers[i].C_Position;
            tObj.transform.localEulerAngles = markers[i].C_EulerAngle;
            Vector3 cr_pos = new(tObj.transform.position.x,
                                 tObj.transform.position.y,
                                 tObj.transform.position.z);
            Vector3 cr_eul = new(tObj.transform.eulerAngles.x,
                                 tObj.transform.eulerAngles.y,
                                 tObj.transform.eulerAngles.z);

            MarkerImportCsv.MarkerLocation newML =
                new(markers[i].name, gt_pos, gt_eul, cr_pos, cr_eul);
            markers[i] = newML;
        }

        // for cameras
        for (int i = 0; i < cameras.Count; i++)
        {
            tObj.transform.localPosition = cameras[i].Position;
            tObj.transform.localEulerAngles = cameras[i].EulerAngle;
            Vector3 pos = new(tObj.transform.position.x,
                              tObj.transform.position.y,
                              tObj.transform.position.z);
            Vector3 eul = new(tObj.transform.eulerAngles.x,
                              tObj.transform.eulerAngles.y,
                              tObj.transform.eulerAngles.z);

            CameraTrajectoryImportCsv.CameraTrajectory newCT =
                new(pos, eul, tObj.transform.rotation);
            cameras[i] = newCT;
        }
        Destroy(tObj);

        // correction START
        CorrectionWithMarker cWM = new();
        var altObjs = cWM.ProcessData(markers, cameras, dataObjs, m_UseCameraTrajectoryData);

        // alternate obj used
        for (int i = 0; i < objList.Count; i++)
        {
            objList[i].transform.position = altObjs[i];
        }

        ///////////////////////////////
        /// VISUALIZATION OF MARKER ///
        ///////////////////////////////

        //foreach (var item in markers)
        //{
        //    //var a = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    //a.transform.SetParent(GlobalConfig.PlaySpaceOriginGO.transform);
        //    //a.transform.localScale = new(0.05f, 0.05f, 0.05f);
        //    //a.transform.localPosition = item.GT_Position;

        //    var a = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    a.transform.localScale = new(0.05f, 0.05f, 0.05f);
        //    a.transform.position = item.GT_Position;
        //}


        //foreach (var item in markers)
        //{
        //    //var a = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    //a.transform.SetParent(GlobalConfig.PlaySpaceOriginGO.transform);
        //    //a.transform.localScale = new(0.05f, 0.05f, 0.05f);
        //    //a.transform.localPosition = item.C_Position;

        //    var a = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    a.transform.localScale = new(0.05f, 0.05f, 0.05f);
        //    a.transform.position = item.C_Position;
        //}

        //// test log
        //for (int i = 0; i < objList.Count; i++)
        //{
        //    Debug.Log("name: " + objList[i].name +
        //              ", real obj: " + LoggingVec3(objList[i].transform.position) +
        //              ", alt obj: " + LoggingVec3(altObjs[i]) );
        //}
    }

    /// <summary>
    /// Update in runtime
    /// </summary>
    public void Main_Version_One()
    {
        var update = m_ImageTargetCorrection
            .GetComponent<NewARSceneImageTrackingCorrection>()
            .GetImageTargetUpdateStatus();

        //Debug.Log("correction need update?: " + update);

        if (!update)
        {
            m_MarkerBeingUpdate = false;
            return;
        }

        var markers = m_ImageTargetCorrection
            .GetComponent<NewARSceneImageTrackingCorrection>()
            .GetImageTrackedList();
        if (markers.Count <= 0) return;

        //Debug.Log("correction marker count: " + markers.Count);

        // data based on online
        if (m_MarkerGroundTruth.Count <= 0) GetMarkerGroundTruth();

        // both marker dan camera data are based on DESIGNATED WORLD origin
        // our AR app works based on SLAM origin, which not matched
        // we have to convert them by putting on DESIGNATED WORLD, then get SLAM position
        var tObj = new GameObject();
        tObj.transform.SetParent(GlobalConfig.PlaySpaceOriginGO.transform);
        List<MarkerImportCsv.MarkerLocation> markerLocations = new();

        // for markers
        //Debug.Log("convert marker to proper form");
        for (int i = 0; i < markers.Count; i++)
        {
            //Debug.Log("convert marker num: " + markers[i]);
            Vector3 gT_pos;
            Vector3 gT_rot;

            foreach (var mGT in m_MarkerGroundTruth)
            {
                if (string.Equals(mGT.name, markers[i].custom_name))
                {
                    //Debug.Log("marker found! name: " + mGT.name);

                    var gT_m44 = GlobalConfig.GetM44ByGameObjRef(
                        mGT,
                        GlobalConfig.PlaySpaceOriginGO);
                    gT_pos = GlobalConfig.GetPositionFromM44(gT_m44);
                    gT_rot = GlobalConfig.GetEulerAngleFromM44(gT_m44);

                    tObj.transform.localPosition = gT_pos;
                    tObj.transform.localEulerAngles = gT_rot;

                    break;
                }
            }

            //Debug.Log("gt pos and rot");
            Vector3 gt_pos = new(tObj.transform.position.x,
                                 tObj.transform.position.y,
                                 tObj.transform.position.z);
            Vector3 gt_eul = new(tObj.transform.eulerAngles.x,
                                 tObj.transform.eulerAngles.y,
                                 tObj.transform.eulerAngles.z);

            //Debug.Log("cr pos and rot");
            tObj.transform.position = markers[i].custom_position;
            tObj.transform.eulerAngles = markers[i].custom_euler_rotation;
            Vector3 cr_pos = new(tObj.transform.position.x,
                                 tObj.transform.position.y,
                                 tObj.transform.position.z);
            Vector3 cr_eul = new(tObj.transform.eulerAngles.x,
                                 tObj.transform.eulerAngles.y,
                                 tObj.transform.eulerAngles.z);

            //Debug.Log("create an object");

            //Debug.Log("gt_pos: " + LoggingVec3(gt_pos));
            //Debug.Log("gt_eul: " + LoggingVec3(gt_eul));
            //Debug.Log("cr_pos: " + LoggingVec3(cr_pos));
            //Debug.Log("cr_eul: " + LoggingVec3(cr_eul));

            MarkerImportCsv.MarkerLocation newML =
                new(markers[i].custom_name, gt_pos, gt_eul, cr_pos, cr_eul);
            markerLocations.Add(newML);
            //Debug.Log("marker " + markers[i].custom_name + "success");
        }       
        Destroy(tObj);
        //Debug.Log("marker convertion success");

        // reset obj location
        if (m_UseResetObjLocation && m_TestKeepOriginalData && !m_MarkerBeingUpdate
                && replicaOriObjListPos.Count > 0)
        {
            Debug.Log("Reset obj pos");
            dataObjs.Clear();

            for (int i = 0; i < oriObjList.Count; i++)
            {
                oriObjList[i].transform.position = replicaOriObjListPos[i];
            }
        }

        // convert object into DataObj form
        //Debug.Log("convert original Obj to proper form");
        if (dataObjs.Count <= 0)
        {
            // get gameObj list from loadManager
            oriObjList = m_LoadObjectManager
                .GetComponent<LoadObject_CatExample_2__NewARScene>()
                .GetMyObjects();

            var s1 = "before: \n";
            foreach (var obj in oriObjList)
            {
                var dataObj = new Test_ImportTrueObjPos.DataObj(obj.transform.position);
                dataObjs.Add(dataObj);

                s1 += obj.name + ", " + dataObj.Position + "\n";

                replicaOriObjListPos.Add(
                    new(obj.transform.position.x,
                        obj.transform.position.y,
                        obj.transform.position.z));
            }
            //if (!m_MarkerBeingUpdate) Debug.Log(s1);
        }
        //Debug.Log("Original obj convertion success");

        // correction START
        //Debug.Log("correction start");
        CorrectionWithMarker cWM = new();
        var altObjs = cWM.ProcessData(
            markerLocations,
            null,
            dataObjs,
            false,
            m_AdjustedWeightCorrectionValue);
        //Debug.Log("correction finish");

        var s2 = "after: \n";
        // alternate obj used
        for (int i = 0; i < oriObjList.Count; i++)
        {
            s2 += oriObjList[i].name + ", " + altObjs[i] + "\n";
            oriObjList[i].transform.position = altObjs[i];
        }
        //Debug.Log("alternate Obj position");
        //if (!m_MarkerBeingUpdate) Debug.Log(s2);

        var s = "";
        foreach (var m in markerLocations)
        {
            s += m.GT_Position + "\n";
            s += m.GT_EulerAngle + "\n";
            s += m.C_Position + "\n";
            s += m.C_EulerAngle + "\n";
            s += "\n";
        }
        Debug.Log(s);

        m_MarkerBeingUpdate = true;
    }
 
    string LoggingVec3(Vector3 v)
    {
        string r = "(";
        r += v.x + ", ";
        r += v.y + ", ";
        r += v.z;
        r += ")";
        return r;
    }

    void GetMarkerGroundTruth()
    {
        if (m_LoadObjectManager == null) return;

        var scriptL = m_LoadObjectManager
            .GetComponent<LoadObject_CatExample_2__NewARScene>();
        foreach (var item in scriptL.GetMyParents())
        {
            //Debug.Log("name: " + item.name);

            string[] names = item.name.Split("_");
            if (names[0] == "img")
                m_MarkerGroundTruth.Add(item);
        }

        //Debug.Log("how many GT: " + m_MarkerGroundTruth.Count);
    }
}
