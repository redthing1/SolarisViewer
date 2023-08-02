using System.IO;
using System.Threading.Tasks;
using CommandLine;
using Godot;
using NSolaris.Client;
using SolarisDIB.Cli.Helpers;
using SolarisDIB.Cli.Util;

public partial class Viewer : Control {
	private Logger _logger;
	private GameLoadHelper.LoadedGame _loadedGame;

	public static string CacheDir => Path.Join(OS.GetUserDataDir(), "client_cache");

	public class CliOptions {
		[Value(0, MetaName = "gameQuery", Required = true, HelpText = "Game name/id or path to dump")]
		public string GameQuery { get; set; } = null!;

		[Option('e', "email", Required = false, HelpText = "Email address to use for login")]
		public string? Email { get; set; }

		[Option('p', "password", Required = false, HelpText = "Password to use for login")]
		public string? Password { get; set; }

		[Option('a', "api", Required = false, HelpText = "API URL to use", Default = "https://api.solaris.games")]
		public string ApiUrl { get; set; } = null!;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		var args = OS.GetCmdlineUserArgs();

		Parser.Default.ParseArguments<CliOptions>(args).WithParsed(async cliOpts => await ReadyWithOpts(cliOpts))
			.WithNotParsed(
				errs => {
					GD.PrintErr($"failed to parse command line arguments:");
					foreach (var err in errs) {
						_logger.Err(err?.ToString() ?? string.Empty);
					}
				});
	}

	private async Task ReadyWithOpts(CliOptions cliOpts) {
		// _logger = new GodotLogger(Verbosity.Trace);
		_logger = new Logger(Verbosity.Trace);
		_logger.Sinks.Add(new Logger.ConsoleSink());

		// see if the game id exists
		if (Directory.Exists(cliOpts.GameQuery)) {
			_logger.Info($"game id {cliOpts.GameQuery} is a directory, assuming it's a dump");
			// load the game data directly
			_loadedGame = await GameLoadHelper.LoadOfflineGameDataInteractive(_logger, cliOpts.GameQuery);
		} else {
			// ensure email and password are provided
			if (cliOpts.Email == null || cliOpts.Password == null) {
				_logger.Err("email and password are required for loading online games");
				return;
			}

			_logger.Info($"searching for an online game matching {cliOpts.GameQuery}");

			// assume the query is for an online game
			var client = new SolarisClient(_logger.BaseLogger, cliOpts.ApiUrl);
			await client.Login(cliOpts.Email!, cliOpts.Password!);

			_logger.Info($"finding game matching {cliOpts.GameQuery}");
			var gameInfo = await client.FindGame(cliOpts.GameQuery);
			if (gameInfo == null) {
				_logger.Err($"no game matching {cliOpts.GameQuery} found");
				return;
			}

			client.LoadCache(CacheDir);
			_loadedGame = await GameLoadHelper.LoadOnlineGameDataInteractive(_logger, client, gameInfo);
			client.SaveCache(CacheDir);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) { }
}
