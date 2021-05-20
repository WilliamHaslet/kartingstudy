using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AnimationCurve))]
public class AnimationCurveDrawer : PropertyDrawer
{

    private const float fieldHeight = 20;

    private int height = 1;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        EditorGUI.BeginProperty(position, label, property);

        Rect propertyRect = position;

        propertyRect.height = fieldHeight;

        SerializedProperty rangeProperty = property.serializedObject.FindProperty(property.name + "Range");

        if (rangeProperty == null)
        {

            property.animationCurveValue = EditorGUI.CurveField(propertyRect, label, property.animationCurveValue);

            height = 1;

        }
        else
        {

            property.animationCurveValue = EditorGUI.CurveField(propertyRect, label, property.animationCurveValue, Color.green, rangeProperty.rectValue);

            propertyRect.y += 22;

            EditorGUI.indentLevel = 1;

            rangeProperty.rectValue = EditorGUI.RectField(propertyRect, rangeProperty.displayName, rangeProperty.rectValue);

            height = 3;

        }

        EditorGUI.EndProperty();

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {

        return fieldHeight * height;

    }

}
