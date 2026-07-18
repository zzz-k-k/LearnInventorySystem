# 构建与学习记录

## 构建流程 1.1：背包 UI 骨架

- 对应高层阶段：完整背包 UI 骨架与基础数据表达。
- 对应需求：打开和关闭背包；显示网格；提供搜索区和物品右键操作入口。
- 当前可观察目标：运行 Unity 后按 B 键显示背包窗口；窗口顶部有搜索框和搜索按钮；窗口主体显示规则网格；右键格子时显示“使用、拆分、丢弃”三个菜单项，菜单项暂时不执行行为。
- 规划分支：`user-behavior-plan`。用户已给出行为流程，AI 负责翻译为最小 Unity 技术方案并在实现后讲解。

### 用户提出的构建步骤

1. 按下 B 键弹出一个表格，这个表格就是背包。
2. 在表格顶部添加搜索框和搜索按钮。
3. 右键一个格子时弹出“使用、拆分、丢弃”三个选项。
4. 本流程主要完成 UI 布局，不加入具体背包功能。

### AI 评审

正确部分：

- 有明确的用户入口：B 键。
- 有明确的完成画面：背包窗口、搜索区域、网格和右键菜单。
- 将菜单行为留空，使本流程只处理 UI 骨架，范围合适。

范围说明：

- 搜索、使用、拆分和丢弃逻辑不属于本流程。
- 人物、背包、物品定义和玩家物品实例的数据表达不属于本流程，作为当前高层阶段的下一条流程。
- 在真实物品数据接入前，右键菜单只用于验证布局和显示。

当前需要用户确认：

- B 键是否采用切换行为：背包关闭时按 B 打开，背包打开时再次按 B 关闭。

### 用户修正方案

B 键采用切换行为：背包关闭时按 B 打开，背包打开时再次按 B 关闭。

### 规范化后的职责与命名

- 输入职责：监听 B 键，只负责请求切换背包窗口显示状态。
- 窗口职责：维护背包 UI 根节点的显示与隐藏；场景开始时背包保持关闭。
- 布局职责：窗口顶部显示搜索框和搜索按钮，主体显示规则网格。
- 格子交互职责：右键格子时显示上下文菜单。
- 菜单职责：显示“使用、拆分、丢弃”三个命令文字，本流程不连接业务行为。
- 范围边界：人物和物品数据、搜索逻辑以及三个菜单命令的实际效果都延后处理。

### 已确认实现方案

1. 场景开始时背包窗口隐藏。
2. 按 B 在显示和隐藏之间切换背包窗口。
3. 背包窗口包含顶部搜索区和主体网格。
4. 右键格子时显示三个菜单项，点击菜单项暂时不产生业务结果。
5. 关闭背包时同时隐藏当前右键菜单，保持 UI 状态一致。

### AI 补充的最小技术方案

- `InventoryUiBootstrap`：当前场景没有背包对象，因此在场景加载后创建 Canvas、搜索区、网格和菜单骨架。
- `InventoryWindowView`：使用 Input System 监听 B 键，切换窗口状态，并负责显示或隐藏右键菜单。
- `InventorySlotView`：接收格子的右键点击，把点击位置交给窗口视图。
- 选择理由：三个职责分别对应“创建画面”“切换窗口”“格子右键”，不加入后续物品数据和业务规则。
- 教学要求：实现后按用户提出的行为顺序解释每个脚本、关键函数和调用关系。

### 代码对应关系

1. “按 B 键打开或关闭背包”对应 `InventoryWindowView.Update()`、`ToggleInventory()` 和 `SetVisible()`。Input System 在这一帧检测到 B 键后，只请求窗口切换状态；窗口关闭时会同时调用 `HideContextMenu()`。
2. “顶部搜索区和主体表格”对应 `InventoryUiBootstrap.BuildSearchArea()` 与 `BuildGrid()`。当前创建一个搜索输入框、一个搜索按钮和 6 列 5 行共 30 个空格子。
3. “右键格子显示三个选项”对应 `InventorySlotView.OnPointerClick()`。它只识别右键并把鼠标位置交给 `InventoryWindowView.ShowContextMenu()`；菜单统一由窗口显示并限制在 Canvas 范围内。
4. “本流程不加入具体背包功能”对应三个菜单按钮没有注册业务回调，搜索按钮也没有搜索回调；项目中没有新增人物、物品、背包数据或规则脚本。

AI 补充而非用户预先指定的技术细节：

- 使用 `[RuntimeInitializeOnLoadMethod]`，因为当前示例场景没有预制好的背包对象，需要在场景加载后生成 UI。
- 使用 `InputSystem.Keyboard.current` 读取 B 键，因为项目已经启用新 Input System。
- 将画面创建、窗口状态和格子点击分成三个脚本，使每个脚本只回答当前流程中的一个问题。
- `BuildIfNeeded()` 让运行时启动和自动校验复用同一条 UI 构建路径，并防止重复创建。

### 验证结果

- Unity `6000.2.14f1` 批处理编译通过，没有 C# 编译错误。
- 临时编辑器校验器调用真实构建路径并通过：初始窗口隐藏；存在 30 个格子和搜索输入；菜单包含“使用、拆分、丢弃”；第一次切换打开、右键显示菜单、第二次切换关闭并隐藏菜单。
- 临时校验器在验证后已删除，项目只保留当前流程需要的三个运行时代码文件。
- 当前完成的是结构与行为自动校验，尚未进行 Phase 5 的完成后痛点审查和用户理解检查。

### 完成后痛点 1：固定 UI 完全由运行时代码创建

