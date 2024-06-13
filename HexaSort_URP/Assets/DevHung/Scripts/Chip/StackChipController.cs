using System;
using System.Collections.Generic;
using System.Linq;
using DevHung.DataSO;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DevHung.Scripts
{
    public class StackChipController : MonoBehaviour
    {
        [SerializeField] private float increaseY;
        private Stack<Chip> currentStack;
        private int chipIndex, chipCount;
        private StackChip stackChipData;
        private readonly List<Stack<Chip>> stackChipList = new();
        public void OnInit(StackChip stackChip)
        {
            stackChipData = stackChip;
            InitializeNewStack();
        }

        [Button]
        public void CheckSpawnComplete()
        {
            if (chipIndex >= stackChipData.listChips.Count) return;

            if (chipCount < stackChipData.listChips[chipIndex].number -1)
            {
                chipCount++;
            }
            else
            {
                chipIndex++;
                if (chipIndex >= stackChipData.listChips.Count) return;
                chipCount = 1;
                InitializeNewStack();
            }

            GenerateStackChipObj(stackChipData.listChips[chipIndex].ChipType);
        }

        public Stack<Chip> GetStackChip()
        {
            return stackChipList.Count == 0 ? null : stackChipList[^1];
        }

        public void DoAnimationUpStack(Stack<Chip> stackChip)
        {
            CacheGameData.Instance.IsUpAnimation = true;
            CacheGameData.Instance.IsSelect = true;
            var chipList = stackChip.ToList();
            AnimateChipRecursive(chipList, 0);
        }
        
        private void AnimateChipRecursive(List<Chip> chipList, int index)
        {
            if (index > chipList.Count) return;
            var chip = chipList[index];
            CacheGameData.Instance.ChipUp.Add(chip, chip.transform.localPosition.y);
            if (index < chipList.Count)
            {
                DOVirtual.DelayedCall(0.1f, () =>
                {
                    MoveChip(chipList, index);
                    if(!CacheGameData.Instance.IsSelect) return;
                    AnimateChipRecursive(chipList, index + 1);
                });
            }
            if (index == chipList.Count -1 )
            {
                MoveChip(chipList, index,(() =>
                {
                    CacheGameData.Instance.IsUpAnimation = false;
                }));
            }
        }

        private void MoveChip(List<Chip> chipList, int index, Action completed = null)
        {
            var chip = chipList[index];
            chip.transform.DOLocalMove(new Vector3(0, chip.transform.localPosition.y + increaseY, 0), 0.1f).SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    completed?.Invoke();
                });
        }
 
        [Button]
        public int GetCountList()
        {
            int count = 0;
            for (int i = 0; i < stackChipList.Count; i++)
            {
                count += stackChipList[i].Count;
            }
            return count;
        }
        public void DoAnimationDownStack()
        {
            foreach (var entry in CacheGameData.Instance.ChipUp)
            {
                Chip chip = entry.Key;
                float value = entry.Value;
                chip.transform.DOLocalMove(new Vector3(0, value, 0), 0.2f).SetEase(Ease.OutExpo);
            }
            
            CacheGameData.Instance.ChipUp.Clear();
            CacheGameData.Instance.IsSelect = false;
        }

        public void AddStack()
        {
            currentStack = new Stack<Chip>();
            stackChipList.Add(currentStack);
        }

        public void RemoveStack()
        {
            stackChipList.RemoveAt(stackChipList.Count-1);
        }
        private void InitializeNewStack()
        {
            AddStack();
            GenerateStackChipObj(stackChipData.listChips[chipIndex].ChipType);
        }

        private void GenerateStackChipObj(ChipType chipType)
        {
            var prefab = Resources.Load<GameObject>($"Chips/Hex_Chip_{chipType}");
            if (prefab == null) return;

            var posSpawn = Vector3.zero;
            switch (currentStack.Count)
            {
                case 0 when stackChipList.Count >= 2:
                    posSpawn = stackChipList[^2].Peek().transform.localPosition + new Vector3(0, increaseY, 0);
                    break;
                case > 0:
                    posSpawn = currentStack.Peek().transform.localPosition + new Vector3(0, increaseY, 0);
                    break;
                default:
                    posSpawn += new Vector3(0, increaseY, 0);
                    break;
            }

            var chipObject = Instantiate(prefab, transform);
            chipObject.transform.localPosition = posSpawn;

            var chip = chipObject.GetComponent<Chip>();
            if (chip == null) return;
            chip.Spawn(0.2f, this, chipType);
            currentStack.Push(chip);
        }
    }
}