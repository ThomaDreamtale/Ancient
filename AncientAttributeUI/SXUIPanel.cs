using UnityEngine;
using UnityEngine.UI;
using SkySwordKill.Next.DialogEvent;
using SkySwordKill.Next.DialogSystem;
using SkySwordKill.NextMoreCommand.Attribute;
using SkySwordKill.NextMoreCommand.Utils;
using System;
using Ancient;

namespace AncientAttributeUI
{
    public class SXUIPanel : MonoBehaviour
    {
        private Text daoliValue;
        private Text lingqiaoValue;
        private Text baojiValue;
        private Text baoshangValue;
        private Text gongdeValue;
        private Text qiyunValue;
        public void Awake()
        {
            daoliValue = gameObject.transform.GetChild(2).GetChild(1).GetComponent<Text>();
            daoliValue.text = Main.Inst.AncientData.power.ToString();
            lingqiaoValue = gameObject.transform.GetChild(3).GetChild(1).GetComponent<Text>();
            lingqiaoValue.text = Main.Inst.AncientData.cardhole.ToString();
            baojiValue = gameObject.transform.GetChild(4).GetChild(1).GetComponent<Text>();
            baojiValue.text = Main.Inst.AncientData.crick.ToString();
            baoshangValue = gameObject.transform.GetChild(5).GetChild(1).GetComponent<Text>();
            baoshangValue.text = Main.Inst.AncientData.crickDamage.ToString();
            gongdeValue = gameObject.transform.GetChild(6).GetChild(1).GetComponent<Text>();
            gongdeValue.text = Main.Inst.AncientData.lord.ToString();
            qiyunValue = gameObject.transform.GetChild(7).GetChild(1).GetComponent<Text>();
            qiyunValue.text = Main.Inst.AncientData.luck.ToString();
            
        }
        public void OnEnable()
        {
            daoliValue.text = Main.Inst.AncientData.power.ToString();
            lingqiaoValue.text = Main.Inst.AncientData.cardhole.ToString();
            baojiValue.text = Main.Inst.AncientData.crick.ToString();
            baoshangValue.text = Main.Inst.AncientData.crickDamage.ToString();
            gongdeValue.text = Main.Inst.AncientData.lord.ToString(); ;
            qiyunValue.text = Main.Inst.AncientData.luck.ToString();
            Tools.canClickFlag = false;
            PlaySoundEffect();
        }

        private void OnDisable()
        {
            Tools.canClickFlag = true;
            PlaySoundEffect();
        }

        private void PlaySoundEffect()
        {
            Main.Inst.UIManagerHandle.prefabBank.TryGetValue("ShuXingUI", out GameObject go);
            var se = go.transform.GetChild(2);
            var instse = Instantiate(se);
            var audio = instse.GetComponent<AudioSource>();
            audio.volume = PlayerPrefs.GetFloat("MusicEffect", 0.5f);
            Destroy(instse.gameObject, 1f);
        }

        public static void SXUpdate(int pow, int cardho, int cr, int crd, int lor, int luc)
        {
            Main.Inst.AncientData.power += pow;
            Main.Inst.AncientData.cardhole += cardho;
            Main.Inst.AncientData.crick += cr;
            Main.Inst.AncientData.crickDamage += crd;
            Main.Inst.AncientData.lord += lor;
            Main.Inst.AncientData.luck += luc;
        }

        public static void SXup1()
        {
            SXUpdate(28, 1, 2, 5, 2, 2);
        }
        public static void SXup2()
        {
            SXUpdate(65, 1, 2, 5, 2, 2);
        }
        public static void SXup3()
        {
            SXUpdate(90, 1, 2, 5, 2, 2);
        }
        public static void SXup4()
        {
            SXUpdate(120, 1, 2, 5, 2, 2);
        }
    }
}
namespace NextEventSX
{
    [RegisterCommand]
    [DialogEvent("PowerUpdate")]
    public class UpdateAttack : IDialogEvent
    {
        public void Execute(DialogCommand command, DialogEnvironment env, Action callback)
        {
            MyLog.LogCommand(command);
            int num = command.GetInt(0);
            Main.Inst.AncientData.power += num;
            MyLog.Log(command, $"道力增加 {num}");
            MyLog.LogCommand(command, false);
            callback?.Invoke();
        }
    }
    [DialogEvent("CardholeUpdate")]
    public class UpdateDefense : IDialogEvent
    {
        public void Execute(DialogCommand command, DialogEnvironment env, Action callback)
        {
            MyLog.LogCommand(command);
            int num = command.GetInt(0);
            Main.Inst.AncientData.cardhole += num;
            MyLog.Log(command, $"灵窍增加 {num}");
            MyLog.LogCommand(command, false);
            callback?.Invoke();
        }
    }
    [DialogEvent("CrickUpdate")]
    public class UpdateCrick : IDialogEvent
    {
        public void Execute(DialogCommand command, DialogEnvironment env, Action callback)
        {
            MyLog.LogCommand(command);
            int num = command.GetInt(0);
            Main.Inst.AncientData.crick += num;
            MyLog.Log(command, $"暴击增加 {num}");
            MyLog.LogCommand(command, false);
            callback?.Invoke();
        }
    }
    [DialogEvent("CrickDamageUpdate")]
    public class UpdateCrickDamage : IDialogEvent
    {
        public void Execute(DialogCommand command, DialogEnvironment env, Action callback)
        {
            MyLog.LogCommand(command);
            int num = command.GetInt(0);
            Main.Inst.AncientData.crickDamage += num;
            MyLog.Log(command, $"暴伤增加 {num}");
            MyLog.LogCommand(command, false);
            callback?.Invoke();
        }
    }
    [DialogEvent("LordUpdate")]
    public class UpdateLord : IDialogEvent
    {
        public void Execute(DialogCommand command, DialogEnvironment env, Action callback)
        {
            MyLog.LogCommand(command);
            int num = command.GetInt(0);
            Main.Inst.AncientData.lord += num;
            MyLog.Log(command, $"功德增加 {num}");
            MyLog.LogCommand(command, false);
            callback?.Invoke();
        }
    }
    [DialogEvent("LuckUpdate")]
    public class UpdateLuck : IDialogEvent
    {
        public void Execute(DialogCommand command, DialogEnvironment env, Action callback)
        {
            MyLog.LogCommand(command);
            int num = command.GetInt(0);
            Main.Inst.AncientData.luck += num;
            MyLog.Log(command, $"气运增加 {num}");
            MyLog.LogCommand(command, false);
            callback?.Invoke();
        }
    }
}
