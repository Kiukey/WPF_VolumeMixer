using NAudio.CoreAudioApi;
using System;
using System.Diagnostics;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using Image = System.Windows.Controls.Image;
using DIcon = System.Drawing.Icon;
using System.Drawing;
using System.Windows.Media;

namespace VolumeMixer.Classes
{
    public class ApplicationUIData
    {
        //Todo Add WrapPanel
        WrapPanel container = null;
        Image image = null;
        Slider volumeSlider = null;
        TextBlock text = null;
        AudioSessionControl audioSession = null;

        public Image Image => image;
        public Slider VolumeSlider => volumeSlider;
        public TextBlock Text => text;
        public WrapPanel Container => container;

        public ApplicationUIData(AudioSessionControl _audioSession,Process _application)
        {
            audioSession = _audioSession;
            container = new WrapPanel();
            container.Orientation = Orientation.Horizontal;
            container.HorizontalAlignment = HorizontalAlignment.Stretch;
            container.VerticalAlignment = VerticalAlignment.Stretch;
            container.ClipToBounds = true;
            //Creating widgets
            volumeSlider = GenerateSlider(_audioSession);
            text = GenerateTextBlock(_application);
            image = GenerateImage(_application);
            //adding widgets to the wrapPanel
            container.Children.Add(image);
            container.Children.Add(text);
            container.Children.Add(volumeSlider);

            volumeSlider.ValueChanged += OnSliderValueChanged;
        }

        private void OnSliderValueChanged(object _sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider _slider = (Slider)_sender;
            if (_slider == null) return;
            audioSession.SimpleAudioVolume.Volume = (float)e.NewValue / (float)_slider.Maximum;
            Console.WriteLine(e.NewValue);
        }


        private Slider GenerateSlider(AudioSessionControl _audioApp)
        {
            //slider side
            Slider _slider = new Slider();
            _slider.Maximum = 1f;
            _slider.Value = _audioApp.SimpleAudioVolume.Volume;
            _slider.HorizontalAlignment = HorizontalAlignment.Center;
            _slider.VerticalAlignment = VerticalAlignment.Center;
            _slider.HorizontalContentAlignment = HorizontalAlignment.Right;
            _slider.Width = 500;
            //_slider.ClipToBounds = true;
            
            return _slider;
        }
        private TextBlock GenerateTextBlock(Process _application)
        {
            //name side
            TextBlock _appName = new TextBlock();
            _appName.Text = _application.ProcessName;
            _appName.HorizontalAlignment = HorizontalAlignment.Center;
            _appName.VerticalAlignment = VerticalAlignment.Center;
            return _appName;
        }

        private BitmapSource GetIconToBitmapImage(DIcon _icon)
        {
            if (_icon == null) return null;
            return Imaging.CreateBitmapSourceFromHBitmap(_icon.ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }


        Image GenerateImage(Process _application)
        {
            Image _appIcon = new Image();
            _appIcon.Width = 35;
            _appIcon.Height = 35;
            //_appIcon.Margin = new Thickness(0, 20, 0, 0);
            if (_application.BasePriority == 0) return _appIcon;
            DIcon _icon = DIcon.ExtractAssociatedIcon(_application.MainModule.FileName);
            _appIcon.Source = GetIconToBitmapImage(_icon);
            return _appIcon;
        }
    }
}
