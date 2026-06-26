using love.sayuki.CardKey.Script.Utils;
using UdonSharp;
using UnityEngine;

namespace Script.Runtime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ColliderTeleport: UdonSharpBehaviour
    {
        public Transform teleportPoint;
        public TeleportHandle teleportHandle;
        public GameObject[] toActivate;
        public GameObject[] toDeactivate;
        private void Start()
        {
            foreach (var t in toActivate)
            {
                t.SetActive(false);
            }
            foreach (var t in toDeactivate)
            {
                t.SetActive(true);
            }
        }
        
        public override void Interact()
        {
            foreach (var t in toActivate)
            {
                t.SetActive(true);
            }
            teleportHandle.TeleportTo(teleportPoint);
            foreach (var t in toDeactivate)
            {
                t.SetActive(false);
            }
        }
    }
}