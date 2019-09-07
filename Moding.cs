using System;
using System.Reflection;
using Harmony;

namespace AssembleHM_EFFUP_MOD
{
    public class Harmony_Patch
    {
        public Harmony_Patch()
        {
            try
            {
                HarmonyInstance harmonyInstance = HarmonyInstance.Create("Lobotomy.AssembleHM.EFFUP");
                // Energy Patch
                harmonyInstance.Patch(typeof(StageTypeInfo).GetMethod("GetEnergyNeed"), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("GetEnergyNeed")), null, null);

                // Clipoter Patch
                harmonyInstance.Patch(typeof(CreatureOverloadManager).GetMethod("AddOverloadGague"), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("AddOverloadGague")), null, null);
                harmonyInstance.Patch(typeof(GameStatusUI.EnergyController).GetMethod("SetOverloadGauge", AccessTools.all), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("SetOverloadGauge", AccessTools.all)), null, null);

                // Stat Patch && PE-box Patch && Don't Effect to monster feeling
                harmonyInstance.Patch(typeof(UseSkill).GetMethod("FinishWorkSuccessfully", AccessTools.all), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("FinishWorkSuccessfully", AccessTools.all)), null, null);
                harmonyInstance.Patch(typeof(UseSkill).GetMethod("GetCurrentFeelingState", AccessTools.all), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("GetCurrentFeelingState", AccessTools.all)), null, null);

                // Work Speed Patch
                harmonyInstance.Patch(typeof(UseSkill).GetMethod("InitUseSkillAction", AccessTools.all), null, new HarmonyMethod(typeof(Harmony_Patch).GetMethod("InitUseSkillAction", AccessTools.all)), null);
            }
            catch (Exception ex)
            {

            }
        }

        public static bool GetEnergyNeed(ref float __result, int day)
        {
            if (GlobalGameManager.instance.gameMode == GameMode.TUTORIAL)
            {
                __result = 20f;
                return false;
            }
            if (day >= StageTypeInfo.instnace.energyVal.Length)
            {
                if(day < 20)
                {
                    __result = 100f;
                    return false;
                }
                __result =  (float)(StageTypeInfo.instnace.energyVal[19] + (day - 19) * 30)/3;
                return false;
            }
            __result = (float)StageTypeInfo.instnace.energyVal[day]/3;
            return false;
        }

        public static bool AddOverloadGague(CreatureOverloadManager __instance)
        {
            int loadgauge  = (int)__instance.GetType().GetField("qliphothOverloadGauge", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
            __instance.GetType().GetField("qliphothOverloadGauge", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(__instance, loadgauge+2);
            return true;
        }

        public static bool SetOverloadGauge(ref int num, ref int max)
        {
            num /= 3;
            max = (max / 3 + 1);
            return true;
        }

        public static bool FinishWorkSuccessfully(UseSkill __instance)
        {
            __instance.successCount *= 3;
            return true;
        }

        public static bool GetCurrentFeelingState(UseSkill __instance)
        {
            __instance.successCount /= 3;
            return true;
        }

        public static void InitUseSkillAction(ref UseSkill __result)
        {
            __result.workSpeed *= 10;
        }
    }
}