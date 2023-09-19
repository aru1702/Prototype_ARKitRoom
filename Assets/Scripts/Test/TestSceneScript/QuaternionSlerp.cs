using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class QuaternionSlerp : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Base is the left, Two is the right, and One is the middle")]
    GameObject m_ObjectBase, m_ObjectTwo, m_ObjectOne;

    [SerializeField]
    [Range(0,1)]
    float m_SlerpRatio;

    [SerializeField]
    Quaternion m_QBase, m_QTwo, m_QOne;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_QBase = m_ObjectBase.transform.rotation;
        m_QTwo = m_ObjectTwo.transform.rotation;
        m_QOne = m_ObjectOne.transform.rotation;

        m_ObjectOne.transform.rotation =
            Quaternion.Slerp(m_QBase, m_QTwo, m_SlerpRatio);
    }
}
