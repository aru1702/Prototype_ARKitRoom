using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eigen : MonoBehaviour
{
    [SerializeField]
    GameObject m_ObjectOne, m_ObjectTwo, m_ObjectThree, m_ObjectResult;

    // Start is called before the first frame update
    void Start()
    {
        //Matrix4x4 mat = transform.localToWorldMatrix;
        //Debug.Log(mat);

        //Vector4 c1 = new(-3f, -3f, 0f, 0f);     // Transpose
        //Vector4 c2 = new(2f, 4f, 0f, 0f);
        //Vector4 c3 = new(0f, 0f, -5f, -2f);
        //Vector4 c4 = new(0f, 0f, -4f, 2f);

        //Matrix4x4 mat = new(c1, c2, c3, c4);
        //Debug.Log(mat);

        //A(mat);


    }

    // Update is called once per frame
    void Update()
    {
        // initialization
        var dist_1 = Vector3.Distance(m_ObjectOne.transform.position, m_ObjectResult.transform.position);
        var dist_2 = Vector3.Distance(m_ObjectTwo.transform.position, m_ObjectResult.transform.position);
        var dist_3 = Vector3.Distance(m_ObjectThree.transform.position, m_ObjectResult.transform.position);

        float w1 = MathFunctions.Sigmoid(dist_1, true, 1);
        float w2 = MathFunctions.Sigmoid(dist_2, true, 1);
        float w3 = MathFunctions.Sigmoid(dist_3, true, 1);

        var one_rot = m_ObjectOne.transform.rotation;
        var two_rot = m_ObjectTwo.transform.rotation;
        var tre_rot = m_ObjectThree.transform.rotation;

        Quaternion[] qs = { one_rot, two_rot, tre_rot };
        float[] ws = { w1, w2, w3 };
        ws = MathFunctions.NormalizedMany(ws);

        // execution
        Matrix4x4 mat = new();
        for (int i = 0; i < qs.Length; i++)
        {
            Vector4 v4 = new(qs[i].x, qs[i].y, qs[i].z, qs[i].w);
            float w = float.IsNaN(ws[i]) ? 1.0f : ws[i];
            Matrix4x4 tm = V4CrossProd(v4, v4);
            tm = V4ScalarProd(tm, ws[i]);
            mat = Mat4Adder(mat, tm);
        }

        EigenCheck(mat, out Vector4 eigen_value);
        float e_value_0 = eigen_value.w;

        Debug.Log(e_value_0);
    }

    void EigenCheck (Matrix4x4 mat, out Vector4 eigen)
    {
        float a = mat.m00; float b = mat.m01; float c = mat.m02; float d = mat.m03;
        float e = mat.m10; float f = mat.m11; float g = mat.m12; float h = mat.m13;
        float i = mat.m20; float j = mat.m21; float k = mat.m22; float l = mat.m23;
        float m = mat.m30; float n = mat.m31; float o = mat.m32; float p = mat.m33;

        // matrix to Ax^4 + Bx^3 + Cx^2 + Dx + E = 0
        float A = 1;
        float B = -(a + f + k + p);
        float C = a * f + a * k + a * p + f * k + f * p + k * p;
        float D = -(a * f * k + a * f * p + a * k * p + f * k * p);
        D += n * k * h + n * h * a + p * i * c + i * f * c;
        float E = b * g * l * m + c * h * i * n + d * e * j * o;
        E -= (m * j * g * d + o * l * e * b);
        E += a * f * k * p - n * k * h * a - p * i * f * c;

        Debug.Log("A: " + A + ", B: " + B + ", C: " + C + ", D: " + D + ", E: " + E);


        // See more https://en.wikipedia.org/wiki/Quartic_function

        // 1. determinant
        float Det = 256 * Pow3(A) * Pow3(E);
        Det -= 192 * Pow2(A) * B * D * Pow2(E);
        Det -= 128 * Pow2(A) * Pow2(C) * Pow2(E);
        Det += 144 * Pow2(A) * C * Pow2(D) * E;
        Det -= 27 * Pow2(A) * Pow4(D);
        Det += 144 * A * Pow2(B) * C * Pow2(E);
        Det -= 6 * A * Pow2(B) * Pow2(D) * E;
        Det -= 80 * A * B * Pow2(C) * D * E;
        Det += 18 * A * B * C * Pow3(D);
        Det += 16 * A * Pow4(C) * E;
        Det -= 4 * A * Pow3(C) * Pow2(D);
        Det -= 27 * Pow4(B) * Pow2(E);
        Det += 18 * Pow3(B) * C * D * E;
        Det -= 4 * Pow3(B) * Pow3(D);
        Det -= 4 * Pow2(B) * Pow3(C) * E;
        Det += Pow2(B) * Pow2(C) * Pow2(D);

        Debug.Log("Det: " + Det);

        // 2. determinant of 2,1 and 3,0
        float Det_21_30 = -27 * Det;

        Debug.Log("Det_21_30: " + Det_21_30);

        // 3. determinant 0 and 1
        float Det_0 = Pow2(C) - 3 * B * D + 12 * A * E;
        float Det_1 = 2 * Pow3(C)
            - 9 * B * C * D + 27 * Pow2(B) * E
            + 27 * A * Pow2(D)
            - 72 * A * C * E;

        Debug.Log("Det_0: " + Det_0);
        Debug.Log("Det_1: " + Det_1);

        // 4. find fp and fq
        float fp = 8 * A * C - 3 * Pow2(B) / (8 * Pow2(A));
        float fq = Pow3(B) - 4 * A * B * C + 8 * Pow2(A) * D / (8 * Pow3(A));

        Debug.Log("fp: " + fp);
        Debug.Log("fq: " + fq);

        // 5. find S and Q
        //float Q = Cbrt((Det_1 + Sqrt(Det_21_30)) / 2);
        float Q = Cbrt((Det_1 + Sqrt(Pow2(Det_1) - 4 * Pow3(Det_0))) / 2);
        float S = 1 / 2 * Sqrt(-2 * fp / 3 + (Q + (Det_0 / Q)) / (3 * A));

        //if (S <= 0)
        //{
        //    Q = Cbrt((Det_1 + Sqrt(Pow2(Det_1) - 4 * Pow3(Det_0))) / 2);
        //    S = 1 / 2 * Sqrt(-2 * fp / 3 + (Q + (Det_0 / Q)) / (3 * A));
        //}

        Debug.Log("Q: " + Q);
        Debug.Log("S: " + S);

        // 6. quadratic X
        float X1 = -b / (4 * a) - S + (1 / 2) * Sqrt(-4 * Pow2(S) - 2 * fp + fq / S);
        float X2 = -b / (4 * a) - S - (1 / 2) * Sqrt(-4 * Pow2(S) - 2 * fp + fq / S);
        float X3 = -b / (4 * a) + S + (1 / 2) * Sqrt(-4 * Pow2(S) - 2 * fp + fq / S);
        float X4 = -b / (4 * a) + S - (1 / 2) * Sqrt(-4 * Pow2(S) - 2 * fp + fq / S);

        Debug.Log("X1: " + X1 + ", X2: " + X2 + ", X3: " + X3 + ", X4: " + X4);

        eigen = new(X1, X2, X3, X4);
    }

    float Pow2(float f) { return Mathf.Pow(f, 2); }
    float Pow3(float f) { return Mathf.Pow(f, 3); }
    float Pow4(float f) { return Mathf.Pow(f, 4); }
    float Sqrt(float f) { return Mathf.Sqrt(f); }
    float Cbrt(float f) { return Mathf.Pow(f, (1/3)); }

    Matrix4x4 V4CrossProd(Vector4 vl, Vector4 vr)
    {
        float a = vl.w * vr.w;  float b = vl.w * vr.x; float c = vl.w * vr.y; float d = vl.w * vr.z;
        float e = vl.x * vr.w; float f = vl.x * vr.x; float g = vl.x * vr.y; float h = vl.x * vr.z;
        float i = vl.y * vr.w; float j = vl.y * vr.x; float k = vl.y * vr.y; float l = vl.y * vr.z;
        float m = vl.z * vr.w; float n = vl.z * vr.x; float o = vl.z * vr.y; float p = vl.z * vr.z;

        Vector4 col_01 = new(a, e, i, m);
        Vector4 col_02 = new(b, f, j, n);
        Vector4 col_03 = new(c, g, k, o);
        Vector4 col_04 = new(d, h, l, p);

        return new(col_01, col_02, col_03, col_04);
    }

    Matrix4x4 V4ScalarProd(Matrix4x4 m, float s)
    {
        Vector4 col_01 = new(m.m00 * s, m.m10 * s, m.m20 * s, m.m30 * s);
        Vector4 col_02 = new(m.m01 * s, m.m11 * s, m.m21 * s, m.m31 * s);
        Vector4 col_03 = new(m.m02 * s, m.m12 * s, m.m22 * s, m.m32 * s);
        Vector4 col_04 = new(m.m03 * s, m.m13 * s, m.m23 * s, m.m33 * s);

        return new(col_01, col_02, col_03, col_04);
    }

    Matrix4x4 Mat4Adder(Matrix4x4 ml, Matrix4x4 mr)
    {
        Vector4 col_01 = new(ml.m00 + mr.m00, ml.m10 + mr.m10, ml.m20 + mr.m20, ml.m30 + mr.m30);
        Vector4 col_02 = new(ml.m01 + mr.m01, ml.m11 + mr.m11, ml.m21 + mr.m21, ml.m31 + mr.m31);
        Vector4 col_03 = new(ml.m02 + mr.m02, ml.m12 + mr.m12, ml.m22 + mr.m22, ml.m32 + mr.m32);
        Vector4 col_04 = new(ml.m03 + mr.m03, ml.m13 + mr.m13, ml.m23 + mr.m23, ml.m33 + mr.m33);

        return new(col_01, col_02, col_03, col_04);
    }
}
