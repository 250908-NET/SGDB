check

# SGDB

Snake Game Database Project 2

# SGDB Project – Git & Code Conventions

## Code & Naming Conventions

Case: Use PascalCase for classes/methods, camelCase for local variables.
Commits: Follow Conventional Commits (https://www.conventionalcommits.org/en/v1.0.0/)

Commit categories:
- `feat`: work on new features
- `fix`: bug fixes
- `refactor`: refactoring production code (not tests, build scripts, or docs)
- `style`: code style fixes
- `test`: tests
- `docs`: documentation (including code documentation)
- `chore`: build and project config, miscellaneous

# Example:

feat: add login endpoint
fix: correct rating calculation
docs: update README

Database name: SGDB
Branches: Use <name/feature> format
Example: vishesh/auth-login, shane/db-migrations

## Branching

### Create a new branch

git checkout -b <name/feature>

### Switch to an existing branch

git checkout <branchname>

## Git Workflow

# Important: Do not commit directly to main.

# Always create a feature branch and work there.

# All changes to main must come through a Pull Request.

### 1. Always sync with main before starting !!!

git checkout main
git pull origin main

### 2. Start work on your branch

git checkout -b <name/feature>

# make changes

git add .
git commit -m "feat: describe your change here"
git push -u origin <name/feature> # only first push

### 3. Keep your branch updated with main

git checkout main
git pull origin main
git checkout <name/feature>
git merge main

## Handling Merge Conflicts

If a conflict occurs during merge:

1. Open the conflicting file(s) and fix the code manually
2. Stage the resolved file(s):
   git add <file>
3. Commit the merge resolution:
   git commit
4. Push your changes:
   git push origin <name/feature>

## Pull Requests

# Open a Pull Request into main when your feature is ready

# At least one reviewer must approve

# Squash & merge to keep history clean

## Quick Reference

# New branch: git checkout -b <name/feature>

# Switch branch: git checkout <branch>

# Sync with main:

git checkout main
git pull origin main
git checkout <branch>
git merge main

# Commit: git commit -m "feat: add X"

# Push: git push origin <branch>
