using UnityEngine;

namespace Code.Core
{
    public static class CircumLayers
    {
        private const string PlayerLayerString = "Player";
        private const string OrbiterLayerString = "Orbiter";
        private const string EnemyLayerString = "Enemy";
        private const string HazardLayerString = "Hazard";

        private static readonly int PlayerLayer;
        private static readonly int OrbiterLayer;
        private static readonly int EnemyLayer;
        private static readonly int HazardLayer;
        
        static CircumLayers()
        {
            PlayerLayer = LayerMask.NameToLayer(PlayerLayerString);
            OrbiterLayer = LayerMask.NameToLayer(OrbiterLayerString);
            EnemyLayer = LayerMask.NameToLayer(EnemyLayerString);
            HazardLayer = LayerMask.NameToLayer(HazardLayerString);
        }

        public static bool IsOrbiter(this GameObject gameObject)
        {
            return gameObject.layer == OrbiterLayer;
        }

        public static bool IsPlayer(this GameObject gameObject)
        {
            return gameObject.layer == PlayerLayer;
        }

        public static bool IsEnemy(this GameObject gameObject)
        {
            return gameObject.layer == EnemyLayer;
        }

        public static bool IsHazard(this GameObject gameObject)
        {
            return gameObject.layer == HazardLayer;
        }
    }
}