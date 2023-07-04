namespace TrueCodeTask.DataAccess.Models
{
    public class Post
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public DateTime PublishDate { get; set; }
        public string Content { get; set; }
    }
}
