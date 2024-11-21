using System;
using System.Collections.Generic;
using FrostfallSaga.Core;
using UnityEngine;

namespace FrostfallSaga.Procedural
{
    [Serializable]
    public class VoronoiBiomeManager
    {
        [field: NonSerialized] public List<Vector2> _sites { get; private set; }
        private int _seed;
        private VoronoiGenerator _voronoiGenerator;

        public VoronoiBiomeManager(int width, int height, int biomeCount, int seed)
        {
            _seed = seed;
            InitSeed();
            GenerateVoronoiSites(width, height, biomeCount);
            _voronoiGenerator = new VoronoiGenerator(_sites, width, height);
        }

        private void InitSeed()
        {
            Randomizer.InitState(_seed);
        }

        private void GenerateVoronoiSites(int width, int height, int biomeCount)
        {
            _sites = new List<Vector2>();

            for (int i = 0; i < biomeCount; i++)
            {
                float x = Randomizer.GetRandomFloatBetween(0f, width);
                float y = Randomizer.GetRandomFloatBetween(0f, height);
                _sites.Add(new Vector2(x, y));
            }
        }

        public int GetClosestBiomeIndex(float x, float y)
        {
            return _voronoiGenerator.GetCloserSite(x, y);
        }
    }
}

