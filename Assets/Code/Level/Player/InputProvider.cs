using Code.Core;
using Code.Debugging;
using UnityEngine;

namespace Code.Level.Player
{
    public abstract class InputProvider
    {
        protected virtual bool CursorLocked => false;
        
        protected InputProvider()
        {
            CircumDebug.Log($"Creating input provider of type {GetType()}");
        }

        protected virtual void Initialise()
        {
            Cursor.lockState = CursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        }
    
        public abstract Vector2 GetMovementInput();

        public abstract bool GetSlingInput();

        public static T CreateInputProvider<T>() where T : InputProvider, new()
        {
            T inputProvider = new T();
            inputProvider.Initialise();
            return inputProvider;
        }

        public static InputProvider CreateDefaultInputProviderForPlatform()
        {
            if (Application.isMobilePlatform)
            {
                return CreateInputProvider<UIInputProvider>();
            }
            
            return CreateInputProvider<KeyboardInputProvider>();
        }
    }
}