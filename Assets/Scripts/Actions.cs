using System;
using System.Collections.Generic;
using UnityEngine;

public static class Actions
{
    public static Action<ObjectController> OnNewObjectInitialized;
    
    public static Action<LimbController> OnLimbStatusChanged;
    
    public static Action<LimbController> OnSelectionChanged;
}