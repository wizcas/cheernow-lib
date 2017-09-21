﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentExtensions
{
    public static void DestroyChildren (this Transform component)
    {
        foreach (Transform trans in component) {
            if (Application.isPlaying) {
                Object.Destroy(trans.gameObject);
            }
        }
    }

    public static T[] GetComponentsInAllChildren<T> (this Component parent, bool includeSelf = false)
        where T: Component
    {
        List<T> ret = new List<T>();
        if (includeSelf) {
            var c = parent.GetComponent<T>();
            if (c != null)
                ret.Add(c);
        }

        ret.AddRange(parent.GetComponentsInChildren<T>());
        foreach (Transform child in parent.transform) {
            ret.AddRange(child.GetComponentsInAllChildren<T>());
        }
        return ret.ToArray();
    }
}
