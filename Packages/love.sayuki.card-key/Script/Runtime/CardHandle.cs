using love.sayuki.CardKey.Script.Utils;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.Dynamics;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace love.sayuki.CardKey.Script.Runtime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class CardHandle : UdonSharpBehaviour
    { 
        public int CardID;
        public TextMeshPro CardText;
        public Transform TeleportPoint;
        public TeleportHandle teleportHandle;
        public ScanDeviceHandle scanDeviceHandle;
        private VRCPlayerApi playerApi;
        [UdonSynced]
        private string OwenerName;

        private void Start()
        {
            CardText.text = CardID.ToString();
        }

        public override void OnPickup()
        {
            var p = Networking.GetOwner(gameObject);
            if (p == null) return;
            if (p.isLocal)
            {
                /*if (OwenerName != "" && 
                    OwenerName != Networking.LocalPlayer.displayName)
                {
                    gameObject.GetComponent<VRC_Pickup>().Drop();
                    return;
                }*/
                OwenerName = p.displayName;
                 if (!p.isLocal) return;
                playerApi = p;
                p.SetPlayerTag(Utils.UserTag, CardID.ToString());
            }
        }

        public override void OnDrop()
        {
            if (playerApi == null) return;
            playerApi.SetPlayerTag(Utils.UserTag, "");
        }

        public override void OnContactEnter(ContactEnterInfo contactInfo)
        {
            if (playerApi.isLocal)
            {
                scanDeviceHandle.ToPass();
                teleportHandle.TeleportTo(TeleportPoint);
            }
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!player.isLocal) return;
            if (OwenerName == player.displayName)
            {
                Vector3 playerPos = player.GetPosition();
                Quaternion playerRot = player.GetRotation();
                Vector3 forwardDirection = playerRot * Vector3.forward;
                float distance = 0.5f;
                Vector3 spawnPosition = playerPos + (forwardDirection * distance);
                Quaternion spawnRotation = playerRot;
                var gs = gameObject.GetComponent<VRCObjectSync>();
                if (gs == null)
                {
                    return;
                }
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                gameObject.transform.position = spawnPosition;
                gameObject.transform.rotation = spawnRotation;
            }
        }
    }
}

