using System.Collections.Generic;
using System.Linq;
using MadCore.API.Misc;
using MadCore.API.Registry;
using UnityEngine;

namespace MadCore.API.World.Item.Craft
{
    public class MadCraftData : IDHolder
    {

        
        public int CraftId;

        private ID _id;
        public Supplier<MadItem>[] Items;
        public CraftManager.CraftData CraftData;

        public MadCraftData(params Supplier<MadItem>[] items)
        {
            Items = items;
        }

        public CraftManager.CraftData BuildData()
        {
            var craftInfoList = new List<CraftInfo>();
            foreach (var supplier in Items)
            {
                var madItem = supplier.Invoke();
                if (madItem != null)
                {
                    var data = madItem.ItemData;
                    var craftInfo = data.GetComponent<CraftInfo>();
                    if (craftInfo != null)
                    {
                        craftInfoList.Add(craftInfo);
                    }
                }
            }
            CraftData = new CraftManager.CraftData
            {
                craftInfo = craftInfoList.ToArray(),
                craftName = _id.ToString()
            };
            return CraftData;
        }
        
        public void SetID(ID id)
        {
            _id = id;
        }

        public ID GetID()
        {
            return _id;
        }

        public override string ToString()
        {
            return _id.ToString();
        }
    }
}