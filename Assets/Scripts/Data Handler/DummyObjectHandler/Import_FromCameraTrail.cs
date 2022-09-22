using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Import_FromCameraTrail
{
    static List<CameraTrail> cameraTrails = new();

    static void Main()
    {
        // file path
        string map = GlobalConfig.LOAD_MAP.ToString();
        string fileName = "RecordedSLAMAdjusted_" + map + ".csv";
        string path = Path.Combine(Application.persistentDataPath, fileName);

        // get data from csv
        List<string[]> data = ImportCSV.getDataOutsource(path, true);

        // put into Things class
        foreach (var csvData in data)
        {
            Vector3 pos = new(
                float.Parse(csvData[1]),
                float.Parse(csvData[2]),
                float.Parse(csvData[3]));
            Quaternion rot = new(
                float.Parse(csvData[4]),
                float.Parse(csvData[5]),
                float.Parse(csvData[6]),
                float.Parse(csvData[7]));

            CameraTrail ct = new(csvData[0], pos, rot, csvData[8]);
            cameraTrails.Add(ct);
        }
    }

    public static List<CameraTrail> GetCameraTrailList()
    {
        if (cameraTrails.Count <= 0) { Main(); }
        return cameraTrails;
    }
}
