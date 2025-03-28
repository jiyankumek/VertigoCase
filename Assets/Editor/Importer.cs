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

    //External okunan dosyalar
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

        DrawTargetFolderSection();
        DrawExternalAssetsSection();
        DrawNamingSection();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        //Import butonu
        GUI.enabled = IsReadyForProcessing();
        if (GUILayout.Button("Import", GUILayout.Height(30), GUILayout.Width(250)))
        {
            ExecuteProcessing();
        }
        GUI.enabled = true;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();
    }

    //katlanma olayi
    private bool showTargetFolder = true;
    private void DrawTargetFolderSection()
    {
        GUILayout.Space(10);
        showTargetFolder = EditorGUILayout.Foldout(showTargetFolder, "Target Folder");

        if (showTargetFolder)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Target Folder Path", GUILayout.Width(120));
            targetFolderPath = EditorGUILayout.TextField(targetFolderPath);
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                targetFolderPath = EditorUtility.OpenFolderPanel("Select Target Folder", "Assets", "");
            }
            GUILayout.EndHorizontal();
        }
    }

    // external listesi ac kapa
    private bool showExternalFiles = true;
    private void DrawExternalAssetsSection()
    {
        GUILayout.Space(15);
        GUILayout.Label("External Assets", EditorStyles.boldLabel);

        // External yolu yaz
        GUILayout.BeginHorizontal();
        GUILayout.Label("External Assets Path", GUILayout.Width(120));
        externalFolderPath = EditorGUILayout.TextField(externalFolderPath);
        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            externalFolderPath = EditorUtility.OpenFolderPanel("Select External Folder", "", "");
            // External dosyalarini cek
            UpdateExternalAssetsList();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        // External listesi ac/kapa
        showExternalFiles = EditorGUILayout.Foldout(showExternalFiles, "External Files");

        if (showExternalFiles)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            
            int indexToRemove = -1;

            for (int i = 0; i < externalAssetFileNames.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(externalAssetFileNames[i], GUILayout.ExpandWidth(true));

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    indexToRemove = i;
                }

                EditorGUILayout.EndHorizontal();
            }

            
            if (indexToRemove != -1)
            {
                externalAssetFileNames.RemoveAt(indexToRemove);
            }

            EditorGUILayout.EndVertical();
        }

        // Harici dosyalari tekrar yüklemek icin buton (reset)
        GUILayout.Space(10);
        GUI.enabled = IsReadyForExternalFiles();
        if (GUILayout.Button("Pick Assets Folder", GUILayout.Height(30)))
        {
            UpdateExternalAssetsList();
        }
        GUI.enabled = true;
    }

    //Find/Replace icin katlanabilir kisim
    private bool showNaming = true;
    private void DrawNamingSection()
    {
        GUILayout.Space(15);
        showNaming = EditorGUILayout.Foldout(showNaming, "Naming");

        if (showNaming)
        {
            findString = EditorGUILayout.TextField("Find", findString);
            replaceString = EditorGUILayout.TextField("Replace", replaceString);
        }
    }

    // İslem öncesinde folderlarin ve find/replace kisimlarinin dolu olup olmadigina bak
    private bool IsReadyForProcessing()
    {
        return Directory.Exists(targetFolderPath) &&
               Directory.Exists(externalFolderPath) &&
               !string.IsNullOrWhiteSpace(findString);
    }

    // external files var mi
    private bool IsReadyForExternalFiles()
    {
        return Directory.Exists(targetFolderPath) && Directory.Exists(externalFolderPath);
    }

    // external filesdeki dosya listesini olustur (.metalari alma direkt unity icinde yapmak istersek sorun cikmasin )
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

    // duplicate ve rename kisimlari yapılıyo
    private void ExecuteProcessing()
    {
        try
        {
            // kopyasini al
            string tempFolder = CreateTempFolder();

            // adini degis
            ProcessRenaming(tempFolder);

            // external fileslari kopyala 
            ReplaceMatchingAssets(tempFolder);

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", "Processing completed!", "OK");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Processing failed: {e}");
            EditorUtility.DisplayDialog("Error", $"Operation failed: {e.Message}", "OK");
        }
    }

    // target'in kopyasini al
    private string CreateTempFolder()
    {
        string folderName = "";
        if (!string.IsNullOrEmpty(externalFolderPath))
        {
            // externalFolderPath'in sonundaki klasor ismini al
            string trimmedPath = externalFolderPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            folderName = Path.GetFileName(trimmedPath);
        }
        else
        {
            folderName = $"PROCESSED_{System.DateTime.Now:yyyyMMdd_HHmmss}";
        }

        // Temp folder, targetFolderPath'in oldugu yere yapistirilsin
        string parentDir = Path.GetDirectoryName(targetFolderPath);
        string tempPath = Path.Combine(parentDir, folderName);

        if (Directory.Exists(tempPath))
        {
            Directory.Delete(tempPath, true);
        }
        FileUtil.CopyFileOrDirectory(targetFolderPath, tempPath);
        AssetDatabase.Refresh();
        return tempPath;
    }

    //externalfilesdaki dosyalari find/replace ile isimlerini degistir
    private void ProcessRenaming(string folderPath)
    {
        DirectoryInfo dir = new DirectoryInfo(folderPath);
        var allFiles = dir.GetFiles("*.*", SearchOption.AllDirectories)
                          .Where(f => !f.Name.EndsWith(".meta"));

        foreach (FileInfo file in allFiles)
        {
            string newName = Regex.Replace(file.Name, findString, replaceString);
            if (newName == file.Name)
                continue;

            string newPath = Path.Combine(file.DirectoryName, newName);
            File.Move(file.FullName, newPath);

            // meta dosyasinin ismini degistir
            string metaFile = file.FullName + ".meta";
            if (File.Exists(metaFile))
            {
                File.Move(metaFile, newPath + ".meta");
            }
        }
    }

    
    private void ReplaceMatchingAssets(string targetFolder)
    {
        // target folderdaki dosya adlarini al
        var targetFiles = Directory.GetFiles(targetFolder, "*.*", SearchOption.AllDirectories)
                                   .Select(Path.GetFileName)
                                   .ToHashSet();

        
        foreach (var fileName in externalAssetFileNames)
        {
            // externalfilesdaki dosya yolundan ismi cek
            string externalFilePath = FindFileInDirectory(externalFolderPath, fileName);
            if (!string.IsNullOrEmpty(externalFilePath) && targetFiles.Contains(fileName))
            {
                // targetfolderda ayni isimdeki dosyanin yolunu bul
                string targetPath = Directory.GetFiles(targetFolder, fileName, SearchOption.AllDirectories)
                                             .FirstOrDefault();
                if (!string.IsNullOrEmpty(targetPath))
                {
                    File.Copy(externalFilePath, targetPath, true);
                    Debug.Log($"Replaced: {targetPath}");
                }
            }
        }
    }

    // Belirtilen dosya adini klasor icerisinde arar
    private string FindFileInDirectory(string root, string fileName)
    {
        var files = Directory.GetFiles(root, fileName, SearchOption.AllDirectories);
        return files.FirstOrDefault();
    }
}
