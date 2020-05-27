namespace KillerAIMP.ViewModels
{
    public class Song
    {
        public Song(string name, string author, string duration, string location)
        {
            Name = name;
            Author = author;
            Duration = duration;
            Location = location;
        }

        public Song(Song newSong)
        {
            Name = newSong.Name;
            Author = newSong.Author;
            Duration = newSong.Duration;
            Location = newSong.Location;
        }

        public string Name { get; set; }
        public string Author { get; set; }
        public string Duration { get; set; }
        public string Location { get; set; }
    }
}