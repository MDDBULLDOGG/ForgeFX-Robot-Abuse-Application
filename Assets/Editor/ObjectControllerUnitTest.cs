using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ObjectControllerUnitTest
{
    public ObjectController objectController;
    
    [SetUp]
    public void SetUp()
    {
        objectController = new GameObject().AddComponent<ObjectController>();
    }

    [Test]
    public void TestObjectRootIsNotNull()
    {
        GameObject testObject = new GameObject("testObject");
        objectController.root = testObject;
        
        Assert.IsNotNull(objectController.root);
    }
    
    [Test]
    public void TestObjectLimbsControllerInitialized()
    {
        GameObject testObject = new GameObject("testObject");
        objectController.root = testObject;
        
        GameObject testLimb = new GameObject("testLimb");
        testLimb.transform.parent = testObject.transform;
   
        objectController.Initialize();
        Assert.IsNotNull(testLimb.GetComponent<LimbController>());
    }
    
    [Test]
    public void TestObjectLimbsListInitialized()
    {
        GameObject testObject = new GameObject("testObject");
        objectController.root = testObject;

        for (int i = 0; i < 5; i++)
        {
            GameObject testLimb = new GameObject("testLimb" + i);
            testLimb.transform.parent = testObject.transform;
        }
   
        objectController.Initialize();
        Assert.AreEqual(5, objectController.limbsList.Count);
    }
}