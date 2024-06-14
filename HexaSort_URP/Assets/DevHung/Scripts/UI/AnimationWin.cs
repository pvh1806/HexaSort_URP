using System;
using DevHung.Scripts.Manager.Audio;
using DG.Tweening;
using UnityEngine;

namespace DevHung.Scripts.UI
{
    public class AnimationWin : MonoBehaviour
    {
        [SerializeField] private PopUpWin popUpWin;
        [SerializeField] private GameObject title;
        private void OnEnable()
        {
            AudioManager.instance.Play("Win");
            title.transform.DOLocalMove(Vector3.zero, 3f).OnComplete(() =>
            {
                title.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 2f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    title.transform.localScale = Vector3.one;
                    popUpWin.gameObject.SetActive(true);
                });
            });
        }
    }
}