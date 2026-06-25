using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace love.sayuki.CardKey.Script.Utils
{
    public class TeleportHandle : UdonSharpBehaviour
    {
        public MeshRenderer FadeCanvas;
        public MeshRenderer TargetFadeCanvas;
        private Transform TeleportPoint;
        private bool isTeleporting;

        public void TeleportTo(Transform TeleportPoint)
        {
            gameObject.SetActive(true);
            if (isTeleporting) return;
            isTeleporting = true;
            this.TeleportPoint = TeleportPoint;
            var playerApi = Networking.LocalPlayer;
            playerApi.Immobilize(true);
            FadeCanvas.material.color = new Color(0, 0, 0, 0);
            FadeCanvas.gameObject.transform.position=Networking.LocalPlayer.GetPosition()
                                                     +new Vector3(0,Networking.LocalPlayer.GetAvatarEyeHeightAsMeters(),0);;
            FadeCanvas.gameObject.SetActive(true);
            VRCTween.TweenColor(FadeCanvas, new Color(0, 0, 0, 1), 2, VRCTweenEase.OutQuad).
                OnComplete(this, nameof(onFadeComplete));
        }

        public void onFadeComplete()
        {
            
            if (TeleportPoint == null)
            {
                Debug.LogError("TeleportPoint is null");
            }
            else
            {
                TargetFadeCanvas.material.color = new Color(0, 0, 0, 1);
                TargetFadeCanvas.gameObject.SetActive(true);
                TargetFadeCanvas.transform.position = TeleportPoint.position+new Vector3(0,Networking.LocalPlayer.GetAvatarEyeHeightAsMeters(),0);
                var playerApi = Networking.LocalPlayer;
                playerApi.TeleportTo(TeleportPoint.position, TeleportPoint.rotation);
                FadeCanvas.gameObject.SetActive(false);
            }
            SendCustomEventDelayedFrames(nameof(FadeComplete),1);
        }

        public void FadeComplete()
        {
            VRCTween.TweenColor(TargetFadeCanvas, new Color(0, 0, 0, 0), 2, VRCTweenEase.OutQuad).
                OnComplete(this, nameof(onFadeOutComplete));
        }

        public void onFadeOutComplete()
        {
            var playerApi = Networking.LocalPlayer;
            gameObject.SetActive(false);
            TargetFadeCanvas.gameObject.SetActive(false);
            playerApi.Immobilize(false);
            TeleportPoint = null;
            isTeleporting = false;
        }
    }
}