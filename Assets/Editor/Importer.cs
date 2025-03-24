using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Importer : EditorWindow
{
    private string targetFolderPath = "";
    private string externalFolderPath = "";
    private string findString = "";
    private string replaceString = "";
    private List<string> externalAssetFileNames = new List<string>();
    private Vector2 scrollPosition;

    [MenuItem("Tools/Importer")]
    public static void ShowWindow()
    {
        GetWindow<Importer>("Importer");
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawTargetFolderSelection();
        DrawExternalAssetsSection(); // Combined external folder selection and file list
        DrawFindReplaceSection();
        DrawProcessButton();

        EditorGUILayout.EndScrollView();
    }

    private void DrawTargetFolderSelection()
    {
        GUILayout.Space(10);
        GUILayout.Label("Target Folder", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Target Folder Path", GUILayout.Width(120));
        targetFolderPath = EditorGUILayout.TextField(targetFolderPath);
        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            targetFolderPath = EditorUtility.OpenFolderPanel("Select Target Folder", "Assets", "");
        }
        GUILayout.EndHorizontal();
    }

    private void DrawExternalAssetsSection()
    {
        GUILayout.Space(15);
        GUILayout.Label("External Assets", EditorStyles.boldLabel);

        // External Assets Path
        GUILayout.BeginHorizontal();
        GUILayout.Label("External Assets Path", GUILayout.Width(120));
        externalFolderPath = EditorGUILayout.TextField(externalFolderPath);
        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            externalFolderPath = EditorUtility.OpenFolderPanel("Select External Folder", "", "");
            UpdateExternalAssetsList(); // Update list immediately after Browse
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        // External Files List
        GUILayout.Label("External Files", EditorStyles.boldLabel);
        if (externalAssetFileNames.Count > 0)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox); // Use a help box for visual grouping
            foreach (string fileName in externalAssetFileNames)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(fileName);
                // You could add a button here to remove individual files if needed
                // if (GUILayout.Button("x", GUILayout.Width(20)))
                // {
                //     // Implement removal logic
                // }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.LabelField("No external assets selected.");
        }

        GUILayout.Space(10);
        GUI.enabled = IsReadyForExternalFiles();
        if (GUILayout.Button("Pick Asset Folder", GUILayout.Height(30))) // Adjust button height
        {
            UpdateExternalAssetsList();
        }
        GUI.enabled = true;
    }

    private void DrawFindReplaceSection()
    {
        GUILayout.Space(15);
        GUILayout.Label("Naming", EditorStyles.boldLabel);
        findString = EditorGUILayout.TextField("Find", findString); // Removed extra space in label
        replaceString = EditorGUILayout.TextField("Replace", replaceString); // Removed extra space in label
    }

    

    private void DrawProcessButton()
    {
        GUILayout.Space(20);
        GUI.enabled = IsReadyForProcessing();
        if (GUILayout.Button("Process Assets", GUILayout.Height(40)))
        {
            ExecuteProcessing();
        }
        GUI.enabled = true;
    }

    private bool IsReadyForExternalFiles()
    {
        return Directory.Exists(targetFolderPath) &&
               Directory.Exists(externalFolderPath);
    }

    private bool IsReadyForProcessing()
    {
        return Directory.Exists(targetFolderPath) &&
               Directory.Exists(externalFolderPath) &&
               !string.IsNullOrWhiteSpace(findString);
    }

    private void UpdateExternalAssetsList()
    {
        externalAssetFileNames.Clear();
        if (Directory.Exists(externalFolderPath))
        {
            var files = Directory.GetFiles(externalFolderPath, "*.*", SearchOption.AllDirectories)
                .Where(file => !file.EndsWith(".meta"))
                .Select(Path.GetFileName);

            externalAssetFileNames.AddRange(files);
        }
    }

    private void ExecuteProcessing()
    {
        try
        {
            // 1. Create working copy
            string tempFolder = CreateTempFolder();

            // 2. Rename operations
            ProcessRenaming(tempFolder);

            // 3. Smart file replacement
            ReplaceMatchingAssets(tempFolder);

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", "Processing completed!", "OK");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Processing failed: {e}");
            EditorUtility.DisplayDialog("Error",
                $"Operation failed: {e.Message}",
                "OK");
        }
    }

    private string CreateTempFolder()
    {
        string tempPath = $"{targetFolderPath}_PROCESSED_{System.DateTime.Now:yyyyMMdd_HHmmss}";

        if (Directory.Exists(tempPath))
        {
            Directory.Delete(tempPath, true);
        }

        FileUtil.CopyFileOrDirectory(targetFolderPath, tempPath);
        AssetDatabase.Refresh();
        return tempPath;
    }

    private void ProcessRenaming(string folderPath)
    {
        DirectoryInfo dir = new DirectoryInfo(folderPath);
        var allFiles = dir.GetFiles("*.*", SearchOption.AllDirectories)
            .Where(f => !f.Name.EndsWith(".meta"));

        foreach (FileInfo file in allFiles)
        {
            string newName = Regex.Replace(file.Name, findString, replaceString);
            if (newName == file.Name) continue;

            string newPath = Path.Combine(file.DirectoryName, newName);
            File.Move(file.FullName, newPath);

            // Handle meta files
            string metaFile = file.FullName + ".meta";
            if (File.Exists(metaFile))
            {
                File.Move(metaFile, newPath + ".meta");
            }
        }
    }

    private void ReplaceMatchingAssets(string targetFolder)
    {
        var targetFiles = Directory.GetFiles(targetFolder, "*.*", SearchOption.AllDirectories)
            .Select(Path.GetFileName)
            .ToHashSet();

        var externalFiles = Directory.GetFiles(externalFolderPath, "*.*", SearchOption.AllDirectories)
            .Where(f => !f.EndsWith(".meta"));

        foreach (string externalFile in externalFiles)
        {
            string fileName = Path.GetFileName(externalFile);
            if (targetFiles.Contains(fileName))
            {
                string targetPath = Directory.GetFiles(targetFolder, fileName, SearchOption.AllDirectories)
                    .FirstOrDefault();

                if (!string.IsNullOrEmpty(targetPath))
                {
                    File.Copy(externalFile, targetPath, true);
                    Debug.Log($"Replaced: {targetPath}");
                }
            }
        }
    }
}