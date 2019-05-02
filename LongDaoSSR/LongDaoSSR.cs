using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Harmony12;
using UnityModManagerNet;
using System.Reflection;
using Random = System.Random;
//using System.IO;

namespace LongDaoSSR
{
    public static class Main
    {
        public static bool enabled;
        public static UnityModManager.ModEntry.ModLogger logger;
        public static int lastNPCid = -1; //最后生成且还未判断的NPC的id，-1表示无
        public static bool oneFlag = false;
        public static bool isInGame = false;
        //public static string logPath; //调试输出路径

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            logger = modEntry.Logger;
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            //logPath = System.IO.Path.Combine(modEntry.Path, "log/debuglog.txt");

            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (!value) return false;
            enabled = value;
            logger.Log("龙岛忠仆MOD正在运行");
            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label("  修改了一些龙岛忠仆的特性，可称最强忠仆!");
            GUILayout.Label("<color=#8FBAE7FF>【志同道合】</color> 与太吾传人立场一致，不求...但求....");
            GUILayout.Label("<color=#F28234FF>【龙·神赐】</color> 被龙神赋予更长的阳寿，神赐·不朽.");
            GUILayout.Label("<color=#E4504DFF>【天资·艺】</color> 夺天造化，大幅增强八项技艺的资质.");
            GUILayout.Label("<color=#E4504DFF>【天资·武】</color> 夺天造化，大幅增强四项进攻武学的资质，大幅增强内功身法绝技的资质.");
            //10%的概率化身
            GUILayout.Label("  大幅优化龙仆造型、魅力，10%的概率化身，跟太吾拥有相同的外观");
            //GUILayout.Label("  <color=#FF0000FF>如果要删除本MOD，请在对应存档内按下清除新特性的按钮并存档，避免坏档，清除新特性只影响显示效果，忠仆不会消失</color>");
            //检测存档
            DateFile tbl = DateFile.instance;
            if (tbl == null || tbl.actorsDate == null || !tbl.actorsDate.ContainsKey(tbl.mianActorId))
            {
                GUILayout.Label("  存档未载入!");
            }
            else
            {
                if (GUILayout.Button("天地洪炉，重置太吾属性，!"))
                {
                    PlayerStrong();
                }

                if (GUILayout.Button("重置太吾道友"))
                {
                    TeamRemoveImpurity();
                }
                //if (GUILayout.Button("清除新特性"))
                //{
                //    deletNewFeature();
                //}
            }
        }

        /// <summary>
        /// 遍历人物列表并清除新特性
        /// </summary>
        public static void deletNewFeature()
        {
            List<int> idlist = new List<int>();
            int num = 0;
            logger.Log("开始清除新特性");
            logger.Log("人物有" + DateFile.instance.actorsDate.Count + "个等待遍历");
            foreach (KeyValuePair<int, Dictionary<int, string>> e in DateFile.instance.actorsDate)
            {
                if (e.Value.ContainsKey(101))
                {
                    if (e.Value[101].Contains("4006"))
                    {
                        num++;
                        idlist.Add(e.Key);
                    }
                }
            }
            logger.Log("检测到" + num + "个龙岛忠仆，开始清除新特性数据...");
            for (int i = 0; i < idlist.Count; i++)
            {
                deletNPCNewFeature(idlist[i]);
            }
            logger.Log("清除完毕");
        }

        /// <summary>
        /// 清除NPC的新特性
        /// </summary>
        /// <param name="id">NPCid</param>
        public static void deletNPCNewFeature(int id)
        {
            bool hasNewFeature = false;
            Dictionary<int, string> npc;
            npc = DateFile.instance.actorsDate[id];

            List<int> feature = new List<int>();
            for (int i = 0; i < DateFile.instance.GetActorFeature(id).Count; i++)
            {
                feature.Add(DateFile.instance.GetActorFeature(id)[i]);
            }
            foreach (int f in feature)
            {
                if (f >= 4006 && f <= 4034)//新特性编号范围
                {
                    hasNewFeature = true;
                    npc[101] = npc[101].Replace("|" + f.ToString(), "");
                }
            }
            if (hasNewFeature)
            {
                DateFile.instance.actorsFeatureCache.Remove(id);
            }

        }


