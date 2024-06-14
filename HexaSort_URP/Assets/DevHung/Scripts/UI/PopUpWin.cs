using System;
using DevHung.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace DevHung.Scripts.UI
{
    public class PopUpWin : MonoBehaviour
    {
        [SerializeField] private Button btnNext, btnX5;

        private void Start()
        {
            btnNext.onClick.AddListener(OnClickWin);
            btnX5.onClick.AddListener(OnClickWin);
        }
        private void OnClickWin()
        {
            LevelController.Instance.OnClickNext();
            gameObject.SetActive(false);
        }
    }
}