using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_ImportTrueObjPos : MonoBehaviour
{
    List<DataObj> datas = new();
    string dataPath;

    [SerializeField]
    string m_predefinedPath;

    [SerializeField]
    bool m_testMode = false;

    public void ImportData(string path = "")
    {
        if (m_testMode)
        {
            if (string.IsNullOrEmpty(m_predefinedPath))
            {
                Debug.LogError("No defined path!");
                return;
            }

            dataPath = m_predefinedPath;
        }
        else
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("No defined path!");
                return;
            }

            dataPath = path;
        }

        List<string[]> data = ImportCSV.getData(dataPath, true);

        // put into class
        foreach (var csvData in data)
        {
            Vector3 position = new(float.Parse(csvData[1]),
                                   float.Parse(csvData[2]),
                                   float.Parse(csvData[3]));

            var cT = new DataObj();
            cT.Position = position;

            datas.Add(cT);
        }
    }

    public List<DataObj> GetObjPoss(string path = "")
    {
        if (datas.Count <= 0) ImportData(path);

        return datas;
    }

    public class DataObj
    {
        public Vector3 Position { get; set; }
        public DataObj(Vector3 Position)
        {
            this.Position = Position;
        }
        public DataObj() { }
    }
}
