# Plan: SDK-style Project Conversion

## Overview
Convert all 4 NUnitForms projects from legacy MSBuild format to modern SDK-style format. No target framework changes will be made during this conversion.

## Conversion Order
Projects are ordered bottom-up (dependencies first) to ensure each project can build successfully after conversion:

1. **NUnitForms.Recorder** (leaf dependency, no custom imports)
2. **NUnitForms** (depends on nothing, but has packages.config + custom imports)
3. **NUnitForms.Test** (depends on NUnitForms and NUnitForms.Recorder)
4. **NUnitForms.Recorder.Application** (depends on NUnitForms.Recorder)

## Tasks

### Task 1: Convert NUnitForms.Recorder to SDK-style
- **Project**: nunitforms/source/NUnitForms.Recorder/NUnitForms.Recorder.csproj
- **Target Framework**: .NET Framework 4.6.2 (no change)
- **Complexity**: Low
- **Complications**: None identified
- **Validation**: Build after conversion

### Task 2: Convert NUnitForms to SDK-style
- **Project**: nunitforms/source/NUnitForms/NUnitForms.csproj
- **Target Framework**: .NET Framework 4.6.2 (no change)
- **Complexity**: Medium
- **Complications**: 
  - Has packages.config file that must be migrated to PackageReference
  - Has 5+ conditional MSBuild imports from NuGet packages
  - Multiple NuGet package references (NUnit, test adapters, testing platform)
- **Validation**: Build after conversion, verify packages.config is removed

### Task 3: Convert NUnitForms.Test to SDK-style
- **Project**: nunitforms/source/NUnitForms.Test/NUnitForms.Test.csproj
- **Target Framework**: .NET Framework 4.6.2 (no change)
- **Complexity**: Low-Medium
- **Complications**: 
  - Has packages.config file that must be migrated
  - Depends on NUnitForms and NUnitForms.Recorder
  - Can only be converted after dependencies are converted
- **Validation**: Build after conversion, verify packages.config is removed

### Task 4: Convert NUnitForms.Recorder.Application to SDK-style
- **Project**: nunitforms/source/NUnitForms.Recorder.Application/NUnitForms.Recorder.Application.csproj
- **Target Framework**: .NET Framework 4.8 (no change)
- **Complexity**: Low
- **Complications**: 
  - WinForms application (OutputType=WinExe)
  - Depends on NUnitForms and NUnitForms.Recorder
  - Can only be converted after dependencies are converted
- **Validation**: Build after conversion

## Validation Criteria
- ✅ All projects target correct .NET Framework version
- ✅ Solution builds successfully (zero errors)
- ✅ All packages.config files removed
- ✅ No explicit MSBuild tool imports remain (e.g., Microsoft.CSharp.targets)
- ✅ Project files use modern SDK-style format with `Sdk` attribute

## Risks & Considerations
- **NUnit.Analyzers incompatibility**: Current build fails due to v4.14.0 not supporting .NET Framework 4.6.2. May need to pin to compatible version (e.g., 4.1.x) during conversion.
- **WinForms project**: NUnitForms.Recorder.Application is a WinForms app; ensure no issues arise from implicit `UseWindowsForms` property that may be needed.
- **No rollback target**: Because we're staying on master branch, ensure each conversion succeeds before proceeding to next.
