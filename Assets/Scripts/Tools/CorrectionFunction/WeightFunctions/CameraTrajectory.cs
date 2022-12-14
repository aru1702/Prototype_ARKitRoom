using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeightFunction
{
    public class CameraTrajectory
    {
        float m_CameraDistance;
        Vector3 m_CameraPrev, m_CameraNow;
        List<CustomTransform> m_Markers;
        List<float> m_Distances;

        /// <summary>
        /// Update camera distance in current runtime.
        /// </summary>
        /// <param name="alreadySetCameraNow">Please only set this true if already assign using SetCameraDistance(float)</param>
        /// <param name="camera_now">Additional input if current camera location not assigned yet, default is (0,0,0)</param>
        /// <param name="a">Scalar multiplier, default is 1.0f.</param>
        public void UpdateCameraDistance(bool alreadySetCameraNow,
                                         Vector3 camera_now = new(),
                                         float a = 1.0f)
        {
            if (!alreadySetCameraNow) { m_CameraNow = camera_now; }

            m_CameraDistance = Distance(m_CameraNow, m_CameraNow, a);
            m_CameraPrev = m_CameraNow;
        }

        /// <summary>
        /// Reset camera distance field variable to zero.
        /// </summary>
        void ResetCameraDistance() { m_CameraDistance = 0; }

        /// <summary>
        /// Add new marker distance respectively to marker list.
        /// Update marker distance respectively to marker list.
        /// </summary>
        /// <param name="marker">Marker input corresponding to detected marker.</param>
        public void AddOrUpdateDistances(CustomTransform marker)
        {
            // add new marker distance
            if (m_Markers.Count > m_Distances.Count)
            {
                m_Distances.Add(m_CameraDistance);
                ResetCameraDistance();
                return;
            }

            // update current marker distance
            for (int i = 0; i < m_Markers.Count; i++)
            {
                if(Equals(m_Markers[i].custom_name, marker.custom_name))
                {
                    m_Distances[i] = m_CameraDistance;
                    ResetCameraDistance();
                    return;
                }
            }
        }

        /// <summary>
        /// Weight function based on camera trajectory distance record on each marker.
        /// </summary>
        /// <param name="weight_function">"sigmoid" or "tanh".</param>
        /// <param name="inverted">Invert the weight function result, default is false.</param>
        /// <param name="normalized">Normalize for each result, default is true.</param>
        /// <returns></returns>
        public List<float> GetWeight(string weight_function,
                                     bool inverted = false,
                                     bool normalized = true)
        {
            List<float> weights = new();

            for (int i = 0; i < m_Markers.Count; i++)
            {
                // calculate object-to-marker distance
                var distance = m_Distances[i];

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

        public void SetCameraDistance(float camera_distance) { m_CameraDistance = camera_distance; }

        public float GetCameraDistance() { return m_CameraDistance; }

        public void SetCameraPrev(Vector3 camera_prev) { m_CameraPrev = camera_prev; }

        public Vector3 GetCameraPrev() { return m_CameraPrev; }

        public void SetCameraNow(Vector3 camera_now) { m_CameraNow = camera_now; }

        public Vector3 GetCameraNow() { return m_CameraNow; }

        public void SetMarkers(List<CustomTransform> markers) { m_Markers = markers; }

        public void SetMarkersAsNew(List<CustomTransform> markers)
        {
            m_Markers = new();
            foreach (var m in markers) { m_Markers.Add(m); }
        }

        public List<CustomTransform> GetMarkers() { return m_Markers; }

        public void SetDistances(List<float> distances) { m_Distances = distances; }

        public void SetDistancesAsNew(List<float> distances)
        {
            m_Distances = new();
            foreach (var d in distances) { m_Distances.Add(d); }
        }

        public List<float> GetDistances() { return m_Distances; }

        float Distance(Vector3 a, Vector3 b, float scalar)
        {
            var distance = Vector3.Distance(a, b);
            return Mathf.Abs(scalar * distance);
        }        
    }
}
