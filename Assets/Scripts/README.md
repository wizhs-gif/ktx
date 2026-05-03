# 游戏脚本架构说明

## 目录结构

```
Scripts/
├── Core/                    # 核心系统
│   ├── GameEvents.cs        # 事件中心
│   ├── GameDataManager.cs   # 数据管理器
│   ├── SaveSystem.cs        # 存档系统
│   ├── InteractSystem.cs    # 交互系统
│   ├── UIManager.cs         # UI管理器
│   ├── GameBootstrap.cs     # 游戏启动器
│   ├── ItemDatabase.cs      # 物品数据库
│   ├── NPCDatabase.cs       # NPC数据库
│   └── GameConfig.cs        # 游戏配置
│
├── UI/                      # UI系统
│   ├── HUDManager.cs        # HUD管理器
│   ├── SanityDisplay.cs     # 精神值显示
│   ├── MoneyDisplay.cs      # 金钱显示
│   ├── TimeDisplay.cs       # 时间显示
│   ├── MapNameDisplay.cs    # 地图名称显示
│   ├── TimeSelectionUI.cs   # 时间选择UI
│   └── DayEndUI.cs          # 每日结束UI
│
├── Inventory/               # 物品系统
│   ├── ItemCollectionUI.cs  # 物品图鉴UI
│   ├── ItemSlot.cs          # 物品槽
│   ├── ItemDetailPanel.cs   # 物品详情面板
│   ├── ItemCombineSystem.cs # 物品组合系统
│   ├── ItemCombineUI.cs     # 物品组合UI
│   └── ItemSelectionPanel.cs# 物品选择面板
│
├── MiniGame/                # 小游戏系统
│   ├── MiniGameBase.cs      # 小游戏基类
│   ├── MiniGameManager.cs   # 小游戏管理器
│   ├── GazeMiniGame.cs      # 眺望小游戏
│   ├── DiceMiniGame.cs      # 摇骰子小游戏
│   └── SlotMachineMiniGame.cs# 老虎机小游戏
│
├── NPC/                     # NPC系统
│   ├── NPCController.cs     # NPC控制器
│   └── NPCManager.cs        # NPC管理器
│
├── Scene/                   # 场景系统
│   ├── SceneController.cs   # 场景控制器
│   └── GameFlowManager.cs   # 游戏流程管理器
│
├── Player/                  # 玩家系统（已有）
│   ├── PlayerMove.cs
│   ├── PlayerManager.cs
│   └── ...
│
├── TimeSystem/              # 时间系统（已有）
│   ├── TimeManager.cs
│   ├── TimeSegment.cs
│   └── TimelineManager.cs
│
├── Dialogue/                # 对话系统（已有）
│   ├── DialogueManager.cs
│   ├── NpcDialogueTrigger3D.cs
│   └── ItemSpawner3D.cs
│
└── Map/                     # 地图系统（已有）
    ├── GameManager.cs
    ├── ImageSelectionManager.cs
    └── SelectableImage.cs
```

## 核心架构

### 1. 事件驱动系统
- **GameEvents**: 全局事件中心，所有系统通过事件通信
- 避免系统间直接引用，降低耦合度

### 2. 数据管理
- **GameDataManager**: 存储所有游戏状态（精神值、金钱、物品、天数）
- **SaveSystem**: 使用PlayerPrefs进行数据持久化

### 3. 条件判断系统
- **InteractSystem**: 统一处理交互条件判断
- 支持天数、精神值、物品、标记等条件

## 已实现功能

### 核心系统 ✓
- [x] 精神值系统（0-5档）
- [x] 金钱系统
- [x] 物品系统（37个物品）
- [x] 存档系统
- [x] 事件系统

### UI系统 ✓
- [x] HUD显示（精神值、金钱、时间）
- [x] 物品图鉴UI
- [x] 物品组合UI
- [x] 时间选择UI
- [x] 每日结束UI

### 小游戏系统 ✓
- [x] 小游戏框架
- [x] 眺望小游戏
- [x] 摇骰子小游戏
- [x] 老虎机小游戏

### NPC系统 ✓
- [x] NPC行为控制
- [x] NPC条件出现
- [x] NPC对话系统

### 场景系统 ✓
- [x] 场景切换
- [x] 地图名称显示
- [x] 游戏流程管理

## 待完成功能

### 1. 美术资源
- [ ] 精神值图标
- [ ] 物品图标
- [ ] NPC立绘
- [ ] 小游戏UI素材

### 2. 配置数据
- [ ] 物品数据库配置
- [ ] NPC数据库配置
- [ ] 游戏参数配置

### 3. 场景搭建
- [ ] 各地图场景
- [ ] 室内场景
- [ ] 交互点布置

### 4. 剧情内容
- [ ] 开场剧情
- [ ] NPC对话内容
- [ ] 支线剧情

## 使用示例

### 修改精神值
```csharp
GameDataManager.Instance.AddSanity(1);
GameDataManager.Instance.ReduceSanity(1);
```

### 管理物品
```csharp
GameDataManager.Instance.AddItem(ItemIDs.MEMORY_BLOOD);
GameDataManager.Instance.HasItem(ItemIDs.MEDICINE_CHECK);
```

### 监听事件
```csharp
GameEvents.OnSanityChanged += (oldVal, newVal) => {
    Debug.Log($"精神值变化: {oldVal} -> {newVal}");
};
```

### 启动小游戏
```csharp
MiniGameManager.Instance.StartMiniGame(MiniGameType.Dice, (result) => {
    if (result.success) {
        Debug.Log("小游戏成功");
    }
});
```

## 下一步开发建议

1. **美术资源整合** - 导入所有美术资源
2. **数据配置** - 配置物品、NPC、游戏参数
3. **场景搭建** - 搭建各个地图场景
4. **剧情编写** - 编写对话和剧情内容
5. **测试优化** - 测试各个系统，优化性能
