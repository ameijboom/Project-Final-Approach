using PFA.GXPEngine.Utils;
using PFA.MyGame.Models;

namespace PFA.MyGame.Managers;

public static class SoundManager
{
	private static readonly ICollection<Snd> HappyCats = new List<Snd>();
	private static readonly ICollection<Snd> SadCats = new List<Snd>();
	private static readonly ICollection<Snd> Background = new List<Snd>();

	public enum BGM
	{
		MmBegin,
		MmLoop,
		Pause,
		Game,
	}

	static SoundManager()
	{
		foreach (string file in Directory.GetFiles("assets/sfx/cat/happy"))
		{
			HappyCats.Add(new Snd(file));
		}

		foreach (string file in Directory.GetFiles("assets/sfx/cat/sad"))
		{
			SadCats.Add(new Snd(file));
		}

		Background.Add(new Snd("assets/bgm/mm_begin.wav", false, true));
		Background.Add(new Snd("assets/bgm/mm_loop.wav", true, true));
		Background.Add(new Snd("assets/bgm/pause.wav", true, false));
		Background.Add(new Snd("assets/bgm/game.wav", false, true));
	}

	public static void PlayHappyCat()
	{
		Snd cat = HappyCats.ElementAt(Utils.Random(0, HappyCats.Count));
		cat.Play();
	}

	public static void PlaySadCat()
	{
		Snd cat = SadCats.ElementAt(Utils.Random(0, SadCats.Count));
		cat.Play();
	}

	public static void PlayBackground(BGM music)
	{
		Background.ElementAt((int)music).Play();
	}

	public static Snd GetBackground(BGM music)
	{
		return Background.ElementAt((int)music);
	}
}
