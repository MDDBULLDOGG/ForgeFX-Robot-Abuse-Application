using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// The CoreController script is responsible for initializing the appropriate objects upon the
// environment's start as well as maintaining access to specific objects we'll need such as
// the raycast plane and limb status indicator.

//TODO: Initialize statusObject for monitoring limb status
public class CoreController : MonoBehaviour
{ 
    [SerializeField] public GameObject raycastPlane;

    void Awake()
    {
        Object prefabObject = Resources.Load("Robot_Toy_Prefab");
        GameObject sceneObject = Instantiate(prefabObject) as GameObject;
        Debug.LogError(sceneObject.GetComponent<ObjectController>());
        sceneObject.GetComponent<ObjectController>().coreController = this;
    }
}