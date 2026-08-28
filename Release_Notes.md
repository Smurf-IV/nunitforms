# 2026-08-17 - V3.POC.Alpha - Release Notes

## ⛓️‍💥 **Breaking Change(s)**

- Removed `WindowSpecificSendKeyboardInput`
- Removed `SimpleAPIKeyboardWithEmulation`
- Removed `VirtualKeyCodes`
- Removed `OldSendKeys`
- Removed `SendKeys`


## Current Changes (So Far)

- ✅ Create Fork and add information to the README.md
- ✅ Restructure the layout of the directories within GitHub
- ✅ Fix `SendKeysParser`
- [ ] Modernize to the supported NUnit TFM(s)
    - ✅ net 4.6.2
    - ✅ net 4.8
    - ✅ net6.0 (Will be dropped in NUnit 5)
- [-] Fix Unit Test projects and should be able to run on all TFM(s)
    - ✅ Remove `NMock#` and replace with `FakeItEasy`
    - ✅ net 4.6.2
    - ✅ net 4.8
    - [-] Fix mouse event usage in tests
