#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// 一键修复Build Settings，确保所有游戏场景已添加
/// 菜单: Tools > KTX > 修复Build Settings
/// </summary>
public static class BuildSettingsFixer
{
    [MenuItem("Tools/KTX/修复Build Settings")]
    public static void FixBuildSettings()
    {
        string[] requiredScenes = new string[]
        {
            "Assets/Scenes/Start.unity",
            "Assets/Scenes/Home.unity",
            "Assets/Scenes/SubWay.unity",
            "Assets/Scenes/Hospital.unity",
            "Assets/Scenes/OfficeBuilding.unity",
            "Assets/Scenes/ResidentialComplex.unity",
            "Assets/Scenes/Square.unity",
        };

        // 获取当前已有的场景
        var existingScenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            existingScenes.Add(scene);
        }

        bool changed = false;

        for (int i = 0; i < requiredScenes.Length; i++)
        {
            string path = requiredScenes[i];
            bool found = false;
            foreach (var existing in existingScenes)
            {
                if (existing.path == path)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                existingScenes.Add(new EditorBuildSettingsScene(path, true));
                changed = true;
                Debug.Log($"[BuildSettingsFixer] 已添加场景: {path}");
            }
        }

        if (changed)
        {
            EditorBuildSettings.scenes = existingScenes.ToArray();
            Debug.Log("[BuildSettingsFixer] Build Settings 修复完成！Start 场景已设为第一个。");
        }
        else
        {
            Debug.Log("[BuildSettingsFixer] 所有场景已在 Build Settings 中，无需修改。");
        }

        // 确保 Start 场景在第一位
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        int startIndex = scenes.FindIndex(s => s.path == "Assets/Scenes/Start.unity");
        if (startIndex > 0)
        {
            var startScene = scenes[startIndex];
            scenes.RemoveAt(startIndex);
            scenes.Insert(0, startScene);
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log("[BuildSettingsFixer] Start 场景已移至第一位。");
        }
    }
}
#endif
