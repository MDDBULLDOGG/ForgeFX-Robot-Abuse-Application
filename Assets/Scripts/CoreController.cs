using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// The CoreController script is responsible for initializing all of the appropriate objects on
// environment start as well as maintaining access to specific objects we'll need such as
// the UI.

public class CoreController : MonoBehaviour
{
    // Inspector Vars
    [SerializeField] public Canvas UICanvas;
    [SerializeField] public GameObject raycastPlane;
    [SerializeField] public GameObject prefabObject;

    // Runtime Vars
    [HideInInspector] public GameObject sceneObject;
    private List<GameObject> UILines = new List<GameObject>();
    
    // Events
    public Events.sceneObjectInitialized sceneObjectInitialized;
    public Events.attachmentStatusChanged attachmentStatusChanged;
    
    // Exposing our CoreController to other scripts
    public static CoreController Instance { get; private set; }

    public void Awake()
    {
        // Making sure this is our only instance of CoreController and assigning it to Instance
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        
        // Instantiating the model prefab we want to play with
        sceneObject = Instantiate(prefabObject);
    }

    // Start() is used to make sure all of our events aren't null
    private void Start()
    {
        sceneObjectInitialized ??= new Events.sceneObjectInitialized();
        sceneObjectInitialized.AddListener(SceneObjectInitializedEvent);
        
        attachmentStatusChanged ??= new Events.attachmentStatusChanged();
        attachmentStatusChanged.AddListener(AttachmentEvent);
    }
    
    // Once the scene object is initialized, we want to initialize the UI with correct values
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
        string substring = attached ? "Attached" : "Unattached";
        
        foreach (GameObject UILine in UILines)
        {
            if (UILine.name == limbName)
            {
                UILine.GetComponent<Text>().text = limbName + ": " + substring;
            }
        }
    }
}