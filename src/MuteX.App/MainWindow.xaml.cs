using System;
using System.Windows;
using System.Windows.Interop;
using MuteX.App.Core;
using MuteX.App.UI;

namespace MuteX.App
{
    public partial class MainWindow : Window
    {
        private HotKeyManager? _hotKeyManager;
        private AudioController? _audioController;
        private SettingsManager? _settingsManager;
        private StartupManager? _startupManager;
        private TrayIcon? _trayIcon;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _audioController = new AudioController();
            _settingsManager = new SettingsManager();
            _startupManager = new StartupManager();

            var handle = new WindowInteropHelper(this).Handle;
            _hotKeyManager = new HotKeyManager(handle);

            RegisterHotKey();

            _trayIcon = new TrayIcon();
            _trayIcon.ToggleRequested += ToggleMute;
            _trayIcon.OpenWindowRequested += () => Show();
            _trayIcon.ExitRequested += () => Application.Current.Shutdown();

            UpdateMicStatus();

            CheckStartup.IsChecked = _settingsManager.Settings.StartWithWindows;
            CheckStartup_Checked(null, null);
        }

        private void RegisterHotKey()
        {
            var s = _settingsManager!.Settings;
            _hotKeyManager!.UnregisterHotKey();
            _hotKeyManager.HotKeyPressed -= ToggleMute;
            _hotKeyManager.HotKeyPressed += ToggleMute;
            _hotKeyManager.RegisterHotKey(s.HotKeyModifier, s.HotKeyKey);

            TextHotKey.Text = KeyToString(s.HotKeyKey, s.HotKeyModifier);
        }

        private void ToggleMute()
        {
            _audioController!.ToggleMute();
            UpdateMicStatus();
        }

        private string KeyToString(uint key, uint mod)
        {
            string k = ((System.Windows.Input.Key)key).ToString();
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
            TextMicStatus.Foreground = muted ?
                System.Windows.Media.Brushes.Red :
                System.Windows.Media.Brushes.LimeGreen;
            _trayIcon!.SetMuted(muted);
        }

        private void ButtonToggleMute_Click(object sender, RoutedEventArgs e)
        {
            ToggleMute();
        }

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
        }
    }
}