using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class QuaternionOperation : MonoBehaviour
{
    [SerializeField]
    GameObject m_Object, m_ObjectAdd, m_ObjectSub, m_ObjectMul;

    [SerializeField]
    Quaternion m_ObjectQuat;

    [SerializeField]
    Quaternion m_Quat1, m_Quat2, m_QuatAdd, m_QuatSub, m_QuatMul;

    // Start is called before the first frame update
    void Start()
    {
        m_Quat1 = new Quaternion();
        m_Quat2 = new Quaternion();
    }

    // Update is called once per frame
    void Update()
    {
        m_ObjectQuat = m_Object.transform.rotation;

        m_QuatAdd = new Quaternion(
                m_Quat1.x + m_Quat2.x,
                m_Quat1.y + m_Quat2.y,
                m_Quat1.z + m_Quat2.z,
                m_Quat1.w + m_Quat2.w
            );

        m_QuatSub = new Quaternion(
                m_Quat1.x - m_Quat2.x,
                m_Quat1.y - m_Quat2.y,
                m_Quat1.z - m_Quat2.z,
                m_Quat1.w - m_Quat2.w
            );

        m_QuatMul = m_Quat1 * m_Quat2;

        m_ObjectAdd.transform.rotation = m_QuatAdd;
        m_ObjectSub.transform.rotation = m_QuatSub;
        m_ObjectMul.transform.rotation = m_QuatMul;
    }
}
