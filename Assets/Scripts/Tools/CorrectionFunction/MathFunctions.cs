using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathFunctions : MonoBehaviour
{
    public static float Sigmoid(float value, bool inverted = false)
    {
        var result = 1 / (1 + Mathf.Exp(-value));

        return inverted ? (1 - result) : result;
    }

    public static float Tanh(float value, bool inverted = false)
    {
        var p_d = Mathf.Exp(value);
        var n_d = Mathf.Exp(-value);
        var result = (p_d - n_d) / (p_d + n_d);

        return inverted ? (1 - result) : result;
    }

    public static float Normalized(float value, List<float> array)
    {
        float sum = 0;
        foreach (var item in array)
        {
            sum += item;
        }

        return value / sum;
    }

    public static float Normalized(float value, float[] array)
    {
        float sum = 0;
        for (int i = 0; i < array.Length; i++)
        {
            sum += array[i];
        }

        return value / sum;
    }

    public static string SIGMOID = "sigmoid";
    public static string TANH = "tanh";
}
