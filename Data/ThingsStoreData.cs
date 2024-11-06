using System.Collections.Generic;
using _Development.Scripts.ThingsStore.Enum;
using UnityEngine;

namespace _Development.Scripts.ThingsStore.Data
{
    [CreateAssetMenu(fileName = "ThingsStore", menuName = "ThingsStore/Data")]
    public class ThingsStoreData : ScriptableObject
    {
        public TypeThings Type = TypeThings.AllItems;
        public Sprite Icon;
        public List<TypeContentThings> FillingStore = new();
    }
}