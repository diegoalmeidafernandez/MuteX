using System;
using System.Drawing;
using System.Windows.Forms;

namespace MuteX.App.UI
{
    public class TrayIcon : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly Icon _iconOn;
        private readonly Icon _iconOff;

        public event Action? ToggleRequested;
        public event Action? OpenWindowRequested;
        public event Action? ExitRequested;

        public TrayIcon()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Visible = true;

            _iconOn = LoadIcon("UI/Icons/mic_on.ico");
            _iconOff = LoadIcon("UI/Icons/mic_off.ico");

            _notifyIcon.Icon = _iconOff;

            var menu = new ContextMenuStrip();
            menu.Items.Add("Toggle Mute", null, (s, e) => ToggleRequested?.Invoke());
            menu.Items.Add("Open Window", null, (s, e) => OpenWindowRequested?.Invoke());
            menu.Items.Add("Exit", null, (s, e) => ExitRequested?.Invoke());
            _notifyIcon.ContextMenuStrip = menu;

            _notifyIcon.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                    ToggleRequested?.Invoke();
            };
        }

        private Icon LoadIcon(string relativePath)
        {
            try
            {
                var uri = new Uri($"pack://application:,,,/{relativePath}");
                var stream = System.Windows.Application.GetResourceStream(uri)?.Stream;

                if (stream == null)
                    throw new Exception($"No se pudo cargar: {relativePath}");

                return new Icon(stream);
            }
            catch
            {
                return SystemIcons.Warning;
            }
        }

        public void SetMuted(bool muted)
        {
            _notifyIcon.Icon = muted ? _iconOff : _iconOn;
        }

        public void Dispose()
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
        }
    }
}
