# Golden master

`ThirtyDays/stdout.gr` is the canonical output of a 30-day simulation run —
the file every language variant must reproduce exactly. It is checked by
`scripts/golden-master-check.sh` (locally and in CI).

The `.gr` file comes from the upstream kata's TextTest suite; we keep the
fixture and check it with a plain `diff` instead of the TextTest tool.

## House rules

1. **Never hand-edit this file** to make a failing check pass.
2. Behavior changes are agreed first; then the fixture is regenerated
   deliberately, from the program itself:
   - TypeScript: `cd TypeScript && npm run golden > ../texttests/ThirtyDays/stdout.gr`
   - C#: `dotnet run --project csharp/GildedRose -- 30 > texttests/ThirtyDays/stdout.gr`
3. After regenerating, `git diff texttests/` must contain **only the
   lines your change was supposed to move.** Anything else means your
   change leaked into behavior you didn't intend to touch.
