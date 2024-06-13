using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace DevHung.Scripts
{
    public class CacheGameData : ProjectSingleton<CacheGameData>
    {
        public bool IsSelect { get; set; }
        public bool IsUpAnimation;
        public readonly Dictionary<Chip, float> ChipUp = new();
        public bool IsCheckMove = true;
        public int MaxChip;
        private StackChipController currentStackChip;
        private const float TimeDelayChipMove = 0.1f;
        private Stack<StackState> previousStates = new Stack<StackState>();
        private int numberComplete;
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
                    return;
                }

                DoAnimationStack();
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
                RemoveStack(stackChipControllerTarget, currentStackChip.GetStackChip().Count - 1, target);
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
                    int countMove = MaxChip - stackChipControllerTarget.GetCountList();
                    Vector3 target = stackChipControllerTarget.GetStackChip().Peek().transform.position +
                                     new Vector3(0, 0.005f, 0);
                    RemoveStack(stackChipControllerTarget, countMove - 1, target);
                }
                else
                {
                    Vector3 target = stackChipControllerTarget.GetStackChip().Peek().transform.position +
                                     new Vector3(0, 0.005f, 0);
                    RemoveStack(stackChipControllerTarget, currentStackChip.GetStackChip().Count - 1, target);
                }
            }
        }

        private void RemoveStack(StackChipController stackChipControllerTarget, int index, Vector3 target, bool unDo = false)
        {
            if (index < 0 || currentStackChip == null)
            {
                return;
            }

            var x = currentStackChip.GetStackChip().Pop();
            x.gameObject.transform.SetParent(stackChipControllerTarget.transform);
            stackChipControllerTarget.GetStackChip().Push(x);
            x.MoveChip(target, 0.3f, new Vector3(0, 0, 180));
            ChipUp.Remove(x);
            if (index == 0)
            {
                if (stackChipControllerTarget.GetStackChip().Count == MaxChip)
                {
                    stackChipControllerTarget.GetComponentInParent<ChipBG>().FullStack();
                    IsCheckMove = false;
                    IsSelect = false;
                    numberComplete++;
                    if (numberComplete == LevelController.Instance.levelData.totalChipType)
                    {
                        numberComplete = 0;
                        LevelController.Instance.Win();
                    }
                    if (!unDo) previousStates.Clear();
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
                if (currentStackChip.GetStackChip().Count == 0)
                {
                    currentStackChip.RemoveStack();
                    if (!unDo) previousStates.Peek().IsAddNewStack = true;
                    IsSelect = false;
                }
                else
                {
                    currentStackChip.DoAnimationDownStack();
                }
                
                IsCheckMove = false;
                currentStackChip = null;
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

        public void Refresh()
        {
            currentStackChip = null;
            IsCheckMove = false;
            IsSelect = false;
            previousStates.Clear();
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
            Vector3 target = lastState.TargetStack.GetStackChip().Peek().transform.position + new Vector3(0, 0.005f, 0);;
            if (lastState.IsAddNewStack) lastState.TargetStack.AddStack();
            IsCheckMove = true;
            RemoveStack(lastState.TargetStack,currentStackChip.GetStackChip().Count-1,target,true);
        }

        private class StackState
        {
            public StackChipController SourceStack { get; set; }
            public StackChipController TargetStack { get; set; }
            public bool IsAddNewStack { get; set; }
        }
    }
}