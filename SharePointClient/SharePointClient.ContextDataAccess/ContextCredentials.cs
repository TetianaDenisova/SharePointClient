namespace SharePointClient.ContextDataAccess
{
    public class ContextCredentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public ContextCredentials(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}
