using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony12;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using Random = System.Random;

namespace ShowQuQu
{
    public static class Main
    {
        [HarmonyPatch(typeof(QuquBattleSystem), "StartBattle")]
        public static class StartBattle_patch
        {
            public static void Prefix(ref int stateIndex)
            {
                if (enabled && settings.alwayswin)
                {
                    stateIndex = 1;
                }
            }
        }

        [HarmonyPatch(typeof(GetQuquWindow), "GetQuquButton")]
        public static class GetQuquButton_patch
        {
            public static bool Prefix(int index, ref bool ___startGetQuqu, ref bool ___startFirstTime, ref bool ___getQuquEnd)
            {
                if (!enabled)
                {
                    return true;
                }
                GetQuquButton(index, ref ___startGetQuqu, ref ___startFirstTime, ref ___getQuquEnd);
                return false;
            }
        }

        [HarmonyPatch(typeof(GetQuquWindow), "SetGetQuquWindow")]
        public static class SetGetQuquWindow_patch
        {
            public static void Postfix()
            {
                if (!enabled)
                {
                    return;
                }
                string str = "   □\t";
                string[] array = new string[21];
                for (int i = 0; i < GetQuquWindow.instance.placeImage.Length; i++)
                {
                    int key = GetQuquWindow.instance.cricketDate[i][1];
                    int key2 = GetQuquWindow.instance.cricketDate[i][2];
                    string str2 = "";
                    string str3 = "";
                    if (DateFile.instance.cricketDate.ContainsKey(key))
                    {
                        str2 = DateFile.instance.SetColoer(int.Parse(DateFile.instance.cricketDate[key][1]) + 20001, DateFile.instance.cricketDate[key][0]);
                    }
                    if (DateFile.instance.cricketDate.ContainsKey(key2))
                    {
                        str3 = DateFile.instance.SetColoer(int.Parse(DateFile.instance.cricketDate[key2][1]) + 20001, DateFile.instance.cricketDate[key2][0]);
                    }
                    array[i] = str2 + str3;
                }
                for (int j = 0; j <= 2; j++)
                {
                    str = str + array[j] + "□\t";
                }
                str += "   □\t\r\n";
                for (int k = 3; k <= 7; k++)
                {
                    str = str + array[k] + "□\t";
                }
                str += "\r\n";
                for (int l = 8; l <= 12; l++)
                {
                    str = str + array[l] + "□\t";
                }
                str += "\r\n";
                for (int m = 13; m <= 17; m++)
                {
                    str = str + array[m] + "□\t";
                }
                str += "\r\n   □\t";
                for (int n = 18; n <= 20; n++)
                {
                    str = str + array[n] + "□\t";
                }
                logger.Log(str);
            }
        }

        public static bool enabled;

        public static UnityModManager.ModEntry.ModLogger logger;

        public static Settings settings;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            HarmonyInstance obj = HarmonyInstance.Create(modEntry.Info.Id);
            settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            obj.PatchAll(Assembly.GetExecutingAssembly());
            logger = modEntry.Logger;
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

