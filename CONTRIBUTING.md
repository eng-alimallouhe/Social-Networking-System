# CONTRIBUTING GUIDELINES

## 1. Introduction
Thank you for your interest in contributing to the Syrian Developers Network!  
To maintain a clean, scalable, and professional codebase, contributors must follow this guide and respect all architecture and coding conventions.

This document merges the **Backend / Frontend Team Conventions** with the project’s overall contribution workflow.

---

## 2. Project Principles
All contributions must follow:
- **Clean Architecture**
- **Repository Pattern**
- **Specification Pattern**
- **Separation of Concerns**
- **Team Coding Conventions (Backend + Frontend)**

---

# 🧱 3. Coding Conventions

## 🔹 Backend Team Convention (C# / .NET)

### Naming Conventions
- **Properties:** PascalCase (start with uppercase). Example: `FirstName`.
- **Private variables:** start with underscore `_variableName`.
- **Interfaces:** start with **I** → `IUserService`.
- **Methods:** PascalCase → `GetUserById()`.
- **Classes:** PascalCase → `UserRepository`.

### Code Style
- **Indentation:** 4 spaces, never tabs.
- **Braces:** Open braces on **next line**.
- **Exception Handling:** Use `try-catch`; log exceptions via the Logger service.

---

## 🔹 Frontend Team Convention (Angular / TS)

### Naming Conventions
- **Components:** PascalCase → `UserProfileComponent`.  
  File name: `user-profile.component.ts`.
- **Services:** PascalCase, end with `Service`.  
  File: `auth.service.ts`.
- **Variables:** camelCase.
- **Constants:** UPPER_SNAKE_CASE.
- **Interfaces:** Start with **I** → `IUser`.
- **Files/Folders:** kebab-case → `user-profile/`.
- **Observables:** name starts with `$` → `userData$`.

### Code Style
- **Indentation:** 2 spaces.
- **Always use braces**, even for single-line conditions.
- Ensure consistent spacing around operators.

---

# 🔹 General Conventions

## Branching Strategy
Always create a new branch for each task.

### Naming Rules
- **Feature:** `feature/feature-name`
- **Bug fix:** `bugfix/bug-description`
- **Hotfix:** `hotfix/urgent-fix`
- **New Module / Part:** `build/part-name`

## Commenting Code
- Use `//` for comments (one-line or multi-line).
- Public methods must include **XML documentation**.
- Keep comments clear and meaningful.

---

# 🪲 4. Reporting Bugs
Before opening an issue, ensure:
- Precise description of the bug
- Steps to reproduce
- Expected vs actual behavior
- Include environment details (OS, .NET version, Browser, etc.)

---

# 💡 5. Suggesting Features
Feature requests should include:
- The problem the feature solves
- Suggested approach
- Alternatives considered

---

# 🔧 6. Submitting Code Contributions
### Steps:
1. Fork the repository.
2. Create a branch following the naming rules:
```bash
git checkout -b feature/your-feature-name
```
3. Write clean code following **all conventions above**.
4. Commit changes with clear messages:
```bash
git commit -m "Add: feature description"
```
5. Push changes:
```bash
git push origin feature/your-feature-name
```
6. Open a Pull Request and describe:
- What you changed
- Why you changed it
- Any breaking changes

### PR Requirements
- Code builds successfully
- Follows Clean Architecture boundaries
- No direct use of `DbContext` in controllers or handlers
- Complex queries implemented using **Specifications**
- Frontend code follows Angular style guide
