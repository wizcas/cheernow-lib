/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorEx
{
    public const float Epsilon = .025f;
    public const float SqrEpsilon = .000625f;

    public static string Print (this Vector3 v3, int digits = 4)
    {
        var fmt = string.Format("({{0:F{0}}}, {{1:F{0}}}, {{2:F{0}}})", digits);
        return string.Format(fmt, v3.x, v3.y, v3.z);
    }

    public static string Print (this Vector2 v2, int digits = 4)
    {
        var fmt = string.Format("({{0:F{0}}}, {{1:F{0}}})", digits);
        return string.Format(fmt, v2.x, v2.y);
    }

    public static Vector3 NaN3 = new Vector3(float.NaN, float.NaN, float.NaN);
    public static Vector2 NaN2 = new Vector2(float.NaN, float.NaN);

    public static bool IsNaN (Vector3 v3)
    {
        return float.IsNaN(v3.x) || float.IsNaN(v3.y) || float.IsNaN(v3.z); 
    }

    public static bool IsNaN (Vector2 v2)
    {
        return float.IsNaN(v2.x) || float.IsNaN(v2.y); 
    }

    public static bool Approximately (this Vector3 a, Vector3 b)
    {
        return Mathf.Approximately(a.x, b.x) &&
        Mathf.Approximately(a.y, b.y) &&
        Mathf.Approximately(a.z, b.z);
    }

    public static bool Approximately (this Vector2 a, Vector2 b)
    {
        return Mathf.Approximately(a.x, b.x) &&
        Mathf.Approximately(a.y, b.y);
    }

    public static bool Near(this Vector3 a, Vector3 b, float nearDistance = Epsilon){
        return (a - b).sqrMagnitude <= nearDistance * nearDistance;
    }

    public static bool Near(this Vector2 a, Vector2 b, float nearDistance = Epsilon){
        return (a - b).sqrMagnitude <= nearDistance * nearDistance;
    }
}
