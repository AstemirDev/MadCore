using System.Collections;
using HarmonyLib;
using MadCore.API.Registry;
using MadCore.API.World.FX;
using MadCore.API.World.Item;
using MadCore.API.World.Sound;
using UnityEngine;

namespace MadCore.API.Utils
{
    public static class PlayerUtils
    {
        public static IEnumerator Slash(PlayerMove player, MadFX fx, MadSound sound, Vector3 dir, float rot, float damageRate, int damageType, float fxTime, Vector3 offset = default (Vector3))
        {
            var tmpFX = fx.Spawn(player.transform.position + Vector3.up * offset.y);
            tmpFX.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, rot, 0.0f));
            tmpFX.transform.position += tmpFX.transform.TransformDirection(Vector3.left * offset.x);
            sound?.PlaySound3D(tmpFX.transform.position, false, SoundRegistry.Instance.SoundManager.soundBaseDist);
            var component1 = tmpFX.GetComponent<AttackCollider>();
            if (component1 != null)
            {
                component1.attacker = player.common;
                component1.damageRate = damageRate;
                component1.damageType = damageType;
            }
            var component2 = tmpFX.GetComponent<ConstantMove>();
            if (component2 != null)
                component2.dir = dir;
            var transform = tmpFX.transform.Find("Coll");
            if (transform != null)
                transform.gameObject.GetComponent<DamageTrigger>().friendry = true;
            var component3 = tmpFX.GetComponent<FXActive>();
            if (!(component3 != null) || component3.disableTime >= (double) fxTime)
            {
                yield return new WaitForSeconds(fxTime);
                tmpFX.gameObject.SetActive(false);
            }
        }
        
        public static IEnumerator HeavyAttack(MadItem item, PlayerMove pm)
        {
            var mn = MadIsland.Managers;
            pm.toolAction = true;
            mn.gameMN.nowAction = true;
            var normalHit = item.CanHitWithItem(pm);
            var isBow = false;
            var hitTime = item.HeavyAttackHitTime;
            if (mn.randChar.EquipBowCheck(pm.common))
            {
                isBow = true;
                normalHit = false;
                hitTime = 31f;
            }
            if (Camera.main == null) yield break;
            var traverse = Traverse.Create(pm);
            var clickDir = traverse.Field("clickDir");
            RaycastHit hit;
            clickDir.SetValue(Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0.0f, 0.0f, 5f)) - Camera.main.transform.position);
            Physics.Raycast(Camera.main.transform.position, clickDir.GetValue<Vector3>(), out hit, 100f,  pm.shootLayer);
            var cancel = false;
            var cancelable = mn.skillMN.skills[8] >= 2;
            pm.Rot(pm.target == null ? hit.point : pm.target.transform.position);
            var attackSpeed = (float) (1.0 + pm.common.status[2] * 0.004999999888241291);
            var tmpTime = hitTime;
            if (mn.skillMN.skills[8] >= 4)
            {
                tmpTime /= attackSpeed;
                pm.common.anim.state.TimeScale = attackSpeed;
            }
            while (tmpTime >= 0.0 && pm.common.dead == 0 && !cancel)
            {
                tmpTime -= Time.deltaTime * 30f;
                if (cancelable && Input.GetMouseButtonDown(1))
                    cancel = true;
                yield return  null;
            }
            if (cancel)
            {
                pm.toolAction = false;
                pm.common.anim.state.TimeScale = 1f;
                pm.StartCoroutine(pm.Guard());
            }
            else if (pm.common.dead != 0)
            {
                pm.toolAction = false;
                mn.gameMN.nowAction = false;
                pm.common.anim.state.TimeScale = 1f;
            }
            else
            {
                var tmpRate = 10002f;
                if (mn.skillMN.skills[8] >= 3)
                    tmpRate = mn.skillMN.skills[8] < 5 ? 10002.5f : (float) (10003.0 + (mn.skillMN.skills[8] - 5) * 0.019999999552965164);
                Vector3 attackDir;
                var attackRot = 0.0F;
                if (pm.target == null)
                {
                    if (hit.collider == null)
                    {
                        var vector2 = Input.mousePosition - Camera.main.WorldToScreenPoint(pm.transform.position);
                        attackDir = Vector3.Normalize(new Vector3(vector2.x, 0.0f, vector2.y));
                    }
                    else
                        attackDir = Vector3.Normalize(new Vector3(hit.point.x, 0.0f, hit.point.z) - new Vector3(pm.transform.position.x, 0.0f, pm.transform.position.z));
                    if (attackDir != Vector3.zero)
                    {
                        attackRot = Mathf.Atan2(attackDir.x, attackDir.z);
                        attackRot *= 57.29578f;
                        attackRot -= 90f;
                        if (normalHit)
                            mn.fxMN.GoAttack(pm.common, pm.transform.position + Vector3.up * (pm.common.attackScale.y / 2f) + attackDir * (pm.common.attackScale.x / 2f), pm.common.attackScale, attackRot, true, tmpRate);
                    }
                }
                else
                {
                    attackDir = Vector3.Normalize(new Vector3(pm.target.transform.position.x, 0.0f, pm.target.transform.position.z) - new Vector3(pm.transform.position.x, 0.0f, pm.transform.position.z));
                    attackRot = Mathf.Atan2(attackDir.x, attackDir.z);
                    attackRot *= 57.29578f;
                    attackRot -= 90f;
                    if (normalHit)
                        mn.fxMN.GoAttack(pm.common, pm.transform.position + Vector3.up * (pm.common.attackScale.y / 2f) + attackDir * (pm.common.attackScale.x / 2f), pm.common.attackScale, attackRot, true, tmpRate);
                }
                if (isBow)
                {
                    var slotID = mn.inventory.ArrowCheck();
                    if (slotID != -1)
                    {
                        var itemSlot = mn.inventory.itemSlot[slotID];
                        var hasArrow = item.CheckAmmo(pm, ID.Of(itemSlot.itemKey));
                        if (hasArrow) {
                            pm.StartCoroutine(item.OnHeavyShot(pm, attackDir, attackRot, tmpRate));
                        }
                        mn.inventory.ConsumeItem(slotID, 1);
                    }
                    else
                        pm.StartCoroutine(mn.eventMN.GoCaution(7));
                }
                else {
                    pm.StartCoroutine(item.OnHeavyAttack(pm, attackDir, attackRot, tmpRate));
                }
                pm.DurabilityCheck();
                var endTime = pm.common.anim.state.GetCurrent(0).AnimationEnd * 30f - hitTime;
                if (mn.skillMN.skills[8] >= 4)
                    endTime /= attackSpeed;
                while (endTime >= 0.0 && pm.common.dead == 0 && !cancel)
                {
                    endTime -= Time.deltaTime * 30f;
                    if (cancelable && Input.GetMouseButtonDown(1))
                        cancel = true;
                    yield return null;
                }
                pm.toolAction = false;
                pm.common.anim.state.TimeScale = 1f;
                if (cancel)
                {
                    pm.StartCoroutine(pm.Guard());
                }
                else
                {
                    if (pm.AttackCheck())
                        pm.common.anim.state.SetAnimation(0, pm.GetMoveAnim(0), true);
                    if (pm.target != null)
                    {
                        var component = pm.target.GetComponent<NPCMove>();
                        if (component != null && component.common.dead != 0)
                            pm.target = null;
                    }
                    mn.gameMN.nowAction = false;
                }
            }
        }
    }
}