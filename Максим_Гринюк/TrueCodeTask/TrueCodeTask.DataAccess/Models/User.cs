namespace TrueCodeTask.DataAccess.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Post> Posts { get; set; }
    }
}
