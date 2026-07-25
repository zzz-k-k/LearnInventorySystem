---
name: feature-study-workflow
description: Use for learning-centered development of a new or substantially changed feature from requirements through implementation. The user proposes requirements, behavior flows, and a high-level construction roadmap before AI review; then one observable flow is built, validated, audited, and understood at a time. Trigger for systematic feature construction, architecture learning during development, implementation progress queries, design-drift analysis, and learning retrospectives. Do not use for primarily studying an existing codebase without a feature goal; use codebase-study-workflow instead.
---

# Feature Study Workflow

Guide a large feature as a user-led construction and learning workflow. Let the user own the initial product reasoning and construction order. Act as reviewer, coach, implementer, and evidence-based post-flow auditor.

## Scope Boundary

Use this skill when the main goal is to create or substantially change behavior from requirements. Existing code may be inspected as the implementation baseline.

Use `$codebase-study-workflow` when the main goal is to understand an existing repository, reconstruct how it works, trace symbols, or derive a teaching order without a defined feature change.

If both goals exist, first use the codebase skill for the relevant module, then start this workflow for the approved feature. Do not run both state machines in the same document directory.

## Core Contract

- Ask the user to propose the requirements, behavior flow, and high-level construction roadmap before AI designs the implementation.
- Review the roadmap for coverage, dependency order, and observable outcomes. Decompose only the currently selected stage.
- Build one observable user or system flow at a time, not all models, repositories, or controllers as separate horizontal batches.
- Let architecture evolve from working flows and real constraints. Treat future architecture as provisional.
- After each flow works, inspect actual behavior and code for evidence-based pain points the user may not recognize. Present one pain point at a time.
- Review user reasoning in this order: correctness, requirement completeness, simplicity, clarity and naming, then justified extensibility.
- Do not replace a workable user idea with a hidden preferred answer. Explain consequences, ask one focused question, and escalate hints gradually.
- Adapt coaching to the user's knowledge. Explain every AI-supplied framework API, responsibility, and technical choice.
- Track implementation completion and user understanding separately.
- Keep generated learning documents in Chinese unless the user requests another language. Record decisions, evidence, and changed reasoning; never copy chat transcripts.

## Pause Rules

Stop and wait:

- after each requirements, user-flow, or roadmap review;
- when the user must propose or revise a construction plan;
- before implementing an accepted plan;
- after one flow is implemented and validated;
- after the post-implementation code walkthrough, until the user has asked their immediate questions or confirms they can continue;
- after presenting one unresolved post-flow pain point that needs user reasoning, approval, or action;
- before starting the next flow;
- before the user-filled retrospective.

A status request reports state only. It does not authorize implementation.

Do not spend a turn only acknowledging that the user completed the previous step. When the user explicitly reports a check passed, a pain point resolved, or an approved action completed, record it and immediately advance to the next required audit, understanding check, or planning boundary in the same response. Stop only when the next step again requires user input or action.

## Project Structure Rules

Before adding files, inspect the repository's existing module boundaries, naming, tests, examples, docs, and generated-file conventions.

- Place behavior beside the module that owns it; do not scatter tutorial files across the source tree.
- Do not create vague dumping grounds such as `utils`, `helpers`, `common`, or `misc` unless the repository already defines a narrow responsibility for them.
- Follow existing test placement. Keep fixtures and resources with the subsystem that owns them.
- State why every new production file belongs in its directory.
- After each completed flow, audit whether files still form a coherent responsibility boundary. Move files only when the flow supplies concrete evidence that ownership changed.
- Store workflow documents under one stable directory rather than next to arbitrary source files.

## Artifact Set

Use `docs/feature-study/<feature-slug>/`, or map these roles into an existing documentation convention:

