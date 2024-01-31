using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace VolumeMixer.Classes.SoundInputManager
{
    public class SoundInputManager
    {
        CoreAudioDevice defaultDevice = null;

        public string MicrophonePictureLink => defaultDevice.IconPath;
        public float InputDeviceVolumeScaled { get => (float)defaultDevice.Volume / 100f; set => defaultDevice.Volume = value *100; }

        public SoundInputManager(CoreAudioDevice _inputDevice)
        {
            defaultDevice = _inputDevice;
        }

        public void SetDefaultInputDevice(CoreAudioDevice _inputDevice)
        {
            defaultDevice = _inputDevice;
            defaultDevice.Controller.DefaultCaptureDevice = _inputDevice;
        }
    }
}
