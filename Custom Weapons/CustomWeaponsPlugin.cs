using BepInEx;
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
        GameObject Weapon;
        GameObject Parent;
        GameObject MagicFirePoint;
        bool firstRun = true;
        public int CurrentWeaponIndex = 0;
        public string CurrentWeaponName = "_default";

        public static List<CustomWeaponData> CustomWeapons { get; private set; } = new List<CustomWeaponData>();

        void Awake()
        {
            Logger.LogInfo("Cunny!");
            UnitySceneManager.activeSceneChanged += ChangedActiveScene;
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

        void ChangedActiveScene(Scene current, Scene next)
        {
#if DEBUG
            string currentName = current.name;
            if (string.IsNullOrEmpty(currentName)) currentName = "[Removed]";
            Logger.LogInfo($"Scenes: {currentName} => {next.name}");
#endif
            if (firstRun && next.name != "ShowLogo" && next.name != "Title")
            {
                firstRun = false;
                Logger.LogInfo("Initiating weapons");
                GetObjects();
                LoadWeapons();
            }
        }

        int Modulo(int a, int b)
        {
            int result = a % b;
            return (result < 0) ? result + b : result;
        }

        void LoadWeapons()
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
                    _weaponData.AssetBundle.Unload(false);
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

        public static void SetChildrenToLayer(GameObject gameObject, int layer)
        {
            foreach (var child in gameObject.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = layer;
            }
        }

        void SwitchWeapon(int NextWeaponIndex)
        {
            Logger.LogInfo($"Switching to weapon: {CustomWeapons[NextWeaponIndex].Descriptor.WeaponName}");
            SetWeaponActive(CurrentWeaponIndex, false);
            SetWeaponActive(NextWeaponIndex, true);
            CurrentWeaponIndex = NextWeaponIndex;
            CurrentWeaponName = CustomWeapons[CurrentWeaponIndex].FileName;
        }

        void SetWeaponActive(int index, bool active)
        {
            CustomWeapons[index].Weapon.SetActive(active);
            if (active && MagicFirePoint != null) MagicFirePoint.SetActive(!CustomWeapons[index].Descriptor.DisableEffect);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F7))
            {
                if (CustomWeapons.Count < 2)
                {
                    Logger.LogWarning("You don't have any custom weapons!");
                    return;
                }
                Logger.LogDebug($"Index: {CurrentWeaponIndex} => {Modulo(CurrentWeaponIndex - 1, CustomWeapons.Count)}");
                SwitchWeapon(Modulo(CurrentWeaponIndex - 1, CustomWeapons.Count));
            }
            if (Input.GetKeyDown(KeyCode.F8))
            {
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
