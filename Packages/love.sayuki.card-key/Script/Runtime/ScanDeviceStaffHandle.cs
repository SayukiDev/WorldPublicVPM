using love.sayuki.CardKey.Script.Utils;
using UdonSharp;
using UnityEngine;
using VRC.Dynamics;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;

namespace love.sayuki.CardKey.Script.Runtime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ScanDeviceStaffHandle : ScanDeviceHandle
    {
        public string[] AllowedPlayers;
        public VRCUrl[] AllowedPlayersUrls;
        private string URLBody;
        public Transform TeleportPoint;
        public GameObject[] toActivate;
        public GameObject[] toDeactivate;
        public TeleportHandle teleportHandle;

        public void Start()
        {
            foreach (var u in AllowedPlayersUrls)
            {
                VRCStringDownloader.LoadUrl(u, this);
            }
        }

        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            URLBody +="\n"+ result.Result;
        }

        public override void OnStringLoadError(IVRCStringDownload result)
        {
            Debug.LogError("Failed to load URL: " + result.Error);
        }

        public override void OnContactEnter(ContactEnterInfo contactInfo)
        {
            if (contactInfo.contactSender.player == null)
            {
                ToWarning();
                return;
            }

            if (!contactInfo.contactSender.player.isLocal)
            {
                return;
            }
            if (IsAllowed(contactInfo.contactSender.player.displayName))
            {
                ToPass();
                Activate();
                foreach (var t in toActivate)
                {
                    t.SetActive(true);
                }

                foreach (var a in toActivate)
                {
                    a.SetActive(true);
                }
                teleportHandle.TeleportTo(contactInfo.contactSender.player, TeleportPoint);
                foreach (var t in toDeactivate)
                {
                    t.SetActive(false);
                }
                return;
            }

            ToWarning();
        }

        public bool IsAllowed(string playerName)
        {
            if (playerName == null)
            {
                return false;
            }

            foreach (var p in AllowedPlayers)
            {
                if (playerName != p)
                {
                    continue;
                }
                return true;
            }

            if (URLBody != null)
            {
                if (URLBody.Contains(playerName) &&
                    playerName != "")
                {
                    return true;
                }
            }
            return false;
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