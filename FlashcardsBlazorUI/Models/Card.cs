namespace FlashcardsBlazorUI.Models
{
    public class Card
    {
        public Guid CardId { get; set; }
        public required string Question { get; set; }
        public required string Answer { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
    }
}
