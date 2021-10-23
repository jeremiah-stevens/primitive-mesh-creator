using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimitiveCreator.MeshCreators;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace PrimitiveCreator
{
    /// <summary>
    /// In-scene component for modifying and creating a primitive mesh. At present, this is designed to only be used in editor-mode.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class PrimitiveCreatorComponent : MonoBehaviour
    {
#if UNITY_EDITOR
        //internal references
        new MeshRenderer renderer;
        MeshFilter meshFilter;

        [Header("Model Settings")]
        [Tooltip("Type of mesh to create.")]
        [SerializeField] PrimitiveCreatorUtility.MeshType meshType = PrimitiveCreatorUtility.MeshType.Pyramid;
        PrimitiveCreatorUtility.MeshType m_meshType;
        [Tooltip("Attributes that modify the final created mesh.")]
        [SerializeReference] public MeshCreator.MeshDetails meshDetails = new MeshCreator.MeshDetails();
        [Tooltip("Actual vertex count used in the mesh. Will use the closest count under the requested amount that still provides for a viable mesh.")]
        [SerializeField, ReadOnlyInInspector] int actualVertexCount;


        [Header("Gizmos Preview")]
        [SerializeField] bool showVertices = true;
        [SerializeField] Color vertexColor = Color.black;
        [SerializeField] bool showEdges = true;
        [SerializeField] Color edgeColor = Color.red;
        [SerializeField] bool showNormals = false;
        [SerializeField] Color normalColor = Color.blue;
        [SerializeField] bool showShaded = true;
        [SerializeField] Color shadedColor = Color.grey;
        [SerializeField] bool showUnitCircle = true;
        [SerializeField] Color unitCircleColor = Color.black;
        [SerializeField, HideInInspector] Mesh previewMesh;


        [Tooltip("Folder to save the new mesh to.")]
        [SerializeField, HideInInspector] DefaultAsset saveDestination;
        [Tooltip("Name to give to the newly-saved mesh.")]
        [SerializeField, HideInInspector] string saveName = "";




        /// <summary>
        /// Updates the preview mesh based on the current set of model settings (MeshType, MeshDetails, ...).
        /// </summary>
        private void UpdatePreviewMesh()
        {
            previewMesh = PrimitiveCreatorUtility.CreateMesh(m_meshType, meshDetails);
        }

        /// <summary>
        /// Gets the currently-generated preview mesh.
        /// </summary>
        /// <returns>Most recently-generated preview mesh.</returns>
        private Mesh GetPreviewMesh()
        {
            return previewMesh;
        }

        /// <summary>
        /// Sets the MeshType and MeshDetails used in the PrimitiveCreator.
        /// </summary>
        private void SetMeshType()
        {
            m_meshType = meshType;
            meshDetails = PrimitiveCreatorUtility.GetMeshDetails(m_meshType, meshDetails);
        }




        private void Awake()
        {
            if(renderer == null)
            {
                renderer = this.GetComponent<MeshRenderer>();
                //renderer.material = PrimitiveCreatorUtility.GetDefaultMaterial();
            }

            meshFilter = this.GetComponent<MeshFilter>();
            meshFilter.mesh = GetPreviewMesh();
            SetMeshType();
        }

        private void Update()
        {
            if (m_meshType != meshType)
                SetMeshType();

            UpdatePreviewMesh();

            if (meshFilter == null) meshFilter = this.GetComponent<MeshFilter>();
            meshFilter.mesh = GetPreviewMesh();
            actualVertexCount = PrimitiveCreatorUtility.GetClosestViableVertexCount(m_meshType, meshDetails.vertexCount);
        }

        private void OnDrawGizmos()
        {
            Mesh mesh = GetPreviewMesh();

            Color prevColor = Gizmos.color;

            if (showShaded)
            {
                Gizmos.color = shadedColor;
                Gizmos.DrawMesh(mesh, this.transform.position, this.transform.rotation);
            }

            if (showEdges)
            {
                Gizmos.color = edgeColor;
                Gizmos.DrawWireMesh(mesh, this.transform.position, this.transform.rotation);
            }

            if (showVertices)
            {
                Gizmos.color = vertexColor;
                foreach (Vector3 currVert in mesh.vertices)
                    Gizmos.DrawSphere((this.transform.rotation * currVert) + this.transform.position, 0.02f);
            }

            if (showNormals)
            {
                Gizmos.color = normalColor;
                for(int v = 0; v < mesh.vertices.Length; v++)
                {
                    Vector3 p0 = (this.transform.rotation * mesh.vertices[v]) + this.transform.position;
                    Vector3 p1 = 0.25f * (this.transform.rotation * mesh.normals[v]) + p0;
                    Gizmos.DrawLine(p0, p1);
                }
            }

            if (showUnitCircle)
            {
                Gizmos.color = unitCircleColor;
                Gizmos.DrawWireSphere(this.transform.position, 0.5f);
            }

            Gizmos.color = prevColor;
        }
#endif
    }
}