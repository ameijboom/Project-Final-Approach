namespace PFA.MyGame.Models;

public class Music : Sfx
{
	public string id { get; }

	public Music(string path, bool looping = true) : base(path, looping)
	{
		string fileName = System.IO.Path.GetFileName(path);
		string[] strings = fileName.Split('.');
		id = strings[0];
	}
}