- 分类：`next-step-needed`
- 观察依据：`InventoryUiBootstrap` 在代码中创建 Canvas、窗口、搜索区、30 个格子和右键菜单，固定布局无法直接在 Unity Scene/Prefab 编辑器中调整。
- 触发场景与后果：后续添加物品图标、数量、拖拽视觉状态或调整布局时，需要持续修改创建代码，视觉结构和交互代码会越来越难分开理解与维护。
- 用户初步方案：固定 UI 可以直接放在场景中；如果需要运行时创建或复用，应该先制作成 Prefab，再实例化 Prefab，而不是用代码逐个创建 UI 节点。
- AI 评审：判断正确。动态物品内容不等于固定 UI 结构也必须动态生成；场景对象适合单场景直接编辑，Prefab 适合复用和保持可视化编辑。当前项目建议把背包窗口制作成 Prefab 实例放入场景，格子可以进一步使用 Slot Prefab。
- 用户修正方案：固定 UI 不再由代码逐节点创建；`InventoryWindow` 直接保存在当前 `main` 场景，重复格子使用 `InventorySlot` Prefab。`InventoryWindowView` 继续检测 B 键和处理右键菜单，`InventoryUiBootstrap` 删除。
- 最终决定：解决。
- AI 对实现方案的修正：Canvas 必须保持激活，否则挂在其上的控制器无法继续检测 B 键；B 键只切换 `InventoryWindow`。右键事件由 `InventorySlotView` 接收，因此不再判断鼠标下是否为格子；`ContextPanel` 已存在于场景中，右键时只移动并激活它，不创建新菜单。
- 对应修改与验证：
  - 删除 `InventoryUiBootstrap.cs`，不再运行时创建 Canvas、窗口、格子或菜单。
  - `InventoryWindowView` 挂在始终激活的 Canvas 上，通过序列化引用控制场景中的 `InventoryWindow` 和 `ContextPanel`。
  - `InventorySlotView` 加入 `InventorySlot.prefab`，激活时从父级查找 Canvas 上的 `InventoryWindowView`。
  - `InventoryWindow` 与 `ContextPanel` 在场景中初始隐藏；`main.unity` 已设为 Build Settings 的首个场景。
  - Unity 序列化引用检查与最终脚本编译通过，没有 C# 编译错误。
  - 临时 Play Mode 验证器未产生可靠结果标记并已删除；之后用户在实际操作基础上接受当前 UI 行为并要求继续流程。
  - 用户补充了“左键点击格子时隐藏菜单”：`InventorySlotView.OnPointerClick()` 识别左键后调用 `InventoryWindowView.HideContextMenu()`。
  - 当前接受的边界是“左键点击格子隐藏”，不是“点击菜单外任意区域隐藏”；用户决定暂时保持这一范围。

### 完成后痛点 2：B 键开关与搜索框输入可能冲突

- 分类：`current-flow-regression`
- 观察依据：`InventoryWindowView.Update()` 在窗口打开期间仍全局监听 `Keyboard.current.bKey.wasPressedThisFrame`；当前窗口已经包含可获得键盘焦点的搜索输入框。
- 触发场景与后果：玩家点击搜索框并尝试输入包含字母 B 的搜索词时，同一次按键可能关闭整个背包，导致文本输入被窗口快捷键打断。
- 用户提出的方案：在 B 键判断外增加一层条件；当搜索输入框正在输入时，不进入 `ToggleInventory()`，离开输入状态后恢复 B 键切换。
- AI 评审：方向正确。TMP 输入框通过 `TMP_InputField.isFocused` 暴露当前是否拥有输入焦点；`InventoryWindowView` 可以序列化引用场景中的搜索输入框，并把 `!searchInputField.isFocused` 作为执行 B 键快捷操作的条件。
- 规范化边界：只屏蔽“搜索框拥有焦点”期间的 B 键快捷操作，不禁用输入框，也不改变右键菜单和其他背包行为；输入框失去焦点后自动恢复，无需额外维护一个容易失真的布尔状态。
- 对应修改：`InventoryWindowView` 新增序列化的 `TMP_InputField` 引用；`Update()` 先检查 B 键，再检查 `searchInputField.isFocused`，只有输入框未获得焦点时才调用 `ToggleInventory()`；`main.unity` 已绑定现有的 `InputField (TMP)`。
- 自动验证：`Assembly-CSharp.csproj` 编译成功，0 个警告、0 个错误；场景序列化字段指向 TMP 输入框组件。
- 运行时验证：用户在 Game 视图确认两种行为均通过：搜索框有焦点时输入包含 B 的文本不会关闭背包；搜索框失去焦点后，B 键恢复开关背包。
- 最终决定：解决。

### 理解状态

`mastered`

### 理解检查记录 1

- 用户已正确说明：`Update()` 检测 B 键；`ToggleInventory()` 对 `IsVisible` 取反并交给 `SetVisible()`；搜索框输入焦点会阻止切换；格子右键显示菜单、左键隐藏菜单。
- 需要修正 1：`SetVisible()` 不是“已经打开就直接进入 Hide”；实际是当前打开时，`ToggleInventory()` 传入 `false`，`SetVisible(false)` 先关闭 `InventoryWindow`，再调用 `HideContextMenu()` 清理菜单自身状态。
- 需要修正 2：`Canvas` 和 `InventoryWindow` 不是同一对象。`Canvas` 是始终激活的 UI 根对象并承载 `InventoryWindowView`；`InventoryWindow` 是 Canvas 下被切换的子对象。
- 需要修正 3：`ContextPanel` 已预先保存在场景中并位于 `InventoryWindow` 下；右键时只移动并激活它，不是在点击时创建。
- 当前结论：主要调用链已掌握；补充说明 Canvas、窗口和菜单的层级及关闭窗口时重置菜单状态的原因后再评审。

### 理解检查记录 2

