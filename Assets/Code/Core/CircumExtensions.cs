using UnityEngine.SocialPlatforms;

namespace Code.Core
{
    public static class CircumExtensions
    {
        public static string GetCircumUsername(this IUserProfile localUser)
        {
            return $"{localUser.userName}_{localUser.id}";
        }
    }
}