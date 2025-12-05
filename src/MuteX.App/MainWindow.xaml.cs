using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using MuteX.App.Core;
using MuteX.App.UI;
using NAudio.Wave;

namespace MuteX.App
{
    public partial class MainWindow : Window
    {
        private HotKeyManager? _hotKeyManager;
        private AudioController? _audioController;
        private SettingsManager? _settingsManager;
        private StartupManager? _startupManager;
        private TrayIcon? _trayIcon;

        // 🔒 --- Mutex para evitar abrir más de una instancia ---
        private static Mutex? _appMutex;

        // 🔊 --- Reproductor para sonidos ---
        private WaveOutEvent? _soundPlayer;
        private AudioFileReader? _audioReader;

        public MainWindow()
        {
            // --- SINGLE INSTANCE CHECK ---
            bool createdNew = false;
            _appMutex = new Mutex(true, "MuteX_SingleInstance_Mutex", out createdNew);

            if (!createdNew)
            {
                MessageBox.Show("MuteX ya está ejecutándose.", "MuteX", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Shutdown();
                return;
            }

            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyMica();
            ApplyWindowCorners();

            _audioController = new AudioController();
            _settingsManager = new SettingsManager();
            _startupManager = new StartupManager();

            var handle = new WindowInteropHelper(this).Handle;
            _hotKeyManager = new HotKeyManager(handle);

            RegisterHotKey();

            _trayIcon = new TrayIcon();

            _trayIcon.ToggleRequested += ToggleMute;

            _trayIcon.OpenWindowRequested += () =>
            {
                this.Show();
                this.WindowState = WindowState.Normal;
                this.Activate();
            };

            _trayIcon.ExitRequested += () => Application.Current.Shutdown();

            UpdateMicStatus();

            CheckStartup.IsChecked = _settingsManager.Settings.StartWithWindows;
            CheckStartup_Checked(null, null);

            this.Hide();
            _trayIcon.ShowStartupNotification();
        }

        // ----------------- MICA -----------------
        private void ApplyMica()
        {
            try
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
                int micaValue = 3;
                DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref micaValue, sizeof(int));
            }
            catch { }
        }

        private void ApplyWindowCorners()
        {
            try
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
                int preference = 2;
                DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref preference, sizeof(int));
            }
            catch { }
        }

        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

        // ----------------- DRAG WINDOW -----------------
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        // ----------------- CLOSE TO TRAY -----------------
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (WindowState == WindowState.Minimized)
                this.Hide();
        }

        // ----------------- HOTKEY -----------------
        private void RegisterHotKey()
        {
            var s = _settingsManager!.Settings;

            _hotKeyManager!.UnregisterHotKey();
            _hotKeyManager.HotKeyPressed -= ToggleMute;
            _hotKeyManager.HotKeyPressed += ToggleMute;
            _hotKeyManager.RegisterHotKey(s.HotKeyModifier, s.HotKeyKey);

            TextHotKey.Text = KeyToString(s.HotKeyKey, s.HotKeyModifier);
        }

        // ----------------- AUDIO + SONIDOS -----------------
        private void ToggleMute()
        {
            _audioController!.ToggleMute();
            UpdateMicStatus();

            PlaySound(_audioController!.IsMuted()
                ? "UI/Sounds/mute.mp3"
                : "UI/Sounds/unmute.mp3");
        }

        private void PlaySound(string path)
        {
            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

                _soundPlayer?.Dispose();
                _audioReader?.Dispose();

                _audioReader = new AudioFileReader(fullPath);
                _soundPlayer = new WaveOutEvent();
                _soundPlayer.Init(_audioReader);
                _soundPlayer.Play();
            }
            catch (Exception ex)
            {
                File.WriteAllText("sound_error.log", ex.ToString());
            }
        }

        private string KeyToString(uint key, uint mod)
        {
            string k = ((Key)key).ToString();
            string prefix = "";

            if (mod == 1) prefix = "Alt+";
            if (mod == 2) prefix = "Ctrl+";
            if (mod == 4) prefix = "Shift+";

            return $"{prefix}{k}";
        }

        private void UpdateMicStatus()
        {
            bool muted = _audioController!.IsMuted();

            TextMicStatus.Text = muted ? "MUTED" : "ACTIVE";
            TextMicStatus.Foreground = muted
                ? System.Windows.Media.Brushes.Red
                : System.Windows.Media.Brushes.LimeGreen;

            _trayIcon!.SetMuted(muted);
        }

        private void ButtonToggleMute_Click(object sender, RoutedEventArgs e) => ToggleMute();

        private void ButtonChangeHotKey_Click(object sender, RoutedEventArgs e)
        {
            var win = new Windows.HotKeyWindow(_settingsManager!);
            win.Owner = this;

            if (win.ShowDialog() == true)
            {
                var s = _settingsManager!.Settings;
                TextHotKey.Text = KeyToString(s.HotKeyKey, s.HotKeyModifier);

                _hotKeyManager!.UnregisterHotKey();
                _hotKeyManager.RegisterHotKey(s.HotKeyModifier, s.HotKeyKey);
            }
        }

        private void CheckStartup_Checked(object? sender, RoutedEventArgs? e)
        {
            _startupManager!.EnableStartup();
            _settingsManager!.Settings.StartWithWindows = true;
            _settingsManager.Save();
        }

        private void CheckStartup_Unchecked(object? sender, RoutedEventArgs? e)
        {
            _startupManager!.DisableStartup();
            _settingsManager!.Settings.StartWithWindows = false;
            _settingsManager.Save();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _hotKeyManager?.Dispose();
            _trayIcon?.Dispose();

            _soundPlayer?.Dispose();
            _audioReader?.Dispose();
            _appMutex?.Dispose();
        }
    }
}
