using UnityEngine;
using UnityEditor;

public class AutoAddCollidersEditor
{
    [MenuItem("Tools/Colliders/给场景所有模型添加 Mesh Collider")]
    public static void AddMeshCollidersToAllObjects()
    {
        // 获取当前场景中所有 MeshFilter 组件
        MeshFilter[] meshFilters = Object.FindObjectsByType<MeshFilter>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        int addCount = 0;

        foreach (MeshFilter meshFilter in meshFilters)
        {
            GameObject obj = meshFilter.gameObject;

            // 如果这个物体没有 MeshRenderer，说明它可能不是可见模型，跳过
            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                continue;
            }

            // 如果已经有碰撞体，就不重复添加
            Collider existingCollider = obj.GetComponent<Collider>();
            if (existingCollider != null)
            {
                continue;
            }

            // 如果没有有效网格，也跳过
            if (meshFilter.sharedMesh == null)
            {
                continue;
            }

            // 添加 MeshCollider
            MeshCollider meshCollider = obj.AddComponent<MeshCollider>();

            // 设置网格
            meshCollider.sharedMesh = meshFilter.sharedMesh;

            // 静态场景物体一般不需要 Convex
            meshCollider.convex = false;

            // 标记场景已修改
            EditorUtility.SetDirty(obj);

            addCount++;
        }

        Debug.Log($"已完成：给 {addCount} 个物体添加了 Mesh Collider。");
    }

    [MenuItem("Tools/Colliders/给场景所有模型添加 Box Collider")]
    public static void AddBoxCollidersToAllObjects()
    {
        // 获取当前场景中所有 MeshRenderer
        MeshRenderer[] renderers = Object.FindObjectsByType<MeshRenderer>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        int addCount = 0;

        foreach (MeshRenderer renderer in renderers)
        {
            GameObject obj = renderer.gameObject;

            // 如果已经有碰撞体，就不重复添加
            Collider existingCollider = obj.GetComponent<Collider>();
            if (existingCollider != null)
            {
                continue;
            }

            // 添加 BoxCollider
            BoxCollider boxCollider = obj.AddComponent<BoxCollider>();

            // 让 BoxCollider 自动适配当前模型的大致大小
            Bounds bounds = renderer.localBounds;
            boxCollider.center = bounds.center;
            boxCollider.size = bounds.size;

            // 标记场景已修改
            EditorUtility.SetDirty(obj);

            addCount++;
        }

        Debug.Log($"已完成：给 {addCount} 个物体添加了 Box Collider。");
    }

    [MenuItem("Tools/Colliders/删除场景中所有自动添加的 Collider")]
    public static void RemoveAllColliders()
    {
        Collider[] colliders = Object.FindObjectsByType<Collider>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        int removeCount = 0;

        foreach (Collider collider in colliders)
        {
            Object.DestroyImmediate(collider);
            removeCount++;
        }

        Debug.Log($"已完成：删除了 {removeCount} 个 Collider。");
    }
}