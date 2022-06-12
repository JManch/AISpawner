using System;
using System.Collections.Generic;
using System.Linq;

using MelonLoader;
using UnityEngine;
using ModThatIsNotMod;
using StressLevelZero.AI;
using StressLevelZero.Pool;

namespace AISpawner
{
    [RegisterTypeInIl2Cpp]
    internal class Spawner : MonoBehaviour
    {
        public Spawner(IntPtr intPtr) : base(intPtr) { }

        public SpawnerSettings settings { get; set; }
        private Dictionary<int, SpawnerAI> aliveAI;
        private Queue<SpawnerAI> deadAI;
        
        public void StartSpawner(bool silent = false)
        {
            enabled = true;
            if (!silent)
                Notifications.SendNotification("Spawner started", 2f, Color.green);
        }

        public void StopSpawner(bool silent = false)
        {
            enabled = false;
            if (!silent)
                Notifications.SendNotification("Spawner stopped", 2f, Color.red);
        }

        private void Awake()
        {
            settings = new SpawnerSettings(UI.UI.menuSettings);
            aliveAI = new Dictionary<int, SpawnerAI>();
            deadAI = new Queue<SpawnerAI>();
            enabled = false;
        }

        private void OnEnable()
        {
#if DEBUG
            MelonLogger.Msg("Spawner enabled");
#endif
            WarmupPools();
            InvokeRepeating("PerformSpawn", settings.spawnFrequency, settings.spawnFrequency);
        }

        private void OnDisable()
        {
            CleanAll();
            CancelInvoke();
        }

        private void PerformSpawn()
        {
#if DEBUG
            MelonLogger.Msg("Performing spawn");
#endif
            // Check that we have not reached max alive enemies
            if (aliveAI.Count >= settings.maxAlive)
                return;

            int randomAI = UnityEngine.Random.Range(0, settings.ai.Count);
            SpawnAI(settings.ai[randomAI]);
        }

        private void SpawnAI(string name)
        {
            if (!randomSpawnPoint(out Vector3 position)) {
#if DEBUG
                MelonLogger.Error("Failed to find spawn location for AI");
#endif
                return;
            }
            // Need to catch error here, if pool is not loaded the AI is null
            GameObject ai = GlobalPool.Spawn(name, position, Quaternion.identity);
            SpawnerAI spawnerAI = ai.GetComponent<SpawnerAI>();
            if (spawnerAI == null)
            {
                spawnerAI = ai.AddComponent<SpawnerAI>();
            }

            spawnerAI.OnSpawn(this);
            aliveAI.Add(spawnerAI.ID, spawnerAI);
            SpawnerManager.IncrementAIID();
        }

        private bool randomSpawnPoint(out Vector3 randomPosition)
        {
            Vector3 originPosition = new Vector3(transform.position.x, transform.position.y + 0.25f, transform.position.z);

            // TODO: Need to ensure that minDistance is strictly < maxDistance
            Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * settings.maxSpawnDistance;
            while (Vector2.Distance(Vector2.zero, randomPoint) < settings.minSpawnDistance)
            {
                randomPoint = UnityEngine.Random.insideUnitCircle * settings.maxSpawnDistance;
            }

            randomPosition = new Vector3(originPosition.x + randomPoint.x, originPosition.y, originPosition.z + randomPoint.y);
            Vector3 direction = randomPosition - originPosition;

            RaycastHit hit;

            // Perform a horizontal raycast
            if (Physics.Raycast(originPosition, direction, out hit, Vector3.Magnitude(direction)))
            {
                if(hit.distance < settings.minSpawnDistance)
                {
                    return false;
                }
                randomPosition = hit.point + (Vector3.Normalize(originPosition - hit.point));
            }

            // Perform a vertical -y raycast
            if (Physics.Raycast(randomPosition, Vector3.down, out hit, settings.maxSpawnDistance))
            {
                randomPosition = hit.point;
            }
            else if (Physics.Raycast(randomPosition, Vector3.up, out hit, settings.maxSpawnDistance))
            {
                randomPosition = hit.point;
            }
            else
            {
                return false;
            }

            return true;
        }

        public void OnAIDeath(SpawnerAI ai)
        {
            aliveAI.Remove(ai.ID);
            deadAI.Enqueue(ai);
            if (deadAI.Count >= settings.maxDead)
            {
                CleanSingleDead();
            }
        }

        public void OnAIDisable(SpawnerAI ai)
        {
            aliveAI.Remove(ai.ID);
        }

        private void CleanSingleDead()
        {
            bool success = false;
            while (!success && deadAI.Count != 0)
            {
                SpawnerAI ai = deadAI.Dequeue();
                if (ai != null && ai.enabled && ai.aiBrain.isDead)
                {
                    ai.gameObject.SetActive(false);
                    success = true;
                }
            }
        }

        private void CleanAll()
        {
            // Clean dead
            foreach (SpawnerAI ai in deadAI.ToList())
            {
                if (ai != null)
                {
                    ai.gameObject.SetActive(false);
                }
            }
            deadAI.Clear();

            // Clean alive

            foreach (SpawnerAI ai in aliveAI.Values.ToList())
            {
                if (ai != null)
                {
                    ai.gameObject.SetActive(false);
                }
            }
            aliveAI.Clear();
        }

        private void WarmupPools()
        {
            foreach (string ai in settings.ai)
            {
                if (PoolManager.DynamicPools.ContainsKey(ai))
                {
                    Pool pool = PoolManager.DynamicPools[ai];
                    for (int i = pool.gameObject.transform.childCount; i < settings.maxAlive + settings.maxDead; i += pool.warmupAmount)
                    {
                        pool.Warmup();
#if DEBUG
                        MelonLogger.Msg("New pooled amount is " + pool.gameObject.transform.childCount);
#endif
                    }
                }
            }
        }
    }        
}
