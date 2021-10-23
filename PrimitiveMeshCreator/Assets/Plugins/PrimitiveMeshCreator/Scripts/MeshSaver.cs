#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PrimitiveCreator
{
    /// <summary>
    /// Provides saving operations for meshes created within the tool.
    /// </summary>
    public static class MeshSaver
    {
        /// <summary>
        /// Saves the provided mesh at the provided destination.
        /// </summary>
        /// <param name="mesh">Mesh to save.</param>
        /// <param name="destination">Directory path to save the mesh at.</param>
        public static void SaveMesh(Mesh mesh, string destination)
        {
            if (mesh == null)
                throw new System.NullReferenceException("Mesh provided is null. Please provide a valid mesh.");
            if (destination == null)
                throw new System.NullReferenceException("Provided destination path is null. Please provide a valid path.");

            AssetDatabase.CreateAsset(mesh, destination);

            Debug.Log($"Mesh saved to '{destination}'.");
        }
    }
}

#endif