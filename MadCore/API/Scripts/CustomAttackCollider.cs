using System;
using System.Collections;
using MadCore.API.Misc;
using MadCore.API.World.FX;
using MadCore.API.World.Sound;
using UnityEngine;

namespace MadCore.API.Scripts
{
    public class CustomAttackCollider : MonoBehaviour
    {
        public CommonStates Damager;

        public MadSound SoundFX;
        
        public bool AttackOnce;
        
        public bool IsBullet;
        
        public bool IsArrow;

        public float DamageRate = 1f;
        
        public float DistDampRate;
        
        public float DampStart;

        public DamageType DamageType = DamageType.Normal;
        
        private CommonStates _damaged;

        private Rigidbody _rigidbody;
        
        private bool _arrowRotation = true;
        
        private bool _isAttacked;
        
        private bool _isGrounded;
        
        private void Start()
        {
            if (IsArrow && _rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();
        }
        
        private void OnEnable() => this.StartCoroutine(this.ArrowRotation());

        private void OnDisable()
        {
            _isAttacked = false;
            _isGrounded = false;
            _arrowRotation = true;
            if (_rigidbody != null)
            {
                _rigidbody.velocity = Vector3.zero;
            }
            if (!IsArrow)
            {
                return;
            }
            var colliders = GetComponents<BoxCollider>();
            colliders[0].enabled = true;
            colliders[1].enabled = true;
            var fxMN = FXRegistry.Instance.FXManager;
            if (fxMN == null)
                return;
            fxMN.StartCoroutine(fxMN.PoolBack(gameObject));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isAttacked || Damager == null)
                return;
            if (other.gameObject == Damager.gameObject) return;
            var targetTag = other.tag;
            if (targetTag == "Player")
            {
                _damaged = other.GetComponent<CommonStates>();
                if (TryToDamagePlayer(other))
                {
                    ArrowCheck();
                    if (!AttackOnce)
                        return;
                    _isAttacked = true;   
                }
            }else
            if (targetTag == "NPC")
            {
                _damaged = other.GetComponent<CommonStates>();
                if (TryToDamageNPC(other))
                {
                    ArrowCheck();
                    if (!AttackOnce)
                        return;
                    _isAttacked = true;   
                }
            }else if (targetTag == "Defence")
            {
                var defenceInfo = other.GetComponent<DefenceInfo>();
                if (defenceInfo != null)
                {
                    if (Damager.groupID != 0)
                    {
                        if (!IsArrow)
                            defenceInfo.DefenceDamage(Damager);
                        else
                            ArrowCheck();
                        if (!AttackOnce)
                            return;
                        _isAttacked = true;
                    }
                }
            }
            if (IsArrow && other.gameObject != Damager.gameObject && other.gameObject.layer != LayerMask.NameToLayer("Bullet"))
            {
                if (_damaged == null || !this.IsInGroup())
                    _arrowRotation = false;
                if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    _isGrounded = true;
            }
            if (!IsBullet || other.gameObject.layer != LayerMask.NameToLayer("Ground"))
                return;
            _isAttacked = true;
        }

        private bool TryToDamageNPC(Collider other)
        {
            var npcMove = _damaged.nMove;
            if ((npcMove != null ? npcMove.orderType == NPCMove.OrderType.Undying ? 1 : 0 : 0) != 0)
                return false;
            if (!IsInGroup() || IsInGroup() && (!_damaged.Employed(true) || _damaged.debuff.discontent == 4) || Damager.debuff.discontent == 4)
            {
                switch (DamageType)
                {
                    case DamageType.Normal:
                    {
                        var damageRate = DamageRate;
                        if (DistDampRate > 0.0)
                        {
                            var damageDistModifier = 1f;
                            var distance = Vector3.Distance(transform.position, other.transform.position);
                            if (distance >= (double) DampStart)
                            {
                                damageDistModifier = (float) (1.0 - (distance - (double) DampStart) * DistDampRate);
                                if (damageDistModifier <= 0.10000000149011612)
                                    damageDistModifier = 0.1f;
                            }
                            damageRate = DamageRate <= 10000.0 ? DamageRate * damageDistModifier : (float) (10000.0 + (DamageRate - 10000.0) * damageDistModifier);
                        }
                        _damaged.NPCDamage(Damager, damageRate);
                        return true;
                    }
                    case DamageType.Faint:
                    {
                        _damaged.StunDamage(Damager, DamageRate);
                        return true;
                    }
                    case DamageType.Slow:
                    {
                        if (_damaged.debuff.slowDamage == 0.0)
                        {
                            SoundFX?.PlaySound3D(transform.position, true, 20.0F);
                            _damaged.debuff.slowDamage = DamageRate;
                            _damaged.StartCoroutine(_damaged.DebuffSlowDamage());
                            return true;
                        }
                        _damaged.debuff.slowDamage = DamageRate;
                        return true;
                    }
                }
            }
            return false;
        }

