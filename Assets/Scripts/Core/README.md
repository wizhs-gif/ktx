# 核心系统框架

## 系统架构

```
GameBootstrap (启动器)
    ├── GameDataManager (数据管理)
    ├── SaveSystem (存档系统)
    ├── InteractSystem (交互系统)
    ├── UIManager (UI管理)
    └── GameEvents (事件中心)
```

## 核心系统说明

### 1. GameEvents (事件中心)
- 所有系统通过此类通信，避免直接引用
- 提供全局事件：精神值变化、金钱变化、物品获取/失去、天数变化等

### 2. GameDataManager (数据管理器)
- 存储所有游戏状态数据
- 精神值（0-5档）
- 金钱
- 物品列表
- 游戏标记

### 3. SaveSystem (存档系统)
- 使用PlayerPrefs存储游戏数据
- 支持保存、读取、删除存档
- 每日结束自动保存

### 4. InteractSystem (交互系统)
- 管理交互条件判断
- 执行交互效果
- 支持天数、精神值、物品、标记等条件

### 5. UIManager (UI管理器)
- 管理所有UI面板
- 面板栈管理
- HUD、对话、物品、时间选择、每日结束等面板

## 数据库

### ItemDatabase (物品数据库)
- 定义所有37个物品的数据
- 物品ID常量：`ItemIDs`
- 物品组常量：`ItemGroups`

### NPCDatabase (NPC数据库)
- 定义所有NPC的数据
- NPC出现条件
- NPC对话数据

### GameConfig (游戏配置)
- 各种游戏参数配置
- 工资、精神值影响、小游戏配置等

## 使用示例

### 修改精神值
```csharp
// 增加精神值
GameDataManager.Instance.AddSanity(1);

// 减少精神值
GameDataManager.Instance.ReduceSanity(1);

// 监听精神值变化
GameEvents.OnSanityChanged += (oldVal, newVal) => {
    Debug.Log($"精神值从{oldVal}变为{newVal}");
};
```

### 管理物品
```csharp
// 添加物品
GameDataManager.Instance.AddItem(ItemIDs.MEMORY_BLOOD);

// 检查物品
if (GameDataManager.Instance.HasItem(ItemIDs.MEDICINE_CHECK)) {
    // 拥有定检药物
}

// 监听物品获取
GameEvents.OnItemObtained += (itemId) => {
    Debug.Log($"获得物品: {itemId}");
};
```

### 检查交互条件
```csharp
InteractCondition condition = new InteractCondition();
condition.minSanity = 3;
condition.requiredItems.Add(ItemIDs.MYSTERY_INCENSE);

if (InteractSystem.Instance.CheckConditions(condition)) {
    // 条件满足，可以交互
}
```

### 执行交互效果
```csharp
InteractEffect effect = new InteractEffect();
effect.sanityChange = 1;
effect.moneyChange = -10;
effect.addItems.Add(ItemIDs.MYSTERY_ZHENHAO);

InteractSystem.Instance.ExecuteEffects(effect);
```

## 下一步开发

1. **UI系统实现** - 各个UI面板的具体实现
2. **交互点系统** - 具体的交互点逻辑
3. **小游戏系统** - 各个小游戏的实现
4. **NPC系统** - NPC行为和对话
5. **场景系统** - 地图切换和状态差分
