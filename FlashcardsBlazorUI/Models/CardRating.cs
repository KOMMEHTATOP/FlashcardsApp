namespace FlashcardsBlazorUI.Models
{
    public class CardRating
    {
        public Guid Id { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CardId { get; set; }
        public Guid UserId { get; set; }
    }
}
