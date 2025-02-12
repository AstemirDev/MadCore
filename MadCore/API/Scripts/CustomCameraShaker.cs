using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MadCore.API.Scripts
{
    public class CustomCameraShaker : MonoBehaviour
    {
        private List<ShakeSource> _sources = new List<ShakeSource>();
        private float _prevIntensityX, _prevIntensityY, _prevIntensityZ;
        private float _intensityX, _intensityY, _intensityZ;

        private void Start()
        {
            StartCoroutine(Tick());
        }

        public IEnumerator Tick()
        {
            while (true)
            {
                if (Time.timeScale != 0.0F)
                {
                    _prevIntensityX = _intensityX;
                    _prevIntensityY = _intensityY;
                    _prevIntensityZ = _intensityZ;
                    if (_sources.Count == 0)
                    {
                        _intensityX = 0;
                        _intensityY = 0;
                        _intensityZ = 0;
                    }
                    else
                    {
                        var intensityX = 0.0F;
                        var intensityY = 0.0F;
                        var intensityZ = 0.0F;
                        for (var i = 0; i < _sources.Count; i++)
                        {
                            var source = _sources[i];
                            source.Tick();
                            if (source.IsStopped())
                            {
                                _sources.RemoveAt(i);
                            }
                            else
                            {
                                var intensity = source.GetIntensity();
                                var newIntensityX = intensityX + intensity.x;
                                var maxX = source.GetMaxBuildup().x;
                                intensityX = newIntensityX <= maxX ? newIntensityX : maxX;

                                var newIntensityY = intensityY + intensity.y;
                                var maxY = source.GetMaxBuildup().y;
                                intensityY = newIntensityY <= maxY ? newIntensityY : maxY;

                                var newIntensityZ = intensityZ + intensity.z;
                                var maxZ = source.GetMaxBuildup().z;
                                intensityZ = newIntensityZ <= maxZ ? newIntensityZ : maxZ;
                            }
                        }

                        _intensityX = intensityX != 0.0F ? randomizeIntensity(intensityX) : 0.0F;
                        _intensityY = intensityY != 0.0F ? randomizeIntensity(intensityY) : 0.0F;
                        _intensityZ = intensityZ != 0.0F ? randomizeIntensity(intensityZ) : 0.0F;
                    }
                }
                yield return new WaitForSeconds(0.05F);
            }
        }
        
        private void Update()
        {
            if (Time.timeScale == 0.0F) return;
            var screenShakeScale = 1.0F;
            if (screenShakeScale > 0.0F)
            {
                var deltaTime = Time.deltaTime;
                var x = Mathf.Lerp(_prevIntensityX, _intensityX, deltaTime);
                var y = Mathf.Lerp(_prevIntensityY, _intensityY, deltaTime);
                var z = Mathf.Lerp(_prevIntensityZ, _intensityZ, deltaTime);
                if (x != 0.0F || y != 0.0F || z != 0.0F)
                {
                    transform.localPosition += new Vector3(x * screenShakeScale, y * screenShakeScale,
                        z * screenShakeScale);
                }
            }
        }

        private static float randomizeIntensity(float intensity)
        {
            var randomFloat = Random.Range(0.0F, 1.0F);
            return (1.0F - randomFloat) * randomFloat * (Random.Range(0,2.0F)-0.5F) * intensity * 2.0F;
        }

        public void AddShakeSource(ShakeSource source)
        {
            if (_sources.Count <= 256)
            {
                _sources.Add(source);
            }
        }
    }

    public class PositionedShakeSource : SimpleShakeSource
    {
        private Vector3 _pos;
        
        public PositionedShakeSource(PlayerMove pm, Vector3 pos, int duration, float intensityX, float intensityY, float intensityZ, float maxBuildupX, float maxBuildupY, float maxBuildupZ, float decayX, float decayY, float decayZ) : base(pm, duration, intensityX, intensityY, intensityZ, maxBuildupX, maxBuildupY, maxBuildupZ, decayX, decayY, decayZ)
        {
            _pos = pos;
        }

        public PositionedShakeSource(PlayerMove pm, Vector3 pos, int duration, float intensity, float maxBuildup, float decay) : base(pm, duration, intensity, maxBuildup, decay)
        {
            _pos = pos;
        }

        public override Vector3 GetIntensity()
        {
            var position = _pm.transform.position;
            var dx = _pos.x - position.x;
            var dy = _pos.y - position.y;
            var dz = _pos.z - position.z;
            var distanceFactor = 0.001F * (dx * dx + dy * dy + dz * dz);
            return new Vector3(Math.Max(0, _intensityX-distanceFactor), Math.Max(0, _intensityY-distanceFactor), Math.Max(0, _intensityZ-distanceFactor));
        }
    }

    public class SimpleShakeSource : ShakeSource
    {

        protected PlayerMove _pm;
        protected int _timer;
        protected float _intensityX, _intensityY, _intensityZ;
        protected float _maxBuildupX, _maxBuildupY, _maxBuildupZ;
        protected float _decayX, _decayY, _decayZ;

        public SimpleShakeSource(PlayerMove pm, int duration, float intensityX, float intensityY, float intensityZ, float maxBuildupX, float maxBuildupY, float maxBuildupZ, float decayX, float decayY, float decayZ)
        {
            _pm = pm;
            _timer = duration;
            _intensityX = intensityX;
            _intensityY = intensityY;
            _intensityZ = intensityZ;
            _maxBuildupX = maxBuildupX;
            _maxBuildupY = maxBuildupY;
            _maxBuildupZ = maxBuildupZ;
            _decayX = decayX;
            _decayY = decayY;
            _decayZ = decayZ;
        }
        
        public SimpleShakeSource(PlayerMove pm, int duration, float intensity, float maxBuildup, float decay)
        {
            _pm = pm;
            _timer = duration;
            _intensityX = intensity;
            _intensityY = intensity;
            _intensityZ = intensity;
            _maxBuildupX = maxBuildup;
            _maxBuildupY = maxBuildup;
            _maxBuildupZ = maxBuildup;
            _decayX = decay;
            _decayY = decay;
            _decayZ = decay;
        }

        public void Tick()
        {
            _intensityX *= _decayX;
            _intensityY *= _decayY;
            _intensityZ *= _decayZ;
            _timer--;
        }

        public bool IsStopped()
        {
            return _timer <= 0;
        }

        public Vector3 GetMaxBuildup()
        {
            return new Vector3(_maxBuildupX, _maxBuildupY, _maxBuildupZ);
        }

        public virtual Vector3 GetIntensity()
        {
            return new Vector3(_intensityX, _intensityY, _intensityZ);
        }
    } 

    public interface ShakeSource
    {
        void Tick();

        bool IsStopped();

        Vector3 GetMaxBuildup();

        Vector3 GetIntensity();
    }
}