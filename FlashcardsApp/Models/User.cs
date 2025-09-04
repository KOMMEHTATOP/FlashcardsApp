using Microsoft.AspNetCore.Identity;

namespace FlashcardsApp.Models;

public class User : IdentityUser<Guid>
{
    public List<Card>? Cards { get; set; }
    public List<CardRating>? CardRatings { get; set; }
    public List<Group>? Groups { get; set; }
}
