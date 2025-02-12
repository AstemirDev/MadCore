using System.Collections;
using MadCore.API.Misc;
using MadCore.API.Registry;
using MadCore.API.Utils;
using MadCore.API.World.Entity;
using MadCore.API.World.Item.Craft;
using UnityEngine;

namespace MadCore.API.World.Item
{
    public class MadItem : IDHolder
    {
        public ItemData.ItemType Type = ItemData.ItemType.None;
        
        public ItemData.SubType SubType = ItemData.SubType.None;
        
        public ItemData.ToolType ToolType = ItemData.ToolType.None;
        
        public LangString Name = new LangString();
        
        public LangString Lore = new LangString();
        
        public int MaxStack = 1;
        
        public int Durability = 0;
        
        public int AttackDamage = 0;

        public float HeavyAttackHitTime = 20.0F;
        
        public CustomAttachment Attachment;
        
        public GameObject PlacePrefab;

        public Supplier<MadCraftData> CraftData;

        private ItemMaterial[] _craftMaterials = {};
        
        private ItemMaterial[] _traderPrice = {};
        
        private NPCId[] _whoCanEquipIds = {};
        
        private ID _id;
        
        private ItemData _data;
        
        public void BuildData()
        {
            var temp = new GameObject(_id.ToString());
            var equipInfo = temp.AddComponent<EquipInfo>();
            equipInfo.npcID = new int[_whoCanEquipIds.Length];
            for (var i = 0; i < _whoCanEquipIds.Length; i++)
            {
                equipInfo.npcID[i] = (int)_whoCanEquipIds[i];
            }
            _data = temp.AddComponent<ItemData>();
            _data.maxStack = MaxStack;
            _data.itemLife = Durability;
            _data.attack = AttackDamage;
            _data.itemObj = PlacePrefab;
            _data.itemType = Type;
            _data.toolType = ToolType;
            _data.subType = SubType;
            _data.localizeName = Name.ToArray();
            _data.localizedTipsText = Lore.ToArray();
            _data.itemName = _data.localizeName.Length != 0 ? _data.localizeName[0] : _id.ToString();
            _data.TipsText = _data.localizedTipsText.Length != 0 ? _data.localizedTipsText[0] : "Item has no description";
            _data.itemSprite = AssetUtils.LoadSprite($"Assets/textures/icons/{_id.Path}.png");
            var craftInfo = temp.AddComponent<CraftInfo>();
            craftInfo.required = ItemMaterial.AllRequired(_craftMaterials);
            craftInfo.shopTrade = ItemMaterial.AllRequired(_traderPrice);
            var additionalData = temp.AddComponent<ItemDataAdditional>();
            additionalData.ItemId = _id;
        }

        public MadItem CraftRecipe(params ItemMaterial[] materials)
        {
            _craftMaterials = materials;
            return this;
        }
    
        public MadItem WhoCanEquip(params NPCId[] npcIds)
        {
            _whoCanEquipIds = npcIds;
            return this;
        }
        
        public ItemData ItemData => _data;
        public bool IsNull => this == ItemRegistry.NullItem;

        public ID GetID()
        {
            return _id;
        }

        public void SetID(ID id)
        {
            _id = id;
        }
        
        public override string ToString() { return _id.ToString(); }

        public virtual IEnumerator OnHeavyAttack(PlayerMove pm, Vector3 attackDir, float attackRot, float tempRate) { yield return null;}
        public virtual IEnumerator OnHeavyShot(PlayerMove pm, Vector3 attackDir, float attackRot, float tempRate) { yield return null; }
        public virtual bool CanHitWithItem(PlayerMove pm) { return false; }
        public virtual bool CheckAmmo(PlayerMove pm, ID itemId) { return false; }
    }
}