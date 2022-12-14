using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeightFunction
{
    public class CameraToMarkers
    {
        GameObject m_ARCamera;
        List<CustomTransform> m_Markers;

        /// <summary>
        /// Weight function from camera to each marker in runtime.
        /// </summary>
        /// <param name="weight_function">"sigmoid" or "tanh".</param>
        /// <param name="inverted">Invert the weight function result, default is true.</param>
        /// <param name="normalized">Normalize for each result, default is true.</param>
        /// <param name="a">Scalar multiplier, default is 1.0f.</param>
        /// <returns></returns>
        public List<float> GetWeight(string weight_function,
                                     bool inverted = true,
                                     bool normalized = true, 
                                     float a = 1.0f)
        {
            List<float> weights = new();
            Vector3 camera_pos;

            // check AR camera
            try
            {
                camera_pos = m_ARCamera.transform.position;
            }
            catch (System.Exception)
            {
                Debug.LogError("No AR camera assigned!");
                return null;
            }

            // calculate each weight to camera
            for (int i = 0; i < m_Markers.Count; i++)
            {
                var distance = Vector3.Distance(camera_pos, m_Markers[i].custom_position);
                distance = Mathf.Abs(a * distance);

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

        public void SetARCamera(GameObject arCamera) { m_ARCamera = arCamera; }

        public GameObject GetARCamera() { return m_ARCamera; }

        public void SetMarkers(List<CustomTransform> markers) { m_Markers = markers; }

        public void SetMarkersAsNew(List<CustomTransform> markers)
        {
            m_Markers = new();
            foreach (var m in markers)
            {
                m_Markers.Add(m);
            }
        }

        public List<CustomTransform> GetMarkers() { return m_Markers; }
    }
}