- 用户已正确说明：Canvas 是整个 UI 的根对象；关闭 Canvas 会使挂载其上的 `InventoryWindowView.Update()` 停止执行。
- 尚需修正：`ContextPanel` 是 `InventoryWindow` 的子对象，所以关闭父对象时它会在层级中暂时不可见；但其自身的 `activeSelf` 不会因此变为 `false`。单独调用 `HideContextMenu()` 是为了重置菜单自身状态，避免重新打开父窗口时菜单再次出现。
- 用户追问并厘清：不需要在打开背包时再次隐藏菜单；关闭背包时已把 `ContextPanel.activeSelf` 清理为 `false`，重新打开父窗口不会改变这个自身状态，只有下一次右键才会再次激活菜单。
- 最终结论：用户能够连接 B 键输入、窗口状态、搜索焦点、格子点击、对象层级及改动影响，理解检查通过。

### 流程 1.1 痛点审查结论

- 必须处理的痛点 1：固定 UI 完全由运行时代码创建，已改为 Scene 与 Slot Prefab 并解决。
- 必须处理的痛点 2：B 键与搜索输入焦点冲突，已使用 `TMP_InputField.isFocused` 解决并通过 Game 视图验证。
- 可选项：点击菜单外任意区域时关闭菜单；当前只支持左键点击格子关闭，用户明确决定暂缓。
- 当前结论：流程 1.1 没有其他有证据支持的必修痛点，进入理解检查。

## 构建流程 1.2：基础数据表达并显示示例物品

- 对应高层阶段：完整背包 UI 骨架与基础数据表达。
- 对应需求：区分物品定义与玩家持有的物品数据；打开背包后能够看到物品、数量和空格。
- 当前可观察目标：运行游戏并打开背包后，至少一个格子根据基础背包数据显示示例物品及其数量，其余格子保持空状态。
- 当前范围：只建立支撑上述画面的最小数据和显示连接；不实现移动、交换、堆叠、拆分、搜索、使用、丢弃或数据库。
- 用户提出的构建步骤：先用一个 `struct` 表示物品，字段暂定为 `projectId`、类型 ID、数量 ID、`code`、特殊信息、显示名、图标 ID 和描述 ID；显示时读取这份结构中的数据，把内容放到格子的对应 UI 位置。
- AI 评审：
  - 正确部分：先建立物品数据表达，再由格子读取数据并显示，已经形成“数据 -> UI”的基本方向。
  - 需要澄清：`projectId`、`数量 ID`、`特殊`和`描述 ID`分别代表什么尚不明确；数量通常是数值而不是 ID。
  - 当前主要问题：显示名、图标、描述、类型和 `code` 属于同一种物品共享的定义；数量、所在格子以及玩家持有记录的唯一标识属于玩家当前拥有的物品。全部放进一个结构会让每一堆同种物品重复保存共享信息，并使以后修改物品定义时难以保持一致。
  - 暂不决定：在数据边界明确前，不先确定使用 `struct`、`class` 还是 `ScriptableObject`。
- 用户第一次修正：`slotId` 保存格子位置；类型 ID 表示物品类型；显示名、`code`、图标、描述和特殊规则由两个苹果共享；数量 ID 保存当前格子数量，两堆苹果的数量不同；用户暂把 `code` 理解为使用效果。
- AI 第二次评审：
  - 已正确划分：格子位置与数量属于玩家当前持有状态；显示名、图标、描述和物品规则属于共享定义。
  - 类型 ID 只表示“消耗品、装备”等类别，不能唯一确定苹果；还需要一个物品定义 ID 区分苹果、红药和蓝药。
  - 格子位置和数量是值，规范命名为 `slotIndex` 与 `quantity`，不使用 `slotId`、`quantityId`，避免把普通数值误解为实体标识。
  - `code` 是稳定的程序识别码，例如 `apple` 或 `small_health_potion`，不是使用效果；效果应作为独立规则表达。
  - 两个格子的持有记录后续还需要各自的实例标识，以便数据库精确更新某一条记录；本流程先确认概念，不提前实现数据库。
- 用户第二次修正：当苹果和红药的 `typeId` 都是“消耗品”时，使用唯一 `code` 区分具体物品。
- AI 第三次评审：该方案可行，前提是 `code` 对每种物品唯一且稳定。当前内存 Demo 可以先让玩家格子数据保存 `itemCode`，再用它查找共享物品定义；UI 不应通过大量 `if/switch` 硬编码每个 `code` 的名称和图标。数据库阶段再根据实际约束决定是否增加内部 `itemDefinitionId` 作为主键并保留 `code` 作为唯一业务键。
- 用户提出的数据到 UI 流程：通过玩家格子数据中的 `itemCode` 找到苹果定义，再把定义传给 `slotIndex` 对应的格子；格子显示定义中的图标，并显示该格子数据中的数量及其他信息。
- AI 流程评审：顺序正确。查找定义和选择目标格子应由格子外的协调职责完成；单个格子只负责接收已经匹配好的定义与数量并刷新自身 UI，避免每个格子各自持有和搜索完整定义集合。
- 规范化后的最小技术方案：
  1. 共享物品定义只保存当前画面需要的稳定 `code`、显示名和图标。
  2. 示例持有数据保存 `itemCode`、`slotIndex` 和 `quantity`。
  3. 一个协调职责在启动时读取示例持有数据，按 `itemCode` 查找定义，再按 `slotIndex` 找到格子。
  4. 格子接收匹配后的物品定义与数量，显示图标和数量；没有持有数据的格子清空显示。
  5. 描述、类型、效果和特殊规则先保留为已确认的定义归属，本流程不为尚未出现的 UI 提前实现。
