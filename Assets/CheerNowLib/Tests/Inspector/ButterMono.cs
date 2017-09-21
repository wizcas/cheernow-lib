/**************************************
/* Created by Wizcas (http://wizcas.me)
**************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cheers;

public class ButterMono : MonoBase
{
    protected override void ByBaseAbstract ()
    {
        PrettyLog.Log("Abstract in butter");
    }

    public override void ByBaseVirtual ()
    {
        base.ByBaseVirtual();
        PrettyLog.Log("Virtual in butter");
    }

    [ClickMe]
    public float ByButter ()
    {
        return .51f;
    }

    [Help("yes yes yes yes <color=green>yes</color> yes yes yes yes yes yes yes yes yes yes yes")]
    public int field1;
    public int field2;
    public int field3;
    public int field4;
    [Help("no no no no no no no no no no no no no no no no no no no no no no", IsExpanded = true)]
    public int field5;
    public int field6;

    public bool fb;

    [ClickMe]
    public float ReadOnlyProp {
        get {
            return Mathf.PI;
        }
    }

    [ClickMe]
    public ButterMono ObjProp {
        get { return this; }
    }

    [ClickMe]
    void TestParamter (int a)
    {
        PrettyLog.Log("A={0}", a);
    }

    public Nested nested;
}


[System.Serializable]
public class Nested
{
    [Help("testtesttesttesttesttesttesttesttesttesttesttesttesttesttesttest")]
    public int nestedInt;
    public int n1;
    [Help("test")]
    public bool nestedBool;

    public Nested2 nested;
}

[System.Serializable]
public class Nested2
{
    [Help("testtesttesttesttesttesttesttesttesttesttesttesttest")]
    public float nestedFloat;
    public int n2;
    [Help("test")]
    public bool n3;
}