# Iterative Retest Procedure

## Purpose
After each batch of null safety fixes, we must validate that the changes don't break PDF.Flow functionality.

## Procedure (Run After Each Batch)

### Step 1: Repack Haru.Net
```bash
cd /home/gleb/work/gehtsoft/haru-net-by-claude/cs-src/Haru
dotnet pack --configuration Release -p:TreatWarningsAsErrors=false -p:WarningsAsErrors="" -o ~/.nuget-local
```

**Important:**
- Use `~/.nuget-local` (NOT /tmp!)
- Allow warnings during pack (null safety errors don't block functionality)

### Step 2: Restore PDF.Flow
```bash
cd /home/gleb/work/gehtsoft/PDF.Flow
dotnet restore Gehtsoft.PDFFlow/Gehtsoft.PDFFlow.sln --force
```

**Why --force?** Forces NuGet to pick up the latest package from ~/.nuget-local

### Step 3: Run Unit Tests
```bash
dotnet test Gehtsoft.PDFFlow/Testing/Gehtsoft.PDFFlow.UnitTests/Gehtsoft.PDFFlow.UnitTests.csproj --no-restore
```

**Expected Result:**
- Passed: 1090 tests
- Failed: 0 tests
- Skipped: 21 tests
- Total: 1111 tests

### Step 4: Decision Point

**If tests PASS ✅:**
- Proceed to next batch
- Update progress tracking

**If tests FAIL ❌:**
- **STOP** - Do not proceed to next batch
- Review failed tests
- Identify which changes broke functionality
- Fix the issue
- Re-run Steps 1-3
- Only proceed when tests pass

## Why This Matters

- **Early Detection**: Catches breaking changes immediately
- **Isolation**: Each batch is validated independently
- **Confidence**: Know exactly which batch caused any issues
- **No Cascading Failures**: Don't compound problems across batches

## Batch Checklist

- [x] **After Batch 1** (Simple null literals - 56 errors) ✅ Tests passed!
- [x] **After Batch 2** (Assignments & dereferences - 54 errors) ✅ Tests passed!
- [x] **After Batch 3** (Null returns - 68 errors) ✅ Tests passed!
- [x] **After Batch 4a** (Simple field init - 90 errors) ✅ Included in Batch 4b
- [x] **After Batch 4b** (Lazy-init fields - 78 errors) ✅ Tests passed!
- [x] **After Batch 5** (Parameter validation - 164 errors) ✅ Tests passed!

**🎉 ALL NULL SAFETY BATCHES COMPLETE!**
**Total errors fixed: ~510 null safety and disposable errors**

## Quick Reference

**One-liner to repack and test:**
```bash
cd /home/gleb/work/gehtsoft/haru-net-by-claude/cs-src/Haru && \
dotnet pack --configuration Release -p:TreatWarningsAsErrors=false -p:WarningsAsErrors="" -o ~/.nuget-local && \
cd /home/gleb/work/gehtsoft/PDF.Flow && \
dotnet restore Gehtsoft.PDFFlow/Gehtsoft.PDFFlow.sln --force && \
dotnet test Gehtsoft.PDFFlow/Testing/Gehtsoft.PDFFlow.UnitTests/Gehtsoft.PDFFlow.UnitTests.csproj --no-restore
```