- 已确认实现方案：用户确认采用“协调职责完成查找，格子只显示图标和数量”的最小方案。
- 代码对应关系：
  1. `ItemDefinition` 表达共享定义，当前只含唯一稳定 `code`、`displayName` 和 `Sprite icon`。
  2. `InventoryItemData` 表达示例持有数据，只含 `itemCode`、从 0 开始的 `slotIndex` 和 `quantity`；使用 class 而不是可变 struct，避免后续更新数量时修改到值副本。
  3. `InventoryDisplayController` 挂在 Canvas 上。启动时按 Content 子对象顺序取得格子，先清空全部格子，再构建 `code -> ItemDefinition` 查找表，最后把匹配到的图标和数量交给目标格子。
  4. `InventorySlotView.ShowItem()` 与 `ClearItem()` 只修改自身的 `ItemIcon` 和 `QuantityText`，不保存完整定义集合，也不根据 `code` 写物品分支。
  5. `InventorySlot.prefab` 新增固定的 `ItemIcon` 与 `QuantityText` 子节点；两者不接收 Raycast，不影响原有左右键点击。
  6. `main.unity` 保存一条苹果定义和一条示例持有数据：`itemCode = apple`、`slotIndex = 0`、`quantity = 3`，并引用 Content 作为格子容器。
  7. `apple-icon.png` 是当前 Demo 的透明苹果 Sprite，只用于让数据到 UI 的结果可识别。
- 验证结果：
  - Unity 正式资源导入与脚本编译完成，没有 C# 编译错误；独立 `dotnet build` 同样为 0 个警告、0 个错误。
  - 临时 Play Mode 校验通过：识别到 36 个格子，恰好 1 个格子拥有启用的 Sprite，数量文本为 `3`，其余格子为空。
  - 临时校验脚本及 Editor 目录已删除，不属于最终运行代码。
  - 用户在 Game 视图确认全部通过：第一格显示苹果图标和数量 3，其余格子为空，B 键、左右键菜单与搜索输入行为均正常。
- 实现后代码讲解状态：`complete`。已按两类数据、Controller 查找与分发、Slot 显示职责解释新增与修改代码；用户确认已经理解完整代码，可以继续流程。
- 理解状态：`not-started`。

### 流程 1.2 完成后痛点 1：空格子仍显示物品操作菜单

- 分类：`must-fix`
- 观察依据：当前只有第一格通过 `ShowItem()` 获得物品，其余 35 格通过 `ClearItem()` 保持为空；但每个 `InventorySlotView.OnPointerClick()` 都会在右键时无条件调用 `ShowContextMenu()`。
- 触发场景与后果：玩家右键任意空格子时，仍会看到“使用、拆分、丢弃”，让界面表现为可以对不存在的物品执行操作；后续给菜单接入真实功能时也无法判断菜单针对哪件物品。
- 用户提出的方案：右键没有物品的格子时不打开菜单；`InventorySlotView` 读取哪些格子存在物品，可通过数量判断，只有这些格子允许打开面板。
- AI 评审：行为结果正确，数量大于 0 确实意味着当前数据有物品；但 View 不应解析 `QuantityText.text` 或读取图标开关来推断业务状态，因为文字和图标只是输出表现，格式变化会让判断失真。
- 规范化建议：`ShowItem()` 在收到有效图标和数量时同步把私有占用状态设为 `true`，`ClearItem()` 设为 `false`；右键只检查该状态。当前只需要知道“是否有物品”，无需让格子持有完整定义集合或重新查找数据。
- 用户确认并给出的实现步骤：新增私有 `bool hasItem`；在 `ShowItem()` 中设为 `true`，在 `ClearItem()` 中设为 `false`；`OnPointerClick()` 的右键分支只有在 `hasItem` 为真时才显示菜单。
- AI 评审：步骤完整且职责位置正确，只需修改 `InventorySlotView`，不需要改动数据类、Controller 或 Prefab。
- 对应修改：`InventorySlotView` 新增私有 `hasItem`；`ShowItem()` 设置为真，`ClearItem()` 设置为假，右键分支增加 `hasItem` 条件。
- 验证结果：编译通过，0 个警告、0 个错误；临时 Play Mode 校验通过，空格子右键保持菜单隐藏，有物品格子右键正常打开菜单。
- 最终决定：解决。

### 流程 1.2 完成后痛点 2：菜单没有记录正在操作哪个格子

- 分类：`next-step-needed`
- 观察依据：`InventorySlotView.OnPointerClick()` 右键时只把屏幕坐标传给 `InventoryWindowView.ShowContextMenu()`；窗口只移动并显示菜单，没有保存触发菜单的格子或物品信息。
- 触发场景与后果：以后背包中有两个物品时，玩家分别右键它们都能看到菜单，但点击“使用、拆分、丢弃”时，系统无法知道命令应该作用于哪个格子。这会阻塞高层阶段 2 的“真正使用物品”。
- 用户初步方案：右键时传递 `slotIndex`，并直觉上由格子保存当前操作目标；同时认为传完整 `InventoryItemData` 或由菜单控制对象保存似乎也可行。
- AI 评审：
  - 传递 `slotIndex`：适合当前阶段，信息最小且不让 UI 依赖完整数据对象；菜单命令以后可凭位置向 Controller 查询最新数据。
  - 传递完整 `InventoryItemData`：技术上可行，但会让菜单直接依赖 Model，并可能在数量或位置变化后持有过期对象；当前没有证据需要这份耦合。
  - 格子适合保存“我是哪一格”的固定身份，即自己的 `slotIndex`；但不适合让 36 个格子分别保存“我是否是当前菜单目标”，否则每次右键都要同步清除其他格子的选择状态。
  - 当前只有一个 ContextPanel，因此“当前操作目标”也是唯一状态，应由统一显示和隐藏菜单的对象保存。
