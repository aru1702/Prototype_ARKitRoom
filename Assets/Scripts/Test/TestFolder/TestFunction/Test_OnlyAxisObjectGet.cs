using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_OnlyAxisObjectGet : MonoBehaviour
{
    // December 19th, 2022
    // axis we need:
    // - desk axis from my desk to tanner desk (area 1 2)
    // - all marker axis
    // - result: 47 -- 78, 112 -- 135

    public static List<float[]> AxisForDec192022(List<float[]> data)
    {
        List<float[]> fs = new();

        for (int i = 47; i <= 78; i++)
        {
            fs.Add(data[i]);
        }

        for (int i = 112; i <= 136; i++)
        {
            fs.Add(data[i]);
        }

        return fs;
    }

    public static List<float[]> MarkerAxisOnlyForDec212022(List<float[]> data)
    {
        List<float[]> fs = new();

        for (int i = 112; i <= 117; i++)
        {
            fs.Add(data[i]);
        }

        return fs;
    }

    public static List<float[]> MarkerAxisThenObject1To16ForDec212022(List<float[]> data)
    {
        List<float[]> fs = new();

        // marker
        for (int i = 112; i <= 117; i++)
        {
            fs.Add(data[i]);
        }

        // desk 1 to 12, and drawer 13 to 16
        for (int i = 47; i <= 62; i++)
        {
            fs.Add(data[i]);
        }

        return fs;
    }

    /// <summary>
    /// Marker, then desk 1 to 12, and drawer 13 to 16, and desk 17 to 32
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static List<float[]> MarkerAxisThenObject1To32ForJan042023(List<float[]> data)
    {
        List<float[]> fs = new();

        // marker
        for (int i = 112; i <= 117; i++)
        {
            fs.Add(data[i]);
        }

        // desk 1 to 12, and drawer 13 to 16, and desk 17 to 32
        for (int i = 47; i <= 78; i++)
        {
            fs.Add(data[i]);
        }

        return fs;
    }
}
