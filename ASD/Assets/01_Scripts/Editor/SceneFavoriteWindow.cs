using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

// ScriptableObject that stores scene path favorites
public class SceneFavorites : ScriptableObject
{
    public List<string> scenePaths = new List<string>();
}

// Editor window to manage favorites
public class SceneFavoritesWindow : EditorWindow
{
    private const string assetPath = "Assets/Editor/SceneFavorites.asset";
    private SceneFavorites favorites;
    private Vector2 scroll;
    private SceneAsset sceneToAdd;

    [MenuItem("Window/Scene Favorites")]
    public static void OpenWindow()
    {
        var w = GetWindow<SceneFavoritesWindow>("Scene Favorites");
        w.minSize = new Vector2(300, 200);
    }

    private void OnEnable()
    {
        LoadOrCreateFavorites();
    }

    void LoadOrCreateFavorites()
    {
        favorites = AssetDatabase.LoadAssetAtPath<SceneFavorites>(assetPath);
        if (favorites == null)
        {
            // Ensure folder exists
            var folder = System.IO.Path.GetDirectoryName(assetPath);
            if (!AssetDatabase.IsValidFolder(folder))
            {
                AssetDatabase.CreateFolder("Assets", "Editor");
            }

            favorites = CreateInstance<SceneFavorites>();
            AssetDatabase.CreateAsset(favorites, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(favorites);
        }
    }

    private void OnGUI()
    {
        if (favorites == null) LoadOrCreateFavorites();

        GUILayout.Space(6);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Current Scene", GUILayout.Height(28)))
        {
            AddCurrentScene();
        }
        if (GUILayout.Button("Add By Asset", GUILayout.Height(28)))
        {
            AddSceneAsset(sceneToAdd);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(6);
        EditorGUILayout.BeginHorizontal();
        sceneToAdd = (SceneAsset)EditorGUILayout.ObjectField(sceneToAdd, typeof(SceneAsset), false);
        if (GUILayout.Button("Clear", GUILayout.Width(50))) sceneToAdd = null;
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(8);

        EditorGUILayout.LabelField("Favorites:", EditorStyles.boldLabel);

        scroll = EditorGUILayout.BeginScrollView(scroll);
        if (favorites.scenePaths.Count == 0)
        {
            EditorGUILayout.HelpBox("No favorite scenes. Add one with 'Add Current Scene' or assign a Scene asset and click 'Add By Asset'.", MessageType.Info);
        }

        for (int i = 0; i < favorites.scenePaths.Count; i++)
        {
            EditorGUILayout.BeginHorizontal("box");

            string path = favorites.scenePaths[i];
            SceneAsset sa = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            EditorGUILayout.ObjectField(sa, typeof(SceneAsset), false);

            if (GUILayout.Button("Open", GUILayout.Width(48)))
            {
                if (PromptSaveIfNeeded())
                {
                    EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                }
            }

            if (GUILayout.Button("Up", GUILayout.Width(36)))
            {
                if (i > 0) { Swap(i, i - 1); }
            }
            if (GUILayout.Button("Down", GUILayout.Width(48)))
            {
                if (i < favorites.scenePaths.Count - 1) { Swap(i, i + 1); }
            }

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                favorites.scenePaths.RemoveAt(i);
                EditorUtility.SetDirty(favorites);
                AssetDatabase.SaveAssets();
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear All"))
        {
            if (EditorUtility.DisplayDialog("Clear All Favorites?", "Remove all saved favorite scenes?", "Yes", "No"))
            {
                favorites.scenePaths.Clear();
                EditorUtility.SetDirty(favorites);
                AssetDatabase.SaveAssets();
            }
        }
        if (GUILayout.Button("Refresh"))
        {
            EditorUtility.SetDirty(favorites);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        EditorGUILayout.EndHorizontal();
    }

    void Swap(int a, int b)
    {
        var tmp = favorites.scenePaths[a];
        favorites.scenePaths[a] = favorites.scenePaths[b];
        favorites.scenePaths[b] = tmp;
        EditorUtility.SetDirty(favorites);
        AssetDatabase.SaveAssets();
    }

    void AddCurrentScene()
    {
        string path = SceneManager.GetActiveScene().path;
        if (string.IsNullOrEmpty(path))
        {
            EditorUtility.DisplayDialog("Can't Add Scene", "Current scene has not been saved. Save the scene before adding.", "OK");
            return;
        }

        if (!favorites.scenePaths.Contains(path))
        {
            favorites.scenePaths.Add(path);
            EditorUtility.SetDirty(favorites);
            AssetDatabase.SaveAssets();
        }
        else
        {
            EditorUtility.DisplayDialog("Already Added", "This scene is already in favorites.", "OK");
        }
    }

    void AddSceneAsset(SceneAsset sa)
    {
        if (sa == null)
        {
            EditorUtility.DisplayDialog("No Scene Selected", "Assign a Scene asset in the field first.", "OK");
            return;
        }

        string path = AssetDatabase.GetAssetPath(sa);
        if (!favorites.scenePaths.Contains(path))
        {
            favorites.scenePaths.Add(path);
            sceneToAdd = null;
            EditorUtility.SetDirty(favorites);
            AssetDatabase.SaveAssets();
        }
        else
        {
            EditorUtility.DisplayDialog("Already Added", "This scene is already in favorites.", "OK");
        }
    }

    bool PromptSaveIfNeeded()
    {
        return EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
    }
}
