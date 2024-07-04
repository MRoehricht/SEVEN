using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SEVEN.Core.Models;
using SEVEN.Core.Models.Configuration;
using SEVEN.MissionControl.API.Client;
using SEVEN.Rover.Core.Clients;

namespace SEVEN.Rover.Console;

internal class Program
{
    public static List<Option> _options = new();
    private static RoverClient? _roverClient;

    private static Task Main()
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false, true);

        IConfiguration config = builder.Build();
        var roverConnection = config["RoverConnection"];

        if (string.IsNullOrWhiteSpace(roverConnection))
        {
            System.Console.WriteLine("RoverConnection ist nicht vergeben!");
            return Task.CompletedTask;
        }

        RoverConnection connection = new() { RoverUrl = roverConnection };
        var roverOptions = Options.Create(connection);

        APIConnection apiConnection = new() { BaseUrl = "https://localhost:7217/" };
        var apiOptions = Options.Create(apiConnection);


        var apiClient = new APIClient(apiOptions);


        _roverClient = new RoverClient(roverOptions);

        // Create _options that you want your menu to have
        _options = new List<Option>
        {
            //new Option("Status", async() => WriteTemporaryMessage(RoverStatusNames.STATUS_HEADLIGHTS +":" + await _roverClient.GetHeadlights_Status())),
            //new Option("Systemcheck", () => WriteTemporaryMessage("Run SystemCheck")),
            new("Headlights ON",  () =>  _ = _roverClient.TurnHeadlights_On()),
            //new Option("Headlights OFF", async() => await _roverClient.TurnHeadlights_Off()),
            //new Option("Take a picture", async() => WriteTemporaryMessage(await _roverClient.TakeFoto())),
            new("Load Rover from API",
                async () => await WriteRoverMessage(
                    await apiClient.GetRover(Guid.Parse("7A73F8AE-0000-0000-AAAA-7AB5A00A9C1D")))),
            new("Load all ready RoverTask from API",
                async () => await WriteIEnumerableRoverTask(
                    await apiClient.GetReadyRoverTasks(Guid.Parse("7A73F8AE-0000-0000-AAAA-7AB5A00A9C1D")))),
            new("API:COMMAND_HEADLIGHTS_ON", async () => await CreateTask(apiClient, true)),
            new("API:COMMAND_HEADLIGHTS_OFF", async () => await CreateTask(apiClient, false)),
            new("API:LOAD TOKEN", async () => await GetToken(apiClient)),
            new("API:LOAD TOKEN", async () => await CreateMe(apiClient)),

            new("Exit", () => Environment.Exit(0))
        };

        // Set the default index of the selected item to be the first
        var index = 0;

        // Write the menu out
        WriteMenu(_options, _options[index]);

        // Store key info in here
        ConsoleKeyInfo keyinfo;
        do
        {
            keyinfo = System.Console.ReadKey();

            // Handle each key input (down arrow will write the menu again with a different selected item)
            if (keyinfo.Key == ConsoleKey.DownArrow)
                if (index + 1 < _options.Count)
                {
                    index++;
                    WriteMenu(_options, _options[index]);
                }

            if (keyinfo.Key == ConsoleKey.UpArrow)
                if (index - 1 >= 0)
                {
                    index--;
                    WriteMenu(_options, _options[index]);
                }

            // Handle different action for the option
            if (keyinfo.Key == ConsoleKey.Enter)
            {
                _options[index].Selected.Invoke();
                index = 0;
            }
        } while (keyinfo.Key != ConsoleKey.X);

        System.Console.ReadKey();
        return Task.CompletedTask;
    }

    // Default action of all the _options. You can create more methods
    private static void WriteTemporaryMessage(string? message)
    {
        System.Console.Clear();
        System.Console.WriteLine(message);
        Thread.Sleep(2000);
        WriteMenu(_options, _options.First());
    }

    private static async Task WriteRoverMessage(SEVEN.Core.Models.Rover? rover)
    {
        System.Console.Clear();

        if (rover == null)
        {
            System.Console.WriteLine("Kein Rover da!");
            await Task.Delay(2000);
            return;
        }

        System.Console.WriteLine(rover.Id);
        System.Console.WriteLine(rover.Name);
        foreach (var task in rover.Tasks)
        {
            System.Console.WriteLine(task.Id);
            System.Console.WriteLine(task.Position);
            System.Console.WriteLine(task.Command);
            System.Console.WriteLine();
        }

        await Task.Delay(2000);
        WriteMenu(_options, _options.First());
    }

    private static async Task WriteIEnumerableRoverTask(IEnumerable<RoverTask> roverTasks)
    {
        System.Console.Clear();
        foreach (var task in roverTasks)
        {
            System.Console.WriteLine(task.Id);
            System.Console.WriteLine(task.Position);
            System.Console.WriteLine(task.Command);
            System.Console.WriteLine();
        }

        await Task.Delay(2000);
        WriteMenu(_options, _options.First());
    }

    private static async Task CreateTask(APIClient client, bool on)
    {
        await client.CreateRoverTask(new RoverTask
        {
            Id = Guid.NewGuid(), RoverId = Guid.Parse("7A73F8AE-0000-0000-AAAA-7AB5A00A9C1D"),
            Command = on ? RoverTaskCommands.CommandHeadlightsOn : RoverTaskCommands.CommandHeadlightsOff
        });
       await WriteIEnumerableRoverTask(await client.GetReadyRoverTasks(Guid.Parse("7A73F8AE-0000-0000-AAAA-7AB5A00A9C1D")));
    }

    private static async Task GetToken(APIClient client)
    {
        var token = await client.GetProbeToken(Guid.Parse("7A73F8AE-0000-0000-BBBB-7AB5A00A9C1D"));
        System.Console.Clear();
        if (token?.Type != null)
        {
            System.Console.WriteLine(token.Type);
            System.Console.WriteLine(token.Token);
        }
        else
        {
            System.Console.WriteLine("Token nicht gefunden!");
        }
        
        System.Console.WriteLine();
        Thread.Sleep(2000);
        WriteMenu(_options, _options.First());
    }
    
    private static async Task CreateMe(APIClient client)
    {
        var token = await client.GetProbeToken(Guid.Parse("7A73F8AE-0000-0000-BBBB-7AB5A00A9C1D"));
        if(token == null)
        {
            System.Console.WriteLine("Token nicht gefunden!");
            return;
        }
        var measurement = await client.CreateMeasurement(
            new Measurement()
            {
                MeasurementType = MeasurementType.Humidity, Value = "56%",
                ProbeId = Guid.Parse("7A73F8AE-0000-0000-BBBB-7AB5A00A9C1D")
            }, token);
        
        System.Console.Clear();
        if (measurement?.Id != null)
        {
            System.Console.WriteLine(measurement.Id);
            System.Console.WriteLine(measurement.Time);
        }
        else
        {
            System.Console.WriteLine("Messung nicht erstellt!");
        }
        
        System.Console.WriteLine();
        Thread.Sleep(2000);
        WriteMenu(_options, _options.First());
    }
    
    

    private static void WriteMenu(List<Option> options, Option selectedOption)
    {
        System.Console.Clear();
        DrawHeader();
        foreach (var option in options)
        {
            if (option == selectedOption)
                System.Console.Write("> ");
            else
                System.Console.Write(" ");

            System.Console.WriteLine(option.Name);
        }
    }

    private static void DrawHeader()
    {
        System.Console.WriteLine("\t\t_______ _______ _    _ _______ __   _ ");
        System.Console.WriteLine("\t\t|______ |______  \\  /  |______ | \\  |");
        System.Console.WriteLine("\t\t______| |______   \\/   |______ |  \\_|");
        System.Console.WriteLine("\t\t\t\t\t__________                          ");
        System.Console.WriteLine("\t\t\t\t\t\\______   \\ _______  __ ___________ ");
        System.Console.WriteLine("\t\t\t\t\t |       _//  _ \\  \\/ // __ \\_  __ \\");
        System.Console.WriteLine("\t\t\t\t\t |    |   (  <_> )   /\\  ___/|  | \\/");
        System.Console.WriteLine("\t\t\t\t\t |____|_  /\\____/ \\_/  \\___  >__|   ");
        System.Console.WriteLine("\t\t\t\t\t        \\/                 \\/       ");
        System.Console.WriteLine();
        System.Console.WriteLine("+-+-+-+-+-+-+-+-+ +-+-+-+-+-+-+-+-+ +-+-+-+-+-+-+-+ +-+-+-+-+-+-+-+ +-+-+-+-+-+-+-+");
        System.Console.WriteLine("|S|a|n|d|b|e|r|g| |E|l|e|c|t|r|i|c| |V|e|h|i|c|l|e| |E|d|e|n|       |N|e|t|w|o|r|k|");
        System.Console.WriteLine("+-+-+-+-+-+-+-+-+ +-+-+-+-+-+-+-+-+ +-+-+-+-+-+-+-+ +-+-+-+-+-+-+-+ +-+-+-+-+-+-+-+");
    }
}

public class Option
{
    public Option(string name, Action selected)
    {
        Name = name;
        Selected = selected;
    }

    public string Name { get; }
    public Action Selected { get; }
}