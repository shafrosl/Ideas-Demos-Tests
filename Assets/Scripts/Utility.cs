using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;
using static System.Array;
using Component = UnityEngine.Component;
using Random = UnityEngine.Random;

namespace Utility
{
    public static class LerpExtensions
    {
        public static float GetLerpTime(float duration, float lerpTime, out float updatedLerpTime, bool debug = false)
        {
            updatedLerpTime = lerpTime + Time.deltaTime * .4f;
            if (debug) DebugExtensions.Log(lerpTime + " + " + Time.deltaTime * .4f + " = " + updatedLerpTime);
            var t = updatedLerpTime / duration;
            t = t * t * (3f - 2f * t);
            return t;
        }
        
        public static float GetReversedLerpTime(float duration, float lerpTime, out float updatedLerpTime, bool debug = false)
        {
            updatedLerpTime = lerpTime - Time.deltaTime * .4f;
            if (debug) DebugExtensions.Log(lerpTime + " - " + Time.deltaTime * .4f + " = " + updatedLerpTime);
            var t = updatedLerpTime / duration;
            t = t * t * (3f - 2f * t);
            return t;
        }
        
        public static float GetFixedLerpTime(float duration, float lerpTime, out float updatedLerpTime, bool debug = false)
        {
            updatedLerpTime = lerpTime + Time.fixedDeltaTime * .4f;
            if (debug) DebugExtensions.Log(lerpTime + " + " + Time.fixedDeltaTime * .4f + " = " + updatedLerpTime);
            var t = updatedLerpTime / duration;
            t = t * t * (3f - 2f * t);
            return t;
        }

        public static float GetConstantLerp(float duration, float lerpTime, out float updatedLerpTime, bool debug = false)
        {
            updatedLerpTime = lerpTime + Time.deltaTime * .4f;
            if (debug) DebugExtensions.Log(lerpTime + " + " + Time.fixedDeltaTime * .4f + " = " + updatedLerpTime);
            var t = updatedLerpTime / duration;
            return t;
        }
    }

    public static class TransformExtensions
    {
        public static List<Transform> GetAllChildrenInTransform(this Transform parent, out int count)
        {
            List<Transform> children = new();
            foreach(Transform child in parent)
            {
                children.Add(child);
            }

            count = children.Count;
            return children;
        }

        public static Transform TryGetChild(this Transform parent, int index)
        {
            var children = parent.GetAllChildrenInTransform(out var count);
            if (count > 0 && index < count) return children[index];
            return null;
        }
    }
    
    public static class ArrayExtensions
    {       
        public static T RandomValue<T>(this IEnumerable<T> list) => list.ElementAt(Random.Range(0, list.Count()));
        public static T RandomValue<T>(this IEnumerable<T> list, System.Func<T, bool> filter) => list.Where(filter).RandomValue();
        public static T LoopFrom<T>(this IList<T> list, int startingPos, int i) => list[(startingPos + i) % list.Count];
        public static bool IsSafe<T>(this IList<T> list) => list != null && list.Count != 0;
        public static bool IsSafe<T>(this ICollection<T> collection) => collection != null && collection.Count != 0;
        public static bool IsSafe<T>(this IList<T> list,int count) => list != null && list.Count == count;
        public static bool IsSafe<T>(this ICollection<T> collection,int count) => collection != null && collection.Count == count;
        public static bool IsSafe<T>(this IEnumerable<T> enumerable) => enumerable != null && enumerable.Any();
        public static bool InRange<T>(this ICollection<T> collection, int index) => collection != null && index >= 0 && collection.Count > index;
        public static bool Get<T>(this ICollection<T> collection, int index,out T item)
        {
            if(InRange(collection,index))
            {
                item = collection.ElementAt(index);
                return true;
            }
            item = default;
            return false;
        }
        
        public static int SafeLength(this IList list) => list?.Count ?? 0;
        
