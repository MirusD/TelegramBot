namespace _102techBot.DTOs
{
    internal class MessageDTO
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public long UserId { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhotoFileId { get; set; }
        public string? CallbackQueryId { get; set; }
        public bool IsPhoto { get; set; }
    }
}
