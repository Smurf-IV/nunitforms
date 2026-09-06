#region Copyright (c) 2003-2005, Luke T. Maxon : 2026-2026 Smurf.IV

/********************************************************************************************************************
'
' Copyright (c) 2003-2005, Luke T. Maxon
' Modernisation 2026-2026 Smurf.IV
' All rights reserved.
' 
' Redistribution and use in source and binary forms, with or without modification, are permitted provided
' that the following conditions are met:
' 
' * Redistributions of source code must retain the above copyright notice, this list of conditions and the
' 	following disclaimer.
' 
' * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and
' 	the following disclaimer in the documentation and/or other materials provided with the distribution.
' 
' * Neither the name of the author nor the names of its contributors may be used to endorse or 
' 	promote products derived from this software without specific prior written permission.
' 
' THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
' WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
' PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
' ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
' LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
' INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
' OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
' IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
'
' ******************************************************************************************************************/

#endregion

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;


namespace NUnit.Extensions.Forms.Util;

internal partial class Win32
{
    /// <summary>User library - provides window management and user interface functions</summary>
    public const string User32 = "user32.dll";

    internal const int BM_CLICK = 0x00F5; //Button
    public const int GENERIC_ALL = 0x10000000;
    internal const int INPUT_MOUSE = 0;
    internal const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
    internal const int MOUSEEVENTF_RIGHTUP = 0x0010;

    internal const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
    internal const int MOUSEEVENTF_MIDDLEUP = 0x0040;
    internal const int MOUSEEVENTF_XDOWN = 0x0080;
    internal const int MOUSEEVENTF_XUP = 0x0100;
    internal const int SPI_GETMOUSEHOVERTIME = 102;
    internal const int XBUTTON1 = 0x1;
    //internal const int XBUTTON1 = 8388608;
    internal const int XBUTTON2 = 0x2; //16777216

    public const uint WM_KEYDOWN = 0x0100;
    public const uint WM_KEYUP = 0x0101;
    public const uint WM_CHAR = 0x0102;
    public const uint WM_SYSKEYDOWN = 0x104;
    public const uint WM_SYSKEYUP = 0x105;
    public const uint WM_LBUTTONDOWN = 0x0201;
    public const uint WM_LBUTTONUP = 0x0202;
    public const uint WM_MOUSEMOVE = 0x0200;
    public const int MK_LBUTTON = 0x0001;
    public const uint WM_MOUSEACTIVATE = 0x0021;
    public const int HTCLIENT = 1;
    public const uint WM_ACTIVATE = 0x0006;
    public const int WA_CLICKACTIVE = 2;
    public const uint WM_SETFOCUS = 0x0007;


    // The signature required for Windows message callbacks
    public delegate IntPtr CBTCallback(int nCode, IntPtr wParam, IntPtr lParam);

    // Native Windows message memory layout 
    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public int ptX;
        public int ptY;
        public uint lPrivate;
    }

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport("user32.dll", EntryPoint="VkKeyScanW", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial short VkKeyScan([MarshalAs(UnmanagedType.U2)] char ch);
#else
    [DllImport(User32)]
    internal static extern short VkKeyScan(char ch);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32, SetLastError = true, EntryPoint = "PostMessageW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
#else
    [DllImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32)]
    internal static partial IntPtr GetDesktopWindow();
#else
    [DllImport(User32)]
    internal static extern IntPtr GetDesktopWindow();
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)] 
    internal static partial bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);
#else
    [DllImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport(User32, EntryPoint = "GetWindowTextW", CharSet = CharSet.Unicode, SetLastError = true)]
    //If the function succeeds, the return value is the length, in characters, of the copied string,
    //not including the terminating null character. If the window has no title bar or text,
    //if the title bar is empty, or if the window or control handle is invalid,
    //the return value is zero. To get extended error information, call GetLastError.
    internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32)]
    internal static partial IntPtr GetDlgItem(IntPtr handleToWindow, int controlId);
#else
    [DllImport(User32)]
    internal static extern IntPtr GetDlgItem(IntPtr handleToWindow, int controlId);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport(User32, CharSet = CharSet.Unicode)]
    internal static extern int GetClassName(IntPtr handleToWindow, StringBuilder className, int maxClassNameLength);

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32, EntryPoint = "SetWindowsHookExW")]
    internal static partial IntPtr SetWindowsHookEx(int code, CBTCallback callbackFunction, IntPtr handleToInstance, uint dwThreadId);
#else
    [DllImport(User32, CharSet = CharSet.Unicode)]
    internal static extern IntPtr SetWindowsHookEx(int code, CBTCallback callbackFunction, IntPtr handleToInstance, uint dwThreadId);
#endif


    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool UnhookWindowsHookEx(IntPtr handleToHook);
#else
    [DllImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnhookWindowsHookEx(IntPtr handleToHook);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32, EntryPoint = "CallNextHookEx")]
    internal static partial IntPtr CallNextHookEx(IntPtr handleToHook, int nCode, IntPtr wParam, IntPtr lParam);
#else
    [DllImport(User32, EntryPoint = "CallNextHookEx")]
    internal static extern IntPtr CallNextHookEx(IntPtr handleToHook, int nCode, IntPtr wParam, IntPtr lParam);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
//#if NET7_0_OR_GREATER
//    [LibraryImport(User32)]
//    internal static partial IntPtr CallNextMSGHookEx(IntPtr handleToHook, int nCode, IntPtr wParam, IntPtr lParam);
//#else
    [DllImport(User32)]
    internal static extern IntPtr CallNextMSGHookEx(IntPtr handleToHook, int nCode, IntPtr wParam, ref Message lParam);
//#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32, EntryPoint = "SendMessageW", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial IntPtr SendMessage(IntPtr handleToWindow, uint message, UIntPtr wParam, IntPtr lParam);
#else
    [DllImport(User32, CharSet = CharSet.Unicode)]
    internal static extern IntPtr SendMessage(IntPtr handleToWindow, uint message, UIntPtr wParam, IntPtr lParam);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32)]
    internal static partial int GetCursorPos(out Point lpWinPoint);
#else
    [DllImport(User32)]
    internal static extern int GetCursorPos(out Point lpWinPoint);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32)]
    internal static partial int SetCursorPos(int x, int y);
#else
    [DllImport(User32)]
    internal static extern int SetCursorPos(int x, int y);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32, EntryPoint = "SystemParametersInfoW")]
    internal static partial int SystemParametersInfo(int uAction, int uParam, out int lpvParam, int fuWinIni);
#else
    [DllImport(User32, CharSet = CharSet.Unicode)]
    internal static extern int SystemParametersInfo(int uAction, int uParam, out int lpvParam, int fuWinIni);
#endif

    /// <summary>
    /// Specifies the function's purpose. If this parameter is TRUE, keyboard and mouse input events are 
    /// blocked. If this parameter is FALSE, keyboard and mouse events are unblocked. Note that only 
    /// the thread that blocked input can successfully unblock input. 
    /// </summary>
    /// <param name="blockIt"></param>
    /// <returns></returns>
    /// <remarks>
    /// When input is blocked, real physical input from the mouse or keyboard will not affect the input queue's 
    /// synchronous key state (reported by GetKeyState and GetKeyboardState), nor will it affect the asynchronous 
    /// key state (reported by GetAsyncKeyState). However, the thread that is blocking input can affect both of 
    /// these key states by calling SendInput. No other thread can do this.
    /// The system will unblock input in the following cases: 
    /// The thread that blocked input unexpectedly exits without calling BlockUserInput with fBlock set to FALSE. 
    /// In this case, the system cleans up properly and re-enables input. Windows 95/98/Me: The system displays the
    /// Close Program/Fault dialog box. This can occur if the thread faults or if the user presses CTRL+ALT+DEL. 
    /// Windows 2000/XP: The user presses CTRL+ALT+DEL or the system invokes the Hard System Error modal message
    /// box (for example, when a program faults or a device fails). 
    /// </remarks>
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool BlockInput([MarshalAs(UnmanagedType.Bool)] bool blockIt);
#else
    [DllImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool BlockInput([MarshalAs(UnmanagedType.Bool)] bool blockIt);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32, EntryPoint = "SendInput", SetLastError = true)]
    internal static partial int SendMouseInput(int cInputs, ref MSINPUT pInputs, int cbSize);
