using System;
using UnityEngine;
using UnityEngine.UI;

namespace DevHung.Scripts.UI
{
    public class PopUpLoss : MonoBehaviour
    {
       [SerializeField] private Button btnPlayOn, btnRevive;

       private void Start()
       {
           btnPlayOn.onClick.AddListener(OnClickLoss);
           btnRevive.onClick.AddListener(OnClickLoss);
       }

       private void OnClickLoss()
       {
           LevelController.Instance.OnClickRefresh();
           gameObject.SetActive(false);
       }
    }
}