using UnityEngine;
using System.Collections.Generic;

namespace Ancient.UI
{
    public class AncientUIManager : MonoBehaviour
    {
        
        public Dictionary<string, GameObject> prefabBank;
        public Dictionary<string, Sprite> spriteBank;

        private void Awake() 
        {
            prefabBank = new Dictionary<string, GameObject>();
            spriteBank = new Dictionary<string, Sprite>();
        }

        private void Start()
        { 
        }
    }
}