/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using UnityEngine;
using System.Collections;
using System;

namespace CheerNow
{
    [System.Serializable]
    public struct UnicodeRange : IComparable<UnicodeRange> {
        public int min, max;

        public UnicodeRange(int min, int max) {
            this.min = min;
            this.max = max;
        }

        public bool IsInRange(int code) {
            return code >= min && code <= max;
        }

        public static bool operator >(UnicodeRange a, UnicodeRange b) {
            if (a.min != b.min)
                return a.min > b.min;
            return a.max > b.max;
        }

        public static bool operator <(UnicodeRange a, UnicodeRange b) {
            if (a.min != b.min)
                return a.min < b.min;
            return a.max < b.max;
        }

        public override string ToString() {
            return string.Format("{0:X}..{1:X}", min, max);
        }

        #region IComparable implementation

        public int CompareTo(UnicodeRange other) {
            if (this < other)
                return -1;
            if (this > other)
                return 1;
            return 0;
        }

        #endregion
    }

    public class UnicodeHelper {

        const string EmojiDataFileName = "emoji-data";

//        private static readonly UnicodeRange[] illegalUnicodeRanges = new UnicodeRange[]
//        {
//            // ---> Control Chars
//            new UnicodeRange(0x00, 0x1f),
//            new UnicodeRange(0x7f, 0x7f),
//            // ---> Image-based Emojis
//            new UnicodeRange(0x1f600, 0x1f64f),     // Emoicons
//            new UnicodeRange(0x1f680, 0x1f6c5),     // Transport & map symbols
//            new UnicodeRange(0x1f170, 0x1f251),     // Enclosed characters
//            new UnicodeRange(0x1f300, 0x1f5ff),     // Other additional symbols & Uncategorized
//            new UnicodeRange(0x1f004, 0x1f0ff),     // Uncategorized
//            // ---> Surrogates
//            new UnicodeRange(0xd800, 0xdbff),       // High & High Private Use Surrogates
//            new UnicodeRange(0xdc00, 0xdfff),        // Low Surrogates
//            // ---> Private Use Area
//            new UnicodeRange(0xe000, 0xf8ff),
//        };

        public static string GetSafeString(string raw) {
            PrintStringCodes(raw);
            string safe = "";
            for (int i = 0; i < raw.Length; i++) {
                char c = raw[i];
                int code = (int)c;
                bool isSurrogates = false;

                if (Char.IsSurrogatePair(raw, i)) {
                    isSurrogates = true;
                    code = Char.ConvertToUtf32(raw, i);
                    i++; // 遇到HIGH-LOW代理区时下标+1，以便后面直接获取低位值写入，并且使下个循环跳过本代理区
                }
                Debug.LogFormat("[Safe String] code@{0}: {1}", i, code);
                bool isLegal = true;
                foreach (var illegalRange in UnicodeConfig.Instance.illegalRanges) {
                    if (code >= illegalRange.min && code <= illegalRange.max) {
                        isLegal = false;
                        break;
                    }
                }
                if (!isLegal)
                    continue;

                safe += c;
                if (isSurrogates) {
                    safe += raw[i]; // 写入低位
                }
            }
            PrintStringCodes(safe);
            Debug.LogFormat("[Safe String] raw: {0}, safe: {1}", raw, safe);
            return safe;
        }

        private static void PrintStringCodes(string str) {
            System.Collections.Generic.List<string> codes = new System.Collections.Generic.List<string>();
            foreach (var c in str) {
                codes.Add(((int)c).ToString());
            }
            Debug.LogFormat("[Safe String] raw: {0}\ncodes:\n{1}", str, string.Join(", ", codes.ToArray()));
        }
    }
}