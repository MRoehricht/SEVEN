using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SEVEN.Core.API.Client;
using SEVEN.Core.Models;
using SEVEN.Core.Models.Configuration;
using SEVEN.Rover.Core.Clients;

namespace SEVEN.Rover
{
    internal class Program
    {
        public static List<Option> _options = new();
        private static RoverClient? _roverClient;
        static async Task Main()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();
            var roverConnection = config["RoverConnection"];

            if (string.IsNullOrWhiteSpace(roverConnection))
            {
                Console.WriteLine("RoverConnection ist nicht vergeben!");
                return;
            }
            RoverConnection connection = new() { RoverUrl = roverConnection };
            IOptions<RoverConnection> roverOptions = Options.Create(connection);

            APIConnection apiConnection = new() { BaseUrl = "https://localhost:7272/" };
            IOptions<APIConnection> apiOptions = Options.Create(apiConnection);


            var apiClient = new APIClient(apiOptions);


            _roverClient = new RoverClient(roverOptions);

            // Create _options that you want your menu to have
            _options = new List<Option>
            {
                //new Option("Status", async() => WriteTemporaryMessage(RoverStatusNames.STATUS_HEADLIGHTS +":" + await _roverClient.GetHeadlights_Status())),
                //new Option("Systemcheck", () => WriteTemporaryMessage("Run SystemCheck")),
                new Option("Headlights ON", async() => await _roverClient.TurnHeadlights_On()),
                //new Option("Headlights OFF", async() => await _roverClient.TurnHeadlights_Off()),
                //new Option("Take a picture", async() => WriteTemporaryMessage(await _roverClient.TakeFoto())),
                new Option("Load Rover from API", async() => WriteRoverMessage(await apiClient.GetRover(Guid.Parse("7A73F8AE-0000-0000-AAAA-7AB5A00A9C1D")))),
                new Option("Load all ready RoverTask from API", async() => WriteIEnumerableRoverTask(await apiClient.GetReadyRoverTasks(Guid.Parse("7A73F8AE-0000-0000-AAAA-7AB5A00A9C1D")))),
                new Option("API:COMMAND_HEADLIGHTS_ON", async() => await CreateTask(apiClient, true)),
                new Option("API:COMMAND_HEADLIGHTS_OFF", async() => await CreateTask(apiClient, false)),

                new Option("Exit", () => Environment.Exit(0)),
            };

            // Set the default index of the selected item to be the first
            int index = 0;

            // Write the menu out
            WriteMenu(_options, _options[index]);

            // Store key info in here
            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();

                // Handle each key input (down arrow will write the menu again with a different selected item)
                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (index + 1 < _options.Count)
                    {
                        index++;
                        WriteMenu(_options, _options[index]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (index - 1 >= 0)
                    {
                        index--;
                        WriteMenu(_options, _options[index]);
                    }
                }
                // Handle different action for the option
                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    _options[index].Selected.Invoke();
                    index = 0;
                }
            }
            while (keyinfo.Key != ConsoleKey.X);

            Console.ReadKey();

        }
        // Default action of all the _options. You can create more methods
        static void WriteTemporaryMessage(string? message)
        {
            Console.Clear();
            Console.WriteLine(message);
            Thread.Sleep(2000);
            WriteMenu(_options, _options.First());
        }

        static void WriteRoverMessage(SEVEN.Core.Models.Rover? rover)
        {
            Console.Clear();

            if (rover == null)
            {
                Console.WriteLine("Kein Rover da!");
                Thread.Sleep(2000);
                return;
            }

            Console.WriteLine(rover.Id);
            Console.WriteLine(rover.Name);
            foreach (var task in rover.Tasks)
            {
                Console.WriteLine(task.Id);
                Console.WriteLine(task.Position);
                Console.WriteLine(task.Command);
                Console.WriteLine();
            }
            Thread.Sleep(2000);
            WriteMenu(_options, _options.First());
        }

        static void WriteIEnumerableRoverTask(IEnumerable<RoverTask> roverTasks)
        {
            Console.Clear();
            foreach (var task in roverTasks)
            {
                Console.WriteLine(task.Id);
                Console.WriteLine(task.Position);
                Console.WriteLine(task.Command);
                Console.WriteLine();
            }
            Thread.Sleep(2000);
            WriteMenu(_options, _options.First());
        }

        static async Task CreateTask(APIClient client, bool on)
        {
            await client.CreateRoverTask(new RoverTask() { Id = Guid.NewGuid(), RoverId = Guid.Parse("7A73F8AE-0000-0000-AAAA-7AB5A00A9C1D"), Command = on ? RoverTaskCommands.COMMAND_HEADLIGHTS_ON : RoverTaskCommands.COMMAND_HEADLIGHTS_OFF });
            WriteIEnumerableRoverTask(await client.GetReadyRoverTasks(Guid.Parse("7A73F8AE-0000-0000-AAAA-7AB5A00A9C1D")));
        }


        static void WriteMenu(List<Option> _options, Option selectedOption)
        {
            Console.Clear();
            DrawHeader();
            foreach (Option option in _options)
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