
using __Sayuki.CardKey.Script.Utils;
using UdonSharp;
using UnityEngine;
using VRC.Dynamics;
using VRC.SDKBase;
using VRC.Udon;

namespace __Sayuki.CardKey.Script.Runtime
{
    public class ScanDeviceStaffHandle : ScanDeviceHandle
    {
        public string[] AllowedPlayers;
        public Transform TeleportPoint;
        public GameObject[] toActivate;
        public TeleportHandle teleportHandle;
        
        public override void OnContactEnter(ContactEnterInfo contactInfo)
        {
            if (contactInfo.contactSender.player == null)
            {
                ToWarning();
                return;
            }
            foreach (var p in AllowedPlayers)
            {
                if (contactInfo.contactSender.player.displayName!=p)
                {
                    continue;
                }
                ToPass();
                Activate();
                teleportHandle.TeleportTo(TeleportPoint);
                return;
            }
            ToWarning();
        }

        private void Activate()
        {
            foreach (var t in toActivate)
            {
                t.SetActive(true);
            }
        }
    }
}