using System.Collections.Generic;
using UnityEngine;

// The ObjectController script is responsible for initializing sceneObject and all of it's parts,
// and then communicating that list to the CoreController for UI init.

public class ObjectController : MonoBehaviour
{
    // Inspector Vars
    public GameObject root;
    public GameObject snapIndicatorPrefab;

    // Runtime Vars
    public List<LimbController> limbsList = new List<LimbController>();
    
    private void Start()
    {
        // Usually I would have this script's Initialize() method as part of Start(), however
        // I separated them out here just for unit-testing demonstration purposes
        Initialize();

        // Call our event so that the CoreController knows our object has been properly initialized
        CoreController.Instance.sceneObjectInitialized.Invoke(limbsList);
    }

    public void Initialize()
    {
        // Initialize colliders for all objects we want to consider movable and form a Dictionary
        // to send to CoreController
        foreach (Transform limb in root.GetComponentsInChildren<Transform>())
        {
            // Since we want to consider our root to be immovable, we skip this transform
            if (limb.transform == root.transform) continue;
            
            limb.gameObject.AddComponent<BoxCollider>();
            LimbController limbController = limb.gameObject.AddComponent<LimbController>();
            limbController.snapIndicatorPrefab = snapIndicatorPrefab;
            
            limbsList.Add(limbController);
        }
    }

    public void ResetLimbs()
    {
        foreach (LimbController limb in limbsList)
        {
            // Since we want to consider our root to be immovable, we skip this transform
            if (limb.transform == root.transform) continue;

            if(limb.attached == false)
                limb.ResetLimb();
        }
    }
}
