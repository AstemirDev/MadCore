using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace MadCore.API.World.Entity
{
    public class NPCExtension : MonoBehaviour
    {
        public CommonStates CommonState;
        private bool _shouldUpdateStatus;
        private bool _syncMaxHealth;
        private bool _isGod;

        public void Start()
        {
        }

        public void Update()
        {
            if (_isGod)
            {
                CommonState.life = CommonState.maxLife;
            }
            if (_syncMaxHealth)
            {
                _syncMaxHealth = false;
                CommonState.life = CommonState.maxLife;
            }
            if (_shouldUpdateStatus)
            {
                _shouldUpdateStatus = false;
                var gameMN = MadIsland.Managers.gameMN;
                gameMN.LifeImageChange();
                gameMN.FaintImageChange();
                gameMN.ExpImageChange();
                gameMN.PlayerStatesCheck();
                MadIsland.Managers.uiMN.StatusChange(CommonState);
            }
        }

        
        public void ToggleGod()
        {
            _isGod = !_isGod;
            SyncMaxHealth();
        }
        
        public void SendUpdateStatus()
        {
            _shouldUpdateStatus = true;
        }
        
        public void SyncMaxHealth()
        {
            _syncMaxHealth = true;
        }

        public float GetMaxSpeed(bool limit = false, bool player = false)
        {
            var totalSpeed = (float) (CommonState.speed * (1.0 - CommonState.debuff.speed) * (player ? 1.0 - CommonState.debuff.weight : 1.0F) * (1.0 - CommonState.debuff.slowDamage));
            if (limit)
                totalSpeed = Mathf.Clamp(totalSpeed, 0.01f, CommonState.speedLimit);
            return _isGod ? 5.0F : totalSpeed;
        }

        public virtual bool IsPlayer()
        {
            return false;
        }
        
        public static NPCExtension GetOrCreate(CommonStates common)
        {
            if (common.GetComponent<NPCExtension>() == null)
            {
                var npcExtension = common.gameObject.AddComponent<NPCExtension>();
                npcExtension.CommonState = common;
                return npcExtension;
            }
            return common.GetComponent<NPCExtension>();
        }
    }
}