using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace love.sayuki.CardKey.Script.Utils
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TeleportHandle : UdonSharpBehaviour
    {
        public MeshRenderer FadeCanvas;
        private Transform TeleportPoint;
        private bool isTeleporting;

        public void TeleportTo(VRCPlayerApi playerApi,Transform TeleportPoint)
        {
            if (isTeleporting) return;
            gameObject.SetActive(true);
            isTeleporting = true;
            this.TeleportPoint = TeleportPoint;
            playerApi.Immobilize(true);
            FadeCanvas.material.color = new Color(0, 0, 0, 0);
            FadeCanvas.gameObject.SetActive(true);
            VRCTween.TweenColor(FadeCanvas, new Color(0, 0, 0, 1), 2, VRCTweenEase.OutQuad).
                OnComplete(this, nameof(onFadeComplete));
        }

        void Update()
        {
            var td = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            FadeCanvas.transform.position = td.position;
            FadeCanvas.transform.rotation = td.rotation;
        }

        public void onFadeComplete()
        {
            
            if (TeleportPoint == null)
            {
                Debug.LogError("TeleportPoint is null");
            }
            else
            {
                /*TargetFadeCanvas.material.color = new Color(0, 0, 0, 1);
                TargetFadeCanvas.gameObject.SetActive(true);
                TargetFadeCanvas.transform.position = TeleportPoint.position+new Vector3(0,Networking.LocalPlayer.GetAvatarEyeHeightAsMeters(),0);*/
                var playerApi = Networking.LocalPlayer;
                playerApi.TeleportTo(TeleportPoint.position, TeleportPoint.rotation);
            }
            SendCustomEventDelayedFrames(nameof(FadeComplete),1);
        }

        public void FadeComplete()
        {
            VRCTween.TweenColor(FadeCanvas, new Color(0, 0, 0, 0), 2, VRCTweenEase.OutQuad).
                OnComplete(this, nameof(onFadeOutComplete));
        }

        public void onFadeOutComplete()
        {
            var playerApi = Networking.LocalPlayer;
            gameObject.SetActive(false);
            FadeCanvas.gameObject.SetActive(false);
            FadeCanvas.material.color = new Color(0, 0, 0, 0);
            playerApi.Immobilize(false);
            TeleportPoint = null;
            isTeleporting = false;
        }
    }
}