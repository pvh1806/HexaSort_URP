using DevHung.Scripts.Data;
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
        private void Start()
        {
            LevelController.Instance.OnInit();
            SetText();
            btnReturn.onClick.AddListener(CacheGameData.Instance.UndoStack);
            btnRefresh.onClick.AddListener(LevelController.Instance.OnClickRefresh);
            btnAddStack.onClick.AddListener(LevelController.Instance.OnClickAddStackEmpty);
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
            int level = PlayerData.Instance.GetLevel() + 1;
            textLevel.SetText("Level" + " "  + level);
            CacheGameData.Instance.SetUpCountMove(countMove,this);
        }

        public void SetTextMove(int moveCount)
        {
            textMove.SetText(moveCount.ToString());
        }
    }
}