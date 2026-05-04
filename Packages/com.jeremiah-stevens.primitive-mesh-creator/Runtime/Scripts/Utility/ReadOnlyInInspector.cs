using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


//TODO: need to find some way to force ReadOnlyInInspector to also use SerializeField

/// <summary>
/// Provides a property attribute that is visible in the editor, but unable to be modified.
/// </summary>
public class ReadOnlyInInspectorAttribute : PropertyAttribute { }


#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ReadOnlyInInspectorAttribute))]
public class ReadOnlyInInspectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}

#endif