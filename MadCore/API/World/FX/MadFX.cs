using System;
using MadCore.API.Registry;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MadCore.API.World.FX
{
    public class MadFX : IDHolder
    {
        private readonly Func<GameObject> _fxPrefab;
        public ID FXId;
        
        public MadFX(Func<GameObject> fxPrefab)
        {
            _fxPrefab = fxPrefab;
            if (fxPrefab == null)
            {
                throw new NullReferenceException("Prefab can't be null");
            }
        }
        
        public GameObject Spawn(Vector3 pos)
        {
            var gameObject = Object.Instantiate(_fxPrefab.Invoke(), pos, Quaternion.identity);
            gameObject.transform.SetParent(FXRegistry.Instance.FXManager.poolGrp.transform);
            gameObject.transform.position = pos;
            return gameObject;
        }
        
        public GameObject SpawnFollowTarget(Vector3 pos, Vector3 fxScale = default (Vector3), GameObject followTarget = null)
        {
            var gameObject = Spawn(pos);
            gameObject.transform.localScale = !(fxScale != Vector3.zero) ? Vector3.one : fxScale;
            if (!followTarget) return gameObject;
            var followPosition = gameObject.GetComponent<FollowPosition>();
            if (followPosition) {
                followPosition.target = followTarget.transform;
            }
            return gameObject;
        }
        
        public GameObject Shoot(CommonStates attacker, Vector3 pos, Vector3 target, float force, float damageRate)
        {
            var gameObject = SpawnFollowTarget(pos);
            var attackCollider = gameObject.GetComponent<AttackCollider>();
            if (attackCollider)
            {
                attackCollider.attacker = attacker;
                attackCollider.damageRate = damageRate;   
            }
            var vector3 = Vector3.Normalize(new Vector3(target.x, target.y, target.z) - pos);
            gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, Mathf.Atan2(vector3.x, vector3.z) * 57.29578f, 0.0f));
            var rigidBody = gameObject.GetComponent<Rigidbody>();
            if (rigidBody)
            {
                rigidBody.AddForce(vector3 * force, ForceMode.Impulse);
            }
            return gameObject;
        }
        
        public void SetID(ID id)
        {
            FXId = id;
        }
        
        public ID GetID()
        {
            return FXId;
        }
    }
}