using System;
using UnityEngine;

/// <summary>
/// The LimbController script is responsible for the logic that drives the limb selection/movement
/// feature, as well as the events for changing attachment status.
/// </summary>

public class LimbController : MonoBehaviour
{
    // Inspector Vars
    public Vector3 snapPos;
    public GameObject snapIndicatorPrefab;
    public static event Action<LimbController> OnLimbStatusChanged = delegate { };

    public class OriginalValues
    {
        public Renderer meshRenderer;
        public Material originalMaterial;
        public GameObject originalParent;
        public Vector3 originalLocalPosition;
        
        public OriginalValues(LimbController limb)
        {
            meshRenderer = limb.GetComponent<Renderer>();
            originalMaterial = meshRenderer.material;
            originalParent = limb.transform.parent.gameObject;
            originalLocalPosition = limb.transform.localPosition;
        }
    }
    
    // Runtime Vars
    private BoxCollider rayCastPlaneCollider;
    public bool attached = true;
    private Vector3 openSocketPos;
    private GameObject snapIndicatorObject;
    public ObjectController rootObject;

    public OriginalValues limbValues;
    
    // isBeingDragged is necessary because our mouse can leave the limb while it's being dragged,
    // and we don't want the highlight to change until OnMouseUp in that case.
    private bool isBeingDragged;

    // Arbitrary distance for when we should snap a limb to it's socket
    private float snapDistance = 0.0025f;

    private void Start()
    {
        limbValues = new OriginalValues(this);
        
        rayCastPlaneCollider = rootObject.raycastPlane.GetComponent<BoxCollider>();
    }
    
    void OnEnable()
    {
        SelectionManager.OnMouseDragLimb += HandleMouseDragLimb;
        SelectionManager.OnSnapLimb += HandleSnapLimb;
    }

    void OnDisable()
    {
        SelectionManager.OnMouseDragLimb -= HandleMouseDragLimb;
        SelectionManager.OnSnapLimb -= HandleSnapLimb;
    }

    private void HandleMouseDragLimb(LimbController limb, Vector2 hit)
    {
        if (limb != this) return;
        
        this.transform.position = hit;

        if (attached)
        {
            this.transform.parent = null;
            attached = false;
            OnLimbStatusChanged(this);
        }
        
    }

    private void HandleSnapLimb(LimbController limb)
    {
        if (limb != this) return;
        
        this.transform.parent = limbValues.originalParent.transform;
        this.transform.position = snapPos;
        attached = true;
        OnLimbStatusChanged(this);
    }

    public void ResetLimb()
    {
        snapPos = (Quaternion.Euler(limbValues.originalParent.transform.eulerAngles) * limbValues.originalLocalPosition) +
                        limbValues.originalParent.transform.position;
        
        this.transform.parent = limbValues.originalParent.transform;
        this.transform.position = snapPos;
                    
        attached = true;
        OnLimbStatusChanged(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var newPos = Quaternion.Euler(limbValues.originalParent.transform.eulerAngles) * limbValues.originalLocalPosition;
        Gizmos.DrawWireSphere(limbValues.originalParent.transform.position + newPos, 0.025f);
    }
}
