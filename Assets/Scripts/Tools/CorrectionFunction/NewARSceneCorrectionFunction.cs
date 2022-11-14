using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NewARSceneCorrectionFunction : MonoBehaviour
{
    [SerializeField]
    GameObject m_LoadObjectManager;

    [SerializeField]
    bool m_UseCorrectionFunction = true, m_UseCameraTrajectoryData = false;

    [SerializeField]
    bool m_TestKeepOriginalData = false;

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

 
    string LoggingVec3(Vector3 v)
    {
        string r = "(";
        r += v.x + ", ";
        r += v.y + ", ";
        r += v.z;
        r += ")";
        return r;
    }
}
