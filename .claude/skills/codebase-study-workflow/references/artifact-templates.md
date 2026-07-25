# Codebase Study Artifact Templates

Use these as compact starting points. Keep evidence links precise and avoid copying chat transcripts.

## `index.md`

```markdown
# <项目或范围>代码库学习

- 学习目标：
- 范围：
- 不在范围内：
- 当前入口：[progress.md](progress.md)

## 文档导航
- [系统地图](system-map.md)
- [教学重建路线](pedagogical-roadmap.md)
- [符号索引](symbol-index.md)
- [概念笔记](concept-notes.md)
- [学习记录](learning-log.md)
```

## `system-map.md`

```markdown
# 系统地图

## 系统目标与参与者
| 结论 | 状态 | 证据 |
|---|---|---|

## 启动、构建与测试
| 操作 | 命令或入口 | 状态 | 证据 |
|---|---|---|---|

## 功能模块
| 模块 | 对外职责 | 入口与结果 | 上下游 | 状态 | 证据 |
|---|---|---|---|---|---|

## 数据、状态与外部系统
| 对象 | 所有者 | 读写位置 | 生命周期 | 状态 | 证据 |
|---|---|---|---|---|---|

## 未确认事项
| 问题 | 为什么重要 | 所需证据 |
|---|---|---|
```

状态只使用 `verified`、`inferred`、`unknown`。

## `pedagogical-roadmap.md`

```markdown
# 教学重建路线

> 这是 AI 根据最终代码推导的教学重建顺序，不是已经验证的 Git 历史或作者实际施工顺序。

## 模块：<名称>

| 步骤 | 学习结果 | 代码职责/符号 | 前置 | 结果类型 | 检查方法 | 新概念 | 后续依赖 |
|---|---|---|---|---|---|---|---|
| 1 | | | | runnable/testable/verifiable/inspectable | | | |
```

当一步无法独立运行时，写明必须与哪些职责一起构成完整能力，以及为什么不应继续拆小。

## `modules/<module-slug>.md`

```markdown
# 模块：<名称>

## 职责与边界
- 对外职责：
- 不负责：
- 上游：
- 下游：

## 当前真实运行流
1. 入口：
2. 调用与分派：
3. 数据和状态变化：
4. 外部副作用与失败：
5. 最终结果与验证：

## 代码对应关系
| 运行步骤 | 文件/符号 | 作用 | 证据状态 | 证据 |
|---|---|---|---|---|

## 为什么这样设计

## 改动影响示例

## 回忆检查
- 我能否从入口讲到结果？
- 我能否解释一个关键职责为什么存在？
- 如果修改 <条件>，我预计哪里受影响？

## 仍未确认
```

## Symbol Card For `symbol-index.md`

```markdown
## `<symbol>` — `<file>`
- 类型：function | method | class | type | variable | config
- 责任：
- 输入/输出/可变状态：
- 调用者或读取者：
- 被调用者或写入位置：
- 创建时机与生命周期：
- 所属运行流：
- 修改影响：
- 状态：verified | inferred | unknown
- 证据：
```

## Concept Note For `concept-notes.md`

```markdown
## <语法/API/概念>
- 出现位置：
- 一句话含义：
- 在本项目中为什么使用：
- 最小示例或反例：
- 本项目再次出现的位置：
- 容易混淆的点：
- 回忆问题：
```

## Analogous Lab Correspondence Table

```markdown
# 类比练习对应表

> 这个练习只重现核心机制，不是原项目的复制品，也不代表原项目的历史施工顺序。

| 原项目职责/符号 | 练习中的对应物 | 保留的机制 | 有意省略的复杂度 |
|---|---|---|---|
```

## `learning-log.md`

```markdown
# 学习记录

## <日期> — <模块/运行流>
- 用户原先的理解：
- 已验证的新理解：
- 修正的误解：
- 仍需回看的概念：
- 下次回忆题：
- 下一步：
```
