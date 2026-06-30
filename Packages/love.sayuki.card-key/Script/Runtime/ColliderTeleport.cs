using System;
using love.sayuki.CardKey.Script.Runtime;
using love.sayuki.CardKey.Script.Utils;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace love.sayuki.CardKey.Script.Runtime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ColliderTeleport: UdonSharpBehaviour
    {
        public Transform teleportPoint;
        public TeleportHandle teleportHandle;
        public GameObject[] toActivate;
        public GameObject[] toDeactivate;
        public GameObject[] fromOtherHandle;
        public bool revertFromOtherHandle;
        private void Start()
        {
            AddListFromOtherHandle();
            foreach (var t in toActivate)
            {
                t.SetActive(false);
            }
            foreach (var t in toDeactivate)
            {
                t.SetActive(true);
            }

            fromOtherHandle = new GameObject[0];
        }

        private void AddListFromOtherHandle()
        {
            foreach (var o in fromOtherHandle)
            {
                // card handle
                var c = o.GetComponent<CardHandle>();
                if (c != null)
                {
                    if (revertFromOtherHandle)
                    {
                        toDeactivate = AddToList(toDeactivate,c.toActivate);
                        toActivate = AddToList(toActivate,c.toDeactivate);
                    }else{
                        toActivate = AddToList(toActivate,c.toActivate);
                        toDeactivate = AddToList(toDeactivate,c.toDeactivate);
                    }
                }
                // staff handle
                var s=o.GetComponent<ScanDeviceStaffHandle>();
                if (s != null)
                {
                    if (revertFromOtherHandle)
                    {
                        toDeactivate = AddToList(toDeactivate,s.toActivate);
                        toActivate = AddToList(toActivate,s.toDeactivate);
                    }
                    else
                    {
                        toActivate = AddToList(toActivate,s.toActivate);
                        toDeactivate = AddToList(toDeactivate,s.toDeactivate);
                    }
                }
            }
        }

        public GameObject[] AddToList(GameObject[] baseL,GameObject[] toAdd)
        {
            if (toAdd == null) return baseL;
            if (toAdd.Length == 0) return baseL;
            var lenght = baseL.Length + toAdd.Length;
            var tempA=new GameObject[baseL.Length+lenght];
            baseL.CopyTo(tempA,0);
            toAdd.CopyTo(tempA,baseL.Length);
            return tempA;
        }
        
        public override void Interact()
        {
            ToActivate();
            teleportHandle.TeleportTo(Networking.LocalPlayer,teleportPoint);
            SendCustomEventDelayedSeconds("ToDeactivate", 10);
        }
        public void ToActivate()
        {
            foreach (var t in toActivate)
            {
                t.SetActive(true);
            }
        }

        public void ToDeactivate()
        {
            foreach (var t in toDeactivate)
            {
                t.SetActive(false);
            }
        }
    }
}