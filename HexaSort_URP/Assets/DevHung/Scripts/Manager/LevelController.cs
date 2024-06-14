using DevHung.DataSO;
using DevHung.Scripts.Chip;
using DevHung.Scripts.Data;
using DevHung.Scripts.DesginPattern;
using DevHung.Scripts.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DevHung.Scripts.Manager
{
    public class LevelController : MonoSingleton<LevelController>
    {
        [SerializeField] private LevelData[] listLevelData;
        [SerializeField] private StackChipController stackChipControllerPrefabs;
        [SerializeField] private ChipBG[] chipBgs;
        [SerializeField] private ChipBG[] chipBgLevel1;
        [SerializeField] private GameObject textSpawnParent;
        [SerializeField] private TextFullUi textFullUiPrefab;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private PopUpGamePlay popUpGamePlay;
        [SerializeField] private AnimationWin animationWin;
        [SerializeField] private PopUpLoss popUpLoss;
        private int indexCount;
        private int indexTotalHexa;
        public LevelData levelData;

        public void OnInit()
        {
            InitLevelData();
            HideAll();
            if(PlayerData.Instance.GetLevel() == 0)
            {
                for (int i = 0; i < chipBgLevel1.Length; i++)
                {
                    chipBgLevel1[i].gameObject.SetActive(true);
                    var x = Instantiate(stackChipControllerPrefabs, chipBgLevel1[i].transform);
                    x.OnInit(levelData.listStackChips[i]);
                    chipBgLevel1[i].OnInit(x);
                }
                return;
            }
            for (int i = chipBgLevel1.Length - 1; i >= 0; i--)
            {
                chipBgLevel1[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < indexTotalHexa; i++)
            {
                chipBgs[i].gameObject.SetActive(true);
            }

            SpawnHexa();
        }

        public StackChipController SetupStackControllerToTutorial(int indexTutorial)
        { 
            return chipBgLevel1[indexTutorial].GetStackController();
        }
        private void AddStackEmpty()
        {
            var x = Instantiate(stackChipControllerPrefabs, chipBgs[indexCount].transform);
            chipBgs[indexCount].OnInit(x);
            indexCount++;
        }

        private void HideAll()
        {
            for (int i = 0; i < chipBgs.Length; i++)
            {
                chipBgs[i].gameObject.SetActive(false);
            }
        }
        private void InitLevelData()
        {
            indexCount = 0;
            levelData = listLevelData[PlayerData.Instance.GetLevel()];
            CacheGameData.Instance.MaxChip = levelData.maxChip;
            indexTotalHexa = levelData.listStackChips.Count + levelData.emptyHexaTitle;
            cameraController.SetUpOnAwake(indexTotalHexa);
        }

        [Button]
        public void OnClickRefresh()
        {
            for (int i = 0; i < indexTotalHexa; i++)
            {
                chipBgs[i].Refresh();
            }

            SpawnHexa();
            CacheGameData.Instance.Refresh();
        }

        private void ClearDataChipBg()
        {
            for (int i = 0; i < indexTotalHexa; i++)
            {
                chipBgs[i].Refresh();
            }
        }

        private void SpawnHexa()
        {
            indexCount = 0;
            for (var i = 0; i < levelData.listStackChips.Count; i++)
            {
                var x = Instantiate(stackChipControllerPrefabs, chipBgs[i].transform);
                x.OnInit(levelData.listStackChips[i]);
                chipBgs[i].OnInit(x);
                indexCount++;
            }

            for (var i = 0; i < levelData.emptyHexaTitle; i++)
            {
                AddStackEmpty();
            }

            CacheGameData.Instance.IsCheckMove = false;
        }

        [Button]
        public void OnClickAddStackEmpty()
        {
            cameraController.SetMainCamera(indexCount);
            if (indexCount < chipBgs.Length)
            {
                chipBgs[indexCount].gameObject.SetActive(true);
                AddStackEmpty();
            }
            else SpawnText("Column Empty is max");
        }

        [Button]
        public void Test()
        {
            CacheGameData.Instance.IsCheckMove = true;
        }
        public void SpawnText(string text)
        {
            var x = Instantiate(textFullUiPrefab, textSpawnParent.transform);
            x.OnInit(text);
        }
        
        public void WinGame()
        {
            Invoke(nameof(OpenPopWin),1.5f);
        }
        
        public void OnClickNext()
        {
            PlayerData.Instance.SetLevel();
            int level =  PlayerData.Instance.GetLevel();
            if (level == 1)
            {
                popUpGamePlay.SetActiveButton(true);
            }
            else ClearDataChipBg();
            CacheGameData.Instance.Refresh();
            OnInit();
            popUpGamePlay.SetText();
        }
        public void LossGame()
        {
            Invoke(nameof(OpenPopUpLose),0.5f);
        }
        public void OpenPopWin()
        {
            animationWin.gameObject.SetActive(true);
        }
        public void OpenPopUpLose()
        {
            popUpLoss.gameObject.SetActive(true);
        }
    }
}