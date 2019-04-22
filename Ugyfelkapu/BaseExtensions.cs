using System;

namespace Mtf.Utils.StringExtensions
{
    public static class BaseExtensions
    {
        private const int NotFound = -1;

        public static string Substring(this string value, string firstElement, string secondElement, bool caseInsensitive = false, int startIndex = 0)
        {
            var sIndex = caseInsensitive ? value.IndexOf(firstElement, startIndex, StringComparison.CurrentCultureIgnoreCase) : value.IndexOf(firstElement, startIndex);

            if (sIndex != NotFound)
            {
                sIndex += firstElement.Length;

                var eIndex = caseInsensitive ? value.IndexOf(secondElement, sIndex, StringComparison.CurrentCultureIgnoreCase) : value.IndexOf(secondElement, sIndex);
                return eIndex == NotFound ? value.Substring(sIndex) : value.Substring(sIndex, eIndex - sIndex);
            }
            return String.Empty;
        }
    }
}