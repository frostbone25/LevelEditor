using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LibTelltale;
using System.IO;
using LibTelltaleWrapper.MetaStreamed;
using System;

public class SceneLoader : EditorWindow
{
    //editor scene file path
    private static string editor_scenePath;

    //editor game ID text
    private static string editor_gameID;

    //add a menu item at the top of the unity editor toolbar
    [MenuItem("Telltale/Import Scene")]
    public static void ShowWindow()
    {
        //get the window and open it
        GetWindow(typeof(SceneLoader));
    }

    /// <summary>
    /// Checks and sets the enviorment path variables for the DLLs in the project.
    /// </summary>
    static SceneLoader()
    {
        string currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
        string dllPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar + "Plugins";

        if (currentPath.Contains(dllPath) == false)
        {
            Environment.SetEnvironmentVariable("PATH", currentPath + Path.PathSeparator + dllPath, EnvironmentVariableTarget.Process);
        }
    }

    /// <summary>
    /// GUI display function for the window
    /// </summary>
    void OnGUI()
    {
        //window title
        GUILayout.Label("Telltale Scene Importer", EditorStyles.largeLabel);

        //text field for the game ID
        editor_gameID = EditorGUILayout.TextField("Game ID", editor_gameID);

        //button for clearing and empty-ing the existing scene
        if (GUILayout.Button("Clear Scene"))
        {
            //call the main function for clearing a scene
            ClearScene();

            //display a dialog that we finished clearing
            EditorUtility.DisplayDialog("Completed", "Cleared Scene", "OK");
        }

        //button for importing the .scene file
        if (GUILayout.Button("Import Scene"))
        {
            //opens up a file browser panel
            editor_scenePath = EditorUtility.OpenFilePanel("Open a .scene file", "", "scene");

            //if the file exists, continue
            if(File.Exists(editor_scenePath))
            {
                //call the main import scene
                ImportScene(editor_gameID, editor_scenePath);

                //once import scene is finished, display a dialog that importing has completed
                EditorUtility.DisplayDialog("Completed", "Finished Importing", "OK");
            }    
        }
    }

    /// <summary>
    /// Clears all gameobjects within the current scene.
    /// </summary>
    private static void ClearScene()
    {
        //get all gameobjects in the scene that we can find
        GameObject[] list = FindObjectsOfType<GameObject>(true);

        //run a loop to delete them all
        for(int i = 0; i < list.Length; i++)
        {
            DestroyImmediate(list[i].gameObject);
        }
    }

    /// <summary>
    /// Main function for importing a scene
    /// </summary>
    /// <param name="gameID"></param>
    /// <param name="path"></param>
    private static void ImportScene(string gameID, string path)
    {
        //tell LibTelltale what the game version will be for the file to be opened
        TTContext context = new TTContext(gameID);

        //creates a bytestream of the given file
        ByteStream byteStream = new ByteStream(path);

        //sets the stream on the context object to the new bytestream of the file we want to open
        context.NextStream(byteStream, false);

        //create our input mapper object
        Scene sceneObj = new Scene(context);

        //open the file (should return 0 if sucessful)
        if (sceneObj.Open() != 0)
            return;

        //run a loop for all of the agents in the scene file
        foreach (Scene.AgentInfo agent in sceneObj.GetAgents())
        {
            //get the current property entry that we are on
            List<PropertySet.PropertyEntry> entry = agent.mAgentSceneProps.GetPropertiesOfType(KeyTypes.TYPE_LOCATION_INFO);

            //create a gameobject that shares the name of the agent
            GameObject unity_gameobject = new GameObject(agent.mAgentName);

            if (entry.Count >= 1)
            {
                //get the position and rotation vectors from the property
                LibTelltale.Vector3 position = PropertySet.GetProperty(KeyTypes.TYPE_LOCATION_INFO, entry[0]).mPosition;
                LibTelltale.Vector3 rotation = PropertySet.GetProperty(KeyTypes.TYPE_LOCATION_INFO, entry[0]).mRotation;

                float test = 180;

                //assign those positions to our unity gameobject
                unity_gameobject.transform.position = new UnityEngine.Vector3(position.x, position.y, position.z);
                unity_gameobject.transform.rotation = UnityEngine.Quaternion.Euler(rotation.x * test, rotation.y * test, rotation.z * test);

                Debug.Log(unity_gameobject.name);
                Debug.Log(unity_gameobject.transform.rotation);

                if(unity_gameobject.name.Contains("light_"))
                {
                    Light unity_light = unity_gameobject.AddComponent<Light>();
                }

            }
        }
    }
}
