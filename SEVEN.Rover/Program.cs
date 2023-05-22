using SEVEN.Rover.Core.Clients;
using SEVEN.Rover.Core.Constants;

namespace SEVEN.Rover
{
    internal class Program
    {
        public static List<Option> options;
        private static RoverClient _roverClient;
        static async Task Main(string[] args)
        {
            _roverClient = new RoverClient("http://192.168.178.37/");

            // Create options that you want your menu to have
            options = new List<Option>
            {
                new Option("Headlights ON", async() => await _roverClient.TurnHeadlights_On()),
                new Option("Headlights OFF", async() => await _roverClient.TurnHeadlights_Off()),
                new Option("Headlights Status", async() => WriteTemporaryMessage(RoverStatusNames.STATUS_HEADLIGHTS +":" + await _roverClient.GetHeadlights_Status())),
                new Option("Exit", () => Environment.Exit(0)),
            };

            // Set the default index of the selected item to be the first
            int index = 0;

            // Write the menu out
            WriteMenu(options, options[index]);

            // Store key info in here
            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();

                // Handle each key input (down arrow will write the menu again with a different selected item)
                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (index + 1 < options.Count)
                    {
                        index++;
                        WriteMenu(options, options[index]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (index - 1 >= 0)
                    {
                        index--;
                        WriteMenu(options, options[index]);
                    }
                }
                // Handle different action for the option
                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    options[index].Selected.Invoke();
                    index = 0;
                }
            }
            while (keyinfo.Key != ConsoleKey.X);

            Console.ReadKey();

        }
        // Default action of all the options. You can create more methods
        static void WriteTemporaryMessage(string message)
        {
            Console.Clear();
            Console.WriteLine(message);
            Thread.Sleep(2000);
            WriteMenu(options, options.First());
        }



        static void WriteMenu(List<Option> options, Option selectedOption)
        {
            Console.Clear();
            DrawHeader();
            foreach (Option option in options)
            {
                if (option == selectedOption)
                {
                    Console.Write("> ");
                }
                else
                {
                    Console.Write(" ");
                }

                Console.WriteLine(option.Name);
            }
        }

        static void DrawHeader()
        {
            Console.WriteLine("\t\t_______ _______ _    _ _______ __   _ ");
            Console.WriteLine("\t\t|______ |______  \\  /  |______ | \\  |");
            Console.WriteLine("\t\t______| |______   \\/   |______ |  \\_|");
            Console.WriteLine("\t\t\t\t\t__________                          ");
            Console.WriteLine("\t\t\t\t\t\\______   \\ _______  __ ___________ ");
            Console.WriteLine("\t\t\t\t\t |       _//  _ \\  \\/ // __ \\_  __ \\");
            Console.WriteLine("\t\t\t\t\t |    |   (  <_> )   /\\  ___/|  | \\/");
            Console.WriteLine("\t\t\t\t\t |____|_  /\\____/ \\_/  \\___  >__|   ");
            Console.WriteLine("\t\t\t\t\t        \\/                 \\/       ");
            Console.WriteLine();
            Console.WriteLine("+-+-+-+-+-+-+-+-+ +-+-+-+-+-+-+-+-+ +-+-+-+-+-+-+-+ +-+-+-+-+-+-+-+ +-+-+-+-+-+-+-+");
            Console.WriteLine("|S|a|n|d|b|e|r|g| |E|l|e|c|t|r|i|c| |V|e|h|i|c|l|e| |E|d|e|n|       |N|e|t|w|o|r|k|");
            Console.WriteLine("+-+-+-+-+-+-+-+-+ +-+-+-+-+-+-+-+-+ +-+-+-+-+-+-+-+ +-+-+-+-+-+-+-+ +-+-+-+-+-+-+-+");
        }
    }

    public class Option
    {
        public string Name { get; }
        public Action Selected { get; }

        public Option(string name, Action selected)
        {
            Name = name;
            Selected = selected;
        }
    }
}