using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// The CoreController script is responsible for initializing the appropriate objects upon the
// environment's start as well as maintaining access to specific objects we'll need such as
// the raycast plane and limb status indicator.

//TODO: Lists for limb status UI updates are really messy. Clean it up
public class CoreController : MonoBehaviour
{ 
    public Events.sceneObjectInitialized sceneObjectInitialized;
    public Events.attachmentStatusChanged attachmentStatusChanged;
    public GameObject prefabObject;

    public static CoreController Instance { get; private set; }

    [SerializeField] public Canvas UICanvas;
    [SerializeField] public GameObject raycastPlane;
    [HideInInspector] public GameObject sceneObject;

    private List<GameObject> UILines = new List<GameObject>();

    public void Awake()
    {
        // Making sure this is our only instance of CoreController
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        
        // Instantiating the model prefab we want to play with
        sceneObject = Instantiate(prefabObject);
    }

    private void Start()
    {
        sceneObjectInitialized ??= new Events.sceneObjectInitialized();
        sceneObjectInitialized.AddListener(SceneObjectInitializedEvent);
        
        attachmentStatusChanged ??= new Events.attachmentStatusChanged();
        attachmentStatusChanged.AddListener(AttachmentEvent);
    }
    
    // Once the scene object is initialized, we want to initialize the UI with the correct values
    private void SceneObjectInitializedEvent(Dictionary<string, bool> limbList)
    {
        foreach (var key in limbList)
        {
            Object prefabStatusLine = Resources.Load("Limb Status Line");
            GameObject UILine = Instantiate(prefabStatusLine, UICanvas.transform) as GameObject;
            UILine.name = key.Key;
            UILines.Add(UILine);
            AttachmentEvent(key.Key, key.Value);
        }
    }
    
    // Updating our UI with correct values every time an attachmentEvent is fired
    private void AttachmentEvent(string limbName, bool attached)
    {
        foreach (GameObject UILine in UILines)
        {
            if (UILine.name == limbName)
            {
                if (attached)
                {
                    UILine.GetComponent<Text>().text = limbName + ": " + "Attached";
                }
                else
                {
                    UILine.GetComponent<Text>().text = limbName + ": " + "Detached";
                }
            }
        }
    }
}