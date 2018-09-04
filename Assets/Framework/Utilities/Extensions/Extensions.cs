using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using Random = System.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace PixelComrades {
    [Serializable]
    public class PrefabHolder {
        public GameObject Prefab;
        public bool CenteredPrefab = false;
        public Vector3 OffsetGridMulti = new Vector3(0, 0, 0);
    }

    [System.Serializable]
    public class RandomPrefabHolder {
        public PrefabHolder[] Prefabs = new PrefabHolder[0];
        public AnimationCurve Curve = new AnimationCurve();

        public PrefabHolder Get() {
            if (Prefabs.Length == 0) {
                return null;
            }
            if (Prefabs.Length == 1) {
                return Prefabs[0];
            }
            //return Prefabs.SafeAccess((int) (Prefabs.Length * Curve.Evaluate(UnityEngine.Random.value)));
            return Prefabs.SafeAccess((int) (Prefabs.Length * Curve.Evaluate(Game.Random.NextFloat(0,1))));
        }

        public int GetIndex() {
            return (int) (Prefabs.Length * Curve.Evaluate(Game.Random.NextFloat(0, 1)));
        }
    }

    [System.Serializable]
    public class RandomObjectHolder {
        public UnityEngine.Object[] Objects = new UnityEngine.Object[0];
        public AnimationCurve Curve = new AnimationCurve();

        public UnityEngine.Object Get() {
            if (Objects.Length == 0) {
                return null;
            }
            if (Objects.Length == 1) {
                return Objects[0];
            }
            //return Prefabs.SafeAccess((int) (Prefabs.Length * Curve.Evaluate(UnityEngine.Random.value)));
            return Objects.SafeAccess((int) (Objects.Length * Curve.Evaluate(Game.Random.NextFloat(0, 1))));
        }

        public int GetIndex() {
            return (int) (Objects.Length * Curve.Evaluate(Game.Random.NextFloat(0, 1)));
        }
    }

    [System.Serializable]
    public class RandomScriptableHolder {
        public UnityEngine.ScriptableObject[] Objects = new UnityEngine.ScriptableObject[0];
        public AnimationCurve Curve = new AnimationCurve();

        public UnityEngine.ScriptableObject Get() {
            if (Objects.Length == 0) {
                return null;
            }
            if (Objects.Length == 1) {
                return Objects[0];
            }
            return Objects.SafeAccess((int) (Objects.Length * Curve.Evaluate(Game.Random.NextFloat(0, 1))));
        }

        public int GetIndex() {
            return (int) (Objects.Length * Curve.Evaluate(Game.Random.NextFloat(0, 1)));
        }
    }

    public static class RandomExtensions {
        public static float NextFloat(this Random random, double minValue, double maxValue) {
            return (float) (random.NextDouble() * (maxValue - minValue) + minValue);
        }

        public static int Range(this Random random, int min, int max) {
            return (int) ((max - min + 1) * (float) random.NextDouble()) + min;
        }

        public static T RandomElement<T>(this IList<T> list) {
            if (list.Count == 0) {
                return default(T);
            }
            return list[Game.Random.Next(0, list.Count)];
        }

        public static T RandomElement<T>(this IList<T> list, System.Random random) {
            if (list.Count == 0) {
                return default(T);
            }
            return list[random.Next(0, list.Count)];
        }

        public static int RandomIndex<T>(this IList<T> list) {
            if (list.Count == 0) {
                return -1;
            }
            return Game.Random.Next(0, list.Count);
        }

        public static int RandomIndex<T>(this IList<T> list, System.Random random) {
            if (list.Count == 0) {
                return -1;
            }
            return random.Next(0, list.Count);
        }

        public static int LastIndex<T>(this IList<T> list) {
            return list.Count - 1;
        }

        public static void RemoveLast<T>(this IList<T> list) {
            list.RemoveAt(list.Count - 1);
        }

        public static T LastElement<T>(this IList<T> list) {
            if (list.Count == 0) {
                return default(T);
            }
            return list[list.Count - 1];
        }

        public static void Shuffle<T>(this IList<T> list, Random rnd) {
            for (var i = 0; i < list.Count - 1; i++) {
                //list.Swap(i, rnd.Next(i, list.Count));
                var temp = list[i];
                var j = rnd.Next(i, list.Count);
                list[i] = list[j];
                list[j] = temp;
            }
        }

        public static void Shuffle<T>(this IList<T> list) {
            Shuffle<T>(list, Game.Random);
        }

        public static void ShuffleArray(this Array list) {
            var rolls = new int[list.Length];
            for (var i = 0; i < list.Length; i++) {
                rolls[i] = Game.Random.DiceRoll();
            }
            System.Array.Sort(rolls, list);
        }

        public static void Swap<T>(this IList<T> list, int i, int j) {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        public static bool WithinRange<T>(this IList<T> list, int index) {
            return index >= 0 && index < list.Count;
        }

        public static bool DiceRollSucess(this System.Random random, float chance) {
            if (chance < 1 && chance > 0) {
                chance *= 100;
            }
            var roll = random.Next(0, 101);
            return roll <= chance;
        }

        public static bool DiceRollSucess(this System.Random random, int chance) {
            var roll = random.Next(0, 101);
            return roll <= chance;
        }

        public static int DiceRoll(this System.Random random) {
            return random.Next(0, 101);
        }

        public static bool CoinFlip(this System.Random random) {
            return random.Next(0, 101) < 50;
        }

        private static FloatRange _standardYClamp = new FloatRange(-15, 15);

        public static Vector3 RandomSpherePosition(Vector3 center, float distance) {
            var pos = center + UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(distance * 0.5f, distance);
            pos.y = _standardYClamp.Clamp(pos.y);
            return pos;
        }

        public static Vector2 RandomCirclePosition(Vector2 center, float distance) {
            return center + UnityEngine.Random.insideUnitCircle * UnityEngine.Random.Range(distance * 0.5f, distance);
        }

        public static Vector2 RandomCirclePosition(Vector2 center, float distance, System.Random random) {
            var angle = random.NextDouble() * Math.PI * 2;
            var radius = Math.Sqrt(random.NextDouble()) * distance;
            var x = center.x + radius * Math.Cos(angle);
            var y = center.y + radius * Math.Sin(angle);
            return new Vector2((float) x, (float) y);
        }

        //public static IComparable<T> QuickSort<T>(this IComparable<T> list, int start, int end) {
        //    int lenght = end - start;
        //    if (lenght < 1) {
        //        return list;
        //    }
        //    int divider = start;
        //    int pivot = end - 1;
        //    for (int position = start; position < end; position++) {
        //    }
        //}
    }

    public static class Algorithms {
        private static void Swap<T>(ref T lhs, ref T rhs) {
            var temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        /// <summary>
        /// The plot function delegate
        /// </summary>
        /// <param name="x">The x co-ord being plotted</param>
        /// <param name="y">The y co-ord being plotted</param>
        /// <returns>True to continue, false to stop the algorithm</returns>
        public delegate bool PlotFunction(int x, int y);

        /// <summary>
        /// Plot the line stopping if plot returns false
        /// </summary>
        public static void Line(Point3 origin, Point3 dest, PlotFunction plot) {
            bool steep = Math.Abs(dest.z - origin.z) > Math.Abs(dest.x - origin.x);
            if (steep) {
                Swap<int>(ref origin.x, ref origin.z);
                Swap<int>(ref dest.x, ref dest.z);
            }
            if (origin.x > dest.x) {
                Swap<int>(ref origin.x, ref dest.x);
                Swap<int>(ref origin.z, ref dest.z);
            }
            int dX = (dest.x - origin.x);
            int dZ = Math.Abs(dest.z - origin.z);
            int err = (dX / 2);
            int ystep = (origin.z < dest.z ? 1 : -1), z = origin.z;
            for (int x = origin.x; x <= dest.x; ++x) {
                if (steep && !plot(z, x)) {
                    return;
                }
                if (!steep && !plot(x, z)) {
                    return;
                }
                err = err - dZ;
                if (err < 0) {
                    z += ystep;
                    err += dX;
                }
            }
        }

        public static bool CanReach(Point3 origin, Point3 dest) {
            bool steep = Math.Abs(dest.z - origin.z) > Math.Abs(dest.x - origin.x);
            if (steep) {
                Swap<int>(ref origin.x, ref origin.z);
                Swap<int>(ref dest.x, ref dest.z);
            }
            if (origin.x > dest.x) {
                Swap<int>(ref origin.x, ref dest.x);
                Swap<int>(ref origin.z, ref dest.z);
            }
            int dX = (dest.x - origin.x);
            int dZ = Math.Abs(dest.z - origin.z);
            int err = (dX / 2);
            int ystep = (origin.z < dest.z ? 1 : -1), z = origin.z;
            //var prev = origin;
            for (int x = origin.x; x <= dest.x; ++x) {
                //var pos = steep ? new Point3(z, 0, x) : new Point3(x, 0, z);
                err = err - dZ;
                if (err < 0) {
                    z += ystep;
                    err += dX;
                }
            }
            return true;
        }

        //Returns a position between 4 Vector3 with Catmull-Rom spline algorithm
        //http://www.iquilezles.org/www/articles/minispline/minispline.htm
        public static Vector3 GetCatmullRomPosition(float t, Vector3 pre, Vector3 start, Vector3 end, Vector3 post) {
            //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
            Vector3 a = 2f * start;
            Vector3 b = end - pre;
            Vector3 c = 2f * pre - 5f * start + 4f * end - post;
            Vector3 d = -pre + 3f * start - 3f * end + post;

            //The cubic polynomial: a + b * t + c * t^2 + d * t^3
            Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

            return pos;
        }
    }

    public static class CameraExtensions {
        public static Bounds OrthographicBounds(this Camera camera) {
            float screenAspect = (float) Screen.width / (float) Screen.height;
            float cameraHeight = camera.orthographicSize * 2;
            Bounds bounds = new Bounds(
                camera.transform.position,
                new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
            return bounds;
        }

        public static void FaceCamera(this Camera cam, Transform tr, bool backwards) {
            tr.LookAt(
                tr.position + cam.transform.rotation * (backwards ? Vector3.back : Vector3.forward),
                cam.transform.rotation * Vector3.up);
        }

        public static Ray EditorRaycast(this Camera current) {
            return current.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, Camera.current.pixelHeight - Event.current.mousePosition.y, 0.0f));
        }
    }

    public static class FloatExtension {
        public static float ToFloatPercentFromBase100(this float fltVal) {
            return fltVal * 0.01f;
        }
    }

    public static class IntExtension {

        public static float ToFloatPercentFromBase100(this int intVal) {
            return intVal * 0.01f;
        }

        public static string ToRomanNumeral(this int value) {
            if (value < 0) {
                throw new ArgumentOutOfRangeException("Please use a positive integer greater than zero.");
            }
            StringBuilder sb = new StringBuilder();
            int remain = value;
            while (remain > 0) {
                if (remain >= 1000) {
                    sb.Append("M");
                    remain -= 1000;
                }
                else if (remain >= 900) {
                    sb.Append("CM");
                    remain -= 900;
                }
                else if (remain >= 500) {
                    sb.Append("D");
                    remain -= 500;
                }
                else if (remain >= 400) {
                    sb.Append("CD");
                    remain -= 400;
                }
                else if (remain >= 100) {
                    sb.Append("C");
                    remain -= 100;
                }
                else if (remain >= 90) {
                    sb.Append("XC");
                    remain -= 90;
                }
                else if (remain >= 50) {
                    sb.Append("L");
                    remain -= 50;
                }
                else if (remain >= 40) {
                    sb.Append("XL");
                    remain -= 40;
                }
                else if (remain >= 10) {
                    sb.Append("X");
                    remain -= 10;
                }
                else if (remain >= 9) {
                    sb.Append("IX");
                    remain -= 9;
                }
                else if (remain >= 5) {
                    sb.Append("V");
                    remain -= 5;
                }
                else if (remain >= 4) {
                    sb.Append("IV");
                    remain -= 4;
                }
                else if (remain >= 1) {
                    sb.Append("I");
                    remain -= 1;
                }
                else {
                    throw new Exception("Unexpected error."); // <<-- shouldn't be possble to get here, but it ensures that we will never have an infinite loop (in case the computer is on crack that day).
                }
            }
            return sb.ToString();
        }
    }

    public static class GeometryExtensions {
        public static Vector3[] GetAllPoints(this Bounds bounds) {
            return new Vector3[] {
                new Vector3(bounds.min.x, bounds.min.y, bounds.min.z),
                new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
                new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
                new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
                new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
                new Vector3(bounds.max.x, bounds.max.y, bounds.max.z),
                new Vector3(bounds.max.x, bounds.max.y, bounds.min.z),
                new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
            };
        }

        public static Vector3[] GetAllPointsVec(this Bounds bounds) {
            return new Vector3[] {
                new Vector3(bounds.min.x, bounds.min.y, bounds.min.z),
                new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),

                new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
                new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),

                new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
                new Vector3(bounds.max.x, bounds.max.y, bounds.max.z),

                new Vector3(bounds.max.x, bounds.max.y, bounds.min.z),
                new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),


                new Vector3(bounds.min.x, bounds.min.y, bounds.min.z),
                new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),

                new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
                new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),

                new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
                new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),

                new Vector3(bounds.max.x, bounds.max.y, bounds.min.z),
                new Vector3(bounds.max.x, bounds.max.y, bounds.max.z),


                new Vector3(bounds.min.x, bounds.min.y, bounds.min.z),
                new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),

                new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
                new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),

                new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
                new Vector3(bounds.max.x, bounds.max.y, bounds.min.z),

                new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
                new Vector3(bounds.max.x, bounds.max.y, bounds.max.z),

            };
        }

        public static List<Vector3> MatrixMultiply(this List<Vector3> points, Matrix4x4 matrix = default(Matrix4x4)) {
            var result = new List<Vector3>(points.Count);
            for (int i = 0; i < points.Count; i++) {
                result.Add(matrix.MultiplyPoint(points[i]));
            }
            return result;
        }

        public static Bounds GetEnclosingBounds(this List<Vector3> points) {
            if (points.Count == 0) {
                return default(Bounds);
            }
            var bounds = new Bounds(points[0], Vector3.zero);
            for (var i = 1; i < points.Count; i++) {
                bounds.Encapsulate(points[i]);
            }
            return bounds;
        }
    }

    public static class RectTrExtensions {
        public static void SetAnchors(this RectTransform tr, Vector2 anchor) {
            tr.anchorMin = anchor;
            tr.anchorMax = anchor;
        }

        public static RectTransform Resize(this RectTransform transform, float percentage, float arcSize) {
            RectTransform parent = transform.parent as RectTransform;
            float arcPercentage = arcSize / parent.rect.width;
            transform.sizeDelta = new Vector2(parent.rect.width * MathEx.Min(percentage, arcPercentage), parent.rect.height * MathEx.Min(percentage, arcPercentage));
            return transform;
        }

        public static RectTransform AnchorsToCorners(this RectTransform transform) {
            RectTransform t = transform;
            RectTransform pt = t.parent as RectTransform;

            if (pt == null) {
                return transform;
            }

            Vector2 newAnchorsMin = new Vector2(
                t.anchorMin.x + t.offsetMin.x / pt.rect.width,
                t.anchorMin.y + t.offsetMin.y / pt.rect.height);
            Vector2 newAnchorsMax = new Vector2(
                t.anchorMax.x + t.offsetMax.x / pt.rect.width,
                t.anchorMax.y + t.offsetMax.y / pt.rect.height);

            t.anchorMin = newAnchorsMin;
            t.anchorMax = newAnchorsMax;
            t.offsetMin = t.offsetMax = new Vector2(0, 0);
            return t;
        }

        public static RectTransform CornersToAnchors(this RectTransform transform) {
            RectTransform t = transform as RectTransform;

            t.offsetMin = t.offsetMax = new Vector2(0, 0);

            return t;
        }

        public static Vector2 ViewportToWorldSpaceUI(this RectTransform transform, Vector3 pos, Vector3 dir) {
            Vector2 result = Vector2.zero;

            Vector3 localPos = transform.InverseTransformPoint(pos);
            Vector3 localDir = transform.InverseTransformDirection(dir);

            result = (Vector2) localPos + ((Vector2) localDir * (Mathf.Abs(localPos.z) / localDir.z));

            return result;
        }
    }

    public static class DictionaryExtensions {
        public static bool TryAdd<T, TV>(this Dictionary<T, TV> dict, T key, TV val) {
            if (dict.ContainsKey(key)) {
                return false;
            }
            dict.Add(key, val);
            return true;
        }

        public static TV GetOrAdd<T, TV>(this Dictionary<T, TV> dict, T key) where TV : new() {
            if (!dict.TryGetValue(key, out var value)) {
                value = new TV();
                dict.Add(key, value);
            }
            return value;
        }

        public static string EncodeToString(this Dictionary<string, string> dict) {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in dict) {
                sb.Append(pair.Key);
                sb.Append('-');
                sb.AppendEntryBreak(pair.Value);
            }
            return sb.ToString();
        }

        public static void DecodeFromString(this Dictionary<string, string> dict, string data) {
            var list = data.SplitMultiEntry();
            for (int i = 0; i < list.Count; i++) {
                var entry = list[i].Split('-');
                if (entry.Length < 2 || dict.ContainsKey(entry[0])) {
                    continue;
                }
                dict.Add(entry[0], entry[1]);
            }
        }

        public static T RetrieveEnum<T>(this Dictionary<string, string> dict, string key, T defaultVal) {
            string data;
            if (!dict.TryGetValue(key, out data)) {
                return defaultVal;
            }
            T val;
            return EnumHelper.TryParse(data, out val) ? val : defaultVal;
        }
        
        public static string Retrieve(this Dictionary<string, string> dict, string key, string defaultVal) {
            string data;
            return !dict.TryGetValue(key, out data) ? defaultVal : data;
        }

        public static Color Retrieve(this Dictionary<string, string> dict, string key, Color defaultVal) {
            string data;
            if (!dict.TryGetValue(key, out data)) {
                return defaultVal;
            }
            Color color;
            return ColorUtility.TryParseHtmlString(data, out color) ? color : defaultVal;
        }

        public static int Retrieve(this Dictionary<string, string> dict, string key, int defaultVal) {
            string data;
            if (!dict.TryGetValue(key, out data)) {
                return defaultVal;
            }
            int val;
            return int.TryParse(data, out val) ? val : defaultVal;
        }

        public static float Retrieve(this Dictionary<string, string> dict, string key, float defaultVal) {
            string data;
            if (!dict.TryGetValue(key, out data)) {
                return defaultVal;
            }
            float val;
            return float.TryParse(data, out val) ? val : defaultVal;
        }

        public static bool Retrieve(this Dictionary<string, string> dict, string key, bool defaultVal) {
            string data;
            if (!dict.TryGetValue(key, out data)) {
                return defaultVal;
            }
            bool val;
            return bool.TryParse(data, out val) ? val : defaultVal;
        }

        public static void Encode(this Dictionary<string, string> dict, string key, string value) {
            if (dict.ContainsKey(key)) {
                dict[key] = value;
            }
            else {
                dict.Add(key, value);
            }
        }

        public static void Encode(this Dictionary<string, string> dict, string key, Color color) {
            if (dict.ContainsKey(key)) {
                dict[key] = ColorUtility.ToHtmlStringRGBA(color);
            }
            else {
                dict.Add(key, ColorUtility.ToHtmlStringRGBA(color));
            }
        }

        public static void EncodeEnum<T>(this Dictionary<string, string> dict, string key, int value) where T : struct, IConvertible {
            if (dict.ContainsKey(key)) {
                dict[key] = EnumHelper.GetString<T>(value);
            }
            else {
                dict.Add(key, EnumHelper.GetString<T>(value));
            }
            
        }
    }

    public static class SerializationExtensions {
        
        public static T GetValue<T>(this SerializationInfo self, string name, T currentValue) {
            //return (T) self.GetValue(name, typeof(T));
            T value;
            try {
                value = (T) self.GetValue(name, typeof(T));
            }
            catch (Exception e) {
                Debug.LogFormat("Name {0} {1} {2}", name, e.TargetSite.ToString(), e.StackTrace);
                value = currentValue;
            }
            return value;
        }

        public static Color GetValue(this SerializationInfo self, string name, Color currentValue) {
            //return (T) self.GetValue(name, typeof(T));
            Color value;
            try {
                string id = (string) self.GetValue(name, typeof(string));
                if (id.Length > 0 && id[0] != '#') {
                    id = id.Insert(0, "#");
                }
                if (!ColorUtility.TryParseHtmlString(id, out value)) {
                    value = currentValue;
                }
            }
            catch (Exception e) {
                Debug.LogFormat("Name {0} {1} {2}", name, e.TargetSite.ToString(), e.StackTrace);
                value = currentValue;
            }
            return value;
        }

        public static void AddValue(this SerializationInfo self, string name, Color color) {
            self.AddValue(name, ColorUtility.ToHtmlStringRGBA(color), typeof(string));
        }

        public static void AddColor(this SerializationInfo self, string name, Color color) {
            self.AddValue(name, ColorUtility.ToHtmlStringRGBA(color), typeof(string));
        }

        public static T Deserialize<T>(this SerializationInfo info, T val) {
            string name = "";
            try {
                name = nameof(val);
            }
            catch (Exception e) {
                Debug.LogFormat("{0} {1} {2}", e.Source, e.TargetSite.ToString(), e.StackTrace);
                throw;
            }
            T value = default(T);
            try {
                value = (T) info.GetValue(name, typeof(T));
            }
            catch (Exception e) {
                Debug.LogFormat("Name {0} {1} {2}", name, e.TargetSite.ToString(), e.StackTrace);
                throw;
            }
            //return (T) info.GetValue(name, typeof(T));
            return value;
        }
    }

    public static class TrExtensions {

        //public static void DeleteChildren(this Transform tr) {
        //    var children = new List<GameObject>();
        //    foreach (Transform child in tr) {
        //        children.Add(child.gameObject);
        //    }
        //    for (var i = 0; i < children.Count; i++) {
        //        DestroyImmediate(children[i]);
        //    }
        //}

        public static void SetParentResetPos(this Transform child, Transform parent) {
            child.SetParent(parent);
            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.identity;
        }

        public static void ResetPos(this Transform child) {
            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.identity;
        }

        public static Vector3 Direction(this Transform tr, Directions dir) {
            switch (dir) {
                case Directions.Forward:
                    return tr.forward;
                case Directions.Right:
                    return tr.right;
                case Directions.Back:
                    return -tr.forward;
                case Directions.Left:
                    return -tr.right;
                case Directions.Up:
                    return tr.up;
                case Directions.Down:
                    return -tr.up;
            }
            return tr.forward;
        }

        public static Directions ForwardDirection(this Transform tr) {
            var v = tr.forward;
            v.y = 0;
            v.Normalize();
            if (Vector3.Angle(v, Vector3.forward) <= 45.0) {
                return Directions.Forward;
            }
            if (Vector3.Angle(v, Vector3.right) <= 45.0) {
                return Directions.Right;
            }
            if (Vector3.Angle(v, Vector3.back) <= 45.0) {
                return Directions.Back;
            }
            if (Vector3.Angle(v, Vector3.left) <= 45.0) {
                return Directions.Left;
            }
            v.y = tr.forward.y;
            if (Vector3.Angle(v, Vector3.up) <= 45.0) {
                return Directions.Up;
            }
            return Directions.Down;
        }

        public static Directions ForwardDirection2D(this Transform tr) {
            var v = tr.forward;
            v.y = 0;
            v.Normalize();
            if (Vector3.Angle(v, Vector3.forward) <= 45.0) {
                return Directions.Forward;
            }
            if (Vector3.Angle(v, Vector3.right) <= 45.0) {
                return Directions.Right;
            }
            if (Vector3.Angle(v, Vector3.back) <= 45.0) {
                return Directions.Back;
            }
            return Directions.Left;
        }

        public static void DeleteChildren(this Transform tr) {
            int childCount = tr.childCount;
            for (int i = 0; i < childCount; i++) {
#if UNITY_EDITOR
                if (Application.isPlaying) {
                    UnityEngine.Object.Destroy(tr.GetChild(0).gameObject);
                }
                else {
                    UnityEngine.Object.DestroyImmediate(tr.GetChild(0).gameObject);
                }
#else
            UnityEngine.Object.Destroy(tr.GetChild(0).gameObject);
#endif
            }
        }

        public static void DespawnChildren(this Transform tr) {
            int childCount = tr.childCount;
            for (int i = 0; i < childCount; i++) {
                ItemPool.Despawn(tr.GetChild(0).gameObject);
            }
        }

        public static Quaternion GetLookAtRotation(this Transform me, Transform target) {
            var targetPos = target.position;
            targetPos.y = me.position.y;
            Vector3 relativePos = targetPos - me.position;
            return Quaternion.LookRotation(relativePos);
        }
    }

    public static class RigidbodyExtensions {

        public static void ApplyDamageForce(this Rigidbody rb, Vector3 dir, float physAmt) {
            rb.AddForce(dir * physAmt, ForceMode.Impulse);
        }

        public static void SetPhysics(this Rigidbody rb, bool enabled) {
            rb.isKinematic = !enabled;
            rb.useGravity = enabled;
        }

        public static Vector3 CalculateThrowVelocity(Vector3 origin, Vector3 target, float timeToTarget) {
            // calculate vectors
            Vector3 toTarget = target - origin;
            Vector3 toTargetXZ = toTarget;
            toTargetXZ.y = 0;

            // calculate xz and y
            float y = toTarget.y;
            float xz = toTargetXZ.magnitude;

            // calculate starting speeds for xz and y. Physics forumulase deltaX = v0 * t + 1/2 * a * t * t
            // where a is "-gravity" but only on the y plane, and a is 0 in xz plane.
            // so xz = v0xz * t => v0xz = xz / t
            // and y = v0y * t - 1/2 * gravity * t * t => v0y * t = y + 1/2 * gravity * t * t => v0y = y / t + 1/2 * gravity * t
            float t = timeToTarget;
            float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
            float v0xz = xz / t;

            // create result vector for calculated starting speeds
            Vector3 result = toTargetXZ.normalized; // get direction of xz but with magnitude 1
            result *= v0xz; // set magnitude of xz to v0xz (starting speed in xz plane)
            result.y = v0y; // set y to v0y (starting speed of y plane)

            return result;
        }

        //public static Vector3 CalculateVelocity(Vector3 origin, Vector3 target, float angle = 20f) {
        //    var range = Vector3.Distance(origin, target);
        //    Vector3 v = new Vector3(0, Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));
        //    return v * Mathf.Sqrt(range * Physics.gravity.magnitude / Mathf.Sin(2*angle*Mathf.Deg2Rad));
        //}

        public static Vector3 CalculateVelocity(Vector3 origin, Vector3 target, float initialAngle) {

            float gravity = Physics.gravity.magnitude;
            // Selected angle in radians
            float angle = initialAngle * Mathf.Deg2Rad;

            // Positions of this object and the target on the same plane
            Vector3 planarTarget = new Vector3(target.x, 0, target.z);
            Vector3 planarPostion = new Vector3(origin.x, 0, origin.z);

            // Planar distance between objects
            float distance = Vector3.Distance(planarTarget, planarPostion);
            // Distance along the y axis between objects
            float yOffset = origin.y - target.y;

            float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt(
                                        (0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) +
                                                                                     yOffset));

            Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

            // Rotate our velocity to match the direction between the two objects
//        float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion);
            float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion) * (target.x > origin.x ? 1 : -1);

            Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;
            return finalVelocity;
            // Alternative way:
            // rigid.AddForce(finalVelocity * rigid.mass, ForceMode.Impulse);
        }
        
    }

    [System.Serializable]
    public struct Vector3Pair {
        public readonly Vector3 Origin;
        public readonly Vector3 Target;

        public Vector3Pair(Vector3 origin, Vector3 target) {
            Origin = origin;
            Target = target;
        }
    }

    public static class VIntExtensions {
        public static Vector3 GenericGridToWorld(this Vector2Int position, float gridSize) {
            return new Vector3(position.x * gridSize, 0, position.y * gridSize);
        }

        public static Vector3 GenericGridToWorld(this Vector3Int position, float gridSize) {
            return new Vector3(position.x * gridSize, position.y * gridSize, position.z * gridSize);
        }

        public static Vector3Int WorldToGenericGridInt3(Vector3 position, float gridSize) {
            return new Vector3Int(
                (int) Math.Round((double) position.x / gridSize),
                (int) Math.Round((double) position.y / gridSize),
                (int) Math.Round((double) position.z / gridSize));
        }

        public static Vector2Int WorldToGenericGridInt2(Vector3 position, float gridSize) {
            return new Vector2Int(
                (int) Math.Round((double) position.x / gridSize),
                (int) Math.Round((double) position.z / gridSize));
        }
    }

    public static class V3Extensions {

        public static Point3 toPoint3(this Vector3 v3) {
            return new Point3(v3);
        }

        public static float ManualDot(Vector3 a, Vector3 b) {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static float ManualSqrMagnitude(Vector3 v) {
            return v.x * v.x + v.y * v.y + v.z * v.z;
        }

        public static Vector3 Multiply(this Vector3 v3, Vector3 other) {
            return new Vector3(v3.x * other.x, v3.y * other.y, v3.z * other.z);
        }

        public static Directions GetDirection(this Vector3 t1, Vector3 t2) {
            if (t1.z < t2.z) {
                return Directions.Forward;
            }

            if (t1.x < t2.x) {
                return Directions.Right;
            }

            if (t1.z > t2.z) {
                return Directions.Back;
            }

            return Directions.Left;
        }

        public static Vector3 Clamp(this Vector3 v3, float min, float max) {
            for (int i = 0; i < 3; i++) {
                v3[i] = Mathf.Clamp(v3[i], min, max);
            }
            return v3;
        }

        public static float EuclideanDistance(this Vector3 t1, Vector3 t2) {
            return Mathf.Abs(t1.x - t2.x) + Mathf.Abs(t1.y - t2.y) + Mathf.Abs(t1.z - t2.z);
        }

        public static float DistanceChebyshev(this Vector3 t1, Vector3 t2) {
            return MathEx.Max(Mathf.Abs(t1.x - t2.x), Mathf.Abs(t1.z - t2.z));
        }

        public static float SqrDistanceXz(this Vector3 t1, Vector3 t2) {
            return new Vector3(t2.x - t1.x, 0, t2.z - t1.z).sqrMagnitude;
        }

        public static float SqrDistance(this Vector3 t1, Vector3 t2) {
            return new Vector3(t2.x - t1.x, t2.y - t1.y, t2.z - t1.z).sqrMagnitude;
        }

        public static float SqrDistance(this Vector3 t1, Point3 t2) {
            return new Vector3(t2.x - t1.x, t2.y - t1.y, t2.z - t1.z).sqrMagnitude;
        }

        public static float DistanceSquared(this Vector3 a, Vector3 b) {
            float dx = b.x - a.x;
            float dy = b.y - a.y;
            float dz = b.z - a.z;
            return dx * dx + dy * dy + dz * dz;
        }

        public static Directions EulerToDirection(this Vector3 eulerRot) {
            var angle = eulerRot.y;
            if (angle >= -45 && angle < 45) {
                return Directions.Forward;
            }
            if (angle >= 45 && angle < 135) {
                return Directions.Right;
            }
            if (angle >= 135 && angle < 225) {
                return Directions.Back;
            }
            if (angle >= 225 && angle < 315) {
                return Directions.Left;
            }
            if (angle > 315) {
                return Directions.Forward;
            }
            return Directions.Left;
        }


        public static Point3 WorldToGenericGrid(this Vector3 position, float gridSize) {
            return new Point3(
                (int) Math.Round((double) position.x / gridSize),
                (int) Math.Round((double) position.y / gridSize),
                (int) Math.Round((double) position.z / gridSize));
        }

        public static Vector3 Rounded(this Vector3 position, float amount) {
            return new Vector3(
                Mathf.Round(position.x / amount) * amount,
                Mathf.Round(position.y / amount) * amount,
                Mathf.Round(position.z / amount) * amount);
        }

        //public override string ToStringSimple() {
        //    return UnityString.Format("({0:F1}, {1:F1}, {2:F1})", new object[]
        //    {
        //            this.x,
        //            this.y,
        //            this.z
        //    });
        //}

        public static float XZSqrMagnitude(Vector3 a, Vector3 b) {
            float dx = b.x - a.x;
            float dz = b.z - a.z;
            return dx * dx + dz * dz;
        }

        public static float SqrMagnitude(Vector3 a, Vector3 b) {
            return (a - b).sqrMagnitude;
        }

        public static float AbsDistance(Vector3 p1, Vector3 p2) {
            return Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.y - p2.y) + Mathf.Abs(p1.z - p2.z);
        }

        public static float AbsDistance(Point3 p1, Point3 p2) {
            return Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.y - p2.y) + Mathf.Abs(p1.z - p2.z);
        }

    }

    public static class DirectionsExtensions {
        public static Directions OppositeDir(this Directions dir) {
            if (dir == Directions.Forward) {
                return Directions.Back;
            }
            if (dir == Directions.Right) {
                return Directions.Left;
            }
            if (dir == Directions.Back) {
                return Directions.Forward;
            }
            if (dir == Directions.Up) {
                return Directions.Down;
            }
            if (dir == Directions.Down) {
                return Directions.Up;
            }
            return Directions.Right;
        }

        public static DirectionsEight ToDirectionEight(this Directions dir) {
            if (dir == Directions.Forward) {
                return DirectionsEight.Front;
            }
            if (dir == Directions.Right) {
                return DirectionsEight.Right;
            }
            if (dir == Directions.Left) {
                return DirectionsEight.Left;
            }
            if (dir == Directions.Back) {
                return DirectionsEight.Rear;
            }
            if (dir == Directions.Up) {
                return DirectionsEight.Top;
            }
            if (dir == Directions.Down) {
                return DirectionsEight.Bottom;
            }
            return DirectionsEight.Front;
        }

        public static Directions ToCardinalDir(this DirectionsEight dir) {
            switch (dir) {
                case DirectionsEight.Front:
                    return Directions.Forward;
                case DirectionsEight.Right:
                    return Directions.Right;
                case DirectionsEight.Left:
                    return Directions.Left;
                case DirectionsEight.Rear:
                    return Directions.Back;
                case DirectionsEight.Top:
                    return Directions.Up;
                case DirectionsEight.Bottom:
                    return Directions.Down;
            }
            return Directions.Forward;
        }

        public static DirectionsEight OppositeDir(this DirectionsEight dir) {
            if (dir == DirectionsEight.Front) {
                return DirectionsEight.Rear;
            }
            if (dir == DirectionsEight.Right) {
                return DirectionsEight.Left;
            }
            if (dir == DirectionsEight.Left) {
                return DirectionsEight.Right;
            }
            if (dir == DirectionsEight.Rear) {
                return DirectionsEight.Front;
            }
            if (dir == DirectionsEight.Top) {
                return DirectionsEight.Bottom;
            }
            if (dir == DirectionsEight.Bottom) {
                return DirectionsEight.Top;
            }
            if (dir == DirectionsEight.FrontRight) {
                return DirectionsEight.RearLeft;
            }
            if (dir == DirectionsEight.RearLeft) {
                return DirectionsEight.FrontRight;
            }
            if (dir == DirectionsEight.RearRight) {
                return DirectionsEight.FrontLeft;
            }
            if (dir == DirectionsEight.FrontLeft) {
                return DirectionsEight.RearRight;
            }
            return DirectionsEight.Front;
        }

        public static Directions[] ShuffledArray = new Directions[] {
            Directions.Forward, Directions.Right, Directions.Back, Directions.Left, Directions.Up, Directions.Down,
        };

        public static Directions[] ShuffledArray2D = new Directions[] {
            Directions.Forward, Directions.Right, Directions.Back, Directions.Left
        };

        public static Directions[] GetDiagonalCheckDirs(Directions dir) {
            switch (dir) {
                default:
                case Directions.Forward:
                    return new Directions[] {
                        Directions.Left, Directions.Forward
                    };
                case Directions.Right:
                    return new Directions[] {
                        Directions.Right, Directions.Forward
                    };
                case Directions.Back:
                    return new Directions[] {
                        Directions.Right, Directions.Back
                    };
                case Directions.Left:
                    return new Directions[] {
                        Directions.Left, Directions.Back
                    };
            }
        }

        public static Directions[] Adjacent(this Directions dir) {
            switch (dir) {
                default:
                case Directions.Forward:
                case Directions.Back:
                    return new Directions[] {
                        Directions.Left, Directions.Right
                    };
                case Directions.Right:
                case Directions.Left:
                    return new Directions[] {
                        Directions.Forward, Directions.Back
                    };
            }
        }

        public static DirectionsEight[] Adjacent(this DirectionsEight dir) {
            switch (dir) {
                default:
                case DirectionsEight.Front:
                    return new DirectionsEight[] {
                        DirectionsEight.FrontLeft, DirectionsEight.FrontRight,
                    };
                case DirectionsEight.FrontRight:
                    return new DirectionsEight[] {
                        DirectionsEight.Front, DirectionsEight.Right,
                    };
                case DirectionsEight.Right:
                    return new DirectionsEight[] {
                        DirectionsEight.FrontRight, DirectionsEight.RearRight,
                    };
                case DirectionsEight.RearRight:
                    return new DirectionsEight[] {
                        DirectionsEight.Right, DirectionsEight.Rear,
                    };
                case DirectionsEight.Rear:
                    return new DirectionsEight[] {
                        DirectionsEight.RearRight, DirectionsEight.RearLeft,
                    };
                case DirectionsEight.RearLeft:
                    return new DirectionsEight[] {
                        DirectionsEight.Rear, DirectionsEight.Left,
                    };
                case DirectionsEight.Left:
                    return new DirectionsEight[] {
                        DirectionsEight.FrontLeft, DirectionsEight.RearLeft,
                    };
                case DirectionsEight.FrontLeft:
                    return new DirectionsEight[] {
                        DirectionsEight.Front, DirectionsEight.Left,
                    };
            }
        }

        public static bool IsAdjacent(this DirectionsEight dir, DirectionsEight compare) {
            switch (dir) {
                default:
                case DirectionsEight.Front:
                    return compare == DirectionsEight.FrontLeft || compare == DirectionsEight.FrontRight;
                case DirectionsEight.FrontRight:
                    return compare == DirectionsEight.Front || compare == DirectionsEight.Right;
                case DirectionsEight.Right:
                    return compare == DirectionsEight.FrontRight || compare == DirectionsEight.RearRight;
                case DirectionsEight.RearRight:
                    return compare == DirectionsEight.Right || compare == DirectionsEight.Rear;
                case DirectionsEight.Rear:
                    return compare == DirectionsEight.RearRight || compare == DirectionsEight.RearLeft;
                case DirectionsEight.RearLeft:
                    return compare == DirectionsEight.Rear || compare == DirectionsEight.Left;
                case DirectionsEight.Left:
                    return compare == DirectionsEight.FrontLeft || compare == DirectionsEight.RearLeft;
                case DirectionsEight.FrontLeft:
                    return compare == DirectionsEight.Front || compare == DirectionsEight.Left;
            }
        }

        public static bool IsCardinal(this DirectionsEight dir) {
            switch (dir) {
                case DirectionsEight.Front:
                case DirectionsEight.Right:
                case DirectionsEight.Left:
                case DirectionsEight.Rear:
                    return true;
            }
            return false;
        }

        public static bool Is2D(this DirectionsEight dir) {
            switch (dir) {
                case DirectionsEight.Top:
                case DirectionsEight.Bottom:
                    return false;
            }
            return true;
        }

        public static Directions[] GetShuffledDirectionsArray(Directions lastDir) {
            ShuffledArray.Shuffle();
            for (int i = 0; i < ShuffledArray.Length - 1; i++) {
                if (ShuffledArray[i] == lastDir) {
                    ShuffledArray[i] = ShuffledArray[ShuffledArray.Length - 1];
                    ShuffledArray[ShuffledArray.Length - 1] = lastDir;
                }
            }
            return ShuffledArray;
        }

        public static bool Is2D(this Directions dir) {
            if (dir == Directions.Up || dir == Directions.Down) {
                return false;
            }
            return true;
        }

        public static Directions ChangeFwdDirection(this Directions dir, int newDir) {
            if (newDir == 0) {
                return dir;
            }
            switch (dir) {
                case Directions.Forward:
                    switch (newDir) {
                        case 1:
                            return Directions.Right;
                        case 2:
                            return Directions.Left;
                        case 3:
                            return Directions.Up;
                        default:
                            return Directions.Down;
                    }
                case Directions.Right:
                    switch (newDir) {
                        case 1:
                            return Directions.Back;
                        case 2:
                            return Directions.Forward;
                        case 3:
                            return Directions.Up;
                        default:
                            return Directions.Down;
                    }
                case Directions.Back:
                    switch (newDir) {
                        case 1:
                            return Directions.Left;
                        case 2:
                            return Directions.Right;
                        case 3:
                            return Directions.Up;
                        default:
                            return Directions.Down;
                    }
                case Directions.Left:
                    switch (newDir) {
                        case 1:
                            return Directions.Forward;
                        case 2:
                            return Directions.Back;
                        case 3:
                            return Directions.Up;
                        default:
                            return Directions.Down;
                    }
                case Directions.Up:
                    switch (newDir) {
                        case 1:
                            return Directions.Right;
                        case 2:
                            return Directions.Left;
                        case 3:
                            return Directions.Back;
                        default:
                            return Directions.Forward;
                    }
                case Directions.Down:
                    switch (newDir) {
                        case 1:
                            return Directions.Left;
                        case 2:
                            return Directions.Right;
                        case 3:
                            return Directions.Back;
                        default:
                            return Directions.Forward;
                    }
            }
            return dir;
        }

        public static Directions TransformDir(this Directions dir, Directions newDir) {
            switch (dir) {
                case Directions.Forward:
                    switch (newDir) {
                        case Directions.Right:
                            return Directions.Right;
                        case Directions.Left:
                            return Directions.Left;
                        case Directions.Back:
                            return Directions.Back;
                    }
                    return Directions.Forward;
                case Directions.Right:
                    switch (newDir) {
                        case Directions.Right:
                            return Directions.Back;
                        case Directions.Left:
                            return Directions.Forward;
                        case Directions.Back:
                            return Directions.Left;
                    }
                    return Directions.Right;
                case Directions.Back:
                    switch (newDir) {
                        case Directions.Right:
                            return Directions.Left;
                        case Directions.Left:
                            return Directions.Right;
                        case Directions.Back:
                            return Directions.Forward;
                    }
                    return Directions.Back;
                case Directions.Left:
                    switch (newDir) {
                        case Directions.Right:
                            return Directions.Forward;
                        case Directions.Left:
                            return Directions.Back;
                        case Directions.Back:
                            return Directions.Right;
                    }
                    return Directions.Left;
            }
            return dir;
        }

        public static Vector3 ToV3(this Directions dir) {
            switch (dir) {
                case Directions.Forward:
                    return Vector3.forward;
                case Directions.Right:
                    return Vector3.right;
                case Directions.Back:
                    return Vector3.back;
                case Directions.Left:
                    return Vector3.left;
                case Directions.Up:
                    return Vector3.up;
                case Directions.Down:
                    return Vector3.down;
            }
            return Vector3.forward;
        }

        public static float ToSimpleAngle(this Directions dir) {
            switch (dir) {
                case Directions.Forward:
                    return 0;
                case Directions.Right:
                    return 90;
                case Directions.Back:
                    return 180;
                case Directions.Left:
                    return -90;
            }
            return 0;
        }

        public static Point3 ToPoint3(this Directions dir) {
            switch (dir) {
                case Directions.Forward:
                    return Point3.forward;
                case Directions.Right:
                    return Point3.right;
                case Directions.Back:
                    return Point3.back;
                case Directions.Left:
                    return Point3.left;
                case Directions.Up:
                    return Point3.up;
                case Directions.Down:
                    return Point3.down;
            }
            return Point3.forward;
        }

        public static DirectionsEight EulerToDirectionEight(this Vector3 eulerRot, bool helpCardinal = false) {
            var angle = eulerRot.y;
            var halfRange = 45 / 2;
            for (int i = 0; i < _eulerEight.Length; i++) {
                var dirRange = halfRange;
                if (helpCardinal && ((DirectionsEight) i).IsCardinal()) {
                    dirRange += 5;
                }
                var center = _eulerEight[i].y;
                var bottom = center - dirRange;
                var top = center + dirRange;
                if (angle >= bottom && angle <= top) {
                    return (DirectionsEight) i;
                }
            }
            for (int i = 4; i < _eulerEightNeg.Length; i++) {
                var dirRange = halfRange;
                if (helpCardinal && ((DirectionsEight) i).IsCardinal()) {
                    dirRange += 5;
                }
                var center = _eulerEightNeg[i].y;
                var bottom = center - dirRange;
                var top = center + dirRange;
                if (angle >= bottom && angle <= top ||
                    (angle >= top && angle <= bottom)) {
                    return ((DirectionsEight) i);
                }
            }
            return DirectionsEight.Front;
        }

        public static int Length = 6;
        public static int Length2D = 4;
        public static int DiagonalLength = 8;
        public static int DiagonalLength3D = 10;

        public static Vector3 ToEuler(this Directions d) {
            //return new Vector3(0, (int)d * 90, 0);
            return EulerRotations[(int) d];
        }

        public static Quaternion ToEulerRot(this Directions d) {
            //return new Vector3(0, (int)d * 90, 0);
            return Quaternion.Euler(EulerRotations[(int) d]);
        }

        //-90 or 
        public static Vector3[] EulerRotations = new Vector3[] {
            new Vector3(0, 0, 0), new Vector3(0, 90, 0), new Vector3(0, 180, 0), new Vector3(0, 270, 0),
            new Vector3(90, 0, 0), new Vector3(-90, 0, 0)
        };

        private static Color[] _dirEightColors = new Color[10] {
            Color.green, new Color32(0, 255, 255, 255), Color.blue, new Color32(128,0,255,255), 
            new Color32(191,0,255,255), new Color32(255,0,255,255), new Color32(255,0,0,255), new Color32(255,191,0,255),
            Color.yellow, Color.white
        };

        public static Color ToColor(this DirectionsEight dir) {
            return _dirEightColors[(int) dir];
        }

        private static Vector3[] _eulerEight = new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(0, 45, 0), new Vector3(0, 90, 0), new Vector3(0, 135, 0),
            new Vector3(0, 180, 0),
            new Vector3(0, 225, 0), new Vector3(0, 270, 0), new Vector3(0, 315, 0)
        };

        private static Vector3[] _eulerEightNeg = new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0),
            new Vector3(0, -180, 0),
            new Vector3(0, -135, 0), new Vector3(0, -90, 0), new Vector3(0, -45, 0)
        };

        public static Quaternion ToEulerRot(this DirectionsEight d) {
            return Quaternion.Euler(_eulerEight[(int) d]);
        }

        private static string[] _toCardinal = new string[] {
            "North", "East", "South", "West", "Up", "Down"
        };

        public static string ToCardinalString(this Directions dir) {
            return _toCardinal[(int) dir];
        }

        public static Directions GetTravelDirTo(this Point3 origin, Point3 newpos) {
            if (origin.z < newpos.z) {
                return Directions.Forward;
            }

            if (origin.x < newpos.x) {
                return Directions.Right;
            }

            if (origin.z > newpos.z) {
                return Directions.Back;
            }

            if (origin.y < newpos.y) {
                return Directions.Up;
            }
            if (origin.y > newpos.y) {
                return Directions.Down;
            }
            return Directions.Left;
        }

        public static Directions ToDirection(this Point3 dir) {
            if (dir.z > 0 && dir.x == 0) {
                return Directions.Forward;
            }

            if (dir.z < 0 && dir.x == 0) {
                return Directions.Back;
            }

            if (dir.x > 0 && dir.z == 0) {
                return Directions.Right;
            }

            return Directions.Left;
        }

        private static Point3[] _diagonalPosition = {
            new Point3(0, 0, 1),
            new Point3(1, 0, 1), new Point3(1, 0, 0), new Point3(1, 0, -1),
            new Point3(0, 0, -1),
            new Point3(-1, 0, -1), new Point3(-1, 0, 0), new Point3(-1, 0, 1),
            new Point3(0, 1, 0), new Point3(0, -1, 0)
        };

        public static Point3 ToP3(this DirectionsEight dir) {
            return _diagonalPosition[(int) dir];
        }

        private static Vector3[] _diagonalPositionV3 = {
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1), new Vector3(1, 0, 0), new Vector3(1, 0, -1),
            new Vector3(0, 0, -1),
            new Vector3(-1, 0, -1), new Vector3(-1, 0, 0), new Vector3(-1, 0, 1),
            new Vector3(0, 1, 0), new Vector3(0, -1, 0)
        };

        public static Vector3 ToV3(this DirectionsEight dir) {
            return _diagonalPositionV3[(int) dir];
        }
    }

    
    public static class ActionDistanceExtensions {
        
        public static float ToFloat(this ActionDistance distance) {
            return RpgSystem.ActionDistances[(int) distance];
        }
    }

    public static class ActionExtensions {
        public static void SafeInvoke(this System.Action callback) {
            if (callback != null) {
                callback();
            }
        }

        public static void SafeInvoke<T>(this System.Action<T> callback, T param) {
            if (callback != null) {
                callback(param);
            }
        }
    }

    public static class RaycastHitExtensions {
        public static void SortByDistanceDesc(this RaycastHit[] rayHits, int hitLimit) {
            for (int write = 0; write < hitLimit; write++) {
                for (int sort = 0; sort < hitLimit - 1; sort++) {
                    if (rayHits[sort].distance < rayHits[sort + 1].distance) {
                        var greater = rayHits[sort + 1];
                        rayHits[sort + 1] = rayHits[sort];
                        rayHits[sort] = greater;
                    }
                }
            }
        }

        public static void SortByDistanceAsc(this RaycastHit[] rayHits, int hitLimit) {
            for (int write = 0; write < hitLimit; write++) {
                for (int sort = 0; sort < hitLimit - 1; sort++) {
                    if (rayHits[sort].distance > rayHits[sort + 1].distance) {
                        var lesser = rayHits[sort + 1];
                        rayHits[sort + 1] = rayHits[sort];
                        rayHits[sort] = lesser;
                    }
                }
            }
        }

        public static void SortByDistanceAsc(this List<Entity> list, Point3 center) {
            for (int write = 0; write < list.Count; write++) {
                for (int sort = 0; sort < list.Count - 1; sort++) {
                    if (list[sort].Get<GridPosition>().Position.SqrDistance(center) > list[sort + 1].Get<GridPosition>().Position.SqrDistance(center)) {
                        var lesser = list[sort + 1];
                        list[sort + 1] = list[sort];
                        list[sort] = lesser;
                    }
                }
            }
        }

        public static void SortByDistanceAsc(this List<WatchTarget> list, Point3 center) {
            for (int write = 0; write < list.Count; write++) {
                for (int sort = 0; sort < list.Count - 1; sort++) {
                    if (list[sort].LastSensedPos.SqrDistance(center) > list[sort + 1].LastSensedPos.SqrDistance(center)) {
                        var lesser = list[sort + 1];
                        list[sort + 1] = list[sort];
                        list[sort] = lesser;
                    }
                }
            }
        }
    }

    public static class Point3Extensions {
        public static int DistanceSquared(this Point3 a, Point3 b) {
            int dx = b.x - a.x;
            int dy = b.y - a.y;
            int dz = b.z - a.z;
            return dx * dx + dy * dy + dz * dz;
        }

        public static int XZDistanceSquared(this Point3 a, Point3 b) {
            int dx = b.x - a.x;
            int dz = b.z - a.z;
            return dx * dx + dz * dz;
        }


        public static Point3 Reverse(this Point3 pos) {
            Point3 newPos = Point3.zero;
            for (int i = 0; i < 3; i++) {
                if (newPos[i] == 0) {
                    continue;
                }
                newPos[i] = pos[i] * -1;
            }
            return newPos;
        }

        public static Vector3 GenericGridToWorld(this Point3 position, float gridSize) {
            return new Vector3(position.x * gridSize, position.y * gridSize, position.z * gridSize);
        }

        public static bool OnAxis2D(this Point3 p, Point3 other) {
            if (p.z == other.z || p.x == other.x) {
                return true;
            }
            return false;
        }

        public static int TileDistance2D(this Point3 p, Point3 other) {
            return Mathf.Abs(p.x - other.x) + Mathf.Abs(p.z - other.z);
        }
    }

    public static class CustomStringExtension {
        public static bool CompareCaseInsensitive(this string data, string other) {
            return string.Equals(data, other, StringComparison.OrdinalIgnoreCase);
        }

        public static List<string> SplitMultiEntry(this string targetLine) {
            return StringUtilities.SplitString(targetLine, StringConst.MultiEntryBreak);
        }

        public static List<string>[] SplitLinesIntoData(this string text, char splitChar) {
            return StringUtilities.SplitStringWithLines(text, splitChar);
        }

        public static List<string>[] SplitLinesIntoData(this string text) {
            return StringUtilities.SplitStringWithLines(text, StringConst.MultiEntryBreak);
        }

        public static string[] SplitIntoLines(this string text) {
            return text.Split('\n');
        }

        public static string EncodeWithEntryBreak(this IList<string> text) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Count; i++) {
                sb.AppendEntryBreak(text[i]);
            }
            return sb.ToString();
        }
    }

    public class Point3Comparer : IEqualityComparer<Point3> {
        public int GetHashCode(Point3 p) {
            unchecked {
                return (p.x.GetHashCode() * 397) ^ p.z.GetHashCode() ^ p.y.GetHashCode();
            }
            //return p.x ^ p.y << 2 ^ p.z >> 2;
            //return p.x.GetHashCode() ^ p.y.GetHashCode() << 2 ^ p.z.GetHashCode() >> 2;
            //return p.x + p.y + p.z;
        }

        public bool Equals(Point3 p1, Point3 p2) {
            return p1.x == p2.x &&
                   p1.y == p2.y &&
                   p1.z == p2.z;
        }
    }

    public static class QuanternionExtensions {
        public static Vector3 RotatePointAroundPivot(this Quaternion angles, Vector3 point, Vector3 axis) {
            var dir = point - axis;
            dir = angles * dir;
            point = dir + axis;
            return point;
        }

        public static Quaternion Rotate2D(Vector3 start, Vector3 end) {
            Vector3 diff = start - end;
            diff.Normalize();
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(0f, 0f, rot_z - 90f);
        }

    }


    public static class CanvasGroupExtension {
        public static void SetActive(this CanvasGroup canvasGroup, bool active) {
            canvasGroup.alpha = active ? 1 : 0;
            canvasGroup.interactable = active;
            canvasGroup.blocksRaycasts = active;
        }
    }

    public static class GameObjectExtensions {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component {
            var newOrExistingComponent = gameObject.GetComponent<T>();
            if (newOrExistingComponent == null) {
                newOrExistingComponent = gameObject.AddComponent<T>();
            }
            //var newOrExistingComponent = gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
            return newOrExistingComponent;
        }

        public static GameObject GetClosest(this IList<GameObject> objList, Vector3 position) {
            GameObject go = null;
            float lowDistance = float.MaxValue;
            for (int i = 0; i < objList.Count; i++) {
                var dist = objList[i].transform.position.SqrDistance(position);
                if (dist < lowDistance) {
                    lowDistance = dist;
                    go = objList[i];
                }
            }
            return go;
        }

        public static void SetActive(this IList<GameObject> objList, bool active) {
            for (int i = 0; i < objList.Count; i++) {
                objList[i].SetActive(active);
            }
        }

        public static void DebugDuplicate(this GameObject go) {
            var components = go.GetComponents<Component>();
            if (components == null) {
                return;
            }
            List<Type> uniqueTypes = new List<Type>();
            List<Type> dupes = new List<Type>();
            for (int j = 0; j < components.Length; j++) {
                var type = components[j].GetType();
                if (!uniqueTypes.Contains(type)) {
                    uniqueTypes.Add(type);
                }
                else if (!dupes.Contains(type)) {
                    dupes.Add(type);
                }
            }
            if (dupes.Count == 0) {
                return;
            }
            Debug.LogFormat("Object {0}: Components {1} Duplicates {2}", go.name, components.Length, dupes.Count);
            //for (int j = 0; j < dupes.Count; j++) {
            //    Debug.Log(dupes[j].Name);
            //}
        }
    }

    public static class FloatExtensions {
        //public static float Remap (this float value, float from1, float to1, float from2, float to2) {
        //    return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        //}

        public static float Remap(this float value, float fromMinValue, float fromMaxValue, float toMinValue, float toMaxValue) {
            try {
                return (value - fromMinValue) * (toMaxValue - toMinValue) / (fromMaxValue - fromMinValue) + toMinValue;
            }
            catch {
                return float.NaN;
            }
        }

        public static float ToPercent(this float value) {
            return value * 100;
        }

        public static float ToPercentNormalized(this float value) {
            if (value < 1) {
                return value;
            }
            return value * .001f;
        }
    }

    public static class MathEx {

        public static int Min(int a, int b) {
            return a < b ? a : b;
        }

        public static int Max(int a, int b) {
            return a > b ? a : b;
        }

        public static int Max(int a, int b, int c) {
            if (c > a && c > b) {
                return c;
            }
            return a > b ? a : b;
        }

        public static float Min(float a, float b) {
            return a < b ? a : b;
        }

        public static float Max(float a, float b) {
            return a > b ? a : b;
        }

        /// <summary>
        /// Max is exclusive
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float WrapAround(float val, float min, float max) {
            val = val - (float) Math.Round((val - min) / (max - min)) * (max - min);
            if (val < 0)
                val = val + max - min;
            return val;
        }

        /// <summary>
        /// Max is inclusive
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int WrapClamp(int val, int min, int max) {
            if (val < min) {
                return max;
            }
            return val > max ? min : val;
        }


        public static double RoundDown(double value) {
            value = System.Math.Floor(value);
            return value * Math.Pow(10, 0);
        }

        //This function returns a point which is a projection from a point to a line segment.
        //If the projected point lies outside of the line segment, the projected point will 
        //be clamped to the appropriate line edge.
        //If the line is infinite instead of a segment, use ProjectPointOnLine() instead.
        public static Vector3 ProjectPointOnLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point) {

            Vector3 vector = linePoint2 - linePoint1;

            Vector3 projectedPoint = ProjectPointOnLine(linePoint1, vector.normalized, point);

            int side = PointOnWhichSideOfLineSegment(linePoint1, linePoint2, projectedPoint);

            //The projected point is on the line segment
            if (side == 0) {

                return projectedPoint;
            }

            if (side == 1) {

                return linePoint1;
            }

            if (side == 2) {

                return linePoint2;
            }

            //output is invalid
            return Vector3.zero;
        }

        //This function returns a point which is a projection from a point to a line.
        //The line is regarded infinite. If the line is finite, use ProjectPointOnLineSegment() instead.
        public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point) {

            //get vector from point on line to point in space
            Vector3 linePointToPoint = point - linePoint;

            float t = Vector3.Dot(linePointToPoint, lineVec);

            return linePoint + lineVec * t;
        }

        //This function finds out on which side of a line segment the point is located.
        //The point is assumed to be on a line created by linePoint1 and linePoint2. If the point is not on
        //the line segment, project it on the line using ProjectPointOnLine() first.
        //Returns 0 if point is on the line segment.
        //Returns 1 if point is outside of the line segment and located on the side of linePoint1.
        //Returns 2 if point is outside of the line segment and located on the side of linePoint2.
        public static int PointOnWhichSideOfLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point) {

            Vector3 lineVec = linePoint2 - linePoint1;
            Vector3 pointVec = point - linePoint1;

            float dot = Vector3.Dot(pointVec, lineVec);

            //point is on side of linePoint2, compared to linePoint1
            if (dot > 0) {

                //point is on the line segment
                if (pointVec.magnitude <= lineVec.magnitude) {

                    return 0;
                }

                //point is not on the line segment and it is on the side of linePoint2
                else {

                    return 2;
                }
            }

            //Point is not on side of linePoint2, compared to linePoint1.
            //Point is not on the line segment and it is on the side of linePoint1.
            else {

                return 1;
            }
        }

        public static Vector3 ClosestPointOnLine(Vector3 start, Vector3 end, Vector3 pos) {
            var localPos = pos - start;
            var lineDir = (end - start).normalized;

            var lineLength = Vector3.Distance(start, end);
            var projAngle = Vector3.Dot(lineDir, localPos);
            //projAngle = Mathf.Clamp(projAngle, 0, lineLength);
            if (projAngle <= 0) {
                return start;
            }

            if (projAngle >= lineLength) {
                return end;
            }

            var localPntOnLine = lineDir * projAngle;
            var worldSpaceOnLine = start + localPntOnLine;
            return worldSpaceOnLine;
        }
    }

    public static class BaseItemExtensions {

        //public static T New<T>(this T item) where T : Entity {
        //    var baseItem = ItemPool.SpawnPrefab<T>(item.gameObject);
        //    //baseItem.Setup(UnityEngine.Random.Range(1,item.MaxStack+1), 
        //    //    UnityEngine.Random.Range(1, PlayerGameStats.LevelReached));
        //    return baseItem;
        //}
    }

    public class ClassLog {
        public static void Log(string msg, UnityEngine.Object originClass = null) {
            if (originClass != null) {
                Debug.Log(originClass.ToString() + " :: " + msg);
            }
            else {
                Debug.Log(msg);
            }
        }
    }

    public static class ScrollRectExtensions {
        public static void ScrollToTop(this ScrollRect scrollRect) {
            scrollRect.normalizedPosition = new Vector2(0, 1);
        }

        public static void ScrollToBottom(this ScrollRect scrollRect) {
            scrollRect.normalizedPosition = new Vector2(0, 0);
        }
    }

    public static class ArrayExtensions {
        public static bool Contains(this IList array1, IList array2) {
            for (int a = 0; a < array1.Count; a++) {
                for (int b = 0; b < array2.Count; b++) {
                    if (array1[a].Equals(array2[b])) {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool Contains<T>(this IList<T> array1, T item) {
            for (int a = 0; a < array1.Count; a++) {
                if (array1[a].Equals(item)) {
                    return true;
                }
            }
            return false;
        }

        public static bool Contains<T>(this List<T> array1, T item) {
            for (int a = 0; a < array1.Count; a++) {
                if (array1[a].Equals(item)) {
                    return true;
                }
            }
            return false;
        }

        public static bool Contains<T>(this T[] array1, T item) {
            for (int a = 0; a < array1.Length; a++) {
                if (array1[a].Equals(item)) {
                    return true;
                }
            }
            return false;
        }

        public static bool HasIndex<T>(this IList<T> array, int index) {
            return index >= 0 && index < array.Count;
        }

        public static T Clamp<T>(this IList<T> array, int index) {
            if (index <= 0) {
                return array[0];
            }
            if (index >= array.Count) {
                return array[array.Count - 1];
            }
            return array[index];
        }

        public static T SafeAccess<T>(this IList<T> array, int index) {
            return array[Mathf.Clamp(index, 0, array.Count - 1)];
        }

        public static int FindIndex(this IList<string> array, string target) {
            for (int i = 0; i < array.Count; i++) {
                if (target.CompareCaseInsensitive(array[i])) {
                    return i;
                }
            }
            return -1;
        }

        public static int FindIndex<T>(this IList<T> array1, T item) {
            for (int a = 0; a < array1.Count; a++) {
                if (array1[a].Equals(item)) {
                    return a;
                }
            }
            return -1;
        }

        public static int FindIndex<T>(this List<T> array1, T item) {
            for (int a = 0; a < array1.Count; a++) {
                if (array1[a].Equals(item)) {
                    return a;
                }
            }
            return -1;
        }

        public static int FindIndex<T>(this T[] array1, T item) {
            for (int a = 0; a < array1.Length; a++) {
                if (array1[a].Equals(item)) {
                    return a;
                }
            }
            return -1;
        }

    }

    public static class TextAssetExtensionMethods {
        public static List<string> TextAssetToLineList(this TextAsset ta) {
            return new List<string>(ta.text.Split('\n'));
        }
    }

    public static class LayerMaskExtensions {
        public static bool ContainsLayer(this LayerMask layerMask, int layer) {
            return layerMask == (layerMask | (1 << layer));
        }
    }

    public static class AssetTypeExtensions {
        private static Dictionary<Type, string> _typeToExtensions = new Dictionary<Type, string>() {
            {typeof(GameObject), "prefab"}, {typeof(ScriptableObject), "asset" },
        };

        public static string GetExtensionFromType<T>() {
            if (_typeToExtensions.TryGetValue(typeof(T), out var extension)) {
                return extension;
            }
            return "asset";
        }
    }

    public static class ParseUtilities {

        public static int TryParseInt(this IList<string> lines, ref int parseIndex) {
            return lines.TryParse(ref parseIndex, 0);
        }

        public static float TryParseFloat(this IList<string> lines, ref int parseIndex) {
            return lines.TryParse(ref parseIndex, 0f);
        }

        public static int TryParse(this IList<string> lines, ref int parseIndex, int defValue) {
            if (!lines.HasIndex(parseIndex)) {
                Debug.LogFormat("TryParseInt out of range at {0}", parseIndex);
                parseIndex++;
                return defValue;
            }
            int value;
            if (!int.TryParse(lines[parseIndex], out value)) {
                value = defValue;
            }
            parseIndex++;
            return value;
        }

        public static float TryParse(this IList<string> lines, ref int parseIndex, float defValue) {
            if (!lines.HasIndex(parseIndex)) {
                Debug.LogFormat("TryParseFloat out of range at {0}", parseIndex);
                parseIndex++;
                return defValue;
            }
            float value;
            if (!float.TryParse(lines[parseIndex], out value)) {
                value = defValue;
            }
            parseIndex++;
            return value;
        }

        public static bool TryParse(this IList<string> lines, ref int parseIndex, bool defValue) {
            if (!lines.HasIndex(parseIndex)) {
                Debug.LogFormat("TryParseBool out of range at {0}", parseIndex);
                parseIndex++;
                return defValue;
            }
            bool value;
            if (!bool.TryParse(lines[parseIndex], out value)) {
                value = defValue;
            }
            parseIndex++;
            return value;
        }

        public static LeveledFloat TryParse(this IList<string> lines, ref int parseIndex, LeveledFloat defValue) {
            if (!lines.HasIndex(parseIndex)) {
                Debug.LogFormat("TryParseBool out of range at {0}", parseIndex);
                parseIndex++;
                return defValue;
            }
            LeveledFloat value = LeveledFloat.Parse(lines[parseIndex]);
            parseIndex++;
            return value == null ? defValue : value;
        }

        public static string ParseString(this IList<string> lines, ref int parseIndex) {
            if (!lines.HasIndex(parseIndex)) {
                Debug.LogFormat("ParseString out of range at {0}", parseIndex);
                parseIndex++;
                return null;
            }
            var value = lines[parseIndex];
            parseIndex++;
            return value;
        }

        public static Color TryParse(this IList<string> lines, ref int parseIndex, Color defValue) {
            if (!lines.HasIndex(parseIndex)) {
                Debug.LogFormat("TryParseColor out of range at {0}", parseIndex);
                parseIndex++;
                return defValue;
            }
            Color color;
            if (ColorUtility.TryParseHtmlString(lines[parseIndex], out color)) {
                parseIndex++;
                return color;
            }
            parseIndex++;
            return defValue;
        }

        public static string TryParseString(this IList<string> lines, ref int parseIndex, string defaultVal) {
            if (!lines.HasIndex(parseIndex)) {
                Debug.LogFormat("TryParseString out of range at {0}", parseIndex);
                parseIndex++;
                return defaultVal;
            }
            var value = lines[parseIndex];
            parseIndex++;
            return value;
        }

        public static T ParseEnum<T>(this IList<string> lines, ref int parseIndex) {
            if (!lines.HasIndex(parseIndex)) {
                Debug.LogFormat("ParseEnum out of range at {0}", parseIndex);
                parseIndex++;
                return default(T);
            }
            var value = EnumHelper.ForceParse<T>(lines[parseIndex]);
            parseIndex++;
            return value;
        }

        public static T TryParse<T>(this IList<string> lines, ref int parseIndex, T defaultValue) {
            if (!lines.HasIndex(parseIndex)) {
                Debug.LogFormat("TryParse out of range at {0}", parseIndex);
                parseIndex++;
                return defaultValue;
            }
            T value;
            if (!EnumHelper.TryParse(lines[parseIndex], out value)) {
                value = defaultValue;
            }
            parseIndex++;
            return value;
        }

        public static LeveledFloatRange TryParseLeveledFloatWithMulti(this IList<string> lines, ref int parseIndex) {
            if (!lines.HasIndex(parseIndex+1)) {
                Debug.LogFormat("TryParse out of range at {0}", parseIndex);
                parseIndex++;
                return null;
            }
            LeveledFloat lvledFloat = LeveledFloat.Parse(lines[parseIndex]);
            if (lvledFloat == null) {
                parseIndex++;
                return null;
            }
            parseIndex++;
            float multi = lines.TryParse(ref parseIndex, 1f);
            return new LeveledFloatRange(lvledFloat.BaseValue, lvledFloat.PerLevel, multi);
        }

        public static string[] SplitStringMultiEntries(this IList<string> lines, ref int parseIndex) {
            return lines.SplitStringMultiEntries(StringConst.MultiEntryBreak, ref parseIndex);
        }

        public static string[] SplitStringMultiEntries(this IList<string> lines, char splitChar, ref int parseIndex) {
            if (!lines.HasIndex(parseIndex)) {
                Debug.LogFormat("SplitStringMultiEnties out of range at {0}", parseIndex);
                parseIndex++;
                return null;
            }
            string[] value = lines[parseIndex].Split(splitChar);
            parseIndex++;
            return value;
        }

        public static string[] SplitString(this IList<string> lines, ref int parseIndex, char splitChar = ' ') {
            var value = lines[parseIndex].Split(splitChar);
            parseIndex++;
            return value;
        }

        public static int TryParse(ref IList<string> lines, ref int parseIndex, int defValue) {
            if (lines.Count <= parseIndex) {
                parseIndex++;
                return defValue;
            }
            int value;
            if (!int.TryParse(lines[parseIndex], out value)) {
                value = defValue;
            }
            parseIndex++;
            return value;
        }

        public static float TryParse(ref IList<string> lines, ref int parseIndex, float defValue) {
            if (lines.Count <= parseIndex) {
                parseIndex++;
                return defValue;
            }
            float value;
            if (!float.TryParse(lines[parseIndex], out value)) {
                value = defValue;
            }
            parseIndex++;
            return value;
        }

        public static bool TryParseBool(ref IList<string> lines, ref int parseIndex, bool defValue) {
            if (lines.Count <= parseIndex) {
                parseIndex++;
                return defValue;
            }
            bool value;
            if (!bool.TryParse(lines[parseIndex], out value)) {
                value = defValue;
            }
            parseIndex++;
            return value;
        }

        public static string TryParseString(ref IList<string> lines, ref int parseIndex, string defaultVal) {
            if (lines.Count <= parseIndex) {
                parseIndex++;
                return defaultVal;
            }
            var value = lines[parseIndex];
            parseIndex++;
            return value;
        }

        public static T ParseEnum<T>(ref IList<string> lines, ref int parseIndex) {
            if (lines.Count <= parseIndex) {
                parseIndex++;
                return default(T);
            }
            var value = EnumHelper.ForceParse<T>(lines[parseIndex]);
            parseIndex++;
            return value;
        }

        public static T TryParse<T>(ref IList<string> lines, ref int parseIndex, T defaultValue) {
            if (lines.Count <= parseIndex) {
                parseIndex++;
                return defaultValue;
            }
            T value;
            if (!EnumHelper.TryParse(lines[parseIndex], out value)) {
                value = defaultValue;
            }
            parseIndex++;
            return value;
        }

        public static string[] SplitStringMultiEntries(ref IList<string> lines, ref int parseIndex) {
            if (lines.Count <= parseIndex) {
                parseIndex++;
                return null;
            }
            var value = lines[parseIndex].Split('-');
            parseIndex++;
            return value;
        }

        public static string[] SplitString(ref IList<string> lines, ref int parseIndex, char splitChar = ' ') {
            if (lines.Count <= parseIndex) {
                parseIndex++;
                return null;
            }
            var value = lines[parseIndex].Split(splitChar);
            parseIndex++;
            return value;
        }

        public static IntRange TryParseRange(this IList<string> lines, ref int parseIndex, IntRange defaultVal) {
            if (!lines.HasIndex(parseIndex)) {
                parseIndex++;
                return defaultVal;
            }
            var numbers = lines[parseIndex].Split('-');
            parseIndex++;
            if (numbers.Length < 2) {
                return defaultVal;
            }
            int num1, num2;
            if (!int.TryParse(numbers[0], out num1)) {
                return defaultVal;
            }
            if (!int.TryParse(numbers[1], out num2)) {
                return defaultVal;
            }
            return new IntRange(num1, num2);
        }

        public static int TryParse(string data, int defaultValue) {
            int value;
            if (int.TryParse(data, out value)) {
                return value;
            }
            return defaultValue;
        }

        public static T TryParseEnum<T>(string data, T defaultValue) {
            T value;
            if (EnumHelper.TryParse(data, out value)) {
                return value;
            }
            return defaultValue;
        }

        public static int TryParse(this IList<string> lines, int parseIndex, int defValue) {
            if (!lines.HasIndex(parseIndex)) {
                Debug.LogFormat("TryParseInt out of range at {0}", parseIndex);
                return defValue;
            }
            int value;
            if (!int.TryParse(lines[parseIndex], out value)) {
                value = defValue;
            }
            return value;
        }

        public static float TryParse(this IList<string> lines, int parseIndex, float defValue) {
            if (!lines.HasIndex(parseIndex)) {
                Debug.LogFormat("TryParseFloat out of range at {0}", parseIndex);
                return defValue;
            }
            float value;
            if (!float.TryParse(lines[parseIndex], out value)) {
                value = defValue;
            }
            return value;
        }

        public static bool TryParse(this IList<string> lines, int parseIndex, bool defValue) {
            if (!lines.HasIndex(parseIndex)) {
                Debug.LogFormat("TryParseBool out of range at {0}", parseIndex);
                return defValue;
            }
            bool value;
            if (!bool.TryParse(lines[parseIndex], out value)) {
                value = defValue;
            }
            return value;
        }

        public static LeveledFloat TryParse(this IList<string> lines, int parseIndex, LeveledFloat defValue) {
            if (!lines.HasIndex(parseIndex)) {
                Debug.LogFormat("TryParseBool out of range at {0}", parseIndex);
                return defValue;
            }
            LeveledFloat value = LeveledFloat.Parse(lines[parseIndex]);
            return value == null ? defValue : value;
        }

        public static Color TryParse(this IList<string> lines, int parseIndex, Color defValue) {
            if (!lines.HasIndex(parseIndex)) {
                Debug.LogFormat("TryParseBool out of range at {0}", parseIndex);
                return defValue;
            }
            Color color;
            return ColorUtility.TryParseHtmlString(lines[parseIndex], out color) ? color : defValue;
        }

        public static string TryParseString(this IList<string> lines, int parseIndex, string defaultVal) {
            if (!lines.HasIndex(parseIndex)) {
                return defaultVal;
            }
            var value = lines[parseIndex];
            return value;
        }

        public static IntRange TryParseRange(this IList<string> lines, int parseIndex, IntRange defaultVal) {
            if (!lines.HasIndex(parseIndex)) {
                return defaultVal;
            }
            var numbers = lines[parseIndex].Split('-');
            if (numbers.Length < 3) {
                return defaultVal;
            }
            int num1, num2;
            if (!int.TryParse(numbers[0], out num1)) {
                return defaultVal;
            }
            if (!int.TryParse(numbers[1], out num2)) {
                return defaultVal;
            }
            return new IntRange(num1, num2);
        }

        public static T TryParse<T>(this IList<string> lines, int parseIndex, T defaultValue) {
            if (!lines.HasIndex(parseIndex)) {
                Debug.LogFormat("TryParse out of range at {0}", parseIndex);
                return defaultValue;
            }
            T value;
            if (!EnumHelper.TryParse(lines[parseIndex], out value)) {
                value = defaultValue;
            }
            return value;
        }

        public static List<string> TryChildSplit(this IList<string> lines, int parseIndex) {
            if (!lines.HasIndex(parseIndex)) {
                return null;
            }
            return StringUtilities.SplitChildMultiEntry(lines[parseIndex]);
            
        }
    }

    public static class PronounExtension {
        public static string NewPlayerName(this PlayerPronouns pronouns) {
            switch (pronouns) {
                case PlayerPronouns.He:
                    return StaticTextDatabase.RandomPlayerMaleName();
                case PlayerPronouns.She:
                    return StaticTextDatabase.RandomPlayerFemaleName();
            }
            return Game.CoinFlip() ? StaticTextDatabase.RandomPlayerMaleName() : StaticTextDatabase.RandomPlayerFemaleName();
        }
    }

#if UNITY_EDITOR
    public static class SceneExtensions {
        public static ValueDropdownList<int> GetBuildSceneList() {
            var list = new ValueDropdownList<int>();
            var sceneList = EditorBuildSettings.scenes;
            Regex regex = new Regex(@"([^/]*/)*([\w\d\-]*)\.unity");
            for (int i = 0; i < sceneList.Length; i++) {
                list.Add(new ValueDropdownItem<int>(regex.Replace(sceneList[i].path, "$2"), i));
            }
            return list;
        }
    }
#endif
}