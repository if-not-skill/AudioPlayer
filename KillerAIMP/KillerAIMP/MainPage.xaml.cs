using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using KillerAIMP.Services;
using KillerAIMP.ViewModels;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Xamarin.Forms;


namespace KillerAIMP
{
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        private List<Song> MySongs { get; }
        private readonly List<Song> _correctList;
        
        private List<string> MyList { get; }

        private string _currentText;

        public string CurrentText
        {
            get => _currentText;

            set
            {
                _currentText = value;
                OnPropertyChanged();
            }
        }

        private bool _bPlay;
        private bool _bRepeat;
        private bool _bRand;
        private bool _bWork;
        private bool _bUserPressed;

        private readonly string _sourcePlay;
        private readonly string _sourcePause;

        private readonly string _sourceRepeat;
        private readonly string _sourceNRepeat;

        private readonly string _sourceRand;
        private readonly string _sourceNRand;

        private readonly AudioPlayerViewModel _player;

        private int _idCurrentSong;

        private readonly Random _random;
        
        public new event PropertyChangedEventHandler PropertyChanged;

        protected override void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainPage()
        {
            InitializeComponent();
            
            MyList = DependencyService.Get<IMyFile>().GetFileLocation() as List<string>;
           
            _player = new AudioPlayerViewModel(DependencyService.Get<AAudio>());

            _bPlay = true;
            _bWork = true;
            _bUserPressed = false;

            Volume.Maximum = _player.GetMaxVolume();
            Volume.Value = _player.GetVolume();

            _player.SubEventVolume(SetVolume);
            _player.SubEventPosition(SetDuration);
            _player.SubCompletion(TrackCompletion);
            
            MySongs = new List<Song>();
            _correctList = new List<Song>();
            AddSongsInMyList();
            
            _sourcePlay = "play.png";
            _sourcePause = "pause.png";
            
            _sourceRand = "rand.png";
            _sourceNRand = "n_rand.png";
            
            _sourceRepeat = "repeat.png";
            _sourceNRepeat = "n_repeat.png";
            
            CurrentText = "0:0";
            
            _idCurrentSong = 0;
            
            _random = new Random();
            
            // check property bRepeat
            if (Application.Current.Properties.ContainsKey("bRepeat"))
            {
                _bRepeat = (bool) Application.Current.Properties["bRepeat"];
            }
            else
            {
                _bRepeat = false;
                Application.Current.Properties["bRepeat"] = _bRepeat;
            }
            Repeat.Source = _bRepeat ? _sourceNRepeat : _sourceRepeat;

            // check property bRand
            if (Application.Current.Properties.ContainsKey("bRand"))
            {
                _bRand = (bool) Application.Current.Properties["bRand"];
                MixListOrNot();
            }
            else
            {
                _bRand = false;
                Application.Current.Properties["bRand"] = _bRand;
            }
            Rand.Source = _bRand ? _sourceNRand : _sourceRand ;
            
            BindingContext = this;
        }

        private void TrackCompletion()
        {
            if (!_bRepeat)
            {
                _player.Play(MySongs[_idCurrentSong].Location);
            }
            else
            {
                Next_OnClicked(this, EventArgs.Empty);
            }
        }

        private void SetVolume(int volume)
        {
            Volume.Value = volume;
        }

        private void SetDuration(int position, int duration)
        {
            TrackDuration.Maximum = duration;
            TrackDuration.Value = position;

            CurrentText = (position / 60000) + ":" + (position / 1000 % 60);
        }

        private void AddSongsInMyList()
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

            if (!_bWork)
            {
                _bWork = true;
                return;
            }

            _idCurrentSong = MySongs.IndexOf(mySong);

            _player.Play(mySong.Location);

            _bPlay = false;
            Play.Source = _sourcePause;
        
            NameSong.Text = mySong.Name;
            Artist.Text = mySong.Author;
            Duration.Text = mySong.Duration;
        }
        
        private void Play_OnClicked(object sender, EventArgs e)
        {
            if (_bPlay)
            {
                _player.Play();
                Play.Source = _sourcePause;
            }
            else
            {
                _player.Stop();
                Play.Source = _sourcePlay;
            }
                
            _bPlay = !_bPlay;
        }
        
        private void Rand_OnClicked(object sender, EventArgs e)
        {
            Rand.Source = _bRand ? _sourceRand : _sourceNRand;

            _bRand = !_bRand;
            Application.Current.Properties["bRand"] = _bRand;
            
            _bWork = false;
            
            MixListOrNot();

            MyListSongs.ItemsSource = null;
            MyListSongs.ItemsSource = MySongs;

            MyListSongs.SelectedItem = null;
            MyListSongs.SelectedItem = MySongs[_idCurrentSong];
        }

        private void MixListOrNot()
        {
            var currentSong = MySongs[_idCurrentSong];
            
            if (!_bRand)
            {

                Song temp;
                for (var i = MySongs.Count - 1; i >= 1; i--)
                {
                    var j = _random.Next(i + 1);
                    temp = MySongs[j];
                    MySongs[j] = MySongs[i];
                    MySongs[i] = temp;
                }
                
                temp = MySongs[0];
                MySongs[MySongs.IndexOf(currentSong)] = temp;
                MySongs[0] = currentSong;
                _idCurrentSong = 0;

            }
            else
            {
                MySongs.Clear(); 
                MySongs.AddRange(_correctList);
                
                _idCurrentSong = MySongs.IndexOf(currentSong);
            }
        }
        
        private void Repeat_OnClicked(object sender, EventArgs e)
        {
            Repeat.Source = _bRepeat ? _sourceRepeat : _sourceNRepeat;
            
            _bRepeat = !_bRepeat;
            Application.Current.Properties["bRepeat"] = _bRepeat;
            
        }
        
        private void Back_OnClicked(object sender, EventArgs e)
        {
            if (--_idCurrentSong < 0)
                    _idCurrentSong = MySongs.Count - 1;

            _bPlay = false;
            Play.Source = _sourcePause;
        
            MyListSongs.SelectedItem = MySongs[_idCurrentSong];
        }
        
        private void Next_OnClicked(object sender, EventArgs e)
        {
            if (++_idCurrentSong >= MySongs.Count)
                _idCurrentSong = 0;

            _bPlay = false;
            Play.Source = _sourcePause;
        
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

        private void TrackDuration_OnDragStarted(object sender, EventArgs e)
        {            
            _bUserPressed = true;
            _player.UnsubEventPosition(SetDuration);
        }
        
        private void TrackDuration_OnValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (_bUserPressed)
            {
                _player.SeekTo((int) e.NewValue);
            }
        }

        private void TrackDuration_OnDragCompleted(object sender, EventArgs e)
        {
            _bUserPressed = false;
            _player.SubEventPosition(SetDuration);
        }
    }
}