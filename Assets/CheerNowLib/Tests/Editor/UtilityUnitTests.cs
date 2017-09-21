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
using System;

namespace Cheers
{
    public class UtilityUnitTests
    {
#pragma warning disable 0414
        abstract class RootBase
        {
            public int rootPublic = 10086;
            protected int rootProtected = 10010;
        }

        struct Nested
        {
            public float someValue;
        }

        class Root : RootBase
        {
            Nested rootPrivate = new Nested();
            ArrayElement[] arrElements = { new ArrayElement() { nested = new Nested() }, new ArrayElement { yesOrNo = true } };
            List<ArrayElement> listElements = new List<ArrayElement>
        {
            new ArrayElement() { nested = new Nested{ someValue = 1.25f } }, new ArrayElement { yesOrNo = true }
        };
        }

        struct ArrayElement
        {
            public bool yesOrNo;
            public Nested nested;
        }
#pragma warning restore 0414

        void AssertTypeAndValue(object obj, string path, Type expectedType, object expectedValue)
        {
            var fi = EditorHelper.GetObjectFieldInfo(obj.GetType(), path);
            var fv = EditorHelper.GetObjectFieldValue(obj, path);
            Assert.IsNotNull(fi, "failed to retrieve field info {0}/{1}", obj.GetType(), path);
            Assert.IsNotNull(fv, "failed to retrieve field value {0}/{1}", obj.GetType(), path);
            Assert.AreEqual(expectedType, fi.FieldType);
            Assert.AreEqual(expectedValue, fv);
        }

        [Test]
        public void TestColorName()
        {
            Assert.AreEqual(true, ColorNames.IsNameAvailable("maroon"));
            Assert.AreEqual(false, ColorNames.IsNameAvailable("wizcas"));
        }

        [Test]
        public void TestGetObjectFieldPlain()
        {
            var root = new Root();
            AssertTypeAndValue(root, "rootPublic", typeof(int), 10086);
            AssertTypeAndValue(root, "rootProtected", typeof(int), 10010);
            AssertTypeAndValue(root, "rootPrivate", typeof(Nested), new Nested());
        }

        [Test]
        public void TestGetObjectFieldNested()
        {
            var root = new Root();
            AssertTypeAndValue(root, "rootPrivate.someValue", typeof(float), 0f);
        }

        [Test]
        public void TestGetObjectFieldArrayPlain()
        {
            AssertTypeAndValue(new Root(), "arrElements.Array.data[0]", typeof(ArrayElement[]), new ArrayElement());
            AssertTypeAndValue(new Root(), "listElements.Array.data[1]", typeof(List<ArrayElement>), new ArrayElement() { yesOrNo = true });
        }

        [Test]
        public void TestGetObjectFieldArrayNested()
        {
            AssertTypeAndValue(new Root(), "arrElements.Array.data[0].yesOrNo", typeof(bool), false);
            AssertTypeAndValue(new Root(), "listElements.Array.data[0].nested.someValue", typeof(float), 1.25f);
        }
    }
}