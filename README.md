# NUnit.Winforms 🌟

## Who

- V1.x Copyright (c) 2003-2006 by Luke T. Maxon
- V2.x Copyright (c) 2006-2010 by Luke T. Maxon and others
- V3.x Copyright (c) 2026-onwards by Smurf-IV

## Why

- I wanted a way to be able to test standalone controls (Actually for wizard pages),
 but could not find a way to do this without having to perform reflection on the `Focus_Lost` events, because of the way Dev-Ops runners work (i.e. headless)
- I found this (I had used it in 2006 for a couple of projects) and thought it would be a good idea to modernize it and make it work with the latest NUnit TFM(s) and also fix / add some new features.

-----

| Badge 🔄 | Area   |
|--------------------------- |-------------|
| [![.NET](https://github.com/Smurf-IV/NUnitForms/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/Smurf-IV/NUnitForms/actions/workflows/dotnet.yml) | Release build and tests |
| [![.NET Publish Alpha](https://github.com/Smurf-IV/nunitforms/actions/workflows/Publish_Alpha.yml/badge.svg)](https://github.com/Smurf-IV/nunitforms/actions/workflows/Publish_Alpha.yml) | Publish Alpha Nuget package |

-----
<!-- TOC-->
<!-- TOC -->
-----

# Targets 🎯

## Version 3 - POC: Alpha

### ⛓️‍💥Breaking Changes

- Recording application "Does Not Work"
- Remove `VirtualKeyCodes`
- Remove `WindowSpecificSendKeyboardInput`
- Remove `SimpleAPIKeyboardWithEmulation`
- Removed `VirtualKeyCodes`
- Removed `OldSendKeys`
- Removed `SendKeys`
- `Finder<T>().Name("xx")` now returns a `KeyNotFoundException` rather than a generic `Exception` when the element is not found
- Non supported winform controls will be gaurded (For backward compatibility)
    - `ToolBarTester`
    - `MenuItemTester`
    - `ToolBarButtonTester`

### Work in Progress (So Far)
- ✅ Create Fork and add information to the README.md
- ✅ Restructure the layout of the directories within GitHub
- ✅ Fix `SendKeysParser` (Removed `SendKeys`)
- ✅ Copy SourceForge issues etc into Github [Bug] / [Feature] issues types
- ✅ Add focused (Single file) PInvoke and LoadLibrary for Win32 API(s) and use it in the codebase
- [-] Modernize to the supported NUnit TFM(s)
    - ✅ net 4.6.2
    - [-] net 4.8 (_Identify obsoleted types and replace_)
        - [ ] `NumericUpDownTester` #15
    - [-] net6.0 (Will be dropped in NUnit 5)
    - [-] net8.0
    - [-] net10.0
- [-] Fix Unit Test projects and should be able to run on all TFM(s)
    - ✅ Remove `NMock#` and replace with `FakeItEasy`
    - ✅ net 4.6.2
    - ✅ net 4.8
    - ✅ Fix mouse event usage in tests
    - ✅ net6.0 (Will be dropped in NUnit 5)
    - ✅ net8.0
    - ✅ net10.0
      - Adding a smart "readiness" check, to ensure that UI controls are fully initialized before the tests try to interact with them, which was the root cause of the flakiness in .net10
    - [ ] Investigate `Explicit` marked tests, and possible fix them (Also check `Ignore`s)
    - [ ] Change _test_names_ to run in declarative order (i.e. `A010_Stest_name`, `A020_Atest_name`, etc.)
- ✅ Add `ThrowHelper` and use it in the codebase
- [ ] Add missing tests for:
    - [ ] `CheckedListBoxTester`
    - [ ] `ComponentTester`
    - [ ] `ListViewTester`
    - [ ] `PanelTester`
- [ ] Got through the `obsolete`s and fix (i.e. `OpenFileDialogTester.OpenFileDialogTester(string)`)
- [-] Non supported winform controls will be guarded (For backward compatibility)
    - [ ] Fix the recorder project to use the new `ToolStripTester` instead of `ToolBarTester` etc.
    - [ ] Create class and add tests to cover deprecated/removed controls:
        - [ ] `ContextMenuTestForm`
        - [ ] `MainMenuTestForm`
        - [ ] `ToolbarTestForm`
        - [ ] `ToolbarTest`
        - [ ] `ContextMenuTest`
        - [ ] `MainMenuTest`
        - [ ] `MenuItemRecorderTest`
- [-] Nuget package just for the tester.dll
- [-] GitHub build scripts (For the badges etc)
- [ ] Convert SourceForge Discussions / Help into GitHub Discussions (If possible)
- [ ] Other stuff to be added later...


# Version x - ideas 💡

- [ ] Investigate "Recorder Application" and see if it can be fixed (Or if it is even worth fixing)
- [ ] More ideas to be added later, Please suggest... ;-)
