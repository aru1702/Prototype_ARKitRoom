using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_LoggingObjPerSecond : MonoBehaviour
{
    [SerializeField]
    GameObject m_Target;

    [SerializeField]
    GameObject m_Source;

    [SerializeField]
    float m_PerSec = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
        StartCoroutine(PerSec());
    }

    IEnumerator PerSec()
    {
        while (true)
        {
            if (m_Target != null && m_Source != null)
            {
                var pos = m_Target.transform.localPosition;
                var rot = m_Target.transform.localEulerAngles;

                var m44 = GlobalConfig.GetM44ByGameObjRef(m_Target, m_Source);
                var npos = GlobalConfig.GetPositionFromM44(m44);
                var nrot = GlobalConfig.GetRotationFromM44(m44);
                var nrote = nrot.eulerAngles;

                Debug.Log("local pos: " + pos + "\n" +
                          "local rot: " + rot + "\n" +
                          "frref pos: " + npos + "\n" +
                          "frref rot: " + nrote);
            }

            yield return new WaitForSeconds(m_PerSec);
        }
    }
}
