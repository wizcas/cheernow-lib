/**************************************
/* Created by Wizcas (http://wizcas.me)
**************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cheers;

public abstract class MonoBase : MonoBehaviour, IMono
{
    [ClickMe]
    protected abstract void ByBaseAbstract();

    [ClickMe]
    public virtual void ByBaseVirtual()
    {
        PrettyLog.Log("virtual in BASE");
    }
    public int ByInterface()
    {
        PrettyLog.Log("implements interface");
        return 80016;
    }
}

public interface IMono
{
    [ClickMe]
    int ByInterface();
}
