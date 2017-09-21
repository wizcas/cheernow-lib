/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CheerNow
{
    [CreateAssetMenu(fileName = "UnicodeConfig", menuName = "Unicode Config", order = 1)]
    public class UnicodeConfig : ScriptableObject {

        const string csUnicodeConfigPrefabName = "UnicodeConfig";
        private static UnicodeConfig _instance;

        public static UnicodeConfig Instance
        {
            get {
                if (_instance == null) {
                    _instance = Resources.Load<UnicodeConfig>(csUnicodeConfigPrefabName);
                }
                return _instance;
            }
        }

        public List<UnicodeRange> illegalExcludes;
        public List<UnicodeRange> illegalRanges;
        public string illegalExcluesFilePath;
        public string illegalRangesFilePath;

        #if UNITY_EDITOR
        public void ParseIllegalExcludes() {
            illegalExcludes = ParseUnicodeRanges(illegalExcluesFilePath, false);
        }

        public void ParseIllegalRanges() {
            illegalRanges = ParseUnicodeRanges(illegalRangesFilePath, true);
        }

        private List<UnicodeRange> ParseUnicodeRanges(string path, bool applyExcludes) {
            TextAsset txt = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            string[] lines = txt.text.Split('\n');
            List<UnicodeRange> ranges = new List<UnicodeRange>();

            for (int i = 0; i < lines.Length; i++) {
                var line = lines[i].Trim();
                if (line.StartsWith("#") || string.IsNullOrEmpty(line))
                    continue;
                    
                var firstColPos = line.IndexOf(' ');
                if (firstColPos < 0)
                    firstColPos = line.IndexOf('\t');
                string firstCol = firstColPos >= 0 ? line.Substring(0, firstColPos) : line;
                if (string.IsNullOrEmpty(firstCol))
                    continue;
                string[] codeStrs = firstCol.Split(new string[]{ ".." }, System.StringSplitOptions.None);
                int min = -1, max = -1;
                if (codeStrs.Length == 1) {
                    min = max = TryParseCode(codeStrs[0]);
                }
                else {
                    min = TryParseCode(codeStrs[0]);
                    max = TryParseCode(codeStrs[1]);
                }

                if (min < 0 || max < 0) {
                    Debug.LogWarningFormat("Error line: {0} ({1})", firstCol, i);
                    continue;
                }

                ranges.Add(new UnicodeRange(min, max));
            }
            if (applyExcludes)
                ranges = VerifyExcludes(MergeIfPossible(ranges));
            else
                ranges = MergeIfPossible(ranges);
            return ranges;
        }

        private int TryParseCode(string str) {
            str = str.Trim();
            if (string.IsNullOrEmpty(str))
                return -1;
            try {
                return System.Convert.ToInt32(str, 16);
            }
            catch (System.Exception e) {
                Debug.LogErrorFormat("Parse code error: {0}", e);
                return -1;
            }
        }

        public List<UnicodeRange> MergeIfPossible(List<UnicodeRange> ranges) {
            ranges.Sort();
            // connect adjacent
            List<UnicodeRange> connected = new List<UnicodeRange>();
            UnicodeRange? prev = null;
            foreach (var range in ranges) {
                if (!prev.HasValue) {
                    prev = range;
                    continue;
                }

                if (range.min == prev.Value.max || range.min == prev.Value.max + 1) {
                    prev = new UnicodeRange?(new UnicodeRange(prev.Value.min, range.max));
                }
                else {
                    connected.Add(prev.Value);
                    prev = range;
                }
            }
            if (prev.HasValue)
                connected.Add(prev.Value);
            // merge ranges
            List<UnicodeRange> merged = new List<UnicodeRange>();
            prev = null;
            foreach (var range in connected) {
                if (!prev.HasValue) {
                    prev = range;
                    continue;
                }

                if (!prev.Value.IsInRange(range.min)) {
                    merged.Add(prev.Value);
                    prev = range;
                }
                else if (range.max > prev.Value.max) {
                    prev = new UnicodeRange?(new UnicodeRange(prev.Value.min, range.max));
                }
            }
            if (prev.HasValue)
                merged.Add(prev.Value);

            return merged;
        }

        private List<UnicodeRange> VerifyExcludes(List<UnicodeRange> ranges) {
            illegalExcludes = MergeIfPossible(illegalExcludes);

            List<UnicodeRange> ret = new List<UnicodeRange>();
            foreach (var raw in ranges) {
                UnicodeRange next = raw;
                bool skip = false;
                foreach (var exclude in illegalExcludes) {

                    if (!next.IsInRange(exclude.min) && !next.IsInRange(exclude.max)) {
                        continue;
                    }
                    if (exclude.IsInRange(next.min) && exclude.IsInRange(next.max)) {
                        skip = true;
                        break;
                    }
                    if (exclude.min > next.min) {
                        ret.Add(new UnicodeRange(next.min, exclude.min - 1));
                    }
                    if (exclude.max < next.max) {
                        next = new UnicodeRange(exclude.max + 1, next.max);
                    }
                }
                if (!skip)
                    ret.Add(next);
            }
            return ret;
        }
        #endif
    }
}