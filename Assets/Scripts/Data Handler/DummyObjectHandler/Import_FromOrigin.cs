using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Import_FromOrigin
{
    // constant string which data being kept
    const string path = "Database/VirtualPosition/desk_origin";

    // list of class that we import
    private static List<MyOrigin> _myOrigins = new List<MyOrigin>();

    private static void Import()
    {
        // get data from csv
        List<string[]> data = ImportCSV.getData(path, true);

        // put into class
        foreach (var csvData in data)
        {
            // based on the csv format
            // 0: name
            // 1: parent
            // 2,3,4: pos x,y,z
            // 5,6,7: rot x,y,z
            // 8: comment

            Vector3 position = new(float.Parse(csvData[2]),
                                   float.Parse(csvData[3]),
                                   float.Parse(csvData[4]));

            Vector3 eulerRotation = new(float.Parse(csvData[5]),
                                        float.Parse(csvData[6]),
                                        float.Parse(csvData[7]));

            MyOrigin origin = new(
                    csvData[0],
                    csvData[1],
                    position,
                    eulerRotation,
                    csvData[8]);

            _myOrigins.Add(origin);
        }
    }

    public static List<MyOrigin> GetMyOriginsList()
    {
        if (_myOrigins.Count <= 0) { Import(); }
        return _myOrigins;
    }

    public static List<MyOrigin> ConvertFromListString(List<string[]> list_string)
    {
        // put into class
        foreach (var csvData in list_string)
        {
            // based on the csv format
            // 0: name
            // 1: parent
            // 2,3,4: pos x,y,z
            // 5,6,7: rot x,y,z
            // 8: comment

            Vector3 position = new(float.Parse(csvData[2]),
                                   float.Parse(csvData[3]),
                                   float.Parse(csvData[4]));

            Vector3 eulerRotation = new(float.Parse(csvData[5]),
                                        float.Parse(csvData[6]),
                                        float.Parse(csvData[7]));

            MyOrigin origin = new(
                    csvData[0],
                    csvData[1],
                    position,
                    eulerRotation,
                    csvData[8]);

            _myOrigins.Add(origin);
        }

        return _myOrigins;
    }
}
