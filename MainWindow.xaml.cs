using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VolumeMixer.Classes;
using VolumeMixer.Classes.SoundInputManager;
using DeviceRole = NAudio.CoreAudioApi.Role;
using DeviceState = NAudio.CoreAudioApi.DeviceState;
using DIcon = System.Drawing.Icon;
//using AudioSwitcher.AudioApi.CoreAudio;
//using Role = 

namespace VolumeMixer
{
    public partial class MainWindow : Window
    {
        Dictionary<int,MMDevice> outputDevices = new Dictionary<int,MMDevice>();
        Mixer defaultDeviceMixer = null;
        SoundInputManager soundInputManager = null;
        CoreAudioController controller = null;
        public MainWindow()
        {
            InitializeComponent();
            //////set up sound output
            RegisterOutputDevices();
            defaultDeviceMixer = new Mixer(GetDefaultDevice());
            GenerateApplications(defaultDeviceMixer);
            defaultDeviceMixer.onNewApplicationDiscovered += (_audioApp) => 
            {
                Application.Current.Dispatcher.Invoke(() => { GenerateOutputUI(_audioApp); });
            };
            controller = new CoreAudioController();
            masterVolumeSlider.Value = defaultDeviceMixer.MasterVolume;
            ////

            ////set up sound input
            soundInputManager = new SoundInputManager(controller.DefaultCaptureDevice);
            RegisterInputDevices();
            microphoneVolume.Value = soundInputManager.InputDeviceVolumeScaled;
            //
        }

        #region output(Mixer to rework maybe)
        private MMDevice GetDefaultDevice()
        {
            MMDeviceEnumerator _outPutList = new MMDeviceEnumerator();
            return _outPutList.GetDefaultAudioEndpoint(DataFlow.Render, DeviceRole.Multimedia & DeviceRole.Communications & DeviceRole.Console);
        }
        private List<MMDevice> GetAllDevices()
        {
            return outputDevices.Values.ToList();
        }
        private void GenerateApplications(Mixer _mixer)
        {
            for (int i = 0; i < _mixer.ApplicationCount; i++)
            {
                GenerateOutputUI(_mixer[i]);
            }
        }
        void RegisterOutputDevices()
        {
            MMDeviceEnumerator _outputList = new MMDeviceEnumerator();
            MMDeviceCollection _allDevices = _outputList.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            for (int i = 0; i < _allDevices.Count; i++)
            {
                MMDevice _device = _allDevices[i];
                outputDevices.Add(i, _device);
                deviceComboBox.Items.Add(_device.FriendlyName);
                if (_device.FriendlyName == GetDefaultDevice().FriendlyName)
                {
                    deviceComboBox.SelectedIndex = deviceComboBox.Items.Count - 1;
                }
            }
        }
        void GenerateOutputUI(AudioApplication _application)
        {
            ApplicationUIData _data = new ApplicationUIData(_application);
            _data.onApplicationClosed += OnAudioAppClosed;
            ApplicationList.Items.Add(_data.Container);
        }
        void OnAudioAppClosed(ApplicationUIData _closed, int _processID)
        {
            ApplicationList.Items.Remove(_closed.Container);
        }
        private void OnOutputMainDeviceChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBox _sender = (ComboBox)sender;
            if (_sender == null || !outputDevices.ContainsKey(_sender.SelectedIndex)) return;
            MMDevice _defaultDevice = outputDevices[_sender.SelectedIndex];
            if (_defaultDevice.FriendlyName == GetDefaultDevice().FriendlyName) return;
            controller.SetDefaultDevice(GetDeviceFromController(_defaultDevice));
            RefreshMixer(_defaultDevice);

        }
        void RefreshMixer(MMDevice _device)
        {
            defaultDeviceMixer = null;
            defaultDeviceMixer = new Mixer(_device);
            masterVolumeSlider.Value = defaultDeviceMixer.MasterVolume;
            ApplicationList.Items.Clear();
            GenerateApplications(defaultDeviceMixer);
        }
        private void OnMasterVolumeChanged(object _sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider _slider = (Slider)_sender;
            if (_slider == null) return;
            defaultDeviceMixer.MasterVolume = (float)e.NewValue / (float)_slider.Maximum; //TODO divide by max
        }
        #endregion
        #region Input
        void RegisterInputDevices()
        {
            List<CoreAudioDevice> _devices = controller.GetCaptureDevices(AudioSwitcher.AudioApi.DeviceState.Active).ToList();
            captureDevicesComboBox.ItemsSource = _devices;
            captureDevicesComboBox.DisplayMemberPath = "FullName";
            captureDevicesComboBox.SelectedItem = controller.DefaultCaptureDevice;
        }
        private void OnInputDeviceChanged(object _sender, SelectionChangedEventArgs e)
        {
            ComboBox _box = (ComboBox)_sender;
            if (_box == null) return;
            CoreAudioDevice _device = (CoreAudioDevice)_box.SelectedItem;
            soundInputManager.SetDefaultInputDevice(_device);
            //controller.DefaultCaptureDevice = (CoreAudioDevice)_box.SelectedItem;
        }
        #endregion
        #region utils
        CoreAudioDevice GetDeviceFromController(MMDevice _device)
        {
            List<CoreAudioDevice> _devices = controller.GetDevices(DeviceType.Playback,AudioSwitcher.AudioApi.DeviceState.Active).ToList();
            foreach (CoreAudioDevice _item in _devices)
            {
                Console.WriteLine(_item.FullName);
                if (_item.FullName == _device.FriendlyName) return _item;
            }
            return null;
        }

        #endregion
        private void OnMicrophoneVolumeChanged(object _sender, RoutedPropertyChangedEventArgs<double> _e)
        {
            soundInputManager.InputDeviceVolumeScaled = (float)_e.NewValue;
        }
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