- 规范化建议：每个 `InventorySlotView` 保存自己的 `slotIndex`；右键时调用类似 `ShowContextMenu(position, slotIndex)`；菜单所有者保存一个 `currentTargetSlotIndex`，关闭菜单时清除它。
- 用户澄清并确认：不是要支持多选；如果每个格子保存“我是当前目标”，会分散成 36 份状态，而菜单保存“当前目标格子是哪个”只需一份状态。
- AI 评审：理解正确。每格仍需要自己的 `slotIndex` 作为身份，但唯一的当前菜单目标由菜单所有者集中保存。
- 对应修改：`InventoryDisplayController` 按 Content 子对象顺序为每个格子设置自己的 `slotIndex`；`InventorySlotView` 右键时把位置和 `slotIndex` 一起传给 `InventoryWindowView`；窗口保存可空的 `currentTargetSlotIndex`，`HideContextMenu()` 将其清为 `null`。
- 自动验证：C# 编译通过，0 个警告、0 个错误；Unity 正式程序集已重新生成。临时 Play Mode 校验依次验证右键格子 0 后目标为 `0`、隐藏菜单后为 `null`、右键格子 1 后同一份目标更新为 `1`、再次隐藏后为 `null`，结果为 `PASS`。
- 清理结果：临时 Play Mode 校验脚本、对应 `.meta` 和临时 Editor 目录均已删除，不属于正式代码。
- 最终决定：解决。每个格子只保存自己的固定身份 `slotIndex`；唯一的当前操作目标只由 `InventoryWindowView.currentTargetSlotIndex` 保存，不存在多选状态。

### 流程 1.2 痛点审查结论

- 已解决痛点 1：空格子不再打开物品操作菜单。
- 已解决痛点 2：菜单所有者只保存一份当前目标格子编号，并在关闭菜单时清空。
- 风险雷达复查：当前可观察流程没有其他有实际证据支持的必修痛点；数据库一致性、失败恢复、规模和真实业务规则留到对应高层阶段检查。
- 当前状态：痛点审查完成，进入最终理解检查。

### 流程 1.2 最终理解检查记录 1

- 用户已经正确连接：物品定义数据与玩家格子数据分开；右键格子的编号成为唯一菜单目标；未来使用命令应先根据目标格子找到物品，再进入具体处理职责。
- 需要修正 1：玩家格子数据保存在 `inventoryItems`，不保存在 `definitionsByCode`；后者只是刷新过程中临时构建的 `itemCode -> ItemDefinition` 查找表。
- 需要修正 2：当前 `RefreshSlots()` 只在 `InventoryDisplayController.Start()` 调用一次，并非每次打开背包都刷新；它先清空所有格子显示，再遍历玩家持有数据，把每条数据匹配到目标格子。
- 需要修正 3：`slot != null` 只表示该位置成功取得 `InventorySlotView` 组件，不表示格子有物品；是否有物品由匹配到该格子的玩家持有数据以及 `ShowItem()` 设置的 `hasItem` 表达。
- 目标清理澄清：关闭菜单或关闭背包会调用 `HideContextMenu()`，直接把 `currentTargetSlotIndex` 清为 `null`；重新打开背包本身不会调用 `ClearSlots()`。
- 当前结论：整体方向正确，但数据容器、刷新触发时机与 `slot != null` 含义仍需一次修正说明，理解状态暂不标记为 `mastered`。

### 流程 1.2 最终理解检查记录 2

- 用户确认已经理解并接受上述修正，要求进入下一步。
- 综合检查：用户此前已经连接两类数据、数据显示、菜单目标、未来使用行为和改动影响；经反馈后进一步澄清了临时定义查找表、刷新时机、格子组件存在与格子有物品的区别。
- 理解状态：`mastered`。
- 流程状态：`complete`。流程 1.2 已实现、验证、完成代码讲解、解决两个痛点并通过最终理解检查。

## 构建流程 1.3：丢弃确认与取消 UI

- 对应高层阶段：完整背包 UI 骨架与基础数据表达。
- 对应需求：点击物品的丢弃入口后显示确认与取消界面；本阶段只完成 UI 流程，不删除物品。
- 当前可观察目标：右键有物品格子，点击 `DiscardButton` 后隐藏右键菜单并显示 `SwitchPanel`；点击 `yes` 或 `no` 后关闭确认界面，物品仍然存在。
- 规划分支：`user-behavior-plan`。

### 用户提出的构建方案

1. 在场景中创建丢弃确认 UI，包含确认提示、`yes` 和 `no`。
2. 点击右键菜单中的丢弃按钮时显示确认界面。
3. 增加显示和隐藏该界面的函数；按钮点击处理可以参考格子的 `OnPointerClick()`。

### AI 评审

- 正确部分：固定确认界面继续由场景负责布局，脚本只控制状态；显示与隐藏函数可以形成当前流程的最小行为边界。
- 场景证据：`SwitchPanel` 已存在且初始隐藏；`DiscardButton`、`yes`、`no` 都具有 `UnityEngine.UI.Button`，当前 `Button.onClick` 尚未绑定函数。
- API 修正：普通 uGUI `Button` 已经接收指针点击，无需像格子一样实现 `IPointerClickHandler.OnPointerClick()`；可以通过 Inspector 的 `Button.onClick` 绑定脚本中的公开方法。
- 状态问题：现有 `HideContextMenu()` 会同时隐藏菜单并把 `currentTargetSlotIndex` 清为 `null`。如果点击丢弃时直接调用它，确认框出现后将失去未来真正执行丢弃所需的目标格子。
- 待用户决定：目标编号应在从右键菜单进入确认框时保留，并在 `yes`/`no` 关闭确认框时清除，还是采用另一套明确的目标转移方式。

### 用户对目标生命周期的判断

- 用户结论：进入确认框时不立即清空目标，等用户完成确认后再清空。
- 用户理由：立即清空会让取消时需要恢复，增加状态处理；用户担心目标清空还会造成物品先消失再出现。
- AI 评审：保留目标直到用户对确认框作出选择是正确的；但取消也代表本次操作结束，所以确认和取消都应清空目标。
- 概念修正：`currentTargetSlotIndex` 只是 UI 当前操作上下文，不是物品数据；清空它不会改变 `inventoryItems`，也不会调用 `ClearItem()`，因此物品不会从格子消失。

