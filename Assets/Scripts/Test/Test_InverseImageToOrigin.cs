using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_InverseImageToOrigin : MonoBehaviour
{
    [SerializeField]
    GameObject worldOriginPrefab, imagePrefab;

    // according to root, the transform information of imageTarget:
    // - position at 3 pixels away from X axis, 1 pixel away from Z axis
    Vector3 rootToImage_pos = new(0.3f, 0.4f, 0.5f);

    // - rotation -90 degree in Y axis
    // - note that once rotation done, CS axis will change direction
    // - we should apply with updated CS, then do rotation one by one
    // - by this we can't apply all rotation at once!!!
    // - for example: after X-axis rot, now Y-axis and Z-axis direction have changed
    Vector3 rootToImage_rot = new(-90, 90, 180);

    // Start is called before the first frame update
    void Start()
    {
        //CreatePebles();

        MyMethod();
    }

    void CreatePebles()
    {
        GameObject point = new("point");
        for (int i = 0; i < 11; i++)
        {
            float x = -0.5f + (0.1f * i);

            for (int j = 0; j < 11; j++)
            {
                float z = -0.5f + (0.1f * j);
                Vector3 pos = new(x, 0, z);
                GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                gameObject.transform.position = pos;
                gameObject.name = "gO-" + x + "-" + z;
                gameObject.transform.SetParent(point.transform);
                gameObject.transform.localScale = new(0.01f, 0.01f, 0.01f);
            }
        }
    }

    GameObject CreateImageTargetPosition()
    {
        GameObject gameObject = Instantiate(imagePrefab);

        // change this if you want
        gameObject.transform.position = new(0, 0, 0);
        gameObject.transform.Rotate(new(0, 0, 0));

        return gameObject;
    }

    void MyMethod()
    {
        // 0. get image target gameobject
        GameObject imageTarget = CreateImageTargetPosition();

        // ================== //
        // 1. create our root based on imagetarget
        GameObject ourRoot = Instantiate(worldOriginPrefab);

            // we need set it as parent first because we want local transformation
        ourRoot.transform.SetParent(imageTarget.transform, false);
        ourRoot.name = "ourRoot";

        // ================== //
        // 2. make dummy object to inverse the transformation
        GameObject dummy = new();

            // rotate with our root to image target ROTATION data
        dummy = GlobalConfig.RotateOneByOne(dummy, rootToImage_rot);

        // get its inverse of rotation
        Quaternion imageTarget_rotinv = Quaternion.Inverse(dummy.transform.rotation);

            // apply to our root
        ourRoot.transform.localRotation = imageTarget_rotinv;

        // ================== //
        // 3. calculate our position with calculating the localToWorldMatrix

            // make our dummy to use the inverse rotation too
        dummy.transform.rotation = imageTarget_rotinv;

            // get the M4x4 matrix of from local to world of our dummy after rotation
        Matrix4x4 mat4 = dummy.transform.localToWorldMatrix;

            // vector multiplication with our root to image target POSITION DATA
        Vector3 vec3 = mat4 * rootToImage_pos;

            // apply to our root, but inverse it (-)
        ourRoot.transform.localPosition = -vec3;

        // ================== //
        // 4. make our root become ROOT now
        ourRoot.transform.SetParent(null);
        imageTarget.transform.SetParent(ourRoot.transform);

            // destroy the dummy object
        Destroy(dummy);

        // ================== //
        // ADDITIONAL STEP
        // 5. to get gitgud visualization

            // put ourRoot in origin of unity
        ourRoot.transform.position = Vector3.zero;
        ourRoot.transform.rotation = Quaternion.identity;
    }

    void Backup()
    {
        //Vector3 desireOrigin_pos = Vector3.zero;
        //Vector3 desireOrigin_rot = Vector3.zero;
        //GameObject origin = Instantiate(worldOriginPrefab,
        //                                desireOrigin_pos,
        //                                Quaternion.identity);
        //origin.transform.Rotate(desireOrigin_rot);
        //origin.name = "origin";

        // camera found the image target on world space
        GameObject imageTarget = Instantiate(worldOriginPrefab);

        // according to world space, it is on (0.2, 0, 0.2) with 180 degree in Y axis
        imageTarget.transform.position = new(0, 0, 0);
        imageTarget.transform.Rotate(new(0, 0, 0));
        imageTarget.name = "imageTarget";

        // we have the information where imagetarget in our map
        // and we have the information where imagetarget in world space
        // how to know where our map origin?

        //Vector3 it_pos_inv = -rootToImage_pos;
        //Vector3 it_rot_inv = -rootToImage_rot;
        //Quaternion it_quat_inv = Quaternion.Inverse(imageTarget.transform.rotation);

        //GameObject imageTarget_inv = Instantiate(imageTarget);
        //imageTarget_inv.transform.SetParent(imageTarget.transform);
        //imageTarget_inv.transform.localPosition = it_pos_inv;
        //imageTarget_inv.transform.Rotate(it_rot_inv);
        ////imageTarget_inv.transform.rotation = it_quat_inv;
        //imageTarget_inv.name = "imageTarget_inv";

        // ==========
        // another way is our previous way but with little changes

        // 1. create the object on specific, or that image target location
        // - DONE IT ABOVE

        // 2. we put the descriptor not from our origin to imageTarget but the opposite
        // - position will be X = -1, Y = 0, and Z = 3
        // - rotation will be 90 degree from imageTarget
        Vector3 fromImageToRoot_pos = new(-0.1f, 0, 0.3f);
        Vector3 fromImageToRoot_rot = new(0, 90, 0);

        // 3. because we define from imageTarget, simply put as child first
        GameObject ourRoot = Instantiate(imageTarget);
        ourRoot.transform.SetParent(imageTarget.transform);
        ourRoot.name = "ourRoot";

        // 4. then apply the transformation
        //ourRoot.transform.localPosition = fromImageToRoot_pos;
        //ourRoot.transform.Rotate(fromImageToRoot_rot);

        // 4.2. if we use inverse of quaternion
        // from 90 --> Inverse(-90) in Y axis
        GameObject dummy = new();
        dummy.transform.Rotate(rootToImage_rot);
        Quaternion imageTarget_rotinv = Quaternion.Inverse(dummy.transform.rotation);
        ourRoot.transform.localRotation = imageTarget_rotinv;

        // 4.3. if we use our root information
        // question: how to convert from rootToImage_pos to fromImageToRoot_pos?
        // from (0.3f, 0, 0.1f) --> (-0.1f, 0, 0.3f)
        dummy.transform.rotation = imageTarget_rotinv;
        Matrix4x4 mat4 = dummy.transform.localToWorldMatrix;
        Vector3 vec3 = mat4 * rootToImage_pos;
        ourRoot.transform.localPosition = -vec3;

        // 5. now disband from parent
        //ourRoot.transform.SetParent(null);
        Destroy(dummy);

        // the result is as same as our old technique
        // the key is, definition from image target, not from root
        // but what if we want the inverse?

        // =========

        //Debug.Log(dummy.transform.localToWorldMatrix);
        //GameObject dummy2 = new();
        //dummy2.transform.rotation = imageTarget_rotinv;
        //Debug.Log(dummy2.transform.localToWorldMatrix);
    }
}
