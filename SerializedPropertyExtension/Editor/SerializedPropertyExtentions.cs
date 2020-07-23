using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using Object = UnityEngine.Object;
using System.Linq;

public static class SerializedPropertyExtentions
{
    // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/SerializedProperty.bindings.cs#L876
    static readonly PropertyInfo GRADIENT_PROPERTY = typeof(SerializedProperty).GetProperty("gradientValue", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly Func<SerializedProperty, Gradient> GRADIENT_GETTER = (Func<SerializedProperty, Gradient>)(GRADIENT_PROPERTY.GetMethod.CreateDelegate(typeof(Func<SerializedProperty, Gradient>)));
    static readonly Action<SerializedProperty, Gradient> GRADIENT_SETTER = (Action<SerializedProperty, Gradient>)(GRADIENT_PROPERTY.SetMethod.CreateDelegate(typeof(Action<SerializedProperty, Gradient>)));
    
    public static IEnumerable<SerializedProperty> EnumrateArray(this SerializedProperty array) => Enumerable.Range(0, array.arraySize).Select(array.GetArrayElementAtIndex);

    public static IEnumerable<SerializedProperty> EnumrateObject(this SerializedProperty target)
    {
        var list = new List<SerializedProperty>();
        var iterator = target.Copy();
        var end = iterator.GetEndProperty();
        do
        {
            yield return iterator.Copy();
        }
        while(iterator.NextVisible(true) && iterator.propertyPath != end.propertyPath); // NextVisibleにするとAnimCurveの中身などを掘らない
    }

    public static void CopyTo(this SerializedProperty src, SerializedProperty dst)
    {
        // 配列のサイズなどが違っていてもOK
        // Enumratorが遅延させているため、サイズの異なる配列をコピーしようとすると、
        // まずArraySizeがコピーされて拡縮される。よってIterator回数は同じになる。
        // 一方で、ManagedRefarenceObjectはどうしても無理
        foreach (var (srcMem, dstMem) in EnumrateObject(src).Zip(EnumrateObject(dst), (s, d) => (s, d)))
        {
            if(srcMem.propertyType != dstMem.propertyType)
            {
                throw new Exception($"Miss match type {srcMem.propertyType} vs{dstMem.propertyType}\n{srcMem.propertyPath} -> {dstMem.propertyPath}");
            }
            
            var value = srcMem.GetValue();
            dstMem.SetValue(value);
        }
    }

    public static void ClearObjectByDefault(this SerializedProperty target) => EnumrateObject(target).ForEach(mem => mem.SetValue(null));

    public static string DumpHierarchy(this SerializedProperty target) => string.Join("\n", EnumrateObject(target).Select(mem => $"{mem?.propertyPath}: {mem?.GetValue()} ({mem.propertyType})"));

    public static object GetValue(this SerializedProperty target)
    {
        switch(target.propertyType)
        {
            case SerializedPropertyType.Integer :
            case SerializedPropertyType.LayerMask :
            case SerializedPropertyType.Enum :
            case SerializedPropertyType.ArraySize :
            case SerializedPropertyType.Character :
            case SerializedPropertyType.FixedBufferSize :
                return target.longValue;
            case SerializedPropertyType.Boolean :
                return target.boolValue;
            case SerializedPropertyType.Float :
                return target.doubleValue;
            case SerializedPropertyType.String :
                return target.stringValue;
            case SerializedPropertyType.Color :
                return target.colorValue;
            case SerializedPropertyType.ObjectReference :
                return target.objectReferenceValue;
            case SerializedPropertyType.Vector2 :
                return target.vector2Value;
            case SerializedPropertyType.Vector3 :
                return target.vector3Value;
            case SerializedPropertyType.Vector4 :
                return target.vector4Value;
            case SerializedPropertyType.Rect :
                return target.rectValue;
            case SerializedPropertyType.AnimationCurve :
                return target.animationCurveValue;
            case SerializedPropertyType.Bounds :
                return target.boundsValue;
            case SerializedPropertyType.Gradient :
                return GRADIENT_GETTER(target);
            case SerializedPropertyType.Quaternion :
                return target.quaternionValue;
            case SerializedPropertyType.ExposedReference :
                return target.exposedReferenceValue;
            case SerializedPropertyType.Vector2Int :
                return target.vector2IntValue;
            case SerializedPropertyType.Vector3Int :
                return target.vector3IntValue;
            case SerializedPropertyType.RectInt :
                return target.rectIntValue;
            case SerializedPropertyType.BoundsInt :
                return target.boundsIntValue;
            case SerializedPropertyType.Generic :
            case SerializedPropertyType.ManagedReference :
            default:
            {
                return null;
            }
        }
    }

    public static void SetValue(this SerializedProperty target, object value)
    {
        switch(target.propertyType)
        {
            case SerializedPropertyType.Integer :
            case SerializedPropertyType.LayerMask :
            case SerializedPropertyType.Enum :
            case SerializedPropertyType.ArraySize :
            case SerializedPropertyType.Character :
            case SerializedPropertyType.FixedBufferSize :
                target.longValue = Convert.ToInt64(value ?? default(long)); return;
            case SerializedPropertyType.Boolean :
                target.boolValue = (value as bool?) ?? default; return;
            case SerializedPropertyType.Float :
                target.doubleValue = (value as double?) ?? default; return;
            case SerializedPropertyType.String :
                target.stringValue = (value as string) ?? default; return;
            case SerializedPropertyType.Color :
                target.colorValue = (value as Color?) ?? default; return;
            case SerializedPropertyType.ObjectReference :
                target.objectReferenceValue = (value as Object) ?? default; return;
            case SerializedPropertyType.Vector2 :
                target.vector2Value = (value as Vector2?) ?? default; return;
            case SerializedPropertyType.Vector3 :
                target.vector3Value = (value as Vector3?) ?? default; return;
            case SerializedPropertyType.Vector4 :
                target.vector4Value = (value as Vector4?) ?? default; return;
            case SerializedPropertyType.Rect :
                target.rectValue = (value as Rect?) ?? default; return;
            case SerializedPropertyType.AnimationCurve :
                target.animationCurveValue = (value as AnimationCurve) ?? new AnimationCurve(); return;
            case SerializedPropertyType.Bounds :
                target.boundsValue = (value as Bounds?) ?? default; return;
            case SerializedPropertyType.Gradient :
                GRADIENT_SETTER(target, (value as Gradient) ?? new Gradient()); return;
            case SerializedPropertyType.Quaternion :
                target.quaternionValue = (value as Quaternion?) ?? default; return;
            case SerializedPropertyType.ExposedReference :
                target.exposedReferenceValue = (value as Object) ?? default; return;
            case SerializedPropertyType.Vector2Int :
                target.vector2IntValue = (value as Vector2Int?) ?? default; return;
            case SerializedPropertyType.Vector3Int :
                target.vector3IntValue = (value as Vector3Int?) ?? default; return;
            case SerializedPropertyType.RectInt :
                target.rectIntValue = (value as RectInt?) ?? default; return;
            case SerializedPropertyType.BoundsInt :
                target.boundsIntValue = (value as BoundsInt?) ?? default; return;
            case SerializedPropertyType.Generic :
            case SerializedPropertyType.ManagedReference :
            default:
            {
                return;
            }
        }
    }

    public static void SortArray<T>(this SerializedProperty keyArray, Func<SerializedProperty, T> keySelector, params SerializedProperty[] additionalArraies)
    {
        var arraies = new[]{keyArray}.Concat(additionalArraies ?? new SerializedProperty[0]).ToArray();
        if(arraies.GroupBy(arr => arr.arraySize).Count() != 1)
        {
            throw new Exception($"Diffarent size {string.Join(",", arraies.Select(arr => arr.arraySize))}");
        }

        var len = keyArray.arraySize;
        var comparer = Comparer<T>.Default;
        for (var i = 1; i < len; i++) {
            var item = keySelector(keyArray.GetArrayElementAtIndex(i));
            var j = i;
            while(j - 1 >= 0 && comparer.Compare(item, keySelector(keyArray.GetArrayElementAtIndex(j - 1))) < 0)
            {
                j--;
            }
            if(j < i)
            {
                arraies.ForEach(arr => arr.MoveArrayElement(i, j));
            }
        }
    }

    public static bool DataEquals(this SerializedProperty a, SerializedProperty b) => SerializedProperty.DataEquals(a, b);

    static void ForEach<T>(this IEnumerable<T> enumrable, Action<T> action)
    {
        foreach (var item in enumrable)
        {
            action(item);
        }
    }


}