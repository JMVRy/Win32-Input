using System.Runtime.InteropServices;

namespace JMVR;

#region Input Operation Classes
/// <summary>
/// Various mouse operations for Windows using P/Invoke<br />
/// Although not all methods are mouse specific, they are adjacently related, so I just decided to put them in here
/// </summary>
public static class MouseOperations
{
    /// <summary>
    /// Flags for which mouse events to do
    /// </summary>
    [Flags]
    public enum MouseEventFlags
    {
        LeftDown = 0x0002,
        LeftUp = 0x0004,
        MiddleDown = 0x0020,
        MiddleUp = 0x0040,
        Move = 0x0001,
        Absolute = 0x8000,
        RightDown = 0x0008,
        RightUp = 0x0010
    }

    /// <summary>
    /// Sets the user's cursor position to the inputted <paramref name="x"/> and <paramref name="y"/> position
    /// </summary>
    /// <param name="x">The x coordinate of the location to move the mouse to</param>
    /// <param name="y">The y coordinate of the location to move the mouse to</param>
    /// <returns>A boolean for whether the cursor was successfully moved to the location</returns>
    [DllImport( "user32.dll", EntryPoint = "SetCursorPos" )]
    [return: MarshalAs( UnmanagedType.Bool )]
    private static extern bool SetCursorPos( int x, int y );

    /// <summary>
    /// Gets the user's current cursor position
    /// </summary>
    /// <param name="lpMousePoint">The coordinate of the current mouse position</param>
    /// <returns>A boolean for whether the current mouse position was successfully obtained</returns>
    [DllImport( "user32.dll" )]
    [return: MarshalAs( UnmanagedType.Bool )]
    private static extern bool GetCursorPos( out MousePoint lpMousePoint );

    /// <summary>
    /// Sends mouse data to Win32 with the selected information, <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-mouse_event"></a>
    /// </summary>
    /// <param name="dwFlags">The <see cref="MouseEventFlags"/> that we wish to input</param>
    /// <param name="dx">The mouse's absolute position along the x-axis or its amount of motion since the last mouse event was generated, depending on the setting of <see cref="MouseEventFlags.Absolute"/>. Absolute data is specified as the mouse's actual x-coordinate; relative data is specified as the number of mickeys moved. A mickey is the amount that a mouse has to move for it to report that it has moved.</param>
    /// <param name="dy">The mouse's absolute position along the y-axis or its amount of motion since the last mouse event was generated, depending on the setting of <see cref="MouseEventFlags.Absolute"/>. Absolute data is specified as the mouse's actual y-coordinate; relative data is specified as the number of mickeys moved.</param>
    /// <param name="dwData"><a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-mouse_event">The full documentation about this can be found here, because dang it's way too long and convoluted to put in short-form</a></param>
    /// <param name="dwExtraInfo">Additional data, usually call GetMessageExtraInfo to set this, but we really don't need to lol</param>
    [DllImport( "user32.dll" )]
    private static extern void mouse_event( int dwFlags, int dx, int dy, int dwData, int dwExtraInfo );

    /// <summary>
    /// Gets the device context for the specified window
    /// </summary>
    /// <param name="hWnd">The window handle to return the device context</param>
    /// <returns>A pointer to the device context</returns>
    [DllImport( "user32.dll" )]
    private static extern IntPtr GetDC( IntPtr hWnd );

    /// <summary>
    /// Releases the device context once we're done with it
    /// </summary>
    /// <param name="hWnd">The window handle that operates under the device context</param>
    /// <param name="hdc">The device context that needs releasing</param>
    /// <returns>A value for whether it released successfully, being 1 for success, 0 for failure</returns>
    [DllImport( "user32.dll" )]
    private static extern int ReleaseDC( IntPtr hWnd, IntPtr hdc );

    /// <summary>
    /// Gets the pixel on screen with the specified coordinates and device context
    /// </summary>
    /// <param name="hdc">The device context handling the window we need the pixel from</param>
    /// <param name="x">The x coordinate to grab the pixel from</param>
    /// <param name="y">The y coordinate to grab the pixel from</param>
    /// <returns>The color of the pixel in the format 0xRRGGBBAA, where R is the red color, G is the green color, B is the blue color, and A is the alpha which is always 0xFF</returns>
    [DllImport( "gdi32.dll", EntryPoint = "GetPixel" )]
    private static extern uint GetPixel( IntPtr hdc, int x, int y );

    /// <summary>
    /// Finds a window by its caption alone
    /// </summary>
    /// <param name="ZeroOnly">Always use <see cref="IntPtr.Zero"/>, because anything else matches a Window Class, which means less likely to retrieve the window</param>
    /// <param name="lpWindowName">The name of the window you want to search for</param>
    /// <returns>A pointer to the window handle you want</returns>
    [DllImport( "user32.dll", EntryPoint = "FindWindow", SetLastError = true )]
    private static extern IntPtr FindWindowByCaption( IntPtr ZeroOnly, string lpWindowName );

    /// <summary>
    /// Finds a window by its caption alone, without the ability to supply a Window Class
    /// </summary>
    /// <param name="lpWindowName">The name of the window you want to search for</param>
    /// <returns>A pointer to the window handle you want</returns>
    public static IntPtr FindWindowByCaption( string caption ) => FindWindowByCaption( IntPtr.Zero, caption );

    /// <summary>
    /// Gets the pixel data on screen with the specified coordinate
    /// </summary>
    /// <param name="mousePoint">A coordinate for the pixel that's needed</param>
    /// <returns>The pixel's data</returns>
    public static Color GetPixelColor( MousePoint mousePoint ) => GetPixelColor( IntPtr.Zero, mousePoint );
    /// <summary>
    /// Gets the pixel data on screen with the specified coordinate
    /// </summary>
    /// <param name="hWnd">The window that uses the pixel whoms data is required</param>
    /// <param name="mousePoint">A coordinate for the pixel that's needed</param>
    /// <returns>The pixel's data</returns>
    public static Color GetPixelColor( IntPtr hWnd, MousePoint mousePoint )
    {
        // Gets the Device Context for the supplied window to get the pixel data
        IntPtr hdc = GetDC( hWnd );
        uint pixel = GetPixel( hdc, mousePoint.X, mousePoint.Y );
        ReleaseDC( hWnd, hdc );

        // Returns the color representation of the data supplied
        Color color = new(
            ( byte )( ( pixel & 0x000000FF ) >> 0 ),
            ( byte )( ( pixel & 0x0000FF00 ) >> 8 ),
            ( byte )( ( pixel & 0x00FF0000 ) >> 16 ),
            ( byte )( ( pixel & 0xFF000000 ) >> 24 )
        );
        return color;
    }

