using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// The CoreController script is responsible for initializing the appropriate objects such as the
// sceneObject and statusObject upon the environment's start.

//TODO: Initialize statusObject for monitoring limb status
public class CoreController : MonoBehaviour
{
    void Start()
    {
        Object prefabObject = Resources.Load("Robot_Toy_Prefab");
        GameObject sceneObject = Instantiate(prefabObject) as GameObject;
    }
}
