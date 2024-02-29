namespace ChatWe.Persistance.Entities
{
    public class Attachments
    {
        public int AttachmentId { get; set; }
        public string Files { get; set; }
        public virtual Conversations Conversation { get; set; }
    }
}