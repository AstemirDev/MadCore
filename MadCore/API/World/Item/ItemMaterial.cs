using System;
using MadCore.API.Registry;

namespace MadCore.API.World.Item
{
    public class ItemMaterial
    {
        private ID _id;
        private int _amount;

        public ItemMaterial(ID id, int amount = 1)
        {
            _id = id;
            _amount = amount;
        }

        public ItemMaterial(MadItem item, int amount = 1)
        {
            _id = item.GetID();
            _amount = amount;
        }

        public ItemData ItemData => ItemRegistry.Instance.ItemManager.FindItem(_id.ToString());

        public CraftInfo.Required Required{
            get
            {
                var required = new CraftInfo.Required
                {
                    count = _amount,
                    itemData = ItemData
                };
                return required;
            }
        }

        public static CraftInfo.Required[] AllRequired(ItemMaterial[] materials)
        {
            var required = new CraftInfo.Required[materials.Length];
            for (var i = 0; i < materials.Length; i++)
            {
                required[i] = materials[i].Required;
            }
            return required;
        }

        public static ItemMaterial Of(string name, int amount = 1)
        {
            return new ItemMaterial(ID.Of(name), amount);
        }

        public static ItemMaterial Of(ID id, int amount = 1)
        {
            return new ItemMaterial(id, amount);
        }
    }
}