    /// <summary>
    /// Gets the pixel data on screen with the specified coordinate
    /// </summary>
    /// <param name="x">The x coordinate of the pixel</param>
    /// <param name="y">The y coordinate of the pixel</param>
    /// <returns>The pixel's data</returns>
    public static Color GetPixelColor( int x, int y ) => GetPixelColor( IntPtr.Zero, x, y );
    /// <summary>
    /// Gets the pixel data on screen with the specified coordinate
    /// </summary>
    /// <param name="hWnd">The window that uses the pixel whoms data is required</param>
    /// <param name="x">The x coordinate of the pixel</param>
    /// <param name="y">The y coordinate of the pixel</param>
    /// <returns>The pixel's data</returns>
    public static Color GetPixelColor( IntPtr hWnd, int x, int y )
    {
        // Gets the Device Context for the supplied window to get the pixel data
        IntPtr hdc = GetDC( hWnd );
        uint pixel = GetPixel( hdc, x, y );
        ReleaseDC( hWnd, hdc );

        // Returns the color representation of the data supplied
        Color color = new Color(
            ( byte )( ( pixel & 0x000000FF ) >> 0 ),
            ( byte )( ( pixel & 0x0000FF00 ) >> 8 ),
            ( byte )( ( pixel & 0x00FF0000 ) >> 16 ),
            ( byte )( ( pixel & 0xFF000000 ) >> 24 )
        );
        return color;
    }

    /// <summary>
    /// Sets the user's cursor position to the supplied coordinates
    /// </summary>
    /// <param name="x">The x coordinate to move the cursor to</param>
    /// <param name="y">The y coordinate to move the cursor to</param>
    public static void SetCursorPosition( int x, int y )
    {
        // Sets the cursor here, nothing more, nothing less
        SetCursorPos( x, y );
    }

    /// <summary>
    /// Sets the user's cursor position to the supplied coordinates
    /// </summary>
    /// <param name="point">The coordinates to move the cursor to</param>
    public static void SetCursorPosition( MousePoint point )
    {
        // Sets the cursor here, nothing more, nothing less
        SetCursorPos( point.X, point.Y );
    }

    /// <summary>
    /// Gets the user's curent cursor position
    /// </summary>
    /// <returns>The coordinates of the current mouse position</returns>
    public static MousePoint GetCursorPosition()
    {
        MousePoint currentMousePoint;

        // Gets the current mouse position, but if an error occurred we don't want random garbage data, so return (0,0) if an error occurred
        var gotPoint = GetCursorPos( out currentMousePoint );
        if ( !gotPoint )
            currentMousePoint = new MousePoint( 0, 0 );

        return currentMousePoint;
    }

    /// <summary>
    /// Sends the specified mouse input events at the current mouse position
    /// </summary>
    /// <param name="value">Any mouse events we wish to do</param>
    public static void MouseEvent( MouseEventFlags value )
    {
        // Sends the supplied events at the current position
        MousePoint position = GetCursorPosition();

        mouse_event( ( int )value, position.X, position.Y, 0, 0 );
    }

    /// <summary>
    /// A struct to hold the mouse coordinates
    /// </summary>
    [StructLayout( LayoutKind.Sequential )]
    public struct MousePoint
    {
        public int X;
        public int Y;

        public MousePoint( int x, int y )
        {
            X = x;
            Y = y;
        }

        public static bool operator !=( MousePoint left, MousePoint right ) => !( left == right );
        public static bool operator ==( MousePoint left, MousePoint right )
        {
            if ( left.X == right.X && left.Y == right.Y ) return true;
            return false;
        }

        public override readonly bool Equals( object? obj )
        {
            if ( obj == null ) return false;
            return this == ( MousePoint )obj;
        }

        /// <summary>
        /// Simple way to allow printing the MousePoint quickly and easily without needing to access the data multiple times
        /// </summary>
        /// <returns>The data converted to a string in a Tuple-like format</returns>
        public override readonly string ToString()
        {
            return $"(X: {X}, Y: {Y})";
        }

        // ~~It's a good thing that ValueTuple<int, int> has its own GetHashCode function, because idk what quick and non-colliding hash functions exist~~
        // Wait nvm System.HashCode exists?!?!?! well it's a good thing that IntelliCode told me because that's cool
        public override readonly int GetHashCode()
        {
            return HashCode.Combine( X, Y );
        }
    }

    /// <summary>
    /// A representation of color data in byte-sized RGBA format
    /// </summary>
    public struct Color
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color( byte r, byte g, byte b, byte a )
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color() : this( 0, 0, 0, 0 ) { }

        public static bool operator !=( Color left, Color right ) => !( left == right );
        public static bool operator ==( Color left, Color right )
        {
            if ( left.R == right.R && left.G == right.G && left.B == right.B && left.A == right.A ) return true;
            return false;
        }

        public override readonly bool Equals( object? obj )
        {
            if ( obj == null ) return false;
            return this == ( Color )obj;
        }

        /// <summary>
        /// Simple way to allow printing the Color quickly and easily without needing to access the data multiple times
        /// </summary>
        /// <returns>The data converted to a string in a Tuple-like format</returns>
        public override readonly string ToString()
        {
            return $"(R: {R}, G: {G}, B: {B}, A: {A})";
        }

        // Good ol System.HashCode coming in clutch so I don't have any ValueTuple overhead
        public override readonly int GetHashCode()
        {
            return HashCode.Combine( R, G, B, A );
        }
    }
}

/// <summary>
/// A class for handling various Input operations, currently without any semi-related methods
/// </summary>
public static class InputOperations
{
    /// <summary>
    /// Sends the supplied general input
    /// </summary>
    /// <param name="cInputs">The number of inputs being sent</param>
    /// <param name="pInputs">An array of inputs to be sent</param>
    /// <param name="cbSize">The size of the <see cref="INPUT"/> struct</param>
    /// <returns>The number of events successfully sent, 0 means completely blocked (unless UIPI interferes, in which case <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendinput">idk what it'll return because learn.microsoft.com says it'll be different, but doesn't say how it'll be different</a>)</returns>
    [DllImport( "user32.dll", SetLastError = true )]
    private static extern uint SendInput( uint cInputs, INPUT[] pInputs, int cbSize );

