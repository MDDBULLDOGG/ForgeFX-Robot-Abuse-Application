using System;
using UnityEngine;

/// <summary>
/// The LimbController script is responsible for the logic that drives the limb selection/movement
/// feature, as well as the events for changing attachment status.
/// </summary>

public class LimbController : MonoBehaviour
{
    public static event Action<LimbController> OnLimbStatusChanged = delegate { };

    public Vector3 snapPos;
    public bool attached = true;

    public OriginalValues limbValues;
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

    private void Start()
    {
        limbValues = new OriginalValues(this);
    }
    
    private void OnEnable()
    {
        SelectionManager.OnMouseDragLimb += HandleDrag;
        SelectionManager.OnSnapLimb += HandleSnap;
        UIManager.OnResetButtonClicked += ResetLimb;
    }

    private void OnDisable()
    {
        SelectionManager.OnMouseDragLimb -= HandleDrag;
        SelectionManager.OnSnapLimb -= HandleSnap;
        UIManager.OnResetButtonClicked -= ResetLimb;
    }

    private void HandleDrag(LimbController limb, Vector2 hit)
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

    private void HandleSnap(LimbController limb)
    {
        if (limb != this) return;
        
        this.transform.parent = limbValues.originalParent.transform;
        this.transform.position = snapPos;
        attached = true;
        OnLimbStatusChanged(this);
    }

    private void ResetLimb()
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
