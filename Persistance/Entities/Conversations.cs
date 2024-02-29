namespace ChatWe.Persistance.Entities
{
    public class Conversations
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public int? GroupId { get; set; }
        public int? AttachmentId { get; set; }
        public virtual ICollection<Attachments> Attachments { get; set; }

        //public virtual User Sender { get; set; }
        //public virtual User Receiver { get; set; }
    }
}