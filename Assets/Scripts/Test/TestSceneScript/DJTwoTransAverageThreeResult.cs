using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Notes:
/// - Our purpose is to deliver an average of two object from one source.
/// - The source is the blue object, while the rest are the target objects.
///
/// - At first, the idea is to get the transformation matrix from the source
///   to each object, which we calculate through the world coordinate (Unity)
///   system.
/// - The base formulation is from source --> world --> target
///   Ts->t = (Tt->w)^-1 * Ts->w
/// - Since the target objects have rotation property, while inverse them or
///   multiply with other matrices, it will affect the translation property.
///   
/// - Since Unity engine is very smart, it makes all kind of relationship for
///   end-user and developer to no need to think of all these relationship,
///   for example: Position, Rotation, Scale, but we are here also want
///   to understand the math logic.
///   
/// - Instead of using Ts->t, we use Unity world coordinate as basis for all
///   objects being drawn inside.
/// - We use transform.localToWorldMatrix to get relationship between object to
///   Unity world origin.
/// - By this manner, we can transform from one to another without think their
///   each relationship, although by concept it is still the same.
/// - For example, instead of transforming from source to target, we use both
///   relationship from source to world (Ts->w), and target to world (Tt->w)
/// - Even we can inverse one of them to get either way of transformation, it is
///   still the same, yet matrices calculation can make it different because of
///   non identity of rotation matrix.
/// </summary>

public class DJTwoTransAverageThreeResult : MonoBehaviour
{
#pragma warning disable 0414
    [SerializeField]
    string text = "=== GameObject ===";

    [SerializeField]
    GameObject m_Source, m_One, m_Two, m_Three;

    [SerializeField]
    string text_2 = "=== TextMesh above the result ===";

    [SerializeField]
    GameObject m_T12, m_T23, m_T13;

    [SerializeField]
    string text_3 = "=== Activation button ===";

    [SerializeField]
    bool m_DirectAveraging = false, m_SLERP = false,
         m_Eigenvector = false, m_ResetAll = false;
    bool debug = false;

    Matrix4x4 InitSource, ms1, ms2, ms3;
    GameObject Result12, Result23, Result13;

    //[SerializeField]
    //[Range(0, 1)]
    //float t = 1;
#pragma warning restore 0414

    // Start is called before the first frame update
    void Start()
    {
        InitSource = MakingNewMatrix4x4(m_Source.transform.localToWorldMatrix);

        ms1 = GetTransformationMatrix(m_Source, m_One);
        ms2 = GetTransformationMatrix(m_Source, m_Two);
        ms3 = GetTransformationMatrix(m_Source, m_Three);

        //ms1 = GetTransformationMatrix(m_One, m_Source);
        //ms2 = GetTransformationMatrix(m_Two, m_Source);
        //ms3 = GetTransformationMatrix(m_Three, m_Source);

        Result12 = Instantiate(m_Source);   Result12.name = "_m_Result12";
        Result23 = Instantiate(m_Source);   Result23.name = "_m_Result23";
        Result13 = Instantiate(m_Source);   Result13.name = "_m_Result13";

        ResultGameObjectSetActive(false);

        DebugBasedObject();

        /// TEST ///
        /// Move m_One to m_Source ///
        /// Result: Success ///
        //var one_to_source = GetTransformationMatrix(m_One, m_Source);   // one -> w -> source
        //var source_pos = m_Source.transform.position;                   // source -> w as position
        //var new_one_pos = one_to_source * source_pos;
        //m_One.transform.position = new(new_one_pos.x, new_one_pos.y, new_one_pos.z);

        /// TEST ///
        //Debug.Log(ms1.GetPosition());
    }

    // Update is called once per frame
    void Update()
    {
        if (m_DirectAveraging)  { DirectAveraging(); }
        if (m_SLERP) { SLERP(); }
        if (m_Eigenvector) { Eigenvector(); }

        if (m_ResetAll) { ResetAll(); }

        /// TEST ///
        /// Move m_One to m_Source with interpolation ///
        /// Result: Only change the translation to world origin (0,0,0)
        //var one_to_source = GetTransformationMatrix(m_One, m_Source);   // one -> w -> source
        //var source_pos = m_Source.transform.position;                   // source -> w as position
        //var new_one_pos = one_to_source * (source_pos * t);
        //m_One.transform.position = new(new_one_pos.x, new_one_pos.y, new_one_pos.z);
    }

