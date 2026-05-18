# OrderHub Lab

Welcome to the OrderHub lab. You have **six tasks** to work through, in any order.
Each one is a real piece of work in a realistic codebase — small but not trivial.
Solo, without AI, most would take 15–45 minutes. With Claude as a colleague, you
should clear all six in well under an hour.

## How to work

1. **Open the app** in your IDE of choice (Rider, VS Code, Visual Studio — there's a Compound run config in `.run/` for Rider that starts API + Blazor together).
2. **Pick a task** below. Read it. Then ask Claude to help.
3. Don't just say "fix it" — try framing things the way you'd brief a colleague: *what's wrong, where to look, what good looks like*. Notice how the conversation feels.
4. When the task is done: verify it works end-to-end (build, run, click the relevant page or hit the endpoint).
5. Optional: do the **stretch** for any task that interested you.

A good lab discipline: after each fix, ask Claude *"how would you have caught this in CI?"* The answer is often a test or a check — and writing that is sometimes more valuable than the fix itself.

---

## Task 1 — Onboard Claude to this codebase

**Goal**: produce a `CLAUDE.md` in the repo root that captures what Claude should know about OrderHub.

This is the first thing Claude itself will propose if you let it. Use the `/init` command (or just ask: *"can you read this codebase and write a CLAUDE.md that captures the stack, structure and conventions?"*).

**Why this matters**: every future Claude session in this repo will read that file first. The better it is, the less prompting you need.

**Done when**: the file exists, accurately describes the stack and architecture, and lists the key commands (`dotnet build`, `dotnet test`, how to run API and Blazor).

**Stretch**: have Claude include a "what NOT to do" section based on what it discovered (e.g. Ovako's "no MVC controllers" rule, no business logic in the frontend).

---

## Task 2 — Write tests for the Order domain

**Goal**: the test project `tests/OrderHub.Domain.Tests/` exists in the solution but contains zero test files. Add meaningful tests for the `Order` entity.

**Why this matters**: the `Order` entity is the heart of the app. Right now nothing prevents someone from breaking it.

**Done when**: at least three tests exist that cover non-trivial scenarios, `dotnet test` runs them, and they pass.

**Stretch**: ask Claude *"what would have caught the filter bug from Task 3 if it had existed earlier?"* Then write that test.

---

## Task 3 — Fix the broken status filter

**Goal**: on the Orders page, set the status filter to **Completed**. You'll see orders that are not completed.

The filter on `/api/orders?status=...` is wrong. Find why, fix it, verify.

**Why this matters**: a subtle filter bug like this is the kind of thing that ships to production and confuses customers for weeks. Spot patterns like this and you save a lot of grief.

**Done when**:
- `curl "http://localhost:5101/api/orders?status=Completed"` returns only Completed orders
- The Orders page shows the right rows when each filter value is selected

**Stretch**: write a test in `OrderHub.Api.Tests` that would have caught this bug.

---

## Task 4 — Build the missing Delete flow

**Goal**: on the Orders page, the **Delete** button does nothing useful — it shows a toast that says "endpoint missing". The button shipped before the backend was ready. Fix it end-to-end.

This is intentionally a full-stack task: there's a piece of work in the backend, a piece in the repository, and a piece in the frontend.

**Done when**:
- A `DELETE /api/orders/{id}` endpoint exists and removes the order
- Clicking Delete in the UI actually deletes the order and refreshes the list
- It still works after the page reloads (data really gone, not just hidden)

**Stretch**: add a confirmation dialog. Make the Delete button require explicit confirmation before it fires.

---

## Task 5 — Address a security vulnerability

**Goal**: the build prints `NU1903` warnings about `System.Security.Cryptography.Xml 9.0.0` having known high-severity CVEs. Find where the vulnerable package comes from and fix it.

You didn't pull this package in directly — it arrives through someone else's dependency chain. The right move is to *force* a patched version with an explicit `<PackageReference>`.

**Why this matters**: transitive vulnerabilities are the bulk of real-world supply-chain issues. The pattern of forcing a safe version is reusable in every .NET project.

**Done when**:
- `dotnet build` no longer reports `NU1903` for this package
- The fix is in the right project (the one that ultimately controls the dependency)

**Stretch**: ask Claude to explain *why* the explicit reference at one level overrides the transitive version. The answer is the difference between someone who can copy-paste a fix and someone who can debug the next one alone.

---

## Task 6 — The Products page won't load

**Goal**: open `/products` in the browser. You'll get a yellow exception page instead of the product list. Make the page load.

The fault is **not** on the page itself. It's somewhere in the wiring between backend and frontend. Read the error carefully — it's specific.

**Why this matters**: type drift between two manually maintained DTOs on either side of an API is one of the most common production bugs in full-stack apps. The pattern of *comparing the two declarations* is reusable forever.

**Done when**:
- `/products` loads the full product list
- All eight products render, including the discontinued ones if your fix lets them through (the existing filter hides them — that's not the bug)

**Stretch**: ask Claude *"what's the structural fix so this can't happen again?"* (Hint: shared contract project, OpenAPI-generated clients, type-checked schemas.) Have it lay out the trade-offs.

---

## After the lab

- Did Claude do something that surprised you? Write it down. That's the seed of a skill you'll want to build later.
- Did Claude do something *wrong*? Even better — that's the seed of a CLAUDE.md instruction you'll want to add.
- Which task felt least valuable with AI? Which felt most valuable? The gap between the two is where you'll get the biggest leverage going forward.
