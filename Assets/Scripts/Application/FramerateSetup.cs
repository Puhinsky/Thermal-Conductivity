using QFSW.QC;
using UnityEngine;

namespace ApplicationLayer
{
    public class FramerateSetup : MonoBehaviour
    {
        private void Awake()
        {
            SetFrameRate(60);
        }

        [Command("set-fps")]
        public void SetFrameRate(int frameRate)
        {
            Application.targetFrameRate = frameRate;
        }
    }
}
