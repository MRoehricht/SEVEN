namespace SEVEN.Core.Models.Messages;

public class BroadcastMessageContent
{
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public MessageType Type { get; set; }
    public string? Value { get; set; }
}