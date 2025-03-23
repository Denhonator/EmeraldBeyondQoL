using EmeraldBeyond;
using Harmony;
using Il2Cpp;
using Il2CppBattle;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Reflection;
using Il2CppUI.CutScene;
using MelonLoader;
using UnityEngine;
using static MelonLoader.MelonLogger;

//[HarmonyLib.HarmonyPatch(typeof(Il2CppMakimono.UITime), "GetDeltaTime_UI", new Type[] { typeof(Il2CppMakimono.AnimationDirector.ETimeScaleMode) })]
//class UITime
//{
//    private static void Postfix(ref float __result)
//    {
//        __result *= 10;
//    }
//}

[HarmonyLib.HarmonyPatch(typeof(Il2CppMakimono.AnimationDirector), "GetCurrentState", new Type[] { typeof(int) })]
class PlayTime
{
    static string lastresult = "";
    private static void Postfix(int layerindex, ref string __result, ref Il2CppMakimono.AnimationDirector __instance)
    {
        __instance.UpdateSpecifiedAnim();
    }
}

[HarmonyLib.HarmonyPatch(typeof(AgentCutScene), "OnOpen", new Type[] {})]
class FastCutscenes
{
    private static void Postfix(ref AgentCutScene __instance)
    {
        MyMod.SetTurbo(true);
        if (!MyMod.acs.Contains(__instance))
        {
            MyMod.acs.Add(__instance);
#if DEBUG
            Msg($"Opened AgentCutScene {__instance.unitid}");
#endif
        }
    }
}

[HarmonyLib.HarmonyPatch(typeof(AgentCutScene), "OnClose", new Type[] { })]
class FastCutscenes2
{
    private static void Postfix(ref AgentCutScene __instance)
    {
        if (MyMod.acs.Contains(__instance))
        {
            MyMod.SetTurbo(false);
            MyMod.acs.Remove(__instance);
#if DEBUG
            Msg($"Closed AgentCutScene {__instance.unitid}");
#endif
        }
    }
}

//[HarmonyLib.HarmonyPatch(typeof(Il2Cpp.ScreenCutSpeechBubble), "OpenSpeechBubble", new Type[] { typeof(Il2CppUI.CutScene.SpeechBubbleCreateInfo), typeof(bool), typeof(Il2CppSystem.Action) })]
//class Bubble
//{
//    static void Postfix(Il2CppUI.CutScene.SpeechBubbleCreateInfo info, bool isFollowSelectBubble, Il2CppSystem.Action callback, ref Il2Cpp.ScreenCutSpeechBubble __instance)
//    {
//        MelonLogger.Msg(info.text);
//    }
//}

//[HarmonyLib.HarmonyPatch(typeof(Il2CppMakimono.Unit), "Transition", new Type[] { typeof(string), typeof(Il2CppMakimono.TransitionType), typeof(bool) })]
//class SetState
//{
//    static void Prefix(ref string unitname, ref Il2CppMakimono.TransitionType transition, ref bool playanimation, ref Il2CppMakimono.Unit __instance)
//    {
//        MelonLogger.Msg($"UnitTransition: {unitname} playanimation: {playanimation}");
//    }
//}
#if DEBUG
[HarmonyLib.HarmonyPatch(typeof(BattleInspiration), "GetArtsRate", new Type[] { typeof(BtArtsDataTableLabel) })]
class Inspiration
{
    static void Postfix(BtArtsDataTableLabel ArtsID, float __result)
    {
        Msg($"{ArtsID}: {__result}");
    }
}

[HarmonyLib.HarmonyPatch(typeof(BattleInspiration), "GetInspirationProbA")]
class InspirationA
{
    static void Postfix(int Difficulty, float ArtsRate, int HistoryBonus, float __result)
    {
        Msg($"Diff: {Difficulty} Rate: {ArtsRate} History: {HistoryBonus} Prob: {__result}%");
    }
}

[HarmonyLib.HarmonyPatch(typeof(BattleInspiration), "GetInspirationProbB")]
class InspirationB
{
    static void Postfix(int InspirationPoint, float __result)
    {
        Msg($"InspPoint: {InspirationPoint} Prob: {__result}%");
    }
}

