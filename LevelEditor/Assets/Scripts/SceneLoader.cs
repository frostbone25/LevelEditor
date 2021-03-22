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
    private static string editor_scenePath;
    private static string editor_gameID;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Telltale/Import Scene")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        GetWindow(typeof(SceneLoader));
    }

    static SceneLoader()
    {
        string currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
        string dllPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar + "Plugins";
        if (currentPath.Contains(dllPath) == false)
        {
            Environment.SetEnvironmentVariable("PATH", currentPath + Path.PathSeparator + dllPath, EnvironmentVariableTarget.Process);
        }
    }

    void OnGUI()
    {
        GUILayout.Label("Telltale Scene Importer", EditorStyles.largeLabel);

        editor_gameID = EditorGUILayout.TextField("Game ID", editor_gameID);

        if (GUILayout.Button("Import Scene"))
        {
            editor_scenePath = EditorUtility.OpenFilePanel("Open a .scene file", "", "scene");

            if(File.Exists(editor_scenePath))
            {
                ImportScene(editor_gameID, editor_scenePath);
                EditorUtility.DisplayDialog("Completed", "Finished Importing", "OK");
            }    
        }
    }

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


        foreach (Scene.AgentInfo agent in sceneObj.GetAgents())
        {
            List<PropertySet.PropertyEntry> entry = agent.mAgentSceneProps.GetPropertiesOfType(KeyTypes.TYPE_LOCATION_INFO);

            GameObject unity_gameobject = new GameObject(agent.mAgentName);

            if (entry.Count >= 1)
            {
                LibTelltale.Vector3 position = PropertySet.GetProperty(KeyTypes.TYPE_LOCATION_INFO, entry[0]).mPosition;
                LibTelltale.Vector3 rotation = PropertySet.GetProperty(KeyTypes.TYPE_LOCATION_INFO, entry[0]).mRotation;

                unity_gameobject.transform.position = new UnityEngine.Vector3(position.x, position.y, position.z);
                unity_gameobject.transform.eulerAngles = new UnityEngine.Vector3(rotation.x, rotation.y, rotation.z);

            }
        }
    }
}
