using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// The ObjectController script is responsible for initializing sceneObject and it's parts

//TODO: Monitor limb status etc from this script as well?
public class ObjectController : MonoBehaviour
{
    public CoreController coreController;
    [SerializeField] private GameObject root;
    void Start()
    {
        // First, initializing colliders for all objects we want to consider movable
        foreach (Transform child in root.GetComponentsInChildren<Transform>())
        {
            // Since we want to consider our root to be immovable, we skip this transform
            if (child.transform == root.transform) continue;
            
            child.gameObject.AddComponent<BoxCollider>();
            child.gameObject.AddComponent<SelectionController>().sceneObjectController = this;
        }
        
        Debug.LogError(coreController);
    }
}
