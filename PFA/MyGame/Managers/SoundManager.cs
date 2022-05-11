using PFA.MyGame.Models;

namespace PFA.MyGame.Managers;

public static class SoundManager
{
    private static ICollection<Sfx> _happyCats = new List<Sfx>();
    private static ICollection<Sfx> _sadCats = new List<Sfx>();
    private static ICollection<Music> Songs = new List<Music>();
    
    static SoundManager()
    {
        
    }
}