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
        public override void Interact()
        {
            teleportHandle.TeleportTo(teleportPoint);
        }
    }
}