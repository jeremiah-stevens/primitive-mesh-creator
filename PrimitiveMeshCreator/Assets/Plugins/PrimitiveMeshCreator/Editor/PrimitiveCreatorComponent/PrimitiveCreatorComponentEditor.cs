#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Reflection;


namespace PrimitiveCreator
{
    [CustomEditor(typeof(PrimitiveCreatorComponent))]
    public class PrimitiveCreatorComponentEditor : Editor
    {
        SerializedProperty meshProp;
        SerializedProperty saveDestinationProp;
        SerializedProperty saveNameProp;




        private void SaveMesh()
        {
            if (saveDestinationProp.objectReferenceValue == null) throw new System.NullReferenceException("No save destination defined. Please select a valid folder and try again.");

            string savePath = $"{AssetDatabase.GetAssetPath((DefaultAsset)saveDestinationProp.objectReferenceValue)}/{saveNameProp.stringValue}.mesh";

            MeshSaver.SaveMesh((Mesh)meshProp.objectReferenceValue, savePath);
        }



        /// <summary>
        /// Gets the GUI items for saving the model as a UnityAsset.
        /// </summary>
        private void GetGUIModelSaving()
        {
            GUILayout.Label("Model Saving", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            EditorGUILayout.PropertyField(saveDestinationProp);
            EditorGUILayout.PropertyField(saveNameProp);

            if (GUILayout.Button("Save Model"))
                SaveMesh();
        }




        private void OnEnable()
        {
            meshProp = serializedObject.FindProperty("previewMesh");
            saveDestinationProp = serializedObject.FindProperty("saveDestination");
            saveNameProp = serializedObject.FindProperty("saveName");
        }

        public override void OnInspectorGUI()
        {
            //base inspector
            base.OnInspectorGUI();

            GUILayout.Space(30f);

            //Model Saving Functionality
            GetGUIModelSaving();

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif