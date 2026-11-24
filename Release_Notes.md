# 2026-12-## V4.Beta

## ⛓️‍💥Breaking Changes

- Move to NUnit 4.6.1
- Change namepace from `NUnit.Extensions.Forms` to `NUnit.Winforms`

### Work done

- ✅ Go through the `obsolete`s and fix (i.e. `OpenFileDialogTester.OpenFileDialogTester(string)`)
    - Legacy modal handlers now match a form’s caption when its name does not match, without changing existing name-based behavior. 
- ✅ Add missing tests for:
- ✅ Create class and add tests to cover deprecated/removed controls:
    - `ContainerControlTester`
    - `ContextMenuStripTester`
    - `DataGridTester`
    - `DataGridViewTester`
    - `DateTimePickerTester`
    - `DomainUpDownTester`
    - `FlowLayoutPanelTester`
    - `GroupBoxTester`
    - `HScrollBarTester`
    - `MaskedTextBoxTester`
    - `MenuStripTester`
    - `MonthCalendarTester`
    - `NumericUpDownTester`
    - `PictureBoxTester`
    - `PrintPreviewControlTester`
    - `PrgressBarTester`
    - `PropertyGridTester`
    - `PrgressBarTester`
    - `PropertyGridTester`
    - `ScrollableControlTester`
    - `ScrollBarTester`
    - `SplitContainerTester`
    - `StatusBarTester`
    - `StatusStripTester`
    - `TableLayoutPanelTester`
    - `TabPageTester`
    - `ToolStripContainerTester`
    - `ToolStripContentPanelTester`
    - `ToolStripDropDownTester`
    - `ToolStripLabelTester`
    - `ToolStripPanelTester`
    - `ToolStripProgressBarTester`
    - `ToolStripSeparatorTester`
    - `ToolStripStatusLabelTester`
    - `ToolStripTester`
    - `TrackBarTester`
    - `UserControlTester`
    - `VScrollBarTester`
    - `WebBrowserTester`

# 2026-11-16 - V3.Beta

## ⛓️‍💥 **Breaking Change(s)**

- Recording application "Does Not Work"
- Removed `WindowSpecificSendKeyboardInput`
- Removed `SimpleAPIKeyboardWithEmulation`
- Removed `VirtualKeyCodes`
- Removed `OldSendKeys`
- Removed `SendKeys`
- `Finder<T>().Name("xx")` now returns a `KeyNotFoundException` rather than a generic `Exception` when the element is not found

### Work done

- ✅ Create Fork and add information to the README.md
- ✅ Restructure the layout of the directories within GitHub
- ✅ Fix `SendKeysParser`
- ✅ Copy SourceForge issues etc into Github [Bug] / [Feature] issues types
- ✅ Add focused (Single file) PInvoke and LoadLibrary for Win32 API(s) and use it in the codebase
- ✅ Modernize to the supported NUnit TFM(s)
    - ✅ net 4.6.2
    - ✅ net 4.8
    - ✅ net6.0 (Will be dropped in NUnit 5)
    - ✅ net8.0
    - ✅ net10.0
    - ✅ net11.0
- ✅ Fix Unit Test projects and should be able to run on all TFM(s)
    - ✅ Remove `NMock#` and replace with `FakeItEasy`
    - ✅ net 4.6.2
    - ✅ net 4.8
    - ✅ Fix mouse event usage in tests
    - ✅ net6.0 (Will be dropped in NUnit 5)
    - ✅ net8.0
    - ✅ net10.0
    - ✅ net11.0
        - Adding a smart "readiness" check, to ensure that UI controls are fully initialized before the tests try to interact with them, which was the root cause of the flakiness in .net10
- ✅ Add `ThrowHelper` and use it in the codebase
- ✅ Nuget package just for the tester.dll
