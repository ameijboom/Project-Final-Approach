using PFA.MyGame.Models;
using PFA.GXPEngine;

namespace PFA.MyGame.Managers;

public static class SceneManager
{
	private static readonly Dictionary<string, Scene> Scenes = new();
	private static Scene _currentScene;

	static SceneManager()
	{
		try
		{
			AddScenes();
		} catch(Exception)
		{
			Console.WriteLine($"There has been a problem loading in scenes from [{Path.GetFullPath("./assets/scenes")}]");
			throw;
		}
	}

	private static void AddScenes()
	{
		string[] files = Directory.GetFiles("./assets/scenes");

		foreach(string file in files)
		{
			Scene scene = new(file);
			Scenes.Add(scene.Id, scene);
		}
	}

	public static void ActivateScene(string scene)
	{
		if (_currentScene != null)
			Game.main.RemoveChild(_currentScene);

		if (!Scenes.ContainsKey(scene))
			throw new Exception($"Scene [{scene}] does not exist or could not be found on path [{Path.GetFullPath("./assets/scenes")}]");

		Scenes.TryGetValue(scene, out _currentScene);

		_currentScene.Init();
		Game.main.AddChild(_currentScene);
	}

	public static void ShowScenes()
	{
		string tmp = $"[{Scenes.Count}] Scenes in memory: ";

		foreach(Scene scene in Scenes.Values)
		{
			tmp += $"{scene.Id}, ";
		}

		Console.WriteLine(tmp);
	}
}
