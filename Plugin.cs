using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using SpinCore.Translation;
using SpinCore.UI;
using UnityEngine;

namespace HitBarMove;

//make sure to reference things i need to reference up here <^ right click on dependencies then reference, then add from...
//DONT FORGET THIS ITS KINDA IMPORTANT AND I DONT WANT TO ASK AGAIN

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    
    //ConfigEntry<var type> VarName;
    
    private static ConfigEntry<int> barY;
    private static ConfigEntry<bool> enableBacking;
    
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} exists!");
        
        //Sets saved var ["name of section (default "general")", "internal key", Default Value, "description of the value"]
        barY = Config.Bind("general", "barY", 300, "Sets the bar Y position"); 
        enableBacking = Config.Bind("general", "enableBacking", true, "Toggles the black border around the timing bar");
        
        //patches typeof(function name) every frame
        Harmony.CreateAndPatchAll(typeof(patches));
        
        //["translationkey", "Visual name in game"]
        TranslationHelper.AddTranslation("HBM_BarHeaderName", "Hit Bar Move");
        TranslationHelper.AddTranslation("HBM_BarYNumName", "Height");
        TranslationHelper.AddTranslation("HBM_ToggleBGName", "Enable Background");
        
        UIHelper.RegisterGroupInQuickModSettings(parent =>
        {
            var bargroup = UIHelper.CreateGroup(parent, "Bargroup");
            
            UIHelper.CreateSectionHeader(
                bargroup,
                "BarHeader",
                "HBM_BarHeaderName",
                false //required
            );
            
            UIHelper.CreateSmallMultiChoiceButton(
                bargroup,
                "BarYNum",
                "HBM_BarYNumName",
                barY.Value, // default Value
                v => barY.Value = v, // sets barY to v
                () => new IntRange(0, 450), //sets the int range for v
                v => v.ToString() //text shown on the thing
            );

            UIHelper.CreateSmallToggle(
                bargroup,
                "ToggleBG",
                "HBM_ToggleBGName",
                enableBacking.Value,
                v => enableBacking.Value = v
            );
        });
    }

    public class patches // does this every frame
    {
        [HarmonyPatch(typeof(HudTimingAccuracyBar), "LateUpdate")] //patch the selected method (typeof(patch)) (nameof(specific)) or in this case lateupdate
        [HarmonyPrefix] //does it before (pre-fix)
        static void meow(HudTimingAccuracyBar __instance)
        {
            
            var parent = __instance._rectTransform.parent;
            parent.localPosition = new Vector3(__instance._rectTransform.localPosition.x, barY.Value, -2);
            
            parent.Find("Accuracy Bar Backing").gameObject.SetActive(enableBacking.Value);
            
        }
    }
}
