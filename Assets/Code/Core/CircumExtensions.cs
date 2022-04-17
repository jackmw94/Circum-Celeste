using System.Text.RegularExpressions;
using UnityEngine.SocialPlatforms;

namespace Code.Core
{
    public static class CircumExtensions
    {
        public static string GetCircumUsername(this IUserProfile localUser)
        {
            return $"{localUser.userName}_{localUser.id}";
        }

        public static bool HasChineseCharacters(this string str)
        {
            Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
            return !string.IsNullOrEmpty(str) && cjkCharRegex.IsMatch(str);
        }
    }
}