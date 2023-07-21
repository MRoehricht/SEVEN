using Microsoft.EntityFrameworkCore;
using SEVEN.Core.Models.Messages;
using SEVEN.Relay.API.Data.Contexts;

namespace SEVEN.Relay.API.Data.Repositories;

public class BroadcastReceiverRepository : IBroadcastReceiverRepository
{
    private readonly RelayContext _context;

    public BroadcastReceiverRepository(RelayContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BroadcastMessage>> GetBroadcastMessages()
    {
        return await _context.Messages.AsNoTracking().ToArrayAsync();
    }

    public async Task<BroadcastMessage> SaveMessage(BroadcastMessage broadcastMessage)
    {
        broadcastMessage.Id = Guid.NewGuid();
        await _context.AddAsync(broadcastMessage);
        await _context.SaveChangesAsync();
        return broadcastMessage;
    }

    public async Task<bool> Remove(Guid broadcastMessageId)
    {
        var broadcastMessage = await _context.Messages.FindAsync(broadcastMessageId);
        if (broadcastMessage is null) return false;

        _context.Messages.Remove(broadcastMessage);
        await _context.SaveChangesAsync();
        return true;
    }
}