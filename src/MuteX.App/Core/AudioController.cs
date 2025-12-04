using NAudio.CoreAudioApi;

namespace MuteX.App.Core
{
    public class AudioController
    {
        private readonly MMDeviceEnumerator _deviceEnumerator;
        private readonly MMDevice _inputDevice;

        public AudioController()
        {
            _deviceEnumerator = new MMDeviceEnumerator();
            _inputDevice = _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);
        }

        public bool IsMuted()
        {
            return _inputDevice.AudioEndpointVolume.Mute;
        }

        public void ToggleMute()
        {
            _inputDevice.AudioEndpointVolume.Mute = !_inputDevice.AudioEndpointVolume.Mute;
        }

        public void SetMute(bool mute)
        {
            _inputDevice.AudioEndpointVolume.Mute = mute;
        }
    }
}