[HarmonyLib.HarmonyPatch(typeof(CatheScript), "AftefFunc", new Type[] { typeof(CatheScript.FuncData), typeof(bool), typeof(CatheScript.Val) })]
class EventScriptFunc
{
    static void Postfix(CatheScript.FuncData data, bool needRet, CatheScript.Val returnVal, CatheScript __instance)
    {
        Msg($"{data.funcName} {data.GetArgNumStr()} \nReturn: {returnVal.valInt} {returnVal.valString}");
        MyMod.cs = __instance;
        if (data.funcName.Contains("Fade") || data.funcName.Contains("Actor") || data.funcName.Contains("Message"))
        {
            Cutscene.close = 1;
        }
    }
}

[HarmonyLib.HarmonyPatch(typeof(CatheScript), "ReverseDictAdd")]
class EventScriptFunc2
{
    static void Postfix(Dictionary<string, CatheScript.Val> dict, string name, CatheScript __instance)
    {
        Msg($"{name}");
    }
}

//[HarmonyLib.HarmonyPatch(typeof(CatheScript), nameof(CatheScript.EvaluteImmediate))]
//class EventScriptFunc2
//{
//    unsafe static void Prefix(CatheScript.ILType type, IntPtr datas)
//    {
//        int len = (datas + 0x18).ToInt32();
//        Msg(len);
//        IntPtr ip = datas + 0x38;
//        byte* bytes = (byte*)ip;
//        //for(int i=0; i<len; i++)
//        //{
//        //    string s = Encoding.Unicode.GetString(bytes, 10);
//        //    Msg(s);
//        //    bytes += 0x10;
//        //}


//        //Msg(Il2CppSystem.String.Join("",datas));
//    }
//}

//[HarmonyLib.HarmonyPatch(typeof(CatheScript), nameof(CatheScript.ILParse))]
//class EventScriptFunc3
//{
//    static void Postfix(string str, CatheScript.ILType __result, CatheScript __instance)
//    {
//        Msg($"ILParse: {__instance.line} = {str}");
//    }
//}

[HarmonyLib.HarmonyPatch(typeof(BattleDamage), "CulcEffectSuccessRate", new Type[] { typeof(Effect), typeof(int) })]
class EffectChance
{
    static void Postfix(Effect effectType, int OverrideSuccessRate, float __result)
    {
        Msg($"Effect: {effectType} Chance: {__result} Override: {OverrideSuccessRate}");
    }
}

[HarmonyLib.HarmonyPatch(typeof(BattleOverAttackInfo), "GetOverDriveProb")]
class OverdriveChance
{
    static void Postfix(int __result)
    {
        Msg($"OverdriveChance: {__result}");
    }
}

[HarmonyLib.HarmonyPatch(typeof(BattleRank), "UpdatePlayerRank", new Type[] { typeof(int) })]
class BattleRankMonitor
{
    static void Postfix(int DeadNum, BattleRank __instance)
    {
        Msg($"Dead: {DeadNum} \nBaseEnemy: {__instance.m_BaseEnemyRank} " +
            $"\nEnemy: {__instance.m_EnemyRank}" +
            $"\nPlayer: {__instance.m_PlayerRank}" +
            $"\nPotential: {__instance.m_PotentialBattleRank}");
    }
}

#endif

[HarmonyLib.HarmonyPatch(typeof(Il2CppMakimono.InputManager), "UpdateAction", new Type[] { })]
class Cutscene2
{
    static void Postfix(Il2CppMakimono.InputManager __instance)
    {
        MyMod.im = __instance;
    }
}

[HarmonyLib.HarmonyPatch(typeof(Il2CppMakimono.DecideButton), "Active", new Type[] { })]
class Cutscene3
{
    static void Postfix(ref Il2CppMakimono.DecideButton __instance)
    {
        if (__instance == null || __instance.m_CurrentState == Il2CppMakimono.DecideButton.StateKind.Inactive)
            return;
        if (Time.timeScale > 3.0f && __instance.navigation.mode==Il2CppMakimono.ButtonNavigation.Mode.Automatic)
            __instance.ClickDecide();
    }
}

