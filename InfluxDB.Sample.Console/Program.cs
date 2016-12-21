using Tharga.Toolkit.Console;
using Tharga.Toolkit.Console.Command;
using Tharga.Toolkit.Console.Command.Base;

namespace InfluxDB.Sample.Console
{
    static class Program
    {
        static void Main(string[] args)
        {
            var console = new ClientConsole();
            var rootCommand = new RootCommand(console);
            rootCommand.RegisterCommand(new PingCommand());
            var engine = new CommandEngine(rootCommand);
            engine.Run(args);
        }
    }
}