    /// <summary>
    /// Gets extra info about the input, kinda required by SendInput
    /// </summary>
    /// <returns>A pointer containing the extra information, which is device specific</returns>
    [DllImport( "user32.dll", SetLastError = true )]
    private static extern IntPtr GetMessageExtraInfo();

    /// <summary>
    /// Gets data from the clipboard
    /// </summary>
    /// <param name="uFormat">The clipboard format, see <see cref="ClipboardFormat"/> for standard formats</param>
    /// <returns>A pointer to the data, use <see cref="Marshal.PtrToStringUni(nint)"/> when converting to <see cref="string"/></returns>
    [DllImport( "user32.dll" )]
    private static extern IntPtr GetClipboardData( uint uFormat );

    /// <summary>
    /// Opens the clipboard for viewing and prevents modification from other applications
    /// </summary>
    /// <param name="hWndNewOwner">A handle to the window to be associated with the open clipboard, use 0 to associate with the current task</param>
    /// <returns>A boolean for success or failure</returns>
    [DllImport( "user32.dll" )]
    private static extern bool OpenClipboard( IntPtr hWndNewOwner );

    /// <summary>
    /// Closes the clipboard, reallowing modification by other clients
    /// </summary>
    /// <returns>A boolean for success or failure</returns>
    [DllImport( "user32.dll" )]
    private static extern bool CloseClipboard();

    /// <summary>
    /// Locks a global memory object
    /// </summary>
    /// <param name="hMem">A handle to the global memory object, obtained using GlobalAlloc or GlobalReAlloc</param>
    /// <returns>A pointer to the first byte of the memory block, or a pointer to 0 for failure</returns>
    [DllImport( "kernel32.dll" )]
    private static extern IntPtr GlobalLock( IntPtr hMem );

    /// <summary>
    /// Unlocks a global memory object
    /// </summary>
    /// <param name="hMem">A handle to the global memory object</param>
    /// <returns>A boolean for whether the global memory object is still locked or not</returns>
    [DllImport( "kernel32.dll" )]
    private static extern bool GlobalUnlock( IntPtr hMem );

    /// <summary>
    /// Gets data from the clipboard, converting between <see cref="ClipboardFormat"/> and <see cref="uint"/> so no casting is needed
    /// </summary>
    /// <param name="format">The <see cref="ClipboardFormat"/> to obtain data as</param>
    /// <returns>A pointer to the clipboard data, use <see cref="Marshal.PtrToStringUni(nint)"/> when converting to <see cref="string"/></returns>
    private static IntPtr GetClipboardData( ClipboardFormat format ) => GetClipboardData( ( uint )format );

    /// <summary>
    /// Gets <see cref="string"/> data from the clipboard
    /// </summary>
    /// <returns>A string containing the last data in the clipboard</returns>
    public static string GetClipboardText()
    {
        if ( !OpenClipboard( 0 ) )
            return string.Empty;

        IntPtr hData = GetClipboardData( ClipboardFormat.UNICODETEXT );
        CloseClipboard();

        if ( hData == IntPtr.Zero )
            return string.Empty;

        string? pszText = Marshal.PtrToStringUni( hData );

        return pszText ?? string.Empty;
    }

    /// <summary>
    /// Sends input events
    /// </summary>
    /// <param name="inputs">An array of inputs to send</param>
    /// <returns>The number of inputs successfully sent, refer to <seealso cref="SendInput(uint, INPUT[], int)"/> for extra details about this number</returns>
    public static uint InputEvent( INPUT[] inputs )
    {
        // Extra processing can be done here, but none is needed as of yet

        // Loops through each input to make sure all extra info is included
        for ( int i = 0; i < inputs.Length; i++ )
        {
            inputs[ i ].U.mi.dwExtraInfo = ( UIntPtr )GetMessageExtraInfo();
            inputs[ i ].U.ki.dwExtraInfo = ( UIntPtr )GetMessageExtraInfo();
        }

        return SendInput( ( uint )inputs.Length, inputs, INPUT.Size );
    }

    public static uint SendUnicode( string s, uint millisecondDelay = 0 ) => SendUnicode( s.ToCharArray(), millisecondDelay );
    public static uint SendUnicode( char c, uint millisecondDelay = 0 ) => SendUnicode( new[] { c }, millisecondDelay );
    public static uint SendUnicode( char[] chars, uint millisecondDelay = 0 )
    {
        INPUT[] inputs = new INPUT[ chars.Length ];

        for ( int i = 0; i < chars.Length; i++ )
        {
            inputs[ i ].type = InputType.INPUT_KEYBOARD;
            inputs[ i ].U.ki.wVk = 0;
            inputs[ i ].U.ki.wScan = ( ScanCodeShort )chars[ i ];
            inputs[ i ].U.ki.dwFlags = KEYEVENTF.UNICODE;
            inputs[ i ].U.ki.time = 0;
        }

        InputEvent( inputs );

        Task delay = Task.CompletedTask;
        if ( millisecondDelay > 0 )
            delay = Task.Delay( ( int )millisecondDelay );

        for ( int i = 0; i < inputs.Length; i++ )
        {
            inputs[ i ].U.ki.dwFlags = KEYEVENTF.KEYUP;
        }

        delay.Wait();

        return InputEvent( inputs );
    }

