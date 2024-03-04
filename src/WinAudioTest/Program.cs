namespace NotADoctor99.WinAudioPlugin
{
    using System;
    using System.Diagnostics;

    using NAudio.CoreAudioApi;

    internal class Program
    {
        static void Main(String[] _)
        {
            TestAudioDevices(DataFlow.Render);
            TestAudioDevices(DataFlow.Capture);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey(true);
        }

        static void TestAudioDevices(DataFlow dataFlow)
        {
            var audioDevices = new AudioDevices(dataFlow);
            audioDevices.DefaultDeviceChanged += OnDefaultAudioDeviceChanged;
            audioDevices.DeviceListChanged += AudioDeviceListChanged;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            audioDevices.Start();

            Console.WriteLine($"> Start {dataFlow} AudioDevices in {stopwatch.Elapsed.TotalMilliseconds:N0} ms");
            stopwatch.Stop();

            String device1Id = null;
            String device2Id = null;

            foreach (var device in audioDevices.EnumerateDevices())
            {
                Console.WriteLine($"{device.LongDisplayName} | {device.Id}");

                if (null == device1Id)
                {
                    device1Id = device.Id;
                }
                else
                {
                    device2Id = device2Id ?? device.Id;
                }
            }

            var defaultDevice = audioDevices.GetDefaultDevice();
            Console.WriteLine($"* {defaultDevice.LongDisplayName}");

            audioDevices.SetDefaultDevice(device1Id);
            System.Threading.Thread.Sleep(1_000);

            audioDevices.SetDefaultDevice(defaultDevice.Id);
            System.Threading.Thread.Sleep(1_000);

            audioDevices.SetDefaultDevice(device2Id);
            System.Threading.Thread.Sleep(1_000);

            audioDevices.SetDefaultDevice(defaultDevice.Id);
            System.Threading.Thread.Sleep(1_000);

            audioDevices.Stop();
            audioDevices.DefaultDeviceChanged -= OnDefaultAudioDeviceChanged;
            audioDevices.DeviceListChanged += AudioDeviceListChanged;
        }

        private static void OnDefaultAudioDeviceChanged(Object sender, AudioDefaultDeviceEventArgs e) => Console.WriteLine($"* {(sender as AudioDevices)?.GetDefaultDevice().LongDisplayName}");

        private static void AudioDeviceListChanged(Object sender, AudioDevicesEventArgs e) => Console.WriteLine($"? Device list changed");

    }
}
