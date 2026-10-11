# 新增技能清单

新增一个技能时需要完成的所有事项。

## A. 代码文件

- [ ] `Assets/Scripts/Units/Skills/` 新增技能类
- [ ] `Assets/Scripts/Units/Skills/` 新增 SkillConfig 和对应 LevelConfig 类
- [ ] `Assets/Scripts/Units/Projectiles/` 新增 Projectile 类（按需）
- [ ] `Assets/Scripts/Units/Skills/Core/SkillFactory.cs` 注册技能 ID 到技能创建器的映射
- [ ] 按技能触发方式实现所需生命周期接口或事件订阅

> **说明：** 技能 Cell 使用通用 `SkillCell`，通过 `SkillBackedCellDefinition` 关联技能定义，通常不需要新增 Cell 子类。
>
> Cell ID 和 Affinity 在对应的 `CellDefinition` 资产中配置，详见下方 Unity 编辑器配置。

## B. Unity 编辑器配置

- [ ] 创建对应类型的 SkillConfig 资产并填写各级数值及所需引用
- [ ] 创建 SkillDefinition 资产，配置显示名称、描述、SkillConfig 和图标，并确保其 ID 与 SkillFactory 注册键一致
- [ ] 创建 SkillBackedCellDefinition 资产，填写 Cell ID、Affinity，并关联 SkillDefinition
- [ ] 将 SkillBackedCellDefinition 加入 CellDatabase 的注册列表
- [ ] 确认 TetriCellFactory 的 cellDatabase 引用目标 CellDatabase
- [ ] 若技能应进入奖励池，确认 RewardFactory 引用同一个 CellDatabase
- [ ] 若技能 Cell 应在开局库存中可用，将 CellDefinition 加入 TetriInventoryInitConfig.CellDefinitions
- [ ] 创建 Projectile Prefab 并挂载脚本组件（按需）
- [ ] 在 SkillConfig 中关联 Projectile Prefab 或其他所需资产（按需）
- [ ] 在 SkillDefinition 的图标字段中直接关联技能图标
- [ ] 在运行时配置/测试场景中加入该 CellDefinition，验证技能格应用流程

## C. 外部资源

- [ ] 制作技能图标并作为 Sprite 资产关联到 SkillDefinition
- [ ] 制作技能特效资源（按需）

## D. 配置验证与手动测试

当前没有覆盖完整技能资产链路的自动化测试，也没有全局 Cell 注册验证菜单。配置校验由各资产的 `OnValidate` 执行；修改或创建资产后，检查 Unity Console 中是否出现校验错误。

### 编辑器配置检查

- [ ] SkillConfig 的等级列表、数值范围和必需引用通过对应配置类的校验
- [ ] SkillDefinition 的 ID 符合 `skill.example_name` 格式并与 SkillFactory 注册键一致，且配置引用已赋值（Inspector 中 ID 为只读，需通过受支持的 Unity 资产编辑流程设置）
- [ ] SkillBackedCellDefinition 的 SkillDefinition 引用已赋值（当前 Inspector 不提供该引用的自动校验）
- [ ] CellDatabase 已注册该 SkillBackedCellDefinition，Cell ID 不重复且 Filler Definition 已赋值
- [ ] 训练单位配置引用的 CellDefinition 正确，等级不小于 1，且该 Cell 未在同一单位上重复配置
- [ ] 确认 SkillDefinition 的 ID 与 SkillFactory 中的注册键完全一致

### 运行时手测

1. **前置状态：** 技能配置、技能定义和技能 Cell Definition 已创建；Cell Definition 已加入 CellDatabase。将其加入 `Assets/Data/Gameplay/Inventory/Units/TranGroundFactionAInitConfig.asset` 中某个训练单位的 `cellDefinitions`。
	**操作：** 保存资产并检查 Unity Console。
	**预期：** 没有配置校验错误；训练单位配置引用的是目标技能 Cell Definition。

2. **前置状态：** Unity 已打开 `Assets/Scenes/Setup.scene`，场景中的 GameController 引用了双方训练场初始化配置。
	**操作：** 进入 Play Mode，选中 GameController，在组件上下文菜单执行 `Test Enter Train Ground`。
	**预期：** 双方训练单位生成并进入战斗；训练场战斗不启用关卡进度。

3. **前置状态：** 携带新技能的单位已生成。
	**操作：** 按技能触发条件操作战斗；死亡触发技能则击杀携带者，主动技能则使其满足施放条件。
	**预期：** 技能被正确创建并触发，效果与配置一致；Console 无异常。

4. **死亡诅咒专项：** 在其他敌方单位仍存活时击杀携带者，确认区域生成并周期性施加诅咒；等待区域持续时间结束，确认区域销毁且不再施加效果。区域仍存在时结束战斗或清场，确认区域也被销毁。
