using System.Collections.Generic;
using System.Linq;
using DevHung.Scripts.Chip;
using DevHung.Scripts.Manager;
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
        public readonly Dictionary<Chip.Chip, float> ChipUp = new();
        public bool IsCheckMove = true;
        private StackChipController currentStackChip;
        private const float TimeDelayChipMove = 0.1f;
        private readonly Stack<StackState> previousStates = new();
        private List<StackChipController> stackChipControllersComplete = new();
        private int numberComplete;
        private PopUpGamePlay popUpGamePlay;
        private int countMove = -1;

        public void CheckStackListChip(StackChipController stackChipControllerTarget)
        {
            IsCheckMove = true;
            if (currentStackChip == null)
            {
                currentStackChip = stackChipControllerTarget;
                if (currentStackChip.GetStackChip() != null && currentStackChip.GetStackChip().Count == MaxChip)
                {
                    currentStackChip = null;
                    IsSelect = false;
                    IsCheckMove = false;
                }
                else DoAnimationStack();

                return;
            }

            if (stackChipControllerTarget != currentStackChip)
            {
                if (!IsSelect)
                {
                    currentStackChip = stackChipControllerTarget;
                    DoAnimationStack();
                }
                else if (IsUpAnimation && currentStackChip != null)
                {
                    currentStackChip.DoAnimationDownStack();
                    currentStackChip = null;
                    IsCheckMove = false;
                }
                else CheckMoveStack(stackChipControllerTarget);
            }
            else DoAnimationStack();
        }

        private void DoAnimationStack()
        {
            var y = currentStackChip.GetStackChip();
            if (y == null) return;
            if (IsSelect) currentStackChip.DoAnimationDownStack();
            else currentStackChip.DoAnimationUpStack(y);
            IsCheckMove = false;
        }

        private void CheckMoveStack(StackChipController stackChipControllerTarget)
        {
            if (stackChipControllerTarget.GetStackChip() == null)
            {
                Vector3 target = stackChipControllerTarget.transform.position + new Vector3(0, 0.005f, 0);
                stackChipControllerTarget.AddStack();
                RemoveStack(stackChipControllerTarget, currentStackChip.GetStackChip().Count - 1, target,
                    SetFloatTimerMove(currentStackChip.GetStackChip().Count - 1));
            }
            else
            {
                if (stackChipControllerTarget.GetStackChip().Peek().ChipType !=
                    currentStackChip.GetStackChip().Peek().ChipType)
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

                if (stackChipControllerTarget.GetCountList() + currentStackChip.GetStackChip().Count > MaxChip)
                {
                    int countList = MaxChip - stackChipControllerTarget.GetCountList();
                    Vector3 target = stackChipControllerTarget.GetStackChip().Peek().transform.position +
                                     new Vector3(0, 0.005f, 0);
                    RemoveStack(stackChipControllerTarget, countList - 1, target, SetFloatTimerMove(countList - 1));
                }
                else
                {
                    Vector3 target = stackChipControllerTarget.GetStackChip().Peek().transform.position +
                                     new Vector3(0, 0.005f, 0);
                    RemoveStack(stackChipControllerTarget, currentStackChip.GetStackChip().Count - 1, target,
                        SetFloatTimerMove(currentStackChip.GetStackChip().Count - 1));
                }
            }
        }

        private void RemoveStack(StackChipController stackChipControllerTarget, int index, Vector3 target,
            float timerMove, bool unDo = false)
        {
            if (index < 0 || currentStackChip == null)
            {
                return;
            }

            var x = currentStackChip.GetStackChip().Pop();
            x.gameObject.transform.SetParent(stackChipControllerTarget.transform);
            stackChipControllerTarget.GetStackChip().Push(x);
            ChipUp.Remove(x);
            if (index > 0)
            {
                x.MoveChip(target, timerMove, new Vector3(0, 0, 180));
            }

            if (index == 0)
            {
                x.MoveChip(target, timerMove, new Vector3(0, 0, 180), () =>
                {
                    CheckStackControllerTarget(stackChipControllerTarget, unDo);
                    CheckCurrentChip(unDo);
                    IsCheckMove = false;
                    currentStackChip = null;
                });
            }
            else
            {
                DOVirtual.DelayedCall(TimeDelayChipMove,
                    () =>
                    {
                        RemoveStack(stackChipControllerTarget, index - 1, target + new Vector3(0, 0.005f, 0), timerMove,
                            unDo);
                    });
            }
        }

        private void CheckCurrentChip(bool unDo)
        {
            if (currentStackChip.GetStackChip().Count == 0)
            {
                currentStackChip.RemoveStack();
                if (!unDo && previousStates.Count > 0) previousStates.Peek().IsAddNewStack = true;
                IsSelect = false;
            }
            else currentStackChip.DoAnimationDownStack();

            if (!unDo && countMove != -1) CheckLoss();
        }

        private void CheckStackControllerTarget(StackChipController stackChipControllerTarget, bool unDo)
        {
            if (stackChipControllerTarget.GetStackChip().Count == MaxChip)
            {
                stackChipControllerTarget.GetComponentInParent<ChipBG>().FullStack();
                stackChipControllersComplete.Add(stackChipControllerTarget);
                numberComplete++;
                if (numberComplete == LevelController.Instance.levelData.totalChipType)
                {
                    for (int i = 0; i < stackChipControllersComplete.Count; i++)
                    {
                        stackChipControllersComplete[i].MoveAllChip();
                    }

                    LevelController.Instance.WinGame();
                    numberComplete = 0;
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
                        TargetStack = currentStackChip,
                        IsAddNewStack = false
                    });
            }
        }

        public void Refresh()
        {
            currentStackChip = null;
            IsCheckMove = false;
            IsSelect = false;
            previousStates.Clear();
            ClearStackComplete();
        }

        public void SetUpCountMove(int _countMove, PopUpGamePlay _popUpGamePlay)
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
            currentStackChip = lastState.SourceStack;
            Vector3 target = lastState.TargetStack.GetStackChip().Peek().transform.position + new Vector3(0, 0.005f, 0);
            ;
            if (lastState.IsAddNewStack) lastState.TargetStack.AddStack();
            IsCheckMove = true;
            if (countMove > 0)
            {
                countMove++;
                popUpGamePlay.SetTextMove(countMove);
            }

            RemoveStack(lastState.TargetStack, currentStackChip.GetStackChip().Count - 1, target,
                SetFloatTimerMove(currentStackChip.GetStackChip().Count - 1), true);
        }

        private float SetFloatTimerMove(int _countMove)
        {
            return _countMove <= 3 ? 0.3f : 0.15f;
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

        public void ClearStackComplete()
        {
            stackChipControllersComplete.Clear();
        }
        private class StackState
        {
            public StackChipController SourceStack { get; set; }
            public StackChipController TargetStack { get; set; }
            public bool IsAddNewStack { get; set; }
        }
    }
}