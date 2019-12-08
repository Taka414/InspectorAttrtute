// 
// Reference by:
// https://forum.unity.com/threads/c-7-3-field-serializefield-support.573988/
//

using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// インスペクターに自動実装プロパティをいい感じに表示するためのEditor拡張機能を表します。
/// </summary>
public sealed class InspectorAttribute : PropertyAttribute
{
    // 
    // 以下のように書くと自動実装プロパティをインスペクター上にいい感じで表示できるようになる
    // 
    // e.g.
    // [field: SerializeField]
    // [field: RenameField(nameof(Name))] // "Name"として名前が表示される
    // public string Name { get; set; }

    public string Name { get; private set; }

    public InspectorAttribute(string dispName) => this.Name = dispName;

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(InspectorAttribute))]
    public class FieldNameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Debug.Log($"{property.propertyPath}");

            if (!(this.attribute is InspectorAttribute fieldName))
            {
                return;
            }

            //Debug.Log($"{property.propertyPath}, {property.type}, {fieldName.Name}, {property.isExpanded}");

            string[] path = property.propertyPath.Split('.');

            if (!(path.Length > 1 && path[1] == "Array"))
            {
                label.text = fieldName.Name;
            }
            else if (path.Length >= 4 && path[path.Length - 3] == "Array") // child items
            {
                label.text = fieldName.Name;
            }
            //else if(path.Length == 3 && path[path.Length -2] == "Array")
            //{
            //    label.text = fieldName.Name;
            //}

            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property); // リストとかオブジェクトで高さは合うようになるけどすごく重くなる
        }
    }
#endif
}
