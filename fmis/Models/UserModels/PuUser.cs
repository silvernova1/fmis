namespace fmis.Models.UserModels
{
    public class PuUser
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        //dtr credentials
        public string Fname { get; set; }

        public string Mname { get; set; }

        public string Lname { get; set; }

        public string UserId { get; set; }
    }
}
