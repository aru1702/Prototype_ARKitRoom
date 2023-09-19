using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MathFunction
{
    public class BasicOperation
    {

        /// <summary>
        /// This is not Matrix4x4 multiplication adder, instead it will
        /// add each component single to single value. For example, the
        /// value on m00 will be A(m00) + B(m00)
        /// </summary>
        /// <param name="left">left matrix</param>
        /// <param name="right">right matrix</param>
        /// <returns></returns>
        public static Matrix4x4 M44Adder(Matrix4x4 left, Matrix4x4 right)
        {
            var col_0 = left.GetColumn(0) + right.GetColumn(0);
            var col_1 = left.GetColumn(1) + right.GetColumn(1);
            var col_2 = left.GetColumn(2) + right.GetColumn(2);
            var col_3 = left.GetColumn(3) + right.GetColumn(3);

            return new(col_0, col_1, col_2, col_3);
        }

        /// <summary>
        /// Divide each component by float division, divisor must not zero.
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="div"></param>
        /// <returns></returns>
        public static Matrix4x4 M44DotDivision(Matrix4x4 mat, float div)
        {
            if (div == 0)
            {
                throw new System.Exception("Cannot divide by zero.");
            }

            return new (mat.GetColumn(0) / div,
                        mat.GetColumn(1) / div,
                        mat.GetColumn(2) / div,
                        mat.GetColumn(3) / div);
        }
    }
}
