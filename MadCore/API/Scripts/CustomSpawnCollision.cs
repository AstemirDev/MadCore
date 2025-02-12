using System;
using System.Collections;
using MadCore.API.Misc;
using MadCore.API.World.FX;
using MadCore.API.World.Sound;
using UnityEngine;

namespace MadCore.API.Scripts
{
    public class CustomSpawnCollision : MonoBehaviour
    {
        
        private LayerMask _collisionLayer = LayerMask.GetMask("Platform","Ground");
        public bool _triggered;
        
        public CommonStates Damager;
        public float DamageRate = 1.0F;
        public DamageType DamageType = DamageType.Normal;
        public MadFX SpawnFX;
        public MadSound SpawnSound;
        public Action<Vector3> OnSpawned;
        
        private void OnDisable()
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            _triggered = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((1 << other.gameObject.layer & _collisionLayer) == 0 || _triggered)
                return;
            _triggered = true;
            SpawnSound?.PlaySound3D(transform.position, true, 40);
            var pos = transform.position;
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position + Vector3.up * 3f, Vector3.down, out hitInfo, 10f, _collisionLayer))
                pos = hitInfo.point;
            var spawnedFX = SpawnFX.Spawn(pos);
            var attackCollider = spawnedFX.AddComponent<CustomAttackCollider>();
            attackCollider.Damager = Damager;
            attackCollider.DamageType = DamageType;
            attackCollider.DamageRate = DamageRate;
            if (hitInfo.collider != null)
            {
                var gameObject2 = spawnedFX.transform.GetChild(0)?.gameObject;
                if (gameObject2 != null && gameObject2.name != "Coll")
                    gameObject2.transform.rotation = Quaternion.FromToRotation(gameObject2.transform.forward, hitInfo.normal) * gameObject2.transform.rotation;
            }
            OnSpawned.Invoke(pos);
            gameObject.SetActive(value: false);
        }
        
    }
}