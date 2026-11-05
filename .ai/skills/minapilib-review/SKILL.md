---
name: minapilib-review
description: Review MinApiLib vertical slices against the project's architecture rules and known library traps. Use when asked to review, audit or check endpoints or a Features folder, and before finishing a task that added or changed slices.
---

# Review slices

Read-only: report findings and don't change code unless asked. Rules and traps are in `AGENT.md`.

Scope: the slices in the current diff, or the folder the user named. Read `Program.cs` and `_Imports.cs` once for context.

## Checklist

### Structure

- [ ] The path is `Features/<Resource>/<VerbNoun>/`, with `Handler.cs`, `Request.cs` and, if there is a body, `Response.cs`.
- [ ] The namespace is `<Root>.Features.<SliceName>`, without the resource folder.
- [ ] The slice uses no type from another slice's namespace, and no new layer folders, dispatcher or base handler.
- [ ] Anything moved to `Features/` is really used by three or more slices.

### Handler

- [ ] It's a `record Handler()` with no constructor dependencies.
- [ ] It uses the `<TRequest>` form returning `IResult`, or always returns 200.
- [ ] `.WithName` equals the slice name and `.WithTags` the resource.
- [ ] **Compare each `return` with each `.Produces`:** nothing is undeclared and nothing is left over.
- [ ] It starts with `ThrowIfCancellationRequested()`, and every async call receives the token.
- [ ] `QUERY` handlers have `.ExcludeFromDescription()` if the app uses Swagger.

### Request

- [ ] It's a `record struct`, with an explicit source on each member.
- [ ] Services use `[FromServices]`.

### Validation

- [ ] A slice that receives input has a validator for `Request`, and calls `.WithValidation()`.
- [ ] It doesn't use the deprecated `MinApiLib.Validation` or data annotations.
- [ ] Each `HashedId` in the body is `required` if it's mandatory, and has a lookup with a 400 path.
- [ ] Paging has a lower bound and a maximum.

### Ids

- [ ] No `int` id or foreign key appears in `Request`, `Response` or `ResponseItem`.
- [ ] Every `href` is built from a `HashedId`.
- [ ] `FindAsync` receives `(int)req.Id`.

### Hypermedia

- [ ] If `.WithHypermedia()` is on the group, every 2xx response with a value has a matching provider, including lists (`HypermediaProvider<IEnumerable<ResponseItem>>`).
- [ ] `.ProducesHypermedia<T>` accompanies `.Produces<T>` when there is a provider.
- [ ] `rel` values match their targets.

### Data

- [ ] Lists project with `Select`, and have a stable `OrderBy` before `Skip`/`Take`.
- [ ] `Include` is present when mapping navigations from a loaded entity.

### Tests

- [ ] The slice has a test class, covering each declared status.

## Report

Group findings by slice, most severe first. For each one give `file:line`, the rule or trap, the concrete failure, and the fix.

- **Bug:** wrong behaviour reachable by a client: skipped validation, a 500, a missing hypermedia provider, a raw id, a missing entity not handled, unchecked paging.
- **Contract:** `Configure` differs from what the handler does.
- **Convention:** structure or naming.

Don't flag duplicated code between slices: it's intentional. If nothing is wrong, say so, and list what you checked.
