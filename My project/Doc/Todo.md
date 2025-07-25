# 💡 Ideas & Future Tasks

## 📦 分类说明
- ✅ Done：已实现
- 🔄 In Progress：正在开发
- 🧠 Idea：仅是灵感，尚未计划实现
- 🐞 Bug：待确认或解决的问题
- 🛠️ Technical: 实现

### 🧩 Cell相关
- 🧠 burn 与 freeze 的联动效果(触发所有dot伤害,并完全僵直一段时间)
- 🧠 每在战场上存活1秒,就提升x属性
- 与一个敌人交换某个属性(一次性)
- Cell类型考虑做成ScrptableObject
- 优化TrainGround 设置Model.Cell 并创建的流程

### 📊 UX / UI
- 🛠️ 单位预览时,需要更清晰的展示单位最终的各种属性(基础属性 + 加成)
- 拖动tetri时,让tetri出现在手指的上方一点,方便玩家观看拖动的位置与效果
- 更直观的血量显示,像LOL那样,随着血量的增多,血条变密
- Unit详情页面,技能冷却时间单独拎出来显示


### 🔧 重构 / 优化
- 🛠️ Unit的enemyUnits 的变量名表意更清晰(按距离排序)
- 🧠 投掷物技能造成伤害的方式似乎很类似,是不是可以统一实现

### 奖励系统


### 战斗系统
- 战斗统计页面需要允许下拉
- 投射物在投出后,但还没命中敌人,此时敌人死亡会导致投掷物停留在敌人位置不被销毁
- 是否换成回合制,所有单位排队依次行动,行动力高的单位排进队伍的次数更多
- Unit战斗时的能量获取优化

### 操作台

### 其他
- 🧠 匹配系统,记录胜场,存入服务器,匹配相同胜场的对手(不需要对手上线)
- 做大关卡时可以保留几个技能到角色上开启新征程
- 更加只能的AI的可行性
- UnitInventoryModel 从ScriptableObject 换成 MonoBehavior



## ✅ 使用建议
- 每次写 Devlog 时整理一遍 ideas.md，将已完成的点挪入归档区
- 不强制时间或优先级排序，灵感最重要
- 若变为正式开发任务，可以考虑复制内容为 GitHub issue 或放入计划文档

## ✅ 已实现历史（可定期归档）
- OperationTable 与 Resource 位置对调
- 有便捷的方式(主要是用来Debug)触发奖励
- 不允许出现相同的character
- 🛠️ 整理和分离UnitManage 和 BattleField 之间的逻辑
- BlazingField 技能视觉效果画一个空心透明的圆,让范围更清晰
- 战场上点击一个Unit,动态显示它的信息(属性,技能)
- 战场上点击一个Unit,动态显示它的信息(当前状态)



## 已取消
- 资源列表(TetriResource)允许下拉
- 🧠 进入战场时再预览会生成的单位?
- 🛠️ 从操作台切换到战场时,提供一个粒子效果,环绕角色Cell一周,表明这个角色会拥有哪些Cell

