using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Custom_Weapons
{
    [BepInPlugin("moe.nobeta.plugins.customweapons", "Custom Weapons", "1.0.0.0")]
    public class CustomWeaponsPlugin : BaseUnityPlugin
    {
        static GameObject Weapon;
        static GameObject Parent;
        static GameObject MagicFirePoint;
        static bool inGame = false;
        public static int CurrentWeaponIndex = 0;
        public static ConfigEntry<string> CurrentWeaponName;

        public static ConfigEntry<KeyboardShortcut> PreviousWeapon;
        public static ConfigEntry<KeyboardShortcut> NextWeapon;


        public static List<CustomWeaponData> CustomWeapons { get; private set; } = new List<CustomWeaponData>();

        void Awake()
        {
            Logger.LogInfo("Cunny!");
            UnitySceneManager.sceneLoaded += OnSceneLoaded;
            CurrentWeaponName = Config.Bind("Data", "CurrentWeaponName", "_default", "Saves the weapon you last used. Do not edit.");
            PreviousWeapon = Config.Bind("Keybinds", "PreviousWeapon", new KeyboardShortcut(KeyCode.F7), "Key to switch to the previous weapon");
            NextWeapon = Config.Bind("Keybinds", "NextWeapon", new KeyboardShortcut(KeyCode.F8), "Key to switch to the next weapon");
            foreach (KeyCode key in KeyboardShortcut.AllKeyCodes)
            {
                Logger.LogMessage($"Key: {key}");
            }
        }

        void GetObjects()
        {
            GameObject[] gameObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.name == "WizardGirl_Weapon")
                {
                    Weapon = gameObject;
                    Logger.LogInfo("Found Weapon.");
                    Parent = gameObject.transform.parent.gameObject;
                    Logger.LogInfo($"Parent: {Parent.name}");
                }
                if (gameObject.name == "MagicFirePoint")
                {
                    MagicFirePoint = gameObject;
                    Logger.LogInfo("Found MagicFirePoint.");
                }
            }
            if (!Weapon) Logger.LogWarning("Failed to find WizardGirl_Weapon GameObject");
            if (!MagicFirePoint) Logger.LogWarning("Failed to find MagicFirePoint GameObject");
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
#if DEBUG
            Logger.LogInfo($"Scene: {scene.name}");
#endif
            if (scene.name != "ShowLogo" && scene.name != "Title")
            {
                inGame = true;
                Logger.LogInfo("Initiating weapons");
                GetObjects();
                LoadWeapons();
            }
            else
            {
                inGame = false;
            }
        }

        int Modulo(int a, int b)
        {
            int result = a % b;
            return (result < 0) ? result + b : result;
        }

        void LoadWeapons()
        {
            if (CustomWeapons.Count == 0)
            {
                string CustomWeaponsDir = $"{Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'))}/Custom/Weapons/";
                if (!Directory.Exists(CustomWeaponsDir)) Directory.CreateDirectory(CustomWeaponsDir);
                Logger.LogInfo($"Weapons directory: {CustomWeaponsDir}");
                CustomWeaponData _default = new CustomWeaponData("_default");
                _default.Weapon = Weapon;
                CustomWeapons.Add(_default);

                string[] _weaponFiles = Directory.GetFiles(CustomWeaponsDir, "*.weapon", SearchOption.AllDirectories);
                foreach (string _weaponFile in _weaponFiles)
                {
                    Logger.LogInfo($"Weapon path: {_weaponFile}");
                    CustomWeaponData _weaponData = new CustomWeaponData(_weaponFile);
                    if (_weaponData != null && _weaponData.Weapon != null)
                    {
                        _weaponData.Weapon.SetActive(false);
                        _weaponData.Weapon = Instantiate(_weaponData.Weapon);
                        DontDestroyOnLoad(_weaponData.Weapon);
                        _weaponData.Weapon.transform.parent = Parent.transform;
                        _weaponData.Weapon.transform.localPosition = CustomWeapons[0].Weapon.transform.localPosition;
                        _weaponData.Weapon.transform.localRotation = CustomWeapons[0].Weapon.transform.localRotation;
                        SetChildrenToLayer(_weaponData.Weapon, 9);
                        CustomWeapons.Add(_weaponData);
                    }
                    else
                    {
                        Logger.LogWarning($"Can't load {_weaponFile}");
                    }
                }
            }
            else
            {
                foreach (CustomWeaponData _weaponData in CustomWeapons)
                {
                    if (_weaponData.FileName != "_default")
                    {
                        _weaponData.Weapon = Instantiate(_weaponData.AssetBundle.LoadAsset<GameObject>("_CustomWeapon"));
                        _weaponData.Weapon.SetActive(false);
                        DontDestroyOnLoad(_weaponData.Weapon);
                        _weaponData.Weapon.transform.parent = Parent.transform;
                        _weaponData.Weapon.transform.localPosition = CustomWeapons[0].Weapon.transform.localPosition;
                        _weaponData.Weapon.transform.localRotation = CustomWeapons[0].Weapon.transform.localRotation;
                        SetChildrenToLayer(_weaponData.Weapon, 9);
                    }
                    else
                    {
                        _weaponData.Weapon = Weapon;
                    }
                }
            }
            if (CurrentWeaponName.Value != "_default" && File.Exists(CurrentWeaponName.Value)) 
            {
                CurrentWeaponIndex = 0;
                for (int i = 0; i < CustomWeapons.Count; i++)
                {
                    if (CustomWeapons[i].FileName == CurrentWeaponName.Value)
                    {
                        SwitchWeapon(i);
                    }
                }
            }
        }

        public static void SetChildrenToLayer(GameObject gameObject, int layer)
        {
            foreach (var child in gameObject.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = layer;
            }
        }

        void SwitchWeapon(int NextWeaponIndex)
        {
            Logger.LogDebug($"Switching to weapon: {CustomWeapons[NextWeaponIndex].Descriptor.WeaponName}");
            SetWeaponActive(CurrentWeaponIndex, false);
            SetWeaponActive(NextWeaponIndex, true);
            CurrentWeaponIndex = NextWeaponIndex;
            CurrentWeaponName.Value = CustomWeapons[CurrentWeaponIndex].FileName;
        }

        void SetWeaponActive(int index, bool active)
        {
            CustomWeapons[index].Weapon.SetActive(active);
            if (active && MagicFirePoint != null) MagicFirePoint.SetActive(!CustomWeapons[index].Descriptor.DisableEffect);
        }

        void Update()
        {
            if (PreviousWeapon.Value.IsDown())
            {
                if (!inGame)
                {
                    Logger.LogWarning("You need to load a game first!");
                    return;
                }
                if (CustomWeapons.Count < 2)
                {
                    Logger.LogWarning("You don't have any custom weapons!");
                    return;
                }
                Logger.LogDebug($"Index: {CurrentWeaponIndex} => {Modulo(CurrentWeaponIndex - 1, CustomWeapons.Count)}");
                SwitchWeapon(Modulo(CurrentWeaponIndex - 1, CustomWeapons.Count));
            }
            if (NextWeapon.Value.IsDown())
            {
                if (!inGame)
                {
                    Logger.LogWarning("You need to load a game first!");
                    return;
                }
                if (CustomWeapons.Count < 2)
                {
                    Logger.LogWarning("You don't have any custom weapons!");
                    return;
                }
                Logger.LogDebug($"Index: {CurrentWeaponIndex} => {Modulo(CurrentWeaponIndex + 1, CustomWeapons.Count)}");
                SwitchWeapon(Modulo(CurrentWeaponIndex + 1, CustomWeapons.Count));
            }
#if DEBUG
            if (Input.GetKeyDown(KeyCode.F10))
            {
                MeshRenderer mesh = Weapon.GetComponent<MeshRenderer>();
                mesh.enabled = !mesh.enabled;
            }
            if (Input.GetKeyDown(KeyCode.F9))
            {
                Logger.LogInfo($"Scene: {UnitySceneManager.GetActiveScene().name}");
            }
#endif
        }
    }
}
