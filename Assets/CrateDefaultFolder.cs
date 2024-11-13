#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public static class ProjectTool
{
    [MenuItem("ProjectTool/Crate Default Folders")]
    public static void CrateDefaultFolder()
    {
        CreateAssetFolder("_Game");
        CreateAssetFolder("Resources");
        CreateAssetFolder("Plugins");
        CreateAssetFolder("SandBox");
        CreateAssetFolder("StreamingAssets");

        List<string> gameFolders = new List<string>
        {
            "_Scenes",
            "Art",
            "Audio",
            "Code",
            "Editor",
            "Level",
            "Settings",
            "ShaderVariants",
        };

        foreach (var folder in gameFolders)
        {
            CreateAssetFolder("_Game/" + folder);
        }

        List<string> artFolders = new List<string>
        {
            "Materials",
            "Models",
            "Prefabs",
            "Textures",
            "UI"
        };

        foreach (var folder in artFolders)
        {
            CreateAssetFolder("_Game/Art/" + folder);
        }

        List<string> codeFolders = new List<string>
        {
            "_Framework",
            "Event",
            "UI",
            "Level"
        };

        foreach (var folder in codeFolders)
        {
            CreateAssetFolder("_Game/Code/" + folder);
        }

        List<string> levelFolders = new List<string>
        {
            "Prefabs",
            "AssetData"
        };

        foreach (var folder in levelFolders)
        {
            CreateAssetFolder("_Game/Level/" + folder);
        }

        AssetDatabase.Refresh();
    }

    private static void CreateAssetFolder(string folderPath)
    {
        var path = "Assets/" + folderPath;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            File.Create(path + "/.gitkeep");
        }
    }
}
#endif