[HarmonyLib.HarmonyPatch(typeof(PartsSpeechBubble), "Update")]
class Cutscene
{
    static Dictionary<string, int> timers = new Dictionary<string, int>();
    static bool startedSkipping = false;
    static float lastUpdate = 0;
    public static int close = 0;
    public static int stop = 0;
    
    static void Postfix(PartsSpeechBubble __instance)
    {
        if (Time.unscaledTime - lastUpdate > 0.5f)
        {
            startedSkipping = false;
            close = 0;
        }
        if(Input.GetKeyDown(KeyCode.F6))
        {
            Msg($"Text: {__instance.m_text.text}\n" +
                $"ActiveEnabled: {__instance.isActiveAndEnabled}\n" +
                $"Open: {__instance.IsOpen}\n" +
                $"OpenAnim: {__instance.m_isOpenAnimPlay}\n" +
                $"ActionCallback:");
        }
        if (close>0)
        {
            __instance.m_isOpenAnimPlay = false;
            __instance.Close();
            close -= 1;
        }
        bool heldDown = Il2CppMakimono.Input.GetButton(Il2CppMakimono.Input.InputCategory.UI, Il2CppMakimono.Input.Button.Cancel);
        //if (Il2CppMakimono.Input.GetButtonUp(Il2CppMakimono.Input.InputCategory.UI, Il2CppMakimono.Input.Button.Cancel))
        //    timers.Clear();
        if (__instance.m_isOpenAnimPlay && (heldDown||MyMod.turboSetting>1||startedSkipping) && stop==0)
        {
            //startedSkipping = true;
            if (!timers.ContainsKey(__instance.m_text.text))
            {
                foreach (string key in timers.Keys)
                    timers[key] -= 1;
                timers.Add(__instance.m_text.text, 3);
            }
            else
            {
                timers[__instance.m_text.text] -= 1;
            }
            if (timers[__instance.m_text.text] <= 0)
            {
                timers.Remove(__instance.m_text.text);
                __instance.m_isOpenAnimPlay = false;
                __instance.Close();
            }
            if (timers.ContainsKey(__instance.m_text.text) && timers[__instance.m_text.text] <= 2)
            {
                __instance.SetActive(__instance.m_isOpenAnimPlay);
                __instance.m_onActionCallback.Invoke(__instance.m_position);
                //__instance.m_isOpenAnimPlay = false;
                if (!__instance.IsOpen)
                {
                    __instance.Close();
                }
            }
        }
        else
            timers.Clear();

        lastUpdate = Time.unscaledTime;
    }
}

//[HarmonyLib.HarmonyPatch(typeof(PartsSpeechBubble), "Open", new Type[] { typeof(string), typeof(SpeechBubbleTailType), typeof(Action) })]
//class Cutscene
//{
//    static void Postfix(string text, SpeechBubbleTailType type, Action callback, PartsSpeechBubble __instance)
//    {
//        Msg(text);
//    }
//}

[HarmonyLib.HarmonyPatch(typeof(TitleController.LogoController.Each), "OnUpdate", new Type[] {  })]
class Logos
{
    static void Prefix(TitleController.LogoController.Each __instance)
    {
        MyMod.SetTurbo(false);
        MelonLogger.Msg($"OnUpdate {__instance.m_step} {__instance.m_nMode}");
        __instance.m_step = TitleController.LogoController.Each.EStep.AllFinished;
        
    }
}

[HarmonyLib.HarmonyPatch(typeof(GameManager), "OnApplicationFocus", new Type[] { typeof(bool) })]
class Focus
{
    static bool Prefix(ref bool focusStatus, GameManager __instance)
    {
        focusStatus = true;
        return false;
    }
}

[HarmonyLib.HarmonyPatch(typeof(GameManager), "OnApplicationPause", new Type[] { typeof(bool) })]
class Pause
{
    static bool Prefix(ref bool pauseStatus, GameManager __instance)
    {
        pauseStatus = true;
        return false;
    }
}

