namespace iGP11.Library
{
    public static class FileUtility
    {
        private static readonly string[] _abbreviations = { "B", "KB", "MB", "GB", "TB" };

        public static string GetFileLengthAbbreviation(long length)
        {
            var order = 0;
            while ((length >= 1024) && (order < _abbreviations.Length - 1))
            {
                order++;
                length = length / 1024;
            }

            return $"{length:0.#} {_abbreviations[order]}";
        }
    }
}