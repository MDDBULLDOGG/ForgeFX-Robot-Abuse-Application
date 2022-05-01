using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    public ObjectController sceneObjectController;
    
    private Color originalColor;
    private BoxCollider rayCastPlaneCollider;
    private bool highlighted = false;
    private bool detached = false;

    private void Start()
    {
        originalColor = this.GetComponent<Renderer>().material.color;
        rayCastPlaneCollider = CoreController.Instance.raycastPlane.GetComponent<BoxCollider>();
    }
    
    private void OnMouseEnter()
    {
        // Since the children will be following the parent we want to highlight/unhighlight all children
        foreach (Transform child in this.GetComponentsInChildren<Transform>())
        {
            child.GetComponent<SelectionController>().ToggleHighlight();
        }
    }

    private void OnMouseDrag()
    {
        this.transform.parent = null;
        detached = true;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (rayCastPlaneCollider.Raycast(ray, out hit, 10))
        {
            this.transform.position = hit.point;
        }
    }

    private void OnMouseExit()
    {
        // Since the children will be following the parent we want to highlight/unhighlight all children
        foreach (Transform child in this.GetComponentsInChildren<Transform>())
        {
            child.GetComponent<SelectionController>().ToggleHighlight();
        }
    }

    private void ToggleHighlight()
    {
        if (highlighted)
        {
            this.GetComponent<Renderer>().material.color = originalColor;
            highlighted = false;
        }
        else
        {
            this.GetComponent<Renderer>().material.color = Color.yellow;
            highlighted = true;
        }
    }
}
