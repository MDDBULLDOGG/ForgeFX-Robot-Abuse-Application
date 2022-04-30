using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    private Color _originalColor;
    private List<Transform> childList = new List<Transform>();
    private bool highlighted = false;

    // TODO: Grab all selectionControllers only once instead of every time on mouse enter/exit
    private void Awake()
    {
        _originalColor = this.GetComponent<Renderer>().material.color;

        foreach (Transform child in this.GetComponentsInChildren<Transform>())
        {
            childList.Add(child);
        }
    }
    
    private void OnMouseEnter()
    {
        // Since the children will be following the parent we want to highlight/unhighlight all children
        foreach (Transform child in childList)
        {
            child.GetComponent<SelectionController>().ToggleHighlight();
        }
    }
    
    private void OnMouseExit()
    {
        // Since the children will be following the parent we want to highlight/unhighlight all children
        foreach (Transform child in childList)
        {
            child.GetComponent<SelectionController>().ToggleHighlight();
        }
    }

    private void ToggleHighlight()
    {
        if (highlighted)
        {
            this.GetComponent<Renderer>().material.color = _originalColor;
            highlighted = false;
        }
        else
        {
            this.GetComponent<Renderer>().material.color = Color.yellow;
            highlighted = true;
        }
    }
}
