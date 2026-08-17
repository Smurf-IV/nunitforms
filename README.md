# NUnit.Winforms 🌟

## Who

- V1.x Copyright (c) 2003-2006 by Luke T. Maxon
- V2.x Copyright (c) 2006-2010 by Luke T. Maxon and others
- V3.x Copyright (c) 2026-onwards by Smurf-IV

## Why

- I wanted a way to be able to test standalone controls (Actually for wizard pages),
 but could not find a way to do this withut having to perform reflection on the `Focus_Lost` events, because of the way Dev-Ops runners work (i.e. headless)
- I found this (I had used it on 2006 for a couple of projects) and thought it would be a good idea to modernize it and make it work with the latest NUnit TFM(s) and also fix / add some new features.

-----

| Badge 🔄 | Area   |
|--------------------------- |-------------|

-----
<!-- TOC-->
<!-- TOC -->
-----

# Targets 🎯

## Version 3 - POC: Alpha

- ✅ Create Fork and add information to the README.md
- [-] Modernize to the supported NUnit TFM(s)
    - ✅ net 4.6.2
    - ✅ net 4.8
    - ✅ net6.0 (Will be dropped in NUnit 5)
    - [ ] net8.0
    - [ ] net10.0
- [-] Fix Unit Test projects and should be able to run on all TFM(s)
    - [-] net 4.6.2
    - [-] net 4.8
    - [ ] Remove `NMock3` and replace with `Moq` or `NSubstitute` (or any other mocking framework)
    - [ ] net6.0 (Will be dropped in NUnit 5)
    - [ ] net8.0
    - [ ] net10.0
- [-] Non supported winform controls will be gaurded (For backward compatibility)
    - [ ] Fix the recorder project to use the new `ToolStripTester` instead of `ToolBarTester` etc.
    - [ ] Replace tests:
        - [ ] `ContextMenuTestForm`
        - [ ] `MainMenuTestForm`
        - [ ] `ToolbarTestForm`
        - [ ] `ToolbarTest`
        - [ ] `ContextMenuTest`
        - [ ] `MainMenuTest`
        - [ ] `MenuItemRecorderTest`
- [ ] Fix `SendKeysParser` (Or remove)
- [ ] Add `ThrowHelper` and use it in the codebase
- [ ] Fix mouse event usage in tests
- [ ] Other stuff to be added later...

### ⛓️‍💥Breaking Changes

- Remove `VirtualKeyCodes`
- Remove `WindowSpecificSendKeyboardInput`
- Remove `SimpleAPIKeyboardWithEmulation`
- Non supported winform controls will be gaurded (For backward compatibility)
    - `ToolBarTester` -> Replaced by `ToolStripTester`
    - `MenuItemTester`
    - `ToolBarButtonTester`

# Version x - ideas 💡

- [ ] More ideas to be added later, Please suggest... ;-)
