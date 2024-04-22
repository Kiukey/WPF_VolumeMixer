using AudioSwitcher.AudioApi.CoreAudio;


namespace VolumeMixer.Classes.SoundInputManager
{
    public class SoundInputManager
    {
        CoreAudioDevice defaultDevice = null;

        public string MicrophonePictureLink => defaultDevice.IconPath;
        public float InputDeviceVolumeScaled { get  
        { 
                if(defaultDevice == null)
                    return 0;
                return (float)defaultDevice.Volume / 100f;
        } 
        set => defaultDevice.Volume = value * 100; }

        //public int Salut { get { return 0; } set { Salut = value; } }
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
