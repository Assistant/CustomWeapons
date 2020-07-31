using BepInEx;
using System.IO;
using UnityEngine;

namespace Custom_Weapons
{
    public class CustomWeaponData
    {
        public string FileName { get; }
        public AssetBundle AssetBundle { get; }
        public WeaponDescriptor Descriptor { get; }
        public GameObject Weapon { get; set; }

        public CustomWeaponData(string filename)
        {
            FileName = filename;

            if (filename == "_default")
            {
                Descriptor = new WeaponDescriptor
                {
                    AuthorName = "Pupuya Games",
                    WeaponName = "Default",
                    DisableEffect = false,
                };
            }
            else
            {
                try
                {
                    AssetBundle = AssetBundle.LoadFromFile(filename);
                    Object.DontDestroyOnLoad(AssetBundle);
                    Weapon = AssetBundle.LoadAsset<GameObject>("_CustomWeapon");
                    Weapon.SetActive(false);
                    Object.DontDestroyOnLoad(Weapon);
                    Descriptor = Weapon.GetComponent<WeaponDescriptor>();
                    Object.DontDestroyOnLoad(Descriptor);
                }
                catch
                {
                    var logger = new BepInEx.Logging.ManualLogSource("CustomWeaponData");
                    BepInEx.Logging.Logger.Sources.Add(logger);
                    logger.LogError($"Failed loading AssetBundle: {filename}");

                    Weapon = null;
                    Descriptor = new WeaponDescriptor
                    {
                        AuthorName = filename,
                        WeaponName = "Invalid weapon",
                    };
                    FileName = "_default";
                }
            }
        }
    }
}
