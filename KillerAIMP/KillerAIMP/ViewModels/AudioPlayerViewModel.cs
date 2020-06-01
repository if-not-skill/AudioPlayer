using System;
using KillerAIMP.Services;

namespace KillerAIMP.ViewModels
{
    public class AudioPlayerViewModel
    {
        private readonly AAudio _audioPlayer;
        private InfoMp3 _info;

        public AudioPlayerViewModel(AAudio audioPlayer)
        {
            _audioPlayer = audioPlayer;
        }

        public void SubEventVolume(VolumeHandler dl)
        {
            _audioPlayer.VolumeEvent += dl;
        }

        public void UnsubEventVolume(VolumeHandler dl)
        {
            _audioPlayer.VolumeEvent -= dl;
        }
        public void SubEventPosition(PositionHandler dl)
        {
            _audioPlayer.PositionEvent += dl;
        }

        public void UnsubEventPosition(PositionHandler dl)
        {
            _audioPlayer.PositionEvent -= dl;
        }

        public void SeekTo(int msec)
        {
            _audioPlayer.SeekTo(msec);
        }

        public void Play(string filePath)
        {
            _audioPlayer.PlayAudioFile(filePath);
        }

        public void SetDataSource(string filePath)
        {
            _audioPlayer.SetDataSource(filePath);
        }

        public void UpdateInfo()
        {
            _info = _audioPlayer.GetInfo();
        }

        public string GetArtist()
        {
            return _info.Artist;
        }

        public string GetNameSong()
        {
            return _info.NameSong;
        }
        public string GetDuration()
        {
            return _info.Duration;
        }


        public void SetVolume(int volume)
        {
            _audioPlayer.SetVolume(volume);
        }

        public int GetMaxVolume()
        {
            return _audioPlayer.GetMaxVolume();
        }

        public int GetVolume()
        {
            return _audioPlayer.GetVolume();
        }

        public void Play()
        {
            _audioPlayer.Play();
        }

        public void Stop()
        {
            _audioPlayer.Stop();
        }

        public void SubCompletion(CompletionHandler dl)
        {
            _audioPlayer.ComplectionEvent += dl;
        }
    }
}