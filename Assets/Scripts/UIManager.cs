using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{ 
    [SerializeField] private Canvas UICanvas;

    private List<GameObject> UILines = new List<GameObject>();

    private void Start()
    {
        if (UICanvas == null)
        {
            Debug.LogError("UIManager: Ensure all SerializedFields are set.");
        }
    }
    
    void OnEnable()
    {
        ObjectController.OnNewObjectInitialized += HandleNewObjectInitialized;
        LimbController.OnLimbStatusChanged += UpdateUI;
    }

    void OnDisable()
    {
        ObjectController.OnNewObjectInitialized -= HandleNewObjectInitialized;
        LimbController.OnLimbStatusChanged -= UpdateUI;
    }

    private void HandleNewObjectInitialized(ObjectController objectRef)
    {
        foreach (LimbController limb in objectRef.limbsList)
        {
            GameObject UILine = Instantiate(Resources.Load<GameObject>("Limb Status Line"), UICanvas.transform);
            UILine.name = limb.gameObject.name;
            UILines.Add(UILine);
            UpdateUI(limb);
        }
    }

    private void UpdateUI(LimbController limb)
    {
        string substring = limb.attached ? "Attached" : "Unattached";
        
        foreach (GameObject UILine in UILines)
        {
            if (UILine.name == limb.name)
            {
                UILine.GetComponent<Text>().text = limb.name + ": " + substring;
            }
        }
    }
}