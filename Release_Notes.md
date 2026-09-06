# 2026-08-17 - V3.POC.Alpha - Release Notes

## ⛓️‍💥 **Breaking Change(s)**

- Recording application "Does Not Work"
- Removed `WindowSpecificSendKeyboardInput`
- Removed `SimpleAPIKeyboardWithEmulation`
- Removed `VirtualKeyCodes`
- Removed `OldSendKeys`
- Removed `SendKeys`
- `Finder<T>().Name("xx")` now returns a `KeyNotFoundException` rather than a generic `Exception` when the element is not found


## Current Changes (So Far)

- ✅ Create Fork and add information to the README.md
- ✅ Restructure the layout of the directories within GitHub
- ✅ Fix `SendKeysParser`
- ✅ Copy SourceForge issues etc into Github [Bug] / [Feature] issues types
- [ ] Modernize to the supported NUnit TFM(s)
    - ✅ net 4.6.2
    - ✅ net 4.8
    - [-] net6.0 (Will be dropped in NUnit 5)
- [-] Fix Unit Test projects and should be able to run on all TFM(s)
    - ✅ Remove `NMock#` and replace with `FakeItEasy`
    - ✅ net 4.6.2
    - ✅ net 4.8
    - ✅ Fix mouse event usage in tests
    - ✅ net6.0 (Will be dropped in NUnit 5)
    - ✅ net8.0
    - ✅ net10.0
- ✅ Add `ThrowHelper` and use it in the codebase
