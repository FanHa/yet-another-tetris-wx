# 新增技能清单

新增一个技能时需要完成的所有事项。

## A. 代码文件

- [ ] `Assets/Scripts/Units/Skills/` 新增技能类
- [ ] `Assets/Scripts/Units/Skills/` 新增 ConfigGroup 和 Config 类
- [ ] `Assets/Scripts/Units/Projectiles/` 新增 Projectile 类（按需）
- [ ] `Assets/Scripts/Model/Tetris/Cells/` 新增对应 Cell 类，设置 CellTypeId 和 Affinity
- [ ] `Assets/Scripts/Model/Tetris/Cells/CellTypeCatalog.cs` 新增 CellTypeId 枚举值
- [ ] `Assets/Scripts/Model/Tetris/Cells/CellLevelConfigManager.cs` 新增 ConfigGroup 字段
- [ ] `Assets/Scripts/Model/Tetris/Cells/TetriCellFactory.cs` 在 BuildTypeMaps() 中注册新 Cell
- [ ] `Assets/Scripts/Model/ProjectileConfig.cs` 新增 Prefab 字段（按需）

## B. Unity 编辑器配置

- [ ] 创建 ConfigGroup ScriptableObject 资产并填写各级数值
- [ ] 在 CellLevelConfigManager 资产中挂接 ConfigGroup
- [ ] 在 TetriCellTypeResourceMapping 资产中新增 CellTypeId → 图标映射
- [ ] 创建 Projectile Prefab 并挂载脚本组件（按需）
- [ ] 在 ProjectileConfig 资产中挂接 Prefab（按需）
- [ ] 在 OprationTable 初始格子列表中加入新 CellTypeId（方便测试）

## C. 外部资源

- [ ] 制作技能图标并导入到 Resources
- [ ] 制作技能特效资源（按需）
