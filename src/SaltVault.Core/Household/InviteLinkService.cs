using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SaltVault.Core.Household.Model;
using SaltVault.Core.Users;

namespace SaltVault.Core.Household
{
    public class InviteLinkService
    {
        private readonly Dictionary<string, HouseInvite> _cachedInviteLinks;
        private readonly int _hoursToLive;

        public InviteLinkService()
        {
            _cachedInviteLinks = new Dictionary<string, HouseInvite>();
            _hoursToLive = 1;
        }

        public string GenerateInviteLinkForHousehold(ActiveUser user)
        {
            string existingKey = _cachedInviteLinks.FirstOrDefault(inviteLink => inviteLink.Value.Id == user.HouseId).Key;
            if (existingKey != null)
            {
                bool valid = CheckExpiry(existingKey);
                if (valid)
                    return existingKey;
            }

            string possibleCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random randomGenerator = new Random();
            StringBuilder stringBuilder = new StringBuilder();

            for (int inviteIndex = 0; inviteIndex < 6; ++inviteIndex)
            {
                stringBuilder.Append(possibleCharacters[randomGenerator.Next(possibleCharacters.Length)]);
            }

            string inviteCode = stringBuilder.ToString();
            _cachedInviteLinks.Add(inviteCode, new HouseInvite
            {
                Id = user.HouseId,
                Created = DateTime.Now
            });
            return inviteCode;
        }

        private bool CheckExpiry(string key)
        {
            if (_cachedInviteLinks[key].Created.AddHours(_hoursToLive) > DateTime.Now)
                return true;

            _cachedInviteLinks.Remove(key);
            return false;
        }
    }
}
