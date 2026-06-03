# 新增Cell的CheckList

- 创建图标并导入到Resources
- Model.Tetri中创建新Cell类
- [技能] 在Units.Skills里新建Skill类
- [投射物] 如果技能涉及projectile,在Units.Projectiles里新增projectile类
- [投射物Prefab] 在prefabs.BattleUnit.Projectiles新增projectile prefab
- CellResourceMapping中新增CellType 与 图标的对应关系
- Model.Rewards中创建对应的reward类
- OprationTable 中的initial cells 新增Cell 类型,方便测试

## ThunderStrike 最小战斗验证清单

- [ ] 配置检查: ProjectileConfig 已配置 ThunderStrikePrefab，且预制体上挂有 ThunderStrikeProjectile 组件
- [ ] 配置检查: AffinityColorConfig 已包含 Electric 配色条目（边框色与遮罩色）
- [ ] 基线验证(电系格子=0): 释放雷击后目标立即受伤，眩晕时长约 0.7 秒
- [ ] 成长验证(电系格子=3): 伤害和眩晕均高于基线，眩晕约 0.94 秒
- [ ] 上限验证(电系格子=10): 眩晕时长封顶在 1.3 秒，不继续增长
- [ ] 视觉验证: 雷击特效在目标头顶生成，短暂显示后自动销毁
- [ ] 健壮性验证: 目标在释放瞬间失效时不报错，不出现空引用
- [ ] 健壮性验证: 连续施放时可重复生效，不出现永久眩晕或状态残留
