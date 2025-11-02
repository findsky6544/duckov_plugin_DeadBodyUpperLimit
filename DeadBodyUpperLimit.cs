using System.IO;
using System.Reflection;
using System;
using HarmonyLib;
using UnityEngine;
using Duckov.Modding;

namespace DeadBodyUpperLimit
{
    [System.Serializable]
    public class DisplayItemValueConfig
    {

        //遗留物上限
        public int DeadBodyUpperLimit = 999;
    }
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        public static string MOD_NAME = "修改遗留物上限";

        public static DisplayItemValueConfig config = new DisplayItemValueConfig();

        private Harmony harmony;

        private string id = "com.findsky.DeadBodyUpperLimit";

        private static string persistentConfigPath => Path.Combine(Application.streamingAssetsPath, "DeadBodyUpperLimit_Data_modConfig.txt");
        private void OnEnable()
        {
            try
            {
                Console.WriteLine("onEnable");
                config.DeadBodyUpperLimit = LoadDataFromFile(1);
                harmony = new Harmony(id);
                harmony.PatchAll();

                ModManager.OnModActivated += OnModActivated;

                // 立即检查一次，防止 ModConfig 已经加载但事件错过了
                if (ModConfigAPI.IsAvailable())
                {
                    Debug.Log("DisplayItemValue: ModConfig already available!");
                    SetupModConfig();
                    LoadConfigFromModConfig();
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        private void OnDisable()
        {
            Console.WriteLine("OnDisable");

            harmony.UnpatchAll(id);

            ModManager.OnModActivated -= OnModActivated;
            ModConfigAPI.SafeRemoveOnOptionsChangedDelegate(OnModConfigOptionsChanged);
        }

        private void OnModActivated(ModInfo info, Duckov.Modding.ModBehaviour behaviour)
        {
            if (info.name == ModConfigAPI.ModConfigName)
            {
                Debug.Log("DisplayItemValue: ModConfig activated!");
                SetupModConfig();
                LoadConfigFromModConfig();
            }
        }
        private void SetupModConfig()
        {
            if (!ModConfigAPI.IsAvailable())
            {
                Debug.LogWarning("DisplayItemValue: ModConfig not available");
                return;
            }

            Debug.Log("准备添加ModConfig配置项");

            // 添加配置变更监听
            ModConfigAPI.SafeAddOnOptionsChangedDelegate(OnModConfigOptionsChanged);

            // 根据当前语言设置描述文字
            SystemLanguage[] chineseLanguages = {
                SystemLanguage.Chinese,
                SystemLanguage.ChineseSimplified,
                SystemLanguage.ChineseTraditional
            };


            // 添加配置项

            ModConfigAPI.SafeAddInputWithSlider(
                MOD_NAME,
                "DeadBodyUpperLimit",
                 "遗留物上限",
                typeof(int),
                config.DeadBodyUpperLimit,
                new Vector2(0, 999)
            );

            Debug.Log("DisplayItemValue: ModConfig setup completed");
        }

        private void OnModConfigOptionsChanged(string key)
        {
            if (!key.StartsWith(MOD_NAME + "_"))
                return;

            // 使用新的 LoadConfig 方法读取配置
            LoadConfigFromModConfig();

            // 保存到本地配置文件
            SaveConfig(config);

            Debug.Log($"DisplayItemValue: ModConfig updated - {key}");
        }

        private void SaveConfig(DisplayItemValueConfig config)
        {
            try
            {
                string json = JsonUtility.ToJson(config, true);
                File.WriteAllText(persistentConfigPath, json);
                Debug.Log("DisplayItemValue: Config saved");
            }
            catch (Exception e)
            {
                Debug.LogError($"DisplayItemValue: Failed to save config: {e}");
            }
        }
        private void LoadConfigFromModConfig()
        {
            // 使用新的 LoadConfig 方法读取所有配置
            config.DeadBodyUpperLimit = ModConfigAPI.SafeLoad<int>(MOD_NAME, "DeadBodyUpperLimit", config.DeadBodyUpperLimit);
        }
        public static int LoadDataFromFile(int defaultValue)
        {
            Console.WriteLine("LoadDataFromFile");
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (directoryName != null)
            {
                string path = Path.Combine(directoryName, "DeadBodyUpperLimit_Data.txt");
                if (File.Exists(path))
                {
                    using (StreamReader streamReader = new StreamReader(path))
                    {
                        while (!streamReader.EndOfStream)
                        {
                            string text = streamReader.ReadLine();
                            if (!string.IsNullOrEmpty(text))
                            {
                                int num;
                                if (int.TryParse(text, out num))
                                {
                                        return num;
                                }
                                return defaultValue;
                            }
                        }
                        return defaultValue;
                    }
                }
                File.Create(path).Close();
            }
            return defaultValue;
        }

    }
}
