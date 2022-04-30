using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// The ObjectController script is responsible for initializing sceneObject and it's parts

//TODO: Monitor limb status etc from this script as well?
public class ObjectController : MonoBehaviour
{
    [SerializeField] private GameObject root;

    void Awake()
    {
        // First, initializing colliders for all objects we want to consider movable
        foreach (Transform child in root.GetComponentsInChildren<Transform>())
        {
            // Since we want to consider our root to be immovable, we skip this transform
            if (child.transform == root.transform) continue;
            
            Debug.Log("Adding default collider to " + child.name);
            child.gameObject.AddComponent<BoxCollider>();
            child.gameObject.AddComponent<SelectionController>();
        }
    }
}
