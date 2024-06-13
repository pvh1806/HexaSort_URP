using System;
using DevHung.DataSO;
using DG.Tweening;
using UnityEngine;

namespace DevHung.Scripts
{
    public class Chip : MonoBehaviour
    {
        [SerializeField] private float duration;
        public ChipType ChipType { get; set; }
        public void Spawn(float timeDuration , StackChipController stackChipController ,ChipType _chipType)
        {
            transform.DOScale(Vector3.one, timeDuration).OnComplete(stackChipController.CheckSpawnComplete);
            ChipType = _chipType;
        }
        public void MoveChip(Vector3 targetPos, float timeMove, Vector3 rotate, Action completed = null)
        {
            transform.DORotate(transform.eulerAngles + rotate, timeMove, RotateMode.FastBeyond360).SetEase(Ease.InOutSine);
            transform.DOJump(targetPos, 0.05f, 1, timeMove).SetEase(Ease.InOutSine).OnComplete(() => completed?.Invoke());
        }
    }
}
