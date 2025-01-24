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
        private UIAudioClipsConfig uIAudioClipsConfig;

        [SetUp]
        public void SetUp()
        {
            audioManager = new GameObject("AudioManager").AddComponent<AudioManager>();
            audioListener = new GameObject("AudioListener").AddComponent<AudioListener>();
            dummyAudioSource = new GameObject("DummyAudioSource").AddComponent<AudioSource>();
            audioManager.GetType().GetField("audioSourceObject", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(audioManager, dummyAudioSource);

            uIAudioClipsConfig = ScriptableObject.CreateInstance<UIAudioClipsConfig>();
            uIAudioClipsConfig.fightBeginSound = AudioClip.Create("FightBegin", 44100, 1, 44100, false);
            uIAudioClipsConfig.buttonClickSound = AudioClip.Create("ButtonClick", 44100, 1, 44100, false);
            uIAudioClipsConfig.buttonHoverSound = AudioClip.Create("ButtonHover", 44100, 1, 44100, false);
            uIAudioClipsConfig.fightWonSound = AudioClip.Create("FightWon", 44100, 1, 44100, false);
            uIAudioClipsConfig.fightLostSound = AudioClip.Create("FightLost", 44100, 1, 44100, false);
            audioManager.GetType().GetField("uiAudioClipsConfig", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(audioManager, uIAudioClipsConfig);
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(audioManager.gameObject);
            Object.Destroy(audioListener.gameObject);
            Object.Destroy(dummyAudioSource.gameObject);
            Object.Destroy(uIAudioClipsConfig);
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

        [UnityTest]
        public IEnumerator PlayUISound_PlaysCorrectClip()
        {
            AudioSource[] audioSources;

            audioManager.PlayUISound(UISounds.FightBegin);
            audioSources = Object.FindObjectsOfType<AudioSource>();
            Assert.AreEqual("FightBegin", audioSources[0].clip.name);
            yield return new WaitForSeconds(audioSources[0].clip.length + 0.1f);

            audioManager.PlayUISound(UISounds.ButtonClick);
            audioSources = Object.FindObjectsOfType<AudioSource>();
            Assert.AreEqual("ButtonClick", audioSources[0].clip.name);
            yield return new WaitForSeconds(audioSources[0].clip.length + 0.1f);

            audioManager.PlayUISound(UISounds.ButtonHover);
            audioSources = Object.FindObjectsOfType<AudioSource>();
            Assert.AreEqual("ButtonHover", audioSources[0].clip.name);
            yield return new WaitForSeconds(audioSources[0].clip.length + 0.1f);

            audioManager.PlayUISound(UISounds.FightWon);
            audioSources = Object.FindObjectsOfType<AudioSource>();
            Assert.AreEqual("FightWon", audioSources[0].clip.name);
            yield return new WaitForSeconds(audioSources[0].clip.length + 0.1f);

            audioManager.PlayUISound(UISounds.FightLost);
            audioSources = Object.FindObjectsOfType<AudioSource>();
            Assert.AreEqual("FightLost", audioSources[0].clip.name);
            yield return new WaitForSeconds(audioSources[0].clip.length + 0.1f);
        }
    }
}