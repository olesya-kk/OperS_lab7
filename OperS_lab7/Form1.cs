using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OperS_lab7
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        // Объявляем делегаты для обработки событий клавиатуры и мыши
        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static HookProc keyboardHookDelegate;
        private static HookProc mouseHookDelegate;

        // Объявляем переменные для хранения хэндлов ловушек
        private static IntPtr keyboardHookHandle = IntPtr.Zero;
        private static IntPtr mouseHookHandle = IntPtr.Zero;

        // Константы для установки глобальных ловушек
        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;

        // Константы для событий клавиатуры
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        // Константы для событий мыши
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_RBUTTONDOWN = 0x0204;

        // Устанавливаем глобальные ловушки
        private void SetHooks()
        {
            keyboardHookDelegate = KeyboardHookCallback;
            keyboardHookHandle = SetHook(WH_KEYBOARD_LL, keyboardHookDelegate);

            mouseHookDelegate = MouseHookCallback;
            mouseHookHandle = SetHook(WH_MOUSE_LL, mouseHookDelegate);
        }

        // Устанавливаем ловушку для указанного типа событий
        private IntPtr SetHook(int hookType, HookProc hookProc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(hookType, hookProc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        // Функция-обработчик событий клавиатуры
        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                listBox1.Items.Add("Нажата клавиша: " + (Keys)vkCode);
            }
            return CallNextHookEx(keyboardHookHandle, nCode, wParam, lParam);
        }

        // Функция-обработчик событий мыши
        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if (wParam == (IntPtr)WM_MOUSEMOVE)
                {
                    listBox1.Items.Add("Мышь дернулась");
                }
                else if (wParam == (IntPtr)WM_LBUTTONDOWN)
                {
                    listBox1.Items.Add("Нажата левая кнопка мыши");
                }
                else if (wParam == (IntPtr)WM_RBUTTONDOWN)
                {
                    listBox1.Items.Add("Нажата правая кнопка мыши");
                }
            }
            return CallNextHookEx(mouseHookHandle, nCode, wParam, lParam);
        }

        // Освобождаем ловушки
        private void ReleaseHooks()
        {
            UnhookWindowsHookEx(keyboardHookHandle);
            UnhookWindowsHookEx(mouseHookHandle);
        }
        // Импорт функций из user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        private void btnStart_Click(object sender, EventArgs e)
        {
            SetHooks();
            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            ReleaseHooks();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }
    }
}
