namespace PFA.MyGame.Models;

public class Music : Sfx
{
    public string Id { get; set; }

    public Music(string path, bool looping = true) : base(path, looping)
    {
        Id = path.Replace(".scenes.tmx", "");
        Id = Id.Replace("./assets/bgm", "");
    }
}