using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MappingV2TestManualInputMarkerData : MonoBehaviour
{
    [SerializeField]
    float m_Tick = 1.0f;

    [SerializeField]
    bool m_Start = false;

    [SerializeField]
    GameObject m_MappingV2;

    int i = 1;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Ticky());
    }

    IEnumerator Ticky()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_Tick);

            if (m_Start) MainFunc();
        }
    }

    void MainFunc()
    {
        int j = i - 1;
        string s = i == 1 ? "none" : ("img_" + j);
        MarkerLocation m = new(
            "img_" + i,
            new Vector3(RV(), RV(), RV()),
            Random.rotation.eulerAngles,
            new Vector3(RV(), RV(), RV()),
            Random.rotation.eulerAngles,
            s
            );
        m_MappingV2.GetComponent<MappingV2>().AddNewMarkerLocation(m);
        m_MappingV2.GetComponent<MappingV2>().MarkersInformationPanelMethod();
        i++;
    }

    float RV()
    {
        return Random.Range(0f, 5f);
    }
}
