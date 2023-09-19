using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The purpose of this script is to save any likely data into single csv file.
/// There is no significant form, even in csv file, it has no feature limitation.
/// Thus, processing the result data might need cleaning first.
///
/// You can throw any data into this script, as it will save into global static.
///
/// Notes:
/// - If you need more field, just copy-paste and add number.
/// - It works independently without class reference, should not be invoke as new class.
/// </summary>

// Still need MonoBehavior in order to be called by Button on GUI
public class GlobalSaveData: MonoBehaviour
{
    static List<string[]> data = new();

    /// <summary>
    /// If you use add_time, there will be loop process inside this function.
    /// We suggest you already add the timestamp before it.
    /// </summary>
    public static void WriteData(string[] new_data, bool add_time = false)
    {
        if (add_time)
        {
            var time = GlobalConfig.GetNowDateandTime(true);
            string[] temp_data = new string[new_data.Length];
            for (int i = 0; i < new_data.Length; i++)
            {
                temp_data[i] = new_data[i];
            }
            new_data = new string[temp_data.Length + 1];
            new_data[0] = time;
            for (int i = 0; i < temp_data.Length; i++)
            {
                new_data[i + 1] = temp_data[i];
            }
        }

        data.Add(new_data);
    }

    /// <summary>
    /// Use this to save into csv file.
    /// </summary>
    public void SaveData(string context)
    {
        var date = GlobalConfig.GetNowDateandTime(true);
        var map = GlobalConfig.LOAD_MAP.ToString();
        var file = date + "_" + context + "_Map" + map + ".csv";
        var path = System.IO.Path.Combine(Application.persistentDataPath, file);
        ExportCSV.exportData(path, data);
    }


    ////////////////////
    /// EXAMPLE
    //////////////


    //static List<string[]> data_2 = new();
    static List<string[]> data_2;               // activate the up one

    /// <summary>
    /// If you use add_time, there will be loop process inside this function.
    /// We suggest you already add the timestamp before it.
    /// </summary>
    public static void WriteData_2(string[] new_data, bool add_time = false)
    {
        if (add_time)
        {
            var time = GlobalConfig.GetNowDateandTime(true);
            string[] temp_data = new string[new_data.Length];
            for (int i = 0; i < new_data.Length; i++)
            {
                temp_data[i] = new_data[i];
            }
            new_data = new string[temp_data.Length + 1];
            new_data[0] = time;
            for (int i = 0; i < temp_data.Length; i++)
            {
                new_data[i + 1] = temp_data[i];
            }
        }

        data_2.Add(new_data);
    }

    /// <summary>
    /// Use this to save into csv file.
    /// </summary>
    public void SaveData_2(string context)
    {
        var date = GlobalConfig.GetNowDateandTime(true);
        var map = GlobalConfig.LOAD_MAP.ToString();
        var file = date + "_" + context + "_Map" + map + ".csv";
        var path = System.IO.Path.Combine(Application.persistentDataPath, file);
        ExportCSV.exportData(path, data_2);
    }
}
