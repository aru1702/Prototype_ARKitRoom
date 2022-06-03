using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Import_FromThings
{
    const string path = "Database/VirtualPosition/things2";
    private static List<Things> thingsList = new List<Things>();

    private static void Main()
    {
        // get data from csv
        List<string[]> data = ImportCSV.getData(path, true);

        // put into Things class
        foreach (var csvData in data)
        {
            Things.Position thing_Pos = new Things.Position(
                float.Parse(csvData[2]),
                float.Parse(csvData[3]),
                float.Parse(csvData[4])
                );

            Things.Rotation thing_Rot = new Things.Rotation(
                float.Parse(csvData[5]),
                float.Parse(csvData[6]),
                float.Parse(csvData[7])
                );

            Things.Scale thing_Sca = new Things.Scale(
                float.Parse(csvData[8]),
                float.Parse(csvData[9]),
                float.Parse(csvData[10])
                );

            Things things = new Things(
                    csvData[0],
                    csvData[1],
                    thing_Pos,
                    thing_Rot,
                    thing_Sca,
                    bool.Parse(csvData[11]));

            thingsList.Add(things);
        }
    }

    public static List<Things> GetThingsList()
    {
        if (thingsList.Count <= 0) { Main(); }
        return thingsList;
    }
}
