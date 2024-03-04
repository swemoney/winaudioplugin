namespace NotADoctor99.WinAudioPlugin
{
    using System;

    public class AudioDefaultDeviceEventArgs : EventArgs
    {

        public String OldDeviceId;

        public String NewDeviceId;

        public AudioDefaultDeviceEventArgs(String oldDeviceId, String newDeviceId)
        {
            this.OldDeviceId = oldDeviceId;
            this.NewDeviceId = newDeviceId;
        }
    }
}
