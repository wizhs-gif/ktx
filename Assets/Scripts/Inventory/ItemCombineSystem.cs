using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品组合系统 - 处理物品组合逻辑
/// </summary>
public class ItemCombineSystem : MonoBehaviour
{
    public static ItemCombineSystem Instance { get; private set; }

    [Header("组合配方")]
    [SerializeField] private List<CombineRecipe> recipes = new List<CombineRecipe>();

    private Dictionary<string, CombineRecipe> recipeLookup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeRecipeLookup();
    }

    private void InitializeRecipeLookup()
    {
        recipeLookup = new Dictionary<string, CombineRecipe>();

        foreach (var recipe in recipes)
        {
            string key = GenerateRecipeKey(recipe.input1, recipe.input2);
            recipeLookup[key] = recipe;
        }
    }

    /// <summary>
    /// 尝试组合两个物品
    /// </summary>
    public bool TryCombine(string item1, string item2, out string resultItem)
    {
        resultItem = null;

        // 检查是否拥有这两个物品
        if (GameDataManager.Instance == null) return false;
        if (!GameDataManager.Instance.HasItem(item1)) return false;
        if (!GameDataManager.Instance.HasItem(item2)) return false;

        // 查找配方
        string key = GenerateRecipeKey(item1, item2);
        if (recipeLookup.TryGetValue(key, out CombineRecipe recipe))
        {
            // 执行组合
            GameDataManager.Instance.RemoveItem(item1);
            GameDataManager.Instance.RemoveItem(item2);
            GameDataManager.Instance.AddItem(recipe.result);

            resultItem = recipe.result;
            Debug.Log($"[ItemCombineSystem] 组合成功: {item1} + {item2} = {recipe.result}");
            return true;
        }

        Debug.Log($"[ItemCombineSystem] 无法组合: {item1} + {item2}");
        return false;
    }

    /// <summary>
    /// 检查两个物品是否可以组合
    /// </summary>
    public bool CanCombine(string item1, string item2)
    {
        string key = GenerateRecipeKey(item1, item2);
        return recipeLookup.ContainsKey(key);
    }

    /// <summary>
    /// 获取组合结果
    /// </summary>
    public string GetCombineResult(string item1, string item2)
    {
        string key = GenerateRecipeKey(item1, item2);
        if (recipeLookup.TryGetValue(key, out CombineRecipe recipe))
        {
            return recipe.result;
        }
        return null;
    }

    private string GenerateRecipeKey(string item1, string item2)
    {
        // 确保顺序一致
        return string.Compare(item1, item2) < 0 ? $"{item1}_{item2}" : $"{item2}_{item1}";
    }
}

/// <summary>
/// 组合配方
/// </summary>
[System.Serializable]
public class CombineRecipe
{
    public string input1;   // 输入物品1
    public string input2;   // 输入物品2
    public string result;   // 结果物品
    public string description; // 配方描述（可选）
}