### 规范化后的最小方案

1. `InventoryWindowView` 序列化引用场景中的丢弃确认面板。
2. `ShowDiscardConfirmation()` 只在存在当前目标时执行：隐藏右键菜单的画面但保留 `currentTargetSlotIndex`，随后显示确认面板。
3. 确认和取消在本流程都只关闭确认面板并清空目标，不删除物品；真正删除行为留到内存背包功能阶段。
4. 关闭整个背包时也隐藏确认面板并清空目标，避免重新打开后恢复旧对话框。
5. `DiscardButton`、确认按钮和取消按钮使用各自的 `Button.onClick` 调用公开方法，不新增按钮指针接口。
6. 建议把场景对象规范命名为 `DiscardConfirmationPanel`、`PromptText`、`ConfirmButton`、`CancelButton`，使层级表达具体职责。

- 当前状态：方案评审完成，等待用户确认后实施。

### 实施确认

- 用户确认按规范化后的最小方案实现。

### 代码对应关系

1. `InventoryWindowView` 新增序列化的 `discardConfirmationPanel`，由场景引用固定确认面板。
2. `ShowDiscardConfirmation()` 先确认存在目标和面板；随后只隐藏 `ContextPanel` 的画面，不调用会清空目标的 `HideContextMenu()`，最后显示确认面板。
3. `ConfirmDiscard()` 与 `CancelDiscard()` 当前都调用私有 `CloseDiscardConfirmation()`；后者隐藏面板并把 `currentTargetSlotIndex` 清为 `null`，没有修改 `inventoryItems` 或格子显示。
4. `SetVisible(false)` 除了关闭右键菜单，也会调用 `CloseDiscardConfirmation()`，防止重新打开背包时恢复旧确认框。
5. `main.unity` 将场景对象规范命名为 `DiscardConfirmationPanel`、`PromptText`、`ConfirmButton`、`CancelButton`，并把 `DiscardButton`、确认按钮、取消按钮的 `Button.onClick` 分别绑定到三个公开方法。

### 验证结果

- `Assembly-CSharp.csproj` 编译通过：0 个警告、0 个错误。
- Unity 场景序列化检查通过：确认面板引用存在，三个按钮各自绑定正确的公开方法，四个场景对象名称已更新。
- 临时 Unity Edit Mode 状态校验通过：打开确认框时目标保持为 `0`；取消、确认和关闭整个背包都关闭确认框并清空目标。
- 临时校验器、对应 `.meta`、临时 Editor 目录和结果标记均已删除，没有进入正式代码。

### 实现后代码讲解状态

- 当前状态：`delivered`。
- 讲解顺序：按钮事件入口 -> 保留目标并切换面板 -> 确认/取消共同收尾 -> 关闭背包的状态清理。
- 等待用户：检查代码与 Game 视图行为，提出即时问题或确认可以继续。

### 实现后代码讲解确认

- 用户追问 `ShowDiscardConfirmation()` 没有显式 C# 调用者时由谁调用。
- AI 澄清：`DiscardButton` 的 uGUI `Button` 通过场景序列化的 `onClick` 事件调用该公开方法；方法中的 `discardConfirmationPanel.SetActive(true)` 负责显示确认框，因此 IDE 的纯 C# 引用搜索可能找不到调用者。
- 用户确认已经理解并要求继续，代码讲解阶段通过。

### 流程 1.3 完成后痛点 1：确认过程中仍能切换操作目标

- 分类：`must-fix`
- 观察依据：`DiscardConfirmationPanel` 的 RectTransform 只有 `360 x 180`，没有覆盖整个背包来阻止底层格子交互；`InventoryWindowView.ShowContextMenu()` 也没有判断确认框是否正在显示。
- 触发场景与后果：玩家为格子 A 打开丢弃确认框后，仍可在确认框外右键格子 B；`ShowContextMenu()` 会把 `currentTargetSlotIndex` 改成 B，并把右键菜单移动到最上层。随后确认操作将不再稳定对应最初看到的格子 A。
- 当前状态：等待用户提出保证确认期间目标稳定的处理思路。
- 用户初步方案：在 `ShowContextMenu()` 增加判断；确认框打开时不再执行其他菜单操作。
- AI 评审：该判断可以阻止右键打开新菜单并覆盖目标，方向正确且检查位置接近状态入口；但当前还没有覆盖左键路径。
- 补充证据：`InventorySlotView.OnPointerClick()` 的左键分支会直接调用 `HideContextMenu()`，该方法会把 `currentTargetSlotIndex` 清为 `null`，同时不会关闭丢弃确认框。因此确认期间左键点击格子仍会让确认框保留但目标丢失。
- 当前状态：等待用户补全同时覆盖右键替换和左键清空的方案。
- 用户补全方案：在 `InventorySlotView.OnPointerClick()` 最前面判断确认框是否打开；打开时直接 `return`，不再进入后面的左键或右键逻辑。
- AI 评审：方案正确且覆盖完整。检查发生在按键分支之前，因此右键不能替换目标、左键也不能清空目标；确认和取消按钮不是格子组件，不会被该判断阻止。
- 规范化：由 `InventoryWindowView` 暴露只读状态 `IsDiscardConfirmationVisible`，只回答确认框是否处于显示状态；`InventorySlotView` 使用该状态做早返回，不直接获取或控制窗口的私有 `discardConfirmationPanel`。
- 最小修改范围：只修改 `InventoryWindowView` 与 `InventorySlotView`；不改场景、物品数据、Controller 或确认按钮绑定。
- 用户实施确认：确认按上述最小方案修改。
- 对应修改：`InventoryWindowView` 新增只读属性 `IsDiscardConfirmationVisible`；`InventorySlotView.OnPointerClick()` 在任何左右键分支之前读取该状态，确认框显示时立即 `return`。
- 自动验证：正式 C# 编译通过，0 个警告、0 个错误；临时 Unity 状态校验通过，确认框打开后左键格子不清空目标、右键另一个格子不替换目标且不重新打开菜单，取消后才清空目标。
- 清理结果：临时校验器、对应 `.meta`、Editor 目录和结果标记均已删除。
- 用户继续确认：用户要求进入下一步，接受当前修复与验证结果。
- 最终决定：解决。确认期间所有格子点击均在入口处停止，目标保持稳定；确认、取消或关闭背包后才结束该操作上下文。

