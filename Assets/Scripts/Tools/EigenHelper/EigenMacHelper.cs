using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class EigenMacHelper
{
    /// <summary>
    /// Struct class data for import-export, QuaternionUnits consists of 4 floats.
    /// </summary>
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
            return x.ToString() + "i\t" +
                y.ToString() + "j\t" +
                z.ToString() + "k\t" +
                w.ToString();
        }
    }

    /// <summary>
    /// Struct class data for import-export, Matrix4x4Units consists of 4 float arrays.
    /// </summary>
    public unsafe struct Matrix4x4Units
    {
        public float[] v0;
        public float[] v1;
        public float[] v2;
        public float[] v3;
    }

    /// <summary>
    /// Struct class data for import-export, Matrix4x4UnitsFloats consists of 16 floats.
    /// </summary>
    public unsafe struct Matrix4x4UnitsFloats
    {
        public float[] m;
    }

    // Import Eigen from Dylib file (only MacOS)
    // currently in latest build dylib, we use libEigenToUnityForMac03
    // import file name can be changed depends on prefered build

    [DllImport("libEigenToUnityForMac03")]
    public static extern QuaternionUnits GetAvgQuaternionsFloats
        (float m00, float m01, float m02, float m03,
        float m10, float m11, float m12, float m13,
        float m20, float m21, float m22, float m23,
        float m30, float m31, float m32, float m33);

    /// <summary>
    /// Convert from Matrix4x4 unity format to Matrix4x4Units struct.
    /// </summary>
    /// <returns>Matrix4x4Units format</returns>
    static Matrix4x4Units Matrix4x4ToMatrix4x4Units(Matrix4x4 m)
    {
        float[] v0 = { m.m00, m.m10, m.m20, m.m30 };
        float[] v1 = { m.m01, m.m11, m.m21, m.m31 };
        float[] v2 = { m.m02, m.m12, m.m22, m.m32 };
        float[] v3 = { m.m03, m.m13, m.m23, m.m33 };

        return new Matrix4x4Units
        {
            v0 = v0,
            v1 = v1,
            v2 = v2,
            v3 = v3
        };
    }

    /// <summary>
    /// Convert from Matrix4x4 unity format to Matrix4x4UnitsFloats with 16 array size float
    /// </summary>
    /// <returns>Matrix4x4UnitsFloats format</returns>
    static Matrix4x4UnitsFloats Matrix4x4ToMatrix4x4UnitsFloats(Matrix4x4 m)
    {
        float[] new_m = { m.m00, m.m01, m.m02, m.m03,
                        m.m10, m.m11, m.m12, m.m13,
                        m.m20, m.m21, m.m22, m.m23,
                        m.m30, m.m31, m.m32, m.m33};

        return new Matrix4x4UnitsFloats { m = new_m };
    }

    /// <summary>
    /// Convert QuaternionUnits to unity Quaternion
    /// </summary>
    static Quaternion QuaternionUnitsToQuaternion(QuaternionUnits q)
    {
        return new Quaternion(q.x, q.y, q.z, q.w);
    }

    /// <summary>
    /// Outer product of two Quaternion (cross product of Vector4), q1 * q2^T
    /// </summary>
    /// <returns>Matrix4x4 result of Vector4 cross product</returns>
    static Matrix4x4 QuaternionOuterProduct(Quaternion q1, Quaternion q2)
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
    /// Sum of two Matrix4x4, a simple arithmetic matrix sum
    /// </summary>
    static Matrix4x4 Matrix4x4AddSum(Matrix4x4 m1, Matrix4x4 m2)
    {
        return new Matrix4x4(m1.GetRow(0) + m2.GetRow(0),
                             m1.GetRow(1) + m2.GetRow(1),
                             m1.GetRow(2) + m2.GetRow(2),
                             m1.GetRow(3) + m2.GetRow(3));
    }

    // <summary>
    /// Sum of dot product between Matrix4x4 and a scalar float.
    /// </summary>
    static Matrix4x4 Matrix4x4ScalarDotProduct(Matrix4x4 m, float a)
    {
        return new Matrix4x4(m.GetRow(0) * a,
                             m.GetRow(1) * a,
                             m.GetRow(2) * a,
                             m.GetRow(3) * a);
    }

    /// <summary>
    /// Get the average of two rotations in Quaternion based on Eigen method.
    /// Given two Quaternions, returns the average rotation in Quaternion.
    /// </summary>
    /// <param name="q1">First quaternion</param>
    /// <param name="q2">Second quaternion</param>
    /// <returns>Average rotation in Quaternion</returns>
    public static Quaternion EigenAvgTwoRotations(Quaternion q1, Quaternion q2)
    {
        Matrix4x4 m = QuaternionOuterProduct(q1, q1);
        m = Matrix4x4AddSum(m, QuaternionOuterProduct(q2, q2));
   
        QuaternionUnits q_units = GetAvgQuaternionsFloats(m.m00, m.m01, m.m02, m.m03,
                                                          m.m10, m.m11, m.m12, m.m13,
                                                          m.m20, m.m21, m.m22, m.m23,
                                                          m.m30, m.m31, m.m32, m.m33);
   
        return QuaternionUnitsToQuaternion(q_units);
    }

    /// <summary>
    /// Get the average of multiple rotations in Quaternion based on Eigen method.
    /// Given the number of Quaternions, returns the average rotation in Quaternion.
    /// </summary>
    /// <param name="qs">Given data of Quaternion</param>
    /// <returns>Average rotation in Quaternion</returns>
    public static Quaternion EigenAvgRotations(params Quaternion[] qs)
    {
        if (qs.Length <= 0) return new Quaternion();

        Matrix4x4 m = Matrix4x4.zero;

        foreach (var q in qs)
        {
            m = Matrix4x4AddSum(m, QuaternionOuterProduct(q, q));
        }

        QuaternionUnits q_units = GetAvgQuaternionsFloats(m.m00, m.m01, m.m02, m.m03,
                                                          m.m10, m.m11, m.m12, m.m13,
                                                          m.m20, m.m21, m.m22, m.m23,
                                                          m.m30, m.m31, m.m32, m.m33);

        return QuaternionUnitsToQuaternion(q_units);
    }

    /// <summary>
    /// Get the average of two rotations in Quaternion based with weighted based on Eigen method.
    /// Given two Quaternions and their respective weight value, returns the average rotation in Quaternion.
    /// </summary>
    /// <param name="q1">First quaternion</param>
    /// <param name="a1">Weight of first quaternion</param>
    /// <param name="q2">Second quaternion</param>
    /// <param name="a2">Weight of second quaternion</param>
    /// <returns>Average rotation in Quaternion</returns>
    public static Quaternion EigenWeightedAvgTwoRotations(Quaternion q1, float a1, Quaternion q2, float a2)
    {
        Matrix4x4 m = Matrix4x4ScalarDotProduct(QuaternionOuterProduct(q1, q1), a1);
        m = Matrix4x4AddSum(m, Matrix4x4ScalarDotProduct(QuaternionOuterProduct(q2, q2), a2));
   
        QuaternionUnits q_units = GetAvgQuaternionsFloats(m.m00, m.m01, m.m02, m.m03,
                                                          m.m10, m.m11, m.m12, m.m13,
                                                          m.m20, m.m21, m.m22, m.m23,
                                                          m.m30, m.m31, m.m32, m.m33);
   
        return QuaternionUnitsToQuaternion(q_units);
    }

    /// <summary>
    /// Get the average of multiple rotations in Quaternion based with weighted based on Eigen method.
    /// Given the number of Quaternions and their respective weight value, returns the average rotation in Quaternion.
    /// Each params must be a class of QuaternionWeighted, consist of both Quaternion and weight.
    /// </summary>
    /// <param name="qws">Given data of QuaternionWeighted</param>
    /// <returns>Average rotation in Quaternion</returns>
    public static Quaternion EigenWeightedAvgMultiRotations(params QuaternionWeighted[] qws)
    {
        Matrix4x4 m = Matrix4x4.zero;

        foreach (var qw in qws)
        {
            m = Matrix4x4AddSum(m, Matrix4x4ScalarDotProduct(QuaternionOuterProduct(qw.Rotation, qw.Rotation), qw.Weight));
        }

        QuaternionUnits q_units = GetAvgQuaternionsFloats(m.m00, m.m01, m.m02, m.m03,
                                                          m.m10, m.m11, m.m12, m.m13,
                                                          m.m20, m.m21, m.m22, m.m23,
                                                          m.m30, m.m31, m.m32, m.m33);

        return QuaternionUnitsToQuaternion(q_units);
    }

    /// <summary>
    /// Class of QuaternionWeighted, consist of Quaternion and float weight.
    /// </summary>
    public class QuaternionWeighted
    {

        public Quaternion Rotation { get; set; }
        public float Weight { get; set; }

        /// <summary>
        /// Assign new QuaternionWeighted class with desire data of Quaternion and weight.
        /// </summary>
        public QuaternionWeighted(Quaternion q, float w)
        {
            Rotation = q;
            Weight = w;
        }

        /// <summary>
        /// Assign default QuaternionWeighted, identity Quaternion with 1.0f weight. 
        /// </summary>
        public QuaternionWeighted()
        {
            Rotation = new Quaternion();
            Weight = 1.0f;
        }
    }
}