    /// <summary>
    /// Sends the key with some delay between press and release
    /// </summary>
    /// <param name="vk">The virtual key to send</param>
    /// <param name="millisecondDelay">The delay between press and release</param>
    /// <returns>The number of inputs successfully sent, refer to <seealso cref="SendInput(uint, INPUT[], int)"/> for extra details about this number</returns>
    public static uint SendKeypress( VirtualKey vk, uint millisecondDelay = 0 ) => SendKeypress( new VirtualKey[] { vk }, millisecondDelay );
    /// <summary>
    /// Sends the keys with some delay between press and release.<br />
    /// NOTE: Sends each key supplied instantly, the delay is only for delay between press and release!!!<br />
    /// It may send the inputs in the order of the array, but that's up for Windows to decide
    /// </summary>
    /// <param name="virtualKeys">The virtual keys to send</param>
    /// <param name="millisecondDelay">The delay between press and release</param>
    /// <returns>The number of inputs successfully sent, refer to <seealso cref="SendInput(uint, INPUT[], int)"/> for extra details about this number</returns>
    public static uint SendKeypress( VirtualKey[] virtualKeys, uint millisecondDelay = 0 )
    {
        // The array of inputs that need to be created from the virtualKeys array
        INPUT[] inputs = new INPUT[ virtualKeys.Length ];

        // Loop through each INPUT and make sure they function as expected
        for ( int i = 0; i < inputs.Length; i++ )
        {
            inputs[ i ].type = InputType.INPUT_KEYBOARD;
            inputs[ i ].U.ki.wScan = 0;
            inputs[ i ].U.ki.time = 0;
            inputs[ i ].U.ki.dwFlags = 0;
            inputs[ i ].U.ki.wVk = virtualKeys[ i ];
        }

        // Sends the inputs to InputEvent for handling
        InputEvent( inputs );

        // Delays for some time, unless delay == 0, in which case it won't delay
        Task delay = Task.CompletedTask;
        if ( millisecondDelay > 0 )
            delay = Task.Delay( ( int )millisecondDelay );

        // Resend each input, but instead we release the key
        for ( int i = 0; i < inputs.Length; i++ )
        {
            inputs[ i ].U.ki.dwFlags = KEYEVENTF.KEYUP;
        }

        // Waits for the delay before sending the new events
        delay.Wait();

        return InputEvent( inputs );
    }

    #region Input Data
    public enum ClipboardFormat : uint
    {
        BITMAP = 2,
        DIB = 8,
        DIBVS = 17,
        DIF = 5,
        DSPBITMAP = 0x82,
        DSPENHMETAFILE = 0x8e,
        DSPMETAFILEPICT = 0x83,
        DSPTEXT = 0x81,
        ENHMETAFILE = 14,
        GDIOBJFIRST = 0x300,
        GDIOBJLAST = 0x3ff,
        HDROP = 15,
        LOCALE = 16,
        METAFILEPICT = 3,
        OEMTEXT = 7,
        OWNERDISPLAY = 0x80,
        PALETTE = 9,
        PENDATA = 10,
        PRIVATEFIRST = 0x200,
        PRIVATELAST = 0x2ff,
        RIFF = 11,
        SYLK = 4,
        TEXT = 1,
        TIFF = 6,
        UNICODETEXT = 13,
        WAVE = 12
    }

    /// <summary>
    /// A struct for handling INPUT data, structured in this way specifically so Windows doesn't freak out or do something unexpected
    /// </summary>
    [StructLayout( LayoutKind.Sequential )]
    public struct INPUT
    {
        /// <summary>
        /// The type of input that we want to send, used specifically for keyboard input but can be used for other input as well
        /// </summary>
        public InputType type;
        /// <summary>
        /// A union of the input data, but since C# doesn't have unions, we'll just use a normal struct with a layout of "Explicit"
        /// </summary>
        public InputUnion U;
        /// <summary>
        /// The size of the <see cref="INPUT"/> struct, used for Win32 inputs because it doesn't know the size by default
        /// </summary>
        public static int Size
        {
            // Marshal.SizeOf allows this to be used in safe environments, which is nice because then we don't need to change our Visual Studio project settings
            get { return Marshal.SizeOf( typeof( INPUT ) ); }
        }
    }

    /// <summary>
    /// An enum of different input types, used for <see cref="INPUT"/>
    /// </summary>
    public enum InputType : uint
    {
        /// <summary>
        /// Used for mouse events
        /// </summary>
        INPUT_MOUSE,
        /// <summary>
        /// Used for keyboard events
        /// </summary>
        INPUT_KEYBOARD,
        /// <summary>
        /// Used for hardware events, but that's too complicated and subject to change so we won't use it
        /// </summary>
        INPUT_HARDWARE
    }

    /// <summary>
    /// A union of the different types of inputs we can send, being <see cref="MOUSEINPUT"/>, <see cref="KEYBDINPUT"/>, and <see cref="HARDWAREINPUT"/>
    /// </summary>
    [StructLayout( LayoutKind.Explicit )]
    public struct InputUnion
    {
        [FieldOffset( 0 )]
        public MOUSEINPUT mi;
        [FieldOffset( 0 )]
        public KEYBDINPUT ki;
        [FieldOffset( 0 )]
        public HARDWAREINPUT hi;
    }

    #region Hardware
    /// <summary>
    /// A struct for handling hardware inputs, but it won't be used, so it's only so Win32 has things it expects to exist
    /// </summary>
    [StructLayout( LayoutKind.Sequential )]
    public struct HARDWAREINPUT
    {
        internal int uMsg;
        internal short wParamL;
        internal short wParamR;
    }
    #endregion

    #region Keyboard
    /// <summary>
    /// A struct for handling keyboard inputs
    /// </summary>
    [StructLayout( LayoutKind.Sequential )]
    public struct KEYBDINPUT
    {
        /// <summary>
        /// An enum of shorts of all possible keys that can be sent
        /// </summary>
        internal VirtualKey wVk;
        /// <summary>
        /// An enum of shorts of the different scancodes for certain keys
        /// </summary>
        internal ScanCodeShort wScan;
        /// <summary>
        /// Types of keys to input, being:
        /// <list type="bullet">
        /// <item><see cref="KEYEVENTF.EXTENDEDKEY"/>, where <see cref="wScan"/> contains 2 bytes starting with 0xE0</item>
        /// <item><see cref="KEYEVENTF.KEYUP"/> for releasing a key</item>
        /// <item><see cref="KEYEVENTF.SCANCODE"/> for using <see cref="wScan"/> to provide the key used (ignoring <see cref="wVk"/>)</item>
        /// <item><see cref="KEYEVENTF.UNICODE"/> for using <see cref="wScan"/> to provide the Unicode character to send (<see cref="wVk"/> must be 0)</item>
        /// </list>
        /// </summary>
        internal KEYEVENTF dwFlags;
        /// <summary>
        /// Time stamp for the event<br />
        /// If 0, the system provides the time
        /// </summary>
        internal int time;
        /// <summary>
        /// Additional info, required to use <see cref="GetMessageExtraInfo"/> for it
        /// </summary>
        internal UIntPtr dwExtraInfo;
    }

