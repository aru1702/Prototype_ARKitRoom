using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightsAndScalar
{
    public List<float[]> Weights { get; set; }
    public float Scalars { get; set; }

    public WeightsAndScalar(List<float[]> weights, float scalars)
    {
        Weights = weights;
        Scalars = scalars;
    }

    public WeightsAndScalar() { }

    /// <summary>
    /// Apply scalar to weight class, which define as weight priority.
    /// </summary>
    public void ApplyScalarToWeights()
    {
        foreach (var w in Weights)
        {
            for (int i = 0; i < w.Length; i++)
            {
                w[i] *= Scalars;
            }
        }
    }

    /// <summary>
    /// Add two weight with scalar applied into each.
    /// </summary>
    /// <param name="a">Left operand of WeightsAndScalar class.</param>
    /// <param name="b">Right operand of WeightsAndScalar class.</param>
    /// <param name="apply_scalar">"true" or "false" to apply scalar</param>
    /// <returns>List of float for proper weight form.</returns>
    public static List<float[]> AddTwoWeightList(WeightsAndScalar a, WeightsAndScalar b, bool apply_scalar = false)
    {
        List<float[]> weights = new();

        if (a.Weights.Count != b.Weights.Count)
        {
            Debug.LogError("Cannot add because both Weights arrays " +
                           "are not in the same size.");
            return weights;
        }

        if (apply_scalar)
        {
            a.ApplyScalarToWeights();
            // GlobalDebugging.DebugLogListFloatArray(Test_OnlyAxisObjectGet.AxisForDec192022(a.Weights), "a");

            b.ApplyScalarToWeights();
            // GlobalDebugging.DebugLogListFloatArray(Test_OnlyAxisObjectGet.AxisForDec192022(b.Weights), "b");
        }

        for (int i = 0; i < a.Weights.Count; i++)
        {
            float[] floats = new float[a.Weights[i].Length];

            for (int j = 0; j < a.Weights[i].Length; j++)
            {
                floats[j] = a.Weights[i][j] + b.Weights[i][j];
            }

            weights.Add(floats);
        }

        return weights;
    }

    /// <summary>
    /// Add two weight with scalar applied into each.
    /// </summary>
    /// <param name="apply_scalar">"true" or "false" to apply scalar</param>
    /// <param name="args">Operands for each WeightsAndScalar class.</param>
    /// <returns>List of float for proper weight form.</returns>
    public static List<float[]> AddMultipleWeightList(params WeightsAndScalar[] args)
    {
        // NEW TRY
        if (args.Length < 2)
        {
            Debug.LogError("Arguments minimum size is two.");
            return args[0].Weights;
        }

        var left_operand = args[0];
        //GlobalDebugging.DebugLogListFloatArray(Test_OnlyAxisObjectGet.AxisForDec192022(left_operand.Weights),
        //        "AddMultipleWeightList: First operand before apply scalar on Version 2a");
        left_operand.ApplyScalarToWeights();
        //GlobalDebugging.DebugLogListFloatArray(Test_OnlyAxisObjectGet.AxisForDec192022(left_operand.Weights),
        //        "AddMultipleWeightList: First operand after apply scalar on Version 2a");

        for (int i = 1; i < args.Length; i++)
        {
            //GlobalDebugging.DebugLogListFloatArray(Test_OnlyAxisObjectGet.AxisForDec192022(args[i].Weights),
            //    "AddMultipleWeightList: "+i+"-th operand before apply scalar on Version 2a");
            args[i].ApplyScalarToWeights();
            //GlobalDebugging.DebugLogListFloatArray(Test_OnlyAxisObjectGet.AxisForDec192022(args[i].Weights),
            //    "AddMultipleWeightList: " + i + "-th operand after apply scalar on Version 2a");
            var result_weight = AddTwoWeightList(left_operand, args[i]);
            var result_scalar = left_operand.Scalars + args[i].Scalars;
            left_operand = new(result_weight, result_scalar);
            //GlobalDebugging.DebugLogListFloatArray(Test_OnlyAxisObjectGet.AxisForDec192022(left_operand.Weights),
            //    "AddMultipleWeightList: Final add with " + i + "-th operand on Version 2a");
        }

        for (int i = 0; i < left_operand.Weights.Count; i++)
        {
            left_operand.Weights[i] = MathFunctions.NormalizedMany(left_operand.Weights[i]);
        }

        //GlobalDebugging.DebugLogListFloatArray(Test_OnlyAxisObjectGet.AxisForDec192022(left_operand.Weights),
        //        "AddMultipleWeightList: Final result after normalized on Version 2a");

        return left_operand.Weights;


        // FIRST TRY
        //List<float[]> temp_ws = AddZeroToSameListArraySize(args[0].Weights.Count, args[0].Weights[0].Length);
        //WeightsAndScalar base_ws = new(temp_ws, 0);

        //if (args.Length <= 1)
        //{
        //    Debug.LogError("Arguments minimum size is two.");
        //    return temp_ws;
        //}

        //for (int i = 0; i < args.Length; i++)
        //{
        //    //GlobalDebugging.DebugLogListFloatArray(Test_OnlyAxisObjectGet.AxisForDec192022(temp_ws),
        //    //    "Add two weights with AddMultipleWeightList on WeightsAndScalar class");

        //    args[i].ApplyScalarToWeights();
        //    temp_ws = AddTwoWeightList(base_ws, args[i], !apply_scalar);

        //    var new_scalar = base_ws.Scalars + args[i].Scalars;
        //    //Debug.Log(new_scalar);
        //    base_ws = new(temp_ws, new_scalar);
        //}

        ////GlobalDebugging.DebugLogListFloatArray(Test_OnlyAxisObjectGet.AxisForDec192022(temp_ws),
        ////    "Final weight with AddMultipleWeightList on WeightsAndScalar class");

        //for (int i = 0; i < temp_ws.Count; i++)
        //{
        //    temp_ws[i] = MathFunctions.NormalizedMany(temp_ws[i]);
        //}

        ////GlobalDebugging.DebugLogListFloatArray(Test_OnlyAxisObjectGet.AxisForDec192022(temp_ws),
        ////    "Normalized weight finalization on WeightsAndScalar class");

        //return temp_ws;
    }

    /// <summary>
    /// Add all zero value to List<float[]> with given list size and array size.
    /// </summary>
    /// <param name="list_size">Integer for List<> size.</param>
    /// <param name="array_size">Integer for float[] array size.</param>
    /// <returns>List of float for with all zero values.</returns>
    static List<float[]> AddZeroToSameListArraySize(int list_size, int array_size)
    {
        List<float[]> lists = new();

        for (int i = 0; i < list_size; i++)
        {
            float[] arrays = new float[array_size];
            for (int j = 0; j < array_size; j++)
            {
                arrays[j] = 0;
            }
            lists.Add(arrays);
        }

        return lists;
    }
}
