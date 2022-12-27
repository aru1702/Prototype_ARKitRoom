using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WeightFunction
{
    public class CameraTrajectoryAndTime
    {
        List<CustomTransform> m_Markers;
        List<CameraTrajectoryData> m_CameraTrajectoryData;

        float m_CameraDistance;
        Vector3 m_CameraPrev, m_CameraNow;

        float m_TotalTime;
        float m_LastTime, m_CurrentTime;

        string m_PrevMarkerName, m_NowMarkerName = "na";

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

            m_CameraDistance += MathFunctions.Distance(m_CameraNow, m_CameraNow, a);
            m_CameraPrev = m_CameraNow;
        }

        /// <summary>
        /// Reset camera distance field variable to zero.
        /// </summary>
        public void ResetCameraDistance() { m_CameraDistance = 0; }

        /// <summary>
        /// Update total time in current runtime.
        /// </summary>
        /// <param name="already_set_current_time">Only set this true if m_CurrentTime has assigned.</param>
        /// <param name="current_time">Passed parameter of current time.</param>
        public void UpdateTotalTime(bool already_set_current_time,
                                    float current_time = 0)
        {
            if (!already_set_current_time) { m_CurrentTime = current_time; }

            m_TotalTime += Mathf.Abs(m_CurrentTime - m_LastTime);
            m_LastTime = m_CurrentTime;
        }

        /// <summary>
        /// Automatically update total time with current stored values.
        /// </summary>
        public void AutomaticUpdateTotalTime()
        {
            var current_time = Time.time;
            UpdateTotalTime(false, current_time);
        }

        /// <summary>
        /// Reset total time field variable to zero.
        /// </summary>
        public void ResetTotalTime() { m_TotalTime = 0; }

        /// <summary>
        /// A trigger to go for next marker when no marker tracked on runtime.
        /// This function will call ResetCameraDistance, ResetTotalTime,
        /// update previous marker name.
        /// </summary>
        public void GoToNextMarker()
        {
            ResetCameraDistance();
            ResetTotalTime();
            SetNowMarkerName(GetPrevMarkerName());
        }

        /// <summary>
        /// Add new or update exist camera trajectory data list.
        /// To add new data, update m_Markers by using SetMarkers first, the pass the required parameter.
        /// To update previous data, only need to pass the required parameter.
        /// </summary>
        /// <param name="detected_marker"></param>
        public void AddOrUpdateCameraTrajectoryData(CustomTransform detected_marker)
        {
            // add new camera trajectory data
            if (m_CameraTrajectoryData.Count < m_Markers.Count)
            {
                // create new data
                CameraTrajectoryData data = new();
                data.Marker_name = detected_marker.custom_name;
                data.Camera_travel_distance = GetCameraDistance();
                data.Camera_travel_time = GetTotalTime();
                data.Previous_marker = m_PrevMarkerName;
                m_CameraTrajectoryData.Add(data);                
            }

            // update camera trajectory data
            foreach (var ctd in m_CameraTrajectoryData)
            {
                if (Equals(detected_marker.custom_name, ctd.Marker_name))
                {
                    ctd.Camera_travel_distance = GetCameraDistance();
                    ctd.Camera_travel_time = GetTotalTime();
                    ctd.Previous_marker = m_PrevMarkerName;
                }
            }

            m_NowMarkerName = detected_marker.custom_name;
        }

        /// <summary>
        /// Get weights based on camera travel time unit, the greater the more important.
        /// You can adjust the time unit by changing the a parameter.
        /// </summary>
        /// <param name="weight_function">"sigmoid" or "tanh".</param>
        /// <param name="inverted">Invert the weight function result, default is false.</param>
        /// <param name="normalized">Normalize for each result, default is true.</param>
        /// <param name="a">Scalar time unit multiplier, default is 1.0f.</param>
        /// <returns></returns>
        public List<float> GetCameraTimeWeights(string weight_function,
                                                bool inverted = false,
                                                bool normalized = true,
                                                float a = 1.0f)
        {
            List<float> weights = new();

            foreach (var ctd in m_CameraTrajectoryData)
            {
                var adjusted_time = ctd.Camera_travel_time * a;

                // get weight
                float w = 0;
                if (weight_function == MathFunctions.SIGMOID)
                {
                    w = MathFunctions.Sigmoid(adjusted_time, inverted);
                }
                else if (weight_function == MathFunctions.TANH)
                {
                    w = MathFunctions.Tanh(adjusted_time, inverted);
                }
                else
                {
                    Debug.LogError("Unrecognized weight function input!");
                }

                weights.Add(w);
            }

            //GlobalDebugging.DebugLogListFloat(weights, "GetCameraTimeWeights before normalization");

            if (normalized)
            {
                weights = MathFunctions.NormalizedMany(weights);
            }

            //GlobalDebugging.DebugLogListFloat(weights, "GetCameraTimeWeights after normalization");

            return weights;
        }

        /// <summary>
        /// Get weights based on camera travel trajectory distance, the greater the more important.
        /// You can adjust the distance unit by changing the a parameter.
        /// </summary>
        /// <param name="weight_function">"sigmoid" or "tanh".</param>
        /// <param name="inverted">Invert the weight function result, default is false.</param>
        /// <param name="normalized">Normalize for each result, default is true.</param>
        /// <param name="a">Scalar distance unit multiplier, default is 1.0f.</param>
        /// <returns></returns>
        public List<float> GetCameraTrajectoryWeights(string weight_function,
                                                      bool inverted = false,
                                                      bool normalized = true,
                                                      float a = 1.0f)
        {
            List<float> weights = new();

            foreach (var ctd in m_CameraTrajectoryData)
            {
                var adjusted_dist = ctd.Camera_travel_distance * a;

                // get weight
                float w = 0;
                if (weight_function == MathFunctions.SIGMOID)
                {
                    w = MathFunctions.Sigmoid(adjusted_dist, inverted);
                }
                else if (weight_function == MathFunctions.TANH)
                {
                    w = MathFunctions.Tanh(adjusted_dist, inverted);
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
        /// Limiting the time based on average time, then subtract by the lowest value.
        /// </summary>
        public void AverageTimeLimiter()
        {
            // nothing can be compared if less than 2 data
            if (m_CameraTrajectoryData.Count < 2) return;

            // find average
            float avg = 0;
            foreach (var ctd in m_CameraTrajectoryData)
            {
                avg += ctd.Camera_travel_time;
            }
            avg /= m_CameraTrajectoryData.Count;

            // find if there any highest lower value that average
            float highest_low = 0;
            foreach (var ctd in m_CameraTrajectoryData)
            {
                if (ctd.Camera_travel_time <= avg               // if value is below than avg
                    && ctd.Camera_travel_time > highest_low     // and value is above than current limited
                    && ctd.Camera_travel_time > 0)              // and value is greater than zero
                {
                    highest_low = ctd.Camera_travel_time;
                }
            }

            // subtract all value by highest_low value, but [0, high)
            foreach (var ctd in m_CameraTrajectoryData)
            {
                var result = ctd.Camera_travel_time - highest_low;
                ctd.Camera_travel_time = Mathf.Max(0, result);
            }

            // find the highest value on data now
            //float new_time = 0;
            //foreach (var ctd in m_CameraTrajectoryData)
            //{
            //    if (ctd.Camera_travel_time > new_time)
            //    {
            //        new_time = ctd.Camera_travel_time;
            //    }
            //}

            // get new total time subtract with subtract-value
            var new_time = GetTotalTime() - highest_low;

            // set value on TotalTime
            SetTotalTime(new_time);
        }

        public List<float> GetCameraTimeRaw(bool normalized = true,
                                            float a = 1.0f)
        {
            List<float> camera_times = new();
            foreach (var d in m_CameraTrajectoryData) { camera_times.Add(a * d.Camera_travel_time); }

            if (normalized)
            {
                camera_times = MathFunctions.NormalizedMany(camera_times);
            }

            return camera_times;
        }

        public List<float> GetCameraTimeRawAsWeight(string weight_function,
                                                    bool inverted = false,
                                                    bool normalized = true,
                                                    float a = 1.0f)
        {
            List<float> weights = GetCameraTimeRaw(true, a);

            List<float> medianed_w = new();
            var median = MathFunctions.Median(weights);
            for (int i = 0; i < weights.Count; i++)
            {
                // apply median
                weights[i] -= median;
                medianed_w.Add(weights[i]);
            }
            GlobalDebugging.DebugLogListFloat(medianed_w, "medianed time weight");
            GlobalDebugging.DebugLogListFloat(weights, "function time weight BEFORE function");

            for (int i = 0; i < weights.Count; i++)           
            {
                if (weight_function == MathFunctions.SIGMOID)
                {
                    weights[i] = MathFunctions.Sigmoid(weights[i], inverted);
                }
                else if (weight_function == MathFunctions.TANH)
                {
                    weights[i] = MathFunctions.Tanh(weights[i], inverted);
                }
                else
                {
                    Debug.LogError("Unrecognized weight function input!");
                }
            }

            GlobalDebugging.DebugLogListFloat(weights, "function time weight AFTER function");

            //if (normalized)
            //{
            //    weights = MathFunctions.NormalizedMany(weights);
            //}

            return weights;            
        }

        public void SetCameraDistance(float camera_distance) { m_CameraDistance = camera_distance; }

        public float GetCameraDistance() { return m_CameraDistance; }

        public void SetCameraPrev(Vector3 camera_prev) { m_CameraPrev = camera_prev; }

        public Vector3 GetCameraPrev() { return m_CameraPrev; }

        public void SetCameraNow(Vector3 camera_now) { m_CameraNow = camera_now; }

        public Vector3 GetCameraNow() { return m_CameraNow; }

        public void SetTotalTime(float total_time) { m_TotalTime = total_time; }

        public float GetTotalTime() { return m_TotalTime; }

        public void SetLastTime(float last_time) { m_LastTime = last_time; }

        public float GetLastTime() { return m_LastTime; }

        public void SetCurrentTime(float current_time) { m_CurrentTime = current_time; }

        public float GetCurrentTime() { return m_CurrentTime; }

        public void SetPrevMarkerName(string prev_marker_name) { m_PrevMarkerName = prev_marker_name; }

        public string GetPrevMarkerName() { return m_PrevMarkerName; }

        public void SetNowMarkerName(string now_marker_name) { m_NowMarkerName = now_marker_name; }

        public string GetNowMarkerName() { return m_NowMarkerName; }

        public void SetMarkers(List<CustomTransform> markers) { m_Markers = markers; }

        public void SetMarkersAsNew(List<CustomTransform> markers)
        {
            m_Markers = new();
            foreach (var m in markers) { m_Markers.Add(m); }
        }

        public List<CustomTransform> GetMarkers() { return m_Markers; }

        public void SetCameraTrajectoryData(List<CameraTrajectoryData> camera_trajectory_data) { m_CameraTrajectoryData = camera_trajectory_data; }

        public void SetCameraTrajectoryDataAsNew(List<CameraTrajectoryData> camera_trajectory_data)
        {
            m_CameraTrajectoryData = new();
            foreach (var ctd in camera_trajectory_data) { m_CameraTrajectoryData.Add(ctd); }
        }

        public List<CameraTrajectoryData> GetCameraTrajectoryData() { return m_CameraTrajectoryData; }
    }
}
