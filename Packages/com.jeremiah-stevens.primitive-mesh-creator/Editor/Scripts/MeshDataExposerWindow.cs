#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PrimitiveCreator
{
    public class MeshDataExposerWindow : EditorWindow
    {
        SerializedObject serializedObject;

        public Mesh meshToExpose;

        Vector2 scrollPosition = Vector2.zero;




        [MenuItem("Window/Primitive Creator/Mesh Data Exposer")]
        static void Init()
        {
            MeshDataExposerWindow window = (MeshDataExposerWindow)EditorWindow.GetWindow(typeof(MeshDataExposerWindow), false, "Mesh Data Exposer");
            window.Show();
        }


        private void GetMeshDetailsSection<T>(string label, List<T> elements)
        {
            GetMeshDetailsSection<T>(label, elements.ToArray());
        }

        private void GetMeshDetailsSection<T>(string label, T[] elements)
        {
            GUILayout.Label(label, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (meshToExpose != null)
            {
                string content = "";
                for (int i = 0; i < elements.Length; i++)
                    content += $"\n[{i}] {elements[i]}";
                if(content.Length > 2) //strip off the first newline
                    content = content.Substring(2);
                GUILayout.Label(content);
            }
        }




        private void Awake()
        {
            serializedObject = new SerializedObject(this);
        }

        private void OnGUI()
        {
            if (serializedObject == null) serializedObject = new SerializedObject(this);

            float space = 25f;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            GUILayout.Space(space);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("meshToExpose"));

            GUILayout.Space(space);
            GetMeshDetailsSection<Vector3>("Vertices", meshToExpose.vertices);

            GUILayout.Space(space);
            GetMeshDetailsSection<Vector2>("UV0", meshToExpose.uv);

            GUILayout.Space(space);
            GetMeshDetailsSection<Vector2>("UV2", meshToExpose.uv2);

            GUILayout.EndScrollView();

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif