// 
// Reference by:
// https://forum.unity.com/threads/c-7-3-field-serializefield-support.573988/
//

using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// インスペクターに自動実装プロパティをいい感じに表示するためのEditor拡張機能を表します。
/// </summary>
public sealed class InspectorAttribute : PropertyAttribute
{
    //
    // Descriptions
    // - - - - - - - - - - - - - - - - - - - -

    // 
    // 以下のように書くと自動実装プロパティをインスペクター上にいい感じで表示できるようになる
    // 
    // e.g.
    // [field: SerializeField]
    // [field: RenameField(nameof(Name))] // "Name"として名前が表示される
    // public string Name { get; set; }

    // 
    // 読み取り専用の場合
    // 
    // e.g.
    // [field: SerializeField]
    // [field: RenameField(nameof(Name), true)] // 読み取り専用フラグの指定
    // public string Name { get; set; }

    // 
    // ツールチップを指定する場合
    // 
    // e.g.
    // [field: SerializeField]
    // [field: RenameField(nameof(Name), "ツールチップに表示する文字列")]
    // public string Name { get; set; }

    //
    // Props
    // - - - - - - - - - - - - - - - - - - - -

    /// <summary>
    /// インスペクター上に表示する名前を取得します。
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// プロパティが読み取り専用かどうかを取得します。
    /// true : 読み取り専用 / false : それ以外
    /// </summary>
    public bool Readonly { get; private set; }

    /// <summary>
    /// ツールチップを使用するかどうかのフラグを取得します。
    /// true : 使用する / false : それ以外
    /// </summary>
    public bool UseTooltip { get; private set; }

    /// <summary>
    /// ツールチップに表示する内容を設定します。
    /// </summary>
    public string TooltipContents { get; private set; }

    //
    // Constructors
    // - - - - - - - - - - - - - - - - - - - -

    public InspectorAttribute(string dispName, bool isReadonly = false)
    {
        this.Name = dispName;
        this.Readonly = isReadonly;
    }

    public InspectorAttribute(string dispName, string toolTip, bool isReadonly = false) : this(dispName, isReadonly)
    {
        this.UseTooltip = true;
        this.TooltipContents = toolTip;
    }

    //
    // Others
    // - - - - - - - - - - - - - - - - - - - -

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

            string[] path = property.propertyPath.Split('.');

            if (path.Length > 1 && path[path.Length - 2] == "Array")
            {
                label.text = "Element " + path[path.Length - 1].Remove(0, 5).TrimEnd(']');
                //Debug.Log($"{property.propertyPath}");
            }
            else if (!(path.Length > 1 && path[1] == "Array"))
            {
                label.text = OptimizeString(fieldName.Name);
                //Debug.Log($"a {label.text}");
            }
            else if (path.Length >= 4 && path[path.Length - 3] == "Array") // child items
            {
                label.text = OptimizeString(fieldName.Name);
                //Debug.Log($"b {label.text}");
            }
            //else if(path.Length == 3 && path[path.Length -2] == "Array")
            //{
            //    label.text = fieldName.Name;
            //}

            if (fieldName.Readonly)
            {
                GUI.enabled = false;
            }

            if (fieldName.UseTooltip)
            {
                label.tooltip = fieldName.TooltipContents;
            }

            EditorGUI.PropertyField(position, property, label, true);
            
            GUI.enabled = true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property); // リストとかオブジェクトで高さは合うようになるけど少し動作が重い
        }

        // Compatibility naming convention with the Editor
        private static StringBuilder sb = new StringBuilder();
        public static string OptimizeString(string str)
        {
            if (str.Length <= 1)
            {
                return str;
            }

            sb.Clear();
            str = str.TrimStart('_'); // leading underscre are remove.
            sb.Append(char.ToUpperInvariant(str[0]));

            for (int i = 1; i < str.Length; i++)
            {
                char a = str[i];
                char b = str[i - 1];

                if (char.IsUpper(a) && char.IsLower(b))
                {
                    sb.Append(' ');
                }
                sb.Append(a);
            }

            return sb.ToString();
        }
    }

#endif
}
