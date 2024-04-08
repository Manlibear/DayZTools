using Spectre.Console;
namespace DayZTools
{

    public static class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText("R&R DayZ Tools").LeftJustified().Color(Color.Red));
                AnsiConsole.Write(new Rule("v0.1")
                {
                    Justification = Justify.Left
                });

                AnsiConsole.WriteLine();

                var menuOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .AddChoices([
                        MenuOptions.TRADER_CLASSES,
                        MenuOptions.EXIT
                    ])
                );

                switch (menuOption)
                {
                    case MenuOptions.TRADER_CLASSES:

                        bool success = true;
                        try
                        {
                            var fileName = AnsiConsole.Ask<string>("Drag and Drop the file, then hit enter. (either XLSX -> JSON or JSON -> XLSX)").Replace("\"", "");
                            var ext = Path.GetExtension(fileName);
                            switch (ext)
                            {
                                case ".json":
                                    DoWork(() => { Functions.TraderClasses.JsonToXLSX(fileName); });
                                    break;

                                case ".xlsx":
                                    DoWork(() => { Functions.TraderClasses.XLSXToJson(fileName); });
                                    break;

                                default:
                                    LogError("Invalid file format", "Only JSON or XLSX files are allowed");
                                    success = false;
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            LogError(ex.Message, "Check the file is valid and an output file with the same name isn't open in another program");
                            success = false;
                        }

                        if (success)
                        {
                            LogSuccess("Success!", "An output file has been created in the same directory");
                        }

                        break;

                    case MenuOptions.EXIT:
                        return;
                }
            }
        }
        public static void DoWork(Action work)
        {
            AnsiConsole.Status()
                .Start("Processing...", ctx =>
                {
                    ctx.Spinner(Spinner.Known.Star);
                    ctx.SpinnerStyle(Style.Parse("green"));
                    work();
                });
        }

        public static void LogError(string title, string message)
        {
            AnsiConsole.Markup($"\n[red](E) {title}[/]\n");
            AnsiConsole.WriteLine(message);
            Console.ReadKey();
        }

        public static void LogSuccess(string title, string message)
        {
            AnsiConsole.Markup($"\n[green]{title}![/]\n");
            AnsiConsole.Write(message);
            Console.ReadKey();
        }
    }

    public class MenuOptions
    {
        public const string TRADER_CLASSES = "Convert Trader Classes";
        public const string EXIT = "Exit";
    }
}