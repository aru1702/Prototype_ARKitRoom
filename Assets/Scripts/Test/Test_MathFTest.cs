using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_MathFTest : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Bottom range of array, must be lower than Top Range.")]
    float m_BottomRange = -100.0f;

    [SerializeField]
    [Tooltip("Top range of array, must be higher than Bottom Range.")]
    float m_TopRange = 100.0f;

    [SerializeField]
    [Tooltip("Step for each value in range, must real non-negative non-zero.")]
    float m_Step = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        //float[] values = new float[]
        //    { -1.0f, -2.0f, -10.0f, -20.0f, -100.0f, 0, 1.0f, 2.0f, 10.0f, 20.0f, 100.0f};

        if (m_BottomRange > m_TopRange)
        {
            Debug.LogError("Bottom range is higher than Top range, or vice versa.");
            return;
        }
        if (m_Step <= 0)
        {
            Debug.LogError("Step is cannot be zero or negative value.");
            return;
        }

        //int b = Mathf.RoundToInt(m_BottomRange);
        //int t = Mathf.RoundToInt(m_TopRange);
        int range = Mathf.RoundToInt((m_TopRange - m_BottomRange) / m_Step) + 1;

        float[] values = new float[range];

        for (int i = 0; i < values.Length; i++)
        {
            values[i] = i * m_Step + m_BottomRange;
        }

        var s = "";

        s = "Sigmoid, true \n";
        for (int i = 0; i < values.Length; i++)
        {
            var result = MathFunctions.Sigmoid(values[i], true, 0.1f);
            s += "value: " + values[i] + ", result: " + result + "\n";
        }
        Debug.Log(s);

        s = "Sigmoid, false \n";
        for (int i = 0; i < values.Length; i++)
        {
            var result = MathFunctions.Sigmoid(values[i], false, 0.1f);
            s += "value: " + values[i] + ", result: " + result + "\n";
        }
        //Debug.Log(s);

        s = "Tanh, true \n";
        for (int i = 0; i < values.Length; i++)
        {
            var result = MathFunctions.Tanh(values[i], true, 0.1f);
            s += "value: " + values[i] + ", result: " + result + "\n";
        }
        //Debug.Log(s);

        s = "Tanh, false \n";
        for (int i = 0; i < values.Length; i++)
        {
            var result = MathFunctions.Tanh(values[i], false, 0.1f);
            s += "value: " + values[i] + ", result: " + result + "\n";
        }
        //Debug.Log(s);


        List<float> ws = new();

        s = "Sigmoid, true, normalized, without scale \n";
        for (int i = 0; i < values.Length; i++)
        {
            var result = MathFunctions.Sigmoid(values[i], true);
            ws.Add(result);
        }
        var ws_n = MathFunctions.NormalizedMany(ws);
        for (int i = 0; i < values.Length; i++)
        {
            s += "v: " + values[i] + "\t\tr: " + ws[i] + "\t\tn: " + ws_n[i] + "\n";
        }
        //Debug.Log(s);

        ws.Clear();
        ws_n.Clear();

        s = "Sigmoid, value scale 0.1, true, normalized \n";
        var vs = 0.1f;
        for (int i = 0; i < values.Length; i++)
        {
            var result = MathFunctions.Sigmoid(values[i] * vs, true);
            ws.Add(result);
        }
        ws_n = MathFunctions.NormalizedMany(ws);
        for (int i = 0; i < values.Length; i++)
        {
            s += "v: " + values[i] * vs + ", r: " + ws[i] + ", n: " + ws_n[i] + "\n";
        }
        //Debug.Log(s);
    }

    double Sigmoid(float value, bool inverted = false)
    {
        var result = 1 / (1 + Mathf.Exp(-value));

        return inverted ? (1 - result) : result;
    }

    double Tanh(float value, bool inverted = false)
    {
        var p_d = Mathf.Exp(value);
        var n_d = Mathf.Exp(-value);
        var result = (p_d - n_d) / (p_d + n_d);

        return inverted ? (1 - result) : result;
    }
}
