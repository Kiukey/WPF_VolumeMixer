
using AudioSwitcher.AudioApi.CoreAudio;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VolumeMixer.Classes;
using VolumeMixer.Classes.SoundInputManager;

namespace VolumeMixer
{
    public partial class MainWindow : Window
    {
        Dictionary<int,CoreAudioDevice> outputDevices = new Dictionary<int, CoreAudioDevice>();
        Mixer defaultDeviceMixer = null;
        SoundInputManager soundInputManager = null;
        CoreAudioController controller = null;
        DiscordWrapper discord = null;
        public MainWindow()
        {
            InitializeComponent();
            controller = new CoreAudioController();
            //////set up sound output
            RegisterOutputDevices();
            defaultDeviceMixer = new Mixer(controller.DefaultPlaybackDevice);
            GenerateApplications(defaultDeviceMixer);
            defaultDeviceMixer.onMasterVolumeChanged += (_newVolume) =>
            {
                Application.Current.Dispatcher.Invoke(() => { OnMixerMasterVolumeChanged(_newVolume);});
            };
            defaultDeviceMixer.onNewApplicationDiscovered += (_audioApp) => 
            {
                Application.Current.Dispatcher.Invoke(() => { GenerateOutputUI(_audioApp); });
            };
            masterVolumeSlider.Value = defaultDeviceMixer.MasterVolume;
            ////
            ////set up sound input
            soundInputManager = new SoundInputManager(controller.DefaultCaptureDevice);
            RegisterInputDevices();
            //if(soundInputManager)
            microphoneVolume.Value = soundInputManager.InputDeviceVolumeScaled;
            //
        }

        #region output(Mixer to rework maybe)
        private CoreAudioDevice GetDefaultDevice()
        {
            return controller.DefaultPlaybackDevice;
        }
        private List<CoreAudioDevice> GetAllDevices()
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
            List<CoreAudioDevice> _devices = controller.GetPlaybackDevices(AudioSwitcher.AudioApi.DeviceState.Active).ToList();
            deviceComboBox.ItemsSource = _devices;
            deviceComboBox.DisplayMemberPath = "FullName";
            deviceComboBox.SelectedItem = controller.DefaultPlaybackDevice;
            int _count = _devices.Count;
            for (int i = 0; i < _count; i++)
            {
                outputDevices.Add(i, _devices[i]);
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
            CoreAudioDevice _defaultDevice = outputDevices[_sender.SelectedIndex];
            if (_defaultDevice.Name == GetDefaultDevice().Name) return;
            controller.SetDefaultDevice(_defaultDevice);
            RefreshMixer(_defaultDevice);

        }
        void RefreshMixer(CoreAudioDevice _device)
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
            defaultDeviceMixer.MasterVolume = (float)e.NewValue;
        }
        private void OnMixerMasterVolumeChanged(float _newVolume)
        {
            if (masterVolumeSlider == null) return;
            masterVolumeSlider.ValueChanged -= OnMasterVolumeChanged;
            masterVolumeSlider.Value = _newVolume;
            masterVolumeSlider.ValueChanged += OnMasterVolumeChanged;
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
        private void OnMicrophoneVolumeChanged(object _sender, RoutedPropertyChangedEventArgs<double> _e)
        {
            soundInputManager.InputDeviceVolumeScaled = (float)_e.NewValue;
        }
        #endregion

        private void AuthorizationButton_Click(object sender, RoutedEventArgs e)
        {
            if(discord == null) return;
            discord.AskPermission();
        }

        private void ValidateCodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (discord == null) return;
            discord.Initilize(codeTextBox.Text);
        }

        private void ApplicationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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