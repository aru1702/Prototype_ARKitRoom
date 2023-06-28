using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeightFunction
{
    public class MarkerRotation
    {
        GameObject m_Camera;
        GameObject m_Root;
        List<MarkerLocation> m_Markers;
        Vector3 m_CurrentMarker;
        float m_Weight;

        /// <summary>
        /// Main function.
        /// </summary>
        public void MarkerRotationStart()
        {
            // create a gameobject based on current marker position
            var temp_gameobject = new GameObject("temp_gameobject");
            if (m_CurrentMarker == null) FindCurrentMarker(GetCameraPosition());
            temp_gameobject.transform.position = m_CurrentMarker;

            // put root as child of temp_gameobject
            SetRootParent(temp_gameobject);

            // rotate temp_gameobject based on weighted average
            RotateGameObjectWithEigenMethod(temp_gameobject);

            // release root from temp_gameobject
            ReleaseRoot();
            Object.Destroy(temp_gameobject);

            // remove current marker
            ResetCurrentMarker();
        }

        /// <summary>
        /// Weight function from single object to each marker
        /// </summary>
        /// <param name="object_position">Object location in Vector3</param>
        /// <param name="weight_function">"sigmoid" or "tanh".</param>
        /// <param name="inverted">Invert the weight function result, default is true.</param>
        /// <param name="normalized">Normalize for each result, default is true.</param>
        /// <param name="a">Scalar multiplier, default is 1.0f.</param>
        /// <returns></returns>
        List<float> GetSingleWeight(Vector3 camera_position,
                                    string weight_function,
                                    bool inverted = true,
                                    bool normalized = true,
                                    float a = 1.0f)
        {
            List<float> weights = new();

            for (int i = 0; i < m_Markers.Count; i++)
            {
                // calculate object-to-marker distance
                var distance = MathFunctions.Distance(camera_position, m_Markers[i].GT_Position, a);

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
        /// Alternative function to find current marker by the greatest weight in position to camera
        /// </summary>
        /// <returns>Position of current marker in Vector3.</returns>
        void FindCurrentMarker(Vector3 camera_position,
                                    string weight_function = "sigmoid",
                                    bool inverted = true,
                                    bool normalized = true,
                                    float a = 1.0f)
        {
            List<float> weights = new();
            for (int i = 0; i < m_Markers.Count; i++)
            {
                // calculate object-to-marker distance
                var distance = MathFunctions.Distance(camera_position, m_Markers[i].GT_Position, a);

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

            float max = 0; Vector3 pos = new Vector3();
            for (int i = 0; i < weights.Count; i++)
            {
                if (weights[i] > max) pos = GlobalConfig.ExtractVector3(m_Markers[i].GT_Position);
            }

            m_CurrentMarker = pos;
        }

        /// <summary>
        /// Find the rotation difference between two quaternions.
        /// Formula is inverse(from) * to.
        /// </summary>
        /// <param name="from">First quaternion, from where.</param>
        /// <param name="to">Second quaternion, to where.</param>
        /// <returns>Difference of two quaternion</returns>
        Quaternion RotationDifference(Quaternion from, Quaternion to)
        {
            return Quaternion.Inverse(from) * to;
        }

        /// <summary>
        /// Main function to rotate the world root by certain gameobject as its parent.
        /// </summary>
        /// <param name="gameObject">The desire gameObject assigned as temporary parent for root.</param>
        void RotateGameObjectWithEigenMethod(GameObject gameObject)
        {
            // get weight list
            List<float> weights = GetSingleWeight(GetCameraPosition(), MathFunctions.SIGMOID, true, true, m_Weight);

            // find each differences of quaternion
            List<EigenMacHelper.QuaternionWeighted> qws = new List<EigenMacHelper.QuaternionWeighted>();
            for (int i = 0; i < m_Markers.Count; i++)
            {
                Quaternion q_diff = RotationDifference(m_Markers[i].GT_Rotation, m_Markers[i].C_Rotation);
                EigenMacHelper.QuaternionWeighted qw = new EigenMacHelper.QuaternionWeighted(q_diff, weights[i]);
                qws.Add(qw);

                string data = "";
                data += "GT: " + m_Markers[i].GT_Rotation.eulerAngles.ToString() + ",  ";
                data += "RT: " + m_Markers[i].C_Rotation.eulerAngles.ToString() + ",  ";
                data += "diff: " + q_diff.eulerAngles.ToString() + ",  ";
                Debugging(m_Markers[i].Marker_name, data);
            }

            // use Eigen method to find weighted average rotation
            Quaternion w_avg_rot = EigenMacHelper.EigenWeightedAvgMultiRotations(qws.ToArray());

            // rotate the gameObject base on this w_avg_rot
            gameObject.transform.rotation = Quaternion.identity * w_avg_rot;

            string data2 = "";
            data2 += "w_avg_rot: " + w_avg_rot.eulerAngles.ToString() + ",  ";
            data2 += "base_rot: " + Quaternion.identity.eulerAngles.ToString() + ",  ";
            data2 += "now_rot: " + gameObject.transform.rotation.eulerAngles.ToString() + ",  ";
            data2 += "root_rot: " + m_Root.transform.rotation.eulerAngles.ToString() + ",  ";
            Debugging("result", data2);
        }


        public void SetCamera(GameObject camera) { m_Camera = camera; }

        public GameObject GetCamera() { return m_Camera; }

        public Vector3 GetCameraPosition() { return GetCamera().transform.position; }


        public void SetRoot(GameObject root) { m_Root = root; }

        public GameObject GetRoot() { return m_Root; }

        public GameObject GetRootParent() { return m_Root.transform.parent.gameObject; }

        public void SetRootParent(GameObject parent) { m_Root.transform.parent = parent.transform; }

        public void ReleaseRoot() { m_Root.transform.parent = null; }

        public void ResetRootRotationToInitial(Quaternion rotation) { m_Root.transform.rotation = rotation; }


        public void SetCurrentMarker(Vector3 marker) { m_CurrentMarker = marker; }

        public void ResetCurrentMarker() { m_CurrentMarker = new Vector3(); }


        public void SetMRWeight(float w) { m_Weight = w; }


        public void SetMarkers(List<MarkerLocation> markers) { m_Markers = markers; }

        public void SetMarkersAsNew(List<MarkerLocation> markers)
        {
            m_Markers = new();
            foreach (var m in markers) { m_Markers.Add(m); }
        }

        public List<MarkerLocation> GetMarkers() { return m_Markers; }


        void Debugging(string context, string data) { Debug.Log(context + ": " + data); }
    }
}