[HarmonyLib.HarmonyPatch(typeof(GameManager), "Update", new Type[] {})]
class background
{
    static void Prefix(GameManager __instance)
    {
        Application.runInBackground = true;
    }
    static void Postfix(GameManager __instance)
    {
        Application.runInBackground = true;
    }
}

namespace EmeraldBeyond
{
    public class MyMod : MelonMod
    {
        public static CatheScript cs;
        UnitUICommon unitUICommon = null;
        List<Component> comps = new List<Component>();
        public static UnitCutScene? cutscene;
        public static int turbo = 0;
        public static bool allowTurbo = false;
        public static List<AgentCutScene> acs = new List<AgentCutScene>();
        public static Il2CppMakimono.InputManager im = null;
        public static int turboSetting = 0;

        public static void SetTurbo(bool turboset)
        {
            allowTurbo = (turboset && turboSetting > 0);
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            string path = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')+1);
            string settingsfile = path + "QoLModSettings.txt";
            if (!File.Exists(settingsfile))
            {
                File.WriteAllText(settingsfile, "turbo = 0");
            }
            else
            {
                string[] sets = File.ReadAllText(settingsfile).Split("\n", StringSplitOptions.TrimEntries);
                foreach(string s in sets)
                {
                    if (s.Contains("turbo"))
                    {
                        turboSetting = int.Parse(s.Substring(s.IndexOf("=")+1));
                        //Msg("Cutscene turbo set to " + (turboSetting == 0 ? "disallowed" : turboSetting == 1 ? "hold to use" : "automatic"));
                    }
                }
            }
        }
        public override void OnUpdate()
        {
            bool heldDown = turboSetting > 1 || Il2CppMakimono.Input.GetButton(Il2CppMakimono.Input.InputCategory.UI, Il2CppMakimono.Input.Button.Cancel);
            if (heldDown && allowTurbo)
            {
                Time.timeScale = 4.0f;
                if (im != null)
                {
                    im.SetActionRepeat(Il2CppMakimono.Input.Button.Decision, true, 0.0f, 0.01f);
                }
            }
            else if (turbo > 0)
            {
                Time.timeScale = 4.0f;
                turbo -= 1;
            }
            else
            {
                Time.timeScale = Time.timeScale == 4.0f ? 1.0f : Time.timeScale;
                if(im!=null)
                    im.SetActionRepeat(Il2CppMakimono.Input.Button.D_Left, false, 0.0f, 0.01f);
            }

            if(Input.GetKeyDown(KeyCode.F1))
            {
                SoundController.BGM.Suspend(SoundController.BGM.ESuspendRequired.Start, true);
            }
            else if(Input.GetKeyDown(KeyCode.F2))
            {
                SoundController.BGM.Resume(SoundController.BGM.ESuspendRequired.Start, true);
            }
            else if (Input.GetKeyDown(KeyCode.F4))
            {
                TitleControllerArgs args = new TitleControllerArgs(false);
                //GameManager.GameModeArgsBase argsBase = new GameManager.GameModeArgsBase(GameManager.EGameMode.Title);
                GameManager.ChangeGameMode(args);
            }
#if DEBUG
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                BattleRank br = Singlton<BattleRank>.Instance;
                Msg($"BattleRank: {br.GetBattleRank()}" +
                $"\nEnemy: {br.m_EnemyRank}" +
                $"\nPlayer: {br.m_PlayerRank}" +
                $"\nPotential: {br.m_PotentialBattleRank}");
            }
            else if (Input.GetKeyDown(KeyCode.F5))
            {
                foreach (string key in cs.m_valGlobalDict.Keys)
                {
                    Msg($"{key} = {cs.m_valGlobalDict[key].valInt}{cs.m_valGlobalDict[key].valBool}{cs.m_valGlobalDict[key].valString}");
                }
                foreach (string key in cs.m_valLocalDict.Keys)
                {
                    Msg($"{key} = {cs.m_valLocalDict[key].valInt}{cs.m_valLocalDict[key].valBool}{cs.m_valLocalDict[key].valString}");
                }
            }
            else if (Input.GetKeyDown(KeyCode.PageUp) && Time.timeScale >= 1.0f && Time.timeScale < 5.0f)
            {
                Time.timeScale += 1.0f;
            }
            else if (Input.GetKeyDown(KeyCode.PageDown))
            {
                Time.timeScale = 1.0f;
            }
            else if (Input.GetKeyDown(KeyCode.F7))
            {
                foreach(AgentCutScene a in acs)
                    Msg($"AgentCutScene ID: {a.unitid}\nCutscene open: {a.IsOpen}\nCutscene enabled: {a.Enable}");
            }
#endif
            //if (Input.GetKeyDown(KeyCode.P))
            //{
            //    comps.Clear();
            //    foreach (GameObject g in SceneManager.GetActiveScene().GetRootGameObjects())
            //    {
            //        if (g.transform && g.transform.childCount > 0 && g.name.Contains("UI"))
            //        {
            //            RecursivePrint(g.transform, "-");
            //        }
            //    }
            //}
            //if(Input.GetKey(KeyCode.O))
            //{
            //    foreach(Component a in comps)
            //    {
            //        Il2CppReferenceArray<FieldInfo> finfos = a.GetIl2CppType().GetFields(BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public);
            //        Il2CppReferenceArray<MethodInfo> minfos = a.GetIl2CppType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            //        //FieldInfo finfo = a.GetIl2CppType().GetField("PlayBackTime");

