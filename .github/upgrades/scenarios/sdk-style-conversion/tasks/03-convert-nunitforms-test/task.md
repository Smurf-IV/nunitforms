# 03-convert-nunitforms-test: Convert NUnitForms.Test to SDK-style

## Objective
Convert NUnitForms.Test from legacy MSBuild format to modern SDK-style format while preserving .NET Framework 4.6.2 target.

## Project Details
- **Full Path**: nunitforms/source/NUnitForms.Test/NUnitForms.Test.csproj
- **Target Framework**: .NET Framework 4.6.2 (no change)
- **Project Type**: Class Library (OutputType=Library, test/sample project)
- **Current Format**: Legacy (ToolsVersion="12.0", MSBuild 2003 namespace)
- **Complexity**: Low-Medium
- **Blockers**: None identified (dependencies already converted)

## Research Findings
- packages.config has 2 packages (NUnit 2.6.4, NUnit.Runners 2.6.4, old versions)
- No custom MSBuild imports (simple packages.config pattern)
- Depends on NUnitForms and NUnitForms.Recorder (both already converted)
- Large project file (507 lines with extensive explicit file includes)

## Conversion Approach
1. Use `convert_project_to_sdk_style` tool
2. Build the project to verify conversion
3. Fix any build errors
4. Remove packages.config file
5. Verify dependencies resolve correctly

## Done When
- ✅ Project file uses SDK-style format (has `Sdk` attribute)
- ✅ Target framework is still .NET Framework 4.6.2
- ✅ Project builds successfully
- ✅ No packages.config file exists
- ✅ All build warnings resolved

