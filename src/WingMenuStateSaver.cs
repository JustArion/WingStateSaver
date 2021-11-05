namespace Dawn.WingStateSave
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using MelonLoader;
    using UnhollowerBaseLib.Attributes;
    using UnityEngine;
    using VRC.UI.Elements;

    public class WingMenuStateSaver : MonoBehaviour
    {
        public WingMenuStateSaver(IntPtr ptr) : base(ptr) {}
        
        private static PropertyInfo WingToggleButton;
        [HideFromIl2Cpp]
        private static void Toggle(Wing instance)
        {
            if (instance is null) throw new ArgumentNullException(nameof(instance));
            
            if (WingToggleButton is { }) ((UnityEngine.UI.Button)WingToggleButton.GetValue(instance)).Press();
            else
            {
                WingToggleButton = typeof(Wing).GetProperties(BindingFlags.Public | BindingFlags.Instance).First(p =>
                    p.PropertyType == typeof(UnityEngine.UI.Button) &&
                    ((UnityEngine.UI.Button)p.GetValue(instance)).name == "Button");
                ((UnityEngine.UI.Button)WingToggleButton.GetValue(instance)).Press();
            }

        }

        [HideFromIl2Cpp][MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Wing GetWing(Wing.WingPanel panel) =>
            Start.QuickMenu.prop_MenuStateController_0.field_Private_ArrayOf_Wing_0.First(w =>
                w.field_Public_WingPanel_0 == panel);

        [HideFromIl2Cpp]
        private IEnumerator EnableCoroutine()
        {
            yield return null;
            //yield return new WaitForSeconds(0.2f); // Wait for QM OnEnable Initialize
            if (Start.LeftEnabledSaveState)
                Toggle(GetWing(Wing.WingPanel.Left));

            if (Start.RightEnabledSaveState)
                Toggle(GetWing(Wing.WingPanel.Right));
            
            //Shhh, don't tell anyone
            MelonLogger.Msg(Start.LeftEnabledSaveState 
                ? Start.RightEnabledSaveState 
                    ? "Showing Left & Right Wings" 
                    : "Showing Left Wing" 
                : Start.RightEnabledSaveState 
                    ? "Showing Right Wing" 
                    : "Default Wing State");
            
            Destroy(this);
        }
        private void OnEnable()=> MelonCoroutines.Start(EnableCoroutine());
    }
}