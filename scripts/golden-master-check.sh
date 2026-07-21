#!/usr/bin/env bash
# Golden-master check: runs the 30-day Gilded Rose simulation and diffs the
# output against the canonical fixture in texttests/ThirtyDays/stdout.gr.
#
# Usage: ./scripts/golden-master-check.sh [csharp|all]   (default: all)
#
# Exit code 0 = output matches the golden master; nonzero = behavior changed.
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
FIXTURE="$ROOT/texttests/ThirtyDays/stdout.gr"
TARGET="${1:-all}"
FAIL=0

normalize() { tr -d '\r'; }

check() {
  local label="$1"; shift
  local out="$(mktemp)" diffout="$(mktemp)"
  if ! "$@" | normalize > "$out"; then
    echo "FAIL [$label]: program did not run"; FAIL=1; rm -f "$out" "$diffout"; return
  fi
  if diff -u <(normalize < "$FIXTURE") "$out" > "$diffout"; then
    echo "OK   [$label]: output matches golden master"
  else
    echo "FAIL [$label]: output differs from golden master:"
    head -40 "$diffout"
    FAIL=1
  fi
  rm -f "$out" "$diffout"
}

run_csharp() { dotnet run --project "$ROOT/csharp/GildedRose" --configuration Release -- 30; }

case "$TARGET" in
  csharp|all) check "C#" run_csharp ;;
  *)          echo "Usage: $0 [csharp|all]"; exit 2 ;;
esac

exit $FAIL
