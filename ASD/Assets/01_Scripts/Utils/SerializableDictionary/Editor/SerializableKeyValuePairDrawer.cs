using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializableKeyValuePair<,>), true)]
public class SerializableKeyValuePairDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUILayout.BeginHorizontal();
        {
            SerializedProperty keyProperty = property.FindPropertyRelative("Key");
            SerializedProperty valueProperty = property.FindPropertyRelative("Value");

            EditorGUILayout.PropertyField(keyProperty, GUIContent.none, true);
            EditorGUILayout.PropertyField(valueProperty, GUIContent.none, true);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUI.EndProperty();
    }

}
