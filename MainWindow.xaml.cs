using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;

namespace VolumeMixer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
     

    public partial class MainWindow : Window
    {

        Dictionary<Slider, AudioSessionControl> sliders = new Dictionary<Slider, AudioSessionControl>();
        public MainWindow()
        {
            InitializeComponent();
            GenerateSliders(GetAllAppsFromDefaultDevice());
        }


        //private void TestButton_Click(object sender, RoutedEventArgs e)
        //{
        //    //MMDeviceEnumerator _outPutList = new MMDeviceEnumerator();
        //    //MMDevice _defautDevice = _outPutList.GetDefaultAudioEndpoint(DataFlow.Render,Role.Multimedia & Role.Communications & Role.Console);
        //    //_defautDevice.AudioSessionManager.SimpleAudioVolume.Volume = 0;
        //    //Console.WriteLine($"masterVolume -> { _defautDevice.AudioEndpointVolume.MasterVolumeLevelScalar}");
        //    //SessionCollection _apps = GetAllAppsFromDefaultDevice();

        //}

        private SessionCollection GetAllAppsFromDefaultDevice()
        {
            MMDeviceEnumerator _outPutList = new MMDeviceEnumerator();
            MMDevice _defautDevice = _outPutList.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia & Role.Communications & Role.Console);
            return _defautDevice.AudioSessionManager.Sessions;
        }
        private void GenerateSliders(SessionCollection _apps)
        {
            for (int _i = 0; _i < _apps.Count ; _i++)
            {
                //Slider _slider = new Slider();
                //sliders.Add(_apps[_i], _slider);
                //LayoutPanel.Children.Add(_slider);
                //_slider.Width = LayoutPanel.Width;
                //_slider.VerticalAlignment = VerticalAlignment.Bottom;
                //_slider.Margin = new Thickness(0,50, 0, 0);
                AddNewApp(_apps[_i]);

                //LayoutPanel.
            }
        }

        private void AddNewApp(AudioSessionControl _app)
        {
            Process _application = Process.GetProcessById((int)_app.GetProcessID);
            //slider side
            Slider _slider = new Slider();
            sliders.Add(_slider, _app);
            SliderLayoutPanel.Children.Add(_slider);
            _slider.Width = SliderLayoutPanel.Width;
            _slider.VerticalAlignment = VerticalAlignment.Bottom;
            _slider.Margin = new Thickness(0, 50, 0, 0);
            _slider.ValueChanged += OnSliderChanged;

            //name side
            TextBlock _appName = new TextBlock();
            NameLayoutPanel.Children.Add(_appName);
            _appName.Text = _application.ProcessName;
            _appName.Margin = _slider.Margin;

            //image side
            Image _appIcon = new Image();
            Console.WriteLine($" salut ? {_application.MainModule.FileName}");
            

            //IAudioSessionControl _app = _app as IAudioSessionControl;

            
            //Uri _fileUri = new Uri(_application.MainModule.FileName);
            //_appIcon.Source = new BitmapImage(_fileUri);
            //_appIcon.Margin = _slider.Margin;
        }

        private void OnSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider _slider = (Slider)sender;
            if (!sliders.ContainsKey(_slider))return;

            sliders[_slider].SimpleAudioVolume.Volume = (float)e.NewValue / (float)_slider.Maximum;
            Console.WriteLine(e.NewValue);
        }
    }
}
