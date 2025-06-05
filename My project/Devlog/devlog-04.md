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
