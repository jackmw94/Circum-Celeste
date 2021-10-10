using UnityEngine;

namespace ToolbarExtender
{
    static class ToolbarStyles
    {
        public static readonly GUIStyle s_commandButtonStyle;
        public static readonly GUIStyle s_testButtonStyle;
        public static readonly GUIStyle s_missionDropDownStyle;

        static ToolbarStyles()
        {
            s_commandButtonStyle = new GUIStyle( "Command" )
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold
            };
        
            s_testButtonStyle = new GUIStyle( "Command" )
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Normal,
                fixedWidth = 140,
            };

            s_missionDropDownStyle = new GUIStyle( "toolbarDropDown" )
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleLeft,
                imagePosition = ImagePosition.TextOnly,
                fontStyle = FontStyle.Normal,
                fixedWidth = 180,
            };
        }
    }
}