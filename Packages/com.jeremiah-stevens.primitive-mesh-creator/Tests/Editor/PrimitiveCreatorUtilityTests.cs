using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PrimitiveCreator;
using PrimitiveCreator.MeshCreators;
using PrimitiveCreator.Tests;
using static PrimitiveCreator.MeshCreators.MeshCreator;

/*TODO: Test internal methods
 * MeshCreatorDictionary
 * • Validate that it checks for all available entries and adds them
 *GetMeshCreator(MeshType)
 * • Returns correct value for all types (check against all MeshTypes and verify with correct type)
 * • Calls initialization when meshCreator is null or no entries included
 * • Returns an exception if entry doesn't exist (ex: enum value out of bounds)
 *    
 * GetDefaultMaterial()
 * • Returns URP/Lit for now; will need to set up support for other render pipelines
 */
namespace PrimitiveCreator
{
    /// <summary>
    /// Test suite for the PrimitiveCreatorUtility interface.
    /// </summary>
    public class PrimitiveCreatorUtilityTests : MonoBehaviour
    {
        private List<MeshCreator> GetMeshCreators()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(MeshCreator));

            var meshCreatorTypes = assembly.GetTypes()
                .Where(t => typeof(MeshCreator).IsAssignableFrom(t) && !t.IsAbstract);

            List<MeshCreator> output = new List<MeshCreator>();
            foreach (var meshCreatorType in meshCreatorTypes)
            {
                output.Add(Activator.CreateInstance(meshCreatorType) as MeshCreator);
            }

