namespace NotADoctor99.WinAudioPlugin
{
    using System;

    using Loupedeck;

    public class WinAudioPlugin : Plugin
    {
        public override Boolean UsesApplicationApiOnly => true;

        public override Boolean HasNoApplication => true;

        public static AudioDevices OutputDevices { get; } = new AudioDevices(NAudio.CoreAudioApi.DataFlow.Render);

        public static AudioDevices InputDevices { get; } = new AudioDevices(NAudio.CoreAudioApi.DataFlow.Capture);

        public WinAudioPlugin()
        {
            PluginLog.Init(this.Log);
            PluginResources.Init(this.Assembly);
        }

        public override void Load() {
            Helpers.StartNewTask(() => WinAudioPlugin.OutputDevices.Start());
            Helpers.StartNewTask(() => WinAudioPlugin.InputDevices.Start());
        }

        public override void Unload() {
            WinAudioPlugin.OutputDevices.Stop();
            WinAudioPlugin.InputDevices.Stop();
        }
    }
}
