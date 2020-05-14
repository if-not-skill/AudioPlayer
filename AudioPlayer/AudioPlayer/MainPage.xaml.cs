using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AudioPlayer.Services;
using AudioPlayer.ViewModels;
using Xamarin.Forms;

namespace AudioPlayer
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public List<Song> MySongs { get; set; }
        //public List<string> MyList { get; }
        public int MaxVolume { get; set; }

        private bool _play;
        private bool _repeat;
        private bool _rand;

        private readonly string _sourcePlay;
        private readonly string _sourcePause;

        private readonly string _sourceRepeat;
        private readonly string _sourceNRepeat;

        private readonly string _sourceRand;
        private readonly string _sourceNRand;

        private readonly AudioPlayerViewModel _player;

        private int _idCurrentSong;

        public MainPage()
        {   
            InitializeComponent();

            _player = new AudioPlayerViewModel(DependencyService.Get<IAudio>());

            MySongs = new List<Song>();
            AddSongsInMyListAsync();

            _player.SetLooping(_repeat);

            MaxVolume = _player.GetMaxVolume();
            Volume.Value = _player.GetVolume();

            _player.SubEventVolume(volume => { Volume.Value = volume; });
            _player.SubEventPosition((position, duration) =>
            {
                TrackDuration.Value = position;
                TrackDuration.Maximum = duration;
            });

            _sourcePlay = "play.png";
            _sourcePause = "pause.png";

            _sourceRand = "rand.png";
            _sourceNRand = "n_rand.png";

            _sourceRepeat = "repeat.png";
            _sourceNRepeat = "n_repeat.png";

            _play = true;
            _repeat = true;
            _rand = true;

            _idCurrentSong = 0;

            BindingContext = this;
        }

        private async void AddSongsInMyListAsync()
        {
            await Task.Run(() =>
            {
                var MyList = (List<string>)DependencyService.Get<IMyFile>().GetFileLocation();

                foreach (var el in MyList)
                {
                    _player.SetDataSource(el);
                    _player.UpdateInfo();

                    MySongs.Add(new Song(_player.GetNameSong(), _player.GetArtist(), _player.GetDuration(), el));
                }

                if (MyList.Count == 0) return;

                MyListSongs.ItemsSource = null;
                MyListSongs.ItemsSource = MySongs;
                MyListSongs.SelectedItem = MySongs[0];
                _player.Stop();
            });
        }

        private void Slider_OnValueChanged(object sender, ValueChangedEventArgs e)
        {
            _player.SetVolume((int)e.NewValue);
        }

        private void MyListSongs_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(e.SelectedItem is Song mySong)) return;

            _idCurrentSong = MySongs.IndexOf(mySong);

            _player.Play(mySong.Location);

            _play = false;
            Play.ImageSource = _sourcePause;

            NameSong.Text = mySong.Name;
            Artist.Text = mySong.Author;
            Duration.Text = mySong.Duration;

            //long dur = Long.ParseLong(_reader.ExtractMetadata(MetadataKey.Duration));
            //var seconds = String.ValueOf((dur % 60000) / 1000);
            //var minutes = String.ValueOf((dur / 60000));
            //_infoMp3.Duration = minutes + ":" + seconds;

            //var dur = mySong.Duration;
            //string[] sa = dur.Split(':');
            //int seconds = Convert.ToInt32(sa[0]) * 60000 * 60000 * 1000;
            //int minutes = Convert.ToInt32(sa[1]) * 60000;

        }

        private void Play_OnClicked(object sender, EventArgs e)
        {
            if (_play)
            {
                _player.Play();
                Play.ImageSource = _sourcePause;
            }
            else
            {
                _player.Stop();
                Play.ImageSource = _sourcePlay;
            }
                
            _play = !_play;
        }

        private void Rand_OnClicked(object sender, EventArgs e)
        {
            Rand.ImageSource = _rand ? _sourceRand : _sourceNRand;
            
            _rand = !_rand;
        }

        private void Repeat_OnClicked(object sender, EventArgs e)
        {
            Repeat.ImageSource = _repeat ? _sourceRepeat : _sourceNRepeat;
            _player.SetLooping(_repeat);
            
            _repeat = !_repeat;
        }

        private void Back_OnClicked(object sender, EventArgs e)
        {
            if (_rand)
            {
                if (--_idCurrentSong < 0)
                    _idCurrentSong = MySongs.Count - 1;
            }
            else
            {
                
            }

            _play = false;
            Play.ImageSource = _sourcePause;

            MyListSongs.SelectedItem = MySongs[_idCurrentSong];
        }

        private void Next_OnClicked(object sender, EventArgs e)
        {
            if (_rand)
            {
                if (++_idCurrentSong >= MySongs.Count)
                    _idCurrentSong = 0;

            }
            else
            {

            }

            _play = false;
            Play.ImageSource = _sourcePause;

            MyListSongs.SelectedItem = MySongs[_idCurrentSong];
        }
    }
}