    /// <summary>
    /// Get transformation matrix from source object to destination object
    /// through Unity world coordinate.
    /// </summary>
    Matrix4x4 GetTransformationMatrix(GameObject source, GameObject destination)
    {
        return Matrix4x4.Inverse(destination.transform.localToWorldMatrix)  // Tw->d
            * source.transform.localToWorldMatrix;                          // Ts->w
    }

    Matrix4x4 MakingNewMatrix4x4(Matrix4x4 m)
    {
        //Vector4 v1 = new(m.m00, m.m01, m.m02, m.m03);
        //Vector4 v2 = new(m.m10, m.m11, m.m12, m.m13);
        //Vector4 v3 = new(m.m20, m.m21, m.m22, m.m23);
        //Vector4 v4 = new(m.m30, m.m31, m.m32, m.m33);

        Vector4 v1 = new(m.m00, m.m10, m.m20, m.m30);
        Vector4 v2 = new(m.m01, m.m11, m.m21, m.m31);
        Vector4 v3 = new(m.m02, m.m12, m.m22, m.m32);
        Vector4 v4 = new(m.m03, m.m13, m.m23, m.m33);

        return new(v1, v2, v3, v4);
    }

    void DirectAveraging()
    {
        PositionAveraging();

        var r1 = m_One.transform.rotation;
        var r2 = m_Two.transform.rotation;
        var r3 = m_Three.transform.rotation;

        //Quaternion r12 = new(ms1.rotation.x + ms2.rotation.x,
        //                     ms1.rotation.y + ms2.rotation.y,
        //                     ms1.rotation.z + ms2.rotation.z,
        //                     ms1.rotation.w + ms2.rotation.w);

        //Quaternion r23 = new(ms2.rotation.x + ms3.rotation.x,
        //                     ms2.rotation.y + ms3.rotation.y,
        //                     ms2.rotation.z + ms3.rotation.z,
        //                     ms2.rotation.w + ms3.rotation.w);

        //Quaternion r13 = new(ms1.rotation.x + ms3.rotation.x,
        //                     ms1.rotation.y + ms3.rotation.y,
        //                     ms1.rotation.z + ms3.rotation.z,
        //                     ms1.rotation.w + ms3.rotation.w);

        Quaternion r12 = new(r1.x + r2.x, r1.y + r2.y, r1.z + r2.z, r1.w + r2.w);
        Quaternion r23 = new(r2.x + r3.x, r2.y + r3.y, r2.z + r3.z, r2.w + r3.w);
        Quaternion r13 = new(r1.x + r3.x, r1.y + r3.y, r1.z + r3.z, r1.w + r3.w);

        Result12.transform.rotation = r12.normalized;
        Result23.transform.rotation = r23.normalized;
        Result13.transform.rotation = r13.normalized;

        ResultGameObjectSetActive(true);

        if (!debug) { DebugResultObject("Direct Averaging"); debug = true; }
    }

    void SLERP()
    {
        PositionAveraging();

        var r1 = m_One.transform.rotation;
        var r2 = m_Two.transform.rotation;
        var r3 = m_Three.transform.rotation;

        Quaternion r12 = Quaternion.Slerp(r1, r2, 0.5f);
        Quaternion r23 = Quaternion.Slerp(r2, r3, 0.5f);
        Quaternion r13 = Quaternion.Slerp(r1, r3, 0.5f);

        Result12.transform.rotation = r12.normalized;
        Result23.transform.rotation = r23.normalized;
        Result13.transform.rotation = r13.normalized;

        ResultGameObjectSetActive(true);

        if (!debug) { DebugResultObject("SLERP"); debug = true; }
    }

