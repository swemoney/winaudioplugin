﻿namespace NotADoctor99.WinAudioPlugin
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;

    using Loupedeck;

    internal static class DeviceHelpers
    {
        public static String GetCommandDisplayName(AudioDevices devices, String deviceId) => devices.TryGetDevice(deviceId, out var device) ? device.LongDisplayName : deviceId;

        public static BitmapImage GetCommandImage(AudioDevices devices, String deviceId)
        {
            if (!devices.TryGetDevice(deviceId, out var device))
            {
                return null;
            }

            using (var bitmapBuilder = new BitmapBuilder(PluginImageSize.Width90))
            {
                bitmapBuilder.Clear(BitmapColor.Black);

                var inputTypeName = devices.GetDataFlow() == NAudio.CoreAudioApi.DataFlow.Render ? "Output" : "Input";

                var deviceTypeBytes = PluginResources.ReadBinaryFile($"Type{inputTypeName}.png");
                bitmapBuilder.DrawImage(deviceTypeBytes, 32, 4);

                if (device.SmallIcon != null)
                {
                    bitmapBuilder.DrawImage(device.SmallIcon, 60, 5);
                }

                if (device.IsDefault)
                {
                    var defaultCheckBytes = PluginResources.ReadBinaryFile("DefaultDevice.png");
                    bitmapBuilder.DrawImage(defaultCheckBytes, 4, 4);
                }

                bitmapBuilder.DrawText(device.ShortDisplayName, 0, 22, 80, 58);
                
                return bitmapBuilder.ToImage();
            }
        }

        public static Boolean ExtractIcon(String iconPath, out Byte[] largeIconBytes, out Byte[] smallIconBytes)
        {
            smallIconBytes = null;
            largeIconBytes = null;

            try
            {
                var parts = iconPath.Split(',');
                if ((parts.Length != 2) || !Int32.TryParse(parts[1], out var iconIndex))
                {
                    return false;
                }

                var iconFilePath = Environment.ExpandEnvironmentVariables(parts[0]);

                if (!File.Exists(iconFilePath))
                {
                    var systemDirectory = @"\windows\system32\";
                    if (iconFilePath.ContainsNoCase(systemDirectory))
                    {
                        var iconFilePath32 = iconFilePath.ReplaceNoCase(systemDirectory, @"\windows\sysnative\");
                        if (File.Exists(iconFilePath32))
                        {
                            iconFilePath = iconFilePath32;
                        }
                    }
                }

                if (UInt32.MaxValue == ExtractIconEx(iconFilePath, iconIndex, out var largeIconHandle, out var smallIconHandle, 1))
                {
                    PluginLog.Warning($"ExtractIconEx failed with error {Marshal.GetLastWin32Error()}");
                    return false;
                }

                largeIconBytes = GetIconBytes(largeIconHandle);
                smallIconBytes = GetIconBytes(smallIconHandle);

                DestroyIcon(largeIconHandle);
                DestroyIcon(smallIconHandle);

                return true;
            }
            catch (Exception ex)
            {
                PluginLog.Warning(ex, "Cannot extract icon");
            }

            return false;

            Byte[] GetIconBytes(IntPtr iconHandle)
            {
                var icon = Icon.FromHandle(iconHandle);

                using (var stream = new MemoryStream())
                {
                    using (var bitmap = icon?.ToBitmap())
                    {
                        if (null == bitmap)
                        {
                            return null;
                        }

                        bitmap.Save(stream, ImageFormat.Png);
                        return stream.ToArray();
                    }
                }
            }
        }

        [DllImport("Shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern UInt32 ExtractIconEx(String lpszFile, Int32 nIconIndex, out IntPtr phiconLarge, out IntPtr phiconSmall, UInt32 nIcons);

        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Boolean DestroyIcon(IntPtr hIcon);
    }
}
