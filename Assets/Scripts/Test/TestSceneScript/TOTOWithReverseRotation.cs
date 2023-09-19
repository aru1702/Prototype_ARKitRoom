using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TOTOWithReverseRotation : MonoBehaviour
{
    [SerializeField]
    GameObject m_Source, m_Destination;

    // Start is called before the first frame update
    void Start()
    {
        m_Source.transform.rotation = Quaternion.identity;

        var sTow = m_Source.transform.localToWorldMatrix;
        var dTow = m_Destination.transform.localToWorldMatrix;

        //var sTow_inv = sTow.inverse;

        //Debug.Log(sTow.GetPosition());
        //Debug.Log(sTow_inv.GetPosition());




        var new_T = dTow.inverse;

        Matrix3x3 a = new(dTow.GetColumn(0), dTow.GetColumn(1), dTow.GetColumn(2));
        a.Negative();
        Vector3 a_newPos = a.MultiplyByVector3(new_T.GetColumn(3));
        a_newPos = -a_newPos;
        Matrix4x4 a_new = new(new_T.GetColumn(0), new_T.GetColumn(1), new_T.GetColumn(2), a_newPos);
        a_new.m33 = 1;

        var sTod = a_new * sTow;
        Vector3 init_pos = m_Destination.transform.position;
        Vector3 new_pos = sTod * init_pos;
        m_Source.transform.position = new_pos;
    }
}
