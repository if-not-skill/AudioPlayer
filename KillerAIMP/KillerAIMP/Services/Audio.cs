
using System;

namespace KillerAIMP.Services
{
    public struct InfoMp3
    {
        public string Artist;
        public string NameSong;
        public string Duration;
    }

    public delegate void VolumeHandler(int volume);
    public delegate void PositionHandler(int position, int duration);
    public delegate void CompletionHandler();
    
    
    public abstract class AAudio
    {
        public abstract event VolumeHandler VolumeEvent;
        public abstract event PositionHandler PositionEvent;
        public abstract event CompletionHandler ComplectionEvent;

        public abstract InfoMp3 GetInfo();
        public abstract void PlayAudioFile(string filePath);
        public abstract void Play();
        public abstract void Stop();
        public abstract void SetLooping(bool loop);
        public abstract void SetVolume(int volume);
        public abstract int GetMaxVolume();
        public abstract int GetVolume();
        public abstract void SetDataSource(string filePath);
        public abstract void SeekTo(int msec);
    }
}