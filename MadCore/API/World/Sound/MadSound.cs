using MadCore.API.Registry;
using MadCore.API.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MadCore.API.World.Sound
{
    public class MadSound : IDHolder
    {
        public ID SoundId;
        private AudioSource _audioSource;
        private AudioClip _audioClip;
        public MadSound(string clipName, string path)
        {
            _audioClip = AssetUtils.LoadAudioClip(clipName, path);
        }

        public void BuildSoundSource()
        {
            var soundManager = SoundRegistry.Instance.SoundManager;
            var soundEffect = Object.Instantiate(soundManager.sePrefab, soundManager.transform.position, Quaternion.identity);
            soundEffect.transform.SetParent(GameObject.Find("SEPool").transform);
            _audioSource = soundEffect.GetComponent<AudioSource>();
            soundManager.sePool.Add(_audioSource);
        }

        public void PlaySound3D(Vector3 pos, bool randomPitch, float dist)
        {
            _audioSource.volume = SaveManager.SaveSettingStatic.seVolumeStatic;
            _audioSource.gameObject.transform.position = pos;
            _audioSource.spatialBlend = 1f;
            _audioSource.maxDistance = dist;
            _audioSource.loop = false;
            _audioSource.pitch = !randomPitch ? 1f : UnityEngine.Random.Range(0.85f, 1.1f);
            _audioSource.PlayOneShot(_audioClip);
        }
        
        public void SetID(ID id)
        {
            SoundId = id;
        }

        public ID GetID()
        {
            return SoundId;
        }
        
        public override string ToString()
        {
            return SoundId.ToString();
        }
    }
}