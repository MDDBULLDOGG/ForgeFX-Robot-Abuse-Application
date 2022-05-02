using System.Collections.Generic;
using UnityEngine;

// The ObjectController script is responsible for initializing sceneObject and it's parts

//TODO: Lists for limb status UI updates are really messy. Clean it up
public class ObjectController : MonoBehaviour
{
    public GameObject root;

    private Dictionary<string, bool> limbList = new Dictionary<string, bool>();
    private void Start()
    {
        // First, initializing colliders for all objects we want to consider movable
        foreach (Transform child in root.GetComponentsInChildren<Transform>())
        {
            // Since we want to consider our root to be immovable, we skip this transform
            if (child.transform == root.transform) continue;
            
            child.gameObject.AddComponent<BoxCollider>();
            SelectionController childController = child.gameObject.AddComponent<SelectionController>();
            
            limbList.Add(child.name, childController.attached);
        }
        
        CoreController.Instance.sceneObjectInitialized.Invoke(limbList);
    }
}
