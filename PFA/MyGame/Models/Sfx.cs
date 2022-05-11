using PFA.GXPEngine;

namespace PFA.MyGame.Models;

public class Sfx
{
    public string Path { get; set; }
    public Sound Sound { get; set; }
    public float Volume { get; set; } = 0.3f;

    public Sfx(string path, bool looping = false)
    {
        Sound = new Sound(path, looping);
    }

    public SoundChannel Play()
    {
        return Sound.Play(volume: Volume);
    }

    public void ChangeVolume(float vol)
    {
        if (vol < 0f || vol > 1f)
            System.Console.WriteLine("Volume must be between 0f and 1f");
        
        Volume = vol;
    }
}