using Ancient;
using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YSGame;
using GameObject = UnityEngine.GameObject;
using SkySwordKill.Next.DialogEvent;
using SkySwordKill.Next.DialogSystem;
using SkySwordKill.NextMoreCommand.Attribute;
using SkySwordKill.NextMoreCommand.Utils;
using System;
using AncientAttributeUI;

namespace AncientAttributeUI
{
    [BepInPlugin("Ancient.NPC.SXpanel", "NPCshuxingPanel", "1.0.0")]
    public class NPCSX : BaseUnityPlugin
    {
        public static NPCSX inst;
        private void Awake()
        {
            inst = this;
            Harmony.CreateAndPatchAll(typeof(NPCpatcher));
        }

    }

    public class NPCSXPanel : MonoBehaviour
    {
        public static List<int> powerlist = new List<int> { 6, 15, 28, 56, 95, 145, 210, 280, 360, 450, 550, 660, 780, 910, 1100 };
        public static List<int> cardholelist = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        public static List<int> cricklist = new List<int> { 22, 24, 26, 28, 30, 32, 34, 36, 38, 40, 42, 44, 46, 48, 50 };
        public static List<int> crickDamagelist = new List<int> { 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80 };
        public static List<int> lordlist = new List<int> { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30 };
        public static List<int> lucklist = new List<int> { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30 };
        public static JSONObject ArrayToJo(List<int> nums)
        {
            var result = JSONObject.arr;
            foreach (var num in nums)
            {
                result.Add(num);
            }
            return result;
        }
        public static JSONObject InitNPCSX(int id)
        {
            var npcData = NpcJieSuanManager.inst.GetNpcData(id);
            if (!npcData.HasField("power"))
            {
                npcData.AddField("power", ArrayToJo(powerlist));
                npcData.AddField("cardhole", ArrayToJo(cardholelist));
                npcData.AddField("crick", ArrayToJo(cricklist));
                npcData.AddField("crickDamage", ArrayToJo(crickDamagelist));
                npcData.AddField("lord", ArrayToJo(lordlist));
                npcData.AddField("luck", ArrayToJo(lucklist));
            }
            return npcData;
        }
        private void Awake()
        {
            if (UINPCJiaoHu.Inst != null) 
            {
                var id = UINPCJiaoHu.Inst.NowJiaoHuNPC.ID;
                var npcData = InitNPCSX(id);
                var level = npcData["Level"].I;
                Text daoliValue = gameObject.transform.GetChild(1).GetChild(1).GetComponent<Text>();
                daoliValue.text = npcData["power"][level - 1].I.ToString();
                Text lingqiaoValue = gameObject.transform.GetChild(2).GetChild(1).GetComponent<Text>();
                lingqiaoValue.text = npcData["cardhole"][level - 1].I.ToString();
                Text baojiValue = gameObject.transform.GetChild(3).GetChild(1).GetComponent<Text>();
                baojiValue.text = npcData["crick"][level - 1].I.ToString();
                Text baoshangValue = gameObject.transform.GetChild(4).GetChild(1).GetComponent<Text>();
                baoshangValue.text = npcData["crickDamage"][level - 1].I.ToString();
                Text gongdeValue = gameObject.transform.GetChild(5).GetChild(1).GetComponent<Text>();
                gongdeValue.text = npcData["lord"][level - 1].I.ToString();
                Text qiyunValue = gameObject.transform.GetChild(6).GetChild(1).GetComponent<Text>();
                qiyunValue.text = npcData["luck"][level - 1].I.ToString();
            }
        }
        private void OnEnable()
        {
            if (UINPCJiaoHu.Inst != null)
            {
                var id = UINPCJiaoHu.Inst.NowJiaoHuNPC.ID;
                var npcData = InitNPCSX(id);
                var level = npcData["Level"].I;
                Text daoliValue = gameObject.transform.GetChild(1).GetChild(1).GetComponent<Text>();
                daoliValue.text = npcData["power"][level - 1].I.ToString();
                Text lingqiaoValue = gameObject.transform.GetChild(2).GetChild(1).GetComponent<Text>();
                lingqiaoValue.text = npcData["cardhole"][level - 1].I.ToString();
                Text baojiValue = gameObject.transform.GetChild(3).GetChild(1).GetComponent<Text>();
                baojiValue.text = npcData["crick"][level - 1].I.ToString();
                Text baoshangValue = gameObject.transform.GetChild(4).GetChild(1).GetComponent<Text>();
                baoshangValue.text = npcData["crickDamage"][level - 1].I.ToString();
                Text gongdeValue = gameObject.transform.GetChild(5).GetChild(1).GetComponent<Text>();
                gongdeValue.text = npcData["lord"][level - 1].I.ToString();
                Text qiyunValue = gameObject.transform.GetChild(6).GetChild(1).GetComponent<Text>();
                qiyunValue.text = npcData["luck"][level - 1].I.ToString();
            }
            Tools.canClickFlag = false;
            MusicMag.instance.PlayEffectMusic("OpenUIMap", 0.5f);
        }
        private void OnDisable()
        {
            Tools.canClickFlag = true;
            MusicMag.instance.PlayEffectMusic("OpenUIMap", 0.5f);
        }
    }
    public class NPCpatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UINPCJiaoHu), nameof(UINPCJiaoHu.ShowNPCInfoPanel))]
        private static void Show_NPCSX()
        {
            var pr = UINPCJiaoHu.Inst.transform.GetComponentInChildren<TabGroup>(true);
            var pr2 = NewUICanvas.Inst.transform;
            if (Main.Inst.UIManagerHandle.prefabBank.TryGetValue("NPCSX", out GameObject go))
            {
                var btn = GameObject.Instantiate<GameObject>(go.transform.GetChild(1).gameObject, pr.transform);
                var npcsx = GameObject.Instantiate<GameObject>(go.transform.GetChild(0).gameObject, pr2);
                npcsx.SetActive(false);
                npcsx.AddComponent<NPCSXPanel>();
                btn.transform.localPosition = new Vector3(-542.37f, -485.694f, 0f);
                Button openBtn = btn.transform.GetComponent<Button>();
                Button closeBtn = npcsx.GetComponentInChildren<Button>();
                openBtn.onClick.AddListener(() => { npcsx.SetActive(true); });
                closeBtn.onClick.AddListener(() => { npcsx.SetActive(false); });
            }
        }
    }
}
namespace NPCnextEventSX
{
    [RegisterCommand]
    [DialogEvent("NPCSXUpdate")]
    public class NPCSXUpdate : IDialogEvent
    {
        public void Execute(DialogCommand command, DialogEnvironment env, Action callback)
        {
            MyLog.LogCommand(command);
            int oldid = command.GetInt(0);
            int id = NPCEx.NPCIDToNew(oldid);
            int power = command.GetInt(1);
            int cardhole = command.GetInt(2);
            int crick = command.GetInt(3);
            int crickDamage = command.GetInt(4);
            int lord = command.GetInt(5);
            int luck = command.GetInt(6);
            var npcData = NPCSXPanel.InitNPCSX(id);
            var level = npcData["Level"].I;
            for (int i = 0; i < 15; i++) npcData["power"][i].i += power;
            for (int i = 0; i < 15; i++) npcData["cardhole"][i].i += cardhole;
            for (int i = 0; i < 15; i++) npcData["crick"][i].i += crick;
            for (int i = 0; i < 15; i++) npcData["crickDamage"][i].i += crickDamage;
            for (int i = 0; i < 15; i++) npcData["lord"][i].i += lord;
            for (int i = 0; i < 15; i++) npcData["luck"][i].i += luck;
            MyLog.Log(command, $"ID为 {id} 的NPC ：道力增加 {power} 灵窍增加 {cardhole} 暴击增加 {crick} 暴伤增加 {crickDamage} 功德增加 {lord} 气运增加 {luck} ");
            MyLog.LogCommand(command, false);
            callback?.Invoke();
        }
    }
        
}
