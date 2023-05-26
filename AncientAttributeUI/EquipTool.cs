using System;
using System.IO;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using BepInEx;
using HarmonyLib;
using System.Linq;

namespace EquipTool
{
    [BepInPlugin("Ancient.AND.JustEquipIt", "EquipTool", "1.0.0")]
    public class JustEquipTool : BaseUnityPlugin 
    {
        public static JustEquipTool inst;
        public static readonly string texture2DPath = Path.Combine(Directory.GetParent(typeof(JustEquipTool).Assembly.Location).FullName, "Texture2D");
        public Dictionary<string, Sprite> spriteBank; 
        private void Awake() 
        {
            inst = this;
            Harmony.CreateAndPatchAll(typeof(Patcher));
            spriteBank = new Dictionary<string, Sprite>();
            LoadTexture2D();
        }

        private void LoadTexture2D()
        {
            var raw_imgs = new DirectoryInfo(texture2DPath).GetFiles();
            var texture2D = new Texture2D(0, 0, TextureFormat.ARGB32, false);
            foreach (var raw_img in raw_imgs)
            {
                if (raw_img.Extension == ".png" || raw_img.Extension == ".jpg")
                {
                    texture2D.LoadImage(File.ReadAllBytes(raw_img.FullName));
                    spriteBank.Add(raw_img.Name, Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f)));
                }
            }
        }
    }

    static class Patcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Tab.TabWuPingPanel), "Init")]
        private static void TabWuPingPanel_Init_Patch(Tab.TabWuPingPanel __instance)
        {
            var slot = GameObject.Instantiate(__instance.EquipDict[4].gameObject, __instance.EquipDict[4].transform.parent).GetComponent<Bag.EquipSlot>();
            slot.transform.localPosition = new Vector3(-50, -350, 0);
            slot.name = "衣服2";
            slot.EquipSlotType = (EquipSlotType)6;
            slot.AcceptType = CanSlotType.衣服;
            __instance.EquipDict.Add(6, slot);
            slot = GameObject.Instantiate(__instance.EquipDict[2].gameObject, __instance.EquipDict[4].transform.parent).GetComponent<Bag.EquipSlot>();
            slot.transform.localPosition = new Vector3(-200, -350, 0);
            slot.name = "武器3";
            slot.EquipSlotType = (EquipSlotType)7;
            slot.AcceptType = CanSlotType.武器;
            __instance.EquipDict.Add(7, slot);
            slot = GameObject.Instantiate(__instance.EquipDict[5].gameObject, __instance.EquipDict[4].transform.parent).GetComponent<Bag.EquipSlot>();
            slot.transform.localPosition = new Vector3(-350, -350, 0);
            slot.name = "饰品2";
            slot.EquipSlotType = (EquipSlotType)8;
            slot.AcceptType = CanSlotType.饰品;
            __instance.EquipDict.Add(8, slot);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(YSGame.Fight.UIFightPanel), "Awake")]
        private static void UIFightPanel_Awake_Patch(YSGame.Fight.UIFightPanel __instance)
        {
            if (__instance.FightWeapon.Count <1) { return; }
            var template = __instance.FightWeapon.First();
            var go1 = GameObject.Instantiate(template, template.transform.parent);
            go1.transform.Find("Slot/LeftUpMask/HotKey").GetComponent<UnityEngine.UI.Text>().text = "G";
            go1.name = "FightWeaponItem_3";
            go1.HotKey = KeyCode.G;
            go1.gameObject.SetActive(false);
            __instance.FightWeapon.Add(go1);

            var wpgo = __instance.transform.GetChild(0).GetChild(0).GetChild(2);
            GameObject.DestroyImmediate(wpgo.GetComponent<VerticalLayoutGroup>());
            var comp = wpgo.gameObject.AddComponent<GridLayoutGroup>();
            comp.transform.localPosition = new Vector3(-700, -160, 0); 
            comp.spacing = new Vector2(-18, -13);
            comp.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            comp.constraintCount = 2;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(DragMag), "EndDrag")]
        private static IEnumerable<CodeInstruction> DragMag_EndDrag_Patch(IEnumerable<CodeInstruction> instrs, ILGenerator iLGen)
        {
            bool flag1 = false;
            bool flag2 = false;
            bool completed = false;
            CodeInstruction pre_instr = new CodeInstruction(OpCodes.Nop);
            var method = typeof(KBEngine.Avatar).GetMethod("checkHasStudyWuDaoSkillByID", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            var field1 = typeof(DragMag).GetField("DragSlot", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            var field2 = typeof(DragMag).GetField("ToSlot", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            var field3 = typeof(Bag.EquipSlot).GetField("EquipSlotType", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            var lb1 = iLGen.DefineLabel();
            var lb2 = iLGen.DefineLabel();
            foreach (var instr in instrs)
            {
                if (!flag1 && pre_instr.opcode == OpCodes.Callvirt && pre_instr.operand == (object)method && instr.opcode == OpCodes.Brfalse)
                {
                    instr.opcode = OpCodes.Brtrue_S;
                    instr.operand = lb1;
                    yield return instr;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, field1);
                    yield return new CodeInstruction(OpCodes.Castclass, typeof(Bag.EquipSlot));
                    yield return new CodeInstruction(OpCodes.Ldfld, field3);
                    yield return new CodeInstruction(OpCodes.Ldc_I4_6);
                    yield return new CodeInstruction(OpCodes.Bge_Un_S, lb1);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, field2);
                    yield return new CodeInstruction(OpCodes.Castclass, typeof(Bag.EquipSlot));
                    yield return new CodeInstruction(OpCodes.Ldfld, field3);
                    yield return new CodeInstruction(OpCodes.Ldc_I4_6);
                    yield return new CodeInstruction(OpCodes.Ble_Un_S, lb2);
                    flag1 = true;
                    continue;
                }
                if (flag1 && !flag2)
                {
                    instr.labels.Add(lb1);
                    flag2 = true;
                }
                if (flag1 && flag2 && !completed && pre_instr.opcode == OpCodes.Br && instr.opcode == OpCodes.Ldarg_0)
                {
                    instr.labels.Add(lb2);
                    completed = true;
                }
                pre_instr = instr;
                yield return instr;
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(KBEngine.Avatar), "addEquipSeid")]
        private static IEnumerable<CodeInstruction> Avatar_addEquipSeid_Patch(IEnumerable<CodeInstruction> instrs, ILGenerator iLGen)
        {
            bool flag1 = false;
            bool flag2 = false;
            Queue<CodeInstruction> pre_instrs = new Queue<CodeInstruction>();
            var mt = typeof(List<YSGame.Fight.UIFightWeaponItem>).GetMethod("get_Count", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            var lb = iLGen.DefineLabel();
            var lv = iLGen.DeclareLocal(typeof(int));
            foreach (var instr in instrs)
            {
                if (!flag1 && pre_instrs.Count > 0 
                        && pre_instrs.Peek().opcode == OpCodes.Callvirt 
                        && pre_instrs.Peek().operand == (object)mt
                        && instr.opcode == OpCodes.Ble)
                {
                    yield return instr;
                    yield return new CodeInstruction(OpCodes.Ldsfld, typeof(YSGame.Fight.UIFightPanel).GetField("Inst", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public));
                    yield return new CodeInstruction(OpCodes.Ldfld, typeof(YSGame.Fight.UIFightPanel).GetField("FightWeapon", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public));
                    yield return new CodeInstruction(OpCodes.Callvirt, mt);
                    yield return new CodeInstruction(OpCodes.Ldloc, lv);
                    yield return new CodeInstruction(OpCodes.Blt, lb);
                    yield return new CodeInstruction(OpCodes.Ldsfld, typeof(YSGame.Fight.UIFightPanel).GetField("Inst", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public));
                    yield return new CodeInstruction(OpCodes.Ldfld, typeof(YSGame.Fight.UIFightPanel).GetField("FightWeapon", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public));
                    yield return new CodeInstruction(OpCodes.Ldloca, lv);
                    yield return new CodeInstruction(OpCodes.Ldloc, 16);
                    yield return new CodeInstruction(OpCodes.Ldloc, 5);
                    yield return new CodeInstruction(OpCodes.Call, typeof(Patcher).GetMethod("SetSkill", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic));
                    flag1 = true;
                    continue;
                }
                if (flag1 && pre_instrs.Peek().opcode == OpCodes.Add && instr.opcode == OpCodes.Ldloc_2)
                {
                    instr.labels.Add(lb);
                    flag2 = true;
                }
                pre_instrs.Enqueue(instr);
                if (pre_instrs.Count > 2)
                    pre_instrs.Dequeue();
                if (flag1 && !flag2)
                    continue;
                yield return instr;
            }
        }

        static void SetSkill(List<YSGame.Fight.UIFightWeaponItem> fightWeapon, ref int num, GUIPackage.Skill skill, KBEngine.ITEM_INFO itemInfo)
        {
            fightWeapon[num].gameObject.SetActive(true);
            fightWeapon[num++].SetWeapon(skill, itemInfo);
        }
    }
}
