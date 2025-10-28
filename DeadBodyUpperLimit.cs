using Duckov.Rules;
using System.IO;
using System.Reflection;
using System;
using Duckov.Scenes;
using UnityEngine.SceneManagement;
using HarmonyLib;
using Duckov;
using System.Collections.Generic;
using Duckov.UI;
using Duckov.Quests;
using UnityEngine;

namespace DeadBodyUpperLimit
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        public static int SaveDeadbodyCount = 1;

        private Harmony harmony;

        private string id = "com.findsky.DeadBodyUpperLimit";
        private void OnEnable()
        {
            try
            {
                Console.WriteLine("onEnable");
                SaveDeadbodyCount = LoadDataFromFile(1);
                harmony = new Harmony(id);
                harmony.PatchAll();
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        private void OnDisable()
        {
            Console.WriteLine("OnDisable");

            harmony.UnpatchAll(id);
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
