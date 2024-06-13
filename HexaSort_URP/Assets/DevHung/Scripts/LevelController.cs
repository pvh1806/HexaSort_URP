using DevHung.DataSO;
using DevHung.Scripts.Data;
using DevHung.Scripts.DesginPattern;
using DevHung.Scripts.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DevHung.Scripts
{
    public class LevelController : MonoSingleton<LevelController>
    {
        [SerializeField] private LevelData[] listLevelData;
        [SerializeField] private StackChipController stackChipControllerPrefabs;
        [SerializeField] private ChipBG[] chipBgs;
        [SerializeField] private GameObject textSpawnParent;
        [SerializeField] private TextFullUi textFullUiPrefab;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private PopUpGamePlay popUpGamePlay;
        private int indexCount;
        private int indexTotalHexa;
        public LevelData levelData;

        public void OnInit()
        {
            InitLevelData();
            HideAll();
            for (int i = 0; i < indexTotalHexa; i++)
            {
                chipBgs[i].gameObject.SetActive(true);
            }

            SpawnHexa();
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

        public void ClearDataChipBg()
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
            Debug.Log( CacheGameData.Instance.IsCheckMove);
            Debug.Log( CacheGameData.Instance.IsSelect);
            //Debug.Log( CacheGameData.Instance.IsSelect);
        }

        public void SpawnText(string text)
        {
            var x = Instantiate(textFullUiPrefab, textSpawnParent.transform);
            x.OnInit(text);
        }
        
        public void Win()
        {
            popUpGamePlay.OnClickNext();
        }
    }
}