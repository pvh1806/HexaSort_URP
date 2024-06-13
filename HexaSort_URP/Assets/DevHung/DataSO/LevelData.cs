using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DevHung.DataSO
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/Level")]
    public class LevelData : ScriptableObject
    { 
        public List<StackChip> listStackChips;
        public int maxChip;
        public int emptyHexaTitle;
        public bool isHardLevel;
        [ShowIf("isHardLevel")] public int moveIndex;
        public int totalChipType;
    }

    [Serializable]
    public class Chip
    {
        public int number = 3;
        public int id;
        public ChipType ChipType => GetChipTye();
        public ChipType GetChipTye()
        {
            return id switch
            {
                0 => ChipType.Red,
                1 => ChipType.DarkBlue,
                2 => ChipType.Green,
                3 => ChipType.Yellow,
                4 => ChipType.Purple,
                5 => ChipType.Black,
                6 => ChipType.LightBlue,
                7 => ChipType.Gray,
                _ => ChipType.None
            };
        }
    }

    [Serializable]
    public class StackChip
    {
        [TableList] public List<Chip> listChips;
    }
    public enum ChipType
    {
        Red,
        DarkBlue,
        Yellow,
        LightBlue,
        Purple,
        Green,
        Black,
        Gray,
        None,
    }
}
