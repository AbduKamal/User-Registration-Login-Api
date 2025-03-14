namespace UserApi.Models
{
    public class ChangePasswordRequestdto
    {
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
