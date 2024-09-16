using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utility
{
    public static class Lerp
    {
        public static float GetLerpTime(float duration, float lerpTime, out float updatedLerpTime, bool debug = false)
        {
            updatedLerpTime = lerpTime + Time.deltaTime * .4f;
            if (debug) Debug.Log(lerpTime + " + " + Time.deltaTime * .4f + " = " + updatedLerpTime);
            var t = updatedLerpTime / duration;
            t = t * t * (3f - 2f * t);
            return t;
        }
        
        public static float GetReversedLerpTime(float duration, float lerpTime, out float updatedLerpTime, bool debug = false)
        {
            updatedLerpTime = lerpTime - Time.deltaTime * .4f;
            if (debug) Debug.Log(lerpTime + " - " + Time.deltaTime * .4f + " = " + updatedLerpTime);
            var t = updatedLerpTime / duration;
            t = t * t * (3f - 2f * t);
            return t;
        }
        
        public static float GetFixedLerpTime(float duration, float lerpTime, out float updatedLerpTime, bool debug = false)
        {
            updatedLerpTime = lerpTime + Time.fixedDeltaTime * .4f;
            if (debug) Debug.Log(lerpTime + " + " + Time.fixedDeltaTime * .4f + " = " + updatedLerpTime);
            float t = updatedLerpTime / duration;
            t = t * t * (3f - 2f * t);
            return t;
        }

        public static float GetConstantLerp(float duration, float lerpTime, out float updatedLerpTime, bool debug = false)
        {
            updatedLerpTime = lerpTime + Time.deltaTime * .4f;
            if (debug) Debug.Log(lerpTime + " + " + Time.fixedDeltaTime * .4f + " = " + updatedLerpTime);
            var t = updatedLerpTime / duration;
            return t;
        }
    }

    public static class Force
    {
        public static Vector2 CalculateForce(Vector3 a, Vector3 b, float multiplier)
        {
            Vector2 difference = (a - b).normalized;
            return difference * multiplier;
        }
    }

    public static class FormatTime
    {
        public static string HHMMSS(float time)
        {
            int hours = (int) time / 3600;
            int minutes = (int) time / 60 % 60;
            int seconds = (int) time % 60;
            return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
        
        public static string MMSS(float time)
        {
            int minutes = (int) time / 60;
            int seconds = (int) time - 60 * minutes;
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public static class Transforms
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
    
    public static class IEnumerables
    {       
        public static bool IsSafe<T>(this IEnumerable<T> source) => source != null && source.Any();
    }

    public static class Angles
    {
        public static float FixedAngle(float angle)
        {
            angle %= 360;
            if (angle < 0) angle = 360 + angle;
            return angle;
        }
    }

    public static class Convert
    {
        public static float ToFloat(this int value, int divisible = 1) => ((float)value) / divisible;
        
        public static string AddSpace(this string internalName)
        {
            if (string.IsNullOrWhiteSpace(internalName))
                return "";
            StringBuilder newText = new StringBuilder(internalName.Length * 2);
            newText.Append(internalName[0]);
            for (int i = 1; i < internalName.Length; i++)
            {
                if (char.IsUpper(internalName[i]) && internalName[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(internalName[i]);
            }
            return newText.ToString();
        }
    }

    public static class Directions
    {
        public static List<Vector2> EightDirections = new()
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
    }

    public static class VectorExtensions
    {
        public static Vector2 Modify(this Vector2 point, float? x = null, float? y = null)
        {
            // transform.position = transform.position.Modify(y: 0)
            return new Vector2(x == null ? point.x : x.Value, y == null ? point.y : y.Value);
        }

        public static Vector3 Modify(this Vector3 point, float? x = null, float? y = null, float? z = null)
        {
            // transform.position = transform.position.Modify(z: 0)
            return new Vector3(x == null ? point.x : x.Value, y == null ? point.y : y.Value,
                z == null ? point.z : z.Value);
        }

        public static Vector4 Modify(this Vector4 point, float? x = null, float? y = null, float? z = null, float? w = null)
        {
            // transform.position = transform.position.Modify(w: 0)
            return new Vector4(x == null ? point.x : x.Value,
                y == null ? point.y : y.Value,
                z == null ? point.z : z.Value,
                w == null ? point.w : w.Value);
        }
        
        public static Vector2 RandomVector2(int xMinRange, int xMaxRange, int yMinRange, int yMaxRange)
        {
            var randX = Random.Range(xMinRange, xMaxRange);
            var randY = Random.Range(yMinRange, yMaxRange);
            return new Vector2(randX, randY);
        }
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
        
        public static Vector2 RandomVector2(int xMinRange, int xMaxRange, int yMinRange, int yMaxRange)
        {
            var randX = Random.Range(xMinRange, xMaxRange);
            var randY = Random.Range(yMinRange, yMaxRange);
            return new Vector2(randX, randY);
        }
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = 
                        Attribute.GetCustomAttribute(field, 
                            typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }

        public static string GetName(this Enum value)
        {
            var name = value.ToString();
            if (string.IsNullOrWhiteSpace(name))
                return "";
            StringBuilder newText = new StringBuilder(name.Length * 2);
            newText.Append(name[0]);
            for (int i = 1; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]) && name[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(name[i]);
            }
            return newText.ToString();
        }
    }
    
    public static class Debug
    {
        public static void Log(object message)
        {
            #if UNITY_EDITOR
            UnityEngine.Debug.Log(message);
            #endif
        }

        public static void DrawCircle(Vector2 center, float radius, Color color, bool transparent = false)
        {
            #if UNITY_EDITOR
            Gizmos.color = transparent ? new Color(color.r, color.g, color.b, 0.2f) : color;
            Gizmos.DrawWireSphere(center, radius);
            #endif
        }

        public static void DrawGrid(Vector2 center, Vector2 size, Color color, bool transparent = false)
        {
            #if UNITY_EDITOR
            Gizmos.color = transparent ? new Color(color.r, color.g, color.b, 0.2f) : color;
            Gizmos.DrawWireCube(center, size);
            #endif
        }

        public static void DrawLine(Vector2 from, Vector2 to, Color color, bool transparent = false)
        {
            #if UNITY_EDITOR
            Gizmos.color = transparent ? new Color(color.r, color.g, color.b, 0.2f) : color;
            Gizmos.DrawLine(from, to);
            #endif 
        }

        public static void DrawSphere(Vector2 center, float radius, Color color, bool transparent = false)
        {
            #if UNITY_EDITOR
            Gizmos.color = transparent ? new Color(color.r, color.g, color.b, 0.2f) : color;
            Gizmos.DrawSphere(center, radius);
            #endif
        }
    }
}

