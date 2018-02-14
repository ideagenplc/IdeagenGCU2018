namespace ExampleS3Upload
{
    public class CreateAttachmentRequest
    {
        public string TenantId { get; set; }
        public string AuthToken { get; set; }
        public string AttachmentId { get; set; }
        public string Title { get; set; }
        public string TimelineEventId { get; set; }
    }
}