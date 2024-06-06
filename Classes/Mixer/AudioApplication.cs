using AudioSwitcher.AudioApi.Session;
using System;
using System.Diagnostics;
using System.Windows;
using DIcon = System.Drawing.Icon;

namespace VolumeMixer.Classes
{
    public class AudioApplication : IObserver<SessionVolumeChangedArgs>
    {
        public event Action<float> onVolumeChanged = null;
        IAudioSession audioSession = null;
        Process application = null;
        public event Action<AudioApplication> onProcessEnd = null;
        public Process ManagedProcess => application;
        public int ProcessID => application.Id;
        public string ProcessName { get; private set;}
        public float ApplicationVolume { get => (float)audioSession.Volume; set => audioSession.Volume = value; }
        public DIcon appIcon { get; private set;}
        public IAudioSession AudioSession => audioSession;
        public bool IsSystemApp => audioSession.IsSystemSession;

        public AudioApplication(IAudioSession _audioSession, Process _application)
        {
            audioSession = _audioSession;
            application = _application;
            bool _isSystemApp = audioSession.IsSystemSession;
            audioSession.VolumeChanged.Subscribe(this);
            try
            {
                application.EnableRaisingEvents = true;
                application.Exited += OnProcessEnd;
                ProcessName = _isSystemApp ? "SystemSounds" : ManagedProcess.ProcessName;
                appIcon = Utils.GetIconFromFile(_isSystemApp ? Process.GetCurrentProcess().MainModule.FileName : _application.MainModule.FileName);
            }
            catch (Exception ex)
            {
                appIcon = Utils.GetIconFromFile(Process.GetCurrentProcess().MainModule.FileName);
                ProcessName = application.BasePriority == 0 ? "SystemSounds" : ManagedProcess.ProcessName;
            }

        }

        ~AudioApplication()
        { 
        }

        private void OnProcessEnd(object sender,EventArgs e)
        {
            onProcessEnd.Invoke(this);
        }
        public void OnNext(SessionVolumeChangedArgs value)
        {
            onVolumeChanged?.Invoke((float)value.Volume);   
        }
        public void OnError(Exception error)
        {
            //TODO pop up a window showing the errors
            MessageBox.Show(error.ToString());
        }
        public void OnCompleted()
        {
        }
    }
}
