using SEVEN.Core.Models.Messages;
using SEVEN.Relay.API.Data.Repositories;

namespace SEVEN.Relay.API.Endpoints;

public static class BroadcastReceiverEndpoints
{
    public static RouteGroupBuilder MessagesGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetMessages).WithName("GetMessages").WithOpenApi();
        group.MapGet("/create/{message}", CreateMessages).WithName("CreateMessages").WithOpenApi();
        group.MapPost("/", PostMessage).WithName("PostMessage").WithOpenApi();
        return group;
    }
    
    private static async Task<IResult> GetMessages(IBroadcastReceiverRepository repository)
    {
        var probe = await repository.GetBroadcastMessages();
        return probe != null ? Results.Ok(probe) : Results.NotFound();
    }

    private static async Task<IResult> CreateMessages(string? message, IBroadcastReceiverRepository repository)
    {
        if (message is null) return Results.NoContent();
        var broadcastMessage = new BroadcastMessage
        {
            Content = message
        };

        var dbMessage = await repository.SaveMessage(broadcastMessage);
        return Results.Ok(dbMessage);
    }

    private static async Task<IResult> PostMessage(BroadcastMessage broadcastMessage, IBroadcastReceiverRepository repository)
    {
        var dbMessage = await repository.SaveMessage(broadcastMessage);
        return Results.Ok(dbMessage);
    }
}