using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System.Linq;

// src: https://gist.github.com/takumifukasawa/f4a4d73143e14ec66e13c992b2d0dd65

public class ImportCSV
{
    [Tooltip("Read whole csv file per line with delimiter")]
    public static List<string[]> getData(string path, bool skipFirstLine = false, string splitStr = ",")
    {
        if (path == "")
        {
            throw new Exception("should be pass csv path.");
        }
        //Debug.Log(path);

        List<string[]> data = new();
        TextAsset csv = Resources.Load<TextAsset>(path);
        //Debug.Log(csv.text);
        StringReader reader = new StringReader(csv.text);

        bool skipHeader = true;
        if (skipFirstLine) skipHeader = false;

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            string[] items = line.Split(splitStr.ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
            if (skipHeader) { data.Add(items); } else { skipHeader = true; }
        }

        return data;
    }

    [Tooltip("Read csv file single line with delimited")]
    public static List<string> getDataPersistentPath(string path, bool skipFirstLine = false, string splitStr = ",")
    {
        if (path == "")
        {
            throw new Exception("should be pass csv path.");
        }

        var tempText = File.ReadAllText(path);
        Debug.Log(tempText);

        var strSplit = tempText.Split(splitStr);
        return strSplit.ToList();

        //reader.Close();
    }
}