#else
    [DllImport(User32, EntryPoint = "SendInput", SetLastError = true)]
    internal static extern int SendMouseInput(int cInputs, ref MSINPUT pInputs, int cbSize);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32, EntryPoint = "keybd_event", SetLastError = true)]
    internal static partial void KeyBdEvent(byte bVk, byte bScan, int dwFlags, UIntPtr dwExtraInfo);
#else
    [DllImport(User32, EntryPoint = "keybd_event", SetLastError = true)]
    internal static extern void KeyBdEvent(byte bVk, byte bScan, int dwFlags, UIntPtr dwExtraInfo);
#endif

    //// Native structure configuration for Windows security settings
    //[StructLayout(LayoutKind.Sequential)]
    //public struct SECURITY_ATTRIBUTES
    //{
    //    public uint nLength;
    //    public IntPtr lpSecurityDescriptor;
    //    [MarshalAs(UnmanagedType.Bool)]
    //    public bool bInheritHandle;
    //}
//    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
//#if NET7_0_OR_GREATER
//    // StringMarshalling.Unicode ensures the string maps perfectly to a native wide-string.
//    [LibraryImport("user32.dll", EntryPoint = "CreateDesktopW", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
//    internal static unsafe partial IntPtr CreateDesktop(
//        string lpszDesktop,
//        IntPtr lpszDevice,        // Always pass IntPtr.Zero / null
//        IntPtr pDevmode,          // Always pass IntPtr.Zero / null
//        uint dwFlags,             // Desktop control flags
//        uint dwDesiredAccess,     // Access rights (e.g., DESKTOP_CREATEWINDOW)
//        SECURITY_ATTRIBUTES* lpSA // Unsafe pointer avoids complex runtime marshalling rules
//    );
//#else
//    [DllImport(User32, CharSet = CharSet.Unicode, SetLastError = true)]
//    internal static extern IntPtr CreateDesktop(string lpszDesktop, IntPtr lpszDevice, IntPtr pDevmode, int dwFlags,
//        int dwDesiredAccess, 
//        SECURITY_ATTRIBUTES* lpsa);
//#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport(User32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern IntPtr CreateDesktop(string lpszDesktop, IntPtr lpszDevice, IntPtr pDevmode, int dwFlags,
        int dwDesiredAccess, IntPtr lpsa);

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32, SetLastError = true)]
    internal static partial int CloseDesktop(IntPtr hDesktop);
#else
    [DllImport(User32, SetLastError = true)]
    internal static extern int CloseDesktop(IntPtr hDesktop);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32, SetLastError = true)]
    internal static partial IntPtr GetThreadDesktop(uint dwThreadId);
#else
    [DllImport(User32, SetLastError = true)]
    internal static extern IntPtr GetThreadDesktop(uint dwThreadId);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32, SetLastError = true)]
    internal static partial int SetThreadDesktop(IntPtr hDesktop);
#else
    [DllImport(User32, SetLastError = true)]
    internal static extern int SetThreadDesktop(IntPtr hDesktop);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32, SetLastError = true)]
    internal static partial int SwitchDesktop(IntPtr hDesktop);
#else
    [DllImport(User32, SetLastError = true)]
    internal static extern int SwitchDesktop(IntPtr hDesktop);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32, StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetDlgItemText(IntPtr hDlg, int nIDDlgItem, string lpString);
