using System.Collections.Generic;
using System.Linq;
using DevHung.Scripts.UI;
using DG.Tweening;
using UnityEngine;

namespace DevHung.Scripts
{
    public class CacheGameData : ProjectSingleton<CacheGameData>
    {
        public bool IsSelect { get; set; }
        public int MaxChip { get; set; }
        public bool IsUpAnimation;
        public readonly Dictionary<Chip, float> ChipUp = new();
        public bool IsCheckMove = true;
        public StackChipController CurrentStackChip;
        private const float TimeDelayChipMove = 0.1f;
        private readonly Stack<StackState> previousStates = new ();
        private int numberComplete;
        private PopUpGamePlay popUpGamePlay;
        private int countMove = -1;
        public void CheckStackListChip(StackChipController stackChipControllerTarget)
        {
            IsCheckMove = true;
            if (CurrentStackChip == null)
            {
                CurrentStackChip = stackChipControllerTarget;
                if (CurrentStackChip.GetStackChip() != null && CurrentStackChip.GetStackChip().Count == MaxChip)
                {
                    CurrentStackChip = null;
                    IsSelect = false;
                    IsCheckMove = false;
                }
                else DoAnimationStack();
                return;
            }

            if (stackChipControllerTarget != CurrentStackChip)
            {
                if (!IsSelect)
                {
                    CurrentStackChip = stackChipControllerTarget;
                    DoAnimationStack();
                }
                else if (IsUpAnimation && CurrentStackChip != null)
                {
                    CurrentStackChip.DoAnimationDownStack();
                    CurrentStackChip = null;
                    IsCheckMove = false;
                }
                else CheckMoveStack(stackChipControllerTarget);
            }
            else DoAnimationStack();
        }

        private void DoAnimationStack()
        {
            var y = CurrentStackChip.GetStackChip();
            if (y == null) return;
            if (IsSelect) CurrentStackChip.DoAnimationDownStack();
            else CurrentStackChip.DoAnimationUpStack(y);
            IsCheckMove = false;
        }

        private void CheckMoveStack(StackChipController stackChipControllerTarget)
        {
            if (stackChipControllerTarget.GetStackChip() == null)
            {
                Vector3 target = stackChipControllerTarget.transform.position + new Vector3(0, 0.005f, 0);
                stackChipControllerTarget.AddStack();
                RemoveStack(stackChipControllerTarget, CurrentStackChip.GetStackChip().Count - 1, target);
            }
            else
            {
                if (stackChipControllerTarget.GetStackChip().Peek().ChipType !=
                    CurrentStackChip.GetStackChip().Peek().ChipType)
                {
                    DoAnimationStack();
                    return;
                }

                if (stackChipControllerTarget.GetCountList() == MaxChip)
                {
                    LevelController.Instance.SpawnText("Column full , Cant's move");
                    DoAnimationStack();
                    return;
                }

                if (stackChipControllerTarget.GetCountList() + CurrentStackChip.GetStackChip().Count > MaxChip)
                {
                    int countMove = MaxChip - stackChipControllerTarget.GetCountList();
                    Vector3 target = stackChipControllerTarget.GetStackChip().Peek().transform.position +
                                     new Vector3(0, 0.005f, 0);
                    RemoveStack(stackChipControllerTarget, countMove - 1, target);
                }
                else
                {
                    Vector3 target = stackChipControllerTarget.GetStackChip().Peek().transform.position +
                                     new Vector3(0, 0.005f, 0);
                    RemoveStack(stackChipControllerTarget, CurrentStackChip.GetStackChip().Count - 1, target);
                }
            }
        }
        
        private void RemoveStack(StackChipController stackChipControllerTarget, int index, Vector3 target, bool unDo = false)
        {
            if (index < 0 || CurrentStackChip == null)
            {
                return;
            }
            var x = CurrentStackChip.GetStackChip().Pop();
            x.gameObject.transform.SetParent(stackChipControllerTarget.transform);
            stackChipControllerTarget.GetStackChip().Push(x);
            ChipUp.Remove(x);
            if (index > 0)
            {
                x.MoveChip(target, 0.3f, new Vector3(0, 0, 180));
            }
            if (index == 0)
            {
                x.MoveChip(target, 0.3f, new Vector3(0, 0, 180), () =>
                {
                    CheckStackControllerTarget(stackChipControllerTarget,unDo);
                    CheckCurrentChip(unDo);
                    IsCheckMove = false;
                    CurrentStackChip = null;
                });
            }
            else
            {
                DOVirtual.DelayedCall(TimeDelayChipMove,
                    () =>
                    {
                        RemoveStack(stackChipControllerTarget, index - 1, target + new Vector3(0, 0.005f, 0), unDo);
                    });
            }
        }

        private void CheckCurrentChip(bool unDo)
        {
            if (CurrentStackChip.GetStackChip().Count == 0)
            {
                CurrentStackChip.RemoveStack();
                if (!unDo && previousStates.Count > 0) previousStates.Peek().IsAddNewStack = true;
                IsSelect = false;
            }
            else CurrentStackChip.DoAnimationDownStack();
            if(!unDo && countMove != -1) CheckLoss();
        }
        private void CheckStackControllerTarget(StackChipController stackChipControllerTarget, bool unDo)
        {
            if (stackChipControllerTarget.GetStackChip().Count == MaxChip)
            {
                stackChipControllerTarget.GetComponentInParent<ChipBG>().FullStack();
                numberComplete++;
                if (numberComplete == LevelController.Instance.levelData.totalChipType)
                {
                    numberComplete = 0;
                    LevelController.Instance.WinGame();
                }
                if (!unDo) previousStates.Clear();
                IsSelect = false;
            }
            else
            {
                if (!unDo)
                    previousStates.Push(new StackState
                    {
                        SourceStack = stackChipControllerTarget,
                        TargetStack = CurrentStackChip,
                        IsAddNewStack = false
                    });
            }
        }
        public void Refresh()
        {
            CurrentStackChip = null;
            IsCheckMove = false;
            IsSelect = false;
            previousStates.Clear();
        }

        public void SetUpCountMove(int _countMove , PopUpGamePlay _popUpGamePlay)
        {
            countMove = _countMove;
            popUpGamePlay = _popUpGamePlay;
        }
        public void UndoStack()
        {
            if (previousStates.Count == 0 || IsCheckMove)
            {
                LevelController.Instance.SpawnText("Undo is over");
                return;
            }
            var lastState = previousStates.Pop();
            CurrentStackChip = lastState.SourceStack;
            Vector3 target = lastState.TargetStack.GetStackChip().Peek().transform.position + new Vector3(0, 0.005f, 0);;
            if (lastState.IsAddNewStack) lastState.TargetStack.AddStack();
            IsCheckMove = true;
            if (countMove > 0)
            {
                countMove++;
                popUpGamePlay.SetTextMove(countMove);
            }
            RemoveStack(lastState.TargetStack,CurrentStackChip.GetStackChip().Count-1,target,true);
        }

        private void CheckLoss()
        {
            switch (countMove)
            {
                case > 0:
                    countMove--;
                    popUpGamePlay.SetTextMove(countMove);
                    break;
                case 0:
                    LevelController.Instance.LossGame();
                    break;
            }
        }
        private class StackState
        {
            public StackChipController SourceStack { get; set; }
            public StackChipController TargetStack { get; set; }
            public bool IsAddNewStack { get; set; }
        }
    }
}