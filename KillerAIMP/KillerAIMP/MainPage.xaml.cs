using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using KillerAIMP.Services;
using KillerAIMP.ViewModels;
using Xamarin.Forms;


namespace KillerAIMP
{
    public partial class MainPage : ContentPage
    {
        private List<Song> MySongs { get; set; }
        private List<Song> _correctList;
        
        private List<string> MyList { get; }
        
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

        private Random _random;

        public MainPage()
        {
            InitializeComponent();
            
            MyList = DependencyService.Get<IMyFile>().GetFileLocation() as List<string>;
           
            _player = new AudioPlayerViewModel(DependencyService.Get<AAudio>());
            
            _play = true;
            _repeat = true;
            _rand = true;

            _player.SetLooping(_repeat);
            
            Volume.Maximum = _player.GetMaxVolume();
            Volume.Value = _player.GetVolume();

            _player.SubEventVolume(SetVolume);
            _player.SubEventPosition(SetDuration);
            
            MySongs = new List<Song>();
            _correctList = new List<Song>();
            AddSongsInMyListAsync();
            
            _sourcePlay = "play.png";
            _sourcePause = "pause.png";
            
            _sourceRand = "rand.png";
            _sourceNRand = "n_rand.png";
            
            _sourceRepeat = "repeat.png";
            _sourceNRepeat = "n_repeat.png";
            
            _idCurrentSong = 0;

            _random = new Random();
            
            BindingContext = this;
        }

        private void SetVolume(int volume)
        {
            Volume.Value = volume;
        }

        private void SetDuration(int position, int duration)
        {
            TrackDuration.Value = position;
            TrackDuration.Maximum = duration;

            // CurrentPosition.Text = (position / 60000) + ":" + (position / 1000 % 60);
        }

        private void AddSongsInMyListAsync()
        {
            foreach (var el in MyList)
            {
                _player.SetDataSource(el);
                _player.UpdateInfo();

                MySongs.Add(new Song(_player.GetNameSong(), _player.GetArtist(), _player.GetDuration(), el));
            }

            MyListSongs.ItemsSource = null;
            MyListSongs.ItemsSource = MySongs;

            _correctList.AddRange(MySongs);
        }

        private void MyListSongs_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(e.SelectedItem is Song mySong)) return;
        
            _idCurrentSong = MySongs.IndexOf(mySong);
        
            _player.Play(mySong.Location);
            CurrentPosition.Text = "0:00";

            _play = false;
            Play.ImageSource = _sourcePause;
        
            NameSong.Text = mySong.Name;
            Artist.Text = mySong.Author;
            Duration.Text = mySong.Duration;
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

            if (_rand)
            {
                for (var i = MySongs.Count - 1; i >= 1; i--)
                {
                    var j = _random.Next(i + 1);
                    var temp = MySongs[j];
                    MySongs[j] = MySongs[i];
                    MySongs[i] = temp;
                }
            }
            else
            {
                MySongs.Clear(); 
                MySongs.AddRange(_correctList);
            }
            
            MyListSongs.ItemsSource = null;
            MyListSongs.ItemsSource = MySongs;
        }
        
        private void Repeat_OnClicked(object sender, EventArgs e)
        {
            Repeat.ImageSource = _repeat ? _sourceRepeat : _sourceNRepeat;
            _player.SetLooping(_repeat);
            
            _repeat = !_repeat;
        }
        
        private void Back_OnClicked(object sender, EventArgs e)
        {
            if (--_idCurrentSong < 0)
                    _idCurrentSong = MySongs.Count - 1;

            _play = false;
            Play.ImageSource = _sourcePause;
        
            MyListSongs.SelectedItem = MySongs[_idCurrentSong];
        }
        
        private void Next_OnClicked(object sender, EventArgs e)
        {
            if (++_idCurrentSong >= MySongs.Count)
                _idCurrentSong = 0;

            _play = false;
            Play.ImageSource = _sourcePause;
        
            MyListSongs.SelectedItem = MySongs[_idCurrentSong];
        }

        private void Volume_OnDragStarted(object sender, EventArgs e)
        {
            _player.UnsubEventVolume(SetVolume);
        }

        private void Volume_OnDragCompleted(object sender, EventArgs e)
        {
            _player.SubEventVolume(SetVolume);
        }
        
        private void Volume_OnValueChanged(object sender, ValueChangedEventArgs e)
        {
            _player.SetVolume((int)Volume.Value);
        }
    }
}