using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System.Linq;

// src: https://gist.github.com/takumifukasawa/f4a4d73143e14ec66e13c992b2d0dd65

public class ImportCSV
{
    /// <summary>
    /// Read whole csv file per line with delimiter, used in Import from CSV
    /// </summary>
    /// <param name="path">String data path</param>
    /// <param name="skipFirstLine">Skip header, default is false</param>
    /// <param name="splitStr">Delimiter option</param>
    /// <returns></returns>
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

        bool hasSkipHeader = true;
        if (skipFirstLine) hasSkipHeader = false;

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            string[] items = line.Split(splitStr.ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
            if (hasSkipHeader) { data.Add(items); } else { hasSkipHeader = true; }
        }

        return data;
    }

    /// <summary>
    /// Read csv file single line with delimited, used for import the recognizable image position
    /// </summary>
    /// <param name="path">Data path</param>
    /// <param name="skipFirstLine">Skip header, default is false</param>
    /// <param name="splitStr">Delimiter option</param>
    /// <returns></returns>
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

    public static List<string[]> GetDataFromRawString(string input, bool skip_header = false, string delimiter = ",", string new_line = "\n")
    {
        if (input == "")
        {
            throw new Exception("No data input");
        }

        List<string[]> data = new();

        bool hasSkipHeader = true;
        if (skip_header) hasSkipHeader = false;

        string[] input_nl = input.Split(new_line);
        for (int i = 0; i < input_nl.Length; i++)
        {
            string[] input_d = input_nl[i].Split(delimiter);
            if (hasSkipHeader) { data.Add(input_d); } else { hasSkipHeader = true; }
        }

        //while (reader.Peek() != -1)
        //{
        //    string line = reader.ReadLine();
        //    string[] items = line.Split(splitStr.ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
        //    if (hasSkipHeader) { data.Add(items); } else { hasSkipHeader = true; }
        //}

        return data;
    }


}