    /// <summary>
    /// An enum of unsigned integers used for <see cref="KEYBDINPUT.dwFlags"/>
    /// </summary>
    [Flags]
    public enum KEYEVENTF : uint
    {
        EXTENDEDKEY = 0x1,
        KEYUP = 0x2,
        UNICODE = 0x4,
        SCANCODE = 0x8,
    }

    /// <summary>
    /// An enum of shorts containing all scancodes used for <see cref="KEYBDINPUT.wScan"/>
    /// </summary>
    public enum ScanCodeShort : short
    {
        VK_LBUTTON = 0,
        VK_RBUTTON = 0,
        VK_CANCEL = 70,
        VK_MBUTTON = 0,
        VK_XBUTTON1 = 0,
        VK_XBUTTON2 = 0,
        VK_BACK = 14,
        VK_TAB = 15,
        VK_CLEAR = 76,
        VK_RETURN = 28,
        VK_SHIFT = 42,
        VK_CONTROL = 29,
        VK_MENU = 56,
        VK_PAUSE = 0,
        VK_CAPITAL = 58,
        VK_KANA = 0,
        VK_HANGUL = 0,
        VK_JUNJA = 0,
        VK_FINAL = 0,
        VK_HANJA = 0,
        VK_KANJI = 0,
        VK_ESCAPE = 1,
        VK_CONVERT = 0,
        VK_NONCONVERT = 0,
        VK_ACCEPT = 0,
        VK_MODECHANGE = 0,
        VK_SPACE = 57,
        VK_PRIOR = 73,
        VK_NEXT = 81,
        VK_END = 79,
        VK_HOME = 71,
        VK_LEFT = 75,
        VK_UP = 72,
        VK_RIGHT = 77,
        VK_DOWN = 80,
        VK_SELECT = 0,
        VK_PRINT = 0,
        VK_EXECUTE = 0,
        VK_SNAPSHOT = 84,
        VK_INSERT = 82,
        VK_DELETE = 83,
        VK_HELP = 99,
        VK_KEY_0 = 11,
        VK_KEY_1 = 2,
        VK_KEY_2 = 3,
        VK_KEY_3 = 4,
        VK_KEY_4 = 5,
        VK_KEY_5 = 6,
        VK_KEY_6 = 7,
        VK_KEY_7 = 8,
        VK_KEY_8 = 9,
        VK_KEY_9 = 10,
        VK_KEY_A = 30,
        VK_KEY_B = 48,
        VK_KEY_C = 46,
        VK_KEY_D = 32,
        VK_KEY_E = 18,
        VK_KEY_F = 33,
        VK_KEY_G = 34,
        VK_KEY_H = 35,
        VK_KEY_I = 23,
        VK_KEY_J = 36,
        VK_KEY_K = 37,
        VK_KEY_L = 38,
        VK_KEY_M = 50,
        VK_KEY_N = 49,
        VK_KEY_O = 24,
        VK_KEY_P = 25,
        VK_KEY_Q = 16,
        VK_KEY_R = 19,
        VK_KEY_S = 31,
        VK_KEY_T = 20,
        VK_KEY_U = 22,
        VK_KEY_V = 47,
        VK_KEY_W = 17,
        VK_KEY_X = 45,
        VK_KEY_Y = 21,
        VK_KEY_Z = 44,
        VK_LWIN = 91,
        VK_RWIN = 92,
        VK_APPS = 93,
        VK_SLEEP = 95,
        VK_NUMPAD0 = 82,
        VK_NUMPAD1 = 79,
        VK_NUMPAD2 = 80,
        VK_NUMPAD3 = 81,
        VK_NUMPAD4 = 75,
        VK_NUMPAD5 = 76,
        VK_NUMPAD6 = 77,
        VK_NUMPAD7 = 71,
        VK_NUMPAD8 = 72,
        VK_NUMPAD9 = 73,
        VK_MULTIPLY = 55,
        VK_ADD = 78,
        VK_SEPARATOR = 0,
        VK_SUBTRACT = 74,
        VK_DECIMAL = 83,
        VK_DIVIDE = 53,
        VK_F1 = 59,
        VK_F2 = 60,
        VK_F3 = 61,
        VK_F4 = 62,
        VK_F5 = 63,
        VK_F6 = 64,
        VK_F7 = 65,
        VK_F8 = 66,
        VK_F9 = 67,
        VK_F10 = 68,
        VK_F11 = 87,
        VK_F12 = 88,
        VK_F13 = 100,
        VK_F14 = 101,
        VK_F15 = 102,
        VK_F16 = 103,
        VK_F17 = 104,
        VK_F18 = 105,
        VK_F19 = 106,
        VK_F20 = 107,
        VK_F21 = 108,
        VK_F22 = 109,
        VK_F23 = 110,
        VK_F24 = 118,
        VK_NUMLOCK = 69,
        VK_SCROLL = 70,
        VK_LSHIFT = 42,
        VK_RSHIFT = 54,
        VK_LCONTROL = 29,
        VK_RCONTROL = 29,
        VK_LMENU = 56,
        VK_RMENU = 56,
        VK_BROWSER_BACK = 106,
        VK_BROWSER_FORWARD = 105,
        VK_BROWSER_REFRESH = 103,
        VK_BROWSER_STOP = 104,
        VK_BROWSER_SEARCH = 101,
        VK_BROWSER_FAVORITES = 102,
        VK_BROWSER_HOME = 50,
        VK_VOLUME_MUTE = 32,
        VK_VOLUME_DOWN = 46,
        VK_VOLUME_UP = 48,
        VK_MEDIA_NEXT_TRACK = 25,
        VK_MEDIA_PREV_TRACK = 16,
        VK_MEDIA_STOP = 36,
        VK_MEDIA_PLAY_PAUSE = 34,
        VK_LAUNCH_MAIL = 108,
        VK_LAUNCH_MEDIA_SELECT = 109,
        VK_LAUNCH_APP1 = 107,
        VK_LAUNCH_APP2 = 33,
        VK_OEM_1 = 39,
        VK_OEM_PLUS = 13,
        VK_OEM_COMMA = 51,
        VK_OEM_MINUS = 12,
        VK_OEM_PERIOD = 52,
        VK_OEM_2 = 53,
        VK_OEM_3 = 41,
        VK_OEM_4 = 26,
        VK_OEM_5 = 43,
        VK_OEM_6 = 27,
        VK_OEM_7 = 40,
        VK_OEM_8 = 0,
        VK_OEM_102 = 86,
        VK_PROCESSKEY = 0,
        VK_PACKET = 0,
        VK_ATTN = 0,
        VK_CRSEL = 0,
        VK_EXSEL = 0,
        VK_EREOF = 93,
        VK_PLAY = 0,
        VK_ZOOM = 98,
        VK_NONAME = 0,
        VK_PA1 = 0,
        VK_OEM_CLEAR = 0,
    }

