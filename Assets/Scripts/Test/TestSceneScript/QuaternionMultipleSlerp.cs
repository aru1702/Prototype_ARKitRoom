using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuaternionMultipleSlerp : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Both are free parameter.")]
    GameObject m_ObjectZero;

    [SerializeField]
    [Range(0, 1)]
    float m_ZeroSlerpRatio;

    [SerializeField]
    [Tooltip("Only ObjectOne is free parameter.")]
    GameObject m_ObjectOne, m_ObjectOneB1, m_ObjectOneB2, m_ObjectOneB3, m_ObjectOneB4;

    [SerializeField]
    [Range(0, 1)]
    float m_OneSlerpRatio;

    [SerializeField]
    [Tooltip("Only ObjectTwo is free parameter.")]
    GameObject m_ObjectTwo, m_ObjectTwoB1, m_ObjectTwoB2, m_ObjectTwoB3;

    [SerializeField]
    [Range(0, 1)]
    float m_TwoSlerpRatio;

    [SerializeField]
    [Tooltip("Only ObjectThree is free parameter.")]
    GameObject m_ObjectThree, m_ObjectThreeB1, m_ObjectThreeB2, m_ObjectThreeB3;


    //[SerializeField]
    Quaternion m_QZero,
               m_QOne, m_QOneB1, m_QOneB2, m_QOneB3,
               m_QTwo, m_QTwoB1, m_QTwoB2, m_QTwoB3,
               m_QThree, m_QThreeB1, m_QThreeB2, m_QThreeB3;

    bool alreadyLog = false;

    // Update is called once per frame
    void Update()
    {
        MultipleSlerp();
        AveragingQ();
        MatrixAverage();
        SimpleTwoQuaternion();

        LogAll();
    }

    void MultipleSlerp()
    {
        m_QZero = m_ObjectZero.transform.rotation;
        m_QOne = m_ObjectOne.transform.rotation;
        m_ObjectOneB1.transform.rotation = Quaternion.Slerp(m_QZero, m_QOne, m_ZeroSlerpRatio);

        m_QOneB1 = m_ObjectOneB1.transform.rotation;
        m_QTwo = m_ObjectTwo.transform.rotation;
        m_ObjectTwoB1.transform.rotation = Quaternion.Slerp(m_QOneB1, m_QTwo, m_OneSlerpRatio);

        m_QTwoB1 = m_ObjectTwoB1.transform.rotation;
        m_QThree = m_ObjectThree.transform.rotation;
        m_ObjectThreeB1.transform.rotation = Quaternion.Slerp(m_QTwoB1, m_QThree, m_TwoSlerpRatio);
    }

    void AveragingQ()
    {
        Vector4 sum = new();
        var snd = MathFunction.QuaternionAverage.AverageQuaternion(ref sum, m_QOne, m_QZero, 1);
        var trd = MathFunction.QuaternionAverage.AverageQuaternion(ref sum, m_QTwo, snd, 2);
        var fth = MathFunction.QuaternionAverage.AverageQuaternion(ref sum, m_QThree, trd, 3);

        m_ObjectOneB2.transform.rotation = snd;
        m_QOneB2 = m_ObjectOneB2.transform.rotation;

        m_ObjectTwoB2.transform.rotation = trd;
        m_QTwoB2 = m_ObjectTwoB2.transform.rotation;

        m_ObjectThreeB2.transform.rotation = fth;
        m_QThreeB2 = m_ObjectThreeB2.transform.rotation;

        if (!alreadyLog)
        {
            string s = "";
            s += "m_QOneB2 = " + GlobalDebugging.LoggingQuat(m_QOneB2) + "\n";
            s += "m_QTwoB2 = " + GlobalDebugging.LoggingQuat(m_QTwoB2) + "\n";
            s += "m_QThreeB2 = " + GlobalDebugging.LoggingQuat(m_QThreeB2) + "\n";
            Debug.Log("AveragingQ result: \n\n" + s);
        }
    }

    void MatrixAverage()
    {
        var tmat_zero = m_ObjectZero.transform.localToWorldMatrix;
        var tmat_one = m_ObjectOne.transform.localToWorldMatrix;
        var tmat_two = m_ObjectTwo.transform.localToWorldMatrix;
        var tmat_three = m_ObjectThree.transform.localToWorldMatrix;

        // 2
        var snd_add = MathFunction.BasicOperation.M44Adder(tmat_zero, tmat_one);
        var snd = MathFunction.BasicOperation.M44DotDivision(snd_add, 2);

        // 3
        var trd_add = MathFunction.BasicOperation.M44Adder(tmat_zero, tmat_one);
        trd_add = MathFunction.BasicOperation.M44Adder(trd_add, tmat_two);
        var trd = MathFunction.BasicOperation.M44DotDivision(trd_add, 3);

        // 4
        var fth_add = MathFunction.BasicOperation.M44Adder(tmat_zero, tmat_one);
        fth_add = MathFunction.BasicOperation.M44Adder(fth_add, tmat_two);
        fth_add = MathFunction.BasicOperation.M44Adder(fth_add, tmat_three);
        var fth = MathFunction.BasicOperation.M44DotDivision(fth_add, 4);

        m_ObjectOneB3.transform.rotation = snd.rotation;
        m_QOneB2 = m_ObjectOneB2.transform.rotation;

        m_ObjectTwoB3.transform.rotation = trd.rotation;
        m_QTwoB2 = m_ObjectTwoB2.transform.rotation;

        m_ObjectThreeB3.transform.rotation = fth.rotation;
        m_QThreeB2 = m_ObjectThreeB2.transform.rotation;

        if (!alreadyLog)
        {
            string s = "";
            s += "m_QOneB3 = " + GlobalDebugging.LoggingQuat(m_QOneB2) + "\n";
            s += "m_QTwoB3 = " + GlobalDebugging.LoggingQuat(m_QTwoB2) + "\n";
            s += "m_QThreeB3 = " + GlobalDebugging.LoggingQuat(m_QThreeB2) + "\n";
            Debug.Log("AveragingTmat result: \n\n" + s);
        }
    }

    void SimpleTwoQuaternion()
    {
        // with quaternion
        //var q_zero = m_ObjectZero.transform.rotation;
        //var q_one = m_ObjectOne.transform.rotation;
        //var sub = q_zero * Quaternion.Inverse(q_one);
        //m_ObjectOneB4.transform.rotation = sub;

        // with matrices
        var m_zero = m_ObjectZero.transform.localToWorldMatrix;
        var m_one = m_ObjectOne.transform.localToWorldMatrix;
        var diff = m_zero.inverse * m_one;
        m_ObjectOneB4.transform.rotation = diff.rotation;
    }

    void LogAll()
    {
        if (!alreadyLog)
        {
            string s = "";
            s += "m_QZero = " + GlobalDebugging.LoggingQuat(m_QZero) + "\n";
            s += "m_QOne = " + GlobalDebugging.LoggingQuat(m_QOne) + "\n";
            s += "m_QOneB1 = " + GlobalDebugging.LoggingQuat(m_QOneB1) + "\n";
            s += "m_QTwo = " + GlobalDebugging.LoggingQuat(m_QTwo) + "\n";
            s += "m_QTwoB1 = " + GlobalDebugging.LoggingQuat(m_QTwoB1) + "\n";
            s += "m_QThree = " + GlobalDebugging.LoggingQuat(m_QThree) + "\n";
            s += "m_QThreeB1 = " + GlobalDebugging.LoggingQuat(m_QThreeB1) + "\n";

            Debug.Log("QuaternionMultipleSlerp result: \n\n" + s);


            // Try debug the tmat
            var tmat_zero = m_ObjectZero.transform.localToWorldMatrix;
            var tmat_one = m_ObjectOne.transform.localToWorldMatrix;
            var tmat_two = m_ObjectTwo.transform.localToWorldMatrix;

            Debug.Log("initialization");
            Debug.Log(tmat_zero);
            Debug.Log(tmat_one);
            Debug.Log(tmat_two);

            Debug.Log("two matrices");

            var add = MathFunction.BasicOperation.M44Adder(tmat_zero, tmat_one);

            Debug.Log(add);

            var div = MathFunction.BasicOperation.M44DotDivision(add, 2);

            Debug.Log(div);

            Debug.Log(div.rotation);

            Debug.Log("three matrices");

            add = MathFunction.BasicOperation.M44Adder(tmat_zero, tmat_one);

            Debug.Log(add);

            add = MathFunction.BasicOperation.M44Adder(add, tmat_two);

            Debug.Log(add);

            div = MathFunction.BasicOperation.M44DotDivision(add, 3);

            Debug.Log(div);

            Debug.Log(div.rotation);

            alreadyLog = true;
        }
    }
}
