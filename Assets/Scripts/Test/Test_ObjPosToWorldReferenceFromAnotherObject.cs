using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_ObjPosToWorldReferenceFromAnotherObject : MonoBehaviour
{
    [SerializeField]
    GameObject m_ARCamera, m_VirtualObject;

    [SerializeField]
    Text m_PanelText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // noted that World origin always in (0,0,0) in world space
        // we calculate from world space

        // what we know, assume we only know mat44 from obj to world
        Matrix4x4 m44_ObjToWorld = m_VirtualObject.transform.localToWorldMatrix;
        Matrix4x4 m44_CamToWorld = Matrix4x4.Inverse(m_ARCamera.transform.localToWorldMatrix);

        // by this we can calculate from obj to cam
        Matrix4x4 m44_ObjToCam = m44_CamToWorld * m44_ObjToWorld;

        // then we calculate back from obj to world
        // with new reference from cam
        Matrix4x4 m44_ObjToWorld_fromCam = m44_CamToWorld * m44_ObjToCam;

        // in theory, both "m44_ObjToWorld_fromCam" and "m44_ObjToWorld" should be same
        // now we see the position
        Vector3 v3_ObjPosByWorld = m_VirtualObject.transform.position;
        Vector3 v3_ObjPosByCam = m44_ObjToCam * v3_ObjPosByWorld;
        Vector3 v3_ObjPosByWorld_fromCam = m44_ObjToWorld_fromCam * v3_ObjPosByCam;

        // to add, we see the camera position from world
        Vector3 v3_CamPosByWorld = m_ARCamera.transform.position;

        if (m_PanelText)
        {
            m_PanelText.text =
                string.Format("obj to world by default:\n" +
                "{0}\n\n" +
                "" +
                "obj to world by camera:\n" +
                "{1}\n\n" +
                "" +
                "cam by world:\n" +
                "{2}",
                v3_ObjPosByWorld,
                v3_ObjPosByWorld_fromCam,
                v3_CamPosByWorld);
        }
    }
}
