# GitHub Copilot Instructions

You are reviewing code in this repository.  
Follow the guidelines below strictly when generating code, reviewing PRs, or answering questions.

---

## 1. General Principles

- Follow Clean Code principles.
- Prefer readability over cleverness.
- Keep methods small and focused.
- Avoid unnecessary abstractions.
- Do not introduce new dependencies without strong justification.
- Match the existing project architecture and conventions.

---

## 2. Code Quality

When reviewing or generating code:

- Ensure clear naming (no vague names like data, obj, temp).
- Remove duplicated logic.
- Avoid deeply nested conditionals.
- Validate null handling.
- Ensure proper exception handling.
- Do not swallow exceptions silently.
- Prefer explicit over implicit behavior.

---

## 3. Security Requirements

Always check for:

- Input validation and sanitization.
- Injection risks (SQL, command, etc.).
- Proper authentication and authorization checks.
- No hardcoded secrets.
- No sensitive data logging.
- Proper use of encryption and hashing APIs.

If a security issue is detected:
- Explain why it is risky.
- Suggest a safer alternative.

---

## 4. Performance

Review for:

- Inefficient database queries.
- N+1 query issues.
- Missing indexes (if applicable).
- Unnecessary allocations.
- Blocking calls inside async methods.
- Improper async/await usage.

In .NET:
- Avoid `.Result` or `.Wait()` on async tasks.
- Use async all the way down.
- Prefer `IQueryable` over `IEnumerable` when querying DB.

---

## 5. Testing

When adding or reviewing features:

- Ensure tests cover edge cases.
- Check for missing negative test cases.
- Avoid overly mocked tests when not needed.
- Ensure test names describe behavior clearly.
- Follow AAA pattern (Arrange-Act-Assert).

If a feature lacks tests:
- Suggest required test scenarios.

---

## 6. Documentation

- Public APIs must have XML documentation.
- Update README for new features.
- Document breaking changes clearly.
- Ensure configuration changes are documented.

---

## 7. Pull Request Review Style

When reviewing PRs:

- Use inline comments for specific code issues.
- Provide top-level summary for:
  - Overall code quality
  - Security concerns
  - Performance impact
  - Test coverage
- Provide constructive feedback.
- Highlight good patterns when appropriate.

Do NOT:
- Rewrite entire files unnecessarily.
- Suggest stylistic changes unless meaningful.
- Block PRs without explanation.

---

## 8. .NET Specific Standards

- Use dependency injection properly.
- Avoid static mutable state.
- Prefer `record` for immutable DTOs.
- Validate async flow and cancellation tokens.
- Use `ConfigureAwait(false)` in library code.
- Ensure EF Core queries are optimized.
- Avoid client-side evaluation in EF Core.

---

## 9. Database Guidelines

- No SELECT *
- Always specify columns when possible.
- Check indexes for large datasets.
- Avoid loading large collections into memory.
- Prefer pagination for list APIs.

---

## 10. Logging

- Use structured logging.
- No logging of secrets.
- Log meaningful context (correlation id if available).
- Avoid excessive logging in hot paths.

---

## 11. What To Do When Unsure

If the repository pattern is unclear:
- Analyze similar existing files.
- Follow established conventions.
- Do not introduce new architecture patterns.

---

## Goal

Produce:
- Secure code
- Maintainable code
- Performant code
- Well-tested code
- Well-documented code
