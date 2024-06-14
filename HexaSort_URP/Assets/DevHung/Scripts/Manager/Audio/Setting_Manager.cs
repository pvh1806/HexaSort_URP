using DevHung.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace DevHung.Scripts.Manager.Audio
{
    public class Setting_Manager : MonoBehaviour
    {
        [SerializeField] private Button _musicbtn;
        [SerializeField] private GameObject _OffMusic;
        [SerializeField] private Button _Soundbtn;
        [SerializeField] private GameObject _OffSound;

        private void OnEnable()
        {
            _musicbtn.onClick.AddListener(OnMusicButtonClicked);
            _Soundbtn.onClick.AddListener(OnSoundButtonClicked);
        }

        private void OnDisable()
        {
            _musicbtn.onClick.RemoveListener(OnMusicButtonClicked);
            _Soundbtn.onClick.RemoveListener(OnSoundButtonClicked);
        }

        private void Start()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            bool isMusicOn = PlayerData.Instance.IsMusicOn();
            bool isSoundOn = PlayerData.Instance.IsSoundOn();

            // Update the sprite displayed in the Image components based on loaded settings

            bool isActiveOffMusic = !isMusicOn;
            _OffMusic.SetActive(isActiveOffMusic);
            bool isActiveSoundMusic = !isSoundOn;
            _OffSound.SetActive(isActiveSoundMusic);
            PlayerData.Instance.SetMusic(isMusicOn);
            PlayerData.Instance.SetSound(isSoundOn);
        }

        private void OnMusicButtonClicked()
        {
            bool isMusicOn = !PlayerData.Instance.IsMusicOn();
            PlayerData.Instance.SetMusic(isMusicOn);
            _OffMusic.SetActive(!isMusicOn);
        }

        private void OnSoundButtonClicked()
        {
            bool isSoundOn = !PlayerData.Instance.IsSoundOn();
            PlayerData.Instance.SetSound(isSoundOn);
            _OffSound.SetActive(!isSoundOn);
        }
        
    }
}
