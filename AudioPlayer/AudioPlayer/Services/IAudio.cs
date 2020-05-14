using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AudioPlayer.Services
{
    public struct InfoMP3
    {
        public string Artist;
        public string NameSong;
        public string Duration;
    }

    public delegate void VolumeHandler(int volume);
    public delegate void PositionHandler(int position, int duration);

    public abstract class IAudio
    {
        public abstract event VolumeHandler VolumeEvent;
        public abstract event PositionHandler PositionHandler;

        public abstract InfoMP3 GetInfo();
        public abstract void PlayAudioFile(string filePath);
        public abstract void Play();
        public abstract void Stop();
        public abstract void SetLooping(bool loop);
        public abstract void SetVolume(int volume);
        public abstract int GetMaxVolume();
        public abstract int GetVolume();
        public abstract void SetDataSource(string filePath);
        
    }
}
