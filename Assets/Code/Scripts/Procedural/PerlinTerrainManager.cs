using System;
using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Procedural
{
    [Serializable]
    public class PerlinTerrainManager
    {
        private Vector2 _noiseOffset;
        private float _noiseScale;
        private PerlinNoiseGenerator _perlinNoiseGenerator;
        private int _seed;

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