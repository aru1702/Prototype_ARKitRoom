using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJAvgQAndNorm : MonoBehaviour
{
    [SerializeField]
    GameObject m_ObjectOne, m_ObjectTwo, m_ObjectThree, m_ObjectResult;

    [SerializeField]
    bool m_WeightByDistance = false;

    [SerializeField]
    float w1 = 1f, w2 = 1f, w3 = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_WeightByDistance)
        {
            var dist_1 = Vector3.Distance(m_ObjectOne.transform.position, m_ObjectResult.transform.position);
            var dist_2 = Vector3.Distance(m_ObjectTwo.transform.position, m_ObjectResult.transform.position);
            var dist_3 = Vector3.Distance(m_ObjectThree.transform.position, m_ObjectResult.transform.position);

            w1 = MathFunctions.Sigmoid(dist_1, true, 1);
            w2 = MathFunctions.Sigmoid(dist_2, true, 1);
            w3 = MathFunctions.Sigmoid(dist_3, true, 1);

            //Debug.Log(w1);
            //Debug.Log(w2);
            //Debug.Log(w3);
        }

        var one_rot = m_ObjectOne.transform.rotation;
        var two_rot = m_ObjectTwo.transform.rotation;
        var tre_rot = m_ObjectThree.transform.rotation;

        Quaternion[] qs = { one_rot, two_rot, tre_rot };
        float[] ws = { w1, w2, w3 };
        ws = MathFunctions.NormalizedMany(ws);

        m_ObjectResult.transform.rotation = Quaternion.Normalize(AvgQ(qs, ws));
    }

    Quaternion AvgQ(Quaternion[] qs, float[] ws)
    {
        if (qs.Length != ws.Length) throw new System.Exception("Mismatch size!");

        Quaternion new_q = new(0,0,0,0);
        for (int i = 0; i < qs.Length; i++)
        {
            new_q = QAddQ(QDotW(qs[i], ws[i]), new_q);
        }

        return new_q;
    }

    Quaternion QDotW(Quaternion q, float w)
    {
        return new(w * q.x, w * q.y, w * q.z, w * q.w);
    }

    Quaternion QAddQ(Quaternion l, Quaternion r)
    {
        return new(l.x + r.x, l.y + r.y, l.z + r.z, l.w + r.w);
    }
}
