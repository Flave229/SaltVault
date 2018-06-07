namespace SaltVault.WebApp.Models.Household
{
    public class JoinHouseholdRequest
    {
        public string InviteLink { get; set; }
    }

    public class JoinHouseholdResponse : CommunicationResponse
    {
        public int Id { get; set; }
    }
}