            return output;
        }




        #region GetDisplayName(MeshType)

        [Test]
        public void GetDisplayNameThrowsExceptionWhenMeshCreatorDoesntExist()
        {
            Assert.That(() => PrimitiveCreatorUtility.GetDisplayName((PrimitiveCreatorUtility.MeshType)int.MaxValue),
                      Throws.TypeOf<KeyNotFoundException>());
        }

        [Test]
        public void GetDisplayNameReturnsProperValueForEachMeshType()
        {
            List<MeshCreator> meshCreators = GetMeshCreators();

            foreach (PrimitiveCreatorUtility.MeshType meshType in Enum.GetValues(typeof(PrimitiveCreatorUtility.MeshType)))
            {
                MeshCreator creator = meshCreators.Find(m => m.MeshType == meshType);

                if (creator == null)
                    Assert.Fail($"No MeshCreator exists for MeshType '{meshType}'.");
                else
                    Assert.AreEqual(creator.DisplayName, PrimitiveCreatorUtility.GetDisplayName(meshType));
            }
        }

        #endregion



        #region CreateMesh(MeshType)

        [Test]
        public void CreateMeshThrowsExceptionWhenMeshCreatorDoesntExist()
        {
            Assert.That(() => PrimitiveCreatorUtility.CreateMesh((PrimitiveCreatorUtility.MeshType)int.MaxValue),
              Throws.TypeOf<KeyNotFoundException>());
        }

        [Test]
        public void CreateMeshReturnsProperValueForEachMeshType()
        {
            List<MeshCreator> meshCreators = GetMeshCreators();

            foreach (PrimitiveCreatorUtility.MeshType meshType in Enum.GetValues(typeof(PrimitiveCreatorUtility.MeshType)))
            {
                MeshCreator creator = meshCreators.Find(m => m.MeshType == meshType);

                if (creator == null)
                    Assert.Fail($"No MeshCreator exists for MeshType '{meshType}'.");
                else
                    TestUtilities.AssertMeshesAreApproximatelyTheSame(creator.CreateMesh(), PrimitiveCreatorUtility.CreateMesh(meshType));
            }
        }

        #endregion



        #region CreateMesh(MeshType, MeshDetails)

        [Test]
        public void CreateMeshWithMeshDetailsThrowsExceptionWhenMeshCreatorDoesntExist()
        {
            MeshDetails meshDetails = new MeshDetails();
            meshDetails.vertexCount = 100;
            meshDetails.translation = new Vector3(-5, 100f, 23f);
            meshDetails.rotation = Quaternion.Euler(30f, -230f, 76.23f);
            meshDetails.scale = new Vector3(0.23f, 4f, 1.423f);

            Assert.That(() => PrimitiveCreatorUtility.CreateMesh((PrimitiveCreatorUtility.MeshType)int.MaxValue, meshDetails),
              Throws.TypeOf<KeyNotFoundException>());
        }

        [Test]
        public void CreateMeshWithMeshDetailsReturnsProperValueForEachMeshType()
        {
            MeshDetails meshDetails = new MeshDetails();
            meshDetails.vertexCount = 100;
            meshDetails.translation = new Vector3(-5, 100f, 23f);
            meshDetails.rotation = Quaternion.Euler(30f, -230f, 76.23f);
            meshDetails.scale = new Vector3(0.23f, 4f, 1.423f);

            List<MeshCreator> meshCreators = GetMeshCreators();

            foreach (PrimitiveCreatorUtility.MeshType meshType in Enum.GetValues(typeof(PrimitiveCreatorUtility.MeshType)))
            {
                MeshCreator creator = meshCreators.Find(m => m.MeshType == meshType);

                if (creator == null)
                    Assert.Fail($"No MeshCreator exists for MeshType '{meshType}'.");

                MeshDetails currMeshDetails = creator.GetMeshDetails(meshDetails);

                TestUtilities.AssertMeshesAreApproximatelyTheSame(creator.CreateMesh(currMeshDetails), PrimitiveCreatorUtility.CreateMesh(meshType, currMeshDetails));
            }
        }

        #endregion



        #region GetMeshDetails(MeshType, MeshDetails)

        [Test]
        public void GetMeshDetailsThrowsExceptionWhenMeshCreatorDoesntExist()
        {
            Assert.That(() => PrimitiveCreatorUtility.GetMeshDetails((PrimitiveCreatorUtility.MeshType)int.MaxValue),
              Throws.TypeOf<KeyNotFoundException>());
        }

        [Test]
        public void GetMeshDetailsReturnsProperValueForEachMeshType()
        {
            List<MeshCreator> meshCreators = GetMeshCreators();

            foreach (PrimitiveCreatorUtility.MeshType meshType in Enum.GetValues(typeof(PrimitiveCreatorUtility.MeshType)))
            {
                MeshCreator creator = meshCreators.Find(m => m.MeshType == meshType);

                if (creator == null)
                    Assert.Fail($"No MeshCreator exists for MeshType '{meshType}'.");

                //TODO: should replace this with thorough testing of the actual properties themselves. Requires a value-based comparison though
                Assert.AreEqual(creator.GetMeshDetails().GetType(), PrimitiveCreatorUtility.GetMeshDetails(meshType).GetType());
            }
        }

        [Test]
        public void GetMeshDetailsCopiesPreviousMeshDetailsProperlyForEachMeshType()
        {
            MeshDetails meshDetails = new MeshDetails();
            meshDetails.vertexCount = 51234;
            meshDetails.translation = new Vector3(-5, 100f, 23f);
            meshDetails.rotation = Quaternion.Euler(30f, -230f, 76.23f);
            meshDetails.scale = new Vector3(0.23f, 4f, 1.423f);

            List<MeshCreator> meshCreators = GetMeshCreators();

            foreach (PrimitiveCreatorUtility.MeshType meshType in Enum.GetValues(typeof(PrimitiveCreatorUtility.MeshType)))
            {
                MeshCreator creator = meshCreators.Find(m => m.MeshType == meshType);

                if (creator == null)
                    Assert.Fail($"No MeshCreator exists for MeshType '{meshType}'.");

                MeshDetails actual = PrimitiveCreatorUtility.GetMeshDetails(meshType, meshDetails);

                Assert.AreEqual(meshDetails.vertexCount, actual.vertexCount);
                Assert.Less(Vector3.Distance(meshDetails.translation, actual.translation), Mathf.Epsilon);
                Assert.Less(Quaternion.Angle(meshDetails.rotation, actual.rotation), Mathf.Epsilon);
                Assert.Less(Vector3.Distance(meshDetails.scale, actual.scale), Mathf.Epsilon);
            }
        }

        #endregion



        #region GetClosestViableVertexCount(MeshType, int)

        [Test]
        public void GetClosestViableVertexCountThrowsExceptionWhenMeshCreatorDoesntExist()
        {
            Assert.That(() => PrimitiveCreatorUtility.GetClosestViableVertexCount((PrimitiveCreatorUtility.MeshType)int.MaxValue, 0),
              Throws.TypeOf<KeyNotFoundException>());
        }

        [Test]
        public void GetClosestViableVertexCountReturnsProperValueForEachMeshType()
        {
            int vertexCount = 5201;

            List<MeshCreator> meshCreators = GetMeshCreators();

            foreach (PrimitiveCreatorUtility.MeshType meshType in Enum.GetValues(typeof(PrimitiveCreatorUtility.MeshType)))
            {
                MeshCreator creator = meshCreators.Find(m => m.MeshType == meshType);

                if (creator == null)
                    Assert.Fail($"No MeshCreator exists for MeshType '{meshType}'.");

                Assert.AreEqual(creator.GetClosestViableVertexCount(vertexCount), PrimitiveCreatorUtility.GetClosestViableVertexCount(meshType, vertexCount));
            }
        }

        #endregion



        #region CreatePrimitive(MeshType)

        [Test]
        public void CreatePrimitiveThrowsExceptionWhenMeshCreatorDoesntExist()
        {
            Assert.That(() => PrimitiveCreatorUtility.CreatePrimitive((PrimitiveCreatorUtility.MeshType)int.MaxValue),
              Throws.TypeOf<KeyNotFoundException>());
        }

        [Test]
        public void CreatePrimitiveReturnsProperValueForEachMeshType()
        {
            List<MeshCreator> meshCreators = GetMeshCreators();

            foreach (PrimitiveCreatorUtility.MeshType meshType in Enum.GetValues(typeof(PrimitiveCreatorUtility.MeshType)))
            {
                MeshCreator creator = meshCreators.Find(m => m.MeshType == meshType);

                if (creator == null)
                    Assert.Fail($"No MeshCreator exists for MeshType '{meshType}'.");

                GameObject actual = PrimitiveCreatorUtility.CreatePrimitive(meshType);

                Assert.AreEqual(creator.DisplayName, actual.name);
                Assert.NotNull(actual.GetComponent<MeshRenderer>());
                Assert.NotNull(actual.GetComponent<MeshFilter>());
                TestUtilities.AssertMeshesAreApproximatelyTheSame(creator.CreateMesh(), actual.GetComponent<MeshFilter>().sharedMesh);

                //physics collider
                MeshCollider actualMeshCollider = actual.GetComponent<MeshCollider>();
                Assert.NotNull(actualMeshCollider);
                TestUtilities.AssertMeshesAreApproximatelyTheSame(creator.CreateColliderMesh(), actualMeshCollider.sharedMesh);
                Assert.IsTrue(actualMeshCollider.convex);
            }
        }

        #endregion



        #region AddPrimitiveCollider(MeshType, GameObject)

        [Test]
        public void AddPrimitiveColliderThrowsExceptionWhenMeshCreatorDoesntExist()
        {
            Assert.That(() => PrimitiveCreatorUtility.AddPrimitiveCollider((PrimitiveCreatorUtility.MeshType)int.MaxValue, new GameObject()),
              Throws.TypeOf<KeyNotFoundException>());
        }

        [Test]
        public void AddPrimitiveColliderThrowsExceptionWhenGameObjectIsNull()
        {
            Assert.That(() => PrimitiveCreatorUtility.AddPrimitiveCollider(PrimitiveCreatorUtility.MeshType.Triangle, null),
                Throws.TypeOf<NullReferenceException>());
        }

        [Test]
        public void AddPrimitiveColliderReturnsProperValueForEachMeshType()
        {
            List<MeshCreator> meshCreators = GetMeshCreators();

            foreach (PrimitiveCreatorUtility.MeshType meshType in Enum.GetValues(typeof(PrimitiveCreatorUtility.MeshType)))
            {
                MeshCreator creator = meshCreators.Find(m => m.MeshType == meshType);

                if (creator == null)
                    Assert.Fail($"No MeshCreator exists for MeshType '{meshType}'.");

                GameObject obj = new GameObject();
                PrimitiveCreatorUtility.AddPrimitiveCollider(meshType, obj);
                MeshCollider collider = obj.GetComponent<MeshCollider>();
                Assert.NotNull(collider);
                TestUtilities.AssertMeshesAreApproximatelyTheSame(creator.CreateColliderMesh(), collider.sharedMesh);
                Assert.IsTrue(collider.convex);
            }
        }

        #endregion
    }
}