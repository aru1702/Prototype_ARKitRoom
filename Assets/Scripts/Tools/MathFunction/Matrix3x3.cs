using UnityEngine;

public class Matrix3x3
{
    public Vector3 column0 { get; set; }
    public Vector3 column1 { get; set; }
    public Vector3 column2 { get; set; }

    public Matrix3x3() { }

    public Matrix3x3(Vector3 column0, Vector3 column1, Vector3 column2)
    {
        this.column0 = column0;
        this.column1 = column1;
        this.column2 = column2;
    }

    public void Transpose()
    {
        Vector3 new_col0 = new(column0.x, column1.x, column2.x);
        Vector3 new_col1 = new(column0.y, column1.y, column2.y);
        Vector3 new_col2 = new(column0.z, column1.z, column2.z);

        column0 = new_col0;
        column1 = new_col1;
        column2 = new_col2;
    }

    public void Negative()
    {
        column0 = -column0;
        column1 = -column1;
        column2 = -column2;
    }

    // See more: https://www.vedantu.com/maths/inverse-of-3-by-3-matrix
    public void Inverse()
    {
        float det = determinant(this) != 0 ?
                determinant(this) :
                throw new System.Exception("Determinant is zero!");

        float m00 = 1 * ((column1.y * column2.z) - (column1.z * column2.y));
        float m10 = -1 * ((column1.x * column2.z) - (column1.z * column2.x));
        float m20 = 1 * ((column1.x * column2.y) - (column1.y * column2.x));

        float m01 = -1 * ((column0.y * column2.z) - (column0.z * column2.y));
        float m11 = 1 * ((column0.x * column2.z) - (column0.z * column2.x));
        float m21 = -1 * ((column0.x * column2.y) - (column0.y * column2.x));

        float m02 = 1 * ((column0.y * column1.z) - (column0.z * column1.y));
        float m12 = -1 * ((column0.x * column1.z) - (column0.z * column1.x));
        float m22 = 1 * ((column0.x * column1.y) - (column0.y * column1.x));

        column0 = new(m00 / det, m10 / det, m20 / det);
        column1 = new(m01 / det, m11 / det, m21 / det);
        column2 = new(m02 / det, m12 / det, m22 / det);

        Transpose();
    }

    float determinant(Matrix3x3 m)
    {
        float a = m.column0.x * m.column1.y * m.column2.z;
        float b = m.column1.x * m.column2.y * m.column0.z;
        float c = m.column2.x * m.column0.y * m.column1.z;

        float d = m.column0.z * m.column1.y * m.column2.x;
        float e = m.column1.z * m.column2.y * m.column0.x;
        float f = m.column2.z * m.column0.y * m.column1.x;

        return a + b + c - d - e - f;
    }

    public Vector3 MultiplyByVector3(Vector3 v)
    {
        float x = column0.x * v.x + column1.x * v.y + column2.x * v.z;
        float y = column0.y * v.x + column1.y * v.y + column2.y * v.z;
        float z = column0.z * v.x + column1.z * v.y + column2.z * v.z;

        return new(x, y, z);
    }

    public Matrix3x3 MultiplyByMatrix3x3(Matrix3x3 m)
    {
        float m00 = column0.x * m.column0.x + column1.x * m.column0.y + column2.x * m.column0.z;
        float m10 = column0.y * m.column0.x + column1.y * m.column0.y + column2.y * m.column0.z;
        float m20 = column0.z * m.column0.x + column1.z * m.column0.y + column2.z * m.column0.z;

        float m01 = column0.x * m.column1.x + column1.x * m.column1.y + column2.x * m.column1.z;
        float m11 = column0.y * m.column1.x + column1.y * m.column1.y + column2.y * m.column1.z;
        float m21 = column0.z * m.column1.x + column1.z * m.column1.y + column2.z * m.column1.z;

        float m02 = column0.x * m.column2.x + column1.x * m.column2.y + column2.x * m.column2.z;
        float m12 = column0.y * m.column2.x + column1.y * m.column2.y + column2.y * m.column2.z;
        float m22 = column0.z * m.column2.x + column1.z * m.column2.y + column2.z * m.column2.z;

        return new(new(m00, m10, m20), new(m01, m11, m21), new(m02, m12, m22));
    }

    public override string ToString()
    {
        return "(" + column0.x + ", " + column1.x + ", " + column2.x + ")\n" +
                "(" + column0.y + ", " + column1.y + ", " + column2.y + ")\n" +
                "(" + column0.z + ", " + column1.z + ", " + column2.z + ")\n";
    }
}
