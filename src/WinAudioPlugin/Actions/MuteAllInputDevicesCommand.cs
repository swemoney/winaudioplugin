namespace NotADoctor99.WinAudioPlugin
{
    using System;

    using Loupedeck;

    public class MuteAllInputDevicesCommand : PluginDynamicCommand
    {
        private readonly DictionaryNoCase<String> _deviceIds = new DictionaryNoCase<String>();
        private readonly DictionaryNoCase<String> _actionParameters = new DictionaryNoCase<String>();

        public MuteAllInputDevicesCommand()
            : base("Mute all input devices", "Mutes all available input devices", "")
        {
        }
        protected override void RunCommand(String actionParameter) => WinAudioPlugin.InputDevices.MuteAllDevices();

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) => PluginResources.ReadImage("MuteAllInputDevices.png");
    }
}
