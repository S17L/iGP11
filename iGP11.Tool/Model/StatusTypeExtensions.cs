namespace iGP11.Tool.Model
{
    public static class StatusTypeExtensions
    {
        public static bool? Translate(this StatusType type)
        {
            if (type == StatusType.Information)
            {
                return null;
            }

            return type == StatusType.Ok;
        }
    }
}