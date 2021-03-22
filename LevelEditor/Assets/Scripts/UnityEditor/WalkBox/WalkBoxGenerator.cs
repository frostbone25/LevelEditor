using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using LibTelltale;

public class WalkBoxGenerator : EditorWindow
{
    //names
    private static WalkBoxMesh editor_wboxObject;

    //TODO : LATER USE WITH LIB TELLTALE
    private static string editor_wboxPath; //editor wbox file path
    private static string editor_gameID; //editor game ID text

    //add a menu item at the top of the unity editor toolbar
    [MenuItem("Telltale/Walk Box Editor")]
    public static void ShowWindow()
    {
        //get the window and open it
        GetWindow(typeof(WalkBoxGenerator));
    }

    /// <summary>
    /// Checks and sets the enviorment path variables for the DLLs in the project.
    /// </summary>
    static WalkBoxGenerator()
    {
        string currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
        string dllPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar + "Plugins";

        if (currentPath.Contains(dllPath) == false)
        {
            Environment.SetEnvironmentVariable("PATH", currentPath + Path.PathSeparator + dllPath, EnvironmentVariableTarget.Process);
        }
    }

    private static void ImportWalkbox()
    {
        //not implemented yet
        EditorUtility.DisplayDialog("Error", "Not Implemented Yet!", "OK");

        /*
        //opens up a file browser panel
        editor_wboxPath = EditorUtility.OpenFilePanel("Open a .wbox file", "", "wbox");

        //tell LibTelltale what the game version will be for the file to be opened
        TTContext context = new TTContext(editor_gameID);

        //creates a bytestream of the given file
        ByteStream byteStream = new ByteStream(editor_wboxPath);

        //sets the stream on the context object to the new bytestream of the file we want to open
        context.NextStream(byteStream, false);

        //to be continued...
        */
    }

    /// <summary>
    /// GUI display function for the window
    /// </summary>
    void OnGUI()
    {
        //window title
        GUILayout.Label("WalkBox", EditorStyles.whiteLargeLabel);

        //text field for the game ID
        editor_gameID = EditorGUILayout.TextField("Import Game ID", editor_gameID);

        if (GUILayout.Button("Import Walkbox"))
        {
            if (string.IsNullOrEmpty(editor_gameID))
            {
                EditorUtility.DisplayDialog("Error", "You need to set a Game ID for LibTelltale to use!", "OK");
                return;
            }

            ImportWalkbox();
        }

        //button for clearing and empty-ing the existing scene
        if (GUILayout.Button("Create New Walkbox"))
        {
            if (editor_wboxObject != null)
            {
                bool willMakeNewOne = EditorUtility.DisplayDialog("Error", "Not Implemented Yet!", "Yes", "No");

                if (!willMakeNewOne)
                {
                    return;
                }

                DestroyImmediate(editor_wboxObject.gameObject);
            }

            GameObject editor_wboxGameobject = new GameObject("New Walkbox");
            editor_wboxObject = editor_wboxGameobject.AddComponent<WalkBoxMesh>();
        }

        //button for clearing and empty-ing the existing scene
        if (Selection.activeGameObject)
        {
            if (Selection.activeGameObject.GetComponent<WalkBoxMesh>())
            {
                if (GUILayout.Button("Select Walkbox"))
                {
                    editor_wboxObject = Selection.activeGameObject.GetComponent<WalkBoxMesh>();
                }
            }
        }

        if (editor_wboxObject != null)
        {
            //section title
            GUILayout.Label("WalkBox Editing", EditorStyles.whiteLargeLabel);

            editor_wboxObject.gameObject.name = EditorGUILayout.TextField("Name", editor_wboxObject.gameObject.name);

            GUILayout.Label("Mesh Data", EditorStyles.boldLabel);
            GUILayout.Label(string.Format("Vertex Count: {0}", editor_wboxObject.editor_wboxVertexGameobjects.Count), EditorStyles.label);
            GUILayout.Label(string.Format("Triangle Count: {0}", editor_wboxObject.editor_wboxVertexGameobjects.Count/3), EditorStyles.label);

            string errorMessage = editor_wboxObject.IsValidMesh_Message();

            if(string.IsNullOrEmpty(errorMessage) == false)
            {
                EditorGUILayout.LabelField(errorMessage, EditorStyles.helpBox);
            }

            GUILayout.Label("Mesh Normals", EditorStyles.boldLabel);
            //editor_wboxObject.forceNormalDirectionCalculation = EditorGUILayout.Toggle("Force Normal Direction", editor_wboxObject.forceNormalDirectionCalculation);

            //if(editor_wboxObject.forceNormalDirectionCalculation)
            //{
            //    editor_wboxObject.forceNormalDirection = EditorGUILayout.Vector3Field("Normal Vector", editor_wboxObject.forceNormalDirection);
            //}

            editor_wboxObject.normalAngle = EditorGUILayout.Slider("Normal Angle", editor_wboxObject.normalAngle, 0, 180);

            if (GUILayout.Button("Recalculate Normals"))
            {
                editor_wboxObject.RecalcNormals();
            }

            GUILayout.Label("Editor Options", EditorStyles.boldLabel);
            editor_wboxObject.updateMeshEveryFrame = EditorGUILayout.Toggle("Generate Mesh Every Frame", editor_wboxObject.updateMeshEveryFrame);
            editor_wboxObject.showMeshPreview = EditorGUILayout.Toggle("Show Mesh Preview", editor_wboxObject.showMeshPreview);
            editor_wboxObject.showVertexName = EditorGUILayout.Toggle("Show Vertex Names", editor_wboxObject.showVertexName);
            editor_wboxObject.showVertexPoint = EditorGUILayout.Toggle("Show Vertex Points", editor_wboxObject.showVertexPoint);
            editor_wboxObject.vertexGizmoSize = EditorGUILayout.DelayedFloatField("Vertex Gizmo Size", editor_wboxObject.vertexGizmoSize);

            //section title
            GUILayout.Label("Commands", EditorStyles.boldLabel);

            if (!editor_wboxObject.updateMeshEveryFrame)
            {
                if (GUILayout.Button("Generate Mesh"))
                {
                    editor_wboxObject.GenerateMesh();
                }
            }

            if (GUILayout.Button("Add Vertex"))
            {
                editor_wboxObject.AddVertexGameObject();
            }

            if (GUILayout.Button("Remove Selected Vertex"))
            {
                editor_wboxObject.RemoveSelectedVertex();
            }

            if (GUILayout.Button("Remove All Vertices"))
            {
                editor_wboxObject.RemoveAllVertices();
            }

            if (GUILayout.Button("Delete Existing Walkbox"))
            {
                string message = string.Format("Delete the current walkbox '{0}'?", editor_wboxObject.name);
                bool willRemoveExistingWalkbox = EditorUtility.DisplayDialog("Warning", message, "Yes", "No");

                if (willRemoveExistingWalkbox)
                {
                    DestroyImmediate(editor_wboxObject.gameObject);
                    return;
                }
            }
        }    
    }
}
