using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolumeMixer.Classes
{
    public class Mixer
    {
        public AudioApplication this[int _index] => applicationList[_index];
        public event Action<AudioApplication> onNewApplicationDiscovered = null;
        public event Action<AudioApplication> onAudioApplicationClosed = null;
        MMDevice device = null;
        List<AudioApplication> applicationList = new List<AudioApplication>();

        public int ApplicationCount => applicationList.Count;

        ~Mixer() 
        {
            applicationList.Clear();
        }
        public Mixer(MMDevice _device)
        {
            device = _device;
            CreateAudioApplications(device);
            device.AudioSessionManager.OnSessionCreated += OnNewAppDetected;
        }
        void CreateAudioApplications(MMDevice _device)
        {
            SessionCollection _apps = _device.AudioSessionManager.Sessions;
            int _count = _apps.Count;
            for (int i = 0; i < _count; i++)
            {
                CreateAudioApp(_apps[i], Process.GetProcessById((int)_apps[i].GetProcessID));
            }
        }
        void CreateAudioApp(AudioSessionControl _audioSession, Process _process)
        {
            AudioApplication _app = new AudioApplication(_audioSession,_process);
            applicationList.Add(_app);
            _app.onProcessEnd += OnProcessEnd;
            onNewApplicationDiscovered?.Invoke(_app);
        }
        void OnProcessEnd(AudioApplication _application)
        {
            onAudioApplicationClosed?.Invoke(_application);
            applicationList.Remove(_application);
            Console.WriteLine($"app closed {_application.ProcessID}");
        }
        void OnNewAppDetected(object _sender, IAudioSessionControl _newSession)
        {
            AudioSessionControl _audioApp = new AudioSessionControl(_newSession);
            CreateAudioApp(_audioApp, Process.GetProcessById((int)_audioApp.GetProcessID));
            Console.WriteLine("new app detected");
        }
    }
}
