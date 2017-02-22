using System;

namespace iGP11.Tool.Model
{
    public class AddedProfile
    {
        public AddedProfile(string profileName, Guid basedOnProfileId)
        {
            ProfileName = profileName;
            BasedOnProfileId = basedOnProfileId;
        }

        public Guid BasedOnProfileId { get; }

        public string ProfileName { get; }
    }
}