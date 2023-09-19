using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_ChangeBehaviorFromScript : MonoBehaviour
{
    [SerializeField]
    GameObject m_Object;

    [SerializeField]
    Vector3 m_Position, m_EulerAngle;

    // Update is called once per frame
    void Update()
    {
        if (m_Object != null)
        {
            m_Object.transform.position = m_Position;
            m_Object.transform.eulerAngles = m_EulerAngle;
        }
    }
}
