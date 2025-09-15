namespace FlashcardsBlazorUI.Models
{
    public class CardRating
    {
        public string Id { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CardId { get; set; }
        public string UserId { get; set; }
    }
}
