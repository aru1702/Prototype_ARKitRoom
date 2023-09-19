using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is called by RaycastManager
/// Unlike the other one, is not bounded by prefab
/// </summary>
public class CrossLikeText_Trails : MonoBehaviour
{
    List<GameObject> trails = new List<GameObject>();
    Vector3 thisObjPos, targetObjPos;
    const float DEFAULT_DIS = 0.01f;
    bool hasCreate = false;
    GameObject m_TargetedGameObject;

    void Update()
    {
        // create only for the first time
        if (!hasCreate) CreateTrails();

        // update trails
        UpdateTrails();
    }

    void CreateTrails()
    {
        // get current obj pos
        thisObjPos = this.transform.position;

        // check if targeted GameObject exist, get the position
        if (!m_TargetedGameObject) return;
        targetObjPos = m_TargetedGameObject.transform.position;

        // get float distance from two GameObject
        float distance = Vector3.Distance(thisObjPos, targetObjPos);
        float perc = DEFAULT_DIS / distance;
        int perc_int = (int)(distance / DEFAULT_DIS);

        //Debug.Log(distance);
        //Debug.Log(perc);
        //Debug.Log(perc_int);

        for (int i = 0; i < perc_int; i++)
        {
            float current_perc = i * perc;
            if (current_perc > 1.00f) break;

            // get position of current point
            Vector3 point = Vector3.Lerp(thisObjPos, targetObjPos, current_perc);

            //Debug.Log(point);

            // create trail here
            BuildTrail(point, i);
        }

        hasCreate = true;
    }

    void UpdateTrails()
    {
        // check if object is moving
        float distance = Vector3.Distance(thisObjPos, this.transform.position);

        // check if distance != zero
        if (distance == 0.00f) return;

        if (distance > DEFAULT_DIS)
        {
            DestroyTrails();
            CreateTrails();
        }
    }

    void DestroyTrails()
    {
        if (trails.Count > 0)
        {
            foreach (var go in trails)
            {
                Destroy(go);
            }
            trails.Clear();
        }

        hasCreate = false;
    }

    void BuildTrail(Vector3 pos, int num)
    {
        float trail_size = DEFAULT_DIS / 2;

        GameObject trail = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //trail.transform.SetParent(this.GetComponent<Transform>());
        trail.name = "_trail_" + num;
        trail.transform.position = pos;
        trail.transform.localScale = new Vector3(trail_size, trail_size, trail_size);
        //trail.GetComponent<Renderer>().material.color = Color.green;
        trails.Add(trail);
    }

    public void SetTargetedGameObject(GameObject targetObject)
    {
        m_TargetedGameObject = targetObject;
    }
}
