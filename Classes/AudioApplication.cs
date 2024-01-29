using NAudio.CoreAudioApi;
using System;
using System.Diagnostics;
using System.Windows.Controls;

namespace VolumeMixer.Classes
{
    public class AudioApplication
    {

        AudioSessionControl audioSession = null;
        Process application = null;
        public event Action<AudioApplication> onProcessEnd = null;

        public Process ManagedProcess => application;
        public int ProcessID => application.Id;
        public float ApplicationVolume { get => audioSession.SimpleAudioVolume.Volume; set => audioSession.SimpleAudioVolume.Volume = value; }
        public AudioSessionControl AudioSession => audioSession;

        public AudioApplication(AudioSessionControl _audioSession, Process _application)
        {
            audioSession = _audioSession;
            application = _application;

            if (application.BasePriority == 0) return;
            application.EnableRaisingEvents = true;
            application.Exited += OnProcessEnd;
        }

        private void OnProcessEnd(object sender,EventArgs e)
        {
            onProcessEnd.Invoke(this);
        }
    }
}