- `progress.md`: current phase, flow, blockers, implementation and understanding state, next action.
- `requirements-clarification.md`: original request, confirmed scope, assumptions, exclusions, and risks.
- `requirements-user-flow.md`: user-authored flows, AI review, edges, and acceptance criteria.
- `architecture-implementation.md`: user roadmap, coverage matrix, actual architecture, and evolution log.
- `risk-radar.md`: broad review categories and evidence status, not speculative solutions.
- `construction-learning-log.md`: per-flow plan, code mapping, validation, pain audits, and understanding checks.
- `bug-repair-log.md`: reproduction, cause, fix, validation, and lesson.
- `implementation-summary.md`: final behavior, architecture, drift, validation, and remaining risks.
- `learning-retrospective.md`: user explanation followed by AI critique.
- `reusable-patterns.md`: reusable signals, checks, snippets, and future prompts.

Create an artifact when its phase begins. Do not create empty files merely to complete the list.

## Progress Model

Use these phases:

1. `requirements-clarification`
2. `requirements-user-flow`
3. `construction-roadmap-review`
4. `single-flow-construction`
5. `post-flow-pain-audit`
6. `roadmap-repeat-and-completion-check`
7. `implementation-summary-and-drift-analysis`
8. `learning-retrospective-and-critique`
9. `reusable-pattern-capture`
10. `complete`

Maintain at least:

```markdown
# Feature Study Progress

- Feature:
- Current phase:
- Status: not-started | in-progress | waiting-for-user | blocked | complete
- Current roadmap stage: <index>/<total>: <title>
- Current observable flow: <stage>.<flow>: <title>
- Flow status: planning | approved | implementing | validating | explaining | auditing | awaiting-understanding | complete
- Implementation status: not-started | in-progress | complete
- Pain audit status: not-started | in-progress | complete
- Understanding status: not-started | in-progress | mastered
- Implemented flows:
- Fully completed flows:
- Completed roadmap stages:
- Open questions:
- Blockers:
- Next action:
- Last updated:
```

A fully completed flow has validated behavior, no unresolved required pain audit, and a passed understanding check. Update progress whenever state starts, pauses, completes, or blocks.

## Progress Queries

When asked for status:

1. Locate the most recently modified active `docs/feature-study/*/progress.md`.
2. Read it and the active artifact when needed.
3. Report phase, roadmap stage, current flow, implementation/audit/understanding states, blockers, waiting party, and next action.
4. Do not continue work unless explicitly asked.

If multiple workflows are plausible, list them. If none exists, say so and offer to start one.

## Phase 1: Clarify Requirements

Create the progress file and `requirements-clarification.md`. Preserve the user's wording before normalization. Ask at most five high-impact questions about outcomes, boundaries, users, data, and constraints. Do not design code yet.

Record:

- original requirement;
- confirmed behavior and non-goals;
- ambiguity and user corrections;
- assumptions and risks.

Stop for confirmation.

## Phase 2: Review Behavior Flows

Ask the user to describe each main flow as `入口 -> 操作 -> 系统反馈 -> 结果`. Preserve their proposal, then review requirement coverage, empty states, permissions, failures, retries, and acceptance criteria. Do not design architecture.

Stop for user revision or confirmation.

## Phase 3: Review The User's Construction Roadmap

Inspect the repository only enough to establish its conventions and constraints. Ask the user for a high-level construction order with an observable result per stage. Exact files, classes, APIs, patterns, and every internal step are not required.

Review only:

- requirement and acceptance-criteria coverage;
- dependency order;
- whether stages end in observable or inspectable outcomes;
- whether unresolved choices are visible;
- whether the stage size can later be split into coherent flows.

Do not silently replace the roadmap. After acceptance, initialize a category-level risk radar covering relevant areas such as behavior, state/data consistency, failure recovery, permissions/security, performance/scale, usability/accessibility, tests/observability, and maintainability. A radar category does not authorize speculative code.

Stop before implementation planning.

## Phase 4: Build One Observable Flow

Select one flow from the current roadmap stage. Ask how the user would construct it from the current repository state. Accept plain behavior language.

