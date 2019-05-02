using Harmony12;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;


namespace GongFaBook
{
    public static class Main
    {
        public class Settings : UnityModManager.ModSettings
        {
            public bool showAll = true;

            public override void Save(UnityModManager.ModEntry modEntry)
            {
                UnityModManager.ModSettings.Save(this, modEntry);
            }
        }

        public static bool enabled;

        public static UnityModManager.ModEntry.ModLogger Logger;

        public static Settings settings;

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

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.BeginVertical("Box", new GUILayoutOption[0]);
            settings.showAll = GUILayout.Toggle(settings.showAll, "是否显示所有不传之秘", (GUILayoutOption[])new GUILayoutOption[0]);
            GUILayout.EndVertical();
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

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
    }

    [HarmonyPatch(typeof(SetGongFaTree), "SetGongFaIcon")]
    public static class SetGongFaTree_SetGongFaIcon_Patch
    {
        private static bool Prefix(SetGongFaTree __instance, int typ, int gangId, int gangValue)
        {
            //IL_04b0: Unknown result type (might be due to invalid IL or missing references)
            //IL_04c6: Unknown result type (might be due to invalid IL or missing references)
            if (!Main.enabled)
            {
                return true;
            }
            int num = DateFile.instance.MianActorID();
            __instance.gongFaImage.sprite = GetSprites.instance.gongFaImage[typ];
            __instance.gongFaNameText.text = DateFile.instance.baseSkillDate[101 + typ][0];
            int num2 = (gangValue >= 0) ? Mathf.Max(gangValue / 100 - 2, 0) : (-1);
            List<int> list = new List<int>();
            List<int> list2 = new List<int>(DateFile.instance.gongFaDate.Keys);
            for (int i = 0; i < __instance.gongFaLevelText.Length; i++)
            {
                __instance.gongFaLevelText[i].text = DateFile.instance.massageDate[7003][3].Split('|')[i];
            }
            for (int j = 0; j < list2.Count; j++)
            {
                int num3 = list2[j];
                if (int.Parse(DateFile.instance.gongFaDate[num3][1]) == typ && int.Parse(DateFile.instance.gongFaDate[num3][3]) == gangId)
                {
                    list.Add(num3);
                }
            }
            for (int k = 0; k < __instance.gongFaIcons.Length; k++)
            {
                if (k < list.Count)
                {
                    int num4 = list[k];
                    __instance.gongFaImages[k].SetActive(true);
                    __instance.gongFaImages[k].name = "GongFaImage," + num4;
                    if (DateFile.instance.actorGongFas.ContainsKey(num) && DateFile.instance.actorGongFas[num].ContainsKey(num4))
                    {
                        __instance.gongFaIcons[k].sprite = GetSprites.instance.gongFaSprites[int.Parse(DateFile.instance.gongFaDate[num4][98])];
                        __instance.gongFaNames[k].text = DateFile.instance.SetColoer((num2 < k) ? 20002 : 10003, DateFile.instance.gongFaDate[num4][0]);
                        if (DateFile.instance.GetGongFaLevel(num, num4) >= 100 && DateFile.instance.GetGongFaFLevel(num, num4) >= 10)
                        {
                            __instance.gongFaStudyMassageText[k].text = DateFile.instance.SetColoer(20009, DateFile.instance.massageDate[7007][5].Split('|')[3]);
                        }
                        else
                        {
                            __instance.gongFaStudyMassageText[k].text = DateFile.instance.SetColoer(20008, DateFile.instance.massageDate[7007][5].Split('|')[2]);
                        }
                        continue;
                    }
                    if (num2 < k && int.Parse(DateFile.instance.gongFaDate[num4][16]) == 1 && !Main.settings.showAll)
                    {
                        __instance.gongFaIcons[k].GetComponent<PointerEnter>().enabled = false;
                        __instance.gongFaIcons[k].sprite = GetSprites.instance.gongFaSprites[int.Parse(DateFile.instance.gongFaDate[num4][98])];
                        __instance.gongFaNames[k].text = DateFile.instance.SetColoer(10004, DateFile.instance.massageDate[7007][5].Split('|')[0]);
                        __instance.gongFaStudyMassageText[k].text = DateFile.instance.SetColoer(10004, DateFile.instance.massageDate[7007][5].Split('|')[1]);
                    }
                    else
                    {
                        __instance.gongFaIcons[k].sprite = GetSprites.instance.gongFaSprites[int.Parse(DateFile.instance.gongFaDate[num4][98])];
                        __instance.gongFaNames[k].text = DateFile.instance.SetColoer((num2 < k) ? 20002 : 10003, DateFile.instance.gongFaDate[num4][0]);
                        __instance.gongFaStudyMassageText[k].text = "";
                    }
                    __instance.gongFaIcons[k].color = (num2 < k) ? new Color(0f, 0f, 0f) : new Color(1f, 1f, 1f);
                }
                else
                {
                    __instance.gongFaImages[k].SetActive(false);
                }
            }
            return false;
        }

        private static void Postfix(int typ, int gangId, int gangValue)
        {
        }
    }


