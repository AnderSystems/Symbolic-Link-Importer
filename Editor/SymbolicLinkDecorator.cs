using UnityEditor;
using UnityEngine;
using System.IO;

/// <summary>
/// Decorates symbolic links in the Project window with an overlay icon.
/// </summary>
[InitializeOnLoad]
public static class SymbolicLinkDecorator
{
    static GUIContent linkIcon;

    static SymbolicLinkDecorator()
    {
        linkIcon = EditorGUIUtility.IconContent("d_Linked");
        EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
    }

    private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
    {
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);

        if (!Directory.Exists(assetPath))
            return;

        string fullPath = Path.GetFullPath(assetPath);

        if (IsSymbolicLink(fullPath))
        {
            Rect iconRect = new Rect(selectionRect.x + selectionRect.width - 16, selectionRect.y, 16, 16);
            GUI.DrawTexture(iconRect, linkIcon.image);
        }
    }

    /// <summary>
    /// Checks if the given folder is a symbolic link.
    /// </summary>
    /// <param name="fullPath">The full path to check.</param>
    /// <returns>True if it's a symbolic link, false otherwise.</returns>
    private static bool IsSymbolicLink(string fullPath)
    {
        var dirInfo = new DirectoryInfo(fullPath);
        return dirInfo.Exists && dirInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
    }
}
