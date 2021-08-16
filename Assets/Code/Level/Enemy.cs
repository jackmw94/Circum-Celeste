using UnityEngine;

namespace Code.Level
{
    public class Enemy : MonoBehaviour
    {
        public bool IsDead { get; private set; }
        
        public bool CanMove { get; set; }
    }
}