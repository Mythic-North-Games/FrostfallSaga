using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FrostfallSaga.Procedural
{
    [Serializable]
    public class PerlinNoiseGenerator
    {
        [field: SerializeField, Range(0.1f, 0.99f)] public float noiseScale { get; private set; } = 0.2f;
        [field: SerializeField, Range(100000000, 999999999)] public int seed { get; private set; } = 112345678;
        private Vector2 noiseOffset;

        public PerlinNoiseGenerator(float noiseScale, int seed)
        {
            this.noiseScale = noiseScale;
            this.seed = seed;
            Initialize();
        }

        /// <summary>
        /// Initialize the noise generator with the seed.
        /// </summary>
        private void Initialize()
        {
            Random.InitState(seed);
            noiseOffset = new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f));
        }

        /// <summary>
        /// Obtain a Perlin noise value based on the x and z coordinates.
        /// </summary>
        /// <param name="x">Coordinate x</param>
        /// <param name="z">Coordinate z</param>
        /// <returns>Valeur de bruit de Perlin</returns>
        public float GetNoiseValue(int x, int z)
        {
            return Mathf.PerlinNoise((x + noiseOffset.x) * noiseScale, (z + noiseOffset.y) * noiseScale);
        }
    }
}
