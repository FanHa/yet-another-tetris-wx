# UnitManager 注入约束说明

本说明用于约束 Battle Unit 的 UnitManager 注入行为，避免运行期出现隐式空引用或注入漂移。

## 约束规则

1. 注入入口唯一：必须通过 Unit.InjectUnitManager(UnitManager manager) 注入。
2. 禁止空注入：manager 为 null 会抛出异常。
3. 禁止替换注入：若已注入不同实例，会抛出异常。
4. 允许幂等重入：若重复注入同一实例，直接返回。
5. 激活前必须注入：Unit.Activate() 在未注入时会拒绝激活。

## 当前调用链

1. UnitManager.SpawnUnits(...) 创建 Unit。
2. UnitManager 在创建流程中调用 unit.InjectUnitManager(this)。
3. 之后进入 unit.Setup(...) 与后续 ActivateAllUnits()。

## 排查建议

1. 出现 ArgumentNullException(manager)：检查创建流程是否传入 null。
2. 出现 "already has a different UnitManager injected"：检查是否存在重复创建后跨管理器复用 Unit 实例。
3. 出现 "Cannot activate" 或 "requires an injected UnitManager"：检查该 Unit 是否通过 UnitManager.SpawnUnits(...) 创建并注入。
