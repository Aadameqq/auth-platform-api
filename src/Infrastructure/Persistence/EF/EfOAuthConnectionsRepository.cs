using Core.Ports;

namespace Infrastructure.Persistence.EF;

public class EfOAuthConnectionsRepository(DatabaseContext ctx) : OAuthConnectionsRepository { }
