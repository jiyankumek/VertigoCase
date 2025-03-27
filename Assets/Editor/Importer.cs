using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Importer : EditorWindow
{
    // Hedef klasör, harici klasör ve ad değiştirme için gerekli değişkenler
    private string targetFolderPath = "";
    private string externalFolderPath = "";
    private string findString = "";
    private string replaceString = "";

    // Harici klasörden okunan dosya isimleri (UI'de gösterilecek ve "X" ile kaldırılabilecek)
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
        // "Import" butonu: İşlem başlatılır
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

    // Hedef klasörün seçilmesi için katlanabilir bölüm
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

    // Harici klasör ve dosya listesini seçmek için bölüm
    private bool showExternalFiles = true;
    private void DrawExternalAssetsSection()
    {
        GUILayout.Space(15);
        GUILayout.Label("External Assets", EditorStyles.boldLabel);

        // Harici klasör yolunu al
        GUILayout.BeginHorizontal();
        GUILayout.Label("External Assets Path", GUILayout.Width(120));
        externalFolderPath = EditorGUILayout.TextField(externalFolderPath);
        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            externalFolderPath = EditorUtility.OpenFolderPanel("Select External Folder", "", "");
            // Klasör seçildiğinde dosyaları oku
            UpdateExternalAssetsList();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        // Dosya listesi katlanabilir bölümde gösteriliyor
        showExternalFiles = EditorGUILayout.Foldout(showExternalFiles, "External Files");

        if (showExternalFiles)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            // Listeden her dosyayı ve yanındaki "X" butonunu göster
            for (int i = 0; i < externalAssetFileNames.Count; i++)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(externalAssetFileNames[i]);
                // "X" butonuna basılırsa dosya listeden kaldırılır
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    externalAssetFileNames.RemoveAt(i);
                    break; // Döngüden çık, layout bozulmasın
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        // İsteğe bağlı: Harici dosyaları tekrar yüklemek için buton
        GUILayout.Space(10);
        GUI.enabled = IsReadyForExternalFiles();
        if (GUILayout.Button("Pick Assets Folder", GUILayout.Height(30)))
        {
            UpdateExternalAssetsList();
        }
        GUI.enabled = true;
    }

    // Ad değiştirme (Find/Replace) için katlanabilir bölüm
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

    // İşleme başlamadan önce gerekli klasörlerin ve adlandırma değerlerinin dolu olduğundan emin olun
    private bool IsReadyForProcessing()
    {
        return Directory.Exists(targetFolderPath) &&
               Directory.Exists(externalFolderPath) &&
               !string.IsNullOrWhiteSpace(findString);
    }

    // Harici klasörün varlığını kontrol et
    private bool IsReadyForExternalFiles()
    {
        return Directory.Exists(targetFolderPath) && Directory.Exists(externalFolderPath);
    }

    // Harici klasörden dosya isimlerini alır (meta dosyaları hariç)
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

    // İşlemleri sırasıyla çalıştırır: çalışma klasörü oluşturma, yeniden adlandırma ve dosya kopyalama
    private void ExecuteProcessing()
    {
        try
        {
            // 1. Hedef klasörün çalışma kopyasını oluştur
            string tempFolder = CreateTempFolder();

            // 2. Dosyaların adını yeniden düzenle
            ProcessRenaming(tempFolder);

            // 3. Harici dosyaları kopyala (yalnızca UI'de kalan dosyalar)
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

    // Hedef klasörün çalışma kopyasını oluşturur.
    // Temp klasör ismi, externalFolderPath'in en sağdaki klasör adı ile belirlenir.
    // Eğer externalFolderPath geçerli değilse, timestamp kullanılır.
    private string CreateTempFolder()
    {
        string folderName = "";
        if (!string.IsNullOrEmpty(externalFolderPath))
        {
            // externalFolderPath'in sonunda varsa '/' veya '\' karakterlerini temizle
            string trimmedPath = externalFolderPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            folderName = Path.GetFileName(trimmedPath);
        }
        else
        {
            folderName = $"PROCESSED_{System.DateTime.Now:yyyyMMdd_HHmmss}";
        }

        // Temp klasör, targetFolderPath'in bulunduğu dizinde oluşturulsun
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

    // Çalışma kopyasındaki dosya adlarını find/replace değerlerine göre değiştirir.
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

            // İlgili meta dosyasını da yeniden adlandır.
            string metaFile = file.FullName + ".meta";
            if (File.Exists(metaFile))
            {
                File.Move(metaFile, newPath + ".meta");
            }
        }
    }

    // UI'de kalan dosya isimlerini kullanarak, harici klasörden hedef çalışma klasörüne kopyalar.
    private void ReplaceMatchingAssets(string targetFolder)
    {
        // Hedef klasördeki dosya adlarını al.
        var targetFiles = Directory.GetFiles(targetFolder, "*.*", SearchOption.AllDirectories)
                                   .Select(Path.GetFileName)
                                   .ToHashSet();

        // UI'de kalan her dosya için.
        foreach (var fileName in externalAssetFileNames)
        {
            // Harici klasördeki tam dosya yolunu bul.
            string externalFilePath = FindFileInDirectory(externalFolderPath, fileName);
            if (!string.IsNullOrEmpty(externalFilePath) && targetFiles.Contains(fileName))
            {
                // Hedef klasörde aynı isimdeki dosyanın yolunu bul.
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

    // Belirtilen dosya adını, kök klasör ve alt klasörlerde arar.
    private string FindFileInDirectory(string root, string fileName)
    {
        var files = Directory.GetFiles(root, fileName, SearchOption.AllDirectories);
        return files.FirstOrDefault();
    }
}
