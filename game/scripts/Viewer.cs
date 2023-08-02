using Godot;
using System;
using SolarisDIB.Cli.Helpers;
using SolarisDIB.Cli.Util;
using SolarisViewer;

public partial class Viewer : Control {
    private GodotLogger _logger;
    private GameLoadHelper.LoadedGame _loadedGame;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        var args = OS.GetCmdlineUserArgs();
        if (args.Length < 1) {
            GD.PrintErr("Usage: pass <gameId> as first argument");
            GetTree().Quit();
        }

        var gameId = args[0];
        GD.Print($"Creating viewer for game {gameId}");

        _logger = new GodotLogger(Verbosity.Trace);

        _loadedGame = GameLoadHelper.LoadOfflineGameDataInteractive(_logger, gameId).Result;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}