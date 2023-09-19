using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this run on the MainMenu, to test if in the iOS can be run successfully
public class TestEigenMacHelper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Quaternion q1 = new Quaternion(0, 0.258819f, 0, 0.9659258f); // Y = 30
        Quaternion q2 = new Quaternion(0, 0.8660254f, 0, 0.5f);      // Y = 120
        Quaternion q3 = new Quaternion(0, 0, 0, 1);                  // Y = 0

        Quaternion diff_1 = Quaternion.Inverse(q1) * q2;    // from q1 to q2
        Quaternion diff_2 = Quaternion.Inverse(q1) * q3;    // from q1 to q3

        EigenMacHelper.QuaternionWeighted qw1 = new EigenMacHelper.QuaternionWeighted(diff_1, 1.0f);
        EigenMacHelper.QuaternionWeighted qw2 = new EigenMacHelper.QuaternionWeighted(diff_2, 1.0f);

        // if we put both of them with same weight, it will perfectly average at half
        Quaternion q_avg = EigenMacHelper.EigenWeightedAvgMultiRotations(qw1, qw2);

        Quaternion q_r = new Quaternion(0, 0, 0, 1);
        q_r *= q_avg;


        // Now is debugging process
        string data_0 = "";
        data_0 += "q1: " + q1.eulerAngles.ToString() + "\n";
        data_0 += "q2: " + q2.eulerAngles.ToString() + "\n";
        data_0 += "q3: " + q3.eulerAngles.ToString() + "\n";
        Debugging("base rotation:\n", data_0);

        string data_1 = "";
        data_1 += "diff_1: " + diff_1.eulerAngles.ToString() + "\n";
        data_1 += "diff_2: " + diff_2.eulerAngles.ToString() + "\n";
        Debugging("rotation differences:\n", data_1);

        string data_2 = "";
        data_2 += "q_avg: " + q_avg.eulerAngles.ToString() + "\n";
        data_2 += "q_r_be: " + Quaternion.identity.eulerAngles.ToString() + "\n";
        data_2 += "q_r_af: " + q_r.eulerAngles.ToString() + "\n";
        Debugging("result:\n", data_2);
    }

    void Debugging(string context, string data)
    {
        Debug.Log(context + ": " + data);
    }
}
