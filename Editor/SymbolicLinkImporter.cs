using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

/// <summary>
/// Editor window to create symbolic links in Windows.
/// </summary>
public class SymbolicLinkImporter : EditorWindow
{
    [Tooltip("The path to the folder to be linked (source).")]
    [SerializeField] private static string sourcePath = "";

    [Tooltip("The base folder where the symbolic link will be created.")]
    [SerializeField] private string baseTargetPath = "";

    [MenuItem("Assets/Import from Symbolic Link", false, 20)]
    private static void OpenWindow()
    {
        SymbolicLinkImporter window = GetWindow<SymbolicLinkImporter>("Symbolic Link Importer");

        if (Selection.activeObject != null)
        {
            string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (Directory.Exists(selectedPath))
            {
                string fullPath = Path.GetFullPath(selectedPath);
                window.baseTargetPath = fullPath;
            }
        }

        string selected = EditorUtility.OpenFolderPanel("Select Source Folder", "", "");
        if (!string.IsNullOrEmpty(selected))
            sourcePath = selected;
    }

    private void OnGUI()
    {
        GUILayout.Label("Symbolic Link Importer", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        sourcePath = EditorGUILayout.TextField("Source Folder", sourcePath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string selected = EditorUtility.OpenFolderPanel("Select Source Folder", "", "");
            if (!string.IsNullOrEmpty(selected))
                sourcePath = selected;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        baseTargetPath = EditorGUILayout.TextField("Target Base Folder", baseTargetPath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string selected = EditorUtility.OpenFolderPanel("Select Target Base Folder", "", "");
            if (!string.IsNullOrEmpty(selected))
                baseTargetPath = selected;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (GUILayout.Button("Create Symbolic Link"))
        {
            TryCreateSymbolicLink(sourcePath, baseTargetPath);
        }
    }

    public void ShowErrorMessage(string message)
    {
        EditorUtility.DisplayDialog("Error!", message, "Ok");
        Debug.LogError(message);
    }

    /// <summary>
    /// Tries to create a symbolic link and handles admin errors and name conflicts.
    /// </summary>
    /// <param name="source">The original folder path.</param>
    /// <param name="targetBase">The base target folder where the link will be created.</param>
    private void TryCreateSymbolicLink(string source, string targetBase)
    {
        if (!Directory.Exists(source))
        {
            ShowErrorMessage("Source folder does not exist.");
            return;
        }

        if (!Directory.Exists(targetBase))
        {
            ShowErrorMessage("Target base folder does not exist.");
            return;
        }

        string sourceName = Path.GetFileName(source.TrimEnd(Path.DirectorySeparatorChar));
        string finalTarget = Path.Combine(targetBase, sourceName);

        int suffix = 1;
        while (Directory.Exists(finalTarget) || File.Exists(finalTarget))
        {
            bool overwrite = EditorUtility.DisplayDialog(
                "Target Already Exists",
                $"A folder named '{Path.GetFileName(finalTarget)}' already exists.\n\nDo you want to create a new one with a numeric suffix?",
                "Yes", "Cancel");

            if (!overwrite)
            {
                ShowErrorMessage("User cancelled symbolic link creation.");
                return;
            }

            finalTarget = Path.Combine(targetBase, sourceName + "_" + suffix);
            suffix++;
        }

        string cmd = $"/C mklink /D \"{finalTarget}\" \"{source}\"";
        ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", cmd)
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        try
        {
            Process p = Process.Start(psi);
            string output = p.StandardOutput.ReadToEnd();
            string error = p.StandardError.ReadToEnd();
            p.WaitForExit();

            if (!string.IsNullOrEmpty(error) || !Directory.Exists(finalTarget))
            {
                bool wantsAdmin = EditorUtility.DisplayDialog("Permission Denied?",
                    "Failed to create symbolic link. Probably needs administrator privileges.\n\nDo you want to relaunch Unity as admin?",
                    "Relaunch", "Cancel");

                if (wantsAdmin)
                    RelaunchAsAdmin();
                else
                    ShowErrorMessage("Symbolic link creation failed: " + error);
            }
            else
            {
                if(EditorUtility.DisplayDialog("Symbolic link created!", $"Symbolic link created:\n {finalTarget}", "Ok"))
                {
                    AssetDatabase.Refresh();
                    Close();
                }
            }
        }
        catch (System.Exception ex)
        {
            ShowErrorMessage("Exception during symbolic link creation: " + ex.Message);
        }
    }

    /// <summary>
    /// Relaunches Unity with admin rights.
    /// </summary>
    private void RelaunchAsAdmin()
    {
        string unityPath = Process.GetCurrentProcess().MainModule.FileName;
        string args = $" -projectPath \"{Directory.GetParent(Application.dataPath)}\"";

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = unityPath,
                Arguments = args,
                Verb = "runas"
            });
            EditorApplication.Exit(0);
        }
        catch (System.Exception ex)
        {
            ShowErrorMessage("Could not restart Unity as admin: " + ex.Message);
        }
    }
}
