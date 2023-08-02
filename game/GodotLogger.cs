using System;
using Godot;
using SolarisDIB.Cli.Util;

namespace SolarisViewer;

public class GodotLogger : ILogger {
    public Verbosity Verbosity { get; set; }
    public Logger BaseLogger => throw new NotImplementedException();

    public GodotLogger(Verbosity verbosity) {
        Verbosity = verbosity;
    }

    public void Trace(string log) {
        Log(log, Verbosity.Trace);
    }

    public void Info(string log) {
        Log(log, Verbosity.Information);
    }

    public void Warn(string log) {
        Log(log, Verbosity.Warning);
    }

    public void Err(string log) {
        Log(log, Verbosity.Error);
    }

    public void Crit(string log) {
        Log(log, Verbosity.Critical);
    }

    public void Log(string log, Verbosity verbosity) {
        if (verbosity < Verbosity) return;
        GD.Print(log);
    }
}