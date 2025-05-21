## 近期进展
- 将原本基于 UI 实现的 Tetri 仓库和操作台转换为世界元素，便于后续扩展更多特性。
- 实现了方块仓库（TetriInventory）的 MVC 架构。
- 实现了操作台（OperationTable）的 MVC 架构。

### 俄罗斯方块（Tetri）从仓库（Inventory）放置到操作台（OperationTable）的代码设计与实现
- **MainController**
  - **TetriInventoryController**
    - 管理 `TetriInventoryView`，通过接口刷新仓库物件的渲染。
    - 管理 `TetriInventoryModel`，监听数据变动并调用 View 展示。
    - 负责创建 `Operation.Tetri`（GameObject）。
  - **OperationTableController**
    - 管理 `OperationTableView`。
    - 管理 `OperationTableModel`，监听数据变动并调用 View 展示。
  - 监听 `TetriInventoryController` 中创建的 `Operation.Tetri`（GameObject）的 `OnBeginDrag`、`OnDrag` 和 `OnEndDrag` 事件：
    - 创建影子 Tetri 以便拖动。
    - 判断拖动到 `OperationTable` 上时，调用 `OperationTableController` 接口修改 `OperationTableModel`（通过事件刷新 View）。
    - 调用 `TetriInventoryController` 接口修改 `TetriInventoryModel`（通过事件刷新 View）。

---

## 问题记录
在实现仓库系统（TetriInventory）时，仓库中有若干个俄罗斯方块（Tetri）。目标是：
- 点击物体时可以拖动物体。
- 点击仓库背景板时可以滑动整个仓库。

### 问题描述
在实际开发中，遇到了点击物体时大部分情况下（这种不确定性让问题排查耗费了大量时间）直接触发了背景板的滑动。

### 排查过程
排查了以下可能的原因：
- **Layer** 设置问题。
- **Mask** 和 **Collider**（包括 CompositeCollider）的配置。
- **Raycast** 检测。
- 世界坐标与 UI 坐标的转换。
- Unity 原生的 `OnDragXXX` 和 `OnPointerXXX` 接口。

### 问题原因
最终发现是由于仓库背景和物体的 `position.z` 都为 `0`，导致点击事件的优先级混乱。将仓库背景的 `position.z` 改为 `1` 后，问题得以解决。

---

## 下一步计划
- 实现操作台上的俄罗斯方块放回仓库的机制。
- 制作一些基本的角色方块和属性方块，打通从操作台到战场 Unit 的完整流程。

## 总结
完成了从仓库拖拽俄罗斯方块到操作台的流程