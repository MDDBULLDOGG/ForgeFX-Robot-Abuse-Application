using System.Collections.Generic;
using UnityEngine;

// The ObjectController script is responsible for initializing sceneObject and all of it's parts,
// and then communicating that list to the CoreController for UI init.

public class ObjectController : MonoBehaviour
{
    // Inspector Vars
    public GameObject root;
    
    // Runtime Vars
    private Dictionary<string, bool> limbList = new Dictionary<string, bool>();
    
    private void Start()
    {
        // Initialize colliders for all objects we want to consider movable and form a Dictionary
        // to send to CoreController
        foreach (Transform limb in root.GetComponentsInChildren<Transform>())
        {
            // Since we want to consider our root to be immovable, we skip this transform
            if (limb.transform == root.transform) continue;
            
            limb.gameObject.AddComponent<BoxCollider>();
            LimbController limbController = limb.gameObject.AddComponent<LimbController>();
            
            limbList.Add(limb.name, limbController.attached);
        }
        
        // Call our event so that the CoreController knows our object has been properly initialized
        CoreController.Instance.sceneObjectInitialized.Invoke(limbList);
    }
}
