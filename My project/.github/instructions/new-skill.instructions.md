---
description: "Use when adding a gameplay skill or following the new-skill workflow, including skill code, Unity asset configuration, and validation."
---

# 新增技能 AI 工作流规范

本文件规定 AI 在新增技能任务中的分析、实施和验证流程。技能职责、数据所有权和代码契约以[技能系统职责与代码约定](../../Doc/技能系统职责与代码约定.md)为准；项目通用工作流程、Todo 执行规则、代码约束及 Unity 编辑器约束仍适用。

## 开始前

1. 读取[技能系统职责与代码约定](../../Doc/技能系统职责与代码约定.md)，并检查相关基类和同类技能实现。
2. 检查是否已有相似技能，并在 Todo 中注明。
3. 按下方完整清单列出可执行的 Todo，完成分析和 Todo 拆解前不得开始写代码。标注“按需”的事项也先列入 Todo；执行到该项时再核对是否适用并询问用户，用户确认不需要时将该 Todo 标记为已完成。
4. 按正常工作流程逐项实施和验证；Unity 资产修改遵守项目的 Unity 编辑器约束。
5. 如果任务同时修改角色原型配置，按角色流程处理并使用 `CharacterConfigRegistry`。

## 实施清单

### 代码文件

- [ ] 在 `Assets/Scripts/Units/Skills/` 新增技能类。
- [ ] 在 `Assets/Scripts/Model/Skills/` 新增该技能所需的具体 SkillDefinition 类；类型职责和实现约定遵循架构规范。
- [ ] 按需在 `Assets/Scripts/Units/Projectiles/` 新增 Projectile 类。
- [ ] 按需在 `Assets/Scripts/Units/Buffs/` 新增 Buff 和对应创建参数，并在 `Assets/Scripts/Units/Buffs/Core/BuffFactory.cs` 注册 Buff ID。
- [ ] 在 `Assets/Scripts/Units/Skills/Core/SkillFactory.cs` 注册技能 ID 到技能创建器的映射。
- [ ] 按技能触发方式实现所需生命周期接口或事件订阅。

### Unity 资产与场景

- [ ] 创建并填写该技能对应的 SkillDefinition 资产。
- [ ] 创建 SkillBackedCellDefinition 资产，填写 Cell ID、resonance，并关联 SkillDefinition。
- [ ] 将 SkillBackedCellDefinition 加入 CellDatabase 的注册列表。
- [ ] 确认 TetriCellFactory 的 cellDatabase 引用目标 CellDatabase。
- [ ] 若技能应进入奖励池，确认 RewardFactory 引用同一个 CellDatabase。
- [ ] 若技能 Cell 应在开局库存中可用，将 CellDefinition 加入 TetriInventoryInitConfig.CellDefinitions。
- [ ] 按需创建 Projectile Prefab 并挂载脚本组件。
- [ ] 按需在 SkillDefinition 中关联 Projectile Prefab 或其他直接下游资产。
- [ ] 按需创建 BuffDefinition 资产，填写与 BuffFactory 注册键一致的 ID；按照技能架构规范确定引用归属，并确认 Prefab 的必需组件引用已赋值。
- [ ] 在 SkillDefinition 的图标字段中直接关联技能图标。
- [ ] 在运行时配置/测试场景中加入该 CellDefinition，验证技能格应用流程。

### 外部资源

- [ ] 制作技能图标并作为 Sprite 资产关联到 SkillDefinition。
- [ ] 按需制作技能特效资源。

## 配置验证与手动测试

当前没有覆盖完整技能资产链路的自动化测试。修改或创建资产后检查 Unity Console；运行 Definition 的 `OnValidate` 和 Unity 菜单 `Tools/Validation/Validate Skill Definition Registry`。校验规则与保证范围以架构规范为准。

### 编辑器配置检查

- [ ] SkillDefinition 的 `OnValidate` 没有报错；通过受支持的 Unity 资产编辑流程设置其 ID。
- [ ] Unity 注册表校验菜单运行通过。
- [ ] SkillBackedCellDefinition 的 SkillDefinition 引用已赋值，且其 OnValidate 没有报错。
- [ ] CellDatabase 已注册该 SkillBackedCellDefinition，Cell ID 不重复且 Filler Definition 已赋值。
- [ ] 训练单位配置引用的 CellDefinition 正确，等级不小于 1，且该 Cell 未在同一单位上重复配置。

### 运行时手测

1. **资产引用检查**
   - 前置状态：具体 SkillDefinition 资产和技能 Cell Definition 已创建，且 Cell Definition 已加入 CellDatabase。将其加入 `Assets/Data/Gameplay/Inventory/TrainingGround/TranGroundFactionAInitConfig.asset` 中某个训练单位的 `cellDefinitions`。
   - 操作：保存资产并检查 Unity Console。
   - 预期：没有资产校验错误；训练单位配置引用的是目标技能 Cell Definition。

2. **训练场启动**
   - 前置状态：Unity 已打开 `Assets/Scenes/Setup.scene`，场景中的 GameController 引用了双方训练场初始化配置。
   - 操作：进入 Play Mode，选中 GameController，在组件上下文菜单执行 `Test Enter Train Ground`。
   - 预期：双方训练单位生成并进入战斗；训练场战斗不启用关卡进度。

3. **技能触发**
   - 前置状态：携带新技能的单位已生成。
   - 操作：按技能触发条件操作战斗；死亡触发技能则击杀携带者，主动技能则使其满足施放条件。
   - 预期：技能被正确创建并触发，效果与配置一致；Console 无异常。
