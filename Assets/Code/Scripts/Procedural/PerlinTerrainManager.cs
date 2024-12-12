using System;
using UnityEngine;
using FrostfallSaga.Utils;

namespace FrostfallSaga.Procedural
{
    [Serializable]
    public class PerlinTerrainManager
    {
        private float _noiseScale;
        private int _seed;
        private Vector2 _noiseOffset;
        private PerlinNoiseGenerator _perlinNoiseGenerator;

        public PerlinTerrainManager(float noiseScale, int seed)
        {
            _seed = seed;
            InitSeed();
            _perlinNoiseGenerator = new PerlinNoiseGenerator(noiseScale);
        }
        private void InitSeed()
        {
            Randomizer.InitState(_seed);
        }

        public float GetNoiseValue(int x, int z)
        {
            return _perlinNoiseGenerator.ComputeNoiseValue(x, z);
        }
    }
}
