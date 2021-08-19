using UnityEngine;
using UnityExtras.Code.Optional.Singletons;

namespace Code.Flow
{
    public class FlowContainer : SingletonMonoBehaviour<FlowContainer>
    {
        [SerializeField] private InterLevelFlow _interLevelFlow;

        public InterLevelFlow InterLevelFlow => _interLevelFlow;
    }
}