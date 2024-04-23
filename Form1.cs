using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VisualKey
{
    public partial class Form1 : Form
    {
        private const byte VK_CONTROL = 0x11;
        private const byte VK_SHIFT = 0x10;
        private const byte VK_ALT = 0x12;
        private const byte VK_RMENU = 0xA5; // VK_RMENU est utilisé pour ALT GR

        // Structures nécessaires pour SendInput
        [StructLayout(LayoutKind.Sequential)]
        internal struct INPUT
        {
            public uint Type;
            public InputUnion Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct InputUnion
        {
            [FieldOffset(0)]
            internal MOUSEINPUT Mouse;
            [FieldOffset(0)]
            internal KEYBDINPUT Keyboard;
            [FieldOffset(0)]
            internal HARDWAREINPUT Hardware;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            public int X;
            public int Y;
            public uint MouseData;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBDINPUT
        {
            public ushort Vk;
            public ushort Scan;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HARDWAREINPUT
        {
            public uint Msg;
            public ushort ParamL;
            public ushort ParamH;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        // Constants for the input type
        private const uint INPUT_KEYBOARD = 1;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;

        private bool ctrlPressed = false;
        private bool shiftPressed = false;
        private bool altPressed = false;
        private bool altGrPressed = false;

        Label lblCtrl, lblShift, lblAlt, lblAltGr;

        public Form1()
        {
            InitializeComponent();
            InitializeLabels();
        }

        private void InitializeLabels()
        {
            lblCtrl = new Label { Location = new System.Drawing.Point(10, 40), Size = new System.Drawing.Size(75, 23), Text = "Released", ForeColor = Color.Red };
            lblShift = new Label { Location = new System.Drawing.Point(90, 40), Size = new System.Drawing.Size(75, 23), Text = "Released", ForeColor = Color.Red };
            lblAlt = new Label { Location = new System.Drawing.Point(170, 40), Size = new System.Drawing.Size(75, 23), Text = "Released", ForeColor = Color.Red };
            lblAltGr = new Label { Location = new System.Drawing.Point(250, 40), Size = new System.Drawing.Size(75, 23), Text = "Released", ForeColor = Color.Red };

            Controls.AddRange(new Control[] { lblCtrl, lblShift, lblAlt, lblAltGr });
        }

        private void SendKey(ushort key, bool down)
        {
            INPUT input = new INPUT
            {
                Type = INPUT_KEYBOARD,
                Data = new InputUnion
                {
                    Keyboard = new KEYBDINPUT
                    {
                        Vk = key,
                        Scan = 0,
                        Flags = down ? KEYEVENTF_EXTENDEDKEY : KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP,
                        Time = 0,
                        ExtraInfo = IntPtr.Zero
                    }
                }
            };
            SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ctrlPressed = !ctrlPressed;
            SendKey(VK_CONTROL, ctrlPressed);
            UpdateLabel(lblCtrl, ctrlPressed);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            shiftPressed = !shiftPressed;
            SendKey(VK_SHIFT, shiftPressed);
            UpdateLabel(lblShift, shiftPressed);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            altPressed = !altPressed;
            SendKey(VK_ALT, altPressed);
            UpdateLabel(lblAlt, altPressed);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            altGrPressed = !altGrPressed;
            if (altGrPressed)
            {
                SendKey(VK_CONTROL, true); // CTRL down
                SendKey(VK_RMENU, true);   // ALT GR down
            }
            else
            {
                SendKey(VK_RMENU, false);  // ALT GR up
                SendKey(VK_CONTROL, false); // CTRL up
            }
            UpdateLabel(lblAltGr, altGrPressed);
        }

        private void UpdateLabel(Label label, bool isPressed)
        {
            label.Text = isPressed ? "Pressed" : "Released";
            label.ForeColor = isPressed ? Color.Green : Color.Red;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = checkBox1.Checked;
        }
    }
}