### 流程 1.3 痛点审查结论

- 已解决痛点 1：确认过程中底层格子点击不再替换或清空当前目标。
- 当前范围内没有第二个有实际证据支持的必修痛点。
- 延后事项：确认按钮真正删除物品、确认文案显示物品名称和数量、完整模态遮罩与焦点管理，分别留到真实丢弃行为或后续 UI 完善流程，不在当前 UI 骨架中提前实现。
- 当前状态：痛点审查完成，进入最终理解检查。

### 流程 1.3 最终理解检查记录 1

- 用户已经正确说明：丢弃按钮调用显示函数；确认和取消都隐藏确认 UI；`HideContextMenu()` 会把当前目标清为 `null`；未来通过 `CurrentTargetSlotIndex` 找到对应格子并修改玩家持有数据。
- 需要补全 1：右键格子首先由 `InventorySlotView` 调用 `ShowContextMenu()`，保存目标并显示右键菜单；之后点击 `DiscardButton`，才由场景中的 `Button.onClick` 调用 `ShowDiscardConfirmation()`。
- 需要补全 2：`IsDiscardConfirmationVisible` 由窗口只读地报告确认框状态；`InventorySlotView.OnPointerClick()` 使用该状态在入口早返回，从而同时阻止左键清空和右键替换目标。
- 责任边界修正：未来 `InventoryWindowView` 不应直接修改 `inventoryItems`；它应把目标格子交给拥有玩家数据和操作规则的 Controller/业务职责处理，再根据结果刷新 UI。
- 顺序约束：真实确认必须先读取目标、执行并成功更新数据、刷新显示，最后才能关闭确认框并清空目标；如果先调用当前 `CloseDiscardConfirmation()`，目标会提前丢失。
- 当前结论：整体行为与状态目的已经连接，但真实删除的职责归属和清理顺序还需一次补充，暂不标记为 `mastered`。

### 流程 1.3 最终理解检查记录 2

- 用户补充说明：`InventoryWindowView` 应只负责 UI 显示和点击交互，不负责后端具体逻辑。
- AI 规范化：这里的“后端”在当前 Unity 内存阶段更准确地称为业务规则与数据修改职责；窗口表达用户意图，Controller/业务层执行删除、报告结果并触发刷新。
- 综合结论：用户已经连接按钮事件、目标生命周期、确认状态、格子交互锁、未来删除顺序与职责边界。
- 理解状态：`mastered`。
- 流程状态：`complete`。流程 1.3 已实现、验证、完成代码讲解、解决完成后痛点并通过最终理解检查。

## 阶段 1 完成与拖拽边界决定

- 用户询问阶段 3 的含义以及拖拽应放在哪一阶段。
- AI 说明：阶段 3 是运行时内存数据上的完整背包行为，包括移动、交换、堆叠合并、拆分、搜索、使用和丢弃。
- 最终决定：不在阶段 1 单独制作“图标跟随鼠标、松开恢复”的空壳拖拽；拖拽视觉与移动到空格的真实数据变化一起作为阶段 3 的首批可观察流程，后续复用于交换、合并和拖出丢弃。
- 阶段 1 状态：`complete`。流程 1.1、1.2、1.3 均完整完成，UI 骨架和基础数据表达已经为后续行为提供入口与目标状态。

## 构建流程 2.1：使用示例物品并改变人物数值

- 对应高层阶段：简单物品使用效果。
- 当前目标：建立一条从背包“使用”入口到人物数值发生可见变化的最小完整链路。
- 当前状态：规划中，等待用户确定具体物品、人物数值、画面反馈和物品数量结果。
- 范围边界：不实现拖拽、交换、合并、拆分、搜索、真实丢弃、数据库或完整通用效果系统。

### 用户提出的行为方案

1. 增加人物生命值 UI。
2. 苹果作为示例物品，使用后恢复人物生命值。
3. 点击使用时，根据当前目标格子找到对应的玩家物品。
4. 根据玩家物品找到共享物品定义，读取其中的具体作用并执行。
5. 苹果数量减 1；数量减到 0 时格子变空。

### AI 初步评审

- 正确部分：流程已经连接使用入口、目标格子、玩家持有数据、共享定义、人物数值、数量变化和 UI 结果，覆盖阶段 2 的核心目标。
- 职责修正：`InventoryWindowView` 和按钮只表达“使用当前目标”；协调/业务职责根据 `CurrentTargetSlotIndex` 查找玩家物品并执行使用流程。
- 定义边界：`ItemDefinition` 适合描述效果类型和效果数值，例如“恢复生命值、10”；它不应直接持有人物对象或自行修改人物数据。
- 执行边界：业务逻辑读取定义中的效果描述，再修改人物生命值；不要在 UI 中按 `itemCode` 编写苹果专用分支。
- 数据结果：效果成功后才把数量减 1，并刷新生命值与背包格子；数量为 0 时移除或清空该格子的玩家持有数据表达。
- 当前待明确：人物生命值需要当前值和最大值；当恢复量超过最大生命值时，本次使用应产生什么结果。

### 生命值溢出决定

