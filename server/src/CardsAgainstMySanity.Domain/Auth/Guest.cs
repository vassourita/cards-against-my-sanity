namespace CardsAgainstMySanity.Domain.Auth
{
    public class Guest : User
    {
        public Guest(string username, string ipAddress) : base(username, ipAddress)
        {
        }
    }
}