    /// <summary>
    /// An enum of shorts containing every key used for <see cref="KEYBDINPUT.wVk"/>
    /// </summary>
    public enum VirtualKey : short
    {
        /// <summary>
        /// Left mouse button
        /// </summary>
        VK_LBUTTON = 0x01,

        /// <summary>
        /// Right mouse button
        /// </summary>
        VK_RBUTTON = 0x02,

        /// <summary>
        /// Control-break processing
        /// </summary>
        VK_CANCEL = 0x03,

        /// <summary>
        /// Middle mouse button
        /// </summary>
        VK_MBUTTON = 0x04,

        /// <summary>
        /// X1 mouse button
        /// </summary>
        VK_XBUTTON1 = 0x05,

        /// <summary>
        /// X2 mouse button
        /// </summary>
        VK_XBUTTON2 = 0x06,

        /// <summary>
        /// BACKSPACE key
        /// </summary>
        VK_BACK = 0x08,

        /// <summary>
        /// TAB key
        /// </summary>
        VK_TAB = 0x09,

        /// <summary>
        /// CLEAR key
        /// </summary>
        VK_CLEAR = 0x0C,

        /// <summary>
        /// ENTER key
        /// </summary>
        VK_RETURN = 0x0D,

        /// <summary>
        /// SHIFT key
        /// </summary>
        VK_SHIFT = 0x10,

        /// <summary>
        /// CTRL key
        /// </summary>
        VK_CONTROL = 0x11,

        /// <summary>
        /// ALT key
        /// </summary>
        VK_MENU = 0x12,

        /// <summary>
        /// PAUSE key
        /// </summary>
        VK_PAUSE = 0x13,

        /// <summary>
        /// CAPS LOCK key
        /// </summary>
        VK_CAPITAL = 0x14,

        /// <summary>
        /// IME Kana mode
        /// </summary>
        VK_KANA = 0x15,

        /// <summary>
        /// IME Hangul mode
        /// </summary>
        VK_HANGUL = 0x15,

        /// <summary>
        /// IME On
        /// </summary>
        VK_IME_ON = 0x16,

        /// <summary>
        /// IME Junja mode
        /// </summary>
        VK_JUNJA = 0x17,

        /// <summary>
        /// IME final mode
        /// </summary>
        VK_FINAL = 0x18,

        /// <summary>
        /// IME Hanja mode
        /// </summary>
        VK_HANJA = 0x19,

        /// <summary>
        /// IME Kanji mode
        /// </summary>
        VK_KANJI = 0x19,

        /// <summary>
        /// IME Off
        /// </summary>
        VK_IME_OFF = 0x1A,

        /// <summary>
        /// ESC key
        /// </summary>
        VK_ESCAPE = 0x1B,

        /// <summary>
        /// IME convert
        /// </summary>
        VK_CONVERT = 0x1C,

        /// <summary>
        /// IME nonconvert
        /// </summary>
        VK_NONCONVERT = 0x1D,

        /// <summary>
        /// IME accept
        /// </summary>
        VK_ACCEPT = 0x1E,

        /// <summary>
        /// IME mode change request
        /// </summary>
        VK_MODECHANGE = 0x1F,

        /// <summary>
        /// SPACEBAR
        /// </summary>
        VK_SPACE = 0x20,

        /// <summary>
        /// PAGE UP key
        /// </summary>
        VK_PRIOR = 0x21,

        /// <summary>
        /// PAGE DOWN key
        /// </summary>
        VK_NEXT = 0x22,

        /// <summary>
        /// END key
        /// </summary>
        VK_END = 0x23,

        /// <summary>
        /// HOME key
        /// </summary>
        VK_HOME = 0x24,

        /// <summary>
        /// LEFT ARROW key
        /// </summary>
        VK_LEFT = 0x25,

        /// <summary>
        /// UP ARROW key
        /// </summary>
        VK_UP = 0x26,

        /// <summary>
        /// RIGHT ARROW key
        /// </summary>
        VK_RIGHT = 0x27,

        /// <summary>
        /// DOWN ARROW key
        /// </summary>
        VK_DOWN = 0x28,

        /// <summary>
        /// SELECT key
        /// </summary>
        VK_SELECT = 0x29,

        /// <summary>
        /// PRINT key
        /// </summary>
        VK_PRINT = 0x2A,

        /// <summary>
        /// EXECUTE key
        /// </summary>
        VK_EXECUTE = 0x2B,

        /// <summary>
        /// PRINT SCREEN key
        /// </summary>
        VK_SNAPSHOT = 0x2C,

        /// <summary>
        /// INS key
        /// </summary>
        VK_INSERT = 0x2D,

        /// <summary>
        /// DEL key
        /// </summary>
        VK_DELETE = 0x2E,

        /// <summary>
        /// HELP key
        /// </summary>
        VK_HELP = 0x2F,

        VK_0 = 0x30,
        VK_1 = 0x31,
        VK_2 = 0x32,
        VK_3 = 0x33,
        VK_4 = 0x34,
        VK_5 = 0x35,
        VK_6 = 0x36,
        VK_7 = 0x37,
        VK_8 = 0x38,
        VK_9 = 0x39,
        VK_A = 0x41,    // Alpha-numeric keys, kinda unnecessary to describe
        VK_B = 0x42,
        VK_C = 0x43,
        VK_D = 0x44,
        VK_E = 0x45,
        VK_F = 0x46,
        VK_G = 0x47,
        VK_H = 0x48,
        VK_I = 0x49,
        VK_J = 0x4A,
        VK_K = 0x4B,
        VK_L = 0x4C,
        VK_M = 0x4D,
        VK_N = 0x4E,
        VK_O = 0x4F,
        VK_P = 0x50,
        VK_Q = 0x51,
        VK_R = 0x52,
        VK_S = 0x53,
        VK_T = 0x54,
        VK_U = 0x55,
        VK_V = 0x56,
        VK_W = 0x57,
        VK_X = 0x58,
        VK_Y = 0x59,
        VK_Z = 0x5A,