- 用户决定：人物 `95/100` 时使用恢复 10 点的苹果，生命值限制为 `100/100`，苹果仍然消耗 1 个。
- AI 复盘：该问题属于常规、低风险默认，不应为了“一次一个问题”单独阻塞一轮；后续对不会改变需求覆盖或架构的常规边界，由 AI 采用保守默认并明确记录。
- 当前流程默认：恢复后的生命值不超过最大生命值；点击使用并找到有效苹果后即执行并消耗 1 个，满血时也允许消耗。是否改为“满血拒绝使用”可在完成后规则审查中根据实际体验再调整。

### 规范化后的最小构建方案

1. 在场景中增加可见的人物生命值 UI，显示当前值与最大值。
2. 增加人物生命值状态，保存当前生命值、最大生命值，并提供受上限约束的恢复行为。
3. 扩展 `ItemDefinition`，用效果类型和效果数值描述苹果的“恢复生命值”效果；定义只描述，不直接修改人物。
4. 让玩家持有数据支持成功使用后数量减 1，并让数量为 0 的记录在刷新时表现为空格。
5. 由 Inventory Controller 接收 `UseButton` 的点击意图：读取当前目标格子，查找玩家物品与定义，执行效果，扣除数量，刷新生命值和格子显示，最后关闭菜单并清理目标。
6. 当前示例采用人物生命值 `50/100`、苹果恢复 `10`、初始数量 `3`；一次使用后的可观察结果为 `60/100` 和数量 `2`。

- 当前状态：行为方案与最小职责方案已形成，等待用户一次性确认实施。
- 用户实施确认：用户要求继续，按完整最小方案实施流程 2.1，不再拆分常规边界问题。

### 代码对应关系

1. 新增 `PlayerHealth`：只保存当前生命值和最大生命值，并用 `Restore()` 把恢复结果限制在上限内，解决“人物生命值由谁维护”的问题。
2. 新增 `PlayerHealthView`：只把 `PlayerHealth` 格式化为 `HP: current / max`，解决“人物数据怎样显示”的问题。
3. 修改 `ItemDefinition`：新增 `ItemEffectType`、`effectType` 和 `effectValue`；苹果在场景中配置为 `RestoreHealth + 10`，定义只描述效果。
4. 修改 `InventoryItemData`：新增 `HasItems` 和 `ConsumeOne()`；成功使用后数量减 1，数量为 0 时该记录仍存在于当前 Demo 数组中，但刷新时被视为空格。
5. `InventoryDisplayController` 改名为 `InventoryController` 并保留原 `.meta` GUID：它继续拥有定义、玩家物品和格子刷新，同时新增 `UseSelectedItem()` 协调目标查询、效果执行、数量消耗、两个 UI 刷新与菜单清理。
6. `main.unity` 新增始终可见的 `PlayerHealthPanel/HealthText`，把 `UseButton.onClick` 绑定到 `InventoryController.UseSelectedItem()`，并保存人物 `50/100`、苹果恢复 10、苹果数量 3 的示例配置。

### 运行时调用链

1. 右键苹果格子后，`InventoryWindowView` 保存 `CurrentTargetSlotIndex = 0` 并显示右键菜单。
2. 点击 `UseButton` 后，场景中的 `Button.onClick` 调用 `InventoryController.UseSelectedItem()`。
3. Controller 用目标编号找到 `InventoryItemData`，再用 `itemCode` 找到 `ItemDefinition`。
4. `TryApplyEffect()` 按 `effectType` 选择恢复生命值行为，调用 `PlayerHealth.Restore(effectValue)`；不是按 `apple` 编写 UI 分支。
5. 效果成功后 `ConsumeOne()` 把数量减 1；Controller 刷新生命值 UI 和全部格子，最后关闭菜单并清空目标。
6. 数量减到 0 后 `RefreshSlots()` 先清空所有格子，再跳过 `HasItems == false` 的记录，因此第一格图标和数量文字保持为空。

### 验证结果

- Unity 正式程序集与独立 `dotnet build` 均编译通过，最终结果为 0 个警告、0 个错误。
- 场景序列化检查通过：生命值面板、文本引用、人物初始值、苹果效果、Controller 引用和 Use 按钮绑定均已保存。
- 临时 Unity 状态校验连续使用三次苹果并通过：`HP 50 -> 60 -> 70 -> 80`，数量 `3 -> 2 -> 1 -> 空格`，每次使用后菜单目标清空。
- 临时 Setup/Validation 脚本、对应 `.meta`、Editor 目录和结果标记均已删除，不属于正式实现。

### 实现后代码讲解状态

- 当前状态：`delivered`。
- 讲解顺序：人物生命值与显示 -> 定义描述效果 -> 玩家数量消耗 -> Controller 协调 -> 场景按钮入口 -> 连续使用结果。
- 等待用户：检查代码与 Game 视图行为，提出即时问题或确认可以继续。

## 全局学习方法风险记录

### 风险：把常规默认也拆成阻塞式问题

- 用户反馈：生命值溢出这种答案明显的问题不应单独暂停询问。
- AI 判断：反馈正确；“一次只处理一个关键问题”不等于所有边界都必须交给用户逐项选择。
- 当前改进：只有会改变需求、可观察结果、职责边界或高成本返工的决定才暂停；常规低风险行为由 AI 采用保守默认、记录假设并继续。
- 状态：已应用到流程 2.1。

### 风险：某类痛点可能从整体视野中缺失

- 用户担忧：如果某个痛点大类没有在最初整体层面出现，后续逐流程审查可能一直忽略它。
- AI 判断：担忧成立。只依赖已经想到的具体痛点，会形成固定盲区。
- 最终改进：在总体路线阶段维护 `risk-radar.md`，只记录可能相关的大类，不提前设计具体问题或解决方案；每条流程完成后，AI 用这些类别重新扫描实际代码，并允许新增此前未知的类别。
- 状态：已写入 Skill，并在当前项目初始化全局风险雷达。
