using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ListCheck : MonoBehaviour
{
    [SerializeField] PureClass pureClass;
    [SerializeField] PureClass referenceClass;
    [SerializeField] string thingToPutIn = "NOTHING";
    [SerializeField] bool createNewClass = false;

    void OnEnable()
    {
        referenceClass = pureClass;
    }

    void Update()
    {
        if (!createNewClass) return;

        createNewClass = false;
        pureClass = new PureClass(thingToPutIn);
    }
}

[Serializable]
public class PureClass
{
    [SerializeField] string checkingStuff;

    public PureClass(string checkingStuff)
    {
        this.checkingStuff = checkingStuff;
    }
    
}