        private bool TryToDamagePlayer(Collider other)
        {
            if (!IsInGroup() || IsInGroup() && (!Damager.Employed(true) || Damager.debuff.discontent == 4))
            {
                switch (DamageType)
                {
                    case DamageType.Normal:
                    {
                        _damaged.PlayerDamage(Damager, DamageRate);
                        return true;
                    }
                    case DamageType.Faint:
                    {
                        if (_damaged.faint > 0.0)
                        {
                            SoundFX?.PlaySound3D(transform.position, true, 20.0F);
                            _damaged.FaintChange(-(double) Damager.DamageCheck(Damager, DamageRate));
                        }
                        return true;
                    }
                    case DamageType.Slow:
                    {
                        if (_damaged.debuff.slowDamage == 0.0)
                        {
                            SoundFX?.PlaySound3D(transform.position, true, 20.0F);
                            _damaged.debuff.slowDamage = DamageRate;
                            _damaged.StartCoroutine(_damaged.DebuffSlowDamage());
                            return true;
                        }
                        _damaged.debuff.slowDamage = DamageRate;
                        return true;
                    }
                }
            }
            return false;
        }

        public IEnumerator ArrowRotation()
          {
            if (IsArrow)
            {
              if (!_rigidbody && !GetComponent<Rigidbody>())
              {
                _rigidbody = gameObject.AddComponent<Rigidbody>();
                _rigidbody.useGravity = true;
                _rigidbody.isKinematic = false;
                _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
              }
              if (_rigidbody != null)
              { 
                  _rigidbody.useGravity = true;
                  _rigidbody.isKinematic = false;
              }
              var prePos = transform.position;
              yield return null;
              var rotTime = 2f;
              _isGrounded = false;
              _arrowRotation = true;
              while (rotTime >= 0.0 && !_isAttacked && !_isGrounded)
              {
                rotTime -= Time.deltaTime;
                if (_arrowRotation)
                {
                  var forward = transform.position - prePos;
                  if (forward != Vector3.zero && _rigidbody !=  null)
                    _rigidbody.rotation = Quaternion.LookRotation(forward, Vector3.up);
                  prePos = transform.position;
                }
                yield return null;
              }
              if (_isGrounded)
                yield return null;
              _isAttacked = true;
              if (_rigidbody != null)
              {
                _rigidbody.useGravity = false;
                _rigidbody.isKinematic = true;
              }
              GetComponents<BoxCollider>()[1].enabled = false;
              prePos = new Vector3();
            }
          }
         
        public void ArrowCheck()
        {
            if (!IsArrow) return;
            var colliders = GetComponents<BoxCollider>();
            colliders[0].enabled = false;
            colliders[1].enabled = false;
            if (_rigidbody != null)
                Destroy(_rigidbody);
            if (Damager == null)
                return;
            var gObj = Damager.gameObject;
            var searchSpine = Damager.GetComponentInChildren<SearchSpine>();
            if (searchSpine != null)
                gObj = searchSpine.gameObject;
            gameObject.transform.SetParent(gObj.transform);
            gameObject.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0.0f);
        }
        
        private bool IsInGroup()
        {
            return Damager == _damaged || Damager.groupID == _damaged.groupID;
        }
    }
}