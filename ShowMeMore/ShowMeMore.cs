using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony12;
using ShowMeMore;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using UnityEngine.UI;
using Random = System.Random;

namespace ShowMeMore
{
    public class Main
    {
        public static Settings settings;

        public static bool enabled;

        public static UnityModManager.ModEntry.ModLogger Logger;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            Logger = modEntry.Logger;
            settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            HarmonyInstance.Create(modEntry.Info.Id).PatchAll(Assembly.GetExecutingAssembly());
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            return true;
        }

        public static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (!value)
            {
                return false;
            }
            enabled = value;
            return true;
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical("Box", (GUILayoutOption[])new GUILayoutOption[0]);
            GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
            settings.ShowGongFaMassage = GUILayout.Toggle(settings.ShowGongFaMassage, "显示功法隐藏属性", (GUILayoutOption[])new GUILayoutOption[1]
            {
            GUILayout.Width(150f)
            });
            settings.ShowHitRatio = (GUILayout.Toggle(settings.ShowHitRatio, "显示功法穿透命中", (GUILayoutOption[])new GUILayoutOption[1]
            {
            GUILayout.Width(150f)
            }) && settings.ShowGongFaMassage);
            settings.ShowRealTime = (GUILayout.Toggle(settings.ShowRealTime, "<color=#8E8E8EFF>显示真实释放时间*</color>", (GUILayoutOption[])new GUILayoutOption[1]
            {
            GUILayout.Width(150f)
            }) && settings.ShowGongFaMassage);
            GUILayout.EndHorizontal();
            GUILayout.Label("<color=#8E8E8EFF>*不选真实释放时间则显示的释放时间是原始数据/100。真实释放时间是40（伤害功法）或20（其他） + 原始数据 * 25 / 100</color>", (GUILayoutOption[])new GUILayoutOption[0]);
            GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
            settings.ShowQiRate = GUILayout.Toggle(settings.ShowQiRate, "显示内力修习进度", (GUILayoutOption[])new GUILayoutOption[1]
            {
            GUILayout.Width(150f)
            });
            settings.AddHunYuan = (GUILayout.Toggle(settings.AddHunYuan, "混元内力加在各项*", (GUILayoutOption[])new GUILayoutOption[1]
            {
            GUILayout.Width(150f)
            }) && settings.ShowQiRate);
            GUILayout.EndHorizontal();
            GUILayout.Label("<color=#8E8E8EFF>在内力属性悬浮窗增加显示</color><color=#FBFBFBFF>各属性真实内力/学到的内功修满后的内力</color><color=#8E8E8EFF>，方便练混元。*默认各属性显示数值不加混元内力。</color>", (GUILayoutOption[])new GUILayoutOption[0]);
            settings.ShowQuquMassage = GUILayout.Toggle(settings.ShowQuquMassage, "显示蛐蛐隐藏属性", (GUILayoutOption[])new GUILayoutOption[0]);
            GUILayout.Label("<color=#8E8E8EFF>*显示的</color><color=#AE5AC8FF>“提供威望”</color><color=#8E8E8EFF>是指放在</color><color=#E4504DFF>神一品蛐蛐罐</color><color=#8E8E8EFF>能提供的威望（低品蛐蛐罐会造成提供的威望打折）</color>", (GUILayoutOption[])new GUILayoutOption[0]);
            GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
            settings.ShowWeaponMassage = (GUILayout.Toggle(settings.ShowWeaponMassage, "显示武器隐藏信息", (GUILayoutOption[])new GUILayoutOption[1]
            {
            GUILayout.Width(150f)
            }) && settings.ShowItemMassage);
            settings.ShowMoreAttackTimeAtEffect = (GUILayout.Toggle(settings.ShowMoreAttackTimeAtEffect, "<color=#8E8E8EFF>连击率按千年醉状态计算</color>", (GUILayoutOption[])new GUILayoutOption[1]
            {
            GUILayout.Width(180f)
            }) && settings.ShowWeaponMassage);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
            settings.ShowHomeMassage = GUILayout.Toggle(settings.ShowHomeMassage, "显示太吾村年均收益<color=#8E8E8EFF>——位于太吾村地块资源面板图标浮动信息</color>", (GUILayoutOption[])new GUILayoutOption[1]
            {
            GUILayout.Width(500f)
            });
            settings.ShowHomeMassageAtTop = (GUILayout.Toggle(settings.ShowHomeMassageAtTop, "在顶部资源栏也显示", (GUILayoutOption[])new GUILayoutOption[1]
            {
            GUILayout.Width(150f)
            }) && settings.ShowHomeMassage);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        private static void SetGUIToToggle(int index, string name, ref List<int> field)
        {
            bool flag = GUILayout.Toggle(field.Contains(index), name, (GUILayoutOption[])new GUILayoutOption[0]);
            if (!GUI.changed)
            {
                return;
            }
            if (flag)
            {
                if (!field.Contains(index))
                {
                    field.Add(index);
                }
            }
            else if (field.Contains(index))
            {
                field.Remove(index);
            }
        }
    }

    public class Settings : UnityModManager.ModSettings
    {
        public bool ShowDebug = false;

        public bool ShowGongFaMassage = true;

        public bool ShowHitRatio = true;

        public bool ShowRealTime = false;

        public bool ShowQiRate = true;

        public bool AddHunYuan = false;

        public bool ShowQuquMassage = true;

        public bool ShowMakingMassage = true;

        public bool ShowItemMassage = true;

        public bool ShowWeaponMassage = true;

        public bool ShowMoreAttackTimeAtEffect = false;

        public bool ShowHomeMassage = true;

        public bool ShowHomeMassageAtTop = true;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            UnityModManager.ModSettings.Save(this, modEntry);
        }
    }

    [HarmonyPatch(typeof(UIDate), "UpdateResourceText")]
    public static class ShowMeMore_UIDate_UpdateResourceText_Patch
    {
        public static void Postfix(GameObject resource)
        {
            if (!Main.enabled || !Main.settings.ShowHomeMassageAtTop || DateFile.instance.gameLine < 10 || ActorMenu.instance == null || resource == null)
            {
                return;
            }
            int num = DateFile.instance.MianActorID();
            string text = WindowManage.instance.informationName.text;
            string[] array = resource.name.Split(',');
            int num2 = (array.Length <= 1) ? (-1) : int.Parse(array[1]);
            if (num2 == -1)
            {
                return;
            }
            float[] taiwuIncome = ShowMeMore_WindowManage_WindowSwitch_Patch.GetTaiwuIncome();
            float[] array2 = new float[2];
            if (taiwuIncome[0] == array2[0])
            {
                return;
            }
            switch (num2)
            {
                case 5:
                    {
                        string text3 = "";
                        text3 = text3 + "\n" + DateFile.instance.SetColoer(20008, "+ " + $"{taiwuIncome[0]:F1}".ToString()) + DateFile.instance.SetColoer(20011, " / 年（期望）") + "来自" + DateFile.instance.SetColoer(10002, "太吾村建筑") + "\n";
                        Text informationMassage2 = WindowManage.instance.informationMassage;
                        informationMassage2.text = informationMassage2.text + text3;
                        break;
                    }
                case 6:
                    {
                        string text2 = "";
                        int taiwuCiTangPrestige = ShowMeMore_WindowManage_WindowSwitch_Patch.GetTaiwuCiTangPrestige();
                        int allQuquPrestige = QuquBox.instance.GetAllQuquPrestige();
                        text2 = text2 + "\n" + DateFile.instance.SetColoer(20007, "+ " + $"{taiwuIncome[1]:F1}".ToString()) + DateFile.instance.SetColoer(20011, " / 年（期望）") + "来自" + DateFile.instance.SetColoer(10002, "太吾村经营建筑") + "\n";
                        if (taiwuCiTangPrestige != 0)
                        {
                            text2 = text2 + DateFile.instance.SetColoer(20007, "+ " + taiwuCiTangPrestige.ToString() + " 威望") + " / " + DateFile.instance.SetColoer(20008, "「立春」") + "来自" + DateFile.instance.SetColoer(20009, "太吾氏祠堂") + "\n";
                        }
                        if (allQuquPrestige > 0)
                        {
                            text2 = text2 + DateFile.instance.SetColoer(20007, "+ " + allQuquPrestige.ToString() + " 威望") + " / " + DateFile.instance.SetColoer(20008, "「立秋」") + "来自" + DateFile.instance.SetColoer(20009, "太吾村") + "陈列的促织\n";
                        }
                        Text informationMassage = WindowManage.instance.informationMassage;
                        informationMassage.text = informationMassage.text + text2;
                        break;
                    }
            }
        }
    }

    [HarmonyPatch(typeof(WindowManage), "QuquMassage")]
    public static class ShowMeMore_WindowManage_QuquMassage_Patch
    {
        private static void Postfix(WindowManage __instance, int ququId, ref string __result)
        {
            if (Main.enabled && Main.settings.ShowQuquMassage)
            {
                string str = "";
                int ququDate = GetQuquWindow.instance.GetQuquDate(ququId, 31, injurys: false);
                int ququDate2 = GetQuquWindow.instance.GetQuquDate(ququId, 32, injurys: false);
                int ququDate3 = GetQuquWindow.instance.GetQuquDate(ququId, 36, injurys: false);
                int num = ququDate + ququDate3;
                int ququDate4 = GetQuquWindow.instance.GetQuquDate(ququId, 33, injurys: false);
                int ququDate5 = GetQuquWindow.instance.GetQuquDate(ququId, 34, injurys: false);
                int ququDate6 = GetQuquWindow.instance.GetQuquDate(ququId, 35);
                int num2 = 2 * DateFile.instance.GetQuquPrestige(ququId);
                int num3 = int.Parse(DateFile.instance.GetItemDate(ququId, 8));
                bool flag = ququId % 5 == 0 && num3 < 8;
                str += DateFile.instance.SetColoer(10002, "\n【隐藏属性】\n");
                str += string.Format("{0}{1}{2}", WindowManage.instance.Dit(), "暴击概率： ", DateFile.instance.SetColoer(20009, ququDate.ToString() + "%"));
                str += string.Format(" ({0}{1}点伤害， ", "增加", DateFile.instance.SetColoer(20009, ququDate2.ToString()));
                str += string.Format("{0}{1})\n", "击伤概率", DateFile.instance.SetColoer(20010, num.ToString() + "%"));
                str += string.Format("{0}{1}{2} ", WindowManage.instance.Dit(), "格挡概率： ", DateFile.instance.SetColoer(20005, ququDate4.ToString() + "%"));
                str += string.Format(" ({0}{1})\n", "减少伤害：", DateFile.instance.SetColoer(20005, ququDate5.ToString()));
                str += string.Format("{0}{1}{2}\n", WindowManage.instance.Dit(), "反击概率： ", DateFile.instance.SetColoer(20006, ququDate6.ToString() + "%"));
                str += string.Format("{0}{1}{2}\n", WindowManage.instance.Dit(), "促织品相： ", DateFile.instance.SetColoer((flag || num3 >= 8) ? 20009 : 20002, flag ? "神采非凡" : "平平无奇"));
                str += string.Format("{0}{1}+{2}\n", WindowManage.instance.Dit(), "提供威望： ", DateFile.instance.SetColoer(20007, num2.ToString()) + " /" + DateFile.instance.SetColoer(20008, "「立秋」"));
                __result += str;
            }
        }
    }

    [HarmonyPatch(typeof(WindowManage), "ShowGongFaMassage")]
    public static class ShowMeMore_WindowManage_ShowGongFaMassage_Patch
    {
        private static void Postfix(WindowManage __instance, int skillId, int skillTyp, int levelTyp, int actorId, Toggle toggle, ref Text ___informationMassage, ref string ___baseGongFaMassage)
        {
            if (!Main.enabled || !Main.settings.ShowGongFaMassage || skillTyp == 0 || skillTyp != 1 || int.Parse(DateFile.instance.gongFaDate[skillId][10]) == 0)
            {
                return;
            }
            int.Parse(DateFile.instance.gongFaDate[skillId][103]);
            int actorId2 = (actorId != -1) ? actorId : ((!ActorMenu.instance.actorMenu.activeInHierarchy) ? DateFile.instance.MianActorID() : ActorMenu.instance.acotrId);
            int num = (levelTyp != -1 && levelTyp != 0) ? 10 : ((skillId != 0) ? DateFile.instance.GetGongFaFLevel(actorId2, skillId) : 0);
            string str = ___baseGongFaMassage;
            string str2 = "";
            str2 += DateFile.instance.SetColoer(10002, "\n【隐藏属性】\n");
            float num2 = float.Parse(DateFile.instance.gongFaDate[skillId][604]);
            float num3 = float.Parse(DateFile.instance.gongFaDate[skillId][614]);
            float num4 = float.Parse(DateFile.instance.gongFaDate[skillId][615]);
            float num5 = float.Parse(DateFile.instance.gongFaDate[skillId][601]);
            float num6 = float.Parse(DateFile.instance.gongFaDate[skillId][602]);
            float num7 = float.Parse(DateFile.instance.gongFaDate[skillId][603]);
            int num8 = int.Parse(DateFile.instance.gongFaDate[skillId][2]);
            int num9 = 80;
            if (num2 > 0f)
            {
                str2 = str2 + WindowManage.instance.Dit() + "基础伤害：";
                if (num8 > 7)
                {
                    num2 *= 10f;
                    num9 = 100;
                }
                else
                {
                    num2 *= 8f;
                    num9 = 80;
                }
                str2 += DateFile.instance.SetColoer(20003, num2.ToString() + "%\n");
                if (Main.settings.ShowHitRatio)
                {
                    num5 *= (float)num9;
                    num6 *= (float)num9;
                    num7 *= (float)num9;
                    str2 = str2 + WindowManage.instance.Dit() + "基础命中：";
                    str2 = ((!(num7 < 0f)) ? (str2 + "迅疾" + DateFile.instance.SetColoer(20005, num7.ToString())) : (str2 + "迅疾" + DateFile.instance.SetColoer(20005, "无懈")));
                    str2 = ((!(num6 < 0f)) ? (str2 + DateFile.instance.massageDate[10][4] + "精妙" + DateFile.instance.SetColoer(20005, num6.ToString())) : (str2 + DateFile.instance.massageDate[10][4] + "精妙" + DateFile.instance.SetColoer(20005, "无懈")));
                    str2 = ((!(num5 < 0f)) ? (str2 + DateFile.instance.massageDate[10][4] + "力道" + DateFile.instance.SetColoer(20005, num5.ToString()) + "\n") : (str2 + DateFile.instance.massageDate[10][4] + "力道" + DateFile.instance.SetColoer(20005, "无懈") + "\n"));
                    num3 *= (float)num9;
                    num4 *= (float)num9;
                    str2 = str2 + WindowManage.instance.Dit() + "基础穿透：";
                    str2 = str2 + DateFile.instance.SetColoer(20003, "破体") + DateFile.instance.SetColoer(20006, num3.ToString()) + DateFile.instance.massageDate[10][4];
                    str2 = str2 + DateFile.instance.SetColoer(20003, "破气") + DateFile.instance.SetColoer(20006, num4.ToString()) + "\n";
                }
            }
            str2 = str2 + WindowManage.instance.Dit() + "释放时间：";
            if (Main.settings.ShowRealTime)
            {
                str2 += DateFile.instance.SetColoer(20005, BattleVaule.instance.GetGongFaMaxUseTime(skillId).ToString());
            }
            else
            {
                float num10 = float.Parse(DateFile.instance.gongFaDate[skillId][10]) / 100f;
                str2 += DateFile.instance.SetColoer(20005, num10.ToString());
            }
            str2 += "\n";
            string text = "胸背|腰腹|头颈|左臂|右臂|左腿|右腿|心神|毒质|全身";
            string text2 = "";
            string text3 = "";
            for (int i = 0; i < 10; i++)
            {
                int key = 21 + i;
                string text4 = DateFile.instance.gongFaDate[skillId][key];
                if (int.Parse(DateFile.instance.gongFaDate[skillId][key]) != 0)
                {
                    text3 = text.Split('|')[i];
                    switch (i)
                    {
                        case 0:
                        case 1:
                            text2 += DateFile.instance.SetColoer(20009, text3);
                            break;
                        case 2:
                            text2 += DateFile.instance.SetColoer(20008, text3);
                            break;
                        default:
                            text2 += DateFile.instance.SetColoer(20006, text3);
                            break;
                    }
                    text2 += DateFile.instance.gongFaDate[skillId][key];
                    text2 += " ";
                }
            }
            if (num2 > 0f)
            {
                str2 = str2 + WindowManage.instance.Dit() + "伤害部位及概率参数：\n  " + text2 + "\n";
            }
            str2 += "\n";
            str = (___baseGongFaMassage = str + str2);
            ___informationMassage.text = str;
        }
    }

    [HarmonyPatch(typeof(WindowManage), "ShowItemMassage")]
    public static class ShowMeMore_WindowManage_ShowItemMassage_Patch
    {
        private static void Postfix(WindowManage __instance, int itemId, ref string ___baseWeaponMassage, ref Text ___informationMassage)
        {
            if (Main.enabled && Main.settings.ShowItemMassage)
            {
                string str = ___baseWeaponMassage;
                if (int.Parse(DateFile.instance.GetItemDate(itemId, 1)) == 1 && Main.settings.ShowWeaponMassage)
                {
                    int num = int.Parse(DateFile.instance.GetItemDate(itemId, 602)) * 75 / 100;
                    float num2 = (float)BattleVaule.instance.GetAttackNeedTime(itemId) / 2.5f;
                    float num3 = (float)num / 10f;
                    int num4 = 0;
                    int num5 = Main.settings.ShowMoreAttackTimeAtEffect ? 250 : 150;
                    int num6 = int.Parse(DateFile.instance.GetItemDate(itemId, 14)) * (num5 - num4 * 20 - num4 * num4 * 5) / 100;
                    string itemDate = DateFile.instance.GetItemDate(itemId, 14);
                    str += DateFile.instance.SetColoer(10002, "\n【隐藏属性】\n");
                    str += string.Format("{0}{1}{2}\n", WindowManage.instance.Dit(), "基础伤害：", DateFile.instance.SetColoer(20003, num3.ToString() + "%"));
                    str += string.Format("{0}{1}{2}  ", WindowManage.instance.Dit(), (Main.settings.ShowMoreAttackTimeAtEffect ? "醉酒后" : "") + "连击概率：", DateFile.instance.SetColoer(20006, num6.ToString() + "%"));
                    num4 = 1;
                    num6 = int.Parse(DateFile.instance.GetItemDate(itemId, 14)) * (num5 - num4 * 20 - num4 * num4 * 5) / 100;
                    str += string.Format("({0}{1})\n", "再次连击概率", DateFile.instance.SetColoer(20006, num6 + "%"));
                    str += string.Format("{0}{1}{2}\n", WindowManage.instance.Dit(), "连招常数：", DateFile.instance.SetColoer(20001, itemDate));
                    str += string.Format("{0}{1}{2}\n", WindowManage.instance.Dit(), "攻击时间：", DateFile.instance.SetColoer(20006, num2.ToString()));
                    str = (___baseWeaponMassage = str + "\n");
                    ___informationMassage.text = str;
                }
            }
        }
    }


    [HarmonyPatch(typeof(WindowManage), "WindowSwitch")]
    public static class ShowMeMore_WindowManage_WindowSwitch_Patch
    {
        public static void Postfix(bool on, GameObject tips, ref Text ___itemMoneyText, ref Text ___itemLevelText, ref Text ___informationMassage, ref Text ___informationName, ref bool ___anTips, ref int ___tipsW, ref int ___tipsH)
        {
            if (!Main.enabled || !on || !Main.enabled || ActorMenu.instance == null || tips == null)
            {
                return;
            }
            int id = DateFile.instance.MianActorID();
            string text = ___informationMassage.text;
            string[] array = tips.name.Split(',');
            int num = (array.Length > 1) ? int.Parse(array[1]) : 0;
            if (num == 713 && Main.settings.ShowQiRate)
            {
                int[] array2 = new int[6]
                {
                20003,
                20008,
                20007,
                20006,
                20010,
                20004
                };
                int[] actorAllTrueQi = GetActorAllTrueQi(id, Most: false);
                int[] actorAllTrueQi2 = GetActorAllTrueQi(id, Most: true);
                text += DateFile.instance.SetColoer(10002, "\n【修习进度】\n");
                for (int i = 0; i < actorAllTrueQi.Length; i++)
                {
                    text += $"{WindowManage.instance.Dit()}{DateFile.instance.massageDate[2004][0].Split('|')[i]}: {DateFile.instance.SetColoer(array2[i], actorAllTrueQi[i].ToString())} / {DateFile.instance.SetColoer(array2[i], actorAllTrueQi2[i].ToString())}\n";
                }
                ___informationMassage.text = text;
            }
            else if (tips.tag == "PlaceMassage" && Main.settings.ShowHomeMassage)
            {
                int choosePartId = WorldMapSystem.instance.choosePartId;
                int choosePlaceId = WorldMapSystem.instance.choosePlaceId;
                if (choosePartId == int.Parse(DateFile.instance.GetGangDate(16, 3)) && choosePlaceId == int.Parse(DateFile.instance.GetGangDate(16, 4)))
                {
                    string str = "";
                    float[] taiwuIncome = GetTaiwuIncome();
                    int[] array3 = new int[3]
                    {
                    GetPlaceMark(3605),
                    GetPlaceMark(3602),
                    GetPlaceMark(2030)
                    };
                    str = str + "\n年均期望银钱收入：" + DateFile.instance.SetColoer(20008, $"{taiwuIncome[0]:F1}".ToString()) + "\n";
                    str = str + "年均期望威望收入：" + DateFile.instance.SetColoer(20007, $"{taiwuIncome[1]:F1}".ToString()) + "\n";
                    str += "\n当前已开通驿站地区平均影响率（不计低于50%的）：\n";
                    str = str + "文化正相关：" + DateFile.instance.SetColoer(20005, array3[0].ToString()) + "%\n";
                    str = str + "安定正相关：" + DateFile.instance.SetColoer(20004, array3[1].ToString()) + "%\n";
                    str = str + "安定负相关：" + DateFile.instance.SetColoer(20010, array3[2].ToString()) + "%\n";
                    Text informationMassage = WindowManage.instance.informationMassage;
                    informationMassage.text = informationMassage.text + str;
                }
            }
        }

        private static int[] GetActorAllTrueQi(int id, bool Most)
        {
            int[] array = new int[6];
            foreach (int key in DateFile.instance.actorGongFas[id].Keys)
            {
                Dictionary<int, string> dictionary = DateFile.instance.gongFaDate[key];
                if (int.Parse(dictionary[1]) <= 0)
                {
                    int gongFaLevel = DateFile.instance.GetGongFaLevel(id, key);
                    int num = (int.Parse(dictionary[4]) != 0) ? gongFaLevel : (Main.settings.AddHunYuan ? (gongFaLevel * 2) : 0);
                    int num2 = (int.Parse(dictionary[4]) == 0) ? (gongFaLevel * 2) : 0;
                    if (!Most)
                    {
                        array[0] += Convert.ToInt32(float.Parse(dictionary[701]) * (float)num2);
                    }
                    else
                    {
                        array[0] += Convert.ToInt32(float.Parse(dictionary[701]) * (float)((int.Parse(dictionary[4]) == 0) ? 200 : 0));
                    }
                    for (int i = 0; i < array.Length - 1; i++)
                    {
                        if (!Most)
                        {
                            array[i + 1] += Convert.ToInt32(float.Parse(dictionary[i + 701]) * (float)num);
                        }
                        else
                        {
                            array[i + 1] += Convert.ToInt32(float.Parse(dictionary[i + 701]) * (float)((int.Parse(dictionary[4]) != 0) ? 100 : (Main.settings.AddHunYuan ? 200 : 0)));
                        }
                    }
                }
            }
            return array;
        }

        public static float[] GetTaiwuIncome()
        {
            float[] array = new float[2];
            int num = int.Parse(DateFile.instance.GetGangDate(16, 3));
            int num2 = int.Parse(DateFile.instance.GetGangDate(16, 4));
            if (Main.settings.ShowDebug)
            {
                UnityModManager.Logger.Log("太吾村partid：" + num + "，placeId：" + num2);
            }
            int[] array2 = new int[3]
            {
            GetPlaceMark(3605),
            GetPlaceMark(3602),
            GetPlaceMark(2030)
            };
            if (GetPlaceMark(3605) == -1)
            {
                return array;
            }
            if (DateFile.instance.homeBuildingsDate.ContainsKey(num) && DateFile.instance.homeBuildingsDate[num].ContainsKey(num2))
            {
                foreach (int key3 in DateFile.instance.homeBuildingsDate[num][num2].Keys)
                {
                    bool flag = DateFile.instance.actorsWorkingDate.ContainsKey(num) && DateFile.instance.actorsWorkingDate[num].ContainsKey(num2) && DateFile.instance.actorsWorkingDate[num][num2].ContainsKey(key3);
                    int[] array3 = DateFile.instance.homeBuildingsDate[num][num2][key3];
                    if (array3[0] != 0 && DateFile.instance.actorsWorkingDate[num][num2].ContainsKey(key3) && flag)
                    {
                        int key = array3[0];
                        int num3 = array3[1];
                        string text = DateFile.instance.basehomePlaceDate[array3[0]][0];
                        int num4 = int.Parse(DateFile.instance.basehomePlaceDate[array3[0]][33]);
                        int num5 = int.Parse(DateFile.instance.basehomePlaceDate[array3[0]][96]);
                        int num6 = int.Parse(DateFile.instance.basehomePlaceDate[array3[0]][91]);
                        int buildingLevelPct = HomeSystem.instance.GetBuildingLevelPct(num, num2, key3);
                        float[] array4 = new float[4];
                        int[] array5 = new int[3]
                        {
                            buildingLevelPct,
                            buildingLevelPct * 30 / 100,
                            buildingLevelPct * 15 / 100
                        };
                        if (buildingLevelPct >= 100)
                        {
                            array4[2] = 0f;
                            array4[3] = 0f;
                            array4[1] = array5[2];
                            array4[0] = 100f - array4[1];
                        }
                        else
                        {
                            array4[3] = (float)(array5[0] * array5[1]) / 100f;
                            array4[2] = (float)(array5[0] * (100 - array5[1])) / 100f;
                            array4[1] = (float)((100 - array5[0]) * array5[2]) / 100f;
                            array4[0] = 100f - array4[1] - array4[2] - array4[3];
                        }
                        float num7 = (float)num6 / (float)buildingLevelPct;
                        float num8 = 12f / num7;
                        if (num5 != 0)
                        {
                            bool flag2 = int.Parse(DateFile.instance.basehomePlaceDate[key][92]) > 0;
                            bool flag3 = int.Parse(DateFile.instance.basehomePlaceDate[key][93]) > 0;
                            bool flag4 = int.Parse(DateFile.instance.basehomePlaceDate[key][93]) < 0;

                            Random valueRd = new Random();

                            for (int i = 0; i < 4; i++)
                            {
                                if (Main.settings.ShowDebug)
                                {
                                    UnityModManager.Logger.Log("现在在计算评价" + i + "，该评价出现概率为" + array4[i] + "%");
                                }

                                string[] array6 = DateFile.instance.homeShopEventTypDate[num5][i + 1].Split('|');
                                int key2 = int.Parse(array6[valueRd.Next(0, array6.Length)]);
                                string[] array7 = DateFile.instance.homeShopEventDate[key2][11].Split('|');
                                string[] array8 = DateFile.instance.homeShopEventDate[key2][12].Split('|');
                                string[] array9 = DateFile.instance.homeShopEventDate[key2][13].Split('|');
                                for (int j = 0; j < array7.Length; j++)
                                {
                                    int num9 = int.Parse(array7[j]);
                                    if (num9 == 1 && (flag2 || flag3 || flag4))
                                    {
                                        int num10 = array2[flag3 ? 1 : (flag4 ? 2 : 0)];
                                        int num11 = int.Parse(array8[j]) - 6;
                                        int num12 = Mathf.Max(1, int.Parse(array9[j]) * (80 + num3 * 8) / 100 * Mathf.Min(num10, 200) / 100);
                                        array[num11] += num8 * (float)num12 * array4[i] / 100f;
                                        if (Main.settings.ShowDebug)
                                        {
                                            UnityModManager.Logger.Log("现在正在计算编号为" + key3 + "的建筑" + text + "，其" + ((num11 == 0) ? "银钱" : "威望") + "单次收入为" + num12 + "，一年收入" + num8 + "次，平均地区资源影响率为" + num10);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return array;
        }

        public static int GetTaiwuCiTangPrestige()
        {
            int result = 0;
            int key = int.Parse(DateFile.instance.GetGangDate(16, 3));
            int key2 = int.Parse(DateFile.instance.GetGangDate(16, 4));
            foreach (int key3 in DateFile.instance.homeBuildingsDate[key][key2].Keys)
            {
                int[] array = DateFile.instance.homeBuildingsDate[key][key2][key3];
                if (array[0] == 1005)
                {
                    result = HomeSystem.instance.StudyGetPrestige(array[1]);
                    break;
                }
            }
            return result;
        }

        private static int GetPlaceMark(int buildingId)
        {
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = int.Parse(DateFile.instance.basehomePlaceDate[buildingId][92]);
            int num5 = int.Parse(DateFile.instance.basehomePlaceDate[buildingId][93]);
            int num6 = int.Parse(DateFile.instance.basehomePlaceDate[buildingId][94]);
            int num7 = int.Parse(DateFile.instance.basehomePlaceDate[buildingId][95]);
            if (num6 != 0)
            {
                return -1;
            }
            foreach (int key in DateFile.instance.gangDate.Keys)
            {
                int num8 = int.Parse(DateFile.instance.GetGangDate(key, 3));
                if (DateFile.instance.baseWorldDate[int.Parse(DateFile.instance.GetGangDate(key, 11))][num8][0] != 0)
                {
                    int placeId = int.Parse(DateFile.instance.GetGangDate(key, 4));
                    int num9 = int.Parse(DateFile.instance.GetNewMapDate(num8, placeId, 7));
                    int num10 = int.Parse(DateFile.instance.GetNewMapDate(num8, placeId, 8));
                    int num11 = (num6 > 0) ? int.Parse(DateFile.instance.GetNewMapDate(num8, placeId, num6)) : 0;
                    if ((num4 == 0 || num9 != 0) && (num5 == 0 || num10 != 0) && (num7 == 0 || num11 != 0))
                    {
                        int[] placeResource = DateFile.instance.GetPlaceResource(num8, placeId);
                        int num12 = 0;
                        int num13 = -1;
                        if (num4 != 0)
                        {
                            num12++;
                            num13 = ((num4 <= 0) ? ((placeResource[6] - 100) * 100 / num4) : ((placeResource[6] + 25) * 100 / num4));
                        }
                        int num14 = -1;
                        if (num5 != 0)
                        {
                            num12++;
                            num14 = ((num5 <= 0) ? ((placeResource[7] - 100) * 100 / num5) : ((placeResource[7] + 25) * 100 / num5));
                        }
                        int num15 = -1;
                        if (num7 != 0)
                        {
                            num12++;
                            num15 = ((num7 <= 0) ? ((placeResource[num6 - 1] - 100) * 100 / num7) : ((placeResource[num6 - 1] + 25) * 100 / num7));
                        }
                        int num16 = (num12 != 0) ? ((num13 + num14 + num15) / num12) : 100;
                        if (num16 >= 50)
                        {
                            num3++;
                            num2 += num16;
                        }
                    }
                }
            }
            return (num3 != 0) ? (num2 / num3) : (-1);
        }
    }

}
