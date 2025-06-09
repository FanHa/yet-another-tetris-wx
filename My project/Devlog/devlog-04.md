## 近期完成
- 战斗单位(Unit)上的dot伤害管理实现(以火系技能为例)
- 火系技能的设计(dot伤害, 火元素Cell对技能的加成)
- 投射物(Projectile)的基础逻辑与衍生投射物(火球)的实现

### Unit 上的技能管理设计
- 技能管理器设计（从以前的冷却时间改成积蓄能量）
- 技能管理分为两层：SkillManager（纯C#，负责技能能量、就绪检测等逻辑）和 SkillHandler（MonoBehaviour，负责与Unit、动画、事件等交互）
- 每个技能有独立的能量需求（RequiredEnergy），每tick由SkillManager分发能量，能量积满后技能进入就绪状态
- SkillHandler监听技能就绪事件，通知Unit播放施法动画，动画结束后由Unit调用SkillHandler释放技能
- 技能释放事件（OnSkillCast）通过Unit对外抛出，BattleField等外部系统只需监听Unit的事件即可实现技能表现（如飘字、特效等）
- 整体结构解耦，便于扩展和维护，支持后续Buff/装备对能量机制的动态调整

## Buff 与 Dot 管理设计

### Buff 管理

- Buff 管理器分为两层：`BuffManager`（纯C#，负责Buff的增删、持续时间、效果叠加等逻辑）和 `BuffHandler`（MonoBehaviour，负责与Unit、表现、事件等交互）。
- BuffManager 负责所有 Buff 的生命周期管理，包括添加、刷新、移除、过期检测等。
- BuffHandler 负责监听 BuffManager 的事件，驱动 BuffViewer（表现层）显示、更新和销毁 Buff 图标。
- 支持 Buff 的唯一性、叠加、刷新等机制，便于扩展多种状态效果。
<!-- - BuffViewer 作为 MonoBehaviour，负责 Buff 图标的视觉表现和跟随。 -->

### Dot（持续伤害）管理

- Dot 管理同样分为两层：`DotManager`（纯C#，负责所有 Dot 的添加、刷新、堆叠、爆炸等逻辑）和 `DotHandler`（MonoBehaviour，负责与Unit、表现、事件等交互）。
- DotManager 负责每种 Dot 的独立管理，支持不同技能来源的 Dot 独立存在、同技能来源的 Dot 替换、堆叠与爆炸等机制。
- DotHandler 负责定时 Tick，驱动 DotManager 结算伤害、触发爆炸等事件，并可用于表现层（如飘字、特效）。
- Dot 机制与 Buff 机制解耦，便于后续扩展更多持续性效果（如中毒、冰冻等）。