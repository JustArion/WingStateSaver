using System;
using Dawn.WingStateSave;
using MelonLoader;

[assembly: MelonColor(ConsoleColor.DarkCyan)]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonInfo(typeof(Start), "WingMenuStateSaver", "1.0.0", "arion#1223")]

namespace Dawn.WingStateSave
{
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnhollowerRuntimeLib;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using VRC.UI.Elements;

    internal sealed class Start : MelonMod
    {
        
        public override void OnApplicationStart()
        {
            var cat = MelonPreferences.CreateCategory("WingStateSaver", "Wing Menu State Saver");
            LeftEnabledSaveState = cat.CreateEntry("Left Wing", true, "Left Wing", "Changes require a restart.").Value;
            RightEnabledSaveState = cat.CreateEntry("Right Wing", true, "Right Wing", "Changes require a restart.").Value;
            ClassInjector.RegisterTypeInIl2Cpp<WingMenuStateSaver>(true);
        }

        private static Scene DontDestroyOnLoad
        {
            get
            {
                try
                {
                    var Cow = new GameObject("Moo");
                    UnityEngine.Object.DontDestroyOnLoad(Cow);
                    var SacrificedCow = Cow.scene;
                    UnityEngine.Object.DestroyImmediate(Cow);
                    return SacrificedCow;
                }
                catch
                {
                    return default;
                }
            }
        }
        private static bool? _Initialized = false;
        private static bool AssumedQMCreated => DontDestroyOnLoad != default && DontDestroyOnLoad.rootCount > 9;
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (_Initialized is not false || !AssumedQMCreated) return;
            _Initialized = null;
            LateUiManagerInit();
        }
        
        internal static bool LeftEnabledSaveState;
        internal static bool RightEnabledSaveState;
        private static QuickMenu _QuickMenu;
        internal static QuickMenu QuickMenu => _QuickMenu ??= UnityEngine.Resources.FindObjectsOfTypeAll<QuickMenu>().FirstOrDefault(p => p.gameObject.scene.name == "DontDestroyOnLoad");
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void LateUiManagerInit()
        {
            if (MelonDebug.IsEnabled())
                MelonDebug.Msg("LateUiManagerInit");
            
            if (QuickMenu is null)
                MelonLogger.Error("Unable to find <VRC.UI.Elements.QuickMenu>");
            else
                QuickMenu.gameObject.AddComponent<WingMenuStateSaver>();
        }
    }
}