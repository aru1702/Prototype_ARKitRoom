using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPrefab_CatExample : MonoBehaviour
{
    [SerializeField]
    List<GameObject> prefabs_collection;

    public GameObject GetPrefab(int index)
    {
        return prefabs_collection[index];
    }

    public int GetCollectionCount()
    {
        return prefabs_collection.Count;
    }

    public void AddCollection(GameObject gameObject)
    {
        prefabs_collection.Add(gameObject);
    }
}
