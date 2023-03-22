using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJSLERPOrdering : MonoBehaviour
{
#pragma warning disable 0414
    [SerializeField]
    string text = "=== Base GameObject ===";

    [SerializeField]
    GameObject m_Source, m_One, m_Two, m_Three;

    [SerializeField]
    string text2 = "=== Activation button ===";

    //[SerializeField]
    //bool m1, m2, m3, m4, m5, m6, m7;

    [SerializeField]
    bool start;

    [SerializeField]
    string text3 = "=== Others ===";

    [SerializeField]
    float speed = 1f;

    [SerializeField]
    bool slerp, nlerp;

    [SerializeField]
    bool debug, add;

    [SerializeField]
    float rotY = 30f;
#pragma warning restore 0414

    GameObject o1, o2, o3, o4, o5, o6;
    GameObject s;
    Quaternion r123, r132, r213, r231, r312, r321, reig;
    bool alreadyDebug = false, alreadyAdd = false;

    // Start is called before the first frame update
    void Start()
    {
        // init
        s = Instantiate(m_Source);
        s.name = "_m_result";

        o1 = Instantiate(m_Source, new Vector3(4.3f, 0f, -8.85f), Quaternion.identity);
        o2 = Instantiate(m_Source, new Vector3(5.3f, 0f, -8.85f), Quaternion.identity);
        o3 = Instantiate(m_Source, new Vector3(6.3f, 0f, -8.85f), Quaternion.identity);
        o4 = Instantiate(m_Source, new Vector3(7.3f, 0f, -8.85f), Quaternion.identity);
        o5 = Instantiate(m_Source, new Vector3(8.3f, 0f, -8.85f), Quaternion.identity);
        o6 = Instantiate(m_Source, new Vector3(9.3f, 0f, -8.85f), Quaternion.identity);

        // position
        var p1 = m_One.transform.position;
        var p2 = m_Two.transform.position;
        var p3 = m_Three.transform.position;

        var p123 = (p1 + p2 + p3) / 3.0f;

        //// rotation
        //var r1 = m_One.transform.rotation;
        //var r2 = m_Two.transform.rotation;
        //var r3 = m_Three.transform.rotation;

        //r123 = ThreeSlerp(r1, r2, r3);
        //r132 = ThreeSlerp(r1, r3, r2);
        //r213 = ThreeSlerp(r2, r1, r3);
        //r231 = ThreeSlerp(r2, r3, r1);
        //r312 = ThreeSlerp(r3, r1, r2);
        //r321 = ThreeSlerp(r3, r2, r1);

        reig = new(0f, 0.904961f, 0f, -0.425494f);

        // apply to unity
        s.transform.position = p123;

        // log
        Debug.Log("\n" +
                  "=== Result of rotation with the matter the order ===" + "\n" +
                  "Avg(1,2,3): " + "\trot1: " + r123.eulerAngles.ToString() + "\trot2: " + r123.ToString() + "\n" +
                  "Avg(1,3,2): " + "\trot1: " + r132.eulerAngles.ToString() + "\trot2: " + r132.ToString() + "\n" +
                  "Avg(2,1,3): " + "\trot1: " + r213.eulerAngles.ToString() + "\trot2: " + r213.ToString() + "\n" +
                  "Avg(2,3,1): " + "\trot1: " + r231.eulerAngles.ToString() + "\trot2: " + r231.ToString() + "\n" +
                  "Avg(3,1,2): " + "\trot1: " + r312.eulerAngles.ToString() + "\trot2: " + r312.ToString() + "\n" +
                  "Avg(3,2,3): " + "\trot1: " + r321.eulerAngles.ToString() + "\trot2: " + r321.ToString() + "\n" +
                  "Avg(reig) : " + "\trot1: " + reig.eulerAngles.ToString() + "\trot2: " + reig.ToString() + "\n" +
                  "=== ========================================================= ===" + "\n");
    }

    Quaternion ThreeSlerp(Quaternion a, Quaternion b, Quaternion c)
    {
        float f = 1.0f / 3.0f;
        var q1 = Quaternion.Slerp(a, b, 0.5f);
        var q2 = Quaternion.Slerp(q1, c, f);
        return q2;
        //return Quaternion.Slerp(Quaternion.Slerp(a, b, 0.5f), c, f);
    }

    Quaternion ThreeNlerp(Quaternion a, Quaternion b, Quaternion c)
    {
        float f = 1.0f / 3.0f;
        var q1 = Quaternion.Lerp(a, b, 0.5f);
        q1.Normalize();
        var q2 = Quaternion.Lerp(q1, c, f);
        q2.Normalize();
        return q2;
    }

    // Update is called once per frame
    void Update()
    {
        // rotation
        var r1 = m_One.transform.rotation;
        var r2 = m_Two.transform.rotation;
        var r3 = m_Three.transform.rotation;

        if (slerp)
        {
            r123 = ThreeSlerp(r1, r2, r3);
            r132 = ThreeSlerp(r1, r3, r2);
            r213 = ThreeSlerp(r2, r1, r3);
            r231 = ThreeSlerp(r2, r3, r1);
            r312 = ThreeSlerp(r3, r1, r2);
            r321 = ThreeSlerp(r3, r2, r1);
        }

        if (nlerp)
        {
            r123 = ThreeNlerp(r1, r2, r3);
            r132 = ThreeNlerp(r1, r3, r2);
            r213 = ThreeNlerp(r2, r1, r3);
            r231 = ThreeNlerp(r2, r3, r1);
            r312 = ThreeNlerp(r3, r1, r2);
            r321 = ThreeNlerp(r3, r2, r1);
        }

        o1.transform.rotation = r123;
        o2.transform.rotation = r132;
        o3.transform.rotation = r213;
        o4.transform.rotation = r231;
        o5.transform.rotation = r312;
        o6.transform.rotation = r321;

        //if (m1) s.transform.rotation = r123;
        //else if (m2) s.transform.rotation = r132;
        //else if (m3) s.transform.rotation = r213;
        //else if (m4) s.transform.rotation = r231;
        //else if (m5) s.transform.rotation = r312;
        //else if (m6) s.transform.rotation = r321;
        //else if (m7) s.transform.rotation = reig;

        if (start)
        {
            var y = m_Two.transform.rotation.eulerAngles.y;
            y += speed;
            Quaternion q = Quaternion.identity;
            q.eulerAngles = new Vector3(0, y, 0);
            m_Two.transform.rotation = q;
        }

        if (!alreadyDebug && debug)
        {
            alreadyDebug = true;
            Debug.Log("\n" +
                  "=== Result of rotation with the matter the order ===" + "\n" +
                  "Avg(1,2,3): " + "\trot1: " + r123.eulerAngles.ToString() + "\trot2: " + r123.ToString() + "\n" +
                  "Avg(1,3,2): " + "\trot1: " + r132.eulerAngles.ToString() + "\trot2: " + r132.ToString() + "\n" +
                  "Avg(2,1,3): " + "\trot1: " + r213.eulerAngles.ToString() + "\trot2: " + r213.ToString() + "\n" +
                  "Avg(2,3,1): " + "\trot1: " + r231.eulerAngles.ToString() + "\trot2: " + r231.ToString() + "\n" +
                  "Avg(3,1,2): " + "\trot1: " + r312.eulerAngles.ToString() + "\trot2: " + r312.ToString() + "\n" +
                  "Avg(3,2,3): " + "\trot1: " + r321.eulerAngles.ToString() + "\trot2: " + r321.ToString() + "\n" +
                  "=== ========================================================= ===" + "\n");
        }
        else if (alreadyDebug && !debug)
        {
            alreadyDebug = false;
        }

        if (!alreadyAdd && add)
        {
            alreadyAdd = true;

            var y = m_Two.transform.rotation.eulerAngles.y;
            y += rotY;
            Quaternion q = Quaternion.identity;
            q.eulerAngles = new Vector3(0, y, 0);
            m_Two.transform.rotation = q;
        }
        else if (alreadyAdd && !add)
        {
            alreadyAdd = false;
        }
    }
}
