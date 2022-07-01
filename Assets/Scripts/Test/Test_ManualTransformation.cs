using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_ManualTransformation : MonoBehaviour
{
    [SerializeField]
    GameObject m_DummyCamera, m_CubeObject;

    [SerializeField]
    Text m_DebugText;

    GameObject newGO;

    // Start is called before the first frame update
    void Start()
    {
        Matrix4x4 cubeToWorld = m_CubeObject.transform.worldToLocalMatrix;
        Matrix4x4 worldToCamera = m_DummyCamera.transform.localToWorldMatrix;
        Vector3 cubeLocalPosition = m_CubeObject.transform.localPosition;

        Vector3 cubeInWorld_Position = cubeToWorld * cubeLocalPosition;
        Vector3 cubeInCamera_Position = worldToCamera * cubeInWorld_Position;

        newGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newGO.name = "newGO";
        newGO.transform.position = cubeInCamera_Position;

        Matrix4x4 cubeToCameraT4 = m_DummyCamera.transform.worldToLocalMatrix *
            m_CubeObject.transform.localToWorldMatrix;

        DebugLog(cubeToWorld.ToString(),
            worldToCamera.ToString(),
            cubeLocalPosition.ToString(),
            cubeInWorld_Position.ToString(),
            cubeInCamera_Position.ToString(),
            cubeToCameraT4.ToString()
            );
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 newPos = m_DummyCamera.transform.localToWorldMatrix *
        //    m_CubeObject.transform.worldToLocalMatrix *
        //    m_CubeObject.transform.localPosition;

        Matrix4x4 cubeToCameraT4 = m_DummyCamera.transform.worldToLocalMatrix *
            m_CubeObject.transform.localToWorldMatrix;

        //Vector3 cubePos_v3 = m_CubeObject.transform.position;
        //Vector4 cubePos_v4 = new(cubePos_v3.x, cubePos_v3.y, cubePos_v3.z, 1);

        //Vector4 new_cubePos_v4 = cubeToCameraT4 * cubePos_v4;

        //newGO.transform.position = new(new_cubePos_v4.x, new_cubePos_v4.y, new_cubePos_v4.z);

        Vector3 newPos = cubeToCameraT4.GetPosition();
        //newGO.transform.position = newPos;

        m_DebugText.text = string.Format("{0}\n{1}",
            cubeToCameraT4,
            newGO.transform.position);
    }

    void DebugLog(params string[] debugtext)
    {
        for (int i = 0; i < debugtext.Length; i++)
        {
            Debug.Log(debugtext[i]);
        }
    }
}
