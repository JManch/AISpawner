using MelonLoader;
using ModThatIsNotMod.BoneMenu;
using ModThatIsNotMod;
using StressLevelZero.Data;
using UnityEngine;

namespace AISpawner.UI
{
    internal static class UI
    {
        private static MenuCategory menuCategory;
        private static MenuCategory aiCategory;

        public static SpawnerSettings menuSettings { get; private set; } = new SpawnerSettings(SpawnerSettings.defaultSettings);

        private static string[] vanillaAI = { 
            "Omni Wrecker",
            "Ford VR Junkie",
            "Crablet Plus",
            "Null Body",
            "Null Rat",
            "Omni Projector",
            "Ford EarlyExit",
            "Ford Early Exit Headset",
            "Ford Head",
            "Ford",
            "Null Body Corrupted",
            "Crablet",
            "Crablet Plus"
        };

        private static void CreateUI()
        {
#if DEBUG
            MelonLogger.Msg("Creating UI");
#endif

            menuCategory = MenuManager.CreateCategory("AI Spawner", Color.grey);
            
            menuCategory.CreateFunctionElement("Start Spawner", Color.green, () => { SpawnerManager.StartSpawner(0); });
            menuCategory.CreateFunctionElement("Stop Spawner", Color.red, () => { SpawnerManager.StopSpawner(0); });
            
            aiCategory = menuCategory.CreateSubCategory("Select AI", Color.gray);

            foreach (string ai in vanillaAI)
            {
                aiCategory.CreateBoolElement(ai, Color.grey, ai == "Null Body" ? true : false, (bool value) => { ToggleAI(ai, value); });
            }

            menuCategory.CreateFloatElement("Spawn Frequency", Color.gray, menuSettings.spawnFrequency , (value) => { menuSettings.spawnFrequency = value; }, invokeOnValueChanged: true, increment: 0.2f);
            menuCategory.CreateIntElement("Max Alive", Color.gray, menuSettings.maxAlive, (value) => { menuSettings.maxAlive = value; }, invokeOnValueChanged: true);
            menuCategory.CreateIntElement("Max Dead", Color.gray, menuSettings.maxDead, (value) => { menuSettings.maxDead = value; }, invokeOnValueChanged: true);
            menuCategory.CreateFloatElement("Max Spawn Distance", Color.gray, menuSettings.maxSpawnDistance, (value) => { menuSettings.maxSpawnDistance = value; }, increment: 0.5f, invokeOnValueChanged: true);
            menuCategory.CreateFloatElement("Min Spawn Distance", Color.gray, menuSettings.minSpawnDistance, (value) => { menuSettings.minSpawnDistance = value; }, invokeOnValueChanged: true, increment: 0.5f);

            //menuCategory.CreateFunctionElement("Update Spawner", Color.green, () => { SpawnerManager.UpdateSpawnerSettings(0, menuSettings); });
        }

        private static void OnSpawnableCreated(SpawnableObject spawnableObject)
        {
#if DEBUG
            MelonLogger.Msg("On spawnable created called {0}", spawnableObject.title);
#endif
            if (spawnableObject.category == CategoryFilters.NPCS)
            {
                //customAI.Add(spawnableObject.title);
                aiCategory.CreateBoolElement(spawnableObject.title, Color.grey, false, (bool value) => { ToggleAI(spawnableObject.title, value); });
            }
        }

        public static void OnApplicationStart()
        {
            //SpawnMenu.OnItemAddedToMenu += OnSpawnableCreated;
            CustomItems.OnSpawnableCreated += OnSpawnableCreated;
            CreateUI();
        }

        private static void ToggleAI(string name, bool value)
        {
            if (value)
                menuSettings.ai.Add(name);
            else
                menuSettings.ai.Remove(name);
        }
    }
}
