using System;
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
    
    // private static ConfigEntry<bool> enableColourBar;
    
    private static ConfigEntry<bool> enableAwful;
    private static ConfigEntry<bool> enableOkay;
    private static ConfigEntry<bool> enableGreat;
    private static ConfigEntry<bool> enablePerfect;
    private static ConfigEntry<bool> enablePerfectPlus;
    
    private static ConfigEntry<int> barXScale;
    private static ConfigEntry<int> barYScale;
    
    private static float barYScaleFloat;
    private static float barXScaleFloat;
    
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} exists!");
        
        //Sets saved var ["name of section (default "general")", "internal key", Default Value, "description of the value"]
        barY              = Config.Bind("general", "barY", 300, "Sets the bar Y position"); 
        
        barXScale         = Config.Bind("general", "barXScale", 100, "Sets the bar X scale");
        barYScale         = Config.Bind("general", "barYScale", 100, "Sets the bar Y scale");

        
        enableBacking     = Config.Bind("general", "enableBacking", true, "Toggles the black border around the timing bar");
        // enableColourBar   = Config.Bind("general", "enableColourBar", true, "Toggles the colours on the timing bar");
        
        enableAwful       = Config.Bind("general", "enableAwful", true, "Toggles Awful colour rectangles on the timing bar");
        enableOkay        = Config.Bind("general", "enableOkay", true, "Toggles Okay colour rectangles on the timing bar");
        enableGreat       = Config.Bind("general", "enableGreat", true, "Toggles Great colour rectangles on the timing bar");
        enablePerfect     = Config.Bind("general", "enablePerfect", true, "Toggles Perfect colour rectangles on the timing bar");
        enablePerfectPlus = Config.Bind("general", "enablePerfectPlus", true, "Toggles PerfectPlus colour rectangles on the timing bar");
        
        
        //patches typeof(function name) every frame
        Harmony.CreateAndPatchAll(typeof(patches));
        
        //["translationkey", "Visual name in game"]
        
        //Quick Mod Settings Page
        TranslationHelper.AddTranslation("HBM_BarHeaderName", "Hit Bar Move");
        TranslationHelper.AddTranslation("HBM_BarYNumName", "Height");
        
        TranslationHelper.AddTranslation("HBM_BarXScaleName", "X Scale");
        TranslationHelper.AddTranslation("HBM_BarYScaleName", "Y Scale");
        
        TranslationHelper.AddTranslation("HBM_ToggleBGName", "Enable Bar Border");
        // TranslationHelper.AddTranslation("HBM_ToggleColourName", "Enable Bar Colours");
        
        //Mod Settings Page
        TranslationHelper.AddTranslation("HBM_ModPageName", "HitBarMove");
        TranslationHelper.AddTranslation("HBM_ModPageSectionName", "HitBarMove");
        
        
        TranslationHelper.AddTranslation("HBM_GeneralSettingsName", "General Settings");
        TranslationHelper.AddTranslation("HBM_ColourSectionName", "Timing Sections");
        TranslationHelper.AddTranslation("HBM_ToggleAwfulName", "Enable Early/Late Colours");
        TranslationHelper.AddTranslation("HBM_ToggleOkayName", "Enable Okay Colours");
        TranslationHelper.AddTranslation("HBM_ToggleGreatName", "Enable Great Colours");
        TranslationHelper.AddTranslation("HBM_TogglePerfectName", "Enable Perfect Colours");
        TranslationHelper.AddTranslation("HBM_TogglePerfectPlusName", "Enable Perfect+ Colours");
        
        
        
        
        
        
        
        UIHelper.RegisterGroupInQuickModSettings(parent =>
        {
            var bargroup = UIHelper.CreateGroup(parent, "BarGroup");
            
            UIHelper.CreateSectionHeader(
                bargroup,
                "BarHeader",
                "HBM_BarHeaderName",
                false //required
            );
            
            UIHelper.CreateLargeMultiChoiceButton(
                bargroup,
                "BarYNum",
                "HBM_BarYNumName",
                barY.Value, // default Value
                v => barY.Value = v, // sets barY to v
                () => new IntRange(-200, 450), //sets the int range for v
                v => v.ToString() //text shown on the thing
            );
            
            UIHelper.CreateSmallMultiChoiceButton(
                bargroup,
                "BarXScale",
                "HBM_BarXScaleName",
                barXScale.Value,
                v => barXScale.Value = v,
                () => new IntRange(0, 300),
                v => v.ToString()
            );
            
            UIHelper.CreateSmallMultiChoiceButton(
                bargroup,
                "BarYScale",
                "HBM_BarYScaleName",
                barYScale.Value,
                v => barYScale.Value = v,
                () => new IntRange(0, 300),
                v => v.ToString()
            );
        });
        
        var barModPage = UIHelper.CreateCustomPage("ModSettings");
        barModPage.OnPageLoad += modPageTransform =>
        {
            var modpage = UIHelper.CreateGroup(modPageTransform, "BarGroup");
            UIHelper.CreateSectionHeader(
                modpage,
                "ModPageSection",
                "HBM_ModPageSectionName",
                false
            );
            
            UIHelper.CreateLargeMultiChoiceButton(
                modpage,
                "BarYNum",
                "HBM_BarYNumName",
                barY.Value, // default Value
                v => barY.Value = v, // sets barY to v
                () => new IntRange(-200, 450), //sets the int range for v
                v => v.ToString() //text shown on the thing
            );
            
            UIHelper.CreateSmallMultiChoiceButton(
                modpage,
                "BarXScale",
                "HBM_BarXScaleName",
                barXScale.Value,
                v => barXScale.Value = v,
                () => new IntRange(0, 300),
                v => v.ToString()
            );
            
            UIHelper.CreateSmallMultiChoiceButton(
                modpage,
                "BarYScale",
                "HBM_BarYScaleName",
                barYScale.Value,
                v => barYScale.Value = v,
                () => new IntRange(0, 300),
                v => v.ToString()
            );
            
            UIHelper.CreateSmallToggle(
                modpage,
                "ToggleBG",
                "HBM_ToggleBGName",
                enableBacking.Value,
                v => enableBacking.Value = v
            );
            
            // UIHelper.CreateSmallToggle(
            //     bargroup,
            //     "ToggleColours",
            //     "HBM_ToggleColourName",
            //     enableColourBar.Value,
            //     v => enableColourBar.Value = v
            // );
            
            UIHelper.CreateSectionHeader(
                modpage,
                "ModPageSection",
                "HBM_ColourSectionName",
                true
            );
                
            UIHelper.CreateSmallToggle(
                modpage,
                "ToggleAwful",
                "HBM_ToggleAwfulName",
                enableAwful.Value,
                v => enableAwful.Value = v
            );

            UIHelper.CreateSmallToggle(
                modpage,
                "ToggleOkay",
                "HBM_ToggleOkayName",
                enableOkay.Value,
                v => enableOkay.Value = v
            );

            UIHelper.CreateSmallToggle(
                modpage,
                "ToggleGreat",
                "HBM_ToggleGreatName",
                enableGreat.Value,
                v => enableGreat.Value = v
            );

            UIHelper.CreateSmallToggle(
                modpage,
                "TogglePerfect",
                "HBM_TogglePerfectName",
                enablePerfect.Value,
                v => enablePerfect.Value = v
            );

            UIHelper.CreateSmallToggle(
                modpage,
                "TogglePerfectPlus",
                "HBM_TogglePerfectPlusName",
                enablePerfectPlus.Value,
                v => enablePerfectPlus.Value = v
            );
        };
        
        UIHelper.RegisterMenuInModSettingsRoot("HBM_ModPageName", barModPage);
    }

    public class patches // does this every frame
    {
        [HarmonyPatch(typeof(HudTimingAccuracyBar), "LateUpdate")] //patch the selected method (typeof(patch)) (nameof(specific)) or in this case lateupdate
        [HarmonyPrefix] //does it before (pre-fix)
        static void meow(HudTimingAccuracyBar __instance)
        {
            barYScaleFloat    = barYScale.Value;
            barXScaleFloat    = barXScale.Value;
            
            var parent = __instance._rectTransform.parent;
            parent.localPosition = new Vector3(__instance._rectTransform.localPosition.x, barY.Value, -2);
            parent.localScale = new Vector3(barXScaleFloat/100, barYScaleFloat/100, 1);
            
            
            parent.Find("Accuracy Bar Backing").gameObject.SetActive(enableBacking.Value);
            
            // parent.Find("Rectangles").gameObject.SetActive(enableColourBar.Value);
            // parent.Find("Caps").gameObject.SetActive(enableColourBar.Value);
            
            //Awful Enabling
            parent.Find("Rectangles").GetChild(0).gameObject.SetActive(enableAwful.Value);
            parent.Find("Rectangles").GetChild(1).gameObject.SetActive(enableAwful.Value);
            parent.Find("Caps").gameObject.SetActive(enableAwful.Value);
            
            //Okay Enabling
            parent.Find("Rectangles").GetChild(2).gameObject.SetActive(enableOkay.Value);
            parent.Find("Rectangles").GetChild(3).gameObject.SetActive(enableOkay.Value);
            
            //Great Enabling
            parent.Find("Rectangles").GetChild(4).gameObject.SetActive(enableGreat.Value);
            parent.Find("Rectangles").GetChild(5).gameObject.SetActive(enableGreat.Value);
            
            //Perfect Enabling
            parent.Find("Rectangles").GetChild(6).gameObject.SetActive(enablePerfect.Value);
            parent.Find("Rectangles").GetChild(7).gameObject.SetActive(enablePerfect.Value);
            
            //PerfectPlus Enabling
            parent.Find("Rectangles").GetChild(8).gameObject.SetActive(enablePerfectPlus.Value);
        }
    }
}
//Todo: Allow Changing Pallete of the timing bar
//Todo: Have the "Enable Bar Colours" have a dropdown when enabled to disable and enable individual components (Impossible)