    [HarmonyPatch(typeof(WindowManage), "ShowItemMassage")]
    public static class WindowManage_ShowBookMassage_Patch
    {
        private static void Postfix(WindowManage __instance, int itemId, int itemTyp, ref string ___baseWeaponMassage, ref Text ___informationMassage, ref Text ___informationName)
        {
            if (!Main.enabled)
            {
                return;
            }
            string text = ___baseWeaponMassage;
            if (int.Parse(DateFile.instance.GetItemDate(itemId, 31)).Equals(17))
            {
                int.Parse(DateFile.instance.GetItemDate(itemId, 999));
                bool num = int.Parse(DateFile.instance.GetItemDate(itemId, 35)).Equals(1);
                string itemDate = DateFile.instance.GetItemDate(itemId, 99);
                text = text.Replace(itemDate, "");
                if (num)
                {
                    string text2 = ___informationName.text;
                    int startIndex = text2.IndexOf("》");
                    ___informationName.text = text2.Insert(startIndex, "·手抄");
                    text = text.Insert(0, DateFile.instance.SetColoer(20010, itemDate));
                }
                else
                {
                    text = text.Insert(0, DateFile.instance.SetColoer(20004, itemDate));
                }
                int key = int.Parse(DateFile.instance.GetItemDate(itemId, 32, otherMassage: false));
                if (DateFile.instance.gongFaDate.ContainsKey(key))
                {
                    string text3 = "";
                    int key2 = int.Parse(DateFile.instance.gongFaDate[key][103]);
                    if (DateFile.instance.gongFaFPowerDate.ContainsKey(key2))
                    {
                        text3 += DateFile.instance.SetColoer(10002, "所载心法\n");
                        string arg = DateFile.instance.gongFaFPowerDate[key2][99];
                        text3 += DateFile.instance.SetColoer(20004, $"·正练:{arg} \n");
                    }
                    int key3 = int.Parse(DateFile.instance.gongFaDate[key][104]);
                    if (DateFile.instance.gongFaFPowerDate.ContainsKey(key3))
                    {
                        string arg2 = DateFile.instance.gongFaFPowerDate[key3][99];
                        string text4 = DateFile.instance.gongFaFPowerDate[key3][98];
                        text3 += DateFile.instance.SetColoer(20010, $"·逆练:{arg2} \n");
                        if (text4.Length > 0)
                        {
                            text3 += DateFile.instance.SetColoer(20010, $"·逆练:{text4} \n");
                        }
                    }
                    int num2 = text.IndexOf("所载心法");
                    if (num2 > -1)
                    {
                        text = text.Substring(0, num2 - 18) + text3;
                    }
                }
            }
            ___baseWeaponMassage = text;
            ___informationMassage.text = text;
        }
    }

    [HarmonyPatch(typeof(WindowManage), "ShowGongFaMassage")]
    public static class WindowManage_ShowGongFaMassage_Patch
    {
        private const int CORRECTED_VALUE = 100;

        private const int GONGFATYPE_PRACTICETYPE_CHECKNUM = 5000;

        private const int NORMAL_TYPE_ID = 103;

        private const int UNNORMAL_TYPE_ID = 104;

        private static void Postfix(WindowManage __instance, int skillId, int skillTyp, int levelTyp, int actorId, Toggle toggle, ref Text ___informationMassage, ref string ___baseGongFaMassage)
        {
            if (Main.enabled && skillTyp != 0 && skillTyp == 1)
            {
                int.Parse(DateFile.instance.gongFaDate[skillId][103]);//Get_activeInHierarchy
                int actorId2 = (actorId != -1) ? actorId : (!ActorMenu.instance.actorMenu.activeInHierarchy ? DateFile.instance.MianActorID() : ActorMenu.instance.acotrId);
                int num = (levelTyp != -1 && levelTyp != 0) ? 10 : ((skillId != 0) ? DateFile.instance.GetGongFaFLevel(actorId2, skillId) : 0);
                string str = ___baseGongFaMassage;
                string text = "";
                string str2 = "";
                int gongFaFTyp = DateFile.instance.GetGongFaFTyp(actorId2, skillId);
                int num2 = int.Parse(DateFile.instance.gongFaDate[skillId][103 + ((gongFaFTyp != 0) ? 1 : 0)]);
                int key = num2 + ((gongFaFTyp == 0) ? 1 : (-1)) * 5000;
                if (num2 > 0)
                {
                    int num3 = (gongFaFTyp != 0) ? 1 : 0;
                    str2 = string.Format("{0}{1}{2}\n\n", __instance.SetMassageTitle(8007, 3, 11 + num3, (num3 != 0) ? 20010 : 20005), __instance.Dit(), DateFile.instance.SetColoer(20002, string.Format("{0}{1}{2}{3}", DateFile.instance.gongFaFPowerDate[num2][99], (!(DateFile.instance.gongFaFPowerDate[num2][98] != "")) ? "" : DateFile.instance.massageDate[5001][4], DateFile.instance.gongFaFPowerDate[num2][98], DateFile.instance.massageDate[5001][5])));
                    str2 = DateFile.instance.SetColoer(20004, string.Format("  如果{0}练\n", (num3 == 0) ? "正" : "逆")) + str2;
                    text = string.Format("{0}{1}{2}\n\n", __instance.SetMassageTitle(8007, 3, 11 + Math.Abs(num3 - 1), (num3 != 0) ? 20005 : 20010), __instance.Dit(), DateFile.instance.SetColoer(20002, string.Format("{0}{1}\n{2}", new object[3]
                    {
                    DateFile.instance.gongFaFPowerDate[key][99],
                    (!(DateFile.instance.gongFaFPowerDate[key][98] != "") || Math.Abs(num3 - 1) == 0) ? "" : DateFile.instance.massageDate[5001][4],
                    DateFile.instance.gongFaFPowerDate[key][98]
                    })));
                    text = DateFile.instance.SetColoer(20004, string.Format("  如果{0}练\n", (num3 == 0) ? "逆" : "正")) + text;
                }
                str = (___baseGongFaMassage = ((num < 5 || gongFaFTyp == 2) ? (str + str2 + text) : (str + text)));
                ___informationMassage.text = str;
            }
        }
    }
}
