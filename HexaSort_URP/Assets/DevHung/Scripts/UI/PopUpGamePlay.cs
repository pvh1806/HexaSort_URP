using DevHung.Scripts.Data;
using DevHung.Scripts.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DevHung.Scripts.UI
{
    public class PopUpGamePlay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textLevel, textMove;
        [SerializeField] private GameObject bgMove;
        [SerializeField] private Button btnReturn, btnAddStack, btnRefresh;
        [SerializeField] private GameObject btnParent;
        [SerializeField] private TutorialLevel1 tutorialLevel1;
        private void Start()
        {
            LevelController.Instance.OnInit();
            SetText();
            btnReturn.onClick.AddListener(CacheGameData.Instance.UndoStack);
            btnRefresh.onClick.AddListener(LevelController.Instance.OnClickRefresh);
            btnAddStack.onClick.AddListener(LevelController.Instance.OnClickAddStackEmpty);
            if (PlayerData.Instance.GetLevel() != 0) return;
            SetActiveButton(false);
            Invoke(nameof(DelaySetActiveTutorial),0.5f);
            CacheGameData.Instance.IsCheckMove = true;
        }
        public void SetActiveButton(bool isActive)
        {
            btnParent.SetActive(isActive);
        }
        public void SetText()
        {
            int countMove = -1;
            if (LevelController.Instance.levelData.isHardLevel)
            {
                bgMove.SetActive(true);
                textMove.SetText(LevelController.Instance.levelData.moveIndex.ToString());
                countMove = LevelController.Instance.levelData.moveIndex;
            }
            else bgMove.SetActive(false);
            var level = PlayerData.Instance.GetLevel() + 1;
            textLevel.SetText("Level" + " "  + level);
            CacheGameData.Instance.SetUpCountMove(countMove,this);
        }

        public void DelaySetActiveTutorial()
        {
            tutorialLevel1.gameObject.SetActive(true);
        }
        public void SetTextMove(int moveCount)
        {
            textMove.SetText(moveCount.ToString());
        }
    }
}