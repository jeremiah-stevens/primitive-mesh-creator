#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using static PrimitiveCreator.PrimitiveCreatorUtility;

namespace PrimitiveCreator
{
    [CustomEditor(typeof(PrimitiveCreatorConfig))]
    public class PrimitiveCreatorConfigEditor : Editor
    {
        SerializedProperty pluginRootFolderProp;



        /// <summary>
        /// Saves a default mesh for each type of MeshCreator. If a mesh already exists, it will overwrite the mesh data with a new mesh.
        /// </summary>
        private void SaveAllDefaultMeshes()
        {
            if (pluginRootFolderProp.objectReferenceValue == null) throw new System.NullReferenceException("No save destination defined. Please select a valid folder and try again.");

            DefaultAsset destinationFolder = (DefaultAsset)pluginRootFolderProp.objectReferenceValue;

            //rebuilds path to destination folder
            string localDirectoryPath = $"{AssetDatabase.GetAssetPath((DefaultAsset)pluginRootFolderProp.objectReferenceValue)}/Resources/{PrimitiveCreatorConfig.SaveSubpathMesh}";
            BuildDirectoryPath(localDirectoryPath);

            foreach (MeshType meshType in System.Enum.GetValues(typeof(MeshType)))
            {
                string destination = $"{localDirectoryPath}/{PrimitiveCreatorUtility.GetDisplayName(meshType)}.mesh";
                Mesh currSavedMesh = (Mesh)AssetDatabase.LoadAssetAtPath(destination, typeof(Mesh));

                if (currSavedMesh != null) //asset already exists, override data
                {
                    Mesh mesh = PrimitiveCreatorUtility.CreateMesh(meshType, PrimitiveCreatorUtility.GetMeshDetails(meshType));

                    CopyMeshData(ref currSavedMesh, ref mesh);
                    DestroyImmediate(mesh); //clean up mesh after we got the data we needed
                }
                else //asset does not exist, just write directly into it
                {
                    Mesh mesh = PrimitiveCreatorUtility.CreateMesh(meshType, PrimitiveCreatorUtility.GetMeshDetails(meshType));
                    MeshSaver.SaveMesh(mesh, destination);
                }
            }
        }

        /// <summary>
        /// Saves a collider-optimized mesh for each type of MeshCreator. If a mesh already exists, it will overwrite the mesh data with a new mesh.
        /// </summary>
        private void SaveAllColliderMeshes()
        {
            if (pluginRootFolderProp.objectReferenceValue == null) throw new System.NullReferenceException("No save destination defined. Please select a valid folder and try again.");

            DefaultAsset destinationFolder = (DefaultAsset)pluginRootFolderProp.objectReferenceValue;

            //rebuilds path to destination folder
            string localDirectoryPath = $"{AssetDatabase.GetAssetPath((DefaultAsset)pluginRootFolderProp.objectReferenceValue)}/Resources/{PrimitiveCreatorConfig.SaveSubpathColliders}";
            BuildDirectoryPath(localDirectoryPath);

            foreach (MeshType meshType in System.Enum.GetValues(typeof(MeshType)))
            {
                string destination = $"{localDirectoryPath}/{PrimitiveCreatorUtility.GetDisplayName(meshType)} (Collider).mesh";
                Mesh currSavedMesh = (Mesh)AssetDatabase.LoadAssetAtPath(destination, typeof(Mesh));

                if (currSavedMesh != null) //asset already exists, override data
                {
                    Mesh mesh = PrimitiveCreatorUtility.CreateColliderMesh(meshType, PrimitiveCreatorUtility.GetMeshDetails(meshType));

                    CopyMeshData(ref currSavedMesh, ref mesh);
                    DestroyImmediate(mesh); //clean up mesh after we got the data we needed
                }
                else //asset does not exist, just write directly into it
                {
                    Mesh mesh = PrimitiveCreatorUtility.CreateColliderMesh(meshType, PrimitiveCreatorUtility.GetMeshDetails(meshType));
                    MeshSaver.SaveMesh(mesh, destination);
                }
            }
        }

        /// <summary>
        /// Builds the set of folders along the directoryPath.
        /// </summary>
        /// <param name="directoryPath">Valid directory path to build folders along. Should start from Assets folder and lead to desired folder (ex: 'Assets/Art/Models').</param>
        private void BuildDirectoryPath(string directoryPath)
        {
            string[] subDirectories = directoryPath.Split('/');
            string currPath = subDirectories[0]; //'Assets' folder
            for(int s = 1; s < subDirectories.Length; s++)
            {
                string currSubpath = subDirectories[s];
                if(!AssetDatabase.IsValidFolder($"{currPath}/{currSubpath}")) //if the folder does not exist, make it
                    AssetDatabase.CreateFolder(currPath, currSubpath);
                currPath = $"{currPath}/{currSubpath}";
            }
        }

        /// <summary>
        /// Copies the data from the meshToReadFrom into the meshToWriteTo, while retaining the underlying instance of the meshToWriteTo.
        /// </summary>
        /// <param name="meshToWriteTo">Mesh that the mesh data should be written to.</param>
        /// <param name="meshToReadFrom">Mesh that mesh data should be read from.</param>
        private void CopyMeshData(ref Mesh meshToWriteTo, ref Mesh meshToReadFrom)
        {
            meshToWriteTo.Clear();

            meshToWriteTo.vertices = meshToReadFrom.vertices;
            meshToWriteTo.uv = meshToReadFrom.uv;
            meshToWriteTo.uv2 = meshToReadFrom.uv2;
            meshToWriteTo.uv3 = meshToReadFrom.uv3;
            meshToWriteTo.uv4 = meshToReadFrom.uv4;
            meshToWriteTo.uv5 = meshToReadFrom.uv5;
            meshToWriteTo.uv6 = meshToReadFrom.uv6;
            meshToWriteTo.uv7 = meshToReadFrom.uv7;
            meshToWriteTo.uv8 = meshToReadFrom.uv8;
            meshToWriteTo.triangles = meshToReadFrom.triangles;
            meshToWriteTo.normals = meshToReadFrom.normals;
            meshToWriteTo.tangents = meshToReadFrom.tangents;
        }




        /// <summary>
        /// Renders the GUI elements for saving the Default Meshes.
        /// </summary>
        private void GetGUIMeshSaving()
        {
            GUILayout.Label("Mesh Saving", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GUIStyle italicLabel = new GUIStyle(GUI.skin.label);
            italicLabel.fontStyle = FontStyle.Italic;
            italicLabel.wordWrap = true;

            GUILayout.Label(
                            "This section allows you to update the default meshes used for creating the models and their colliders. " +
                            "Note that these operations rewrite the data of existing meshes without replacing the underlying GUID (i.e. same Mesh, new mesh data). " +
                            "Any objects currently referencing these Meshes will now use the newly rewritten mesh data.",
                            italicLabel
                           );

            GUILayout.Space(10f);

            EditorGUILayout.PropertyField(pluginRootFolderProp);

            //button for saving default meshes
            if (GUILayout.Button("Save All Default Meshes"))
                SaveAllDefaultMeshes();

            GUILayout.Space(10f);

            if (GUILayout.Button("Save All Collider Meshes"))
                SaveAllColliderMeshes();
        }




        private void OnEnable()
        {
            pluginRootFolderProp = serializedObject.FindProperty("pluginRootFolder");
        }

        public override void OnInspectorGUI()
        {
            //base inspector
            base.OnInspectorGUI();

            GUILayout.Space(30f);

            GetGUIMeshSaving();

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif