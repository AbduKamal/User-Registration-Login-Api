namespace UserApi.Models
{
    public class EmailVerificationdto
    {
        public string Email { get; set; }
        public int VerificationCode { get; set; }
    }
}
