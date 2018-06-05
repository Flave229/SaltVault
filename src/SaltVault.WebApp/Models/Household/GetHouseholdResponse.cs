namespace SaltVault.WebApp.Models.Household
{
    public class GetHouseholdResponse : CommunicationResponse
    {
        public Core.Household.Model.House House { get; set; }
    }
}
