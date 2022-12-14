using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeightFunction
{
    public class ObjectToMarkers
    {
        List<GameObject> m_Objects;
        List<CustomTransform> m_Markers;

        /// <summary>
        /// Weight function from single object to each marker
        /// </summary>
        /// <param name="object_position">Object location in Vector3</param>
        /// <param name="weight_function">"sigmoid" or "tanh".</param>
        /// <param name="inverted">Invert the weight function result, default is true.</param>
        /// <param name="normalized">Normalize for each result, default is true.</param>
        /// <param name="a">Scalar multiplier, default is 1.0f.</param>
        /// <returns></returns>
        public List<float> GetSingleWeight(Vector3 object_position,
                                           string weight_function,
                                           bool inverted = true,
                                           bool normalized = true,
                                           float a = 1.0f)
        {
            List<float> weights = new();

            for (int i = 0; i < m_Markers.Count; i++)
            {
                // calculate object-to-marker distance
                var distance = Distance(object_position, m_Markers[i].custom_position, a);

                // get weight
                float w = 0;
                if (weight_function == MathFunctions.SIGMOID)
                {
                    w = MathFunctions.Sigmoid(distance, inverted);
                }
                else if (weight_function == MathFunctions.TANH)
                {
                    w = MathFunctions.Tanh(distance, inverted);
                }
                else
                {
                    Debug.LogError("Unrecognized weight function input!");
                }

                weights.Add(w);
            }

            if (normalized)
            {
                weights = MathFunctions.NormalizedMany(weights);
            }

            return weights;
        }

        /// <summary>
        /// Weight function for all objects to each marker.
        /// </summary>
        /// <param name="weight_function">"sigmoid" or "tanh".</param>
        /// <param name="inverted">Invert the weight function result, default is true.</param>
        /// <param name="normalized">Normalize for each result, default is true.</param>
        /// <param name="a">Scalar multiplier, default is 1.0f.</param>
        /// <returns></returns>
        public List<float[]> GetAllWeights(string weight_function,
                                           bool inverted = true,
                                           bool normalized = true,
                                           float a = 1.0f)
        {
            List<float[]> weights = new();

            for (int i = 0; i < m_Objects.Count; i++)
            {
                var temp_w = GetSingleWeight(m_Objects[i].transform.position,
                                             weight_function,
                                             inverted,
                                             normalized,
                                             a);
                float[] w = new float[temp_w.Count];
                for (int j = 0; j < temp_w.Count; j++) { w[j] = temp_w[j]; }
                weights.Add(w);
            }

            return weights;
        }

        public void SetObjects(List<GameObject> objects) { m_Objects = objects; }

        public void SetObjectsAsNew(List<GameObject> objects)
        {
            m_Objects = new();
            foreach (var o in objects) { m_Objects.Add(o); }
        }

        public List<GameObject> GetObjects() { return m_Objects; }

        public void SetMarkers(List<CustomTransform> markers) { m_Markers = markers; }

        public void SetMarkersAsNew(List<CustomTransform> markers)
        {
            m_Markers = new();
            foreach (var m in markers) { m_Markers.Add(m); }
        }

        public List<CustomTransform> GetMarkers() { return m_Markers; }

        float Distance(Vector3 a, Vector3 b, float scalar)
        {
            var distance = Vector3.Distance(a, b);
            return Mathf.Abs(scalar * distance);
        }        
    }
}
