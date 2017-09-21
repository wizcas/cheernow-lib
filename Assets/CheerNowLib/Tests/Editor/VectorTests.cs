/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class VectorTests
{
    [Test]
    public void TestApproximately ()
    {
        var a = new Vector3(5, 5, 5);
        var b = new Vector3(3, 9, 0);
        var c = new Vector3(2, -4.001f, 5);
        var d = new Vector3(2, -4, 5);
        Assert.AreEqual(false, a.Approximately(b + c));
        Assert.AreEqual(true, a.Approximately(b + d));
    }
}
