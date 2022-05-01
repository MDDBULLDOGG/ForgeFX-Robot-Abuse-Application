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
    public static CoreController Instance { get; private set; }

    [SerializeField] public GameObject raycastPlane;
    [HideInInspector] public GameObject sceneObject;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        Object prefabObject = Resources.Load("Robot_Toy_Prefab");
        sceneObject = Instantiate(prefabObject) as GameObject;
    }
}