using PFA.GXPEngine;
using PFA.GXPEngine.Utils;

namespace PFA.MyGame.Models;

public class Sfx
{
	private float volume { get; set; } = 0.3f;
	private readonly Sound _sound;
	private SoundChannel _channel;

	public Sfx(string path, bool looping = false)
	{
		_sound = new Sound(path, looping);
		_channel = _sound.Play(volume: volume, paused: true);
	}

	public void Play()
	{
		_channel = _sound.Play(volume: volume, paused: false);
	}

	public bool IsPlaying()
	{
		return _channel.IsPlaying;
	}

	/// <param name="vol">Range: 0..1</param>
	public void ChangeVolume(float vol)
	{
		volume = Mathf.Clamp(vol, 0f, 1f);
	}
}
