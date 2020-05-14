using System;
using System.Threading.Tasks;
using Android.Media;
using AudioPlayer.Droid.Services;
using AudioPlayer.Services;
using Java.Lang;
using Xamarin.Forms;
using Context = Android.Content.Context;
using Stream = Android.Media.Stream;
using String = Java.Lang.String;

[assembly: Xamarin.Forms.Dependency(typeof(AudioService))]
namespace AudioPlayer.Droid.Services
{
    internal class AudioService : IAudio

    {
        private MediaPlayer _mediaPlayer;
        private readonly MediaMetadataRetriever _reader;

        private readonly AudioManager _audioManager;
        private readonly int _maxVolume;

        public override event VolumeHandler VolumeEvent;
        public override event PositionHandler PositionHandler;

        private InfoMP3 _infoMp3;

        public AudioService()
        {
            _reader = new MediaMetadataRetriever();

            if (Forms.Context != null)
                _audioManager = (AudioManager)Forms.Context.GetSystemService(Context.AudioService);

            _maxVolume = _audioManager.GetStreamMaxVolume(Stream.Music);
            
            CheckVolumeAsync();
            CheckPositionAsync();
        }

        private async void CheckVolumeAsync()
        {
            await Task.Run(() =>
            {
                var oldVolume = _audioManager.GetStreamVolume(Stream.Music);
                while (true)
                {
                    int newVolume;
                    if ((newVolume = _audioManager.GetStreamVolume(Stream.Music)) != oldVolume)
                    {
                        oldVolume = newVolume;
                        VolumeEvent?.Invoke(newVolume);
                    }
                }
            });
        }

        public async void CheckPositionAsync()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if (_mediaPlayer != null && _mediaPlayer.IsPlaying)
                    {
                        PositionHandler?.Invoke(_mediaPlayer.CurrentPosition, _mediaPlayer.Duration);
                    }
                }
            });
        }

        public override void SetDataSource(string filePath)
        {
            _reader.SetDataSource(filePath);

            if (_reader != null)
            {
                _infoMp3.Artist = _reader.ExtractMetadata(MetadataKey.Artist)
                                  ?? _reader.ExtractMetadata(MetadataKey.Author)
                                  ?? _reader.ExtractMetadata(MetadataKey.Composer)
                                  ?? "unknown artist";


                var lastFilePath = filePath.LastIndexOf("/", StringComparison.Ordinal);
                var name = filePath.Remove(0, lastFilePath + 1).Replace(".mp3", "");

                _infoMp3.NameSong = _reader.ExtractMetadata(MetadataKey.Title)
                                    ?? name;

                long dur = Long.ParseLong(_reader.ExtractMetadata(MetadataKey.Duration));
                var seconds = String.ValueOf((dur % 60000) / 1000);
                var minutes = String.ValueOf((dur / 60000));
                _infoMp3.Duration = minutes + ":" + seconds;
            }
        }

        public override void PlayAudioFile(string filePath)
        {
            SetDataSource(filePath);

            if (_mediaPlayer == null)
            {
                _mediaPlayer = new MediaPlayer();
                _mediaPlayer.Prepared += (s, e) => { _mediaPlayer.Start(); };
            }
            _mediaPlayer.Reset();
            _mediaPlayer.SetVolume(1.0f, 1.0f);

            _mediaPlayer.SetDataSource(filePath);
            _mediaPlayer.PrepareAsync();
        }

        public override void Play()
        {
            _mediaPlayer?.Start();
        }

        public override void Stop()
        {
            _mediaPlayer?.Pause();
        }

        public override void SetLooping(bool loop)
        {
            if (_mediaPlayer == null) return;
            _mediaPlayer.Looping = loop;
        }


        public override InfoMP3 GetInfo()
        {
            return _infoMp3;
        }

        public override void SetVolume(int volume)
        {
            if( volume <= _maxVolume)
                _audioManager.SetStreamVolume(Stream.Music, volume, 0);
        }


        public override int GetMaxVolume()
        {
            return _maxVolume;
        }

        public override int GetVolume()
        {
            return _audioManager.GetStreamVolume(Stream.Music);
        }
    }
}