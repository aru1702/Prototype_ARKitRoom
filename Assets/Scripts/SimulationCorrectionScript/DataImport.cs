using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataImport : MonoBehaviour
{
    [SerializeField]
    string m_MyOriginPath, m_MyObjectPath;

    [SerializeField]
    bool debug = false;

    List<string[]> MyOrigins = new List<string[]>();
    List<string[]> MyObjects = new List<string[]>();

    // Start is called before the first frame update
    void Start()
    {
        MyOrigins = ImportCSV.getDataOutsource(m_MyOriginPath, true, ",");
        if (debug) Debug.Log(GlobalDebugging.LoggingListofStringArray(MyOrigins));

        MyObjects = ImportCSV.getDataOutsource(m_MyObjectPath, true, ",");
        if (debug) Debug.Log(GlobalDebugging.LoggingListofStringArray(MyObjects));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<string[]> GetMyOrigins()
    {
        return MyOrigins;
    }

    public List<string[]> GetMyObjects()
    {
        return MyObjects;
    }
}
