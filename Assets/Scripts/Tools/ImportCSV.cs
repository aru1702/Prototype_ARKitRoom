using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

// src: https://gist.github.com/takumifukasawa/f4a4d73143e14ec66e13c992b2d0dd65

public class ImportCSV
{
    public static List<string[]> getData(string path, bool skipTitle = false, string splitStr = ",")
    {
        if (path == "")
        {
            throw new Exception("should be pass csv path.");
        }
        //Debug.Log(path);

        List<string[]> data = new List<string[]>();
        TextAsset csv = Resources.Load<TextAsset>(path);
        //Debug.Log(csv.text);
        StringReader reader = new StringReader(csv.text);

        bool skipHeader = true;
        if (skipTitle) skipHeader = false;

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            string[] items = line.Split(splitStr.ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
            if (skipHeader) { data.Add(items); } else { skipHeader = true; }
        }

        return data;
    }
}