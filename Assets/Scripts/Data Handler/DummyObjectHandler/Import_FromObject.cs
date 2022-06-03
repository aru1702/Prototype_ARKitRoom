using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Import_FromObject
{
    // constant string which data being kept
    const string path = "Database/VirtualPosition/desk_object";

    // list of class that we import
    private static List<MyObject> _myObjects = new();

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
            // 2,3,4: l,h,w
            // 5, 6: origin type, descriptor
            // 7, 8: prefab type, specila
            // 9: comment

            MyObject origin = new(
                    csvData[0],                 // name
                    csvData[1],                 // parent
                    float.Parse(csvData[2]),    // l
                    float.Parse(csvData[3]),    // h
                    float.Parse(csvData[4]),    // w
                    csvData[5], csvData[6],     // origin
                    csvData[7], csvData[8],     // prefab
                    csvData[9]);                // comment

            _myObjects.Add(origin);
        }
    }

    public static List<MyObject> GetMyObjectsList()
    {
        if (_myObjects.Count <= 0) { Import(); }
        return _myObjects;
    }
}
