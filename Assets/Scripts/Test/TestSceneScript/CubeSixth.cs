using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSixth : MonoBehaviour
{
    [SerializeField]
    GameObject m_Six, m_SixB1;

    [SerializeField]
    float m_DiffAngle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var mat_six = m_Six.transform.localToWorldMatrix;
        var mat_sixb1 = m_SixB1.transform.localToWorldMatrix;

        // diff = sixb1 - six --> direction to sixb1
        var diff = mat_six.inverse * mat_sixb1;

        var new_q = diff.rotation;
        var angle = Mathf.Acos(new_q.w) * Mathf.Rad2Deg * 2;
        m_DiffAngle = angle;
    }
}
