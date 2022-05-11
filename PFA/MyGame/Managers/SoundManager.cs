using PFA.GXPEngine.Utils;
using PFA.MyGame.Models;

namespace PFA.MyGame.Managers;

public static class SoundManager
{
	private static readonly ICollection<Sfx> HappyCats = new List<Sfx>();
	private static readonly ICollection<Sfx> SadCats = new List<Sfx>();
	private static readonly ICollection<Music> Background = new List<Music>();

	public enum BackgroundMusic
	{
		MmBegin,
		MmLoop,
		Pause,
	}

	static SoundManager()
	{
		foreach (string file in Directory.GetFiles("assets/sfx/cat/happy"))
		{
			HappyCats.Add(new Sfx(file));
		}

		foreach (string file in Directory.GetFiles("assets/sfx/cat/sad"))
		{
			SadCats.Add(new Sfx(file));
		}

		Background.Add(new Music("assets/bgm/mm_begin.mp3", false));
		Background.Add(new Music("assets/bgm/mm_loop.mp3", true));
		Background.Add(new Music("assets/bgm/pause.mp3", true));
	}

	public static void PlayHappyCat()
	{
		Sfx cat = HappyCats.ElementAt(Utils.Random(0, HappyCats.Count));
		cat.Play();
	}

	public static void PlaySadCat()
	{
		Sfx cat = SadCats.ElementAt(Utils.Random(0, SadCats.Count));
		cat.Play();
	}

	public static void PlayBackground(BackgroundMusic music)
	{
		Background.ElementAt((int)music).Play();
	}

	public static Music GetBackground(BackgroundMusic music)
	{
		return Background.ElementAt((int)music);
	}

	public static void PlayBackground(string id)
	{
		foreach (Music music in Background)
		{
			if (music.id != id) continue;
			music.Play();
			return;
		}

		Console.WriteLine("Music not found: " + id);
	}
}
