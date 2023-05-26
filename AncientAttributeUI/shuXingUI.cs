using System.IO;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Ancient.UI;
using AncientAttributeUI;
using YSGame;
using GameObject = UnityEngine.GameObject;
using TuPo;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KBEngine;
using WXB;
using SkySwordKill.Next.Utils;
using DG.Tweening;

namespace Ancient
{
    [BepInPlugin("Thoma.HongHuang.ShuXingUI", "Ancient", "1.0.0")]
    public class Main : BaseUnityPlugin
    {
        private static Main m_inst;
        private AncientUIManager m_UIManagerHandle;

        public static readonly string modPath = Directory.GetParent(typeof(Main).Assembly.Location)?.FullName;
        public static readonly string assetsPath = Path.Combine(modPath, "AssetBundle");

        public static Main Inst => m_inst;
        public AncientUIManager UIManagerHandle => m_UIManagerHandle;
        internal AncientData AncientData { get; set; }

        public static void log(object message) => Inst.Logger.LogInfo(message);

        public void Awake()
        {
            m_inst = this;
            var go = new GameObject("洪荒MOD");
            m_UIManagerHandle = go.AddComponent<AncientUIManager>();
            DontDestroyOnLoad(go);
            LoadAssetBundle();
            Harmony.CreateAndPatchAll(typeof(Patcher));
        }

        private void Update()
        {
            
        }

        void LoadAssetBundle()
        {
            var assets = new DirectoryInfo(assetsPath).GetFiles();
            foreach (var item in assets)
            {
                var req = AssetBundle.LoadFromFileAsync(item.FullName);
                req.completed += (op) =>
                {
                    var loadedAB = req.assetBundle;
                    var req2 = loadedAB.LoadAllAssetsAsync();
                    req2.completed += (op2) =>
                    {
                        var gos = req2.allAssets;
                        foreach (var go in gos)
                        {
                            if (go is GameObject)
                                m_UIManagerHandle.prefabBank.TryAdd<string, GameObject>(go.name, go as GameObject);
                            if (go is Sprite)
                                m_UIManagerHandle.spriteBank.TryAdd<string, Sprite>(go.name, go as Sprite);
                        }
                    };
                };
            }
        }
    }

    public class HelpTuPo : MonoBehaviour
    {
        public static HelpTuPo inst;
        private void Awake()
        {
            inst = this;
        }