Choose one coaching branch:

- `user-technical-plan`: review the user's responsibilities, data flow, and APIs.
- `user-behavior-plan`: translate behavior steps into the smallest technical plan, explain the mapping, and request confirmation.
- `user-needs-scaffold`: give one question or hint, then a small structural example; provide a minimal plan only when still blocked or explicitly requested.

Record a flow card containing its requirement, observable outcome, user plan, review, accepted plan, code mapping, validation, and understanding state.

Before editing, state the outcome, accepted steps, and minimum files/responsibilities that must change. Implement only this flow. If it unexpectedly crosses unrelated responsibilities, pause and re-split it.

Validate behavior and relevant regressions. Then enter an explicit post-implementation code walkthrough before any pain audit. Explain the implementation in the same order as the user's behavior plan:

1. restate the problem each code unit solves;
2. distinguish existing, modified, and new files;
3. trace callers, callees, data, and state changes at runtime;
4. explain new syntax and framework APIs the user did not supply;
5. connect each normalized responsibility back to the user's proposal;
6. invite immediate questions and correct misunderstandings without turning the walkthrough into a memorization test.

Record the walkthrough and its open questions. Stop until the user confirms they understand enough to continue. This is a construction explanation, not the final mastery check. Update only architecture that now exists.

## Phase 5: Audit One Evidence-Based Pain At A Time

Begin only after the post-implementation code walkthrough has been delivered and the user confirms they understand enough to continue. Inspect the completed flow, its validation, the relevant radar categories, and the next accepted dependency. A pain point must be supported by current evidence:

- incorrect, unsafe, inconsistent, or lossy behavior;
- visible duplication, mixed ownership, unclear state, or fragile coupling;
- a blocker for the next accepted stage;
- an unmet confirmed requirement or edge case.

Classify it as `must-fix`, `next-step-needed`, or `optional-later`. Present exactly one highest-priority problem by scenario and consequence without naming a predetermined solution. Ask the user to propose a response and stop.

If accepted, confirm the smallest fix, request implementation approval, implement, and rerun the flow checks. If deferred, record why. Keep only one unresolved pain active at a time. Once the user confirms that pain is resolved or deferred, record the result and immediately present the next required pain; if none remains, proceed directly to the understanding check.

Then ask the user to explain the behavior, changed reasoning, runtime/data flow, responsibility boundaries, and one change-impact example. This final mastery check consolidates the initial walkthrough plus any code changes caused by the pain audit. Mark understanding mastered only when requirements, construction, code, and impact are connected.

## Phase 6: Repeat Or Finish

After a flow is implemented, audited, and mastered:

1. update coverage, actual architecture, risk review, and design decisions;
2. determine whether the current roadmap stage is complete;
3. ask the user to choose the next flow; or
4. ask them to confirm implementation completion.

Never automatically begin the next flow.

## Bugs And Requirement Changes

For a bug, record symptom, reproduction, impact, cause, repair, validation, and the earlier decision that enabled it. When requirements or architecture change, update the authoritative artifact instead of leaving the truth only in chat.

## Phase 7: Summarize Implementation And Drift

Create `implementation-summary.md` covering final behavior, actual construction order, architecture, differences from the original roadmap, evidence that caused each change, validation, and remaining risks. Stop before retrospective.

## Phase 8: User Retrospective And Critique

Ask the user to fill `learning-retrospective.md` first. It should explain requirements, original and actual construction order, runtime/data flow, key pain points and solutions, necessary versus premature abstractions, what they would do differently, and remaining confusion.

Then identify accurate reasoning, vague claims, memorized conclusions without causes, and three transfer questions. Stop.

## Phase 9: Capture Reusable Patterns

Create `reusable-patterns.md` with the reusable construction sequence, justified abstractions, pain signals, validation checks, concise patterns, and a future prompt. Mark progress complete.
