using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class AvgTwoQuaternions : MonoBehaviour
{
    //[DllImport("EigenToUnity02")]
    //public static extern Quaternion<float> AverageTwoQuaternion(Vector4f q1, Vector4f q2);
    //public static extern Vector4f Convert4QtoVector4f(float w, float x, float y, float z);
    //public static extern float ConvertQFloattoEachFloat(Quaternion<float> q, string s);
    // --> import unknown data type of Unity from C++ libraries is not working

    //[DllImport("EigenToUnity03")]
    //public static extern void AverageTwoQuaternion(
    //    float q1_w, float q1_x, float q1_y, float q1_z,
    //    float q2_w, float q2_x, float q2_y, float q2_z,
    //    float qr_w, float qr_x, float qr_y, float qr_z);

    //Quaternion AvgTwoQ(Quaternion q1, Quaternion q2)
    //{
    //    float w = 0, x = 0, y = 0, z = 0;
    //    AverageTwoQuaternion(q1.w, q1.x, q1.y, q1.z,
    //                         q2.w, q2.x, q2.y, q2.z,
    //                         w, x, y, z);
    //    return new Quaternion(x, y, z, w);
    //}
    // --> pointer usage in Unity will return loop error, and crash

    // Source: https://stackoverflow.com/questions/30399339/marshalling-float-array-to-c-sharp
    public unsafe struct FloatArray4f
    {
        public float data1;
        public float data2;
        public float data3;
        public float data4;

        public float[] Data
        {
            get
            {
                return new[] { data1, data2, data3, data4 };
            }
        }
    }

    //[DllImport("EigenToUnity001", CallingConvention = CallingConvention.Cdecl)]
    //private static extern FloatArray4f getData();

    public unsafe struct QuaternionUnits
    {
        public float x; 
        public float y;
        public float z;
        public float w;

        public float[] Data
        {
            get
            {
                return new[] { x, y, z, w };
            }
        }

        public override string ToString()
        {
            return x.ToString() + "\t" +
                y.ToString() + "\t" +
                z.ToString() + "\t" +
                w.ToString();
        }
    }

    public unsafe struct Matrix4x4Units
    {
        public float[] v0;
        public float[] v1;
        public float[] v2;
        public float[] v3;

        public override string ToString()
        {
            return v0[0] + "\t" + v1[0] + "\t" + v2[0] + "\t" + v3[0] + "\n" +
                v0[1] + "\t" + v1[1] + "\t" + v2[1] + "\t" + v3[1] + "\n" +
                v0[2] + "\t" + v1[2] + "\t" + v2[2] + "\t" + v3[2] + "\n" +
                v0[3] + "\t" + v1[3] + "\t" + v2[3] + "\t" + v3[3];
        }
    }

    //[DllImport("EigenToUnity06", CallingConvention = CallingConvention.Cdecl)]
    //private static extern QuaternionUnits GetAvgQuaternions(Matrix4x4Units m);

    [DllImport("EigenToUnity08", CallingConvention = CallingConvention.Cdecl)]
    private static extern float Qxyzw(Matrix4x4Units m, int i);

    [DllImport("EigenToUnity08", CallingConvention = CallingConvention.Cdecl)]
    private static extern float Eval(Matrix4x4Units m, int i);

    [DllImport("EigenToUnity10", CallingConvention = CallingConvention.Cdecl)]
    private static extern float QDef(int i);

    [DllImport("EigenToUnity10", CallingConvention = CallingConvention.Cdecl)]
    private static extern float QDef2(int i);

    [DllImport("EigenToUnity11", CallingConvention = CallingConvention.Cdecl)]
    private static extern QuaternionUnits GetAvgQuaternions
                                (float m00, float m01, float m02, float m03,
                                float m10, float m11, float m12, float m13,
                                float m20, float m21, float m22, float m23,
                                float m30, float m31, float m32, float m33);

    [SerializeField]
    Vector3 rot1, rot2;

    // Start is called before the first frame update
    void Start()
    {
        // Y: 30
        //Quaternion q1 = new(0, 0.258819f, 0, 0.9659258f);
        Quaternion q1 = new();
        q1.eulerAngles = rot1;

        // Y: 90
        //Quaternion q2 = new(0, 0.7071068f, 0, 0.7071068f);
        Quaternion q2 = new();
        q2.eulerAngles = rot2;

        // sum product then arithmetic sum
        Matrix4x4 m = Matrix4x4.zero;
        Debug.Log(m.ToString());

        m = SumOfMatrix4x4(m, OuterProduct(q1, q1));
        Debug.Log(m.ToString());

        m = SumOfMatrix4x4(m, OuterProduct(q2, q2));
        Debug.Log(m.ToString());

        // eigen function, then convert
        Matrix4x4Units m4x4_units = ConvertToMatrix4x4Units(m);
        Debug.Log(m4x4_units.ToString());

        //QuaternionUnits q_units = GetAvgQuaternions(m4x4_units);
        QuaternionUnits q_units = GetAvgQuaternions(m.m00, m.m01, m.m02, m.m03,
                                                    m.m10, m.m11, m.m12, m.m13,
                                                    m.m20, m.m21, m.m22, m.m23,
                                                    m.m30, m.m31, m.m32, m.m33);
        Debug.Log(q_units.ToString());

        Quaternion avg_q = new(q_units.x, q_units.y, q_units.z, q_units.w);

        // check data
        Debug.Log(avg_q.ToString());

        Debug.Log("Eigenvalue:");

        float i = Eval(m4x4_units, 0);
        float ii = Eval(m4x4_units, 1);
        float iii = Eval(m4x4_units, 2);
        float iv = Eval(m4x4_units, 3);

        Debug.Log(i); Debug.Log(ii); Debug.Log(iii); Debug.Log(iv);

        Debug.Log("Quaternions:");

        float x = Qxyzw(m4x4_units, 0);
        float y = Qxyzw(m4x4_units, 1);
        float z = Qxyzw(m4x4_units, 2);
        float w = Qxyzw(m4x4_units, 3);

        Debug.Log(x); Debug.Log(y); Debug.Log(z); Debug.Log(w);

        Debug.Log("Defined quaternions:");
        Debug.Log(QDef(0)); Debug.Log(QDef(1)); Debug.Log(QDef(2)); Debug.Log(QDef(3));
        Debug.Log(QDef2(0)); Debug.Log(QDef2(1)); Debug.Log(QDef2(2)); Debug.Log(QDef2(3));

        Quaternion qq = new(QDef(0), QDef(1), QDef(2), QDef(3));
        qq = qq.normalized;

        Debug.Log(qq);

        // --> this is working
        //FloatArray4f data = getData();
        //Vector4 v = new(data.data1, data.data2, data.data3, data.data4);
        //Debug.Log(v.ToString());

        // --> IntPtr is not a float
        //float[] f = new { q1.x, q1.y, q1.z, q1.w };
        //IntPtr pointer = Marshal.AllocHGlobal(f.Length);
        //Marshal.Copy(f, 0, pointer, f.Length);
        //Debug.Log(pointer);
        //Marshal.FreeHGlobal(pointer);

        //Quaternion r = AvgTwoQ(q1, q2);
        //Debug.Log(r);


    }

    // input:
    // - Quat: two / multiple quaternions
    // - int: n of quaternions

    // output to Eigen:
    // - matrix4x4

    // input from Eigen:
    // - Quaternion v4

    // output:
    // - avg quaternion

    // what we can do:
    // - OuterProduct, where q*qT --> M4x4
    // - Sum of outerproduct to make Matrix4x4
    // - convert Quaternion into float[4]

    // what we need from cpp:
    // - calculation of Eigenvalue
    // - calculation of Eigenvector
    // - return to float[4] --> cpp cannot return array
    // - must use a long char?

    /// <summary>
    /// OuterProduct of two quaternions, where q1 * q2^T.
    /// </summary>
    /// <param name="q1">Left quaternion</param>
    /// <param name="q2">Right quaternion</param>
    /// <returns>Matrix4x4 of outerproduct</returns>
    Matrix4x4 OuterProduct(Quaternion q1, Quaternion q2)
    {
        // the order: x,y,z,w
        // the direction is vertical
        Vector4 v1 = new(q1.x * q2.x, q1.y * q2.x, q1.z * q2.x, q1.w * q2.x);
        Vector4 v2 = new(q1.x * q2.y, q1.y * q2.y, q1.z * q2.y, q1.w * q2.y);
        Vector4 v3 = new(q1.x * q2.z, q1.y * q2.z, q1.z * q2.z, q1.w * q2.z);
        Vector4 v4 = new(q1.x * q2.w, q1.y * q2.w, q1.z * q2.w, q1.w * q2.w);

        return new Matrix4x4(v1, v2, v3, v4);
    }

    /// <summary>
    /// Sum of two Matrix4x4, arithmetic sum.
    /// </summary>
    /// <param name="m1">Left matrix</param>
    /// <param name="m2">Right matrix</param>
    /// <returns>Matrix4x4 of sum</returns>
    Matrix4x4 SumOfMatrix4x4(Matrix4x4 m1, Matrix4x4 m2)
    {
        return new Matrix4x4(m1.GetRow(0) + m2.GetRow(0),
                             m1.GetRow(1) + m2.GetRow(1),
                             m1.GetRow(2) + m2.GetRow(2),
                             m1.GetRow(3) + m2.GetRow(3));
    }

    Matrix4x4Units ConvertToMatrix4x4Units(Matrix4x4 m)
    {
        float[] v0 = { m.m00, m.m10, m.m20, m.m30 };
        float[] v1 = { m.m01, m.m11, m.m21, m.m31 };
        float[] v2 = { m.m02, m.m12, m.m22, m.m32 };
        float[] v3 = { m.m03, m.m13, m.m23, m.m33 };

        Matrix4x4Units new_m = new();
        new_m.v0 = v0;
        new_m.v1 = v1;
        new_m.v2 = v2;
        new_m.v3 = v3;

        return new_m;
    }

    float[] QuaternionToFloats(Quaternion q)
    {
        float[] result = new float[4];
        result[0] = q.x;
        result[1] = q.y;
        result[2] = q.z;
        result[3] = q.w;
        return result;
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
