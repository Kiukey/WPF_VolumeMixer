using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Diagnostics;
using System.Windows.Controls;

namespace VolumeMixer.Classes
{
    public class AudioApplication : IAudioSessionEventsHandler
    {
        public event Action<float> onVolumeChanged = null;
        AudioSessionControl audioSession = null;
        Process application = null;
        public event Action<AudioApplication> onProcessEnd = null;

        public Process ManagedProcess => application;
        public int ProcessID => application.Id;
        public string ProcessName { get; private set;}
        public float ApplicationVolume { get => audioSession.SimpleAudioVolume.Volume; set => audioSession.SimpleAudioVolume.Volume = value; }
        public AudioSessionControl AudioSession => audioSession;

        public AudioApplication(AudioSessionControl _audioSession, Process _application)
        {
            audioSession = _audioSession;
            application = _application;

            audioSession.RegisterEventClient(this);
            ProcessName = ManagedProcess.ProcessName;
            if (application.BasePriority == 0)
            {
                ProcessName = "SystemSounds";
                return;
            }
            
            application.EnableRaisingEvents = true;
            application.Exited += OnProcessEnd;
        }

        private void OnProcessEnd(object sender,EventArgs e)
        {
            onProcessEnd.Invoke(this);
        }

        public void OnVolumeChanged(float _volume, bool _isMuted)
        {
            onVolumeChanged?.Invoke(_volume);
        }

        public void OnDisplayNameChanged(string _displayName)
        {
            //don't want to do anything anyway
            //to think about though
        }

        public void OnIconPathChanged(string _iconPath)
        {
            //don't want to do anything anyway
        }

        public void OnChannelVolumeChanged(uint _channelCount, IntPtr _newVolumes, uint _channelIndex)
        {
            //don't want to do anything anyway
        }

        public void OnGroupingParamChanged(ref Guid _groupingId)
        {
            //don't want to do anything anyway
        }

        public void OnStateChanged(AudioSessionState _state)
        {
            //don't want to do anything anyway
        }

        public void OnSessionDisconnected(AudioSessionDisconnectReason _disconnectReason)
        {
            //is not working ?
        }
    }
}
