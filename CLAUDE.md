# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this repository is

A training fork of the Gilded Rose Refactoring Kata (Emily Bache's version, MIT).
It is **not** a product. `UpdateQuality` is deliberately tangled legacy code, and
the shipped test suite is deliberately thin. Both are the exercise, not defects
to report.

The point of the kata: make the code safe to change *without* changing what it
does. Treat "I made this cleaner" as worthless unless the golden master still
matches.

## Commands

```bash
dotnet test csharp/GildedRose.sln            # full suite
dotnet run --project csharp/GildedRose -- 30 # print the 30-day simulation
./scripts/golden-master-check.sh csharp      # diff that simulation vs the golden master
```

Single test or group, by substring on the fully-qualified name:

```bash
dotnet test csharp/GildedRose.sln --filter "FullyQualifiedName~BackstagePasses"
```

`golden-master-check.sh` is bash — on Windows run it from Git Bash or WSL, not
PowerShell. It accepts `csharp` or `all` (both do the same thing now that this
is a single-language repo) and exits 2 on an unknown target.

## The golden master is a hard constraint

`texttests/ThirtyDays/stdout.gr` is the canonical 30-day output. CI diffs the
program's stdout against it byte-for-byte on every PR, and green CI is the merge
condition.

Any change to observable behavior fails CI — **including changes that fix what
looks like a bug**. Per `README.md`, that is a conversation, not a commit. Never
regenerate `stdout.gr` to make a failing check pass; the regeneration procedure
in `texttests/README.md` applies only after a behavior change has been agreed.

If you break it, revert the step that broke it rather than debugging forward.
Refactor in small increments and re-run the check after each.

## Changing behavior: the Conjured precedent

The Conjured rule is implemented — 2/day before the sell-by date, 4/day after,
floored at 0. How it got there is the worked example of how a behavior change
happens in this repo:

1. The spec demanded the rule, but `Conjured Mana Cake` was already in the
   30-day simulation, so its old 1/day decay was baked into the golden master.
   Implementing the spec necessarily failed CI.
2. The conflict was raised and agreed **before** any fixture was touched.
3. Only then was `stdout.gr` regenerated from the program, and the resulting
   `git diff texttests/` contained only the four `Conjured Mana Cake` lines.

Follow that sequence for any further behavior change. The failure mode to avoid
is regenerating the fixture first and treating the resulting green check as
proof of anything.

Conjured items are matched on the `"Conjured"` name prefix rather than the exact
product name, on the reading that the spec describes a supplier category.

## Off-limits

`Item.cs` — the `Item` class and the `Items` property must not be modified
(`GildedRoseRequirements.md`). All work goes in `GildedRose.cs` or new files.

## Test layout and what each layer is for

Three distinct kinds of test live in `csharp/GildedRoseTests/`, and they fail
very differently:

- `GildedRoseTest.KeepsItemNameUnchanged` — the kata's shipped stub. Asserts only
  that the name survives; passes against an empty method body. Provides no
  protection. Do not mistake it for coverage.
- `CharacterizationTest` — one named test per rule (decay either side of the
  sell-by date, the 0/50 bounds, Aged Brie, Sulfuras, backstage thresholds at
  11/10/6/5/1/0, Conjured, and a null `Name`). These localize a break to a
  specific rule. Most were derived by *running* the implementation rather than
  from the spec; the Conjured ones assert the spec, because that rule was
  deliberately changed.
- `ApprovalTest.ThirtyDays` — VerifyXunit snapshot of the whole 30-day stdout,
  stored in `ApprovalTest.ThirtyDays.verified.txt`. Broad but coarse: it reports
  a 373-line diff and leaves you to bisect.

When adding characterization tests, derive expected values by running the current
code. Where code and spec disagree, the code wins until the change is agreed —
and say so in the test name.

Neither the suite nor the golden master exercises unusual inputs: nothing
constructs an item with a null name, a quality outside [0, 50], or an unknown
category. A null-name crash once passed all 25 tests and the golden master
before a differential sweep caught it. If you change dispatch logic, check those
inputs explicitly rather than trusting a green run.

For a change that is supposed to preserve behavior, the strongest available
check is a differential probe: paste the previous algorithm into a throwaway
test as a reference oracle, run both over a grid of names x `sellIn` x
`quality`, and assert they agree. That is machine-checked proof across thousands
of cases rather than the handful anyone thinks to write. Delete it afterwards —
it embeds a stale copy of the implementation.

`ApprovalTest.ThirtyDays.verified.txt` and `stdout.gr` are separate files with
the same content: the former is VerifyXunit's per-language snapshot, the latter
is the fixture the CI script checks. Both must stay in sync.

## Environment

- .NET 10 SDK required (`net10.0`, `LangVersion 14.0`); CI pins `10.0.x`. An
  older SDK fails at restore with NETSDK1045.
- `core.autocrlf=true` is the Windows default and there is no `.gitattributes`.
  Since the golden master is compared byte-for-byte on a Linux runner, a fixture
  regenerated on Windows can pick up CRLF and fail CI with no visible diff. If a
  golden-master check fails but the diff looks empty, suspect line endings.

## History

The kata originally shipped C# and TypeScript variants here; the TypeScript tree
was removed. References to `npm`, `vitest`, or a `ts` target in older commits or
docs are stale.
