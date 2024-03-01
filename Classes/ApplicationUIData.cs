using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Image = System.Windows.Controls.Image;
using DIcon = System.Drawing.Icon;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VolumeMixer.Classes
{
    public class ApplicationUIData
    {
        //Todo Add WrapPanel
        WrapPanel container = null;
        Image image = null;
        Slider volumeSlider = null;
        TextBlock text = null;
        AudioApplication audioApplication = null;
        public event Action<ApplicationUIData,int> onApplicationClosed = null;

        public Image Image => image;
        public Slider VolumeSlider => volumeSlider;
        public TextBlock Text => text;
        public WrapPanel Container => container;

        public ApplicationUIData(AudioApplication _application)
        {
            audioApplication = _application;
            container = new WrapPanel();
            volumeSlider = GenerateSlider(audioApplication);
            text = GenerateTextBlock(_application);
            image = GenerateImage(_application);
            ////adding widgets to the wrapPanel
            
            container.Children.Add(image);
            container.Children.Add(text);
            container.Children.Add(volumeSlider);

            volumeSlider.ValueChanged += OnSliderValueChanged;
            _application.onVolumeChanged += (_volume) =>
            {
                Application.Current.Dispatcher.Invoke(() => { OnVolumeChanged(_volume); });
            };
            _application.onProcessEnd += (audioApplication) =>
            {
                Application.Current.Dispatcher.Invoke(() => { OnApplicationClose(audioApplication); });
            };
        }

        private void OnSliderValueChanged(object _sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider _slider = (Slider)_sender;
            if (_slider.Value == audioApplication.ApplicationVolume) return;
            audioApplication.ApplicationVolume = (float)e.NewValue;
            Console.WriteLine(e.NewValue);
        }
        private Slider GenerateSlider(AudioApplication _audioApp)
        {
            //slider side
            Slider _slider = new Slider();
            _slider.Maximum = 100f;
            _slider.Value = _audioApp.ApplicationVolume;
            _slider.HorizontalAlignment = HorizontalAlignment.Center;
            _slider.VerticalAlignment = VerticalAlignment.Center;
            _slider.HorizontalContentAlignment = HorizontalAlignment.Right;
            _slider.AutoToolTipPlacement = System.Windows.Controls.Primitives.AutoToolTipPlacement.TopLeft;
            _slider.AutoToolTipPrecision = 3;
            _slider.Width = 300;
            //_slider.ClipToBounds = true;
            
            return _slider;
        }
        private TextBlock GenerateTextBlock(AudioApplication _app)
        {
            //name side
            TextBlock _appName = new TextBlock();
            _appName.Text = _app.ProcessName;
            _appName.HorizontalAlignment = HorizontalAlignment.Center;
            _appName.VerticalAlignment = VerticalAlignment.Center;
            _appName.Width = 50;
            _appName.TextWrapping = TextWrapping.Wrap;
            return _appName;
        }
        private Image GenerateImage(AudioApplication _application)
        {
            Image _appIcon = new Image();
            _appIcon.Width = 35;
            _appIcon.Height = 35;
            //if (_application.BasePriority == 0) return _appIcon;
            

            DIcon _icon = _application.appIcon; /*Utils.GetIconFromFile(_application.BasePriority == 0 ? Process.GetCurrentProcess().MainModule.FileName : _application.MainModule.FileName);*/
            _appIcon.Source = Utils.GetIconToBitmapImage(_icon);

            return _appIcon;
        }
        void RemoveController()
        {
            container.Children.Remove(image);
            container.Children.Remove(text);
            container.Children.Remove(volumeSlider);
            volumeSlider = null;
            text = null;
            text = null;
        }
        void OnApplicationClose(AudioApplication _app)
        {
            RemoveController();
            onApplicationClosed?.Invoke(this, _app.ProcessID);
        }
        void OnVolumeChanged(float _newVolume)
        {
            if (_newVolume == (volumeSlider.Value / volumeSlider.Maximum)) return;
            volumeSlider.ValueChanged -= OnSliderValueChanged;
            volumeSlider.Value = _newVolume;
            volumeSlider.ValueChanged += OnSliderValueChanged;
        }
    }
}
