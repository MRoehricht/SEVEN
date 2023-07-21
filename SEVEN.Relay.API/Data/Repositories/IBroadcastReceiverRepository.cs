using SEVEN.Core.Models.Messages;

namespace SEVEN.Relay.API.Data.Repositories;

public interface IBroadcastReceiverRepository
{
    Task<IEnumerable<BroadcastMessage>> GetBroadcastMessages();
    Task<BroadcastMessage> SaveMessage(BroadcastMessage broadcastMessage);
    Task<bool> Remove(Guid broadcastMessageId);
}