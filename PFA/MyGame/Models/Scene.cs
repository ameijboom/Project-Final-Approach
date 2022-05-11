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
        Id = path.Replace(".scene.tmx", "");

        if (Id.Contains("\\"))
            Id = path.Replace("./assets/scenes\\", "");
        
        Id = path.Replace("./assets/scenes/", "");
    }
    
    public void Init()
    {
        _loader = new TiledLoader(Path);
    }

    private void CreateLevel()
    {
        _loader.autoInstance = true;
        _loader.LoadObjectGroups();
    }
}