#else
    [DllImport(User32, CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetDlgItemText(IntPtr hDlg, int nIDDlgItem, string lpString);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32, EntryPoint = "SetWindowTextW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetWindowText(IntPtr hwnd, string lpString);
#else
    [DllImport(User32, CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetWindowText(IntPtr hWnd, string lpString);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
//#if NET7_0_OR_GREATER
//    [LibraryImport(User32, StringMarshalling = StringMarshalling.Utf16)]
//    internal static partial int GetDlgItemText(IntPtr hDlg, int nIDDlgItem, StringBuilder lpString, int maxCount);
//#else
    [DllImport(User32, CharSet = CharSet.Unicode)]
    internal static extern int GetDlgItemText(IntPtr hDlg, int nIDDlgItem, StringBuilder lpString, int maxCount);
//#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool IsWindowVisible(IntPtr hDlg);
#else
    [DllImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsWindowVisible(IntPtr hDlg);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport("kernel32")]
    internal static partial uint GetCurrentThreadId();
#else
    [DllImport("kernel32", SetLastError = true)]
    internal static extern uint GetCurrentThreadId();
#endif

    #region Nested type: EnumDelegate

    internal delegate bool EnumDelegate(IntPtr hWnd, int lParam);

    #endregion

    #region Nested type: MOUSEINPUT


    [StructLayout(LayoutKind.Explicit)]
    internal struct MSINPUT
    {
        [FieldOffset(0)] internal /*DWord*/ uint type;  // https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-input

        // https://techcpb.wordpress.com/2010/03/15/new-blog-post-getting-sendinput-pinvoke-in-c/
#if x86 //32bit
        [FieldOffset(4)]
#else   //64bit
        [FieldOffset(8)]
#endif
        public MOUSEINPUT mi;

        public MSINPUT(int mouseEvent)
        {
            type = INPUT_MOUSE;
            mi = new MOUSEINPUT(mouseEvent);
        }

    }


    [StructLayout(LayoutKind.Sequential)]
    internal struct MOUSEINPUT
    {

        // Rest from https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-mouseinput
        internal /*Long*/ Int32 dx;

        internal /*Long*/ Int32 dy;

        internal /*DWord*/ UInt32 mouseData;

        internal /*DWord*/ UInt32 dwFlags;
        internal /*DWord*/ UInt32 time;
        internal /*ULong_Ptr*/IntPtr ExtraInfo;

        ///<summary>
        /// 
        /// </summary>
        /// <remarks>
        ///dx
        ///Specifies the absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member. Absolute data is specified as the x coordinate of the mouse; relative data is specified as the number of pixels moved. 
        ///dy
        ///Specifies the absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member. Absolute data is specified as the y coordinate of the mouse; relative data is specified as the number of pixels moved. 
        ///mouseData
        ///If dwFlags contains MOUSEEVENTF_WHEEL, then mouseData specifies the amount of wheel movement. A positive value indicates that the wheel was rotated forward, away from the user; a negative value indicates that the wheel was rotated backward, toward the user. One wheel click is defined as WHEEL_DELTA, which is 120. 
        ///Windows 2000/XP: IfdwFlags does not contain MOUSEEVENTF_WHEEL, MOUSEEVENTF_XDOWN, or MOUSEEVENTF_XUP, then mouseData should be zero. 
        ///
        ///If dwFlags contains MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP, then mouseData specifies which X buttons were pressed or released. This value may be any combination of the following flags. 
        ///
        ///XBUTTON1
        ///Set if the first X button is pressed or released.
        ///XBUTTON2
        ///Set if the second X button is pressed or released.
        ///dwFlags
        ///A set of bit flags that specify various aspects of mouse motion and button clicks. The bits in this member can be any reasonable combination of the following values. 
        ///The bit flags that specify mouse button status are set to indicate changes in status, not ongoing conditions. For example, if the left mouse button is pressed and held down, MOUSEEVENTF_LEFTDOWN is set when the left button is first pressed, but not for subsequent motions. Similarly, MOUSEEVENTF_LEFTUP is set only when the button is first released. 
        ///
        ///You cannot specify both the MOUSEEVENTF_WHEEL flag and either MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP flags simultaneously in the dwFlags parameter, because they both require use of the mouseData field. 
        ///
        ///MOUSEEVENTF_ABSOLUTE
        ///Specifies that the dx and dy members contain normalized absolute coordinates. If the flag is not set, dxand dy contain relative data (the change in position since the last reported position). This flag can be set, or not set, regardless of what kind of mouse or other pointing device, if any, is connected to the system. For further information about relative mouse motion, see the following Remarks section.
        ///MOUSEEVENTF_MOVE
        ///Specifies that movement occurred.
        ///MOUSEEVENTF_LEFTDOWN
        ///Specifies that the left button was pressed.
        ///MOUSEEVENTF_LEFTUP
        ///Specifies that the left button was released.
        ///MOUSEEVENTF_RIGHTDOWN
        ///Specifies that the right button was pressed.
        ///MOUSEEVENTF_RIGHTUP
        ///Specifies that the right button was released.
        ///MOUSEEVENTF_MIDDLEDOWN
        ///Specifies that the middle button was pressed.
        ///MOUSEEVENTF_MIDDLEUP
        ///Specifies that the middle button was released.
        ///MOUSEEVENTF_VIRTUALDESK
        ///Windows 2000/XP:Maps coordinates to the entire desktop. Must be used with MOUSEEVENTF_ABSOLUTE.
        ///MOUSEEVENTF_WHEEL
        ///Windows NT/2000/XP: Specifies that the wheel was moved, if the mouse has a wheel. The amount of movement is specified in mouseDataÂ� .
        ///MOUSEEVENTF_XDOWN
        ///Windows 2000/XP: Specifies that an X button was pressed.
        ///MOUSEEVENTF_XUP
        ///Windows 2000/XP: Specifies that an X button was released.
        ///time
        ///Time stamp for the event, in milliseconds. If this parameter is 0, the system will provide its own time stamp. 
        ///dwExtraInfo
        ///</remarks>
        public MOUSEINPUT(int mouseEvent)
        {
            dx = 0;
            dy = 0;
            mouseData = 0;
            dwFlags = (uint)mouseEvent;
            time = 0;
            ExtraInfo = IntPtr.Zero;
        }
    }

    #endregion

    #region Nested type: Point

    internal struct Point
    {
        internal int x;

        internal int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    #endregion

    #region Nested type: WindowEnumProc

    internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);

    #endregion

    #region Nested type: WindowMessages

    internal enum WindowMessages : uint
    {
        WM_CLOSE = 0x0010,
        WM_COMMAND = 0x0111
    }

    #endregion

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32)]
    internal static partial IntPtr GetKeyboardLayout(uint dwThreadId);
#else
    [DllImport(User32)]
    internal static extern IntPtr GetKeyboardLayout(uint dwThreadId);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32, StringMarshalling = StringMarshalling.Utf16, EntryPoint = "SendMessageW")]
    internal static partial IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