        /// <summary>
        /// Left Windows key
        /// </summary>
        VK_LWIN = 0x5B,

        /// <summary>
        /// Right Windows key
        /// </summary>
        VK_RWIN = 0x5C,

        /// <summary>
        /// Applications key
        /// </summary>
        VK_APPS = 0x5D,

        /// <summary>
        /// Computer Sleep key
        /// </summary>
        VK_SLEEP = 0x5F,

        VK_NUMPAD0 = 0x60,
        VK_NUMPAD1 = 0x61,
        VK_NUMPAD2 = 0x62,
        VK_NUMPAD3 = 0x63,  // Numpad keys, kinda unnecessary to describe
        VK_NUMPAD4 = 0x64,
        VK_NUMPAD5 = 0x65,
        VK_NUMPAD6 = 0x66,
        VK_NUMPAD7 = 0x67,
        VK_NUMPAD8 = 0x68,
        VK_NUMPAD9 = 0x69,

        /// <summary>
        /// Numpad Multiply key
        /// </summary>
        VK_MULTIPLY = 0x6A,

        /// <summary>
        /// Numpad Add key
        /// </summary>
        VK_ADD = 0x6B,

        /// <summary>
        /// Numpad Separator key
        /// </summary>
        VK_SEPARATOR = 0x6C,

        /// <summary>
        /// Numpad Subtract key
        /// </summary>
        VK_SUBTRACT = 0x6D,

        /// <summary>
        /// Numpad Decimal key
        /// </summary>
        VK_DECIMAL = 0x6E,

        /// <summary>
        /// Numpad Divide key
        /// </summary>
        VK_DIVIDE = 0x6F,

        VK_F1 = 0x70,
        VK_F2 = 0x71,
        VK_F3 = 0x72,
        VK_F4 = 0x73,
        VK_F5 = 0x74,
        VK_F6 = 0x75,
        VK_F7 = 0x76,
        VK_F8 = 0x77,
        VK_F9 = 0x78,
        VK_F10 = 0x79,
        VK_F11 = 0x7A,  // Function keys, kinda unnecessary to describe
        VK_F12 = 0x7B,
        VK_F13 = 0x7C,
        VK_F14 = 0x7D,
        VK_F15 = 0x7E,
        VK_F16 = 0x7F,
        VK_F17 = 0x80,
        VK_F18 = 0x81,
        VK_F19 = 0x82,
        VK_F20 = 0x83,
        VK_F21 = 0x84,
        VK_F22 = 0x85,
        VK_F23 = 0x86,
        VK_F24 = 0x87,

        /// <summary>
        /// NUM LOCK key
        /// </summary>
        VK_NUMLOCK = 0x90,

        /// <summary>
        /// SCROLL LOCK key
        /// </summary>
        VK_SCROLL = 0x91,

        /// <summary>
        /// Left SHIFT key
        /// </summary>
        VK_LSHIFT = 0xA0,

        /// <summary>
        /// Right SHIFT key
        /// </summary>
        VK_RSHIFT = 0xA1,

        /// <summary>
        /// Left CONTROL key
        /// </summary>
        VK_LCONTROL = 0xA2,

        /// <summary>
        /// Right CONTROL key
        /// </summary>
        VK_RCONTROL = 0xA3,

        /// <summary>
        /// Left ALT key
        /// </summary>
        VK_LMENU = 0xA4,

        /// <summary>
        /// Right ALT key
        /// </summary>
        VK_RMENU = 0xA5,

        /// <summary>
        /// Browser Back key
        /// </summary>
        VK_BROWSER_BACK = 0xA6,

        /// <summary>
        /// Browser Forward key
        /// </summary>
        VK_BROWSER_FORWARD = 0xA7,

        /// <summary>
        /// Browser Refresh key
        /// </summary>
        VK_BROWSER_REFRESH = 0xA8,

        /// <summary>
        /// Browser Stop key
        /// </summary>
        VK_BROWSER_STOP = 0xA9,

        /// <summary>
        /// Browser Search key
        /// </summary>
        VK_BROWSER_SEARCH = 0xAA,

        /// <summary>
        /// Browser Favorites key
        /// </summary>
        VK_BROWSER_FAVOTITES = 0xAB,

        /// <summary>
        /// Browser Start and Home key
        /// </summary>
        VK_BROWSER_HOME = 0xAC,

        /// <summary>
        /// Volume Mute key
        /// </summary>
        VK_VOLUME_MUTE = 0xAD,

        /// <summary>
        /// Volume Down key
        /// </summary>
        VK_VOLUME_DOWN = 0xAE,

        /// <summary>
        /// Volume Up key
        /// </summary>
        VK_VOLUME_UP = 0xAF,

        /// <summary>
        /// Next Track key
        /// </summary>
        VK_MEDIA_NEXT_TRACK = 0xB0,

        /// <summary>
        /// Previous Track key
        /// </summary>
        VK_MEDIA_PREV_TRACK = 0xB1,

        /// <summary>
        /// Stop Media key
        /// </summary>
        VK_MEDIA_STOP = 0xB2,

        /// <summary>
        /// Play/Pause Media key
        /// </summary>
        VK_MEDIA_PLAY_PAUSE = 0xB3,

        /// <summary>
        /// Start Mail key
        /// </summary>
        VK_LAUNCH_MAIL = 0xB4,

        /// <summary>
        /// Start Media key
        /// </summary>
        VK_LAUNCH_MEDIA_SELECT = 0xB5,

        /// <summary>
        /// Start Application 1 key
        /// </summary>
        VK_LAUNCH_APP1 = 0xB6,

        /// <summary>
        /// Start Application 2 key
        /// </summary>
        VK_LAUNCH_APP2 = 0xB7,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ;: key
        /// </summary>
        VK_OEM_1 = 0xBA,

        /// <summary>
        /// For any country/region, the + key
        /// </summary>
        VK_OEM_PLUS = 0xBB,

        /// <summary>
        /// For any country/region, the , key
        /// </summary>
        VK_OEM_COMMA = 0xBC,

        /// <summary>
        /// For any country/region, the - key
        /// </summary>
        VK_OEM_MINUS = 0xBD,

