using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Thanks to SLERP (Quaternions): https://youtu.be/BXajpAy5-UI?t=947

public class MatrixAndSlerp : MonoBehaviour
{
    [SerializeField]
    GameObject m_One, m_Two, m_Three, m_Four, m_Five;

    [SerializeField]
    [Tooltip("Read-only.")]
    GameObject m_Result;

    [SerializeField]
    [Tooltip("Read-only.")]
    float[] weights = { 0, 0, 0, 0, 0 };

    [SerializeField]
    [Tooltip("Read-only.")]
    float[] ts = { 0, 0, 0, 0, 0 };

    Matrix4x4 result_const_transmat44;

    private void Start()
    {
        result_const_transmat44 = m_Result.transform.localToWorldMatrix;
    }

    // Update is called once per frame
    void Update()
    {
        // get each distance to m_Result
        float[] dists =
        {
            Vector3.Distance(m_One.transform.position, m_Result.transform.position),
            Vector3.Distance(m_Two.transform.position, m_Result.transform.position),
            Vector3.Distance(m_Three.transform.position, m_Result.transform.position),
            Vector3.Distance(m_Four.transform.position, m_Result.transform.position),
            Vector3.Distance(m_Five.transform.position, m_Result.transform.position)
        };

        // get each data transformation matrix according to the world origin
        Matrix4x4[] matrices =
        {
            m_One.transform.localToWorldMatrix,
            m_Two.transform.localToWorldMatrix,
            m_Three.transform.localToWorldMatrix,
            m_Four.transform.localToWorldMatrix,
            m_Five.transform.localToWorldMatrix
        };

        // what if we just use Quaternion differences
        Quaternion[] quats =
        {
            m_One.transform.rotation,
            m_Two.transform.rotation,
            m_Three.transform.rotation,
            m_Four.transform.rotation,
            m_Five.transform.rotation
        };

        if (dists.Length != matrices.Length) throw new System.Exception("Mismatch length.");

        Quaternion result = Quaternion.identity;

        for (int i = 0; i < matrices.Length; i++)
        {
            float w = MathFunctions.Sigmoid(dists[i], true, 1);
            weights[i] = w;

            // if we want to normalize the weight
            w = MathFunctions.Normalized(w, weights);

            // if we use Transformation matrices
            Quaternion diff = (matrices[i].inverse * result_const_transmat44).rotation;

            // if we use Quaternions
            //Quaternion diff = result_const_transmat44.rotation * Quaternion.Inverse(quats[i]);

            // if weight doesn't included
            //float t = w / (i + 1);
            //float t = 1f / (i + 1);
            float t = w;

            result = Quaternion.Slerp(result, diff, t);
            ts[i] = t;
        }

        var new_q = result_const_transmat44.rotation * result;

        m_Result.transform.rotation = Quaternion.Inverse(new_q);
    }
}
