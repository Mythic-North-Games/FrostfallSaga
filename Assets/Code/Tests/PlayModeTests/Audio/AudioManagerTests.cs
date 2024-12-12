using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using FrostfallSaga.Audio;

namespace FrostfallSaga.PlayModeTests
{
    public class AudioManagerTests
    {
        private AudioManager audioManager;
        private AudioListener audioListener;
        private AudioSource dummyAudioSource;

        [SetUp]
        public void SetUp()
        {
            audioManager = new GameObject("AudioManager").AddComponent<AudioManager>();
            audioListener = new GameObject("AudioListener").AddComponent<AudioListener>();
            dummyAudioSource = new GameObject("DummyAudioSource").AddComponent<AudioSource>();
            audioManager.GetType().GetField("audioSourceObject", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(audioManager, dummyAudioSource);
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(audioManager.gameObject);
            Object.Destroy(audioListener.gameObject);
            Object.Destroy(dummyAudioSource.gameObject);
            AudioManager.instance = null;
        }

        [Test]
        public void AudioManager_Instance_IsNotNull()
        {
            Assert.IsNotNull(AudioManager.instance);
        }

        [UnityTest]
        public IEnumerator PlaySoundEffectClip_CreatesAndDestroysAudioSource()
        {
            AudioSource[] audioSources;
            AudioClip testClip = AudioClip.Create("TestClip", 44100, 1, 44100, false);
            Transform testTransform = new GameObject().transform;
            float volume = 1.0f;

            audioManager.PlaySoundEffectClip(testClip, testTransform, volume);
            audioSources = Object.FindObjectsOfType<AudioSource>();
            Assert.AreEqual(2, audioSources.Length); //The dummyAudioSource prefab and the audioSOurceObject created by the function

            yield return new WaitForSeconds(testClip.length + 0.1f);

            audioSources = Object.FindObjectsOfType<AudioSource>();
            Assert.AreEqual(1, audioSources.Length); //The dummyAudioSource prefab is still in the scene

            Object.Destroy(testTransform.gameObject);
        }
    }
}