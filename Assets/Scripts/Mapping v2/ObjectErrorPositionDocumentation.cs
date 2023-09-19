using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectErrorPositionDocumentation
{
    [SerializeField]
    GameObject m_LoadObjectManager;

    List<Transform> m_ObjTransformList = new();
    List<string[]> m_AvgObjTransformData = new();
    List<string[]> m_FullObjTransformData = new();

    public void RecordData()
    {
        if (m_ObjTransformList.Count <= 0) ImportFromObjectManager();


    }

    void ImportFromObjectManager()
    {
        var objList = m_LoadObjectManager
            .GetComponent<LoadObject_CatExample_2>()
            .GetMyObjects();

        var origin = GlobalConfig.PlaySpaceOriginGO;

        GameObject go = new();
        foreach (var obj in objList)
        {
            var m44 = GlobalConfig.GetM44ByGameObjRef(obj, origin);
            var pos = GlobalConfig.GetPositionFromM44(m44);
            var rot = GlobalConfig.GetRotationFromM44(m44);
            go.transform.SetPositionAndRotation(pos, rot);
            m_ObjTransformList.Add(go.transform);
        }
        Object.Destroy(go);
    }
}
