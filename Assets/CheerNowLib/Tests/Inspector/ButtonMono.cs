/**************************************
/* Created by Wizcas (http://wizcas.me)
**************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cheers;

public class ButtonMono : MonoBase 
{
    public int yes;
    [ReadOnly]
    public int no;
    [SerializeField, ReadOnly] Something sth;

    [ClickMe]
    public int IntProp
    {
        get { return yes; }
        set { no = value; }
    }

    [ClickMe]
    public int ReadOnlyIntProp
    {
        get { return yes; }
        set { no = value; }
    }

    protected override void ByBaseAbstract()
    {
        PrettyLog.Log("Abstract in Button");
    }

    public override void ByBaseVirtual()
    {
        base.ByBaseVirtual();
        PrettyLog.Log("Virtual in Button");
    }

    [ClickMe(BgColorString="maroon", TextColorString="white")]
    string ByButtonMono()
    {
        return "this is a string from button";
    }
}

[System.Serializable]
class Something
{
    public string text = "nothing";
}