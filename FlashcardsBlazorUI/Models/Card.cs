namespace FlashcardsBlazorUI.Models
{
    public class Card
    {
        public string CardId { get; set; }
        public required string Question { get; set; }
        public required string Answer { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string GroupId { get; set; }
        public string UserId { get; set; }
    }
}
