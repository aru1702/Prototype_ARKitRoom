using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Test_NewARScene_ImgTracking_SaveWeightData : MonoBehaviour
{
    List<string[]> m_CSVValue;
    bool m_UpdateData = false;

    private void Start()
    {
        m_CSVValue = new();
    }

    public void AddCSVValue(string[] values)
    {
        if (!m_UpdateData)
            m_CSVValue.Add(values);
    }

    public int GetCSVValueCount()
    {
        return m_CSVValue.Count;
    }

    public void SetUpdateData(bool value) { m_UpdateData = value; }
    public bool GetUpdateData() { return m_UpdateData; }

    public void SaveData()
    {
        if (GetCSVValueCount() > 0)
        {

            var date = GlobalConfig.GetNowDateandTime();
            var map = GlobalConfig.LOAD_MAP.ToString();
            var file = date + "_Test_NewARScene_ImgTracking_SaveWeightData_" + map + ".csv";
            var path = Path.Combine(Application.persistentDataPath, file);
            ExportCSV.exportData(path, m_CSVValue);
        }

        if (AllDebugInput.Count > 0)
        {
            SaveDebugInput();
        }
    }

    void AddHeaderAsFollowDec19()
    {
        int base_v = 4;
        int table = 32;
        int img = 6;
        int side = 19;

        int size = base_v + table + img + side;

        List<string> values = new();

        values.Add("date");
        values.Add("context");
        values.Add("marker_name");
        values.Add("time_elapse");

        int base_array = base_v;

        for (int i = 0; i < table; i++)
        {
            var th = i + 1;
            values.Add("table_axis_" + th);
        }

        base_array = base_v + table;

        for (int i = 0; i < img; i++)
        {
            var th = i + 1;
            values.Add("img_axis_" + th);
        }

        base_array = base_v + table + img;

        for (int i = 0; i < side; i++)
        {
            var th = i + 1;
            values.Add("img_axis_" + th);
        }

        AddCSVValue(values.ToArray());
    }

    public void InsertDataAsFollowDec19(string context, List<CameraTrajectoryData> camera_markers, List<float[]> weights)
    {
        if (GetCSVValueCount() <= 0) AddHeaderAsFollowDec19();

        if (m_UpdateData) return;

        var date = GlobalConfig.GetNowDateandTime();
        Debug.Log(weights.Count);
        if (weights.Count > 0) Debug.Log(weights[0].Length);

        for (int i = 0; i < camera_markers.Count; i++)
        {
            var marker_name = camera_markers[i].Marker_name;
            var marker_time = camera_markers[i].Camera_travel_time;

            int base_v = 4;
            int table = 32;
            int img = 6;
            int side = 19;

            int size = base_v + table + img + side;

            List<string> values = new();

            values.Add(date);
            values.Add(context);
            values.Add(marker_name);
            values.Add(marker_time.ToString());

            int base_array = base_v;

            // axis start from 47 to 78
            int start = 47;
            for (int j = 0; j < table; j++)
            {
                values.Add(weights[start + j][i].ToString());
            }

            base_array = base_v + table;

            // img start from 112 to 117
            start = 112;
            for (int j = 0; j < img; j++)
            {
                values.Add(weights[start + j][i].ToString());
            }

            base_array = base_v + table + img;

            // side start from 118 to 136
            start = 118;
            for (int j = 0; j < side; j++)
            {
                values.Add(weights[start + j][i].ToString());
            }

            AddCSVValue(values.ToArray());
        }

        //SetUpdateData(true);
    }

    List<string> AllDebugInput = new();
    string LoggingFloat(float[] array)
    {
        string r = "[";

        for (int i = 0; i < array.Length; i++)
        {
            r += array[i] + ", ";
        }

        r += "]";
        return r;
    }
    public void AddAllDebugInputListFloatArray(List<float[]> floats, string context)
    {
        var s = context + ": \n";
        for (int i = 0; i < floats.Count; i++)
        {
            s += i + ", " + LoggingFloat(floats[i]) + "\n";
        }
        AllDebugInput.Add(s);
    }
    public void AddAllDebugInputListFloat(List<float> floats, string context)
    {
        var s = context + ": \n";
        for (int i = 0; i < floats.Count; i++)
        {
            s += i + ", " + floats[i] + "\n";
        }
        AllDebugInput.Add(s);
    }
    public void AddIterationPart(int iteration)
    {
        var s = "//////////////////////////////" +
                "======  ITERATION " + iteration + " ======" +
                "//////////////////////////////";
        AllDebugInput.Add(s);
    }
    void SaveDebugInput()
    {
        // get path
        var date = GlobalConfig.GetNowDateandTime();
        var map = GlobalConfig.LOAD_MAP.ToString();
        var file = date + "_Test_NewARScene_AllDebugInput_" + map + ".txt";
        var path = Path.Combine(Application.persistentDataPath, file);
        StreamWriter writer = new(path);

        // read each string array
        foreach (var i in AllDebugInput)
        {            
            // write data
            writer.WriteLine(i + "\n\n");
        }

        // close writer
        writer.Flush();
        writer.Close();
    }
}
