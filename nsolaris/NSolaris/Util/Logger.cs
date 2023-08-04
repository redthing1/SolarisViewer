namespace SolarisDIB.Cli.Util;

public enum Verbosity : long {
    Debug = 5,
    Trace = 4,
    Information = 3,
    Warning = 2,
    Error = 1,
    Critical = 0
}

public interface ILogger {
    Verbosity Verbosity { get; set; }
    Logger BaseLogger { get; }

    void Debug(string log);
    void Trace(string log);
    void Info(string log);
    void Warn(string log);
    void Err(string log);
    void Crit(string log);
}

public class LoggerFor<T> : ILogger {
    public Logger _logger;
    public Logger BaseLogger => _logger;

    public LoggerFor(Logger logger) {
        _logger = logger;
    }

    private string FormatWithContext(string log) {
        return $"[{typeof(T).Name}] {log}";
    }

    public Verbosity Verbosity {
        get => _logger.Verbosity;
        set => _logger.Verbosity = value;
    }

    public void Debug(string log) => _logger.Debug(FormatWithContext(log));
    public void Trace(string log) => _logger.Trace(FormatWithContext(log));
    public void Info(string log) => _logger.Info(FormatWithContext(log));
    public void Warn(string log) => _logger.Warn(FormatWithContext(log));
    public void Err(string log) => _logger.Err(FormatWithContext(log));
    public void Crit(string log) => _logger.Crit(FormatWithContext(log));
}

public class Logger : ILogger {
    public Verbosity Verbosity { get; set; }
    public List<ILogSink> Sinks = new List<ILogSink>();
    public Logger BaseLogger => this;

    public Logger(Verbosity verbosity) {
        Verbosity = verbosity;
    }

    public ILogger For<T>() {
        return new LoggerFor<T>(this);
    }

    public void WriteLine(string log, Verbosity level) {
        if (level <= Verbosity) {
            foreach (var sink in Sinks) {
                sink.WriteLine(log, level);
            }
        }
    }

    public void Debug(string log) => WriteLine(log, Verbosity.Debug);
    public void Trace(string log) => WriteLine(log, Verbosity.Trace);
    public void Info(string log) => WriteLine(log, Verbosity.Information);
    public void Warn(string log) => WriteLine(log, Verbosity.Warning);
    public void Err(string log) => WriteLine(log, Verbosity.Error);
    public void Crit(string log) => WriteLine(log, Verbosity.Critical);

    private static string ShortVerbosity(Verbosity level) {
        switch (level) {
            case Verbosity.Debug: return "dbug";
            case Verbosity.Trace: return "trce";
            case Verbosity.Information: return "info";
            case Verbosity.Warning: return "warn";
            case Verbosity.Error: return "err!";
            case Verbosity.Critical: return "crit";
            default: return level.ToString();
        }
    }

    private static string FormatMeta(Verbosity level) {
        // return $"[{ShortVerbosity(level)}/:{DateTime.Now:T}]";
        return $"[{ShortVerbosity(level)}/:{DateTime.Now:hh:mm:ss}]";
    }

    public interface ILogSink {
        void WriteLine(string log, Verbosity level);
    }

    public class ConsoleSink : ILogSink {
        public ConsoleSink(ConsoleStream stream = ConsoleStream.Stdout) {
            OutputStream = stream switch {
                ConsoleStream.Stderr => Console.Error,
                _ => Console.Out,
            };
        }

        public enum ConsoleStream {
            Stdout,
            Stderr
        }

        public TextWriter OutputStream { get; }

        public void WriteLine(string log, Verbosity level) {
            Console.ResetColor();
            var col = ColorFor(level);
            Console.ForegroundColor = col;
            Console.BackgroundColor = ConsoleColor.Black;
            OutputStream.Write(FormatMeta(level));
            Console.ResetColor();
            OutputStream.WriteLine($" {log}");
        }

        private ConsoleColor ColorFor(Verbosity level) {
            switch (level) {
                case Verbosity.Debug: return ConsoleColor.DarkGray;
                case Verbosity.Trace: return ConsoleColor.Gray;
                case Verbosity.Information: return ConsoleColor.Green;
                case Verbosity.Warning: return ConsoleColor.Yellow;
                case Verbosity.Error: return ConsoleColor.Red;
                case Verbosity.Critical: return ConsoleColor.DarkRed;
                default: return ConsoleColor.White;
            }
        }
    }

    public class FileSink : ILogSink, IDisposable {
        public string path;
        private StreamWriter _sw;

        public FileSink(string path) {
            this.path = path;
            this._sw = new StreamWriter(File.Open(path, FileMode.Append, FileAccess.Write));
        }

        public void WriteLine(string log, Verbosity level) {
            _sw.Write(FormatMeta(level));
            _sw.WriteLine($" {log}");
            _sw.Flush();
        }

        public void Dispose() {
            _sw.Dispose();
        }
    }
}