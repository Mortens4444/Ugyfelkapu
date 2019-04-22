using Mtf.Utils.StringExtensions;
using System;
using System.Collections.Generic;

namespace Ugyfelkapu
{
    public static class ResponseChecker
    {
        public static void CreckForError(string content)
        {
            var errors = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("<ul class=\"fielderror\">\r\n		<li><span>", "</span>"),
                new Tuple<string, string>("<p class=\"fielderror\">", "</p>"),
                new Tuple<string, string>("<span class=\"fielderror\">", "</span>")
            };

            foreach (var error in errors)
            {
                var errorMessage = content.Substring(error.Item1, error.Item2).Trim();
                if (!String.IsNullOrEmpty(errorMessage))
                {
                    throw new Exception(errorMessage);
                }
            }            
        }
    }
}
