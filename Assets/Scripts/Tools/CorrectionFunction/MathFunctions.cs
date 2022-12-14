using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathFunctions : MonoBehaviour
{
    public static float Sigmoid(float value, bool inverted = false, float exp_scale = 1.0f)
    {
        var numerator = 1;
        var denominator = 1 + Mathf.Exp(exp_scale * -value);

        if (denominator == 0) denominator = 1;

        var result = numerator / denominator;

        return inverted ? (1 - result) : result;
    }

    public static float Tanh(float value, bool inverted = false, float exp_scale = 1.0f)
    {
        var p_d = Mathf.Exp(exp_scale * value);
        var n_d = Mathf.Exp(exp_scale * -value);

        var numerator = p_d - n_d;
        var denominator = p_d + n_d;

        if (denominator == 0) denominator = 1;

        var result = numerator / denominator;

        // This is temporary solution for NaN float value on Tanh function.
        // Since it calls while loop, we still don't know how much resources needed
        //  for the calculation.
        // Thus, this function can affect performance in future.
        int scl = 1; float diff;
        if (result < 0) diff = -0.01f;
        else diff = 0.01f;

        while (float.IsNaN(result))
        {
            result = Tanh(value + (diff * scl), inverted, exp_scale);
            scl++;
        }

        return inverted ? (1 - result) : result;
    }

    public static float Normalized(float value, List<float> array)
    {
        float sum = 0;
        foreach (var item in array)
        {
            sum += item;
        }
        if (sum == 0) sum = 1;

        return value / sum;
    }

    public static List<float> NormalizedMany(List<float> array)
    {
        float sum = 0;
        foreach (var item in array)
        {
            sum += item;
        }
        if (sum == 0) sum = 1;

        List<float> floats = new();
        foreach (var item in array)
        {
            floats.Add(item / sum);
        }

        return floats;
    }

    public static float Normalized(float value, float[] array)
    {
        float sum = 0;
        for (int i = 0; i < array.Length; i++)
        {
            sum += array[i];
        }
        if (sum == 0) sum = 1;

        return value / sum;
    }

    public static float[] NormalizedMany(float[] array)
    {
        float sum = 0;
        foreach (var item in array)
        {
            sum += item;
        }
        if (sum == 0) sum = 1;

        float[] floats = new float[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            floats[i] = array[i] / sum;
        }

        return floats;
    }

    public static float Max(List<float> array)
    {
        float f = -99999.0f;
        foreach (var a in array)
        {
            if (a > f) f = a;
        }
        return f;
    }

    public static float Max(float[] array)
    {
        float f = -99999.0f;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] > f) f = array[i];
        }
        return f;
    }

    public static float Min(List<float> array)
    {
        float f = 99999.0f;
        foreach (var a in array)
        {
            if (a < f) f = a;
        }
        return f;
    }

    public static float Min(float[] array)
    {
        float f = 99999.0f;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] < f) f = array[i];
        }
        return f;
    }

    public static string SIGMOID = "sigmoid";
    public static string TANH = "tanh";
}
