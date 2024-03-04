namespace NotADoctor99.WinAudioPlugin
{
    using System;

    using Loupedeck;

    public class DefaultInputDeviceCommand : PluginTwoStateDynamicCommand
    {
        private readonly DictionaryNoCase<String> _deviceIds = new DictionaryNoCase<String>();
        private readonly DictionaryNoCase<String> _actionParameters = new DictionaryNoCase<String>();

        public DefaultInputDeviceCommand()
        {
            this.GroupName = "Set Default Input Device";
            this.Description = "Sets this device as default input device";

            this.SetOffStateDisplayName("Non-default input device");
            this.SetOnStateDisplayName("Default input device");

            this.AddToggleCommand("Set default device").SetDescription(this.Description);
        }

        protected override Boolean OnLoad()
        {
            WinAudioPlugin.InputDevices.DeviceListChanged += this.OnDeviceListChanged;
            WinAudioPlugin.InputDevices.DefaultDeviceChanged += this.OnDefaultDeviceChanged;

            return true;
        }

        protected override Boolean OnUnload()
        {
            WinAudioPlugin.InputDevices.DeviceListChanged -= this.OnDeviceListChanged;
            WinAudioPlugin.InputDevices.DefaultDeviceChanged -= this.OnDefaultDeviceChanged;

            return true;
        }

        protected override void RunCommand(String actionParameter)
        {
            if (this.TryGetDeviceId(actionParameter, out var deviceId))
            {
                WinAudioPlugin.InputDevices.SetDefaultDevice(deviceId);
            }
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
            => this.TryGetDeviceId(actionParameter, out var deviceId) ? DeviceHelpers.GetCommandImage(WinAudioPlugin.InputDevices, deviceId) : null;

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => actionParameter;

        private Boolean TryGetDeviceId(String actionParameter, out String deviceId) => this._deviceIds.TryGetValueSafe(actionParameter, out deviceId);

        private void UpdateParameters()
        {
            this.RemoveAllParameters();

            foreach (var device in WinAudioPlugin.InputDevices.Devices)
            {
                this._deviceIds[device.LongDisplayName] = device.Id;
                this._actionParameters[device.Id] = device.LongDisplayName;
                this.AddParameter(device.LongDisplayName, device.LongDisplayName, this.GroupName);
                this.SetCurrentState(device.LongDisplayName, device.IsDefault ? 1 : 0);
            }

            this.ParametersChanged();
            this.ActionImageChanged(null);
        }

        private void OnDeviceListChanged(Object sender, AudioDevicesEventArgs e) => this.UpdateParameters();

        private void OnDefaultDeviceChanged(Object sender, AudioDefaultDeviceEventArgs e)
        {
            this.ActionImageChangedByDeviceId(e.OldDeviceId, false);
            this.ActionImageChangedByDeviceId(e.NewDeviceId, true);
        }

        private void ActionImageChangedByDeviceId(String deviceId, Boolean isDefault)
        {
            if (this._actionParameters.TryGetValueSafe(deviceId, out var actionParameter))
            {
                this.SetCurrentState(actionParameter, isDefault ? 1 : 0);
                this.ActionImageChanged(actionParameter);
            }
        }
    }
}
