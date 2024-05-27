namespace Bloggie.web.Models.ViewModel
{
    public class userviewModel
    {
        public List<User> Users { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool AdminRoleCheckbox { get; set; }
    }
}
