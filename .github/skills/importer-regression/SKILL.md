---
name: importer-regression
description: 'Use when changing importer mapping, synchronizer, merger, ExternalReference matching, deleteExtra or AllowDeletions behavior, deduplication, or re-import/idempotency semantics. Adds the right regression tests and validates importer stability.'
---

# Importer Regression Skill

Use this skill when a change touches importer behavior that can affect existing models after re-import.

## Goals

- Preserve stable behavior for repeated imports.
- Protect ID stability and existing model matches.
- Catch regressions in matching precedence, deduplication, and deletion paths.

## Workflow

1. Identify the true matching boundary.

- Determine whether the change lives in task validation, source parsing, persistable generation, shared synchronization, or importer-specific merge logic.
- Find the existing matching key or precedence path, especially `ExternalReference`, schema-aware lookup, or manual-association preservation.

2. Inspect the nearest regression tests first.

- Look for existing tests that already cover idempotency, duplicate handling, or deletion behavior in the touched importer.
- Reuse existing `TestData` factories and scenario helpers.

3. Make the smallest behavior change that fixes the root cause.

- Prefer changing the actual matching or merge rule rather than adding post-processing cleanups.
- Avoid broad refactors unless they are necessary to preserve correctness.

4. Add targeted regression coverage.

- Include at least one focused test for the changed scenario.
- If structure changes materially, add or update a snapshot test.
- For merge changes, consider both first import and re-import behavior.

5. Validate like an importer maintainer.

- Run the nearest importer tests.
- If the change is broad, run a focused cross-section across affected importer suites.

## Checklist

- Does a re-import preserve IDs where it should?
- Are `ExternalReference` values still stable and unique?
- Does deletion behavior still differ correctly between enabled and disabled modes?
- Are manually modeled items preserved when tests expect them to be?
- Did you add a regression test in the closest existing test suite?