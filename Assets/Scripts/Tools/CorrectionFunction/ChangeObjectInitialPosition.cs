using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The purpose of this script is to get all ground truth object location
/// when there is certain condition that change them. For example: the change
/// of world coordinate position, rotation, etc.
///
/// Feb 5th, 2023:
/// - We use this function to create dummy position of all object
/// - Rotate in certain position with certain rotation (Q)
/// - Get the alternate position
/// - Save as new ground position
/// </summary>
public class ChangeObjectInitialPosition
{
    /// <summary>
    /// Change ground truth position when there is certain rotation.
    /// Convert the game objects position to list of vector3 first.
    /// 
    /// <b>Make sure that the game objects position also based on
    /// Unity world coordinate system</b>, not by user-defined coordinate system.
    /// </summary>
    /// <param name="object_positions">Provided object position in list of vector3.</param>
    /// <param name="rotation">Rotation in quaternion.</param>
    /// <param name="rotation_center">Rotation center.</param>
    /// <returns>New list of vector3</returns>
    public static List<Vector3> ChangeByRotation(List<Vector3> object_positions,
                                                 Quaternion rotation,
                                                 Vector3 rotation_center)
    {
        List<Vector3> results = new();
        List<GameObject> gameObjects = new();

        // by default it will put in root of Unity world coordinate system
        GameObject center = new();
        center.name = "center";
        center.transform.position = rotation_center;

        // by Unity documentation, creating empty gameobject will not
        // cause too many memory allocation, unless we define some render data
        for (int i = 0; i < object_positions.Count; i++)
        {
            GameObject gameObject = new();
            gameObject.name = "gameObject_" + i;
            gameObject.transform.position = object_positions[i];
            gameObject.transform.SetParent(center.transform);
            gameObjects.Add(gameObject);
        }

        // rotate the center
        center.transform.rotation = rotation;

        // retrieve back the list of game object
        foreach (var go in gameObjects)
        {
            results.Add(go.transform.position);
            Object.Destroy(go);
        }

        Object.Destroy(center);

        return results;
    }
}