        /// <summary>
        /// For any country/region, the . key
        /// </summary>
        VK_OEM_PERIOD = 0xBE,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the /? key
        /// </summary>
        VK_OEM_2 = 0xBF,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the `~ key
        /// </summary>
        VK_OEM_3 = 0xC0,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the [{ key
        /// </summary>
        VK_OEM_4 = 0xDB,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the \\| key
        /// </summary>
        VK_OEM_5 = 0xDC,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ]} key
        /// </summary>
        VK_OEM_6 = 0xDD,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '\" key
        /// </summary>
        VK_OEM_7 = 0xDE,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
        VK_OEM_8 = 0xDF,

        /// <summary>
        /// The <> keys on the US standard keyboard, or the \\| key on the non-US 102-key keyboard
        /// </summary>
        VK_OEM_102 = 0xE2,

        /// <summary>
        /// IME PROCESS key
        /// </summary>
        VK_PROCESSKEY = 0xE5,

        /// <summary>
        /// Used to pass Unicode characters as if they were keystrokes. The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP
        /// </summary>
        VK_PACKET = 0xE7,

        /// <summary>
        /// Attn key
        /// </summary>
        VK_ATTN = 0xF6,

        /// <summary>
        /// CrSel key
        /// </summary>
        VK_CRSEL = 0xF7,

        /// <summary>
        /// ExSel key
        /// </summary>
        VK_EXSEL = 0xF8,

        /// <summary>
        /// Erase EOF key
        /// </summary>
        VK_EREOF = 0xF9,

        /// <summary>
        /// Play key
        /// </summary>
        VK_PLAY = 0xFA,

        /// <summary>
        /// Zoom key
        /// </summary>
        VK_ZOOM = 0xFB,

        /// <summary>
        /// Reserved
        /// </summary>
        VK_NONAME = 0xFC,

        /// <summary>
        /// PA1 key
        /// </summary>
        VK_PA1 = 0xFD,

        /// <summary>
        /// Clear key
        /// </summary>
        VK_OEM_CLEAR = 0xFE
    }
    #endregion

    #region Mouse
    /// <summary>
    /// A struct for handling mouse inputs, not really used because I already made <see cref="MouseOperations"/> before this, so it's kinda grandfathered in for my own applications
    /// </summary>
    [StructLayout( LayoutKind.Sequential )]
    public struct MOUSEINPUT
    {
        /// <summary>
        /// The absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member. Absolute data is specified as the x coordinate of the mouse; relative data is specified as the number of pixels moved.
        /// </summary>
        internal int dx;
        /// <summary>
        /// The absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member. Absolute data is specified as the y coordinate of the mouse; relative data is specified as the number of pixels moved.
        /// </summary>
        internal int dy;
        /// <summary>
        /// <see cref="dwFlags"/> specific data, such as:
        /// <list type="bullet">
        /// <item>Amount of wheel movement if <see cref="MOUSEEVENTF.WHEEL"/> is specified, one mouse click == 120</item>
        /// <item>Amount of wheel movement if using Windows Vista and <see cref="MOUSEEVENTF.HWHEEL"/> is specified, one mouse click == 120</item>
        /// <item>Which X button should be pressed (being 1 for XBUTTON1, 2 for XBUTTON2) if <see cref="MOUSEEVENTF.XDOWN"/> or <see cref="MOUSEEVENTF.XUP"/> is specified</item>
        /// </list>
        /// Should be 0 if none of the above are used
        /// </summary>
        internal int mouseData;
        /// <summary>
        /// Flags for what event to do, being:
        /// <list type="bullet">
        /// <item><see cref="MOUSEEVENTF.MOVE"/> for mouse movement</item>
        /// <item><see cref="MOUSEEVENTF.LEFTDOWN"/> for holding left click</item>
        /// <item><see cref="MOUSEEVENTF.LEFTUP"/> for releasing left click</item>
        /// <item><see cref="MOUSEEVENTF.RIGHTDOWN"/> for holding right click</item>
        /// <item><see cref="MOUSEEVENTF.RIGHTUP"/> for releasing right click</item>
        /// <item><see cref="MOUSEEVENTF.MIDDLEDOWN"/> for holding middle click</item>
        /// <item><see cref="MOUSEEVENTF.MIDDLEUP"/> for releasing middle click</item>
        /// <item><see cref="MOUSEEVENTF.XDOWN"/> for holding an X button</item>
        /// <item><see cref="MOUSEEVENTF.XUP"/> for releasing an X button</item>
        /// <item><see cref="MOUSEEVENTF.WHEEL"/> for scrolling</item>
        /// <item><see cref="MOUSEEVENTF.HWHEEL"/> for scrolling in Vista</item>
        /// <item><see cref="MOUSEEVENTF.MOVE_NOCOALESCE"/> for mouse movement without coalescing messages</item>
        /// <item><see cref="MOUSEEVENTF.VIRTUALDESK"/> for mapping coordinates to the entire desktop, must be used with <see cref="MOUSEEVENTF.ABSOLUTE"/></item>
        /// <item><see cref="MOUSEEVENTF.ABSOLUTE"/> for normalizing <see cref="dx"/> and <see cref="dy"/> to contain absolute coordinates, where it'd normally be relative coordinates without this set</item>
        /// </list>
        /// </summary>
        internal MOUSEEVENTF dwFlags;
        /// <summary>
        /// Time stamp for the event<br />
        /// If 0, the system provides the time
        /// </summary>
        internal uint time;
        /// <summary>
        /// Additional info, required to use <see cref="GetMessageExtraInfo"/> for it
        /// </summary>
        internal UIntPtr dwExtraInfo;
    }

    /// <summary>
    /// An enum of unsigned integers used for <see cref="MOUSEINPUT.dwFlags"/>
    /// </summary>
    [Flags]
    public enum MOUSEEVENTF : uint
    {
        ABSOLUTE = 0x8000,
        HWHEEL = 0x01000,
        MOVE = 0x0001,
        MOVE_NOCOALESCE = 0x2000,
        LEFTDOWN = 0x0002,
        LEFTUP = 0x0004,
        RIGHTDOWN = 0x0008,
        RIGHTUP = 0x0010,
        MIDDLEDOWN = 0x0020,
        MIDDLEUP = 0x0040,
        VIRTUALDESK = 0x4000,
        WHEEL = 0x0800,
        XDOWN = 0x0080,
        XUP = 0x0100
    }
    #endregion
    #endregion
}
#endregion


