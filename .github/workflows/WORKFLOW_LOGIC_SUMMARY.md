# GitHub Actions Workflow Logic Summary

## Current Workflow Behavior Analysis

### The Problem We Solved:
When a PR is merged to master, GitHub triggers **TWO workflow runs**:
1. **Feature branch workflow** (pull_request event) - ❌ We don't want CD here
2. **Master branch workflow** (push event) - ✅ We want CI+CD here

### Triggers Configured:
```yaml
on:
  push:
    branches: ['**']  # Any branch
  pull_request:
    branches: [master]  # Only PRs to master
    types: [opened, reopened, synchronize, closed]
  workflow_dispatch:  # Manual trigger
```

### Job Execution Logic:

#### **CI Job (build-and-test)**
**Runs when:**
✅ `push` to **any branch** (including master after PR merge)
✅ `pull_request` to **master** with actions: `opened`, `reopened`, `synchronize`
❌ `pull_request` to **master** with action: `closed`
✅ `workflow_dispatch` (manual trigger)

**Condition:**
```yaml
if: |
  github.event_name == 'push' || 
  (github.event_name == 'pull_request' && 
   github.event.action != 'closed') ||
  github.event_name == 'workflow_dispatch'
```

#### **CD Job (build-and-deploy)**
**CRITICAL: Only runs on master branch workflows, never on feature branch workflows**

**Runs when:**
✅ `push` to **master branch** (requires CI to pass) - This covers PR merges
✅ `workflow_dispatch` on **master branch** (requires CI to pass)
❌ `pull_request` events (even when merged) - This prevents duplicate runs

**Condition Logic:**
```bash
# Only deploy from master branch workflows
if push to master AND CI success -> deploy
if workflow_dispatch on master AND CI success -> deploy

# NEVER deploy from pull_request events (prevents duplicate runs)
```

## Workflow Scenarios:

### **Scenario 1: Feature Development (Normal Flow)**
1. **Push to feature/xyz** → ✅ CI runs on feature branch
2. **Open PR to master** → ✅ CI runs on feature branch (validates PR)
3. **Push updates to feature/xyz** → ✅ CI runs on feature branch for each update
4. **Merge PR to master** → 
   - ❌ Feature branch workflow runs (pull_request closed) → ❌ CD skipped
   - ✅ Master branch workflow runs (push to master) → ✅ CI + CD runs

### **Scenario 2: Hotfix (Direct Push)**
1. **Direct push to master** → ✅ CI runs → ✅ CD runs (if CI passes)

### **Scenario 3: Manual Deployment**
1. **Manual trigger on master** → ✅ CI runs → ✅ CD runs (if CI passes)

## Key Benefits:

✅ **No duplicate deployments** - CD only runs once per merge (on master branch workflow)
✅ **No feature branch deployments** - CD never runs from feature branch workflows
✅ **Fast PR feedback** - CI runs immediately on pushes and PR creation
✅ **Safe deployments** - CD only runs for validated code on master branch
✅ **Clear separation** - CI validates PRs, CD deploys from master

## Expected Workflow Runs:

### When PR is opened to master:
- **Feature branch run**: ✅ CI only
- **Master branch run**: ❌ None

### When PR is updated:
- **Feature branch run**: ✅ CI only  
- **Master branch run**: ❌ None

### When PR is merged to master:
- **Feature branch run**: ❌ No jobs (CD conditions not met)
- **Master branch run**: ✅ CI + CD (triggered by push to master)

### When pushing directly to master:
- **Master branch run**: ✅ CI + CD

This configuration ensures only **ONE** deployment per merge and prevents any CD execution from feature branch workflows.
