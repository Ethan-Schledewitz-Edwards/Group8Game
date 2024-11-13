using FMOD.Studio;
using FMODUnity;
using Sounds;
using UnityEngine;

namespace Sound.Scripts.Sound
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        private GameObject _player;
        private EventInstance _musicEventInstance;
        
        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
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
            eventInstance.set3DAttributes(_player.transform.position.To3DAttributes());
            return eventInstance;
        }

        private void InitializeMusic(EventReference musicReference)
        {
            _musicEventInstance = CreateEventInstance(musicReference);
            _musicEventInstance.start();
        }

        public void SetMusicParameter(string parameterName, float parameterValue)
        {
            _musicEventInstance.setParameterByName(parameterName, parameterValue);
        }
    }
}