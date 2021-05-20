using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Layer))]
public class LayerDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        EditorGUI.BeginProperty(position, label, property);

        Rect propertyRect = position;

        SerializedProperty layer = property.FindPropertyRelative("value");

        layer.intValue = EditorGUI.LayerField(propertyRect, label, layer.intValue);

        EditorGUI.EndProperty();

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {

        return 18;

    }

}
