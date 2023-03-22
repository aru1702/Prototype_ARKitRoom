using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TOTOFiveObject : MonoBehaviour
{

#pragma warning disable 0414
    [SerializeField]
    string tooltip = "=== GameObject assignment ===";

    [SerializeField]
    [Tooltip("The source of transformation.")]
    GameObject m_Source;

    [SerializeField]
    [Tooltip("The targets of transformation.")]
    GameObject m_One, m_Two, m_Three, m_Four, m_Five;

    [SerializeField]
    string tooltip_2 = "=== Multiple data switch assignment ===";

    [SerializeField]
    bool m_ThreeData, m_FourData, m_FiveData;

    [SerializeField]
    string tooltip_3 = "=== Method switch assignment ===";

    [SerializeField]
    bool m_SLERP = false, m_Eigenvector = false,
         m_ResetAll = false;

    [SerializeField]
    string tooltip_4 = "=== Others ===";

    [SerializeField]
    [Tooltip("quaternion/euler")]
    string m_RotationDebug = "quaternion";

    // field
    bool debug = false;

#pragma warning restore 0414

    // Initial field initialization
    Matrix4x4 mat_src, mat_one, mat_two, mat_three, mat_four, mat_five;

    // Result field initialization
    GameObject Result123, Result124, Result125, Result134, Result135, Result145;
    GameObject Result234, Result235, Result245, Result345;

    GameObject Result1234, Result1235, Result1245, Result1345, Result2345;

    GameObject Result12345;

    void SLERP()
    {
        var r1 = m_One.transform.rotation;
        var r2 = m_Two.transform.rotation;
        var r3 = m_Three.transform.rotation;
        var r4 = m_Four.transform.rotation;
        var r5 = m_Five.transform.rotation;


        // first two rotation
        Quaternion r12 = Quaternion.Slerp(r1, r2, 0.5f);
        Quaternion r13 = Quaternion.Slerp(r1, r3, 0.5f);
        Quaternion r14 = Quaternion.Slerp(r1, r4, 0.5f);
        //Quaternion r15 = Quaternion.Slerp(r1, r5, 0.5f);
        Quaternion r23 = Quaternion.Slerp(r2, r3, 0.5f);
        Quaternion r24 = Quaternion.Slerp(r2, r4, 0.5f);
        //Quaternion r25 = Quaternion.Slerp(r2, r5, 0.5f);
        Quaternion r34 = Quaternion.Slerp(r3, r4, 0.5f);
        //Quaternion r35 = Quaternion.Slerp(r3, r5, 0.5f);
        //Quaternion r45 = Quaternion.Slerp(r4, r5, 0.5f);

        // second two rotation
        Quaternion r123 = Quaternion.Slerp(r12, r3, 0.333f);
        Quaternion r124 = Quaternion.Slerp(r12, r4, 0.333f);
        Quaternion r125 = Quaternion.Slerp(r12, r5, 0.333f);
        Quaternion r134 = Quaternion.Slerp(r13, r4, 0.333f);
        Quaternion r135 = Quaternion.Slerp(r13, r5, 0.333f);
        Quaternion r145 = Quaternion.Slerp(r14, r5, 0.333f);
        Quaternion r234 = Quaternion.Slerp(r23, r4, 0.333f);
        Quaternion r235 = Quaternion.Slerp(r23, r5, 0.333f);
        Quaternion r245 = Quaternion.Slerp(r24, r5, 0.333f);
        Quaternion r345 = Quaternion.Slerp(r34, r5, 0.333f);

        // third rotation
        Quaternion r1234 = Quaternion.Slerp(r123, r4, 0.25f);
        Quaternion r1235 = Quaternion.Slerp(r123, r5, 0.25f);
        Quaternion r1245 = Quaternion.Slerp(r124, r5, 0.25f);
        Quaternion r1345 = Quaternion.Slerp(r134, r5, 0.25f);
        Quaternion r2345 = Quaternion.Slerp(r234, r5, 0.25f);

        // fourth rotation
        Quaternion r12345 = Quaternion.Slerp(r1234, r5, 0.2f);


        // apply
        Result123.transform.rotation = r123;
        Result124.transform.rotation = r124;
        Result125.transform.rotation = r125;
        Result134.transform.rotation = r134;
        Result135.transform.rotation = r135;
        Result145.transform.rotation = r145;
        Result234.transform.rotation = r234;
        Result235.transform.rotation = r235;
        Result245.transform.rotation = r245;
        Result345.transform.rotation = r345;

        Result1234.transform.rotation = r1234;
        Result1235.transform.rotation = r1235;
        Result1245.transform.rotation = r1245;
        Result1345.transform.rotation = r1345;
        Result2345.transform.rotation = r2345;

        Result12345.transform.rotation = r12345;

        // debug
        if (!debug) { DebugResultObject("SLERP", m_RotationDebug); debug = true; }
    }

    void Eigenvector()
    {
        // apply
        Result123.transform.rotation = new(-0.187176f, -0.133119f, -0.231287f, -0.945384f);
        Result124.transform.rotation = new(-0.260434f, -0.322762f, 0.0241565f, -0.909624f);
        Result125.transform.rotation = new(0.211292f, 0.228864f, 0.121988f, 0.942388f);
        Result134.transform.rotation = new(-0.330453f, -0.21284f, -0.0433981f, -0.918486f);
        Result135.transform.rotation = new(0.272099f, 0.10667f, 0.202853f, 0.934577f);
        Result145.transform.rotation = new(0.337885f, 0.293749f, -0.0473827f, 0.892917f);
        Result234.transform.rotation = new(0.255248f, 0.199467f, 0.188436f, 0.927121f);
        Result235.transform.rotation = new(0.20485f, 0.104072f, 0.310248f, 0.922471f);
        Result245.transform.rotation = new(-0.27919f, -0.296741f, -0.0678702f, -0.91071f);
        Result345.transform.rotation = new(-0.346455f, -0.171933f, -0.153073f, -0.909383f);

        Result1234.transform.rotation = new(-0.258076f, -0.220953f, -0.108831f, -0.934201f);
        Result1235.transform.rotation = new(-0.219587f, -0.145621f, -0.216101f, -0.940147f);
        Result1245.transform.rotation = new(-0.272573f, -0.285333f, -0.0309586f, -0.91833f);
        Result1345.transform.rotation = new(0.323265f, 0.199702f, 0.0859524f, 0.920995f);
        Result2345.transform.rotation = new(-0.270349f, -0.193138f, -0.184776f, -0.924915f);

        Result12345.transform.rotation = new(0.268841f, 0.210099f, 0.125704f, 0.931548f);

        // debug
        if (!debug) { DebugResultObject("Eigenvector", m_RotationDebug); debug = true; }
    }

    // Start is called before the first frame update
    void Start()
    {
        mat_src = m_Source.transform.localToWorldMatrix;
        mat_one = m_One.transform.localToWorldMatrix;
        mat_two = m_Two.transform.localToWorldMatrix;
        mat_three = m_Three.transform.localToWorldMatrix;
        mat_four = m_Four.transform.localToWorldMatrix;
        mat_five = m_Five.transform.localToWorldMatrix;

        CombinationObjectCreation();
        CombinationObjectRenaming();
        CombinationPositionAveraging();

        CombinationObjectSetActive(false, 0);

        DebugQuaternionforEigen();   // disable this if not need
    }

    // Update is called once per frame
    void Update()
    {
        if (m_ThreeData) { CombinationObjectSetActive(true, 3); } else { CombinationObjectSetActive(false, 3); }
        if (m_FourData) { CombinationObjectSetActive(true, 4); } else { CombinationObjectSetActive(false, 4); }
        if (m_FiveData) { CombinationObjectSetActive(true, 5); } else { CombinationObjectSetActive(false, 5); }

        if (m_SLERP) { SLERP(); }
        if (m_Eigenvector) { Eigenvector(); }
        if (m_ResetAll) { ResetAll(); }
    }

    void SetActive(GameObject go, bool active)
    {
        go.SetActive(active);
    }

    void ResetAll()
    {
        CombinationObjectSetActive(false, 0);
        CombinationRotationReset();

        m_SLERP = false;
        m_Eigenvector = false;
        debug = false;

        m_ResetAll = false;
    }

    void CombinationObjectCreation()
    {
        Result123 = Instantiate(m_Source);
        Result124 = Instantiate(m_Source);
        Result125 = Instantiate(m_Source);
        Result134 = Instantiate(m_Source);
        Result135 = Instantiate(m_Source);
        Result145 = Instantiate(m_Source);
        Result234 = Instantiate(m_Source);
        Result235 = Instantiate(m_Source);
        Result245 = Instantiate(m_Source);
        Result345 = Instantiate(m_Source);

        Result1234 = Instantiate(m_Source);
        Result1235 = Instantiate(m_Source);
        Result1245 = Instantiate(m_Source);
        Result1345 = Instantiate(m_Source);
        Result2345 = Instantiate(m_Source);

        Result12345 = Instantiate(m_Source);
    }

    void CombinationObjectRenaming()
    {
        Result123.name = "_m_result123";
        Result124.name = "_m_result124";
        Result125.name = "_m_result125";
        Result134.name = "_m_result134";
        Result135.name = "_m_result135";
        Result145.name = "_m_result145";
        Result234.name = "_m_result234";
        Result235.name = "_m_result235";
        Result245.name = "_m_result245";
        Result345.name = "_m_result345";

        Result1234.name = "_m_result1234";
        Result1235.name = "_m_result1235";
        Result1245.name = "_m_result1245";
        Result1345.name = "_m_result1345";
        Result2345.name = "_m_result2345";

        Result12345.name = "_m_result12345";
    }

    void CombinationPositionAveraging()
    {
        var p1 = mat_one.GetPosition(); var p2 = mat_two.GetPosition();
        var p3 = mat_three.GetPosition(); var p4 = mat_four.GetPosition();
        var p5 = mat_five.GetPosition();

        Result123.transform.position = (p1 + p2 + p3) * (1f / 3f);
        Result124.transform.position = (p1 + p2 + p4) * (1f / 3f);
        Result125.transform.position = (p1 + p2 + p5) * (1f / 3f);
        Result134.transform.position = (p1 + p3 + p4) * (1f / 3f);
        Result135.transform.position = (p1 + p3 + p5) * (1f / 3f);
        Result145.transform.position = (p1 + p4 + p5) * (1f / 3f);
        Result234.transform.position = (p2 + p3 + p4) * (1f / 3f);
        Result235.transform.position = (p2 + p3 + p5) * (1f / 3f);
        Result245.transform.position = (p2 + p4 + p5) * (1f / 3f);
        Result345.transform.position = (p3 + p4 + p5) * (1f / 3f);

        Result1234.transform.position = (p1 + p2 + p3 + p4) * (1f / 4f);
        Result1235.transform.position = (p1 + p2 + p3 + p5) * (1f / 4f);
        Result1245.transform.position = (p1 + p2 + p4 + p5) * (1f / 4f);
        Result1345.transform.position = (p1 + p3 + p4 + p5) * (1f / 4f);
        Result2345.transform.position = (p2 + p3 + p4 + p5) * (1f / 4f);

        Result12345.transform.position = (p1 + p2 + p3 + p4 + p5) * (1f / 5f);
    }

    /// <summary>
    /// Set active of result.
    /// </summary>
    /// <param name="active">true/false</param>
    /// <param name="part">0: all, 3: three-combo, 4:four-combo, 5:five-combo</param>
    void CombinationObjectSetActive(bool active, short part)
    {
        if (part == 3 || part == 0)
        {
            Result123.SetActive(active);
            Result124.SetActive(active);
            Result125.SetActive(active);
            Result134.SetActive(active);
            Result135.SetActive(active);
            Result145.SetActive(active);
            Result234.SetActive(active);
            Result235.SetActive(active);
            Result245.SetActive(active);
            Result345.SetActive(active);
        }

        if (part == 4 || part == 0)
        {
            Result1234.SetActive(active);
            Result1235.SetActive(active);
            Result1245.SetActive(active);
            Result1345.SetActive(active);
            Result2345.SetActive(active);
        }

        if (part == 5 || part == 0)
        {
            Result12345.SetActive(active);
        }
    }

    void CombinationRotationReset()
    {
        Result123.transform.rotation = Quaternion.identity;
        Result124.transform.rotation = Quaternion.identity;
        Result125.transform.rotation = Quaternion.identity;
        Result134.transform.rotation = Quaternion.identity;
        Result135.transform.rotation = Quaternion.identity;
        Result145.transform.rotation = Quaternion.identity;
        Result234.transform.rotation = Quaternion.identity;
        Result235.transform.rotation = Quaternion.identity;
        Result245.transform.rotation = Quaternion.identity;
        Result345.transform.rotation = Quaternion.identity;

        Result1234.transform.rotation = Quaternion.identity;
        Result1235.transform.rotation = Quaternion.identity;
        Result1245.transform.rotation = Quaternion.identity;
        Result1345.transform.rotation = Quaternion.identity;
        Result2345.transform.rotation = Quaternion.identity;

        Result12345.transform.rotation = Quaternion.identity;
    }

    void DebugQuaternionforEigen()
    {
        Vector3[] pos = { m_Source.transform.position,
                             m_One.transform.position,
                             m_Two.transform.position,
                             m_Three.transform.position,
                             m_Four.transform.position,
                             m_Five.transform.position };

        Quaternion[] eul = { m_Source.transform.rotation,
                             m_One.transform.rotation,
                             m_Two.transform.rotation,
                             m_Three.transform.rotation,
                             m_Four.transform.rotation,
                             m_Five.transform.rotation };

        Debug.Log("\n" +
                  "=== Object initial quaternion rotation: ===" + "\n" +
                  "Source: " + "\tpos: " + pos[0] + "\trot: " + eul[0] + "\n" +
                  "One: " + "\tpos: " + pos[1] + "\trot: " + eul[1] + "\n" +
                  "Two: " + "\tpos: " + pos[2] + "\trot: " + eul[2] + "\n" +
                  "Three: " + "\tpos: " + pos[3] + "\trot: " + eul[3] + "\n" +
                  "Four: " + "\tpos: " + pos[4] + "\trot: " + eul[4] + "\n" +
                  "Five: " + "\tpos: " + pos[5] + "\trot: " + eul[5] + "\n" +
                  "=== =================================== ===" + "\n");
    }

    void DebugResultObject(string context, string rotation = "quaternion")
    {
        Vector3[] pos = { Result123.transform.position,
                          Result124.transform.position,
                          Result125.transform.position,
                          Result134.transform.position,
                          Result135.transform.position,
                          Result145.transform.position,
                          Result234.transform.position,
                          Result235.transform.position,
                          Result245.transform.position,
                          Result345.transform.position,

                          Result1234.transform.position,
                          Result1235.transform.position,
                          Result1245.transform.position,
                          Result1345.transform.position,
                          Result2345.transform.position,

                          Result12345.transform.position};

        Vector3[] eul = { Result123.transform.eulerAngles,
                          Result124.transform.eulerAngles,
                          Result125.transform.eulerAngles,
                          Result134.transform.eulerAngles,
                          Result135.transform.eulerAngles,
                          Result145.transform.eulerAngles,
                          Result234.transform.eulerAngles,
                          Result235.transform.eulerAngles,
                          Result245.transform.eulerAngles,
                          Result345.transform.eulerAngles,

                          Result1234.transform.eulerAngles,
                          Result1235.transform.eulerAngles,
                          Result1245.transform.eulerAngles,
                          Result1345.transform.eulerAngles,
                          Result2345.transform.eulerAngles,

                          Result12345.transform.eulerAngles};

        Quaternion[] rot = { Result123.transform.rotation,
                             Result124.transform.rotation,
                             Result125.transform.rotation,
                             Result134.transform.rotation,
                             Result135.transform.rotation,
                             Result145.transform.rotation,
                             Result234.transform.rotation,
                             Result235.transform.rotation,
                             Result245.transform.rotation,
                             Result345.transform.rotation,

                             Result1234.transform.rotation,
                             Result1235.transform.rotation,
                             Result1245.transform.rotation,
                             Result1345.transform.rotation,
                             Result2345.transform.rotation,

                             Result12345.transform.rotation};

        if (rotation == "quaternion")
        {
            Debug.Log("\n" +
                  "=== Result with " + context + " position and rotation (quaternion) ===" + "\n" +
                  "Avg(1,2,3): " + "\tpos: " + pos[0] + "\trot: " + rot[0] + "\n" +
                  "Avg(1,2,4): " + "\tpos: " + pos[1] + "\trot: " + rot[1] + "\n" +
                  "Avg(1,2,5): " + "\tpos: " + pos[2] + "\trot: " + rot[2] + "\n" +
                  "Avg(1,2,4): " + "\tpos: " + pos[3] + "\trot: " + rot[3] + "\n" +
                  "Avg(1,3,5): " + "\tpos: " + pos[4] + "\trot: " + rot[4] + "\n" +
                  "Avg(1,3,5): " + "\tpos: " + pos[5] + "\trot: " + rot[5] + "\n" +
                  "Avg(2,3,4): " + "\tpos: " + pos[6] + "\trot: " + rot[6] + "\n" +
                  "Avg(2,3,5): " + "\tpos: " + pos[7] + "\trot: " + rot[7] + "\n" +
                  "Avg(2,4,5): " + "\tpos: " + pos[8] + "\trot: " + rot[8] + "\n" +
                  "Avg(3,4,5): " + "\tpos: " + pos[9] + "\trot: " + rot[9] + "\n" +

                  "Avg(1,2,3,4): " + "\tpos: " + pos[10] + "\trot: " + rot[10] + "\n" +
                  "Avg(1,2,3,5): " + "\tpos: " + pos[11] + "\trot: " + rot[11] + "\n" +
                  "Avg(1,2,4,5): " + "\tpos: " + pos[12] + "\trot: " + rot[12] + "\n" +
                  "Avg(1,3,4,5): " + "\tpos: " + pos[13] + "\trot: " + rot[13] + "\n" +
                  "Avg(2,3,4,5): " + "\tpos: " + pos[14] + "\trot: " + rot[14] + "\n" +

                  "Avg(1,2,3,4,5): " + "\tpos: " + pos[15] + "\trot: " + rot[15] + "\n" +
                  "=== ========================================================= ===" + "\n");
        }

        if (rotation == "euler")
        {
            Debug.Log("\n" +
                  "=== Result with " + context + " position and rotation (Euler Angle) ===" + "\n" +
                  "Avg(1,2,3): " + "\tpos: " + pos[0] + "\trot: " + eul[0] + "\n" +
                  "Avg(1,2,4): " + "\tpos: " + pos[1] + "\trot: " + eul[1] + "\n" +
                  "Avg(1,2,5): " + "\tpos: " + pos[2] + "\trot: " + eul[2] + "\n" +
                  "Avg(1,2,4): " + "\tpos: " + pos[3] + "\trot: " + eul[3] + "\n" +
                  "Avg(1,3,5): " + "\tpos: " + pos[4] + "\trot: " + eul[4] + "\n" +
                  "Avg(1,3,5): " + "\tpos: " + pos[5] + "\trot: " + eul[5] + "\n" +
                  "Avg(2,3,4): " + "\tpos: " + pos[6] + "\trot: " + eul[6] + "\n" +
                  "Avg(2,3,5): " + "\tpos: " + pos[7] + "\trot: " + eul[7] + "\n" +
                  "Avg(2,4,5): " + "\tpos: " + pos[8] + "\trot: " + eul[8] + "\n" +
                  "Avg(3,4,5): " + "\tpos: " + pos[9] + "\trot: " + eul[9] + "\n" +

                  "Avg(1,2,3,4): " + "\tpos: " + pos[10] + "\trot: " + eul[10] + "\n" +
                  "Avg(1,2,3,5): " + "\tpos: " + pos[11] + "\trot: " + eul[11] + "\n" +
                  "Avg(1,2,4,5): " + "\tpos: " + pos[12] + "\trot: " + eul[12] + "\n" +
                  "Avg(1,3,4,5): " + "\tpos: " + pos[13] + "\trot: " + eul[13] + "\n" +
                  "Avg(2,3,4,5): " + "\tpos: " + pos[14] + "\trot: " + eul[14] + "\n" +

                  "Avg(1,2,3,4,5): " + "\tpos: " + pos[15] + "\trot: " + eul[15] + "\n" +
                  "=== ========================================================= ===" + "\n");
        }
    }
}

//Result123.
//Result124.
//Result125.
//Result134.
//Result135.
//Result145.
//Result234.
//Result235.
//Result245.
//Result345.

//Result1234.
//Result1235.
//Result1245.
//Result1345.
//Result2345.

//Result12345.