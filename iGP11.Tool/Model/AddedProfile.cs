using System;

namespace iGP11.Tool.Model
{
    public class AddedProfile
    {
        public AddedProfile(string profileName, Guid basedOnGameProfileId)
        {
            ProfileName = profileName;
            BasedOnGameProfileId = basedOnGameProfileId;
        }

        public Guid BasedOnGameProfileId { get; }

        public string ProfileName { get; }
    }
}