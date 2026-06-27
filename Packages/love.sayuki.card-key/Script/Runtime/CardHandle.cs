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
        public Transform TeleportPoint;
        public bool FollowToPlayer;
        public GameObject[] toActivate;
        public GameObject[] toDeactivate;
        public TextMeshPro CardText;
        public TeleportHandle teleportHandle;
        public ScanDeviceHandle scanDeviceHandle;
        private VRCPlayerApi playerApi;
        [UdonSynced] private bool Lock;
        [UdonSynced] private string OwenerName;

        private void Start()
        {
            CardText.text = CardID.ToString();
            foreach (var t in toActivate)
            {
                t.SetActive(false);
            }
            foreach (var t in toDeactivate)
            {
                t.SetActive(true);
            }
            gameObject.transform.SetParent(null,true);
        }

        public override void OnPickup()
        {
            var p = Networking.GetOwner(gameObject);
            if (p == null) return;
            if (p.isLocal)
            {
                if (Lock &&
                    OwenerName != "" &&
                    OwenerName != Networking.LocalPlayer.displayName)
                {
                    gameObject.GetComponent<VRC_Pickup>().Drop();
                    return;
                }

                if (Lock)
                {
                    Lock = false;
                    RequestSerialization();
                }

                if (OwenerName == "" && FollowToPlayer)
                {
                    SendCustomEventDelayedSeconds("CheckDistance", 5);
                }

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
            if (!playerApi.isLocal)
            {
                return;
            }

            scanDeviceHandle.ToPass();
            foreach (var t in toActivate)
            {
                t.SetActive(true);
            }

            foreach (var a in toActivate)
            {
                a.SetActive(true);
            }
            teleportHandle.TeleportTo(playerApi,TeleportPoint);
            SendCustomEventDelayedSeconds("ToDeactivate", 10);
        }

        public void ToDeactivate()
        {
            foreach (var t in toDeactivate)
            {
                t.SetActive(false);
            }
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!player.isLocal) return;
            if (OwenerName == player.displayName)
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                if (!Lock)
                {
                    Lock = true;
                }

                RespawnToPlayer(player, true);
            }
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
            if (OwenerName != player.displayName)
            {
                return;
            }

            Lock = true;
            RequestSerialization();
        }

        private void RespawnToPlayer(VRCPlayerApi player, bool forward = false)
        {
            Vector3 playerPos = player.GetPosition();
            Quaternion playerRot = player.GetRotation();
            Vector3 direction;
            Quaternion spawnRotation = playerRot;
            if (forward)
            {
                direction = playerRot * Vector3.forward;
            }
            else
            {
                direction = playerRot * Vector3.back;
                spawnRotation = Quaternion.Euler(0, 180, 0);
            }

            float distance = 0.5f;
            Vector3 spawnPosition = playerPos + (direction * distance);
            var gs = gameObject.GetComponent<VRCObjectSync>();
            if (gs == null)
            {
                return;
            }

            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            float h = (float)(player.GetAvatarEyeHeightAsMeters() - 0.1);
            if (h < 0) h = 0;
            spawnPosition.y = h;
            gameObject.transform.position = spawnPosition;
            gameObject.transform.rotation = spawnRotation;
            gs.TeleportTo(gameObject.transform);
        }

        public void CheckDistance()
        {
            if (Networking.LocalPlayer.displayName != OwenerName)
            {
                return;
            }

            if (Vector3.Distance(Networking.LocalPlayer.GetPosition(), gameObject.transform.position) > 3)
            {
                if (Networking.LocalPlayer.GetVelocity().magnitude > 0.1f)
                {
                    SendCustomEventDelayedSeconds("CheckDistance", 5);
                    return;
                }
                RespawnToPlayer(Networking.LocalPlayer, false);
            }

            SendCustomEventDelayedSeconds("CheckDistance", 5);
        }
    }
}