using System;
using DevHung.Scripts.Manager;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DevHung.Scripts.UI
{
    public class TutorialLevel1 : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Button btn;
        [SerializeField] private Image icTick;
        private int indexCount;

        private void Start()
        {
            btn.onClick.AddListener(OnClick);
        }

        private void DoMove()
        {
            btn.gameObject.transform.DOMove(target.position, 0.1f);
        }

        private void OnClick()
        {
            switch (indexCount)
            {
                case 0:
                    CacheGameData.Instance.CheckStackListChip(LevelController.Instance.SetupStackControllerToTutorial(0));
                    DoMove();
                    indexCount++;
                    CacheGameData.Instance.IsCheckMove = true;
                    icTick.gameObject.SetActive(true);
                    break;
                case 1:
                    CacheGameData.Instance.CheckStackListChip(LevelController.Instance.SetupStackControllerToTutorial(1));
                    Destroy(gameObject);
                    break;
            }
        }
    }
}