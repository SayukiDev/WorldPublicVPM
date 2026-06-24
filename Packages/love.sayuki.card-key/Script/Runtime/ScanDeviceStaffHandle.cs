using love.sayuki.CardKey.Script.Utils;
using UnityEngine;
using VRC.Dynamics;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;

namespace love.sayuki.CardKey.Script.Runtime
{
    public class ScanDeviceStaffHandle : ScanDeviceHandle
    {
        public string[] AllowedPlayers;
        public VRCUrl[] AllowedPlayersUrls;
        private string URLBody;
        public Transform TeleportPoint;
        public GameObject[] toActivate;
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

            foreach (var p in AllowedPlayers)
            {
                if (contactInfo.contactSender.player.displayName != p)
                {
                    continue;
                }

                ToPass();
                Activate();
                teleportHandle.TeleportTo(TeleportPoint);
                return;
            }

            if (URLBody != null)
            {
                if (URLBody.Contains(contactInfo.contactSender.player.displayName) &&
                    contactInfo.contactSender.player.displayName != "")
                {
                    ToPass();
                    Activate();
                    teleportHandle.TeleportTo(TeleportPoint);
                    return;
                }
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