        /// <summary>
        /// 在开始游戏界面注入新特性
        /// </summary>
        [HarmonyPatch(typeof(MainMenu), "ShowStartGameWindow")]
        public static class MainMenu_ShowStartGameWindow_Patch
        {
            private static void Postfix()
            {
                if (!Main.enabled)
                {
                    return;
                }
                if (!oneFlag)
                {
                    addAllFeature();
                    //debugLogIntIntString(DateFile.instance.actorFeaturesDate);//显示全部特性
                }
                return;
            }
        }

        /// <summary>
        /// 获取新生NPC的ID
        /// </summary>
        [HarmonyPatch(typeof(DateFile), "MakeNewActor")]
        public static class DateFile_MakeNewActor_Patch
        {
            private static void Postfix(DateFile __instance, int __result)
            {
                if (!Main.enabled)
                {
                    return;
                }
                logger.Log("新的NPC生成了！id:" + __result);
                lastNPCid = __result;
                DateFile.instance.actorsFeatureCache.Remove(__result); //刷新特性
                return;
            }
        }

        /// <summary>
        /// 创建NPC之后，显示新相知之前执行的函数，用来修改龙岛忠仆
        /// </summary>
        [HarmonyPatch(typeof(DateFile), "ChangeFavor")]
        public static class DateFile_ChangeFavor_Patch
        {
            private static void Postfix()
            {
                if (!Main.enabled)
                {
                    return;
                }
                if (lastNPCid != -1)
                {
                    //logger.Log("特性:" + DateFile.instance.actorsDate[lastNPCid][101]);
                    if (isLongDaoZhongPu(lastNPCid))
                    {
                        npcChange(lastNPCid);
                    }
                    lastNPCid = -1;
                }
            }
        }

        /// <summary>
        /// 判断指定idNPC是否为龙岛忠仆
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool isLongDaoZhongPu(int id)
        {
            bool flag = false;
            List<int> npcFeature = DateFile.instance.GetActorFeature(lastNPCid);
            for (int i = 0; i < npcFeature.Count; i++)
            {
                if (npcFeature[i] == 4005) //4005为龙岛忠仆特性
                {
                    flag = true;
                    logger.Log("检测到新加入的龙岛忠仆");
                }
            }
            return flag;
        }