        public static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.nomiss = GUILayout.Toggle(settings.nomiss, "抓蛐蛐不会失手", (GUILayoutOption[])new GUILayoutOption[0]);
            settings.allQuQu = GUILayout.Toggle(settings.allQuQu, "抓到蛐蛐时，一网打尽本次奇遇中所有蛐蛐", (GUILayoutOption[])new GUILayoutOption[0]);
            settings.alwayswin = GUILayout.Toggle(settings.alwayswin, "斗蛐蛐必胜", (GUILayoutOption[])new GUILayoutOption[0]);
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        public static void GetAllQuqu()
        {
            List<int[]> list = new List<int[]>();
            Random valueRd = new Random();
            for (int i = 0; i < GetQuquWindow.instance.placeImage.Length; i++)
            {
                int key = i;
                int actorId = DateFile.instance.MianActorID();
                int num = DateFile.instance.MakeNewItem(int.Parse(DateFile.instance.cricketPlaceDate[GetQuquWindow.instance.cricketDate[key][0]][102]));
                int colorId = GetQuquWindow.instance.cricketDate[key][1];
                int partId = GetQuquWindow.instance.cricketDate[key][2];
                GetQuquWindow.instance.MakeQuqu(num, colorId, partId);
                int ququDate = GetQuquWindow.instance.GetQuquDate(num, 93);
                DateFile.instance.getQuquTrun += ququDate;
                DateFile.instance.AddActorScore(501, 100 + Mathf.Abs(ququDate) * 5);
                int num2 = int.Parse(DateFile.instance.GetItemDate(num, 8));
                if (valueRd.Next(0, 100) < num2 * 2)
                {
                    DateFile.instance.ChangeItemHp(actorId, num, -1);
                    GetQuquWindow.instance.QuquAddInjurys(num);
                    list.Add(new int[2]
                    {
                    num,
                    1
                    });
                    list.Add(new int[2]
                    {
                    96,
                    valueRd.Next(1, num2)
                    });
                }
                else if (valueRd.Next(0, 100) < 10)
                {
                    int num3 = DateFile.instance.MakRandQuqu((num2 - 1) * 3);
                    DateFile.instance.ChangeItemHp(actorId, num, -(int.Parse(DateFile.instance.GetItemDate(num, 901)) / 2));
                    DateFile.instance.ChangeItemHp(actorId, num3, -(int.Parse(DateFile.instance.GetItemDate(num3, 901)) / 2));
                    list.Add(new int[2]
                    {
                    num,
                    1
                    });
                    list.Add(new int[2]
                    {
                    num3,
                    1
                    });
                }
                else
                {
                    list.Add(new int[2]
                    {
                    num,
                    1
                    });
                }
            }
            DateFile.instance.GetItem(DateFile.instance.MianActorID(), list, newItem: false, bookObbs:0);
        }

        public static void GetQuquButton(int index, ref bool ___startGetQuqu, ref bool ___startFirstTime, ref bool ___getQuquEnd)
        {
            MethodInfo method = typeof(GetQuquWindow).GetMethod("GetQuqu", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);
            Random valueRd = new Random();
            if (!___startGetQuqu && !___startFirstTime)
            {
                return;
            }
            ___startGetQuqu = false;
            int num = 10 + GetQuquWindow.instance.cricketDate[index][6] - Mathf.Min(GetQuquWindow.instance.cricketDate[index][3] * 5, 40);
            Debug.Log((object)(index + ":" + GetQuquWindow.instance.cricketDate[index][6] + "|" + num));
            if (valueRd.Next(0, 100) < ((GetQuquWindow.instance.cricketDate[index][6] >= GetQuquWindow.instance.highLevel) ? num : (num / 2)) || settings.nomiss)
            {
                if (settings.allQuQu)
                {
                    GetAllQuqu();
                }
                else
                {
                    method.Invoke(GetQuquWindow.instance, new object[1]
                    {
                    index
                    });
                }
            }
            else if (GetQuquWindow.instance.cricketDate[index][6] >= GetQuquWindow.instance.highLevel)
            {
                string[] array = DateFile.instance.cricketPlaceDate[GetQuquWindow.instance.cricketDate[index][0]][101].Split('|');
                int itemId = int.Parse(array[valueRd.Next(0, array.Length)]);
                DateFile.instance.GetItem(DateFile.instance.MianActorID(), itemId, 1, newItem: true, bookObbs:0);
            }
            else
            {
                TipsWindow.instance.SetTips(22, new string[1]
                {
                ""
                }, 300);
            }
            ___getQuquEnd = true;
        }
    }

    public class Settings : UnityModManager.ModSettings
    {
        public bool allQuQu;

        public bool nomiss;

        public bool alwayswin;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            UnityModManager.ModSettings.Save(this, modEntry);
        }
    }

}
