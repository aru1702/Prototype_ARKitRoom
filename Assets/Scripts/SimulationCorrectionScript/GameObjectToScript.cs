using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectToScript : MonoBehaviour
{
    [SerializeField]
    GameObject m_TempOriginGO;

    [SerializeField]
    GameObject m_Root;


    List<GameObject> Parents = new();
    List<GameObject> Objects = new();
    List<GameObject> ObjectsGroundTruth = new();
    GameObject OriginChild;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<GameObject> GetParents() { return Parents; }
    public List<GameObject> GetObjects() { return Objects; }
    public GameObject GetOriginChild() { return OriginChild; }
    public List<GameObject> GetObjectsGroundTruth() { return ObjectsGroundTruth; }
}
