using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using VolumeMixer.Classes;
using DIcon = System.Drawing.Icon;

namespace VolumeMixer
{
    public partial class MainWindow : Window
    {

        MMDevice defaultDevice = null;
        Dictionary<int,ApplicationUIData> applications = new Dictionary<int, ApplicationUIData>();

        public MainWindow()
        {
            defaultDevice = GetDefaultDevice();
            InitializeComponent();
            GenerateSliders(defaultDevice.AudioSessionManager.Sessions);
            defaultDevice.AudioSessionManager.OnSessionCreated += (_object, _session) => 
            {
                Application.Current.Dispatcher.Invoke(() => { OnNewAppDetected(_object, _session); });
            };
        }

        private MMDevice GetDefaultDevice()
        {
            MMDeviceEnumerator _outPutList = new MMDeviceEnumerator();
            return _outPutList.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia & Role.Communications & Role.Console);
        }
        private void GenerateSliders(SessionCollection _apps)
        {
            for (int _i = 0; _i < _apps.Count ; _i++)
            {
                AddNewApp(_apps[_i]);

                //LayoutPanel.
            }
        }

        private void AddNewApp(AudioSessionControl _app)
        {
            Process _application = Process.GetProcessById((int)_app.GetProcessID);

            WrapPanel _panel = new WrapPanel();
            
            //todo CreateWrapPanel
            ApplicationUIData _data = new ApplicationUIData(_app,_application) ;
            applications.Add(_application.Id, _data);
            ApplicationList.Items.Add(_data.Container);
            _data.Container.Width = ApplicationList.Width;
            //_data.VolumeSlider.
            //applications.Add(_application.Id,_data);
            //TODO AddWrapPanel to listView
            //if (_application.BasePriority == 0) return;
            //_application.EnableRaisingEvents = true;
            //_application.Exited += (_object, _args) =>
            //{
            //    Application.Current.Dispatcher.Invoke(() => { OnApplicationClosed(_object, _args); });
            //};
        }

        void OnNewAppDetected(object _sender, IAudioSessionControl _newSession)
        {
            AddNewApp(new AudioSessionControl(_newSession));
        }

        //void OnApplicationClosed(object _sender, EventArgs e)
        //{
        //    Process _closingApp = (Process)_sender;
        //    if (_closingApp == null || !applications.ContainsKey(_closingApp.Id)) return;
        //    ApplicationData _data = applications[_closingApp.Id];
        //    ImageLayoutPanel.Children.Remove(_data.Image);
        //    NameLayoutPanel.Children.Remove(_data.Text);
        //    SliderLayoutPanel.Children.Remove(_data.VolumeSlider);
        //    applications.Remove(_closingApp.Id);
        //}
    }
}

#region comments
//Slider _slider = new Slider();
//sliders.Add(_slider, _app);
//SliderLayoutPanel.Children.Add(_slider);
//_slider.Width = SliderLayoutPanel.Width;
//_slider.VerticalAlignment = VerticalAlignment.Bottom;
//_slider.Margin = new Thickness(0, 30, 0, 0);
//_slider.ValueChanged += OnSliderChanged;
//_slider.Maximum = 1f;
//_slider.Value = _app.SimpleAudioVolume.Volume;

//TextBlock _appName = new TextBlock();
//NameLayoutPanel.Children.Add(_appName);
//_appName.Text = _application.ProcessName;
//_appName.Margin = _slider.Margin;

//Console.WriteLine($" salut ? {_application.BasePriority}");
//if (_application.BasePriority ==0 ) return;
//DIcon _icon = DIcon.ExtractAssociatedIcon(_application.MainModule.FileName); 
//Image _appIcon = new Image();
//ImageLayoutPanel.Children.Add(_appIcon);
//_appIcon.Source = GetIconToBitmapImage(_icon);
//_appIcon.Width = 35;
//_appIcon.Height = 35;
//_appIcon.Margin = new Thickness(0,20,0,0);

//Slider GenerateSlider(AudioSessionControl _audioApp)
//{
//    //slider side
//    Slider _slider = new Slider();
//    //sliders.Add(_slider, _audioApp);
//    SliderLayoutPanel.Children.Add(_slider);
//    _slider.Width = SliderLayoutPanel.Width;
//    _slider.VerticalAlignment = VerticalAlignment.Bottom;
//    _slider.Margin = new Thickness(0, 30, 0, 0);
//    _slider.Maximum = 1f;
//    _slider.Value = _audioApp.SimpleAudioVolume.Volume;
//    return _slider;
//}

//{
//    //name side
//    TextBlock _appName = new TextBlock();
//    NameLayoutPanel.Children.Add(_appName);
//    _appName.Text = _application.ProcessName;
//    _appName.Margin = new Thickness(0,33, 0,0);
//    return _appName;
//}
#endregion