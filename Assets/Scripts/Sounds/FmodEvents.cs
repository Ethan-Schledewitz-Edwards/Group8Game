using FMODUnity;
using UnityEngine;

namespace Sounds
{
    public class FmodEvents : MonoBehaviour
    {
        [field: Header("Music")]
        [field: SerializeField] public EventReference Music { get; private set; }
        
        [field: Header("Character Noises")]
        [field: SerializeField] public EventReference Walk { get; private set; }
        [field: SerializeField] public EventReference Swing { get; private set; }
        
        public static FmodEvents Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            if (Instance == null)
            {
                Debug.LogError("More then 1 instance");
            }
        }
    }
}