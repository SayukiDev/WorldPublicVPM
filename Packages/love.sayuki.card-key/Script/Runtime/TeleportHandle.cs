using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace __Sayuki.CardKey.Script.Utils
{
    public class TeleportHandle : UdonSharpBehaviour
    {
        public CanvasGroup FadeCanvas;
        private Transform TeleportPoint;
        private bool isTeleporting;

        public void TeleportTo(Transform TeleportPoint)
        {
            if (isTeleporting) return;
            isTeleporting = true;
            this.TeleportPoint = TeleportPoint;
            var playerApi = Networking.LocalPlayer;
            playerApi.Immobilize(true);
            FadeCanvas.alpha = 0;
            FadeCanvas.gameObject.SetActive(true);
            VRCTween.TweenFade(FadeCanvas, 1, 2, VRCTweenEase.OutQuad).
                OnComplete(this, nameof(onFadeComplete));
        }

        public void onFadeComplete()
        {
            if (TeleportPoint == null)
            {
                Debug.LogError("TeleportPoint is null");
                VRCTween.TweenFade(FadeCanvas, 0, 2, VRCTweenEase.OutQuad).
                    OnComplete(this, nameof(onFadeOutComplete));
                return;
            }

            var playerApi = Networking.LocalPlayer;
            playerApi.TeleportTo(TeleportPoint.position, TeleportPoint.rotation);
            VRCTween.TweenFade(FadeCanvas, 0, 2, VRCTweenEase.OutQuad).
                OnComplete(this, nameof(onFadeOutComplete));
        }

        public void onFadeOutComplete()
        {
            var playerApi = Networking.LocalPlayer;
            FadeCanvas.gameObject.SetActive(false);
            playerApi.Immobilize(false);
            TeleportPoint = null;
            isTeleporting = false;
        }
    }
}