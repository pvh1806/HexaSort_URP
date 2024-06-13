using DevHung.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DevHung.Scripts.UI
{
    public class PopUpGamePlay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textLevel, textMove;
        [SerializeField] private GameObject Bgmove;
        [SerializeField] private Button btnReturn, btnAddStack, btnRefresh;
        [SerializeField] private Button btnNext;
        private void Start()
        {
            LevelController.Instance.OnInit();
            SetText();
            btnReturn.onClick.AddListener(CacheGameData.Instance.UndoStack);
            btnRefresh.onClick.AddListener(LevelController.Instance.OnClickRefresh);
            btnAddStack.onClick.AddListener(LevelController.Instance.OnClickAddStackEmpty);
            btnNext.onClick.AddListener(OnClickNext);
        }

        public void OnClickNext()
        {
            PlayerData.Instance.SetLevel();
            LevelController.Instance.ClearDataChipBg();
            CacheGameData.Instance.Refresh();
            LevelController.Instance.OnInit();
            SetText();
        }
        private void SetText()
        {
            if (LevelController.Instance.levelData.isHardLevel) textMove.SetText(LevelController.Instance.levelData.moveIndex.ToString());
            else Bgmove.SetActive(false);
            textLevel.SetText("Level" + " "  + PlayerData.Instance.GetLevel());
        }
    }
}