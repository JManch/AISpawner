using System;

using UnityEngine;
using MelonLoader;
using StressLevelZero.AI;

namespace AISpawner
{
    [RegisterTypeInIl2Cpp]
    internal class SpawnerAI : MonoBehaviour
    {
        public SpawnerAI(IntPtr intPtr) : base(intPtr) { }

        public int ID { get; private set; }
        public AIBrain aiBrain { get; private set; }
        private Spawner spawner;
        
        public void OnSpawn(Spawner parentSpawner)
        {
            aiBrain = GetComponent<AIBrain>();
            aiBrain.onDeathDelegate += new Action(OnDeath);
            ID = SpawnerManager.aiID;
            spawner = parentSpawner;

            aiBrain.behaviour.SetAgro(SpawnerManager.playerTrigger);
        }

        public void OnDisable()
        {
#if DEBUG
            MelonLogger.Msg("AI with ID " + ID + " has died :(");
#endif
            spawner?.OnAIDisable(this);
            spawner = null;
        }

        private void OnDeath()
        {
#if DEBUG
            MelonLogger.Msg("AI with ID " + ID + " has died :(");
#endif
            spawner?.OnAIDeath(this);
        }
    }
}
