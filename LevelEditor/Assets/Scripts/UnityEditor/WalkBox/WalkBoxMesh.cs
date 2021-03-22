using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class WalkBoxMesh : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> editor_wboxVertexGameobjects;

    [HideInInspector]
    public bool updateMeshEveryFrame = false;

    [HideInInspector]
    public bool showMeshPreview = false;

    [HideInInspector]
    public bool showVertexName = false;

    [HideInInspector]
    public bool showVertexPoint = false;

    [HideInInspector]
    public float vertexGizmoSize = 0.1f;

    [HideInInspector]
    public bool forceNormalDirectionCalculation = false;

    [HideInInspector]
    public Vector3 forceNormalDirection = Vector3.up;

    [HideInInspector]
    public float normalAngle;

    private Mesh mesh;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private void Awake()
    {
        editor_wboxVertexGameobjects = new List<GameObject>();

        GenerateMesh();
    }

    private void Update()
    {
        if (editor_wboxVertexGameobjects != null && updateMeshEveryFrame)
        {
            UpdateChildren();

            if (updateMeshEveryFrame)
            {
                GenerateMesh();

                if (showMeshPreview)
                    ShowMeshPreview();
                else
                {
                    if (meshRenderer != null)
                        DestroyImmediate(meshRenderer);

                    if (meshFilter != null)
                        DestroyImmediate(meshFilter);
                }
            }
        }
    }

    public void ShowMeshPreview()
    {
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        meshFilter.mesh = mesh;

        if(meshRenderer.sharedMaterial == null)
        {
            meshRenderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
        }
    }

    public void GenerateMesh()
    {
        if(editor_wboxVertexGameobjects.Count < 3 || editor_wboxVertexGameobjects.Count % 3 != 0)
        {
            return;
        }

        mesh = new Mesh();
        mesh.RecalculateBounds();

        mesh.name = name;

        List<Vector3> vertexPositions = new List<Vector3>();
        //List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        int index = 0;

        foreach(GameObject obj in editor_wboxVertexGameobjects)
        {
            vertexPositions.Add(obj.transform.position);
            triangles.Add(index);
            //uvs.Add(new Vector2());
            index++;
        }

        mesh.vertices = vertexPositions.ToArray();

        mesh.triangles = triangles.ToArray();

        //mesh.SetUVs(0, uvs);

        mesh.RecalculateBounds();


        mesh.RecalculateUVDistributionMetrics(mesh.bounds.size.magnitude);
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();

        mesh.Optimize();
        mesh.OptimizeIndexBuffers();
        mesh.OptimizeReorderVertexBuffer();

        /*
        if (forceNormalDirectionCalculation)
        {
            Vector3[] normalVectors = new Vector3[mesh.vertexCount];

            for (int i = 0; i < normalVectors.Length; i++)
            {
                normalVectors[i] = forceNormalDirection;
            }

            mesh.normals = normalVectors;
        }
        */
    }

    public void RecalcNormals()
    {
        mesh.RecalculateNormals(); //old unity method

        if (meshFilter != null)
            meshFilter.mesh = mesh;
    }

    public void AddVertexGameObject()
    {
        string newName = string.Format("vertex{0}", editor_wboxVertexGameobjects.Count);

        GameObject vertexObj = new GameObject(newName);

        vertexObj.transform.SetParent(transform);
        if (Selection.activeGameObject != null)
        {
            vertexObj.transform.position = Selection.activeGameObject.transform.position;
        }

        Selection.activeGameObject = vertexObj;

        UpdateChildren();
    }

    public void RemoveSelectedVertex()
    {
        GameObject selectedObj = Selection.activeGameObject;

        if (selectedObj == null)
        {
            string message = string.Format("Select a vertex gameobject of the current walkbox gameobject '{0}' to remove a vertex!", name);
            EditorUtility.DisplayDialog("Error", message, "OK");
        }
        else if (selectedObj.transform.parent != transform)
        {
            string message = string.Format("Didn't select a vertex of the current walkbox gameobject '{0}'!", name);
            EditorUtility.DisplayDialog("Error", message, "OK");
        }
        else
        {
            DestroyImmediate(selectedObj);
            UpdateChildren();
        }
    }

    public void RemoveAllVertices()
    {
        string message = string.Format("Do you want to delete all the vertices on '{0}'?", name);
        bool willDeleteAllVertex = EditorUtility.DisplayDialog("Warning", message, "Yes", "No");

        if (willDeleteAllVertex)
        {
            int totalVertices = editor_wboxVertexGameobjects.Count;

            for (int i = 0; i < totalVertices; i++)
            {
                DestroyImmediate(editor_wboxVertexGameobjects[i]);
            }

            UpdateChildren();
        }
    }

    public void UpdateChildren()
    {
        if (editor_wboxVertexGameobjects == null)
            editor_wboxVertexGameobjects = new List<GameObject>();

        editor_wboxVertexGameobjects.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            editor_wboxVertexGameobjects.Add(transform.GetChild(i).gameObject);
        }
    }

    public string IsValidMesh_Message()
    {
        string message = "";

        if (editor_wboxVertexGameobjects.Count < 3)
        {
            message += "Not enough verticies to generate a mesh! ";
        }

        if (editor_wboxVertexGameobjects.Count % 3 != 0)
        {
            message += "Total amount of verticies are not a multiple of 3, no triangles can be generated! ";
        }

        return message;
    }

    private void OnDrawGizmos()
    {
        if (editor_wboxVertexGameobjects != null && editor_wboxVertexGameobjects.Count > 0)
        {
            foreach (GameObject vertex in editor_wboxVertexGameobjects)
            {
                Color gizmoColor = Color.yellow;

                foreach (GameObject secondVertex in editor_wboxVertexGameobjects)
                {
                    if(vertex != secondVertex)
                    {
                        if (Vector3.Distance(vertex.transform.position, secondVertex.transform.position) == 0)
                        {
                            gizmoColor = Color.green;
                        }
                    }
                }

                Gizmos.color = gizmoColor;

                if (showVertexPoint)
                {
                    Gizmos.DrawSphere(vertex.transform.position, vertexGizmoSize);
                }

                if (showVertexName)
                {
                    Handles.Label(vertex.transform.position, vertex.name);
                }
            }

            Gizmos.color = Color.white;
            Gizmos.DrawWireMesh(mesh);
        }
    }
}