        /// <summary>
        /// 修改指定idNPC数据
        /// </summary>
        /// <param name="id"></param>
        public static void npcChange(int id)
        {
            Dictionary<int, string> npc;
            npc = DateFile.instance.actorsDate[id];
            Dictionary<int, string> player;
            player = DateFile.instance.actorsDate[DateFile.instance.mianActorId];

            Random valueRd = new Random();

            #region 塑金身

            if (!npc.ContainsKey(14))
            {
                //logger.Log("GenderValue Random");
                npc.Add(14, DateFile.instance.GetActorDate(id, 14, false));
                var genderValue = valueRd.Next(1, 3);
                npc[14] = genderValue.ToString();
            }

            npc[11] = Convert.ToString(valueRd.Next(16, 25));//出生年龄

            if (valueRd.Next(1, 11) == 1)//10%的概率化身
            {
                npc[996] = player[996];
                npc[995] = player[995];
                npc[15] = player[15];
                if (!player.ContainsKey(14))
                {
                    npc[14] = "1";
                }
                if (player.ContainsKey(17))
                {
                    if (!npc.ContainsKey(17))
                    {
                        npc.Add(17, DateFile.instance.GetActorDate(id, 17, false));
                    }
                    npc[17] = player[17];
                }
            }
            else
            {
                if (!npc.ContainsKey(995))
                {
                    npc.Add(995, DateFile.instance.GetActorDate(id, 995, false));
                }
                if (!npc.ContainsKey(996))
                {
                    npc.Add(996, DateFile.instance.GetActorDate(id, 996, false));
                }
                if (!npc.ContainsKey(15))
                {
                    npc.Add(15, DateFile.instance.GetActorDate(id, 15, false));
                }

                //鼻子 31~45[30,45)
                string rdNose = Convert.ToString(valueRd.Next(30, 45));
                //特征 21~30[20,30) 保留9个选项
                List<int> listSign = new List<int> { 1, 12, 13, 21, 22, 24 };
                string rdSign = (listSign[valueRd.Next(0, 6)] - 1).ToString();
                //眼睛 31~45 保留9个选项
                List<int> listEyes = new List<int> { 33, 36, 37, 38, 39, 40, 41, 42, 45 };
                string rdEyes = (listEyes[valueRd.Next(0, 9)] - 1).ToString();
                //眉毛 31~45
                string rdBrow = Convert.ToString(valueRd.Next(30, 45));
                //嘴唇 31~45
                string rdMouse = Convert.ToString(valueRd.Next(30, 45));
                //胡子
                //发型 男女有别
                string rdHair = "2";
                if (npc[14] == "1")
                {
                    //男生发型18项
                    List<int> listManHair = new List<int> { 1, 2, 3, 10, 18, 10, 18, 19, 21, 22, 23, 24, 27, 36, 37, 40, 45, 48 };
                    rdHair = (listManHair[valueRd.Next(0, 18)] - 1).ToString();
                }
                else
                {
                    //女生发型16项
                    List<int> listFemaleHair = new List<int> { 2, 4, 8, 10, 11, 18, 22, 27, 34, 37, 40, 44, 45, 50, 52, 54 };
                    rdHair = (listFemaleHair[valueRd.Next(0, 16)] - 1).ToString();
                }
                //捏脸 0|鼻子|特征|眼睛|眉毛|嘴|胡子|发型
                npc[995] = "0" + "|" + rdNose + "|" + rdSign + "|" + rdEyes + "|" + rdBrow + "|" + rdMouse + "|" + "0" + "|" + rdHair;
                //着色
                //发眉同色
                string valueHairBrow = valueRd.Next(0, 10).ToString();
                npc[996] = valueRd.Next(1, 8).ToString()         //皮肤
                           + "|" + valueHairBrow                 //眉毛
                           + "|" + valueRd.Next(0, 4).ToString() //#
                           + "|" + valueRd.Next(1, 7).ToString() //嘴唇
                           + "|" + valueRd.Next(0, 10).ToString()//眼睛
                           + "|" + valueRd.Next(0, 10).ToString()//特征
                           + "|" + valueHairBrow                 //头发
                           + "|" + valueRd.Next(0, 8).ToString();//衣着
                //魅梦 魅力值
                npc[15] = Convert.ToString(valueRd.Next(580, 901));
                logger.Log(string.Format("npc - 995:{0} - 996:{1} - 15:{2} - 14:{3}", npc[995], npc[996], npc[15], npc[14]));
            }

            #endregion

            //【龙·造化】
            var baseNature = npc.Where(p => p.Key >= 61 && p.Key <= 66).ToList();
            int baseValue = baseNature.Max(p => Int32.Parse(p.Value));
            foreach (var item in baseNature)
            {
                npc[item.Key] = (baseValue + valueRd.Next(10, 100)).ToString();
            }

            //【脱胎换骨】
            npc[101] = RemoveImpurity(npc[101]);

            //1.【志同道合】与太吾传人立场一致，不求...但求...
            //logger.Log("npc[16]:"+npc[16] + ",player[16]:"+player[16]);
            npc[16] = player[16]; //修改立场
            //npc[101] += "|4006"; //添加立场特性

            //2.【龙·神赐】被龙神赋予更长的阳寿，神赐·不朽
            //logger.Log("npc[11] npc[12] npc[13]:"+npc[11] +" "+ npc[12] + " " + npc[13]);
            npc[13] = "100";
            npc[12] = "100";
            //logger.Log("npc[11] npc[12] npc[13]:" + npc[11] + " " + npc[12] + " " + npc[13]);
            //npc[101] += "|4007"; //添加寿命特性
            //资质均衡
            npc[551] = "2";
            npc[651] = "2";

            //3.【精于道·艺】技艺资质
            var jiYiNpcAll = npc.Where(p => p.Key >= 501 && p.Key <= 516).ToList();
            var jiYiNpc = jiYiNpcAll.OrderByDescending(p => Int32.Parse(p.Value)).Take(8).ToList();
            foreach (var item in jiYiNpc)
            {
                npc[item.Key] = valueRd.Next(100, 120).ToString();
            }
            foreach (var item in jiYiNpcAll)
            {
                if (jiYiNpc.All(p => p.Key != item.Key))
                {
                    npc[item.Key] = valueRd.Next(45, 100).ToString();
                }
            }
            //var topShowJiYi = jiYiNpc.OrderByDescending(p => Int32.Parse(p.Value)).Take(2).ToList();
            //foreach (var item in topShowJiYi)
            //{
            //    npc[101] += "|" + (3507 + item.Key).ToString(); //TOP2 SHOW
            //}

            //4.【精于道·武】功法资质
            var jgongFaNpcAll = npc.Where(p => p.Key >= 604 && p.Key <= 614).ToList();
            var gongFaNpc = jgongFaNpcAll.OrderByDescending(p => Int32.Parse(p.Value)).Take(4).ToList();
            foreach (var item in gongFaNpc)
            {
                npc[item.Key] = valueRd.Next(110, 120).ToString();
            }
            foreach (var item in jgongFaNpcAll)
            {
                if (gongFaNpc.All(p => p.Key != item.Key))
                {
                    npc[item.Key] = valueRd.Next(40, 105).ToString();
                }
            }
            //var topShowGongFa = gongFaNpc.OrderByDescending(p => Int32.Parse(p.Value)).Take(2).ToList();
            //foreach (var item in topShowGongFa)
            //{
            //    npc[101] += "|" + (3420 + item.Key).ToString(); //TOP2 SHOW
            //}

            //5.【洗筋易髓】三分归元气
            npc[601] = valueRd.Next(110, 120).ToString();//内功：601
            npc[602] = valueRd.Next(105, 120).ToString();//身法：602
            npc[603] = valueRd.Next(100, 120).ToString();//绝技：603

            //6.【新手村】员工套餐
            //工作服 73703 劲衣 工作车 83503 下泽车
            npc[305] = DateFile.instance.MakeNewItem(73703).ToString();
            npc[311] = DateFile.instance.MakeNewItem(83503).ToString();

            DateFile.instance.actorsFeatureCache.Remove(id); //刷新特性
        }

