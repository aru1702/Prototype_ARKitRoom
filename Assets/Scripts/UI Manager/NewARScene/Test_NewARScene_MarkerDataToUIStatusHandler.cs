using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_NewARScene_MarkerDataToUIStatusHandler : MonoBehaviour
{
    [SerializeField]
    GameObject m_MarkerStatusHandler;

    [SerializeField]
    GameObject m_ImageRecogCorrectionHandler;

    [SerializeField]
    GameObject m_VersionOneRot;

    [SerializeField]
    float m_IntervalDataUpdate = 1.0f;

    void Start()
    {
        StartCoroutine(LoopMain());
    }

    IEnumerator LoopMain()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_IntervalDataUpdate);
            MainFunc();
        }
    }

    void MainFunc()
    {
        var uiHandler = m_MarkerStatusHandler
            .GetComponent<Test_NewARScene_MarkerStatusHandler>();

        var markerHandler = m_ImageRecogCorrectionHandler
            .GetComponent<NewARSceneImageTrackingCorrection>();

        var markers = markerHandler.GetImageTrackedList();

        if (markers.Count <= 0)
        {
            //Debug.Log(GlobalConfig.GetNowDateandTime() + ", No markers in sight!");
            
        }

        var text = "";
        //text += VersionTwoConfiguration(); 
        //text += NewLineTwoTimes();

        //text += ExtractCustomTransformList(markers);

        // this will change with
        if (GlobalConfig.CorrectionFunctionVersion == (int)GlobalConfig.VER.Version1BLast + 1)
        {
            var ML_data = m_VersionOneRot
                .GetComponent<CorrectionFunctions.VersionOneBLastMarker>()
                .GetMarkerLocations();
            text += ExtractMarkerLocation(ML_data);
        }
        else if (GlobalConfig.CorrectionFunctionVersion == (int)GlobalConfig.VER.Version1BAvg + 1)
        {
            var ML_data = m_VersionOneRot
                .GetComponent<CorrectionFunctions.VersionOneBAvgWMarker>()
                .GetMarkerLocations();
            text += ExtractMarkerLocation(ML_data);
        }

        uiHandler.SetMarkerStatusText(text);
    }

    string ExtractCustomTransformList(List<CustomTransform> customTransforms)
    {
        string str = "";

        foreach (var trf in customTransforms)
        {
            GameObject go = new();
            go.transform.position = trf.custom_position;
            var m44 = GlobalConfig.GetM44ByGameObjRef(go, GlobalConfig.PlaySpaceOriginGO);
            var new_pos = GlobalConfig.GetPositionFromM44(m44);
            var new_rot = GlobalConfig.GetEulerAngleFromM44(m44);


            //str += "name: " + trf.custom_name + ", ";
            //str += "pos: " + trf.custom_position.ToString() + ", ";
            //str += "world pos: " + new_pos.ToString();
            //str += "\n";

            str += trf.custom_name + ", ";
            str += "p: " + trf.custom_position.ToString() + ", ";
            str += "wp: " + new_pos.ToString() + ", ";
            str += "r: " + trf.custom_euler_rotation.ToString() + ", ";
            str += "wr: " + new_rot.ToString() + ", ";
            str += "\n";

            Destroy(go);
        }

        return str;
    }

    string ExtractMarkerLocation(List<MarkerLocation> markerLocations)
    {
        string str = "";

        foreach (var mL in markerLocations)
        {
            str += mL.Marker_name + ", ";
            str += "GTP: " + mL.GT_Position.ToString() + ", ";
            str += "RTP: " + mL.C_Position.ToString() + ", ";
            str += "GTR: " + mL.GT_EulerAngle.ToString() + ", ";
            str += "RTR: " + mL.C_EulerAngle.ToString();
            str += "\n";
        }

        return str;
    }

    string VersionTwoConfiguration()
    {
        string
        str  = "Object to marker weight scalar: " + GlobalConfig.OTM_SCALAR + "\n";
        str += "Object to marker priority: " + (int) (GlobalConfig.OTM_PRIORITY * 100) + "%\n";
        str += "Camera time usage weight scalar: " + GlobalConfig.CTTtime_SCALAR + "\n";
        str += "Camera time usage priority: " + (int)(GlobalConfig.CTTtime_PRIORITY * 100) + "%";
        return str;
    }

    string NewLineTwoTimes()
    {
        return "\n\n";
    }
}
