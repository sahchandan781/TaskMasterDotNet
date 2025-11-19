namespace TaskMasterAPI.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public string Email { get; set; }
        public string PasswordHash { set; get; }

        public string Role { get; set; } = "User";

        public ICollection<TaskItemModel> Tasks { get; set; }
    }
}
