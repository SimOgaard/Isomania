using UnityEngine;

public class Video : ScriptableObject
{
    // on, off, borderless
    private FullScreenMode fullScreen;
    public FullScreenMode FullScreen
    {
        get => fullScreen;
        set
        {
            fullScreen = value;
            Screen.fullScreenMode = value;
        }
    }

    // on, off
    private bool vSync;
    public bool VSync
    {
        get => vSync;
        set
        {
            vSync = value;
            if (vSync)
            {
                QualitySettings.vSyncCount = 1;
                FrameRateCap = false;
            }
            else
            {
                QualitySettings.vSyncCount = 0;
            }
        }
    }

    private int[] targetFrameRates;
    // on, off
    private bool frameRateCap;
    public bool FrameRateCap
    {
        get => frameRateCap;
        set
        {
            frameRateCap = value;
            if (frameRateCap)
            {
                Application.targetFrameRate = -1;
                VSync = false;
            }
            else
            {
                Application.targetFrameRate = -1;
            }
        }
    }
}
