using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Description:
 * 1. to show 3D position of object related to loaded map (world coordinate)
 * 2. to show 3D position of object related to slam coordinate
 * 
 * Actual process:
 * - loaded map or Wc is a single point assigned as origin
 * - this origin also assigned to Unity world origin
 * - Unity world origin usually assigned by slam coordinate
 * - because in ARKit, Unity world origin assigned where app opened
 * 
 * Addition:
 * - we can also change world origin position
 * - this also change slam coordinate origin
 */

public class Test_ShowLocationAboveObject : MonoBehaviour
{
    List<GameObject> gameObjects_List = new();

    [SerializeField]
    GameObject m_PrefabShowLocation;

    [SerializeField]
    GameObject m_ARCamera;

    const string showTextName = "LocationText";

    public void AddGameObject (GameObject gO)
    {
        gameObjects_List.Add(gO);
    }

    bool CheckIfWorldOriginExists()
    {
        if (GlobalConfig.PlaySpaceOriginGO != null) return true;

        return false;
    }

    bool CheckChildShowTextPrefab(GameObject go, out GameObject textGameObject)
    {
        int child = go.transform.childCount;
        for (int i = 0; i < child; i++)
        {
            textGameObject = go.transform.GetChild(i).gameObject;
            if (textGameObject.name == showTextName)
                return true;
        }

        textGameObject = null;
        return false;
    }

    private void OnEnable()
    {
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);

            //Debug.Log(gameObjects_List.Count);
            //Debug.Log(CheckIfWorldOriginExists());
            //Debug.Log(GlobalConfig.TempOriginGO.transform.position.ToString());

            if (CheckIfWorldOriginExists() && gameObjects_List.Count > 0)
            {

                foreach (var go in gameObjects_List)
                {
                    if (!CheckChildShowTextPrefab(go, out GameObject textGameObject))
                    {
                        textGameObject = Instantiate(m_PrefabShowLocation, go.transform);
                        textGameObject.name = showTextName;
                        textGameObject.AddComponent<ShowTextLocationText>();

                        // scaling
                        Vector3 scl = go.transform.localScale;
                        float avg = (scl.x + scl.y + scl.z) / 3;
                        float scaling = (float)(0.025 / avg);
                        textGameObject.transform.localScale = new Vector3(scaling, scaling, scaling);
                    }

                    //Debug.Log(textGameObject.name);
                    //Debug.Log(textGameObject.transform.parent);

                    Matrix4x4 worldToRoot = GlobalConfig.PlaySpaceOriginGO.transform.worldToLocalMatrix;
                    Matrix4x4 worldToCam = m_ARCamera.transform.worldToLocalMatrix;
                    Matrix4x4 objToWorld = go.transform.localToWorldMatrix;
                    Vector3 objPos = go.transform.localPosition;
                                        
                    Vector3 objPos_SLAM = go.transform.position;
                    //Vector3 objPos_SLAM = (worldToCam * objToWorld).GetPosition();
                    string objPos_SLAM_Text = "(" + objPos_SLAM.x.ToString("0.000") + ", "
                                                  + objPos_SLAM.y.ToString("0.000") + ", "
                                                  + objPos_SLAM.z.ToString("0.000") + ")";

                    //Vector3 objPos_World;

                    //if (go.transform.parent == null) objPos_World = objPos_SLAM;
                    //else objPos_World = (worldToRoot * objToWorld).GetPosition();

                    Vector3 objPos_World = (worldToRoot * objToWorld).GetPosition();

                    string objPos_World_Text = "(" + objPos_World.x.ToString("0.000") + ", "
                                                   + objPos_World.y.ToString("0.000") + ", "
                                                   + objPos_World.z.ToString("0.000") + ")";

                    textGameObject.GetComponent<ShowTextLocationText>()
                        .SetLocation1Text("World = " + objPos_World_Text);

                    textGameObject.GetComponent<ShowTextLocationText>()
                        .SetLocation2Text("SLAM = " + objPos_SLAM_Text);
                }

            }
        }
    }
}
