using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CoreControllerUnitTest
{
    public CoreController coreController;
    
    [SetUp]
    public void SetUp()
    {
        coreController = new GameObject().AddComponent<CoreController>();
    }
    
    [Test]
    public void TestCoreControllerIsNotNull()
    {
        Assert.IsNotNull(coreController);
    }
    
    [Test]
    public void TestCoreControllerInitializedPrefab()
    {
        GameObject testObject = new GameObject("testObject");
        coreController.prefabObject = testObject;
        coreController.Awake();

        Assert.IsNotNull(GameObject.Find(testObject.name));
    }
}
