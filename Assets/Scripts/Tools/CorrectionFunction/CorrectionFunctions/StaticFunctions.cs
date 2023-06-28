using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CorrectionFunctions
{
    public class StaticFunctions
    {
        /// <summary>
        /// To calculate each marker ground truth IN RUNTIME and current position as drift.
        /// </summary>
        /// <param name="markers">Marker list.</param>
        /// <returns>Error Vector3 in list.</returns>
        public static List<Vector3> MarkerErrorDifference(List<MarkerLocation> markers)
        {
            List<Vector3> mev = new();

            foreach (var m in markers)
            {
                // C position currently held the target position,
                //  while GT position held the drift position.
                Vector3 ev = m.C_Position - m.GT_Position;
                mev.Add(ev);
            }

            return mev;
        }

        /// <summary>
        /// To convert from MarkerLocation into CustomTransform by extracting the ground truth information.
        /// </summary>
        /// <param name="markers">Marker list.</param>
        /// <returns>Converted marker list.</returns>
        public static List<CustomTransform> ExtractToCustomTransform(List<MarkerLocation> markers)
        {
            List<CustomTransform> ct = new();
            foreach (var m in markers)
            {
                ct.Add(new(
                        m.Marker_name,
                        null,
                        m.GT_Position,
                        m.GT_EulerAngle,
                        m.GT_Rotation
                    ));
            }
            return ct;
        }

        /// <summary>
        /// To process objects into compensated objects by weights and marker error vectors.
        /// </summary>
        /// <param name="objects_locations">Object location in Vector3 list.</param>
        /// <param name="weights">Weight list from which determined.</param>
        /// <param name="marker_error_vectors">Marker error in Vector3 list.</param>
        /// <returns>Compensated Vector3 list.</returns>
        public static List<Vector3> CorrectedVector(List<Vector3> objects_locations,
                                                    List<float[]> weights,
                                                    List<Vector3> marker_error_vectors)
        {
            List<Vector3> new_vectors = new();

            for (int i = 0; i < weights.Count; i++)
            {
                Vector3 temp_v = new();
                for (int j = 0; j < weights[i].Length; j++)
                {
                    temp_v += marker_error_vectors[j] * weights[i][j];
                }
                temp_v += objects_locations[i];
                new_vectors.Add(temp_v);
            }

            return new_vectors;
        }

        /// <summary>
        /// To process objects into compensated objects by weights and marker error vectors.
        /// This is the continuation of previous function where two different weights applied.
        /// </summary>
        /// <param name="objects_locations">Object location in Vector3 list.</param>
        /// <param name="marker_error_vectors">Marker error in Vector3 list.</param>
        /// <param name="weights_args">Weight list arguments from which determined.</param>
        /// <returns>Compensated Vector3 list.</returns>
        public static List<Vector3> CorrectedVector(List<Vector3> objects_locations,
                                                    List<Vector3> marker_error_vectors,
                                                    params WeightsAndScalar[] weights_args)
        {
            List<Vector3> new_vectors = new();

            for (int i = 0; i < objects_locations.Count; i++)
            {
                Vector3 temp_v = new();

                for (int j = 0; j < weights_args.Length; j++)
                {
                    var Weights = weights_args[j].Weights;
                    var Scalar = weights_args[j].Scalars;

                    for (int k = 0; k < Weights[i].Length; k++)
                    {
                        temp_v += Scalar * Weights[i][k] * marker_error_vectors[k];
                    }
                }

                temp_v += objects_locations[i];
                new_vectors.Add(temp_v);
            }

            return new_vectors;           
        }

        /// <summary>
        /// To process objects into compensated objects by weights and marker error vectors.
        /// This is the continuation of previous function where two different weights applied.
        /// </summary>
        /// <param name="objects_locations">Object location in Vector3 list.</param>
        /// <param name="marker_error_vectors">Marker error in Vector3 list.</param>
        /// <param name="weights_args">Weight list arguments from which determined.</param>
        /// <returns>Compensated Vector3 list.</returns>
        public static List<Vector3> CorrectedVectorV2(List<Vector3> objects_locations,
                                                      List<Vector3> marker_error_vectors,
                                                      params WeightsAndScalar[] weights_args)
        {
            var new_weights = WeightsAndScalar.AddMultipleWeightList(weights_args);

            //GlobalDebugging.DebugLogListFloatArray(Test_OnlyAxisObjectGet.AxisForDec192022(new_weights), "Add two weights with AddMultipleWeightList on CorrectedVectorV2");

            var new_vectors = CorrectedVector(objects_locations, new_weights, marker_error_vectors);
            return new_vectors;
        }

        /// <summary>
        /// Search specific MarkerLocation through the list with return boolean.
        /// </summary>
        /// <param name="list">List of MarkerLocations.</param>
        /// <param name="name">Given name of MarkerLocation.</param>
        /// <param name="found">Out of found MarkerLocation, will return new() if not found.</param>
        /// <returns>True if found, false if not.</returns>
        public static bool SearchMarkerLocationOnListByName(List<MarkerLocation> list, string name, out MarkerLocation found)
        {
            foreach (var ml in list)
            {
                if (ml.Marker_name == name)
                {
                    found = ml;
                    return true;
                }
            }

            found = new();
            return false;
        }
    }
}
