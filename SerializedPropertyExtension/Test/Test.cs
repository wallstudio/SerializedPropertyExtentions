using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

[Serializable] abstract class PiyoBase {}
[Serializable] class Piyo : PiyoBase
{
    [SerializeField] int number;
    [SerializeField] string text;
}
[Serializable] class Piyo2 : PiyoBase
{
    [SerializeField] bool condition;
    [SerializeField] string text;
}

[Serializable] class Fuga
{
    [SerializeField] byte @byte;
    [SerializeField] short @short;
    [SerializeField] int @int;
    [SerializeField] long @long;
    [SerializeField] bool @bool;
    [SerializeField] float @float;
    [SerializeField] double @double;
    [SerializeField] string @string;
    [SerializeField] Color color;
    [SerializeField] Color32 color32;
    [SerializeField] UnityEngine.Object @object;
    [SerializeField] LayerMask layerMask;
    [SerializeField] Enum obejectEnum;
    [SerializeField] RuntimePlatform @enum;
    [SerializeField] Rect rect;
    [SerializeField] List<int> list;
    [SerializeField] char @chara;
    [SerializeField] AnimationCurve animationCurve;
    [SerializeField] Bounds bounds;
    [SerializeField] Gradient gradient;
    [SerializeField] Quaternion quaternion;
    [SerializeField] ExposedReference<UnityEngine.Object> exposedReference;
    [SerializeField] Vector2 vector2;
    [SerializeField] Vector3 vector3;
    [SerializeField] Vector4 vector4;
    [SerializeField] Vector2Int vector2Int;
    [SerializeField] Vector3Int vector3Int;
    [SerializeField] RectInt rectInt;
    [SerializeField] BoundsInt boundsInt;
    [SerializeReference] PiyoBase piyo1;

    public Fuga(PiyoBase piyo1) => this.piyo1 = piyo1;
}

[Serializable] public class Hoge
{
    [SerializeField] List<int> list1;
    [SerializeField] List<int> list2;
    [SerializeField] string a;
    [SerializeField] string b;
    [SerializeField] Fuga x = new Fuga(new Piyo());
    [SerializeField] Fuga y = new Fuga(new Piyo());
    // [SerializeField] Fuga y = new Fuga(new Piyo2()); // これはコピーできなくなる
}

public class Test : MonoBehaviour
{
    [SerializeField] Hoge hoge;
}
