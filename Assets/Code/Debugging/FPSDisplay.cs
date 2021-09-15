using UnityEngine;

namespace Code.Debugging
{
    public class FPSDisplay : MonoBehaviour
    {
        private float _deltaTime = 0.0f;
        private Texture2D _boxTexture;
        private GUIStyle _boxStyle;

        protected void Update()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        }

        protected void OnGUI()
        {

            int w = Screen.width, h = Screen.height;
            Rect rect = new Rect(0, 0, w / 8f, h / 50f);

            GUIStyle style = new GUIStyle
            {
                alignment = TextAnchor.UpperLeft, fontSize = h * 2 / 100,
                normal = {textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f)}
            };

            float msec = _deltaTime * 1000.0f;
            float fps = 1.0f / _deltaTime;
            string text = $"{msec:0.0} ms ({fps:0.} fps)";

            GUIDrawRect(rect, Color.cyan);
            GUI.Label(rect, text, style);
        }

        private void GUIDrawRect( Rect position, Color color )
        {
            if( _boxTexture == null )
            {
                _boxTexture = new Texture2D( 1, 1 );
            }

            if( _boxStyle == null )
            {
                _boxStyle = new GUIStyle();
            }

            _boxTexture.SetPixel( 0, 0, color );
            _boxTexture.Apply();

            _boxStyle.normal.background = _boxTexture;

            GUI.Box( position, GUIContent.none, _boxStyle );
        }
    }
}