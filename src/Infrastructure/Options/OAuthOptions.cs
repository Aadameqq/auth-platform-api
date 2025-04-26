using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Options;

public class OAuthOptions
{
    [Required]
    public required string GithubClientId { get; init; }

    public required string GithubClientSecret { get; init; }
}