        public static IList Swap(this IList list, int oldIndex, int newIndex)
        {
            (list[oldIndex], list[newIndex]) = (list[newIndex], list[oldIndex]);
            return list;
        }
        
        public static void Shift<T>(this List<T> list, int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex || oldIndex < 0 || oldIndex >= list.Count || newIndex < 0 ||
                newIndex >= list.Count) return;
            var tmp = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex,tmp);
        }
        
        public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
        {
            // exit if positions are equal or outside array
            if ((oldIndex == newIndex) || (oldIndex < 0) || (oldIndex >= list.Count) || (newIndex < 0) ||
                (newIndex >= list.Count)) return;
            // local variables
            int i;
            var tmp = list[oldIndex];
            // move element down and shift other elements up
            if (oldIndex < newIndex)
            {
                for (i = oldIndex; i < newIndex; i++)
                {
                    list[i] = list[i + 1];
                }
            }
            // move element up and shift other elements down
            else
            {
                for (i = oldIndex; i > newIndex; i--)
                {
                    list[i] = list[i - 1];
                }
            }
            // put element from position 1 to destination
            list[newIndex] = tmp;
        }
        
        public static IList<T> Shuffle<T>(this IList<T> list,System.Random rng)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
            return list;
        }
        
        public static IList Shuffle(this IList list,bool seeded= false,int seed = 0)
        {
            var rng = seeded ? new System.Random(seed) : new System.Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
            return list;
        }
        
        public static T[] ShuffleArray<T>(this IList<T> list,bool seeded= false,int seed = 0)
        {
            var arr = new T[list.Count];
            list.CopyTo(arr,0);
            var rng = seeded ? new System.Random(seed) : new System.Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                (arr[k], arr[n]) = (arr[n], arr[k]);
            }
            return arr;
        }
        
        public static T[] ShuffleSelect<T>(this IList<T> list, int length, bool seeded = false, int seed = 0)
        {
            var arr = new T[list.Count];
            list.CopyTo(arr,0);
            var rng = seeded ? new System.Random(seed) : new System.Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                (arr[k], arr[n]) = (arr[n], arr[k]);
            }

            return arr[..length];
        }
        
        public static T[] AddToArray<T>(this T[] array, T variable)
        {
            var arr = array.ToList();
            arr.Add(variable);
            return arr.ToArray();
        }
        
        public static T[] RemoveFromArray<T>(this T[] array, T variable)
        {
            var arr = array.ToList();
            arr.Remove(variable);
            return arr.ToArray();
        }
        
        public static void ClearNullEntries<T>(this List<T> arr) where T : class
        {
            for (var i = arr.Count-1; i >= 0; i--)
            {
                if (arr[i] == null) arr.RemoveAt(i);
            }
        }
        
        public static T[] ClearNullEntries<T>(this T[] array) where T : class
        {
            var arr = array.ToList();
            for (var i = arr.Count-1; i >= 0; i--)
            {
                if (arr[i] == null) arr.RemoveAt(i);
            }
            return arr.ToArray();
        }
        
        public static bool TryGetPooledObject<T>(this IList<T> array, out T obj) where T : Component
        {
            foreach (var o in array)
            {
                if (o.gameObject.activeSelf) continue;
                obj = o;
                obj.gameObject.SetActive(true);
                return true;
            }
            obj = default;
            return false;
        }
    }

    public static class MathExtensions
    {
        public static float ToFloat(this int value, int divisible = 1) => ((float)value) / divisible;
        
        public static float FixedAngle(float angle)
        {
            angle %= 360;
            if (angle < 0) angle = 360 + angle;
            return angle;
        }
        
        public static Vector2 CalculateForce(Vector3 a, Vector3 b, float multiplier)
        {
            Vector2 difference = (a - b).normalized;
            return difference * multiplier;
        }
        
        public static string HHMMSS(float time)
        {
            var hours = (int) time / 3600;
            var minutes = (int) time / 60 % 60;
            var seconds = (int) time % 60;
            return $"{hours:00}:{minutes:00}:{seconds:00}";
        }
        
        public static string MMSS(float time)
        {
            var minutes = (int) time / 60;
            var seconds = (int) time - 60 * minutes;
            return $"{minutes:00}:{seconds:00}";
        }
    }

    public static class StringExtensions
    {
        public static string AddSpace(this string internalName)
        {
            if (string.IsNullOrWhiteSpace(internalName)) return "";
            var newText = new StringBuilder(internalName.Length * 2);
            newText.Append(internalName[0]);
            for (var i = 1; i < internalName.Length; i++)
            {
                if (char.IsUpper(internalName[i]) && internalName[i - 1] != ' ') newText.Append(' ');
                newText.Append(internalName[i]);
            }

            return newText.ToString();
        }
    }

    public static class MeshExtensions
    {
        public static Mesh SpriteToMesh(this Sprite sprite)
        {
            var mesh = new Mesh();
            mesh.SetVertices(Array.ConvertAll(sprite.vertices, i => (Vector3)i).ToList());
            mesh.SetUVs(0, sprite.uv.ToList());
            mesh.SetTriangles(Array.ConvertAll(sprite.triangles, i => (int)i), 0);
            return mesh;
        }
    }

    public static class VectorExtensions
    {
        public static Vector2 CreateV2(float value) => new (value, value);
        public static Vector3 CreateV3(float value) => new (value, value, value);
        public static Vector4 CreateV4(float value) => new (value, value, value, value);
        public static Vector2 Modify(this Vector2 point, float? x = null, float? y = null)
        {
            return new Vector2(
                x ?? point.x, 
                y ?? point.y);
        }

        public static Vector3 Modify(this Vector3 point, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(
                x ?? point.x, 
                y ?? point.y,
                z ?? point.z);
        }

        public static Vector4 Modify(this Vector4 point, float? x = null, float? y = null, float? z = null, float? w = null)
        {
            return new Vector4(
                x ?? point.x,
                y ?? point.y,
                z ?? point.z,
                w ?? point.w);
        }
        
        public static Vector2 RandomVector2(int xMinRange, int xMaxRange, int yMinRange, int yMaxRange)
        {
            var randX = Random.Range(xMinRange, xMaxRange);
            var randY = Random.Range(yMinRange, yMaxRange);
            return new Vector2(randX, randY);
        }
        
        public static readonly List<Vector2> EightDirections2D = new()
        {
            new Vector2(0,1).normalized,
            new Vector2(1,1).normalized,
            new Vector2(1,0).normalized,
            new Vector2(1,-1).normalized,
            new Vector2(0,-1).normalized,
            new Vector2(-1,-1).normalized,
            new Vector2(-1,0).normalized,
            new Vector2(-1,1).normalized
        };
        
        public static readonly List<Vector3> EightDirections3D = new()
        {
            new Vector3(0, 0, 1).normalized,
            new Vector3(1, 0, 1).normalized,
            new Vector3(1, 0,0).normalized,
            new Vector3(1, 0,-1).normalized,
            new Vector3(0, 0,-1).normalized,
            new Vector3(-1, 0,-1).normalized,
            new Vector3(-1, 0,0).normalized,
            new Vector3(-1, 0,1).normalized
        };
        
        public static readonly List<Vector3> ForwardDirections3D = new()
        {
            new Vector3(0, 0, 1).normalized,
            new Vector3(1, 0, 1).normalized,
            new Vector3(-1, 0,1).normalized,
            new Vector3(1, 0,0).normalized,
            new Vector3(-1, 0,0).normalized
        };
    }

    public static class ColorExtensions
    {
        public static Color Modify(this Color color, float? r = null, float? g = null, float? b = null, float? a = null)
        {
            return new Color(r ?? color.r,
                g ?? color.g,
                b ?? color.b,
                a ?? color.a);
        }
    }

    public static class RayExtensions
    {
        private static RaycastHit[] results = new RaycastHit[16];
        private static Collider[] colliderResult = new Collider[16];
        public static RaycastHit[] GetRaycastHits3D(Vector3 origin, Vector3 direction, out int size)
        {
            var ray = new Ray(origin, direction);
            Clear(results, 0, results.Length);
            size = Physics.RaycastNonAlloc(ray, results);
            Sort(results, (a, b) => (a.distance.CompareTo(b.distance)));
            return results;
        }

        public static Collider[] GetOverlapBox3D(Vector3 center, Vector3 halfExtents, out int size)
        {
            size = Physics.OverlapBoxNonAlloc(center, halfExtents, colliderResult);
            return colliderResult;
        } 

        public static RaycastHit GetRaycastHit3D(Vector3 origin, Vector3 direction)
        {
            var ray = new Ray(origin, direction);
            Physics.Raycast(ray, out var hit);
            return hit;
        }
        
        public static RaycastHit GetRaycastHit3D(Vector3 origin, Vector3 direction, float distance)
        {
            Physics.Raycast(origin, direction, out var hit, distance);
            return hit;
        }
    }

    public static class EnumExtensions
    {
        public static IEnumerable<T> GetValues<T>() => Enum.GetValues(typeof(T)).Cast<T>();
        
        public static string GetDescription(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            if (name == null) return null;
            var field = type.GetField(name);
            if (field == null) return null;
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr) return attr.Description;
            return null;
        }

        public static string GetName(this Enum value)
        {
            var name = value.ToString();
            if (string.IsNullOrWhiteSpace(name)) return "";
            var newText = new StringBuilder(name.Length * 2);
            newText.Append(name[0]);
            for (var i = 1; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]) && name[i - 1] != ' ') newText.Append(' ');
                newText.Append(name[i]);
            }
            return newText.ToString();
        }
    }
    
    public static class DebugExtensions
    {
        public static void Log(object message)
        {
            #if UNITY_EDITOR
            Debug.Log(message);
            #endif
        }

        public static void DrawCircle(Vector2 center, float radius, Color color, bool transparent = false)
        {
            #if UNITY_EDITOR
            Gizmos.color = transparent ? color.Modify(a:0.2f) : color;
            Gizmos.DrawWireSphere(center, radius);
            #endif
        }

        public static void DrawGrid(Vector2 center, Vector2 size, Color color, bool transparent = false)
        {
            #if UNITY_EDITOR
            Gizmos.color = transparent ? color.Modify(a:0.2f) : color;
            Gizmos.DrawWireCube(center, size);
            #endif
        }

        public static void DrawLine(Vector2 from, Vector2 to, Color color, bool transparent = false)
        {
            #if UNITY_EDITOR
            Gizmos.color = transparent ? color.Modify(a:0.2f) : color;
            Gizmos.DrawLine(from, to);
            #endif 
        }

        public static void DrawSphere(Vector2 center, float radius, Color color, bool transparent = false)
        {
            #if UNITY_EDITOR
            Gizmos.color = transparent ? color.Modify(a:0.2f) : color;
            Gizmos.DrawSphere(center, radius);
            #endif
        }
        
        public static void DrawLine(Vector3 from, Vector3 to, Color color, bool transparent = false)
        {
            #if UNITY_EDITOR
            Gizmos.color = transparent ? color.Modify(a:0.2f) : color;
            Gizmos.DrawLine(from, to);
            #endif 
        }
        
        public static void DrawLine(Vector3 from, Vector3 to, float distance, Color color, bool transparent = false)
        {
            #if UNITY_EDITOR
            Gizmos.color = transparent ? color.Modify(a:0.2f) : color;
            Gizmos.DrawLine(from, to * distance);
            #endif 
        }
    }
}

