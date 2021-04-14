namespace JewishCat.DiscordBot.Models
{
    public class User
    {
        public User() {}
        public User(ulong id, string name)
        {
            Id = id;
            Name = name;
        }

        public ulong Id { get; }
        public string Name { get; }
        public bool Ignore { get; private set; }

        public void ResetIgnore()
        {
            Ignore = false;
        }

        public void SetIgnore()
        {
            Ignore = true;
        }
    }
}