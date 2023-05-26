using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using Ancient;
using UniqueCream;
using UniqueCream.MCSExtraTools.Interfaces;
using UniqueCream.MCSExtraTools.MoreTools;
using System;

namespace AncientAttributeUI
{
    [Serializable]
    class AncientData
    {
        public int power = 6;
        public int cardhole = 1;
        public int crick = 22;
        public int crickDamage = 10;
        public int lord = 2;
        public int luck = 2;
    }
    
    public class SaveLoad : AutoSaveEvent
    {
        public override void OnLoadSave()
        {
            Main.Inst.AncientData = DataTools.Instance.Load("AncientMCS", "AncientData" , new AncientData());
        }
        public override void OnSaveGame()
        {   
            DataTools.Instance.Save("AncientMCS", "AncientData", Main.Inst.AncientData ?? new AncientData());
        }
    }  
    /*
    static class SaveLoad
    {
        private static string SavePath
        {
            get {
                if (clientApp.IsTestVersion)
                    return Application.dataPath + "/../MCSSave_TestBranch";
                else
                    return Application.dataPath + "/../MCSSave";
            }
        }

        public static void SaveGame(int index , int slot)
        {
            var datas = JsonConvert.SerializeObject(Main.Inst.AncientData);
            using (FileStream fs = File.Create($"{SavePath}/Avatar{index}/Slot{slot}/AncientSave.json"))
            {
                var sw = new StreamWriter(fs);
                sw.Write(datas);
                sw.Dispose();
            }
        }
        
        public static void LoadGame(int index , int slot)
        {
            string fp = $"{SavePath}/Avatar{index}/Slot{slot}/AncientSave.json";
            if (!File.Exists(fp))
            {
                Main.Inst.AncientData = new AncientData();
                return;
            }
            using (FileStream fs = new FileStream(fp, FileMode.Open, FileAccess.Read))
            {
                var sr = new StreamReader(fs);
                string line;
                StringBuilder sb = new StringBuilder();
                while ((line = sr.ReadLine()) != null) 
                {
                    sb.Append(line);
                }
                Main.Inst.AncientData = JObject.Parse(sb.ToString()).ToObject<AncientData>();
                sr.Dispose();
            }
        }
    }
    */
}
