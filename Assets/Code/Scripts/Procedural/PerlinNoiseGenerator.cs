using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FrostfallSaga.Procedural
{
    [Serializable]
    public class PerlinNoiseGenerator
    {
        private float _noiseScale;
        private Vector2 _noiseOffset;

        public PerlinNoiseGenerator(float noiseScale)
        {
            _noiseScale = noiseScale;
            Initialize();
        }

        private void Initialize()
        {
            _noiseOffset = new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f));
        }

        public float ComputeNoiseValue(int x, int z)
        {
            return Mathf.PerlinNoise((x + _noiseOffset.x) * _noiseScale, (z + _noiseOffset.y) * _noiseScale);
        }
    }
}
