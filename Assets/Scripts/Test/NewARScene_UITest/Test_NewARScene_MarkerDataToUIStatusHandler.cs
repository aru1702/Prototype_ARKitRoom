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

        var text = ExtractCustomTransformList(markers);
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
}
