using System.Collections;
using MadCore.API;
using MadCore.API.Misc;
using MadCore.API.Scripts;
using MadCore.API.World.Entity;
using MadCore.API.World.Item;
using UnityEngine;

namespace MadCore.Example.Item
{
    public class ItemRpg : MadItem
    {
        public ItemRpg()
        {
            Type = ItemData.ItemType.Tool;
            ToolType = ItemData.ToolType.Weapon;
            Name = LangString.Text("<color=green>RPG</color>");
            Lore = LangString.Text("Sounds great!");
            Durability = 800;
            AttackDamage = 50;
            Attachment = CustomAttachment.Load("Assets/attachments/rpg.json");
            HeavyAttackHitTime = 5;
            WhoCanEquip(NPCId.Yona, NPCId.Man);
            CraftRecipe(ItemMaterial.Of("wood_01", 4));
        }
        
        public override IEnumerator OnHeavyAttack(PlayerMove pm, Vector3 attackDir, float attackRot, float tempRate)
        {
            pm.common.anim.state.SetAnimation(0, "A_attack_gun", false);
            ExampleSounds.Missle.PlaySound3D(pm.transform.position, true, 40.0F);
            yield return new WaitForSeconds(0.15F);
            var missle = ExampleFX.MissleFX.Shoot(pm.common, pm.transform.position + Vector3.up * 0.6f, pm.AimingPosition(), 30.0F, 1.0F);
            missle.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, attackRot, 90.0f));
            missle.transform.position += missle.transform.TransformDirection(Vector3.left * -1.5F);
            var spawnCollision = missle.AddComponent<CustomSpawnCollision>();
            spawnCollision.Damager = pm.common;
            spawnCollision.DamageRate = 10005.0F;
            spawnCollision.SpawnFX = ExampleFX.NuclearFX;
            spawnCollision.SpawnSound = ExampleSounds.Explosion;
            spawnCollision.OnSpawned = (pos) => {
                MadIsland.CameraShaker.AddShakeSource(new PositionedShakeSource(pm, pos, 200, 0.3F, 0.99F, 0.95F));
            };
        }
    }
}