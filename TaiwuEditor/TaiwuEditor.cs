using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony12;
using System.Reflection;
using System.Timers;
using TaiwuEditor;
using UnityEngine;
using UnityModManagerNet;

namespace TaiwuEditor
{
    public static class Main
    {
        [HarmonyPatch(typeof(MassageWindow), "SetMassageWindow")]
        private static class SAW_Hook
        {
            private static bool Prefix(ref int[] baseEventDate)
            {
                if (settings.lockMaxOutRelation)
                {
                    int key = baseEventDate[2];
                    int num = DateFile.instance.ParseInt(DateFile.instance.eventDate[key][2]);
                    int num2 = DateFile.instance.MianActorID();
                    int num3;
                    switch (num)
                    {
                        case 0:
                            num3 = baseEventDate[1];
                            break;
                        case -1:
                            num3 = num2;
                            break;
                        default:
                            num3 = num;
                            break;
                    }
                    int num4 = num3;
                    if (num4 != num2 && DateFile.instance.actorsDate.ContainsKey(num4))
                    {
                        try
                        {
                            DateFile.instance.ChangeFavor(num4, 60000, updateActor: true, showMassage: false);
                        }
                        catch (Exception ex)
                        {
                            logger.Log("TaiwuEditor");
                            logger.Log("好感修改失败");
                            logger.Log(ex.Message);
                            logger.Log(ex.StackTrace);
                        }
                        if (settings.lockMaxLifeFace)
                        {
                            try
                            {
                                DateFile.instance.ChangeActorLifeFace(num4, 0, 100);
                            }
                            catch (Exception ex2)
                            {
                                logger.Log("TaiwuEditor");
                                logger.Log("印象修改失败");
                                logger.Log(ex2.Message);
                                logger.Log(ex2.StackTrace);
                            }
                        }
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(HomeSystem), "StudySkillUp")]
        private static class SSU_Hook
        {
            private static bool Prefix(ref int ___studySkillId, ref int ___studySkillTyp, ref HomeSystem __instance)
            {
                if (!settings.lockMaxOutProficiency)
                {
                    return true;
                }
                int num = DateFile.instance.MianActorID();
                if (___studySkillId > 0)
                {
                    if (___studySkillTyp == 17)
                    {
                        int num2 = DateFile.instance.ParseInt(DateFile.instance.gongFaDate[___studySkillId][2]);
                        DateFile.instance.addGongFaStudyValue = 0;
                        DateFile.instance.ChangeActorGongFa(num, ___studySkillId, 100, 0, 0, nomoUp: false);
                        DateFile.instance.AddActorScore(302, num2 * 100);
                        if (DateFile.instance.GetGongFaLevel(num, ___studySkillId) >= 100 && DateFile.instance.GetGongFaFLevel(num, ___studySkillId) >= 10)
                        {
                            DateFile.instance.AddActorScore(304, DateFile.instance.ParseInt(DateFile.instance.gongFaDate[___studySkillId][2]) * 100);
                        }
                    }
                    else
                    {
                        int num3 = DateFile.instance.ParseInt(DateFile.instance.skillDate[___studySkillId][2]);
                        DateFile.instance.addSkillStudyValue = 0;
                        DateFile.instance.ChangeMianSkill(___studySkillId, 100, 0, nomoUp: false);
                        DateFile.instance.AddActorScore(202, num3 * 100);
                        if (DateFile.instance.GetSkillLevel(___studySkillId) >= 100 && DateFile.instance.GetSkillFLevel(___studySkillId) >= 10)
                        {
                            DateFile.instance.AddActorScore(204, num3 * 100);
                        }
                    }
                    __instance.UpdateStudySkillWindow();
                    __instance.UpdateLevelUPSkillWindow();
                    __instance.UpdateReadBookWindow();
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(StorySystem), "OpenStory")]
        private static class OS_Hook
        {
            private static bool Prefix(ref StorySystem __instance)
            {
                if (!settings.lockFastQiyuCompletion)
                {
                    return true;
                }
                if (__instance.storySystemStoryId == 10002 || __instance.storySystemStoryId == 10003 || __instance.storySystemStoryId == 10004)
                {
                    return true;
                }
                __instance.ClossToStoryMenu();
                int num = DateFile.instance.ParseInt(DateFile.instance.baseStoryDate[__instance.storySystemStoryId][302]);
                if (num != 0)
                {
                    DateFile.instance.SetEvent(new int[3]
                    {
                    0,
                    -1,
                    num
                    }, addToFirst: true);
                    logger.Log("MassageWindow.DoEvent called");
                    MassageWindow_DoEvent.Invoke(MassageWindow.instance, new object[1]
                    {
                    0
                    });
                }
                else
                {
                    DateFile.instance.SetStory(true, __instance.storySystemPartId, __instance.storySystemPlaceId, 0, 0);
                    __instance.StoryEnd();
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(ActorMenu), "GetMaxItemSize")]
        private static class GMIS_Hook
        {
            private static void Postfix(ref int key, ref int __result)
            {
                if (settings.lockNeverOverweigh && DateFile.instance.mianActorId == key)
                {
                    __result = 999999999;
                }
            }
        }

        [HarmonyPatch(typeof(HomeSystem), "StartReadBook")]
        private static class SRB_Hook
        {
            private static bool Prefix()
            {
                if (!settings.lockFastRead)
                {
                    return true;
                }
                EasyReadV2();
                HomeSystem.instance.UpdateReadBookWindow();
                return false;
            }
        }

        private static UnityModManager.ModEntry.ModLogger logger;

        private static string errorString = "存档未载入！";

        private static Timer timer;

        private static FieldInfo ReadBook_ReadLevel;

        private static MethodInfo MassageWindow_DoEvent;

        private static Settings settings;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            HarmonyInstance val = HarmonyInstance.Create(modEntry.Info.Id);
            val.PatchAll(Assembly.GetExecutingAssembly());
            settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            logger = modEntry.Logger;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            ReadBook_ReadLevel = typeof(ReadBook).GetField("readLevel", BindingFlags.Instance | BindingFlags.NonPublic);
            if (ReadBook_ReadLevel == null)
            {
                logger.Log("获取ReadBook.readLevel失败");
            }
            MassageWindow_DoEvent = typeof(MassageWindow).GetMethod("DoEvent", BindingFlags.Instance | BindingFlags.NonPublic);
            if (MassageWindow_DoEvent == null)
            {
                logger.Log("获取MassageWindow.DoEvent失败");
            }
            timer = new Timer();
            timer.Interval = 1000.0;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            return true;
        }

        private static void EasyReadV2()
        {
            List<int[]> list = new List<int[]>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(new int[6]);
            }
            for (int j = 0; j < 10; j++)
            {
                int num = DateFile.instance.MianActorID();
                int num2 = 100;
                for (int k = 0; k < 3; k++)
                {
                    int key = list[j][k];
                    num2 += DateFile.instance.ParseInt(DateFile.instance.readBookDate[key][6]);
                }
                int num3 = DateFile.instance.ParseInt(DateFile.instance.GetItemDate(HomeSystem.instance.readBookId, 32));
                int num4 = DateFile.instance.ParseInt(DateFile.instance.GetItemDate(HomeSystem.instance.readBookId, 34)) * num2 / 100;
                int[] bookPage = DateFile.instance.GetBookPage(HomeSystem.instance.readBookId);
                if (HomeSystem.instance.studySkillTyp == 17)
                {
                    if (!DateFile.instance.gongFaBookPages.ContainsKey(num3))
                    {
                        DateFile.instance.gongFaBookPages.Add(num3, new int[10]);
                    }
                    int num5 = DateFile.instance.gongFaBookPages[num3][j];
                    if (num5 != 1 && num5 > -100)
                    {
                        int num6 = DateFile.instance.ParseInt(DateFile.instance.gongFaDate[num3][2]);
                        int num7 = DateFile.instance.ParseInt(DateFile.instance.GetItemDate(HomeSystem.instance.readBookId, 35));
                        DateFile.instance.ChangeActorGongFa(num, num3, 0, 0, num7);
                        if (num7 != 0)
                        {
                            ActorMenu.instance.ChangeMianQi(num2, 50 * num6);
                        }
                        list[j][5] = 1;
                        DateFile.instance.gongFaBookPages[num3][j] = 1;
                        DateFile.instance.AddActorScore(303, num6 * 100);
                        if (DateFile.instance.GetGongFaLevel(num, num3) >= 100 && DateFile.instance.GetGongFaFLevel(num, num3) >= 10)
                        {
                            DateFile.instance.AddActorScore(304, num6 * 100);
                        }
                        if (bookPage[j] == 0)
                        {
                            DateFile.instance.AddActorScore(305, num6 * 100);
                        }
                    }
                    else
                    {
                        num4 = num4 * 10 / 100;
                    }
                    DateFile.instance.gongFaExperienceP += num4;
                    continue;
                }
                if (!DateFile.instance.skillBookPages.ContainsKey(num3))
                {
                    DateFile.instance.skillBookPages.Add(num3, new int[10]);
                }
                int num8 = DateFile.instance.skillBookPages[num3][j];
                if (num8 != 1 && num8 > -100)
                {
                    int num9 = DateFile.instance.ParseInt(DateFile.instance.skillDate[num3][2]);
                    if (!DateFile.instance.actorSkills.ContainsKey(num3))
                    {
                        DateFile.instance.ChangeMianSkill(num3, 0, 0);
                    }
                    list[j][5] = 1;
                    DateFile.instance.skillBookPages[num3][j] = 1;
                    DateFile.instance.AddActorScore(203, num9 * 100);
                    if (DateFile.instance.GetSkillLevel(num3) >= 100 && DateFile.instance.GetSkillFLevel(num3) >= 10)
                    {
                        DateFile.instance.AddActorScore(204, num9 * 100);
                    }
                    if (bookPage[j] == 0)
                    {
                        DateFile.instance.AddActorScore(205, num9 * 100);
                    }
                }
                else
                {
                    num4 = num4 * 10 / 100;
                }
                DateFile.instance.gongFaExperienceP += num4;
            }
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (DateFile.instance != null && settings.lockTime)
            {
                DateFile.instance.dayTime = 30;
            }
        }

        private static void FieldHelper(int resId, string fieldname)
        {
            DateFile instance = DateFile.instance;
            bool flag = instance == null || instance.actorsDate == null || !instance.actorsDate.ContainsKey(instance.mianActorId);
            GUILayout.BeginHorizontal("Box", (GUILayoutOption[])new GUILayoutOption[0]);
            GUILayout.Label(fieldname, (GUILayoutOption[])new GUILayoutOption[0]);
            if (flag)
            {
                GUILayout.TextField(errorString, (GUILayoutOption[])new GUILayoutOption[0]);
            }
            else
            {
                Dictionary<int, string> dictionary = instance.actorsDate[instance.mianActorId];
                instance.actorsDate[instance.mianActorId][resId] = GUILayout.TextField(instance.actorsDate[instance.mianActorId][resId], (GUILayoutOption[])new GUILayoutOption[0]);
            }
            GUILayout.EndHorizontal();
        }

        private static void FieldHelper(ref int field, string fieldname)
        {
            DateFile instance = DateFile.instance;
            bool flag = instance == null || instance.actorsDate == null || !instance.actorsDate.ContainsKey(instance.mianActorId);
            GUILayout.BeginHorizontal("Box", (GUILayoutOption[])new GUILayoutOption[0]);
            GUILayout.Label(fieldname, (GUILayoutOption[])new GUILayoutOption[0]);
            if (flag)
            {
                GUILayout.TextField(errorString, (GUILayoutOption[])new GUILayoutOption[0]);
            }
            else
            {
                Dictionary<int, string> dictionary = instance.actorsDate[instance.mianActorId];
                field = DateFile.instance.ParseInt(GUILayout.TextField(field.ToString(), (GUILayoutOption[])new GUILayoutOption[0]));
            }
            GUILayout.EndHorizontal();
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        public static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.BeginHorizontal("Box", (GUILayoutOption[])new GUILayoutOption[0]);
            GUILayout.BeginVertical("Box", (GUILayoutOption[])new GUILayoutOption[0]);
            FieldHelper(61, "膂力");
            FieldHelper(62, "体质");
            FieldHelper(63, "灵敏");
            FieldHelper(64, "根骨");
            FieldHelper(65, "悟性");
            FieldHelper(66, "定力");
            GUILayout.Space(10f);
            FieldHelper(401, "食材");
            FieldHelper(402, "木材");
            FieldHelper(403, "金石");
            FieldHelper(404, "织物");
            FieldHelper(405, "草药");
            FieldHelper(406, "金钱");
            FieldHelper(407, "声望");
            GUILayout.Space(10f);
            FieldHelper(ref DateFile.instance.gongFaExperienceP, "历练");
            settings.lockTime = GUILayout.Toggle(settings.lockTime, "锁定一月行动不减", (GUILayoutOption[])new GUILayoutOption[0]);
            settings.lockFastRead = GUILayout.Toggle(settings.lockFastRead, "快速读书（对残缺篇章有效）", (GUILayoutOption[])new GUILayoutOption[0]);
            settings.lockMaxOutProficiency = GUILayout.Toggle(settings.lockMaxOutProficiency, "修习单击全满", (GUILayoutOption[])new GUILayoutOption[0]);
            settings.lockFastQiyuCompletion = GUILayout.Toggle(settings.lockFastQiyuCompletion, "奇遇直接到达目的地", (GUILayoutOption[])new GUILayoutOption[0]);
            settings.lockNeverOverweigh = GUILayout.Toggle(settings.lockNeverOverweigh, "身上物品永不超重（仓库无效）", (GUILayoutOption[])new GUILayoutOption[0]);
            settings.lockMaxOutRelation = GUILayout.Toggle(settings.lockMaxOutRelation, "见面关系全满", (GUILayoutOption[])new GUILayoutOption[0]);
            settings.lockMaxLifeFace = GUILayout.Toggle(settings.lockMaxLifeFace, "见面印象最深", (GUILayoutOption[])new GUILayoutOption[0]);
            GUILayout.EndVertical();
            GUILayout.BeginVertical("Box", (GUILayoutOption[])new GUILayoutOption[0]);
            FieldHelper(501, "音律");
            FieldHelper(502, "弈棋");
            FieldHelper(503, "诗书");
            FieldHelper(504, "绘画");
            FieldHelper(505, "术数");
            FieldHelper(506, "品鉴");
            FieldHelper(507, "锻造");
            FieldHelper(508, "制木");
            FieldHelper(509, "医术");
            FieldHelper(510, "毒术");
            FieldHelper(511, "织锦");
            FieldHelper(512, "巧匠");
            FieldHelper(513, "道法");
            FieldHelper(514, "佛学");
            FieldHelper(515, "厨艺");
            FieldHelper(516, "杂学");
            GUILayout.EndVertical();
            GUILayout.BeginVertical("Box", (GUILayoutOption[])new GUILayoutOption[0]);
            FieldHelper(601, "内功");
            FieldHelper(602, "身法");
            FieldHelper(603, "绝技");
            FieldHelper(604, "拳掌");
            FieldHelper(605, "指法");
            FieldHelper(606, "腿法");
            FieldHelper(607, "暗器");
            FieldHelper(608, "剑法");
            FieldHelper(609, "刀法");
            FieldHelper(610, "长兵");
            FieldHelper(611, "奇门");
            FieldHelper(612, "软兵");
            FieldHelper(613, "御射");
            FieldHelper(614, "乐器");
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }

    public class Settings : UnityModManager.ModSettings
    {
        public bool lockTime = false;

        public bool lockFastRead = false;

        public bool lockMaxOutProficiency = false;

        public bool lockFastQiyuCompletion = false;

        public bool lockNeverOverweigh = false;

        public bool lockMaxOutRelation = false;

        public bool lockMaxLifeFace = false;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            UnityModManager.ModSettings.Save(this, modEntry);
        }
    }
}
