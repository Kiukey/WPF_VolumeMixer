using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
using System.CodeDom;
using System.Diagnostics;
using DIcon = System.Drawing.Icon;

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
        public DIcon appIcon { get; private set;}
        public AudioSessionControl AudioSession => audioSession;

        public AudioApplication(AudioSessionControl _audioSession, Process _application)
        {
            audioSession = _audioSession;
            application = _application;

            audioSession.RegisterEventClient(this);
            ProcessName = ManagedProcess.ProcessName;
            //if (application.BasePriority == 0)
            //{
            //    ProcessName = "SystemSounds";
            //    return;
            //}
            try
            {
                application.EnableRaisingEvents = true;
                application.Exited += OnProcessEnd;
                appIcon = Utils.GetIconFromFile(_application.MainModule.FileName);
            }
            catch (Exception ex)
            {
                appIcon = Utils.GetIconFromFile(Process.GetCurrentProcess().MainModule.FileName);
                ProcessName = application.BasePriority == 0 ? "SystemSounds" : ManagedProcess.ProcessName;
                Console.WriteLine("no enough permission");
            }

        }

        ~AudioApplication() 
        {
            audioSession.UnRegisterEventClient(this);
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
            //OnProcessEnd(this,null);
            //Console.WriteLine("Session ended");
            Console.WriteLine("Session Ended");
        }
    }
}
