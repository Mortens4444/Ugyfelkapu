using System;
using System.Text.RegularExpressions;

namespace Ugyfelkapu
{
	public static class ResponseChecker
    {
        public static void CreckForError(string content)
        {
			var matches = Regex.Match(content, @"<.* class\=\""fielderror\"">\s*(.*<span>)*([^<]*)<");
            if (matches.Groups.Count > 0)
            {
				var lastGroup = matches.Groups[matches.Groups.Count - 1];
				throw new Exception(lastGroup.Value);
            }
        }
    }
}
