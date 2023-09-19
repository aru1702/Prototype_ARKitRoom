using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSimulationVer1A : MonoBehaviour
{
    [SerializeField]
    string m_MarkerDataPath, m_ObjectGTDataPath, m_ObservationOrderDataPath;

    // Start is called before the first frame update
    void Start()
    {
        List<string[]> marker_data = ImportCSV.getDataOutsource(m_MarkerDataPath, true, ",");
        Debug.Log(GlobalDebugging.LoggingListofStringArray(marker_data));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
