using PFA.MyGame.Models;
using System.IO;

namespace PFA.MyGame.Managers;

public static class SceneManager
{
    private static Dictionary<string, Scene> Scenes = new();
    private static Scene _currentScene;

    static SceneManager()
    {
        try 
        {
            AddScenes();
        } catch(Exception except)
        {
            System.Console.WriteLine($"There has been a problem loading in scenes from [{Path.GetFullPath("./assets/scenes")}]");
            throw;
        }
    }

    private static void AddScenes()
    {
        string[] files = Directory.GetFiles("./assets/scenes");

        foreach(var file in files)
        {
            var scene = new Scene(file);
            Scenes.Add(scene.Id, scene);
        }
    }

    public static void ActivateScene(string scene)
    {
        if (_currentScene != null)
            MyGame.main.RemoveChild(_currentScene);
        
        if (!Scenes.ContainsKey(scene))
            throw new Exception($"Scene [{scene}] does not exist or could not be found on path [{Path.GetFullPath("./assets/scenes")}]");
        
        Scenes.TryGetValue(scene, out _currentScene);
        MyGame.main.AddChild(_currentScene);
    }

    public static void ShowScenes()
    {
        var tmp = $"[{Scenes.Count}] Scenes in memory: ";

        foreach(Scene scene in Scenes.Values)
        {
            tmp += $"{scene.Id}, ";
        }

        System.Console.WriteLine(tmp);
    }
}