        /// <summary>
        /// 注入新特性，占用特性表4006-4034
        /// </summary>
        public static void addAllFeature()
        {
            //志同道合
            addNewFeature(4006, "<color=#8FBAE7FF>志同道合</color>", "<color=#EFE38EFF>与太吾传人立场一致，不求...但求...</color>", "0", "1", "1|1", "4006");
            //龙神赐寿
            addNewFeature(4007, "<color=#F28234FF>龙·神赐</color>", "<color=#EFE38EFF>被龙神赋予更长的阳寿，神赐·不朽</color>", "0", "3|3|3", "0", "4007");

            //精于道·艺
            String[] yiWrod = new string[] { "音律", "弈棋", "诗书", "绘画", "术数", "品鉴", "锻造", "制木", "医术", "毒术", "织锦", "巧匠", "道法", "佛学", "厨艺", "杂学" };
            for (int i = 4008; i < 4024; i++)
            {
                addNewFeature(i, "<color=#E4504DFF>天资·" + yiWrod[i - 4008] + "</color>", "<color=#EFE38EFF>天生对</color><color=#E4504DFF>" + yiWrod[i - 4008] + "</color><color=#EFE38EFF>拥有异样的体悟，精于此道</color>", "0", "0", "1|1|1", "4008");
            }

            //精于道·武
            String[] wuWrod = new string[] { "拳掌", "指法", "腿法", "暗器", "剑法", "刀法", "长兵", "奇门", "软兵", "御射", "乐器" };
            for (int i = 4024; i < 4035; i++)
            {
                addNewFeature(i, "<color=#E4504DFF>天资·" + wuWrod[i - 4024] + "</color>", "<color=#EFE38EFF>天生对</color><color=#E4504DFF>" + wuWrod[i - 4024] + "</color><color=#EFE38EFF>拥有异样的体悟，精于此道</color>", "1|1|1", "0", "0", "4008");
            }
        }

        /// <summary>
        /// 向特性表中添加特性
        /// </summary>
        /// <param name="featureID">特性id</param>
        /// <param name="featureName">特性名称</param>
        /// <param name="featureDisc">特性描述</param>
        /// <param name="zhanDou">战斗点</param>
        /// <param name="fangYu">防御点</param>
        /// <param name="jiLue">机略点</param>
        /// <param name="zu">所属组</param>
        public static void addNewFeature(int featureID, string featureName, string featureDisc, string zhanDou, string fangYu, string jiLue, string zu)
        {
            DateFile.instance.actorFeaturesDate[featureID] = new Dictionary<int, string>();
            foreach (KeyValuePair<int, string> kv in DateFile.instance.actorFeaturesDate[0])
            {
                DateFile.instance.actorFeaturesDate[featureID][kv.Key] = kv.Value;
            }
            DateFile.instance.actorFeaturesDate[featureID][0] = featureName;
            DateFile.instance.actorFeaturesDate[featureID][99] = featureDisc;
            DateFile.instance.actorFeaturesDate[featureID][1] = zhanDou;
            DateFile.instance.actorFeaturesDate[featureID][2] = fangYu;
            DateFile.instance.actorFeaturesDate[featureID][3] = jiLue;
            DateFile.instance.actorFeaturesDate[featureID][5] = zu;
        }

