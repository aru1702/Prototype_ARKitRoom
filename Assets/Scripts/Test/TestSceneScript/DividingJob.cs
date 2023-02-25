using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DividingJob : MonoBehaviour
{
    [SerializeField]
    GameObject m_ObjectOne, m_ObjectTwo, m_ObjectThree, m_ObjectResult;

    [SerializeField]
    [Tooltip("Free mode will unlock the Result Object translation, user able to freely move it.")]
    bool m_FreeMode = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // translation average
        if (!m_FreeMode)
        {
            var one_pos = m_ObjectOne.transform.position;
            var two_pos = m_ObjectTwo.transform.position;
            var tre_pos = m_ObjectThree.transform.position;
            m_ObjectResult.transform.position = (one_pos + two_pos + tre_pos) / 3;
        }

        var one_rot = m_ObjectOne.transform.rotation;
        var two_rot = m_ObjectTwo.transform.rotation;
        var tre_rot = m_ObjectThree.transform.rotation;

        Quaternion fst = Quaternion.Slerp(one_rot, two_rot, 0.5f);
        Quaternion scd = Quaternion.Slerp(fst, tre_rot, 0.33f);
        m_ObjectResult.transform.rotation = scd;
    }
}
