using PFA.GXPEngine;
using PFA.GXPEngine.AddOns;

namespace PFA.MyGame.Models;

public class Scene : GameObject
{
    private TiledLoader _loader;
    public string Id { get; set; }
    public string Path { get; set; }

	public Scene(string path)
	{
		Path = path;
		string fileName = System.IO.Path.GetFileName(Path);
		string[] strings = fileName.Split('.');
		Id = strings[0];
	}

	public void Init()
	{
		_loader = new TiledLoader(Path);

		CreateLevel();
	}

    private void CreateLevel()
    {
        _loader.autoInstance = true;
        _loader.LoadObjectGroups();
    }
}