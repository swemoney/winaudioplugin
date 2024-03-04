namespace NotADoctor99.WinAudioPlugin
{
    using System;
    using System.Collections.Generic;

    using Loupedeck;

    public class DefaultInputDeviceControlCenter : PluginDynamicFolder
    {
        public DefaultInputDeviceControlCenter()
        {
            this.DisplayName = "Change Default Input Device";
            this.Description = "Sets any available device as default input device";
        }

        public override PluginDynamicFolderNavigation GetNavigationArea(DeviceType _) => PluginDynamicFolderNavigation.ButtonArea;

        public override Boolean Load()
        {
            WinAudioPlugin.InputDevices.DefaultDeviceChanged += this.OnDefaultDeviceChanged;

            return true;
        }

        public override Boolean Unload()
        {
            WinAudioPlugin.InputDevices.DefaultDeviceChanged -= this.OnDefaultDeviceChanged;

            return true;
        }

        public override BitmapImage GetButtonImage(PluginImageSize imageSize) => PluginResources.ReadImage("ChangeDefaultInputDevice.png");

        public override IEnumerable<String> GetButtonPressActionNames(DeviceType deviceType)
        {
            foreach (var device in WinAudioPlugin.InputDevices.EnumerateDevices())
            {
                yield return this.CreateCommandName(device.Id);
            }
        }

        public override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) => DeviceHelpers.GetCommandImage(WinAudioPlugin.InputDevices, actionParameter);

        public override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => DeviceHelpers.GetCommandDisplayName(WinAudioPlugin.InputDevices, actionParameter);

        public override void RunCommand(String actionParameter) => WinAudioPlugin.OutputDevices.SetDefaultDevice(actionParameter);

        private void OnDefaultDeviceChanged(Object sender, AudioDefaultDeviceEventArgs e)
        {
            this.CommandImageChanged(e.OldDeviceId);
            this.CommandImageChanged(e.NewDeviceId);
        }
    }
}

