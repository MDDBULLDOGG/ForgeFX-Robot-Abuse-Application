using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Events : MonoBehaviour
{
    public class sceneObjectInitialized : UnityEvent<List<LimbController>>
    {
    }
    
    public class attachmentStatusChanged : UnityEvent<string, bool>
    {
    }
}
