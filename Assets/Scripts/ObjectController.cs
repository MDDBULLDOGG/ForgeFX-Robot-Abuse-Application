using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ObjectController script is responsible for initializing sceneObject and all of it's parts,
/// and then communicating that list to the CoreController for UI init.
/// </summary>

public class ObjectController : MonoBehaviour
{
    [SerializeField] private GameObject root;

    public List<LimbController> limbsList = new List<LimbController>();
    public static event Action<ObjectController> OnNewObjectInitialized = delegate { };
    
    private void Start()
    {
        foreach (Transform limb in root.GetComponentsInChildren<Transform>())
        {
            if (limb.transform == root.transform) continue;
            
            limb.gameObject.AddComponent<BoxCollider>();
            LimbController limbController = limb.gameObject.AddComponent<LimbController>();
            
            limbsList.Add(limbController);
        }
        
        OnNewObjectInitialized(this);
    }
}
