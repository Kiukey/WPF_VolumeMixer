using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Session;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace VolumeMixer.Classes
{
    public class Mixer : IObserver<IAudioSession>, IObserver<DeviceVolumeChangedArgs>
    {
        public AudioApplication this[int _index] => applicationList[_index];
        public event Action<AudioApplication> onNewApplicationDiscovered = null;
        public event Action<AudioApplication> onAudioApplicationClosed = null;
        public event Action<float> onMasterVolumeChanged = null;
        //MMDevice device = null;
        CoreAudioDevice device = null;
        List<AudioApplication> applicationList = new List<AudioApplication>();
        public float MasterVolume { get => (float)device.Volume; set => device.Volume = value; }
        public int ApplicationCount => applicationList.Count;

        ~Mixer() 
        {
            applicationList.Clear();
        }

        public Mixer(CoreAudioDevice _Device)
        {
            //device = _device;
            device = _Device;
            //device.Volume
            CreateAudioApplications(device);
            device.SessionController.SessionCreated.Subscribe(this);
            //device.AudioSessionManager.OnSessionCreated += OnNewAppDetected;
            device.VolumeChanged.Subscribe(this);

            MMDevice _device;

            
        }
        void CreateAudioApplications(CoreAudioDevice _device)
        {
            List<IAudioSession> _sessions = _device.SessionController.ToList();
            int _count = _sessions.Count;
            for (int i = 0; i < _count; i++)
            {
                CreateAudioApp(_sessions[i], Process.GetProcessById((int)_sessions[i].ProcessId));
            }
        }
        void CreateAudioApp(IAudioSession _audioSession, Process _process)
        {
            AudioApplication _app = new AudioApplication(_audioSession,_process);
            _app.onProcessEnd += OnProcessEnd;
            onNewApplicationDiscovered?.Invoke(_app);
            if (_app.IsSystemApp)
                applicationList.Insert(0, _app);
            else
                applicationList.Add(_app);
        }
        void OnProcessEnd(AudioApplication _application)
        {
            onAudioApplicationClosed?.Invoke(_application);
            applicationList.Remove(_application);
            Console.WriteLine($"app closed {_application.ProcessID}");
        }

        public void OnNext(IAudioSession value)
        {
            AudioApplication _newApp = new AudioApplication(value, Process.GetProcessById(value.ProcessId));
            Console.WriteLine("new app discovered");
            applicationList.Add( _newApp );
            onNewApplicationDiscovered?.Invoke(_newApp);
        }

        public void OnError(Exception error)
        {
            //TODO pop up a window showing the error
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            
        }
        public void OnNext(DeviceVolumeChangedArgs value)
        {
            onMasterVolumeChanged?.Invoke((float)value.Volume);
            Console.WriteLine(value.Volume);
        }
    }
}
