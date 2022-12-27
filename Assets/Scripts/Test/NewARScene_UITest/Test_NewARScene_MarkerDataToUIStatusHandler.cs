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

        var text = VersionTwoConfiguration(); 
        text += NewLineTwoTimes();
        text += ExtractCustomTransformList(markers);

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

            str += "name: " + trf.custom_name + ", ";
            str += "pos: " + trf.custom_position.ToString() + ", ";
            str += "world pos: " + new_pos.ToString();
            str += "\n";

            Destroy(go);
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