        /// <summary>
        /// 资质清洗，脱胎换骨
        /// </summary>
        /// <param name="features"></param>
        /// <returns></returns>
        public static string RemoveImpurity(string features)
        {
            string result = features;
            if (!string.IsNullOrEmpty(features))
            {
                var beanFeatures = features.Split('|').ToList();
                //地狱特性
                var allImpurityFeatures = DateFile.instance.actorFeaturesDate.Where(p => int.Parse(p.Value[4]) < 0).Select(p => p.Key.ToString()).ToList();
                //造化特性
                var allJadeFeatures = DateFile.instance.actorFeaturesDate.Where(p => int.Parse(p.Value[4]) > 0 && beanFeatures.All(m => m != p.Value[4]))
                                                                         .Select(p => p.Key.ToString())
                                                                         .OrderBy(p => Guid.NewGuid()).ToList();
                var reFeatures = beanFeatures.Where(p => allImpurityFeatures.All(m => m != p)).ToList();
                reFeatures.AddRange(allJadeFeatures.Take(beanFeatures.Count() - reFeatures.Count()));
                // p.Value[0] == "福星高照"
                var fuyuanFeatures = DateFile.instance.actorFeaturesDate.Where(p => p.Value[0] == "禄马同乡" || p.Value[0] == "如天至福" || p.Value[0] == "福星高照")
                                                                        .Select(p => p.Key.ToString())
                                                                        .OrderBy(p => Guid.NewGuid()).Take(1).ToList();
                var meiliFeatures = DateFile.instance.actorFeaturesDate.Where(p => p.Value[0] == "良才美玉" || p.Value[0] == "不世奇才")
                                                                        .Select(p => p.Key.ToString())
                                                                        .OrderBy(p => Guid.NewGuid()).Take(1).ToList();
                var fireFeatures = DateFile.instance.actorFeaturesDate.Where(p => p.Value[0] == "形正神明" || p.Value[0] == "克己慎行" ||
                                                                                  p.Value[0] == "亭亭鹤立" || p.Value[0] == "一支玉箫")
                                                                        .Select(p => p.Key.ToString())
                                                                        .ToList();
                //邪魔消亡
                var fireSelfFeatures = DateFile.instance.actorFeaturesDate.Where(p => p.Value[0] == "执迷化魔" || p.Value[0] == "执迷入邪")
                                                        .Select(p => p.Key.ToString())
                                                        .ToList();
                foreach (var item in fuyuanFeatures)
                {
                    if (reFeatures.All(p => p != item))
                    {
                        reFeatures.Add(item);
                    }
                }
                foreach (var item in meiliFeatures)
                {
                    if (reFeatures.All(p => p != item))
                    {
                        reFeatures.Add(item);
                    }
                }
                foreach (var item in fireFeatures)
                {
                    if (reFeatures.All(p => p != item))
                    {
                        reFeatures.Add(item);
                    }
                }
                foreach (var item in fireSelfFeatures)
                {
                    if (reFeatures.Any(p => p == item))
                    {
                        reFeatures.Remove(item);
                    }
                }
                result = string.Join("|", reFeatures);
            }
            return result;
        }

        /// <summary>
        /// 造化钟神
        /// </summary>
        /// <param name="features"></param>
        /// <returns></returns>
        public static string MuImpurity(string features)
        {
            string result = features;
            if (!string.IsNullOrEmpty(features))
            {
                var reFeatures = features.Split('|').ToList();
                var meiliFeatures = DateFile.instance.actorFeaturesDate.Where(p => p.Value[0] == "八面玲珑" || p.Value[0] == "恶鬼罗刹" || p.Value[0] == "天元一气"
                                                                                || p.Value[0] == "蜂腰猿背" || p.Value[0] == "高视阔步" || p.Value[0] == "如天至福"
                                                                                || p.Value[0] == "无所畏惧" || p.Value[0] == "沉机观变" || p.Value[0] == "奔逸绝尘"
                                                                                || p.Value[0] == "体质特异" || p.Value[0] == "优游自若")
                                                                        .Select(p => p.Key.ToString())
                                                                        .ToList();
                foreach (var item in meiliFeatures)
                {
                    if (reFeatures.All(p => p != item))
                    {
                        reFeatures.Add(item);
                    }
                }
                result = string.Join("|", reFeatures);
            }
            return result;
        }

