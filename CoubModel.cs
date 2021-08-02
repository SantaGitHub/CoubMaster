using System.Collections.Generic;

namespace JewishCat.DiscordBot
{
    public class Higher
    {
        public string url { get; set; }
        public int size { get; set; }
    }

    public class High
    {
        public string url { get; set; }
        public int size { get; set; }
    }

    public class Med
    {
        public string url { get; set; }
        public int size { get; set; }
    }

    public class Video
    {
        public Higher higher { get; set; }
        public High high { get; set; }
        public Med med { get; set; }
    }

    public class Audio
    {
        public High high { get; set; }
        public Med med { get; set; }
        public double sample_duration { get; set; }
    }

    public class Html5
    {
        public Video video { get; set; }
        public Audio audio { get; set; }
    }

    public class Mobile
    {
        public string video { get; set; }
        public List<string> audio { get; set; }
    }

    public class Share
    {
        public string @default { get; set; }
    }

    public class FileVersions
    {
        public Html5 html5 { get; set; }
        public Mobile mobile { get; set; }
        public Share share { get; set; }
    }

    public class CoubModel
    {
        public FileVersions file_versions { get; set; }
        public List<Tags> tags { get; set; }
        public List<Communities> communities { get; set; }
    }
    
    public class Tags
    {
        public long id { get; set; }
        public string title { get; set; }
    }    
    public class Communities
    {
        public long id { get; set; }
        public string title { get; set; }
    }
}