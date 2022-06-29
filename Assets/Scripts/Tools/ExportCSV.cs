using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ExportCSV : MonoBehaviour
{
    /// <summary>
    /// One line saved with long string, must already delimited with (,)
    /// </summary>
    /// <param name="path"></param>
    /// <param name="dataArray"></param>
    public static void exportData(string path, string dataArray)
    {
        // get path
        StreamWriter writer = new(path);

        // write data
        writer.WriteLine(dataArray);
        writer.Flush();
        writer.Close();
    }

    /// <summary>
    /// One line saved with array, default delimiter is (,)
    /// </summary>
    /// <param name="path"></param>
    /// <param name="dataArray"></param>
    public static void exportData(string path, string[] dataArray, string delimiter = ",")
    {
        // get path
        StreamWriter writer = new(path);

        // read each string array into one line
        string strLong = "";
        for (int i = 0; i < dataArray.Length; i++)
        {
            strLong += dataArray[i];

            // add comma
            if (i < dataArray.Length - 1)
            {
                strLong += delimiter;
            }
        }

        // write data
        writer.WriteLine(strLong);
        writer.Flush();
        writer.Close();
    }

    /// <summary>
    /// Many lines save for List of string, default delimiter is (,)
    /// </summary>
    /// <param name="path"></param>
    /// <param name="dataMany"></param>
    public static void exportData(string path, List<string[]> dataMany, string delimiter = ",")
    {
        // get path
        StreamWriter writer = new(path);

        // read each string array
        foreach (var i in dataMany)
        {
            string strLong = "";
            for (int j = 0; j < i.Length; j++)
            {
                strLong += i[j];

                // add comma
                if (j < i.Length - 1)
                {
                    strLong += delimiter;
                }
            }

            // write data
            writer.WriteLine(strLong);
        }

        // close writer
        writer.Flush();
        writer.Close();
    }
}
