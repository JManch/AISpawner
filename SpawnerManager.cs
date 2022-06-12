using System.Collections.Generic;

using UnityEngine;
using ModThatIsNotMod;
using StressLevelZero.AI;
using MelonLoader;

namespace AISpawner
{
    public static class SpawnerManager
    {
        // Stores global AI spawned count for unique IDs
        internal static int aiID { get; private set; } = 0;
        internal static int spawnerID { get; private set; } = 0;

        private static Dictionary<int, Spawner> spawners = new Dictionary<int, Spawner>();
        internal static TriggerRefProxy playerTrigger { get; private set; }

        // Creates a spawner and returns it's ID, spawner uses menuSettings by default
        public static int CreateSpawner(GameObject gameObject)
        {
            if (gameObject.GetComponent<Spawner>() == null)
            {
                Spawner spawner = gameObject.AddComponent<Spawner>();
                spawners.Add(spawnerID, spawner);
                spawnerID++;
#if DEBUG
                MelonLogger.Msg("Created spawner on object {0}, spawner count is {1}", gameObject.name, spawners.Count);
#endif
                return spawnerID;
            }
            else
            {
#if DEBUG
                MelonLogger.Msg("Object {0} already has a spawner component ", gameObject.name);
#endif
            }
            return -1;
        }

        public static void UpdateSpawnerSettings(int spawnerID, SpawnerSettings newSettings)
        {
            if (spawners.ContainsKey(spawnerID))
            {
                Spawner spawner = spawners[spawnerID];
                if (spawner.enabled)
                {
                    spawners[spawnerID].StopSpawner(true);
                    spawners[spawnerID].settings = newSettings;
                    spawners[spawnerID].StartSpawner(true);
                }
                else
                {
                    spawners[spawnerID].settings = newSettings;
                }
            }
            else
            {
                MelonLogger.Error("Spawner does not exist");
            }
        }

        public static void StartSpawner(int spawnerID)
        {
            // Create player spawner if it does not exist
            if (spawnerID == 0 && !spawners.ContainsKey(0))
            {
                CreateSpawner(Player.GetPlayerHead());
            }

            if (spawners.ContainsKey(spawnerID))
            {
                spawners[spawnerID].StartSpawner();
            }
            else
            {
                MelonLogger.Error("Spawner does not exist");
            }
        }

        public static void StopSpawner(int spawnerID)
        {
            if (spawners.ContainsKey(spawnerID))
            {
                spawners[spawnerID].StopSpawner();
            }
            else
            {
                MelonLogger.Error("Spawner does not exist");
            }
        }

        internal static void IncrementAIID()
        {
            aiID++;
        }

        internal static void OnSceneWasInitialized()
        {
            playerTrigger = Player.GetPlayerHead().GetComponent<TriggerRefProxy>();
        }

        internal static void OnSceneWasUnloaded()
        {
            // Delete all spawners
            foreach(KeyValuePair<int, Spawner> pair in spawners)
            {
                if (pair.Value != null)
                {
                    // TODO: Check if this is necessary, they might all be destroyed anyway
                    Object.Destroy(pair.Value);
                }
            }
            spawners.Clear();
            spawnerID = 0;
        }
    }
}
