# 新增技能清单

新增一个技能时需要完成的所有事项。

## A. 代码文件

- [ ] `Assets/Scripts/Units/Skills/` 新增技能类
- [ ] `Assets/Scripts/Units/Skills/` 新增 SkillConfig 和对应 LevelConfig 类
- [ ] `Assets/Scripts/Units/Projectiles/` 新增 Projectile 类（按需）
- [ ] `Assets/Scripts/Model/Tetris/Cells/` 新增对应 Cell 类，设置 CellTypeId 和 Affinity
- [ ] `Assets/Scripts/Model/Tetris/Cells/CellTypeCatalog.cs` 新增 CellTypeId 枚举值
- [ ] `Assets/Scripts/Model/Tetris/Cells/CellDefinition.cs` 新建或更新对应 CellDefinition（填写 id/runtimeTypeName/config/affinity）
- [ ] `Assets/Scripts/Model/Tetris/Cells/CellDatabase.cs` 将该 CellDefinition 加入 definitions 列表
- [ ] `Assets/Scripts/Model/Tetris/Cells/CharacterConfigRegistry.cs` 若涉及角色原型，新增对应 CharacterBaseStatConfig 字段
- [ ] `Assets/Scripts/Model/ProjectileConfig.cs` 新增 Prefab 字段（按需）

## B. Unity 编辑器配置

- [ ] 创建 SkillConfig ScriptableObject 资产并填写各级数值
- [ ] 在 CellDefinition 资产中挂接 SkillConfig
- [ ] 确认 TetriCellFactory 的 cellDatabase 已绑定到目标 CellDatabase 资产
- [ ] 若涉及角色原型，在 CharacterConfigRegistry 资产中挂接 CharacterBaseStatConfig
- [ ] 在 TetriCellTypeResourceMapping 资产中新增 CellTypeId → 图标映射
- [ ] 创建 Projectile Prefab 并挂载脚本组件（按需）
- [ ] 在 ProjectileConfig 资产中挂接 Prefab（按需）
- [ ] 在 OprationTable 初始格子列表中加入新 CellTypeId（方便测试）
- [ ] 执行 `Tools/Validation/Validate CellType Registry`

## B+. 迁移辅助（仅首次重构后或批量导入时）

- [ ] 执行 `Tools/Validation/Migrate Cell Definitions From Code` 自动生成/更新 CellDefinition 并回写 CellDatabase

## C. 外部资源

- [ ] 制作技能图标并导入到 Resources
- [ ] 制作技能特效资源（按需）
