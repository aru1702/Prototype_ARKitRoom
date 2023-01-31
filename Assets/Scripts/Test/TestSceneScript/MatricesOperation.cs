using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Thanks to https://stackoverflow.com/questions/10176456/subtracting-two-4x4-matrices-is-this-possible

public class MatricesOperation : MonoBehaviour
{
    [SerializeField]
    GameObject m_Zero;

    [SerializeField]
    GameObject m_One, m_OneB1, m_OneB2, m_OneB3, m_OneB4;

    [SerializeField]
    GameObject m_Four, m_FourB1;

    [SerializeField]
    GameObject m_Five, m_FiveB1, m_Five_clone;

    [SerializeField]
    [Range(0, 1)]
    float m_CubeFiveScale = 1f;


    bool alreadyLog = false;
    Matrix4x4 const_mat_five_b1;
    Quaternion const_q_five_b1;

    // Update is called once per frame
    void Update()
    {
        var mat_zero = m_Zero.transform.localToWorldMatrix;
        var mat_one = m_One.transform.localToWorldMatrix;

        // adder
        //var add_one = mat_one * mat_zero;
        //var add_inv_one = mat_zero * mat_one;

        //m_OneB1.transform.rotation = add_one.rotation;
        //m_OneB2.transform.rotation = add_inv_one.rotation;

        // substract
        var sub_one = mat_one * mat_zero.inverse;
        var sub_inv_one = mat_zero * mat_one.inverse;

        m_OneB1.transform.rotation = sub_one.rotation;
        m_OneB2.transform.rotation = sub_inv_one.rotation;

        var sub_one_c = mat_one.inverse * mat_zero;
        var sub_inv_one_c = mat_zero.inverse * mat_one;

        m_OneB3.transform.rotation = sub_one_c.rotation;
        m_OneB4.transform.rotation = sub_inv_one_c.rotation;






        // inverse matrix
        var mat_four = m_Four.transform.localToWorldMatrix;

        m_FourB1.transform.rotation = mat_four.inverse.rotation;






        // fifth cube
        var mat_five = m_Five.transform.localToWorldMatrix;
        var mat_five_b1 = m_FiveB1.transform.localToWorldMatrix;

        // only done once
        if (!alreadyLog)
        {
            const_mat_five_b1 = new Matrix4x4(
                mat_five_b1.GetColumn(0),
                mat_five_b1.GetColumn(1),
                mat_five_b1.GetColumn(2),
                mat_five_b1.GetColumn(3)
            );
            const_q_five_b1 = new Quaternion(
                m_FiveB1.transform.rotation.x,
                m_FiveB1.transform.rotation.y,
                m_FiveB1.transform.rotation.z,
                m_FiveB1.transform.rotation.w
            );
        }

        // emat = ori - b1
        var emat_five = mat_five_b1.inverse * mat_five;     // this is true for rotation

        var emat_five_2 = mat_five.inverse * mat_five_b1;
        var pos = -emat_five_2.GetPosition();
        var rot = emat_five.rotation;

        if (!alreadyLog)
        {
            Debug.Log("Pos: " + GlobalDebugging.LoggingVec3(pos) + "\n" +
                  "Rot: " + GlobalDebugging.LoggingQuat(rot));

            Debug.Log(emat_five);

            //var mat_five_clone = m_Five_clone.transform.localToWorldMatrix;
            ////var new_mat_five_clone = emat_five * mat_five_clone;
            //var new_mat_five_clone = mat_five_clone * emat_five;        // this is true for translation
            //m_Five_clone.transform.position = new_mat_five_clone.GetPosition();
            //m_Five_clone.transform.rotation = new_mat_five_clone.rotation;

            var em_pos = m_Five.transform.position - m_FiveB1.transform.position;
            Debug.Log("Pos sub: " + GlobalDebugging.LoggingVec3(em_pos));
            //m_Five_clone.transform.position += em_pos; 

            alreadyLog = true;
        }

        var mat_five_clone = m_Five_clone.transform.localToWorldMatrix;
        //var new_mat_five_clone = emat_five * mat_five_clone;

        // changed from:
        // - mat_five_clone which is changed
        // - const_mat_five_b1 is not changed
        var new_mat_five_clone = const_mat_five_b1 * emat_five;        // this is true for translation

        // this is so true
        //m_Five_clone.transform.position = new_mat_five_clone.GetPosition();
        //m_Five_clone.transform.rotation = new_mat_five_clone.rotation;

        var eq_five = Quaternion.Inverse(m_FiveB1.transform.rotation) * m_Five.transform.rotation;
        var new_q = const_q_five_b1 * eq_five;
        m_Five_clone.transform.rotation = new_q;


        // what if we make them like slerp mode
        // for translation it's easy since linear
        Vector3 pos_diff = (mat_five.GetPosition() - mat_five_b1.GetPosition()) * m_CubeFiveScale;
        m_Five_clone.transform.position = pos_diff + const_mat_five_b1.GetPosition();
    }
}
