using System;
using System.Collections.Generic;
using _Development.Scripts.ThingsStore.Enum;
using UnityEngine;

namespace _Development.Scripts.ThingsStore.Data
{
    [Serializable]
    public class TypeContentThings
    {
        public TypeThings Type;
        public Sprite Icon;
        public List<RPGItem> Items = new();
    }
}