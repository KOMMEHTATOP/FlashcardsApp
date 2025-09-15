namespace FlashcardsBlazorUI.Models
{
    public class Group
    {
        public Guid Id { get; set; }
        public required string GroupName { get; set; }
        public required string GroupColor { get; set; } 
        public DateTime CreatedAt { get; set; }
        public int Order { get; set; } = 0;
    }
}
