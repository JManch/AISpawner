using MelonLoader;
using ModThatIsNotMod;
using HarmonyLib;
using UnityEngine;
using System;
using StressLevelZero;
using System.Reflection;

namespace AISpawner
{
    public class Main : MelonMod
    {
        public override void OnApplicationStart()
        {
            UI.UI.OnApplicationStart();

            //Hooking.CreateHook(typeof(GlobalPool).GetMethod("Spawn", AccessTools.all, null, CallingConventions.Any, new Type[] { typeof(string), typeof(Vector3), typeof(Quaternion) }, null), typeof(Main).GetMethod("OnSpawnHook", AccessTools.all)); ; ;;
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
#if DEBUG
            //MelonLogger.Msg("Scene initialized called with build index {0} and sceneName {1}", buildIndex, sceneName);
#endif
            SpawnerManager.OnSceneWasInitialized();
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
#if DEBUG
            //MelonLogger.Msg("Scene unloaded called with build index {0} and sceneName {1}", buildIndex, sceneName);
#endif
            SpawnerManager.OnSceneWasUnloaded();
        }

        public static void OnSpawnHook(string id, Vector3 position, Quaternion rotation, GameObject __result)
        {
            MelonLogger.Msg("On spawn was called");
            MelonLogger.Msg("OnSpawn was called and spawned object" + __result?.name);
        }
    }
}
