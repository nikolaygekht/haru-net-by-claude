# Haru.Net Null Safety Fixes - Batched Plan

## Overview
Total: **514 null safety errors** across 8 error types

## Error Categories & Strategy

| Error Code | Count | Description | Fix Strategy |
|------------|-------|-------------|--------------|
| CS8618 | 192 | Non-nullable field must contain non-null value when exiting constructor | Initialize or make nullable |
| CA1062 | 164 | Validate public method parameters | Add ArgumentNullException.ThrowIfNull() |
| CS8603 | 58 | Possible null reference return | Add checks or make return type nullable |
| CS8625 | 44 | Cannot convert null literal to non-nullable | Make parameter/field nullable (?) |
| CS8600 | 36 | Converting null to non-nullable type | Add null check or make target nullable |
| CS8604 | 10 | Possible null reference argument | Add null check before passing |
| CS8602 | 8 | Dereference of possibly null reference | Add null check or null-conditional (?.) |
| CS8601 | 2 | Possible null reference assignment | Add check or make nullable |

## Batch Plan

### Batch 1: Simple Null Literal Issues - 56 ERRORS ✅ RECOMMENDED START
**Breakdown:** CS8625 (44) + CS8601 (2) + CS8604 (10) = **56 errors**
**Complexity:** ⭐ Low - mostly adding `?` to parameters
**Impact:** Quick wins, reduces error count by 11%

**What we'll do:**
- Find places where code passes `null` but parameter expects non-nullable
- Make parameters/fields nullable with `?` operator
- Examples: `method(null)` where parameter is `string value` → change to `string? value`

**Estimated time:** 30-45 minutes

---

### Batch 2: Null Assignments & Dereferences - 44 ERRORS
**Breakdown:** CS8600 (36) + CS8602 (8) = **44 errors**
**Complexity:** ⭐⭐ Low-Medium - requires analyzing code flow
**Impact:** Reduces error count by 9%

**What we'll do:**
- Add null checks before assignments: `if (value != null) target = value;`
- Use null-conditional operators: `obj?.Property`
- Make target variables nullable where appropriate

**Estimated time:** 45-60 minutes

---

### Batch 3: Null Returns - 58 ERRORS
**Breakdown:** CS8603 (58) = **58 errors**
**Complexity:** ⭐⭐⭐ Medium - need to understand method contracts
**Impact:** Reduces error count by 11%

**What we'll do:**
- Analyze if method should return null (make return type `T?`)
- Add null checks and throw exceptions if null is invalid
- Use null-forgiving operator `!` only if we're certain value is non-null

**Estimated time:** 60-90 minutes

---

### Batch 4: Field Initialization - 192 ERRORS ⚠️ LARGEST BATCH
**Breakdown:** CS8618 (192) = **192 errors** - SPLIT INTO 3 SUB-BATCHES
**Complexity:** ⭐⭐⭐⭐ Medium-High - requires understanding object lifecycle
**Impact:** Reduces error count by 37%

**Sub-batches:**
1. **Batch 4a:** Simple fields (~64 errors) - can be initialized inline or in constructor
2. **Batch 4b:** Lazy-initialized fields (~64 errors) - initialized later, use `= null!`
3. **Batch 4c:** Optional fields (~64 errors) - should be nullable `T?`

**Estimated time:** 2-3 hours (split across multiple sessions)

---

### Batch 5: Parameter Validation - 164 ERRORS ⚠️ SECOND LARGEST
**Breakdown:** CA1062 (164) = **164 errors**
**Complexity:** ⭐⭐ Low - repetitive but tedious
**Impact:** Reduces error count by 32%

**What we'll do:**
- Add `ArgumentNullException.ThrowIfNull(parameter)` at start of public methods
- Pattern: One-liner per parameter
- Example:
  ```csharp
  public void Method(string value)
  {
      ArgumentNullException.ThrowIfNull(value);
      // ... rest of method
  }
  ```

**Estimated time:** 90-120 minutes (can be partially automated)

---

## Recommended Execution Order

1. **Start:** Batch 1 (Simple null literals) - Quick wins, builds momentum
2. **Next:** Batch 2 (Assignments & dereferences) - Still relatively easy
3. **Then:** Batch 3 (Null returns) - Medium complexity
4. **Split:** Batch 4a → 4b → 4c (Field initialization in parts)
5. **Finish:** Batch 5 (Parameter validation) - Tedious but straightforward

## Progress Tracking

- [ ] **Batch 1:** Simple null literals - **56 errors** (CS8625, CS8601, CS8604)
- [ ] **Batch 2:** Assignments & dereferences - **44 errors** (CS8600, CS8602)
- [ ] **Batch 3:** Null returns - **58 errors** (CS8603)
- [ ] **Batch 4a:** Simple field initialization - **~64 errors** (CS8618 part 1/3)
- [ ] **Batch 4b:** Lazy-initialized fields - **~64 errors** (CS8618 part 2/3)
- [ ] **Batch 4c:** Optional nullable fields - **~64 errors** (CS8618 part 3/3)
- [ ] **Batch 5:** Parameter validation - **164 errors** (CA1062)

**TOTAL: 514 errors**

## Notes

- After each batch, rebuild and verify error count decreased
- **MANDATORY AFTER EACH BATCH:**
  1. Repack Haru.Net to ~/.nuget-local (no /tmp!)
  2. Restore PDF.Flow with updated package
  3. Run unit tests to validate changes
  4. Only proceed to next batch if tests pass
- Each batch should be a separate commit if possible
- This iterative validation catches issues early before they compound

## Bird's Eye View

```
Starting point:                             514 errors

Phase 1: Quick Wins (Batches 1-2)          100 errors  → Down to 414 errors
  └─ Batch 1: Simple null literals           56 errors → Down to 458 errors
  └─ Batch 2: Assignments & dereferences     44 errors → Down to 414 errors

Phase 2: Returns (Batch 3)                  58 errors  → Down to 356 errors
  └─ Batch 3: Null returns                   58 errors → Down to 356 errors

Phase 3: Field Init (Batch 4)              192 errors  → Down to 164 errors
  └─ Batch 4a: Simple fields                ~64 errors → Down to 292 errors
  └─ Batch 4b: Lazy-init fields             ~64 errors → Down to 228 errors
  └─ Batch 4c: Optional fields              ~64 errors → Down to 164 errors

Phase 4: Validation (Batch 5)              164 errors  → Down to 0 errors ✅
  └─ Batch 5: Parameter guards              164 errors → DONE! 🎉
```
