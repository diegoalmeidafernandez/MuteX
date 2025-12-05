using System;
using System.Drawing;
using System.Windows.Forms;

namespace MuteX.App.UI
{
    public class TrayIcon : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;
        private Icon _iconOn;
        private Icon _iconOff;

        public event Action? ToggleRequested;
        public event Action? OpenWindowRequested;
        public event Action? ExitRequested;

        public TrayIcon()
        {
            _iconOn = new Icon("UI/Icons/mic_on.ico");
            _iconOff = new Icon("UI/Icons/mic_off.ico");

            _notifyIcon = new NotifyIcon
            {
                Visible = true,
                Icon = _iconOn,
                Text = "MuteX - Microphone Controller"
            };

            CreateContextMenu();
        }

        private void CreateContextMenu()
        {
            var menu = new ContextMenuStrip();

            var toggle = new ToolStripMenuItem("Mute / Unmute");
            toggle.Click += (s, e) => ToggleRequested?.Invoke();

            var open = new ToolStripMenuItem("Open MuteX");
            open.Click += (s, e) => OpenWindowRequested?.Invoke();

            var exit = new ToolStripMenuItem("Exit");
            exit.Click += (s, e) => ExitRequested?.Invoke();

            menu.Items.Add(toggle);
            menu.Items.Add(open);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(exit);

            _notifyIcon.ContextMenuStrip = menu;
        }

        public void SetMuted(bool muted)
        {
            _notifyIcon.Icon = muted ? _iconOff : _iconOn;
        }

        public void Dispose()
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _iconOn.Dispose();
            _iconOff.Dispose();
        }
    }
}
