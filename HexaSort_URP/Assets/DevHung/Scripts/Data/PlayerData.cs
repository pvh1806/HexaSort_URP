using DevHung.Scripts.DesginPattern;
using DevHung.Scripts.Manager.Audio;
using UnityEngine;

namespace DevHung.Scripts.Data
{
    public class PlayerData : MonoSingleton<PlayerData>
    {
        private int level;
        [Header("SETTING")]
        [SerializeField] private bool MusicOn;
        [SerializeField] private bool SoundOn;
        private void Start()
        {
            // PROPERTIES
            if (SecurePlayerPrefs.HasKey("Level"))
            {
                level = SecurePlayerPrefs.GetInt("Level");
            }
            else
            {
                level = 0;
                SecurePlayerPrefs.SetInt("Level", level);
            }
        }
        
        public void SetLevel()
        {
            level +=1;
            SecurePlayerPrefs.SetInt("Level", level);
        }
        public int GetLevel()
        {
            return level;
        }
        #region GET Sound

        public bool IsSoundOn()
        {
            return SoundOn;
        }

        public bool IsMusicOn()
        {
            return MusicOn;
        }
        #endregion
        public void SetMusic(bool onOff)
        {
            MusicOn = onOff;
            SecurePlayerPrefs.SetBool("MusicOn", MusicOn);
            AudioManager.instance.Musicmixer.SetFloat("Music", MusicOn ? 0 : -80);
        }

        public void SetSound(bool onOff)
        {
            SoundOn = onOff;
            SecurePlayerPrefs.SetBool("SoundOn", SoundOn);
            AudioManager.instance.Musicmixer.SetFloat("SoundVFX", SoundOn ? 0 : -80);
        }
    }
}