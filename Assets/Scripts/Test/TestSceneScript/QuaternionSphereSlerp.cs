using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To see math reference https://www.euclideanspace.com/maths/algebra/realNormedAlgebra/quaternions/slerp/index.htm
// We use optimized of how many Slerp we do
// The more we have, the more 1/n of interpolation being calculated

public class QuaternionSphereSlerp : MonoBehaviour
{
    [SerializeField]
    GameObject m_SphereZero, m_SphereOne, m_SphereTwo, m_SphereThree, m_SphereResult;

    [SerializeField]
    [Range(0, 1)]
    float m_IntPolOne = (float) 1f/2f;

    [SerializeField]
    [Range(0, 1)]
    float m_IntPolTwo = (float) 1f/3f;

    [SerializeField]
    [Range(0, 1)]
    float m_IntPolThree = (float) 1f/4f;


    // Update is called once per frame
    void Update()
    {
        var q0 = m_SphereZero.transform.rotation;
        var q1 = m_SphereOne.transform.rotation;
        var q2 = m_SphereTwo.transform.rotation;
        var q3 = m_SphereThree.transform.rotation;

        var rot1 = Quaternion.Slerp(q0, q1, m_IntPolOne);
        var rot2 = Quaternion.Slerp(rot1, q2, m_IntPolTwo);
        var rot3 = Quaternion.Slerp(rot2, q3, m_IntPolThree);

        m_SphereResult.transform.rotation = rot3;
    }
}
