using System.Collections.Generic;

namespace AISpawner
{
    public class SpawnerSettings
    {
        public readonly static SpawnerSettings defaultSettings = new SpawnerSettings(2f, 10, 3, 15f, 5f, new string[] {"Null Body"});

        public float spawnFrequency { get; set; }
        public int maxAlive { get; set; }
        public int maxDead { get; set; }
        public float maxSpawnDistance { get; set; }
        public float minSpawnDistance { get; set; }
        public List<string> ai { get; set; }
        
        public SpawnerSettings(float spawnFrequency, int maxAlive, int maxDead, float maxSpawnDistance, float minSpawnDistance, string[] ai)
        {
            this.spawnFrequency = spawnFrequency;
            this.maxAlive = maxAlive;
            this.maxDead = maxDead;
            this.maxSpawnDistance = maxSpawnDistance;
            this.minSpawnDistance = minSpawnDistance;
            this.ai = new List<string>(ai);
        }

        public SpawnerSettings(SpawnerSettings spawnerSettings)
        {
            spawnFrequency = spawnerSettings.spawnFrequency;
            maxAlive = spawnerSettings.maxAlive;
            maxDead = spawnerSettings.maxDead;
            maxSpawnDistance = spawnerSettings.maxSpawnDistance;
            minSpawnDistance = spawnerSettings.minSpawnDistance;
            ai = spawnerSettings.ai;
        }
    }
}
