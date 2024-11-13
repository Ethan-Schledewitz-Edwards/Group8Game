using FMOD.Studio;
using FMODUnity;
using Sounds;
using UnityEngine;

namespace Sound.Scripts.Sound
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        private GameObject player;
        private EventInstance musicEventInstance;
        
        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            InitializeMusic(FmodEvents.Instance.Music);
            Instance = this;
            if (Instance == null)
            {
                Debug.LogError("More then one audio manager");
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void PlayOneShot(EventReference sound, Vector3 worldPos)
        {
            RuntimeManager.PlayOneShot(sound, worldPos);
        }
        
        public StudioEventEmitter InitializeEventEmitter(GameObject emitterGameObject)
        {
            StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
            return emitter;
        }
        public EventInstance CreateEventInstance(EventReference eventReference)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            eventInstance.set3DAttributes(player.transform.position.To3DAttributes());
            return eventInstance;
        }

        private void InitializeMusic(EventReference musicReference)
        {
            musicEventInstance = CreateEventInstance(musicReference);
            musicEventInstance.start();
        }

        public void SetMusicParameter(string parameterName, float parameterValue)
        {
            musicEventInstance.setParameterByName(parameterName, parameterValue);
        }
    }
}