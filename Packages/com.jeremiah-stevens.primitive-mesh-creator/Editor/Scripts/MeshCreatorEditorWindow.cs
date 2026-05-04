#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;
using PrimitiveCreator.MeshCreators;


namespace PrimitiveCreator
{
    public class MeshCreatorEditorWindow : EditorWindow
    {
        //mesh to generate
        SerializedObject serializedObject;
        PrimitiveCreatorUtility.MeshType m_meshType;
        PrimitiveCreatorUtility.MeshType meshType;
        public MeshCreator.MeshDetails meshDetails;
        int actualVertexCount;

        //saving settings
        DefaultAsset saveDestination = null;
        string saveName = "";

        //preview items
        Mesh previewMesh;
        Editor previewMeshEditor;



        [MenuItem("Window/Primitive Creator/(Experimental) Primitive Mesh Creator")]
        static void Init()
        {
            MeshCreatorEditorWindow window = (MeshCreatorEditorWindow)EditorWindow.GetWindow(typeof(MeshCreatorEditorWindow), false, "Primitive Mesh Creator");
            window.Show();
        }



        private void SaveMesh()
        {
            if (saveDestination == null) throw new System.NullReferenceException("No save destination defined. Please select a valid folder and try again.");

            Mesh mesh = GetMesh();
            string savePath = $"{AssetDatabase.GetAssetPath(saveDestination)}/{saveName}.mesh";

            AssetDatabase.CreateAsset(mesh, savePath);

            Debug.Log($"Mesh saved to '{savePath}'.");
        }

        private Mesh GetMesh()
        {
            return PrimitiveCreatorUtility.CreateMesh(meshType, meshDetails);
        }

        private void SetMeshDetails()
        {
            m_meshType = meshType;
            meshDetails = PrimitiveCreatorUtility.GetMeshDetails(m_meshType, meshDetails);
        }



        #region GUI Sections
        /// <summary>
        /// Gets the GUI items for the mesh configuration settings (translation, scale, vertex count, etc.)
        /// </summary>
        private void GetGUIMeshOptions()
        {
            GUILayout.Label("Mesh Options", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //option for changing mesh type
            meshType = (PrimitiveCreatorUtility.MeshType)EditorGUILayout.EnumPopup("Primitive Mesh", meshType);

            if (m_meshType != meshType)
                SetMeshDetails();
            //TODO: update meshDetails to show custom properties of MeshDetails (ex: width/height for MeshDetailsQuad)


            EditorGUILayout.PropertyField(serializedObject.FindProperty("meshDetails"));

            serializedObject.ApplyModifiedProperties();

            actualVertexCount = PrimitiveCreatorUtility.GetClosestViableVertexCount(meshType, meshDetails.vertexCount);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.IntField("Actual Vertex Count", actualVertexCount);
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// Gets the GUI items for saving the model as a UnityAsset.
        /// </summary>
        private void GetGUIModelSaving()
        {
            GUILayout.Label("Model Saving", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            saveDestination = (DefaultAsset)EditorGUILayout.ObjectField("Save Destination", saveDestination, typeof(DefaultAsset), false); //TODO: validation that this is a folder
            saveName = EditorGUILayout.TextField("Save Name", saveName);

            if (GUILayout.Button("Save Model"))
                SaveMesh();
        }

        /// <summary>
        /// Gets the GUI items for previewing the mesh in the EditorWindow
        /// </summary>
        private void GetGUIModelPreview()
        {
            /* Mesh Preview */
            GUILayout.Label("Preview", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            if (GUILayout.Button("Update Preview") || previewMeshEditor == null)
            {
                previewMesh = GetMesh();
                previewMeshEditor = Editor.CreateEditor(previewMesh);
            }

            GUILayout.Space(10f);

            //TODO: have this slotted at the bottom of the window with a slider to scale it up
            previewMeshEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(200, 400), GUIStyle.none);
        }

        #endregion




        private void Awake()
        {
            serializedObject = new SerializedObject(this);
            SetMeshDetails();
        }

        private void OnGUI()
        {
            //set the serialized object
            if (serializedObject == null) serializedObject = new SerializedObject(this);


            GetGUIMeshOptions();

            GUILayout.Space(30f);

            GetGUIModelSaving();

            GUILayout.Space(30f);

            GetGUIModelPreview();
        }
    }
}

#endif