using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathFunctions : MonoBehaviour
{
    /// <summary>
    /// Sigmoid function of given value, which is 1 / (1 - exp(-value)).
    /// </summary>
    /// <param name="value">Given value.</param>
    /// <param name="inverted">Invert the end result (1 - result).</param>
    /// <param name="exp_scale">Scalar parameter for exponential power.</param>
    /// <returns>Result of sigmoid function.</returns>
    public static float Sigmoid(float value, bool inverted = false, float exp_scale = 1.0f)
    {
        var numerator = 1;
        var denominator = 1 + Mathf.Exp(exp_scale * -value);

        if (denominator == 0) denominator = 1;

        var result = numerator / denominator;

        return inverted ? (1 - result) : result;
    }

    /// <summary>
    /// Tanh function of given value, which is (exp(value) - exp(-value)) / (exp(value) + exp(-value)).
    /// </summary>
    /// <param name="value">Given value.</param>
    /// <param name="inverted">Invert the end result (1 - result).</param>
    /// <param name="exp_scale">Scalar parameter for exponential power.</param>
    /// <returns>Result of sigmoid function.</returns>
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

    public static string SIGMOID = "sigmoid";
    public static string TANH = "tanh";

    /// <summary>
    /// Normalized one value over list form array.
    /// </summary>
    /// <param name="value">Given value.</param>
    /// <param name="array">Given array in list form.</param>
    /// <returns>Result of normalized value.</returns>
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

    /// <summary>
    /// Normalized all values in list form array.
    /// </summary>
    /// <param name="array">Given array in list form.</param>
    /// <returns>Result of normalized array.</returns>
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

    /// <summary>
    /// Normalized one value over array.
    /// </summary>
    /// <param name="value">Given value.</param>
    /// <param name="array">Given array.</param>
    /// <returns>Result of normalized value.</returns>
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

    /// <summary>
    /// Normalized all values in array.
    /// </summary>
    /// <param name="array">Given array.</param>
    /// <returns>Result of normalized array.</returns>
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

    /// <summary>
    /// Search the max value inside float array.
    /// </summary>
    public static float Max(List<float> array)
    {
        float f = -99999.0f;
        foreach (var a in array)
        {
            if (a > f) f = a;
        }
        return f;
    }

    /// <summary>
    /// Search the max value inside float array.
    /// </summary>
    public static float Max(float[] array)
    {
        float f = -99999.0f;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] > f) f = array[i];
        }
        return f;
    }

    /// <summary>
    /// Search the min value inside float array.
    /// </summary>
    public static float Min(List<float> array)
    {
        float f = 99999.0f;
        foreach (var a in array)
        {
            if (a < f) f = a;
        }
        return f;
    }

    /// <summary>
    /// Search the min value inside float array.
    /// </summary>
    public static float Min(float[] array)
    {
        float f = 99999.0f;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] < f) f = array[i];
        }
        return f;
    }

    /// <summary>
    /// Find distance between two vectors with scalar multiplier.
    /// </summary>
    public static float Distance(Vector3 a, Vector3 b, float scalar)
    {
        var distance = Vector3.Distance(a, b);
        return Mathf.Abs(scalar * distance);
    }

    /// <summary>
    /// Add two float arrays which must in the same size.
    /// </summary>
    /// <param name="a">Left operand of float array.</param>
    /// <param name="b">Right operand of float array.</param>
    /// <returns>Float array.</returns>
    public static float[] AddTwoFloatArray(float[] a, float[] b)
    {
        if (a.Length != b.Length)
        {
            Debug.LogError("Both arrays not in the same size!");
            return new float[a.Length];
        }

        float[] result = new float[a.Length];
        for (int i = 0; i < a.Length; i++)
        {
            result[i] = a[i] + b[i];
        }

        return result;
    }

    /// <summary>
    /// Add two float arrays which both list and arrays inside must in the same size.
    /// </summary>
    /// <param name="a">Left operand of list of float array.</param>
    /// <param name="b">Right operand of list of float array.</param>
    /// <returns>List of float array.</returns>
    public static List<float[]> AddTwoListFloatArray(List<float[]> a, List<float[]> b)
    {
        if (a.Count != b.Count)
        {
            Debug.LogError("Both lists not in the same size!");
            return new List<float[]> { };
        }

        List<float[]> result = new();
        for (int i = 0; i < a.Count; i++)
        {
            result.Add(AddTwoFloatArray(a[i], b[i]));
        }

        return result;
    }

    /// <summary>
    /// To find mean or average of list form array.
    /// </summary>
    /// <param name="array">Given array in list form.</param>
    /// <returns>Average of list.</returns>
    public static float Mean(List<float> array)
    {
        if (array.Count <= 0) return 0;

        float sum = 0;
        foreach (var item in array)
        {
            sum += item;
        }

        return sum / array.Count;
    }

    /// <summary>
    /// To find mean or average of array.
    /// </summary>
    /// <param name="array">Given array.</param>
    /// <returns>Average of array.</returns>
    public static float Mean(float[] array)
    {
        if (array.Length <= 0) return 0;

        float sum = 0;
        for (int i = 0; i < array.Length; i++)
        {
            sum += array[i];
        }

        return sum / array.Length;
    }

    /// <summary>
    /// To find median of list form array.
    /// </summary>
    /// <param name="array">Given array in list form.</param>
    /// <returns>Median of list.</returns>
    public static float Median(List<float> array)
    {
        array.Sort();

        var c = array.Count;
        if (c % 2 == 0)
        {
            return (array[(c / 2) - 1] + array[c / 2]) / 2;
        }
        else
        {
            return array[(c + 1) / 2 - 1];
        }
    }

    /// <summary>
    /// To find median of array.
    /// </summary>
    /// <param name="array">Given array.</param>
    /// <returns>Median of list.</returns>
    public static float Median(float[] array)
    {
        List<float> temp_array = new();
        for (int i = 0; i < array.Length; i++)
        {
            temp_array.Add(array[i]);
        }

        var c = temp_array.Count;
        if (c % 2 == 0)
        {
            return (temp_array[c / 2] + temp_array[c / 2 + 1]) / 2;
        }
        else
        {
            return temp_array[(c + 1) / 2];
        }
    }
}
