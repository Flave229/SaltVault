namespace SaltVault.WebApp.Models.Household
{
    public class AddHouseholdRequest
    {
        public string Name { get; set; }
    }

    public class AddHouseholdResponse : CommunicationResponse
    {
        public int Id { get; set; }
    }
}