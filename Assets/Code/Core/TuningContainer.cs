using UnityEngine;
using UnityExtras.Code.Optional.Singletons;

namespace Code.Core
{
    public class TuningContainer : SingletonMonoBehaviour<TuningContainer>
    {
        [SerializeField] private GameplayTuning _gameplayTuning;
        
        public static GameplayTuning GameplayTuning => Instance._gameplayTuning;
    }
}