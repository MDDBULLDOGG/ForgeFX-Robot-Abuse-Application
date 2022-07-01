using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{ 
    [SerializeField] private Canvas UICanvas;
    [SerializeField] private Button resetButton;

    private List<GameObject> UILines = new List<GameObject>();
    
    public static event Action OnResetButtonClicked = delegate { };

    private void OnEnable()
    {
        ObjectController.OnNewObjectInitialized += HandleNewObjectInitialized;
        LimbController.OnLimbStatusChanged += UpdateUI;
        resetButton.onClick.AddListener(ResetButtonClicked);
    }

    private void OnDisable()
    {
        ObjectController.OnNewObjectInitialized -= HandleNewObjectInitialized;
        LimbController.OnLimbStatusChanged -= UpdateUI;
        resetButton.onClick.RemoveAllListeners();
    }
    
    private void Start()
    {
        if (UICanvas == null || resetButton == null)
        {
            Debug.LogError("UIManager: Ensure all SerializedFields are set.");
        }
    }
    
    private void ResetButtonClicked()
    {
        OnResetButtonClicked();
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