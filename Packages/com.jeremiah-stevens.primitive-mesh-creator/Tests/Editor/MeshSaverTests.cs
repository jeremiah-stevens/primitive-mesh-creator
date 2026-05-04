using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using PrimitiveCreator;

namespace PrimitiveCreator
{
    public class MeshSaverTests
    {
        [Test]
        public void MeshSaverThrowsExceptionWhenMeshIsNull()
        {
            string destination = "Assets/itsANewMesh.mesh";
            Assert.That(() => MeshSaver.SaveMesh(null, destination),
                      Throws.TypeOf<System.NullReferenceException>());
        }

        [Test]
        public void MeshSaverThrowsExceptionWhenPathIsNull()
        {
            Mesh mesh = new Mesh();
            Assert.That(() => MeshSaver.SaveMesh(mesh, null),
                      Throws.TypeOf<System.NullReferenceException>());
        }

        [Test]
        public void MeshSaverSuccessfullySavesFileToProvidedPath()
        {
            Mesh mesh = new Mesh();
            string destination = "Assets/itsANewMesh.mesh";

            MeshSaver.SaveMesh(mesh, destination);

            Mesh savedMesh = (Mesh)AssetDatabase.LoadAssetAtPath(destination, typeof(Mesh));

            Assert.IsNotNull(savedMesh);

            //clean-up
            AssetDatabase.DeleteAsset(destination);
        }
    }
}