#else
    [DllImport(User32, CharSet = CharSet.Unicode)]
    internal static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32, EntryPoint = "MapVirtualKeyExW")]
    internal static partial uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);
#else
    [DllImport(User32)]
    internal static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32, SetLastError = true, EntryPoint = "PostThreadMessageW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool PostThreadMessage(uint dwThreadId, uint msg, UIntPtr wParam, IntPtr lParam);
#else
    [DllImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool PostThreadMessage(uint dwThreadId, uint msg, UIntPtr wParam, IntPtr lParam);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32, EntryPoint = "DestroyWindow")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool DestroyWindow(IntPtr hWnd);
#else
    [DllImport(User32, EntryPoint = "DestroyWindow")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DestroyWindow(IntPtr hWnd);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32, EntryPoint = "RegisterWindowMessageW", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    internal static partial int RegisterWindowMessage(string lpstring);
#else
    [DllImport(User32, EntryPoint = "RegisterWindowMessageW", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int RegisterWindowMessage(string lpstring);
#endif


    // Bring a window to the foreground/top of Z-order
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetForegroundWindow(IntPtr hWnd);
#else
    [DllImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetForegroundWindow(IntPtr hWnd);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool BringWindowToTop(IntPtr hWnd);
#else
    [DllImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool BringWindowToTop(IntPtr hWnd);
#endif

    // Release any current mouse capture so subsequent clicks go to the intended control
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ReleaseCapture();
#else
    [DllImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ReleaseCapture();
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32)]
    internal static partial IntPtr GetForegroundWindow();
#else
    [DllImport(User32)]
    internal static extern IntPtr GetForegroundWindow();
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32)]
    internal static partial uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
#else
    [DllImport(User32)]
    internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
#endif

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if NET7_0_OR_GREATER
    [LibraryImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool AttachThreadInput(uint idAttach, uint idAttachTo, [MarshalAs(UnmanagedType.Bool)]bool fAttach);
#else
    [DllImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, [MarshalAs(UnmanagedType.Bool)] bool fAttach);
#endif


}