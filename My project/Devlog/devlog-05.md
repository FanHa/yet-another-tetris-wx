## 近期完成
### 奖励系统
- 战斗结束选取奖励流程
- 奖励类别与应用: 新角色, 新Tetri, 升级Tetri核心Cell, 升级Tetri非核心Cell, 升级角色
- UI实现的奖励页面中显示世界元素创建的Tetri

### 优化与Tetri相关的MVC流程
现在Tetri会被多个板块引用, 各个板块自己监听Tetri的变动并刷新显示, 当tetri变化时(比如通过奖励升级),各个板块自动更新
- OperationTable
- TetriInventory
- UnitInventory(监听OperationTable)

### 冰系技能
- 冰系主题: 控制, 限制地方移动,攻击,能量回复
- 4个冰系技能的全流程实现

### 其他
- 重构优化BattleFieldController 的逻辑, 梳理拆分出了UnitManager