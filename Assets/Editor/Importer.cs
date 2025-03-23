using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class Importer : EditorWindow
{
    private string targetFolder = "";
    private string externalAssetsFolder = "";
    private string findString = "";
    private string replaceString = "";

    [MenuItem("Tools/Custom Importer")]
    public static void ShowWindow()
    {
        GetWindow<Importer>("Custom Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Custom Importer Tool", EditorStyles.boldLabel);

        // Hedef klasörü seçme
        if (GUILayout.Button("Select Target Folder"))
        {
            targetFolder = EditorUtility.OpenFolderPanel("Select Target Folder", Application.dataPath, "");
        }
        GUILayout.Label("Target Folder: " + targetFolder);

        // Harici varlık klasörünü seçme
        if (GUILayout.Button("Select External Assets Folder"))
        {
            externalAssetsFolder = EditorUtility.OpenFolderPanel("Select External Assets Folder", Application.dataPath, "");
        }
        GUILayout.Label("External Assets Folder: " + externalAssetsFolder);

        // Find & Replace giriş kutuları
        findString = EditorGUILayout.TextField("Find:", findString);
        replaceString = EditorGUILayout.TextField("Replace:", replaceString);

        // İçe Aktarma İşlemini Başlat
        if (GUILayout.Button("Start Import Process"))
        {
            if (!string.IsNullOrEmpty(targetFolder) && !string.IsNullOrEmpty(externalAssetsFolder))
            {
                ImportAssets();
            }
            else
            {
                Debug.LogWarning("Please select both folders before starting the import process.");
            }
        }
    }

    private void ImportAssets()
    {
        Debug.Log("Import process started...");

        if (!Directory.Exists(targetFolder) || !Directory.Exists(externalAssetsFolder))
        {
            Debug.LogError("Invalid folder paths.");
            return;
        }

        // 1️⃣ Hedef klasörü kopyala
        string duplicatedFolder = targetFolder + "_Duplicated";
        if (Directory.Exists(duplicatedFolder))
            Directory.Delete(duplicatedFolder, true);

        DirectoryCopy(targetFolder, duplicatedFolder);

        // 2️⃣ Dosya adlarını değiştirme işlemi
        RenameFilesInFolder(duplicatedFolder, findString, replaceString);

        // 3️⃣ Harici dosyaları içe aktarma
        MoveMatchingFiles(externalAssetsFolder, duplicatedFolder);

        Debug.Log("Import process completed!");
    }

    private void DirectoryCopy(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);

        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(destDir, fileName);
            File.Copy(file, destFile, true);
        }

        foreach (string subDir in Directory.GetDirectories(sourceDir))
        {
            string subDirName = Path.GetFileName(subDir);
            string newDestDir = Path.Combine(destDir, subDirName);
            DirectoryCopy(subDir, newDestDir);
        }
    }

    private void RenameFilesInFolder(string folderPath, string find, string replace)
    {
        if (string.IsNullOrEmpty(find)) return;

        foreach (string file in Directory.GetFiles(folderPath))
        {
            string fileName = Path.GetFileName(file);
            string newFileName = fileName.Replace(find, replace);
            string newPath = Path.Combine(folderPath, newFileName);
            File.Move(file, newPath);
        }
    }

    private void MoveMatchingFiles(string sourceFolder, string destFolder)
    {
        Dictionary<string, string> fileMappings = new Dictionary<string, string>();

        foreach (string file in Directory.GetFiles(destFolder))
        {
            string fileName = Path.GetFileName(file);
            fileMappings[fileName] = file;
        }

        foreach (string file in Directory.GetFiles(sourceFolder))
        {
            string fileName = Path.GetFileName(file);
            if (fileMappings.ContainsKey(fileName))
            {
                string destinationPath = fileMappings[fileName];
                File.Copy(file, destinationPath, true);
            }
        }
    }
}
