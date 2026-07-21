# AENT Gilded Rose

Training fork of the classic **Gilded Rose Refactoring Kata**, trimmed to the
two languages our teams use. You have inherited a small, old, and thoroughly
untidy inventory system. It works. Nobody knows quite why. Your job this
session is to change it *safely*.

Credit: this kata is by [Emily Bache](https://github.com/emilybache/GildedRose-Refactoring-Kata)
(MIT licensed — see [LICENSE](LICENSE)), based on the original by Terry Hughes.

## Layout

| Path | What it is |
|---|---|
| `GildedRoseRequirements.md` | The business spec. Read it *against* the code — they don't always agree. |
| `TypeScript/` | The TypeScript variant (vitest). |
| `csharp/` | The C# variant (.NET 10, xUnit). |
| `texttests/ThirtyDays/stdout.gr` | **The golden master** — the canonical 30-day output. Both languages must reproduce it byte-for-byte. House rules: [texttests/README.md](texttests/README.md). |
| `scripts/golden-master-check.sh` | Runs a language's 30-day simulation and diffs it against the golden master. |

Pick **one** language folder for your work.

## Quickstart

TypeScript:

```bash
cd TypeScript
npm ci
npm test        # unit + approval (snapshot) tests
npm run golden  # print the 30-day simulation
```

C#:

```bash
dotnet test csharp/GildedRose.sln
dotnet run --project csharp/GildedRose -- 30
```

Golden master (either or both languages):

```bash
./scripts/golden-master-check.sh ts
./scripts/golden-master-check.sh csharp
```

## The rules of this repo

1. **The golden master is the law.** Any change to current behavior — even
   behavior that looks like a bug — fails CI. If you believe behavior should
   change, that's a conversation, not a commit.
2. The shipped unit tests are… thin. Before you refactor anything, ask
   yourself what actually protects you. (This is not an accident.)
3. CI runs both languages' tests plus the golden-master check on every PR.
   Green CI is the merge condition.
