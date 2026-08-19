using System;
using Godot;

public class GameRunState : GameState
{
    private const string GAME_SCENE_PATH = "res://scenes/levels/test_level.tscn";

    public GameRunState(GameManager owner)
        : base(owner) { }

    private Node3D _level;

    public override void Enter(State previous = null)
    {
        PackedScene levelScene = GD.Load(GAME_SCENE_PATH) as PackedScene;
        _level = levelScene.Instantiate<Node3D>();
        _level.Ready += OnLevelLoaded;

        _manager.GetTree().ChangeSceneToNode(_level);
    }

    private void OnLevelLoaded() { }

    public override void Exit()
    {
        _level.Ready -= OnLevelLoaded;
    }
}
