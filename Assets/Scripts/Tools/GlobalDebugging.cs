using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDebugging
{
    public static void DebugLogListVector(List<Vector3> vectors, string context)
    {
        var s = context + ": \n";
        for (int i = 0; i < vectors.Count; i++)
        {
            s += i + ", " + LoggingVec3(vectors[i]) + "\n";
        }
        Debug.Log(s);
    }

    public static void DebugLogListFloatArray(List<float[]> floats, string context)
    {
        var s = context + ": \n";
        for (int i = 0; i < floats.Count; i++)
        {
            s += i + ", " + LoggingFloat(floats[i]) + "\n";
        }
        Debug.Log(s);
    }

    public static void DebugLogListFloat(List<float> floats, string context)
    {
        var s = context + ": \n";
        for (int i = 0; i < floats.Count; i++)
        {
            s += i + ", " + floats[i] + "\n";
        }
        Debug.Log(s);
    }

    static string LoggingFloat(float[] array)
    {
        string r = "[";

        for (int i = 0; i < array.Length; i++)
        {
            r += array[i] + ", ";
        }

        r += "]";
        return r;
    }

    static string LoggingVec3(Vector3 v)
    {
        string r = "(";
        r += v.x + ", ";
        r += v.y + ", ";
        r += v.z;
        r += ")";
        return r;
    }
}