    void Eigenvector()
    {
        PositionAveraging();

        var r1 = m_One.transform.rotation;
        var r2 = m_Two.transform.rotation;
        var r3 = m_Three.transform.rotation;

        // calculate with Eigen c++ library in cpp
        // we haven't include the dll library in here

        // the answers:
        var r12 = new Quaternion(0, 0.194515f, 0, 0.980900f);
        var r23 = new Quaternion(0, 0.939658f, 0, 0.342117f);
        var r13 = new Quaternion(0, 0.806308f, 0, -0.591495f);

        Result12.transform.rotation = r12;
        Result23.transform.rotation = r23;
        Result13.transform.rotation = r13;

        ResultGameObjectSetActive(true);

        if (!debug) { DebugResultObject("Eigenvector"); debug = true; }
    }

    void ResetAll()
    {
        ResultGameObjectSetActive(false);

        ResetGameObject(Result12, InitSource);
        ResetGameObject(Result23, InitSource);
        ResetGameObject(Result13, InitSource);

        m_DirectAveraging = false;
        m_SLERP = false;
        m_Eigenvector = false;
        debug = false;

        m_ResetAll = false;
    }

    void PositionAveraging()
    {
        var ps1 = m_One.transform.localToWorldMatrix.GetPosition();
        var ps2 = m_Two.transform.localToWorldMatrix.GetPosition();
        var ps3 = m_Three.transform.localToWorldMatrix.GetPosition();
              
        var p12 = (ps1 + ps2) * 0.5f;
        var p23 = (ps2 + ps3) * 0.5f;
        var p13 = (ps1 + ps3) * 0.5f;
              
        Result12.transform.position = p12;
        Result23.transform.position = p23;
        Result13.transform.position = p13;
    }

    void ResultGameObjectSetActive(bool trigger)
    {
        Result12.SetActive(trigger);
        Result23.SetActive(trigger);
        Result13.SetActive(trigger);

        m_T12.SetActive(trigger);
        m_T23.SetActive(trigger);
        m_T13.SetActive(trigger);
    }

    void ResetGameObject(GameObject go, Matrix4x4 m)
    {
        go.transform.SetPositionAndRotation(m.GetPosition(), m.rotation);
    }

    void DebugBasedObject()
    {
        Vector3[] pos = { m_Source.transform.position,
                          m_One.transform.position,
                          m_Two.transform.position,
                          m_Three.transform.position };

        Vector3[] eul = { m_Source.transform.eulerAngles,
                          m_One.transform.eulerAngles,
                          m_Two.transform.eulerAngles,
                          m_Three.transform.eulerAngles };

        Debug.Log("\n" +
                  "=== Initial position and rotation (euler) ===" + "\n" +
                  "Source: " + "\tpos: " + Vector3Debug(pos[0]) + "\trot: " + Vector3Debug(eul[0]) + "\n" +
                  "One: " + "\tpos: " + Vector3Debug(pos[1]) + "\trot: " + Vector3Debug(eul[1]) + "\n" +
                  "Two: " + "\tpos: " + Vector3Debug(pos[2]) + "\trot: " + Vector3Debug(eul[2]) + "\n" +
                  "Three: " + "\tpos: " + Vector3Debug(pos[3]) + "\trot: " + Vector3Debug(eul[3]) + "\n" +
                  "=== ===================================== ===" + "\n");
    }

    void DebugResultObject(string context)
    {
        Vector3[] pos = { Result12.transform.position,
                          Result23.transform.position,
                          Result13.transform.position};

        Vector3[] eul = { Result12.transform.eulerAngles,
                          Result23.transform.eulerAngles,
                          Result13.transform.eulerAngles};

        Debug.Log("\n" +
                  "=== Result with " + context + " position and rotation (euler) ===" + "\n" +
                  "Avg(1,2): " + "\tpos: " + Vector3Debug(pos[0]) + "\trot: " + Vector3Debug(eul[0]) + "\n" +
                  "Avg(2,3): " + "\tpos: " + Vector3Debug(pos[1]) + "\trot: " + Vector3Debug(eul[1]) + "\n" +
                  "Avg(1,3): " + "\tpos: " + Vector3Debug(pos[2]) + "\trot: " + Vector3Debug(eul[2]) + "\n" +
                  "=== ========================================================= ===" + "\n");
    }

    string Vector3Debug(Vector3 v)
    {
        return "(X: " + v.x + ", Y: " + v.y + ", Z: " + v.z + ")";
    }
}