            //        LoggerInstance.Msg(a.name);
            //        foreach (FieldInfo finfo in finfos)
            //        {
            //            if (finfo.GetValue(a) != null && finfo.Name == "playbackTime")
            //            {
            //                LoggerInstance.Msg($"{finfo.Name}, {finfo.FieldType.Name}: {finfo.GetValue(a).Unbox<float>()}");
            //            }
            //        }
            //        //foreach (MethodInfo minfo in minfos)
            //        //{
            //        //    string par = "";
            //        //    foreach(Il2CppSystem.Type t in minfo.GetParameterTypes())
            //        //    {
            //        //        par += t.Name;
            //        //    }
            //        //    LoggerInstance.Msg($"{minfo.Name}, {minfo.ReturnType.GetIl2CppType()}: {par}");
            //        //}
            //    }
            //}
            //if(Input.GetKey(KeyCode.T))
            //{
            //    Il2CppMakimono.UIManager.deltatime = Il2CppMakimono.UIManager.deltatime *= 5;
            //    Il2CppMakimono.UIManager.elapsedtime += Il2CppMakimono.UIManager.deltatime;
            //    LoggerInstance.Msg(Il2CppMakimono.UITime.GetDeltaTime_UI(Il2CppMakimono.AnimationDirector.ETimeScaleMode.Normal));
            //}
            //if(Input.GetKeyDown(KeyCode.O))
            //{
            //    foreach (GameObject g in SceneManager.GetActiveScene().GetRootGameObjects())
            //    {
            //        foreach(Makimono.AnimationDirector ad in g.GetComponentsInChildren<Makimono.AnimationDirector>())
            //        {
            //            ad.Stop();
            //            ad.StopAllCoroutines();
            //        }
            //    }
            //}
        }

        void RecursivePrint(Transform t, string pre)
        {
            Component[] components = t.GetComponents<Component>();
            string compnames = "";
            foreach (Component comp in components)
            {
                compnames += comp.GetIl2CppType().FullName+", ";
                if(comp.GetIl2CppType().FullName == "Makimono.AnimationDirector")
                {
                    comps.Add(comp);
                }
            }

            //LoggerInstance.Msg(pre + t.name +"\n"+comps);
            for(int i=0; i<t.childCount;i++)
            {
                if (pre.Length < 12)
                {
                    if(t.GetChild(i).gameObject.activeSelf)
                        RecursivePrint(t.GetChild(i), pre + "-");
                }
            }
        }

        //public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        //{
        //    LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been loaded!");
        //    foreach(GameObject g in SceneManager.GetActiveScene().GetRootGameObjects())
        //    {
        //        if (g.transform && g.transform.childCount>0 && g.name.Contains("UI"))
        //        {
        //            RecursivePrint(g.transform, "-");
        //        }
        //    }
        //}
    }
}