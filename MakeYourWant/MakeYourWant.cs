using Harmony12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;

namespace MakeYourWant
{
    public class Settings : UnityModManager.ModSettings
    {
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            UnityModManager.ModSettings.Save<Settings>(this, modEntry);
        }
        public bool IMBA = false;
        public bool global = false;
        public bool needNoTime = false;
        public bool freeTyp = false;
        public bool changeRate = false;
        public int rate = 0;
        public bool changePoison = false;
        public int[] poison = new int[6] { 0, 0, 0, 0, 0, 0 };
        public bool unbreakable = false;
        public bool lastWish = false;
        //武器
        public bool SetWeapon = false;
        public bool WeaponMoreDuration = false;
        public bool AttackTypExtend = false;
        public int[] AttackTyp = new int[6] { 15, 15, 15, 15, 15, 15 };//式
        public int WeaponPower0 = 3;//词条
        public int WeaponPower1 = 0;
        public int WeaponPower2 = 0;
        public int WeaponPower3 = 5;
        public bool WeaponPowerMustBe = false;//必出
        public int WeaponEngraving = 0;//功法发挥
        public bool WeaponEngravingMustBe = false;
        //防具
        public bool SetArmor = false;
        public int ArmorPower = 0;
        public bool ArmorPowerMustBe = false;
        public int ArmorAttr = 0;
        public bool ArmorAttrMustBe = false;
        //饰品
        public bool SetJewelry = false;
        public bool JewelryPowerMustBe = false;
        public int JewelryAttr0 = 0;
        public int JewelryAttr1 = 0;
        public int JewelryAttr2 = 0;
        public bool JewelryAttrMustBe = false;
        //食物
        public bool SetFood = false;
        public int FoodAttr0 = 0;
        public int FoodAttr1 = 0;
        public int FoodAttr2 = 0;
        public bool FoodAttrMustBe = false;
        //药品
        public bool setDrug = false;

    }

    public static class Main
    {
        public static bool enabled;
        public static Settings settings;
        public static UnityModManager.ModEntry.ModLogger Logger;
        static readonly string[] AttackTypName = { "掷", "弹", "御", "劈", "刺", "撩",
            "崩", "点", "拿", "音", "缠", "咒", "机", "药", "毒", "？" };
        static readonly string[] WeaponPowerName0 = { "破", "辟", "杀", "其他" };
        static readonly string[] WeaponPowerName1 = { "掌", "剑", "刀", "毒", "长鞭", "软兵", "暗器",
            "奇门", "魔音", "金", "木", "玉" };
        static readonly string[] WeaponPowerName2 = { "武器杀", "材质杀" };
        static readonly string[] WeaponPowerName3 = { "厚重", "轻盈", "锋锐", "钝拙", "贵重", "随机" };
        static readonly string[] WeaponEngravingName = { "随机", "力道", "精妙", "迅疾" };
        static readonly string[] ArmorPowerName = { "随机", "不变", "厚重", "轻盈", "藏锋", "百折", "贵重" };
        static readonly string[] ArmorAttrName = { "随机", "膂力", "体质", "灵敏", "根骨", "悟性", "定力" };
        static readonly string[] AttrName0 = { "随机", "技艺", "武学" };
        static readonly string[] AttrName1 = { "音律", "弈棋", "诗书", "绘画",
            "术数", "品鉴", "锻造", "制木", "医术", "毒术", "织锦", "巧匠", "道法", "佛学", "厨艺", "杂学" };
        static readonly string[] AttrName2 = { "内功", "身法", "绝技", "拳掌",
            "指法", "腿法", "暗器", "剑法", "刀法", "长兵", "奇门", "软兵", "御射", "乐器" };
        static readonly string[] PoisonName = { "烈毒", "郁毒", "寒毒", "赤毒", "腐毒", "幻毒" };


        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            Logger = modEntry.Logger;
            settings = Settings.Load<Settings>(modEntry);
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            return true;
        }

        public static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;
            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<color=#F28234FF>跨月制作的装备在投入材料时决定</color>", new GUILayoutOption[0]);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            settings.SetWeapon = GUILayout.Toggle(settings.SetWeapon, "定制武器", new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            if (settings.SetWeapon)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("式排列", GUILayout.Width(100));
                string[] s = new string[6];
                for (int i = 0; i < 6; i = i + 1)
                {
                    s[i] = AttackTypName[settings.AttackTyp[i]];
                }
                GUILayout.Label(string.Concat("<size=15>", string.Join("|", s), "</size>"), GUILayout.Width(150));
                bool flag = GUILayout.Button((settings.AttackTypExtend ? "收起" : "展开"), GUILayout.Width(100));
                GUILayout.Label("<color=#E4504DFF>不会产生武器没有的式</color>", GUILayout.Width(210));
                //GUILayout.FlexibleSpace();
                settings.WeaponMoreDuration = GUILayout.Toggle(settings.WeaponMoreDuration, "较高耐久", new GUILayoutOption[0]);
                if (flag)
                {
                    settings.AttackTypExtend = !settings.AttackTypExtend;
                }
                GUILayout.EndHorizontal();
                if (settings.AttackTypExtend)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        GUILayout.BeginHorizontal();
                        settings.AttackTyp[i] = GUILayout.SelectionGrid(settings.AttackTyp[i], AttackTypName, 16);
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label("词条", GUILayout.Width(100));
                settings.WeaponPower0 = GUILayout.SelectionGrid(settings.WeaponPower0, WeaponPowerName0, 4);
                //GUILayout.FlexibleSpace();
                settings.WeaponPowerMustBe = GUILayout.Toggle(settings.WeaponPowerMustBe, "必出", new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (settings.WeaponPower0 == 2)
                {
                    settings.WeaponPower2 = GUILayout.SelectionGrid(settings.WeaponPower2, WeaponPowerName2, 2);
                }
                else
                {
                    if (settings.WeaponPower0 == 3)
                    {
                        settings.WeaponPower3 = GUILayout.SelectionGrid(settings.WeaponPower3, WeaponPowerName3, 6);
                    }
                    else
                    {
                        settings.WeaponPower1 = GUILayout.SelectionGrid(settings.WeaponPower1, WeaponPowerName1, 12);
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("功法发挥", GUILayout.Width(100));
                settings.WeaponEngraving = GUILayout.SelectionGrid(settings.WeaponEngraving, WeaponEngravingName, 4);
                //GUILayout.FlexibleSpace();
                settings.WeaponEngravingMustBe = GUILayout.Toggle(settings.WeaponEngravingMustBe, "必出", new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal("Box");
            settings.SetArmor = GUILayout.Toggle(settings.SetArmor, "定制防具", new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            if (settings.SetArmor)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("词条", GUILayout.Width(100));
                settings.ArmorPower = GUILayout.SelectionGrid(settings.ArmorPower, ArmorPowerName, 7);
                //GUILayout.FlexibleSpace();
                settings.ArmorPowerMustBe = GUILayout.Toggle(settings.ArmorPowerMustBe, "必出", new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("额外属性", GUILayout.Width(100));
                settings.ArmorAttr = GUILayout.SelectionGrid(settings.ArmorAttr, ArmorAttrName, 7);
                //GUILayout.FlexibleSpace();
                settings.ArmorAttrMustBe = GUILayout.Toggle(settings.ArmorAttrMustBe, "必出", new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal("Box");
            settings.SetJewelry = GUILayout.Toggle(settings.SetJewelry, "定制饰品", new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            if (settings.SetJewelry)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("词条", GUILayout.Width(100));
                //GUILayout.FlexibleSpace();
                settings.JewelryPowerMustBe = GUILayout.Toggle(settings.JewelryPowerMustBe, "必出贵重", new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("额外属性", GUILayout.Width(100));
                settings.JewelryAttr0 = GUILayout.SelectionGrid(settings.JewelryAttr0, AttrName0, 3);
                //GUILayout.FlexibleSpace();
                settings.JewelryAttrMustBe = GUILayout.Toggle(settings.JewelryAttrMustBe, "必出", new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                if (settings.JewelryAttr0 != 0)
                {
                    GUILayout.BeginHorizontal();
                    if (settings.JewelryAttr0 == 1)
                        settings.JewelryAttr1 = GUILayout.SelectionGrid(settings.JewelryAttr1, AttrName1, 8);
                    else
                        settings.JewelryAttr2 = GUILayout.SelectionGrid(settings.JewelryAttr2, AttrName2, 7);
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.BeginHorizontal("Box");
            settings.SetFood = GUILayout.Toggle(settings.SetFood, "定制食物", new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            if (settings.SetFood)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("额外属性", GUILayout.Width(100));
                settings.FoodAttr0 = GUILayout.SelectionGrid(settings.FoodAttr0, AttrName0, 3);
                settings.FoodAttrMustBe = GUILayout.Toggle(settings.FoodAttrMustBe, "必出", new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                if (settings.FoodAttr0 != 0)
                {
                    GUILayout.BeginHorizontal();
                    if (settings.FoodAttr0 == 1)
                        settings.FoodAttr1 = GUILayout.SelectionGrid(settings.FoodAttr1, AttrName1, 8);
                    else
                        settings.FoodAttr2 = GUILayout.SelectionGrid(settings.FoodAttr2, AttrName2, 7);
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.BeginHorizontal("Box");
            bool setDrug = GUILayout.Toggle(settings.setDrug, "定制药品", new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            if (settings.setDrug != setDrug)
            {
                settings.setDrug = setDrug;
                DrugSwitch();
            }
            if (setDrug)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("必定产出品阶更高的药品", new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal("Box");
            settings.IMBA = GUILayout.Toggle(settings.IMBA, "IMBA模式（破坏游戏平衡，慎用）", new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            if (settings.IMBA)
            {
                GUILayout.BeginHorizontal();
                settings.global = GUILayout.Toggle(settings.global, "影响所有新产生的装备，包括商店出售的与NPC获得的（如果版本变化有小概率坏档，慎用）", new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                settings.needNoTime = GUILayout.Toggle(settings.needNoTime, "所有装备制造立刻完成", new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                settings.freeTyp = GUILayout.Toggle(settings.freeTyp, "式的定制不受限制", new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                settings.changeRate = GUILayout.Toggle(settings.changeRate, "改变内伤比例", GUILayout.Width(120));
                string rate = GUILayout.TextField(settings.rate.ToString(), GUILayout.Width(40));
                if (int.TryParse(rate, out int temp) && temp >= 0 && temp <= 100)
                {
                    settings.rate = temp;
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                settings.changePoison = GUILayout.Toggle(settings.changePoison, "含有毒素：", GUILayout.Width(100));
                for (int i = 0; i < 6; i++)
                {
                    GUILayout.Label("  " + PoisonName[i], GUILayout.Width(40));
                    string poison = GUILayout.TextField(settings.poison[i].ToString(), new GUILayoutOption[0]);
                    if (int.TryParse(poison, out int temp1) && temp1 >= 0)
                    {
                        settings.poison[i] = temp1;
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                settings.unbreakable = GUILayout.Toggle(settings.unbreakable, "神兵（耐久不会因任何原因减少）", new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                settings.lastWish = GUILayout.Toggle(settings.lastWish, "遗愿加成(必得双倍额外属性+必得两种副产品)", new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
            }
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        public static string[] drugIds = new string[25];
        public static void DrugSwitch()
        {
            if (DateFile.instance == null) return;
            var makeItemDate = DateFile.instance.makeItemDate;
            if (makeItemDate == null || makeItemDate.Count == 0) return;
            if (!makeItemDate.ContainsKey(901))
            {
                Main.Logger.Log("makeItemDate没有901项");
                return;
            }
            var makeDrugDate = makeItemDate[901];
            bool enabled = settings.setDrug;
            if (enabled)
            {
                drugIds[0] = "";
                for (int i = 1; i <= 24; i++)
                {
                    string oldStr = makeDrugDate[i];
                    List<string> ids = new List<string>(oldStr.Split('|'));
                    drugIds[i] = oldStr;
                    if (ids.Count <= 1)
                    {
                        Main.Logger.Log(GetItemName(oldStr));
                    }
                    else
                    {
                        string newStr = KeyOfMaxValue(ids, GetGrade);
                        makeDrugDate[i] = newStr;
                        string log = string.Join("|", ids.Select(id => GetItemName(id)).ToArray());
                        Main.Logger.Log($"{log} => {GetItemName(newStr)}");
                    }
                }
            }
            else
            {
                if (drugIds[0] == null) return;
                for (int i = 1; i <= 24; i++)
                {
                    string tmp = drugIds[i];
                    drugIds[i] = makeDrugDate[i];
                    makeDrugDate[i] = tmp;
                    string log = string.Join("|", drugIds.Select(id => GetItemName(id)).ToArray());
                    Main.Logger.Log($"{GetItemName(drugIds[i])} => {log}");
                }
            }
        }
        static T KeyOfMaxValue<T>(IEnumerable<T> enumerable, Func<T, int> Key2Value)
        {
            T result = enumerable.First();
            int maxValue = int.MinValue;
            foreach (T key in enumerable)
            {
                int value = Key2Value(key);
                if (value > maxValue)
                {
                    result = key;
                    maxValue = value;
                }
            }
            return result;
        }
        static int GetGrade(string id) => int.Parse(DateFile.instance.GetItemDate(int.Parse(id), 8, false));
        public static string GetItemName(string itemId, bool showTyp = false)
        {
            return int.TryParse(itemId, out int id) ? GetItemName(id, showTyp) : $"I2S Failed: itemId = {itemId}";
        }
        public static string GetItemName(int itemId, bool showTyp = false)
        {
            string name = DateFile.instance.GetItemDate(itemId, 0, false);
            if (int.TryParse(DateFile.instance.GetItemDate(itemId, 8, false), out int grade))
            {
                name = DateFile.instance.SetColoer(20001 + grade, name);
            }
            return name;
        }

    }

    [HarmonyPatch(typeof(MakeSystem), "SetMakeItem")]
    public static class MakeYourWant_SetMakeItem_Patch
    {
        static void Prefix(MakeSystem __instance)
        {
            if (!Main.enabled) return;
            bool lastWish = Main.settings.IMBA && Main.settings.lastWish;
            if (lastWish)
            {
                DateFile.instance.makePower = true;
            }
            if (Main.settings.setDrug && Main.drugIds[0] == null) Main.DrugSwitch();
        }
    }
    /*[HarmonyPatch(typeof(DateFile), "SetHomeBuildingValue")]
    public static class MakeYourWant_SetHomeBuildingValue_Patch
    {
        static void Prefix(DateFile __instance, ref int dateIndex,ref int dateValue)
        {
            if (!Main.enabled) return;
            bool needNoTime = Main.settings.IMBA && Main.settings.needNoTime;
            if (!needNoTime) return;
            if (dateIndex == 4) dateValue = 0;
        }
    }*/
    [HarmonyPatch(typeof(DateFile), "GetItemDate")]
    public static class MakeYourWant_GetItemDate_Patch
    {
        static bool Prefix(DateFile __instance, ref string __result, ref int index)
        {
            if (!Main.enabled) return true;
            bool needNoTime = Main.settings.IMBA && Main.settings.needNoTime;
            if (needNoTime && index == 47)
            {
                __result = "0";
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(DateFile), "MakeNewItem")]
    public static class MakeYourWant_MakeNewItem_Patch
    {
        static bool Prefix(DateFile __instance, ref int __result, ref int id, ref int temporaryId,
            ref int bookObbs, ref int buffObbs, ref int sBuffObbs)
        {
            if (!Main.enabled) return true;//mod未开启
            var presetItem = __instance.presetitemDate[id];
            int itemTyp = int.Parse(presetItem[5]);
            int equipTyp = int.Parse(presetItem[1]);//1武器，2防具，3饰品
            bool isFood = itemTyp == 34 || itemTyp == 35;
            if (isFood)//食物
            {
                if (int.Parse(presetItem[6]) == 1) return true;//可堆叠，是野果
                if (!Main.settings.SetFood) return true;
            }
            else
            {
                if (presetItem[4] != "4") return true;//排除非装备
                if (equipTyp <= 0 || equipTyp >= 4) return true;//装备类型不符
                if ((equipTyp == 1 && !Main.settings.SetWeapon) || (equipTyp == 2 && !Main.settings.SetArmor) ||
                    (equipTyp == 3 && !Main.settings.SetJewelry)) return true;//装备类型对应选项未开启
                if (itemTyp == 42) return true;//排除神书
                if (itemTyp == 36) return true;//排除神兵
                if (itemTyp == 21 || int.Parse(presetItem[31]) > 0) return true;//排除书籍
            }
            if (!Main.settings.IMBA || !Main.settings.global)//仅对玩家制造的物品生效
            {
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(false);
                if (!st.GetFrames().Select(f => f.GetMethod().Name).Any(n => n.Contains("SetMakeItem")))
                    return true;
            }
            Dictionary<int, string> value = new Dictionary<int, string>();
            int newItemId;
            if (temporaryId < 0)
            {
                newItemId = temporaryId;
                __instance.itemsDate.Remove(temporaryId);
                __instance.itemsChangeDate.Remove(temporaryId);
            }
            else
            {
                __instance.newItemId++;
                newItemId = __instance.newItemId;
            }
            __instance.itemsDate.Add(newItemId, value);
            var newItem = __instance.itemsDate[newItemId];
            newItem[999] = id.ToString();//物品类型id
            bool isFixed = int.Parse(presetItem[902]) <= 0;
            int duration = Mathf.Abs(int.Parse(presetItem[902]));
            //食物
            if (isFood)
            {
                //耐久
                int minimum = 50;
                duration = isFixed ? duration : 1 + duration * UnityEngine.Random.Range(minimum, 101) / 100;
                newItem[902] = duration.ToString();//最大耐久
                newItem[901] = duration.ToString();//当前耐久
                if (Main.settings.FoodAttrMustBe || UnityEngine.Random.Range(0, 100) < buffObbs)
                {
                    int attrTyp;
                    if (Main.settings.FoodAttr0 == 0) attrTyp = (UnityEngine.Random.Range(0, 100) < 50) ?
                            UnityEngine.Random.Range(0, 16) + 50501 : UnityEngine.Random.Range(0, 14) + 50601;
                    else
                    {
                        if (Main.settings.FoodAttr0 == 1) attrTyp = Main.settings.FoodAttr1 + 50501;
                        else attrTyp = Main.settings.FoodAttr2 + 50601;
                    }
                    newItem[attrTyp] = (Mathf.Max(int.Parse(presetItem[8]) / 2, 1) * 20).ToString();
                }
                __result = newItemId;
                return false;
            }
            else
            {
                if (Main.settings.IMBA && Main.settings.changePoison)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        newItem[i + 71] = Main.settings.poison[i].ToString();
                    }
                }
                if (Main.settings.IMBA && Main.settings.unbreakable)
                {
                    isFixed = true;
                    duration = 1;
                    newItem[5] = "36";
                }
                //武器
                if (equipTyp == 1)
                {
                    //耐久
                    if (!isFixed)
                    {
                        int minimum = Main.settings.WeaponMoreDuration ? 80 : 50;
                        duration = 1 + duration * UnityEngine.Random.Range(minimum, 101) / 100;
                    }
                    newItem[902] = duration.ToString();//最大耐久
                    newItem[901] = duration.ToString();//当前耐久
                    //式
                    bool freeTyp = Main.settings.IMBA && Main.settings.freeTyp;
                    if (freeTyp || int.Parse(presetItem[606]) > 0)
                    {
                        string attackTypText = "";
                        string[] newTyps = new string[6];
                        for (int i = 0; i < 6; i++)
                        {
                            newTyps[i] = Main.settings.AttackTyp[i].ToString();
                        }
                        if (!freeTyp)
                        {
                            List<string> presetTyps = new List<string>(presetItem[7].Split('|'));
                            for (int i = 0; i < 6; i++)
                            {
                                string typ = newTyps[i];
                                if (presetTyps.Contains(typ))
                                    presetTyps.Remove(typ);
                                else newTyps[i] = "";
                            }
                            int count = presetTyps.Count;
                            for (int i = 0; i < 6; i++)
                            {
                                if (newTyps[i] == "")
                                {
                                    string typ = presetTyps[UnityEngine.Random.Range(0, presetTyps.Count)];
                                    newTyps[i] = typ;
                                    presetTyps.Remove(typ);
                                }
                            }
                        }
                        attackTypText = String.Join("|", newTyps);
                        newItem[7] = attackTypText;
                    }
                    //内伤比例
                    if (Main.settings.IMBA && Main.settings.changeRate)
                    {
                        newItem[10] = Main.settings.rate.ToString();
                    }
                    //功法发挥
                    if (Main.settings.WeaponEngravingMustBe || UnityEngine.Random.Range(0, 100) < buffObbs)
                    {
                        int carving = Main.settings.WeaponEngraving == 0 ? (UnityEngine.Random.Range(0, 3) + 1) : Main.settings.WeaponEngraving;
                        if (buffObbs == 999)
                        {
                            carving *= 100;
                        }
                        newItem[505] = carving.ToString();
                    }
                    //词条
                    int power = int.Parse(presetItem[504]);
                    if (Main.settings.WeaponPowerMustBe || (power != 0 && UnityEngine.Random.Range(0, 100) < sBuffObbs))
                    {
                        if (Main.settings.WeaponPower0 == 3)//其他
                        {
                            if (Main.settings.WeaponPower3 == 5)//随机
                            {
                                List<int> powerList = new List<int>
                                    {
                                        2,
                                        3,
                                        4,
                                        5,
                                        6,
                                        7,
                                        8,
                                        9,
                                        10,
                                        11,
                                        12,
                                        13,
                                        101,
                                        102,
                                        103,
                                        104,
                                        105,
                                        106,
                                        107,
                                        108,
                                        109,
                                        110,
                                        111,
                                        112,
                                        401,
                                        402,
                                        403,
                                        404,
                                        407
                                    };
                                if (power < 0)
                                {
                                    powerList.Add(Mathf.Abs(power));//武器杀
                                }
                                int material = int.Parse(presetItem[506]);
                                if (material != 0)
                                {
                                    powerList.Add(209 + material);//材质杀
                                }
                                power = powerList[UnityEngine.Random.Range(0, powerList.Count)];
                            }
                            else power = Main.settings.WeaponPower3 == 4 ? 407 : 401 + Main.settings.WeaponPower3;
                        }
                        else
                        {
                            if (Main.settings.WeaponPower0 == 2)//杀
                            {
                                if (Main.settings.WeaponPower2 == 1)//材质杀
                                {
                                    int material = int.Parse(presetItem[506]);
                                    if (material == 0)//没有布杀
                                    {
                                        __result = newItemId;
                                        return false;
                                    }
                                    else power = 209 + material;
                                }
                                else power = Mathf.Abs(power);//武器杀
                            }
                            else power = (Main.settings.WeaponPower0 == 0 ? 2 : 101) + Main.settings.WeaponPower1;
                        }
                        //__instance.ChangeSPower(id, num, list4[UnityEngine.Random.Range(0, list4.Count)]);
                        var method = typeof(DateFile).GetMethod("ChangeNewItemSPower",
                            BindingFlags.NonPublic | BindingFlags.Instance);
                        method.Invoke(__instance, new object[] { id, newItemId, power });
                    }
                    __result = newItemId;
                    return false;
                }
                //防具
                if (equipTyp == 2)
                {
                    //耐久
                    if (!isFixed)
                    {
                        int minimum = 50;
                        duration = 1 + duration * UnityEngine.Random.Range(minimum, 101) / 100;
                    }
                    newItem[902] = duration.ToString();//最大耐久
                    newItem[901] = duration.ToString();//当前耐久
                    if (Main.settings.ArmorAttrMustBe || UnityEngine.Random.Range(0, 100) < buffObbs)
                    {
                        int attrTyp = (Main.settings.ArmorAttr == 0) ? UnityEngine.Random.Range(0, 6) : (Main.settings.ArmorAttr - 1);
                        newItem[51361 + attrTyp] =
                            (Mathf.Max(int.Parse(presetItem[8]) / 2, 1) * ((buffObbs != 999) ? 5 : 10)).ToString();
                    }
                    if (Main.settings.ArmorPower != 1 &&
                        (Main.settings.ArmorPowerMustBe || UnityEngine.Random.Range(0, 100) < sBuffObbs))
                    {
                        int power;
                        if (Main.settings.ArmorPower == 0)
                        {
                            List<int> list2 = new List<int>
                                    {
                                        401,
                                        402,
                                        405,
                                        406,
                                        407
                                    };
                            power = list2[UnityEngine.Random.Range(0, list2.Count)];
                        }
                        else
                        {
                            power = (Main.settings.ArmorPower <= 3) ?
                                (Main.settings.ArmorPower + 399) : (Main.settings.ArmorPower + 401);
                        }
                        var method = typeof(DateFile).GetMethod("ChangeNewItemSPower",
                            BindingFlags.NonPublic | BindingFlags.Instance);
                        method.Invoke(__instance, new object[] { id, newItemId, power });
                        //__instance.ChangeNewItemSPower(id, num, list2[UnityEngine.Random.Range(0, list2.Count)]);
                    }
                    __result = newItemId;
                    return false;
                }
                //饰品
                if (equipTyp == 3)
                {
                    //耐久
                    if (!isFixed)
                    {
                        int minimum = 50;
                        duration = 1 + duration * UnityEngine.Random.Range(minimum, 101) / 100;
                    }
                    newItem[902] = duration.ToString();//最大耐久
                    newItem[901] = duration.ToString();//当前耐久
                    if (Main.settings.JewelryAttrMustBe || UnityEngine.Random.Range(0, 100) < buffObbs)
                    {
                        int attrTyp;
                        if (Main.settings.JewelryAttr0 == 0) attrTyp = (UnityEngine.Random.Range(0, 100) < 50) ?
                                UnityEngine.Random.Range(0, 16) + 50501 : UnityEngine.Random.Range(0, 14) + 50601;
                        else
                        {
                            if (Main.settings.JewelryAttr0 == 1) attrTyp = Main.settings.JewelryAttr1 + 50501;
                            else attrTyp = Main.settings.JewelryAttr2 + 50601;
                        }
                        newItem[attrTyp] =
                            (Mathf.Max(int.Parse(presetItem[8]) / 2, 1) * ((buffObbs != 999) ? 5 : 10)).ToString();
                    }
                    if (Main.settings.JewelryPowerMustBe || UnityEngine.Random.Range(0, 100) < sBuffObbs)
                    {
                        int power = 407;
                        var method = typeof(DateFile).GetMethod("ChangeNewItemSPower",
                            BindingFlags.NonPublic | BindingFlags.Instance);
                        method.Invoke(__instance, new object[] { id, newItemId, power });
                        //__instance.ChangeNewItemSPower(id, num, list2[UnityEngine.Random.Range(0, list2.Count)]);
                    }
                    __result = newItemId;
                    return false;
                }
            }
            Main.Logger.Log("wrong type:" + equipTyp.ToString() + "; id:" + id.ToString());
            __result = newItemId;
            return false;
        }
    }
}