using HandyControl.Controls;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Resources;

namespace ErHuo.Utilities
{
    public static class SoundPlayUtil
    {

        private static readonly NAudioSound _startSound = new NAudioSound(Properties.Resources.startsound);
        private static readonly NAudioSound _stopSound = new NAudioSound(Properties.Resources.stopsound);

        public static void PlayStartSound()
        {
            _startSound.Play();
        }

        public static void PlayStopSound()
        {
            _stopSound.Play();
        }

        public static void ChangeVolume(int volume)
        {
            _startSound.ChangeVolume(volume / 100f);
            _stopSound.ChangeVolume(volume / 100f);
        }

    }

    class NAudioSound
    {
        private readonly IWavePlayer _soundDevice;
        private readonly WaveFileReader _soundReader;
        public NAudioSound(Stream stream)
        {

            _soundReader = new WaveFileReader(stream);
            _soundDevice = new WaveOutEvent();
            _soundDevice.Init(_soundReader);
            _soundDevice.PlaybackStopped += OnPlaybackStopped;
        }

        public void Play()
        {
            _soundDevice.Play();
        }

        private void OnPlaybackStopped(object obj, StoppedEventArgs arg)
        {
            _soundReader.Seek(0, SeekOrigin.Begin);
        }

        public void ChangeVolume(float volume)
        {
            _soundDevice.Volume = volume;
        }
    }

}
