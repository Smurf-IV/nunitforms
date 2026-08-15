# 04-convert-nunitforms-recorder-application: Convert NUnitForms.Recorder.Application to SDK-style

## Objective
Convert NUnitForms.Recorder.Application from legacy MSBuild format to modern SDK-style format while preserving .NET Framework 4.6.2 target.

## Project Details
- **Full Path**: nunitforms/source/NUnitForms.Recorder.Application/NUnitForms.Recorder.Application.csproj
- **Target Framework**: .NET Framework 4.6.2 (not 4.8 as mentioned in assessment)
- **Project Type**: WinForms Application (OutputType=WinExe)
- **Current Format**: Legacy (ToolsVersion="12.0", MSBuild 2003 namespace)
- **Complexity**: Low
- **Blockers**: None identified (dependencies already converted)

## Research Findings
- No packages.config file (native .NET Framework dependencies only)
- No custom MSBuild imports
- Depends on NUnitForms and NUnitForms.Recorder (both already converted)
- Simple WinForms application with no external package dependencies
- Only legacy project metadata and configuration-specific PropertyGroups

## Conversion Approach
1. Use `convert_project_to_sdk_style` tool
2. Add UseWindowsForms property (automatically detected by SDK)
3. Build the project to verify conversion
4. Verify dependencies resolve correctly

## Done When
- ✅ Project file uses SDK-style format (has `Sdk` attribute)
- ✅ Target framework is still .NET Framework 4.6.2
- ✅ Project builds successfully
- ✅ All build warnings resolved

