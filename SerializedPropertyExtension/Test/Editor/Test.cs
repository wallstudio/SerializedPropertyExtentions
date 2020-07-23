using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomPropertyDrawer(typeof(Hoge))]
class HogeEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var list1 = property.FindPropertyRelative("list1");
        var list2 = property.FindPropertyRelative("list2");
        var a = property.FindPropertyRelative("a");
        var b = property.FindPropertyRelative("b");
        var x = property.FindPropertyRelative("x");
        var y = property.FindPropertyRelative("y");
        if(position.Contains(Event.current.mousePosition) && Event.current.type == EventType.ContextClick)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent(nameof(SerializedPropertyExtentions.EnumrateArray)), false, () =>
            {
                Debug.Log(list1.EnumrateArray().Count());
            });
            menu.AddItem(new GUIContent(nameof(SerializedPropertyExtentions.CopyTo)), false, () =>
            {
                a.CopyTo(b);
                x.CopyTo(y);
                property.serializedObject.ApplyModifiedProperties();
            });
            menu.AddItem(new GUIContent(nameof(SerializedPropertyExtentions.ClearObjectByDefault)), false, () =>
            {
                b.ClearObjectByDefault();
                y.ClearObjectByDefault();
                property.serializedObject.ApplyModifiedProperties();
            });
            menu.AddItem(new GUIContent(nameof(SerializedPropertyExtentions.DumpHierarchy)), false, () =>
            {
                Debug.Log(list1.DumpHierarchy());
                Debug.Log(a.DumpHierarchy());
                Debug.Log(b.DumpHierarchy());
                Debug.Log(x.DumpHierarchy());
                Debug.Log(y.DumpHierarchy());
            });
            menu.AddItem(new GUIContent(nameof(SerializedPropertyExtentions.SortArray)), false, () =>
            {
                list1.SortArray(item => item.intValue, list2);
                property.serializedObject.ApplyModifiedProperties();
            });
            menu.ShowAsContext();
            Event.current.Use(); 
        }

        label.text += " 右クリック";
        EditorGUI.PropertyField(position, property, label, true);
    }
 
     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
     {
         return EditorGUI.GetPropertyHeight(property);
     }
}
