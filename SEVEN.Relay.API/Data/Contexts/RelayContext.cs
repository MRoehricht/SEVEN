using Microsoft.EntityFrameworkCore;
using SEVEN.Core.Models;
using SEVEN.Core.Models.Messages;

namespace SEVEN.Relay.API.Data.Contexts;

public class RelayContext: DbContext
{
    public RelayContext(DbContextOptions<RelayContext> options)
        : base(options)
    {
    }

    public DbSet<BroadcastMessage> Messages { get; set; }
}