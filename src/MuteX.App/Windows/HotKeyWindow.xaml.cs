using System.Windows;
using System.Windows.Input;
using MuteX.App.Core;

namespace MuteX.App.Windows
{
    public partial class HotKeyWindow : Window
    {
        private readonly SettingsManager _settings;

        public HotKeyWindow(SettingsManager settings)
        {
            InitializeComponent();
            _settings = settings;
            KeyDown += Window_KeyDown;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            uint keyValue = (uint)KeyInterop.VirtualKeyFromKey(e.Key);

            _settings.Settings.HotKeyKey = keyValue;
            _settings.Settings.HotKeyModifier = 0;
            _settings.Save();

            TextKeyPreview.Text = $"Selected: {e.Key}";

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
