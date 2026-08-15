# Assessment: SDK-style Conversion

## Projects to Convert

| Project | Path | Target Framework | packages.config | Custom Imports | Project Type | Risk |
|---------|------|------------------|----------------|----------------|-------------|------|
| NUnitForms | nunitforms/source/NUnitForms/NUnitForms.csproj | .NET Framework 4.6.2 | Yes | Yes (5+ NuGet imports) | Class Library | Low |
| NUnitForms.Test | nunitforms/source/NUnitForms.Test/NUnitForms.Test.csproj | .NET Framework 4.6.2 | Yes | No | Class Library | Low |
| NUnitForms.Recorder | nunitforms/source/NUnitForms.Recorder/NUnitForms.Recorder.csproj | .NET Framework 4.6.2 | No | No | Class Library | Low |
| NUnitForms.Recorder.Application | nunitforms/source/NUnitForms.Recorder.Application/NUnitForms.Recorder.Application.csproj | .NET Framework 4.8 | No | No | WinForms Application | Low-Medium |

## Already SDK-style (no action needed)
None

## Baseline
- **Solution builds**: No (current errors due to NuGet compatibility)
- **Pending changes**: Committed before scenario start
- **Workspace**: master branch

## Key Findings

### Non-SDK-Style Characteristics Identified
All 4 projects use the legacy MSBuild project format with:
- No `Sdk` attribute on `<Project>` element
- MSBuild 2003 namespace
- Configuration-specific PropertyGroup sections (legacy approach)
- Explicit publish/deployment settings

### Patterns to Migrate
1. **packages.config migration**:
   - NUnitForms: Uses packages.config with NUnit, NUnit3TestAdapter, and Microsoft.Testing packages
   - NUnitForms.Test: Uses packages.config with test frameworks
   - NUnitForms.Recorder: No packages.config (modern PackageReference or native only)
   - NUnitForms.Recorder.Application: No packages.config

2. **Custom MSBuild Imports**:
   - NUnitForms has 5 conditional Import statements for NuGet build props files (typical for packages.config projects)
   - All other projects have no explicit custom imports

3. **Explicit File Includes**:
   - All projects use explicit `<Compile>`, `<None>`, and `<Content>` includes
   - SDK-style will use implicit globbing patterns

4. **Special Project Types**:
   - NUnitForms.Recorder.Application is a WinForms application (OutputType=WinExe)
   - Requires `Microsoft.NET.Sdk.WindowsDesktop` SDK consideration if converted to modern .NET later

### Complexity Assessment
- **Low complexity**: NUnitForms.Test and NUnitForms.Recorder (simple class libraries)
- **Medium complexity**: NUnitForms (has custom NuGet imports) and NUnitForms.Recorder.Application (WinForms)
- **No ASP.NET or heavy custom MSBuild logic** detected
- **Recommended order**: Start with simple libraries, handle test/packages.config projects last

### Build Status Pre-Conversion
Currently fails to build due to:
- NUnit.Analyzers 4.14.0 incompatibility with .NET Framework 4.6.2 (NuGet package resolution issue)
- Missing dependencies in bin directories

This is not a conversion issue but a pre-existing package compatibility problem that may resolve during/after conversion.

## Next Steps
1. Create plan with task breakdown in dependency order
2. Execute conversion using the convert_project_to_sdk_style tool
3. Verify solution builds successfully after each project conversion
4. Commit changes per the "After Each Task" strategy