        public static Dictionary<int, string> NewLvName = new Dictionary<int, string> {
            { 1, "炼气境" }, { 2, "筑基境" }, { 3, "金丹境" }, { 4, "元婴境" }, { 5, "化神境" },
            { 6, "返墟境" }, { 7, "人 仙" }, { 8, "地 仙" }, { 9, "天 仙" }, { 10, "金 仙" },
            { 11, "太乙金仙" }, { 12, "大罗金仙" }, { 13, "准 圣" }, { 14, "圣 人" }, { 15, "道 圣" }
            };
    }
    class Patcher
    {
        public static Patcher inst = new Patcher();
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIHeadPanel), "Awake")]
        private static void UIHeadPanel_Awake_Patch()
        {
            var pr = UIHeadPanel.Inst.transform.GetComponentInChildren<HorizontalOrVerticalLayoutGroup>(true);
            var pr2 = NewUICanvas.Inst.transform;
            if (Main.Inst.UIManagerHandle.prefabBank.TryGetValue("ShuXingUI", out GameObject go))
            {
                var btn = GameObject.Instantiate<GameObject>(go.transform.GetChild(1).gameObject, pr.transform);
                GameObject bg = GameObject.Instantiate<GameObject>(go.transform.GetChild(0).gameObject, pr2);
                bg.SetActive(false);
                bg.AddComponent<SXUIPanel>();
                if (Main.Inst.AncientData == null)
                {
                    Main.Inst.AncientData = new AncientData();
                }
                Button openBtn = btn.transform.GetComponent<Button>();
                Button closeBtn = bg.GetComponentInChildren<Button>();
                openBtn.onClick.AddListener(() => { bg.SetActive(true); });
                closeBtn.onClick.AddListener(() => { bg.SetActive(false); MusicMag.instance.PlayEffectMusic("OpenUIMap", 1f); });
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TuPoUIMag), nameof(TuPoUIMag.ShowTuPo))]
        private static bool SmallLvUp(ref string desc)
        {
            var pr = NewUICanvas.Inst.transform;
            switch ((int)Tools.instance.getPlayer().level)
            {
                case 2:
                    {
                        desc = "\u3000\u3000以气通络，滋血养气，强身淬体，筑基运气，洗涤身体，" +
                            "提升体质，沟通天地，你终于初步踏入了筑基境界。 " +
                        "道力+9  灵窍+1  暴击+2  暴伤+5  功德+2  气运+2";
                        SXUIPanel.SXUpdate(9, 1, 2, 5, 2, 2);
                    }
                    break;
                case 3:
                    {
                        desc = "\u3000\u3000凝精聚气，蜕变成丹，三花聚顶，五气朝元。" +
                            "丹成天地变，气运吞山河，你终于初步踏入了金丹境界。 " +
                            "道力+13  灵窍+1  暴击+2  暴伤+5  功德+2  气运+2";
                        SXUIPanel.SXUpdate(13, 1, 2, 5, 2, 2);
                    }
                    break;
                case 4:
                    {
                        desc = "\u3000\u3000碎丹凝婴，上冲中宫，溯本求渊，明心炼神，脑中见性，霞光满室，" +
                            "遍体生白，一战将息，你终于成功踏入元婴境界。";
                        SXUIPanel.SXup1();
                    }
                    break;
                case 5:
                    {
                        desc = "\u3000\u3000蜕丹炼神，忘情天地，沟通领域，呼风唤雨，" +
                            "一念移山，再念填海，你成功突破化神境界。 " +
                            "道力+39  灵窍+1  暴击+2  暴伤+5  功德+2  气运+2";
                        SXUIPanel.SXUpdate(39, 1, 2, 5, 2, 2);
                    }
                    break;
                case 6:
                    {
                        desc = "\u3000\u3000培虚引气，大道烙元，灵身渡虚，无所不成，你终于踏入返墟境界。  " +
                            "道力+50  灵窍+1  暴击+2  暴伤+5  功德+2  气运+2";
                        SXUIPanel.SXUpdate(50, 1, 2, 5, 2, 2);
                    }
                    break;
                case 7:
                    {
                        desc = "\u3000\u3000仙灵入体，粹体塑魂，以身证道，蜕虚成仙，道中得一法，法中得一术，" +
                            "信心苦志，终世不移，你终于成就人仙之位  ";
                        SXUIPanel.SXup2();
                    }
                    break;
                case 8:
                    {
                        desc = "\u3000\u3000收真一，察二仪，列三才，分四象，" +
                            "别五运，定六气，聚七宝，序八卦，行九洲，你成功突破地仙境界。  " +
                            "道力+70  灵窍+1  暴击+2  暴伤+5  功德+2  气运+2";
                        SXUIPanel.SXUpdate(70, 1, 2, 5, 2, 2);
                    }
                    break;
                case 9:
                    {
                        desc = "\u3000\u3000上士举形升虚，一飞冲天，不恋人间，你成功突破天仙境界。  " +
                            "道力+80  灵窍+1  暴击+2  暴伤+5  功德+2  气运+2";
                        SXUIPanel.SXUpdate(80, 1, 2, 5, 2, 2);
                    }
                    break;
                case 10:
                    {
                        desc = "\u3000\u3000人之修道，必由五行归五老，三花而化三清，始能归原无极本体，" +
                            "而达圆通究竟，你成功踏足金仙境界。";
                        SXUIPanel.SXup3();
                    }
                    break;
                case 11:
                    {
                        desc = "\u3000\u3000溯世求法，身化外道，不以内一，你成功踏足太乙金仙境界。  " +
                            "道力+100  灵窍+1  暴击+2  暴伤+5  功德+2  气运+2";
                        SXUIPanel.SXUpdate(100, 1, 2, 5, 2, 2);
                    }
                    break;
                case 12:
                    {
                        desc = "\u3000\u3000不老不死，永生不灭，仙境极乐，无所忧愁，" +
                            "飞升紫府，位列仙班，你成功踏足大罗金仙境界。  " +
                            "道力+110  灵窍+1  暴击+2  暴伤+5  功德+2  气运+2";
                        SXUIPanel.SXUpdate(110, 1, 2, 5, 2, 2);
                    }
                    break;
                case 13:
                    {
                        desc = "\u3000\u3000混元体正合先天，万劫千番只自然。渺渺无为浑太乙，" +
                            "如如不动号初玄。你成功踏足准圣境界。  ";
                        SXUIPanel.SXup4();
                    }
                    break;
                case 14:
                    {
                        desc = "\u3000\u3000炉中久炼非铅汞，物外长生是本仙。变化无穷还变化，" +
                            "三皈五戒总休言，你终于成就圣人境界。  " +
                            "道力+130  灵窍+1  暴击+2  暴伤+5  功德+2  气运+2";
                        SXUIPanel.SXUpdate(130, 1, 2, 5, 2, 2);
                    }
                    break;
                case 15:
                    {
                        desc = "\u3000\u3000以身合道，身化万千，天地渺渺，归一自然。你终于成就道圣境界。  " +
                            "道力+190  灵窍+1  暴击+2  暴伤+5  功德+2  气运+2";
                        SXUIPanel.SXUpdate(190, 1, 2, 5, 2, 2);
                    }
                    break;
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BigTuPoResultIMag), nameof(BigTuPoResultIMag.ShowSuccess))]
        private static void TuPoPatch(BigTuPoResultIMag __instance, int type)
        {
            Color32 col = new Color(0f, 1f, 1f, 1f);
            if (Main.Inst.UIManagerHandle.prefabBank.TryGetValue("UIReplaceComponent", out GameObject go)) 
            {
                var spArray = go.GetComponentsInChildren<SpriteRenderer>();
                if (type == 1)
                {
                    var tr = typeof(BigTuPoResultIMag).GetField("SuccessPanel", System.Reflection.BindingFlags.Instance
                        | System.Reflection.BindingFlags.NonPublic).GetValue(__instance) as Transform;
                    tr.GetChild(2).GetChild(0).GetComponent<Image>().sprite = spArray[3].sprite;
                    tr.GetChild(2).GetComponent<Image>().sprite = spArray[2].sprite;
                    tr.GetChild(2).GetChild(1).GetComponent<Text>().text = "【炼神凝婴】";
                    tr.GetChild(1).GetComponent<Text>().color = col;
                    tr.GetChild(2).GetChild(1).GetComponent<Text>().color = col;
                    tr.GetChild(2).GetChild(2).GetComponent<Text>().color = col;
                }
                else if (type == 2)
                {
                    var tr = typeof(BigTuPoResultIMag).GetField("SuccessPanel", System.Reflection.BindingFlags.Instance
                        | System.Reflection.BindingFlags.NonPublic).GetValue(__instance) as Transform;
                    tr.GetChild(3).GetChild(0).GetComponent<Image>().sprite = spArray[7].sprite;
                    tr.GetChild(3).GetComponent<Image>().sprite = spArray[6].sprite;
                    tr.GetChild(1).GetComponent<Text>().color = col;
                    tr.GetChild(3).GetChild(1).GetComponent<Text>().color = col;
                }
                else if (type == 3)
                {
                    var tr = typeof(BigTuPoResultIMag).GetField("SuccessPanel", System.Reflection.BindingFlags.Instance
                        | System.Reflection.BindingFlags.NonPublic).GetValue(__instance) as Transform;
                    tr.GetChild(4).GetChild(0).GetComponent<Image>().sprite = spArray[11].sprite;
                    tr.GetChild(4).GetComponent<Image>().sprite = spArray[10].sprite;
                    tr.GetChild(1).GetComponent<Text>().color = col;
                    tr.GetChild(4).GetChild(1).GetComponent<Text>().color = col;
                    tr.GetChild(4).GetChild(2).GetComponent<Text>().color = col;
                }
                else if (type == 4)
                {
                    var tr = typeof(BigTuPoResultIMag).GetField("SuccessPanel", System.Reflection.BindingFlags.Instance
                        | System.Reflection.BindingFlags.NonPublic).GetValue(__instance) as Transform;
                    tr.GetChild(5).GetChild(0).GetComponent<Image>().sprite = spArray[15].sprite;
                    tr.GetChild(5).GetComponent<Image>().sprite = spArray[14].sprite;
                    tr.GetChild(1).GetComponent<Text>().color = col;
                    tr.GetChild(5).GetChild(1).GetComponent<Text>().color = col;
                    tr.GetChild(5).GetChild(2).GetComponent<Text>().color = col;
                    tr.GetChild(5).GetChild(3).GetComponent<Text>().color = col;
                } 
            }

            if (Main.Inst.UIManagerHandle.prefabBank.TryGetValue("TuPoBg", out GameObject go2))
            {
                var spArray = go2.GetComponentsInChildren<SpriteRenderer>();
                var tr = typeof(BigTuPoResultIMag).GetField("SuccessPanel", System.Reflection.BindingFlags.Instance
                        | System.Reflection.BindingFlags.NonPublic).GetValue(__instance) as Transform;
                tr.GetChild(0).GetChild(1).GetComponent<Image>().sprite = spArray[0].sprite;
                tr.GetChild(0).GetChild(1).localPosition = new Vector3(-7.036f, 273.97f, 0f);
                tr.GetChild(0).GetChild(1).localScale = new Vector3(1.045f, 1.999f, 1f); 
                tr.GetChild(0).GetChild(2).GetComponent<Image>().sprite = spArray[1].sprite;
                tr.GetChild(0).GetChild(2).localPosition = new Vector3(0.025f, 70.03f, 0f);
                tr.GetChild(0).GetChild(2).localScale = new Vector3(1.1f, 2.5f, 1f);
                tr.GetChild(0).GetChild(3).gameObject.SetActive(false);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TuPoUIMag),nameof(TuPoUIMag.ShowTuPo))]

        private static void TuPoComPatch(TuPoUIMag __instance)
        {
            Color32 col = new Color(0f, 1f, 1f, 1f);
            if (Main.Inst.UIManagerHandle.prefabBank.TryGetValue("UIReplaceComponent", out GameObject go))
            {
                var spArray = go.GetComponentsInChildren<SpriteRenderer>();
                var newlist = new List<Sprite>() { null, spArray[0].sprite, spArray[1].sprite, spArray[2].sprite, spArray[4].sprite, spArray[5].sprite,
                                            spArray[6].sprite, spArray[8].sprite, spArray[9].sprite, spArray[10].sprite, spArray[12].sprite,
                                            spArray[13].sprite, spArray[14].sprite, spArray[16].sprite, spArray[17].sprite };
                var splist = typeof(TuPoUIMag).GetField("SpritesList", System.Reflection.BindingFlags.Instance
                        | System.Reflection.BindingFlags.NonPublic);
                splist.SetValue(__instance, newlist);
            }
            if (Main.Inst.UIManagerHandle.prefabBank.TryGetValue("TuPoBg", out GameObject go2))
            {
                var spArray = go2.GetComponentsInChildren<SpriteRenderer>();
                var tr = typeof(TuPoUIMag).GetField("Panel", System.Reflection.BindingFlags.Instance
                        | System.Reflection.BindingFlags.NonPublic).GetValue(__instance) as Transform;
                tr.GetChild(0).GetChild(1).GetComponent<Image>().sprite = spArray[0].sprite;
                tr.GetChild(0).GetChild(1).localPosition = new Vector3(-12.61f, 305.19f, 0f);
                tr.GetChild(0).GetChild(1).localScale = new Vector3(1.2f, 2.2f, 1f);
                tr.GetChild(0).GetChild(2).GetComponent<Image>().sprite = spArray[1].sprite;
                tr.GetChild(0).GetChild(2).localPosition = new Vector3(-4f, 72.308f, 0f);
                tr.GetChild(0).GetChild(2).localScale = new Vector3(1.1f, 2.3f, 1f);
                tr.GetChild(0).GetChild(3).gameObject.SetActive(false);
                tr.GetChild(5).GetChild(1).GetComponent<Text>().color = col;
                tr.GetChild(6).GetChild(1).GetComponent<Text>().color = col;
                tr.GetChild(7).GetChild(1).GetComponent<Text>().color = col;
                tr.GetChild(8).GetChild(1).GetComponent<Text>().color = col;
                tr.GetChild(5).GetChild(2).GetComponent<Text>().color = col;
                tr.GetChild(6).GetChild(2).GetComponent<Text>().color = col;
                tr.GetChild(7).GetChild(2).GetComponent<Text>().color = col;
                tr.GetChild(8).GetChild(2).GetComponent<Text>().color = col;
                tr.GetChild(5).GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().color = col;
                tr.GetChild(6).GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().color = col;
                tr.GetChild(7).GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().color = col;
                tr.GetChild(8).GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().color = col;
                tr.GetChild(3).GetComponent<Text>().color = col;
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BigTuPoResultIMag), "ShowSuccess")]
        private static IEnumerable<CodeInstruction> BigTuPoResultIMag_ShowSuccess_patch(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            string text1 = "\u3000\u3000碎丹凝婴，上冲中宫，溯本求渊，明心炼神，脑中见性，霞光满室，" +
                            "遍体生白，一战将息，你终于成功踏入元婴境界。" +
                            "道力+28  灵窍+1  暴击+2  暴伤+5  功德+2  气运+2";
            string text2 = "\u3000\u3000仙灵入体，粹体塑魂，以身证道，蜕虚成仙，道中得一法，法中得一术，" +
                            "信心苦志，终世不移，你终于成就人仙之位  " +
                            "道力+65  灵窍+1  暴击+2  暴伤+5  功德+2  气运+2  \n  原世界金丹：结成：<color=#";
            string text3 = "\u3000\u3000人之修道，必由五行归五老，三花而化三清，始能归原无极本体，" +
                            "而达圆通究竟，你成功踏足金仙境界。" +
                            "道力+90  灵窍+1  暴击+2  暴伤+5  功德+2  气运+2    (原世界元婴)";
            string text4 = "\u3000\u3000混元体正合先天，万劫千番只自然。渺渺无为浑太乙，" +
                            "如如不动号初玄。你成功踏足准圣境界。  " +
                            "道力+120  灵窍+1  暴击+2  暴伤+5  功德+2  气运+2 \n" +
                            "（原世界天道压制：天劫倒计时）";
            foreach (var code in codes)
            {
                if (code.opcode == OpCodes.Ldstr)
                {
                    if (code.operand.ToString() == "\u3000\u3000体内翻涌的灵气逐渐平息，原本呈气体状态下的灵气逐渐凝实液化，" +
                    "剩余的灵力则被肉体与经脉吸收。你只觉得全身舒畅，一种从未有过的力量感开始凝聚起来。")
                    {
                        code.operand = text1;
                        yield return code;
                    }
                    else if (code.operand.ToString() == "\u3000\u3000周边天地的灵气突然开始涌入你的体内，你感到体内的真元犹如沸腾的开水一般迅速流动起来，" +
                        "体内的灵气在你的不断压缩下，逐渐凝聚为实体，结成：<color=#")
                    {
                        code.operand = text2;
                        yield return code;
                    }
                    else if (code.operand.ToString() == "\u3000\u3000你在心魔的幻象中不知迷失了多久，仿若经历过数世的大喜大悲后，才猛然醒悟过来，摆脱了心魔的迷惑，将元婴凝结成型。" +
                        "刚刚凝结而成的元婴脱离丹室，飞腾而出，显于头颅之上。你只觉得心境祥和，仿佛重新回到了无忧无虑年幼时期。")
                    {
                        code.operand = text3;
                        yield return code;
                    }
                    else if (code.operand.ToString() == "\u3000\u3000你的神魂自天灵脱壳而出，融于周身天地之间。你开始清晰的感知到周天大道法则的力量，仿佛举手投足间便可调动这天地的力量为己所用。" +
                        "但下一秒，另一个无比强大的意志将你的神魂重新压制回肉身之中。你顿时不寒而栗，随即便意识到，这是天道...")
                    {
                        code.operand = text4;
                        yield return code;
                    }
                    else
                    {
                        yield return code;
                    }
                }
                else
                {
                    yield return code;
                }
            }
        }
        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BigTuPoResultIMag), "ShowFail")]
        private static IEnumerable<CodeInstruction> BigTuPoResultIMag_ShowFail_patch(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            var txt1 = "\u3000\u3000碎丹凝婴，上冲中宫，溯本求渊，阴神离宫，散于泥丸，" +
                            "莲台欲催，未出天门，你未能成功突破元婴境界。";
            var txt2 = "\u3000\u3000大道无情，刍狗苍生，你未能成功证道人仙。";
            var txt31 = "\u3000\u3000不由五行归五老，不以三花化三清，未能归原无极本体，" +
                                "则不达圆通究竟，你突破金仙失败，身死道消。";
            var txt32 = "\u3000\u3000不由五行归五老，不以三花化三清，未能归原无极本体，" +
                                "则不达圆通究竟，你突破金仙失败，就当你以为要消散于天地时，丹田之中" +
                                "药力磅礴，将你身形魂魄稳固。";
            var txt4 = "\u3000\u3000千般万般，因果难断，你未能成功踏入准圣境界，修为大退。 ";
            codes[8].operand = txt1;
            codes[20].operand = txt2;
            codes[35].operand = txt31;
            codes[70].operand = txt32;
            codes[105].operand = txt4;
            return codes.AsEnumerable();
        }

        
        /*
        [HarmonyPostfix]
        [HarmonyPatch(typeof(YSNewSaveSystem), "SaveGame")]
        private static void YSNewSaveSystem_SaveGame_Patch(int avatarIndex, int slot)
        {
            SaveLoad.SaveGame(avatarIndex, slot);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(YSNewSaveSystem), "LoadSave")]
        private static void YSNewSaveSystem_LoadSave_Patch(int avatarIndex, int slot)
        {
            SaveLoad.LoadGame(avatarIndex, slot);
        }  
        */
    }
}