        /// <summary>
        /// 太吾觉醒
        /// </summary>
        public static void PlayerStrong()
        {
            Dictionary<int, string> player;
            player = DateFile.instance.actorsDate[DateFile.instance.mianActorId];

            Random valueRd = new Random();

            player[13] = "100";
            player[12] = "100";

            //资质均衡
            player[551] = "2";
            player[651] = "2";

            //【脱胎换骨】
            player[101] = RemoveImpurity(player[101]);
            player[101] = MuImpurity(player[101]);

            player[601] = valueRd.Next(120, 151).ToString();//内功：601
            player[602] = valueRd.Next(120, 151).ToString();//身法：602
            player[603] = valueRd.Next(120, 151).ToString();//绝技：603

            //技艺资质
            var jiYiPlayerAll = player.Where(p => p.Key >= 501 && p.Key <= 516).ToList();
            foreach (var item in jiYiPlayerAll)
            {
                player[item.Key] = valueRd.Next(110, 126).ToString();
            }

            //功法资质
            var jgongFaPlayerAll = player.Where(p => p.Key >= 604 && p.Key <= 614).ToList();
            foreach (var item in jgongFaPlayerAll)
            {
                player[item.Key] = valueRd.Next(100, 121).ToString();
            }
            player[608] = valueRd.Next(120, 141).ToString();
            player[609] = valueRd.Next(120, 141).ToString();
            player[604] = valueRd.Next(120, 141).ToString();
            player[605] = valueRd.Next(120, 141).ToString();

            //{ 61, "膂力"},{ 62, "体质"},{ 63, "灵敏"},{ 64, "根骨"},{ 65, "悟性"},{ 66, "定力"}
            player[61] = valueRd.Next(200, 251).ToString();
            player[62] = valueRd.Next(120, 251).ToString();
            player[63] = valueRd.Next(150, 251).ToString();
            player[64] = valueRd.Next(180, 251).ToString();
            player[65] = valueRd.Next(150, 251).ToString();
            player[66] = valueRd.Next(120, 251).ToString();

            ////魅梦 魅力值
            //player[15] = Convert.ToString(valueRd.Next(650, 901));
            DateFile.instance.actorsFeatureCache.Remove(DateFile.instance.mianActorId); //刷新特性

        }

        /// <summary>
        /// 道友觉醒
        /// </summary>
        public static void TeamRemoveImpurity()
        {
            for (int i = 1; i < 5; i++)
            {
                if (DateFile.instance.acotrTeamDate[i] != -1)
                {
                    var actor_id = DateFile.instance.acotrTeamDate[i];//队友
                    Dictionary<int, string> teamPlayer = DateFile.instance.actorsDate[actor_id];
                    //【脱胎换骨】
                    teamPlayer[101] = RemoveImpurity(teamPlayer[101]);
                    DateFile.instance.actorsFeatureCache.Remove(actor_id); //刷新特性
                }
            }
        }

        /*
        //debug遍历输出Dictionary<int, Dictionary<int, string>>
        public static void debugLogIntIntString(Dictionary<int, Dictionary<int, string>> dic, bool savelog)
        {
            String logText = "";
            int tmpnum = 0;
            foreach (KeyValuePair<int, Dictionary<int, string>> e in dic)
            {
                logText += "\nkey:" + e.Key + " value: ";
                foreach (KeyValuePair<int, string> kv in e.Value)
                {
                    logText += kv.Value + ",";
                }
                tmpnum++;
                if (tmpnum > 10000)
                {
                    break;
                }
            }
            if (savelog) saveLog(logText);
            else logger.Log(logText);
        }

        //debug遍历输出List<int>
        public static void debugLogListInt(List<int> list)
        {
            String logText = "";
            for (int i = 0; i < list.Count; i++)
            {
                logText += list[i] + ",";
            }
            logger.Log(logText);
        }

        //保存日志到log目录
        public static void saveLog(string logtext)
        {
            FileStream fs = new FileStream(logPath, FileMode.Create);
            byte[] logdata = System.Text.Encoding.Default.GetBytes(logtext);
            fs.Write(logdata, 0, logdata.Length);
            fs.Flush();
            fs.Close();
        }
        */
    }
}
