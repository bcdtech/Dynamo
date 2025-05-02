using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Dynamo.Utilities;
public static class ScreenHelper
{
    public static double ScreenScale;
    public static double MmToPixelsWidth;
    static ScreenHelper()
    {
        ScreenScale = ResetScreenScale();
        MmToPixelsWidth = MillimetersToPixelsWidth(1);
    }


    [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
    public static extern IntPtr ReleaseDC(
        IntPtr hWnd,
        IntPtr hDc
        );

    [DllImport("gdi32.dll")]
    public static extern int GetDeviceCaps(
        IntPtr hdc, // handle to DC
        int nIndex // index of capability
        );

    public static System.Drawing.Size GetPhysicalDisplaySize()
    {
        Graphics g = Graphics.FromHwnd(IntPtr.Zero);
        IntPtr desktop = g.GetHdc();
        int physicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.Desktopvertres);
        int physicalScreenWidth = GetDeviceCaps(desktop, (int)DeviceCap.Desktophorzres);
        ReleaseDC(IntPtr.Zero, desktop);
        g.Dispose();
        return new System.Drawing.Size(physicalScreenWidth, physicalScreenHeight);
    }

    public enum DeviceCap
    {
        Desktopvertres = 117,
        Desktophorzres = 118
    }

    public static double ResetScreenScale()
    {
        using (var g = Graphics.FromHwnd(IntPtr.Zero))
        {
            IntPtr desktop = g.GetHdc();
            int physicalScreenWidth = GetDeviceCaps(desktop, (int)DeviceCap.Desktophorzres);
            return physicalScreenWidth * 1.0000 / System.Windows.SystemParameters.PrimaryScreenWidth;
        }
    }

    //public static List<string> GetMonitorPnpDeviceId()
    //{
    //    List<string> rt = new List<string>();

    //    using (ManagementClass mc = new ManagementClass("Win32_DesktopMonitor"))
    //    {
    //        using (ManagementObjectCollection moc = mc.GetInstances())
    //        {
    //            foreach (var o in moc)
    //            {
    //                var each = (ManagementObject)o;
    //                object obj = each.Properties["PNPDeviceID"].Value;
    //                if (obj == null)
    //                    continue;

    //                rt.Add(each.Properties["PNPDeviceID"].Value.ToString());
    //            }
    //        }
    //    }

    //    return rt;
    //}

    //public static byte[] GetMonitorEdid(string monitorPnpDevId)
    //{
    //    return (byte[])Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Enum\" + monitorPnpDevId + @"\Device Parameters", "EDID", new byte[] { });
    //}

    ////获取显示器物理尺寸(cm)
    //public static SizeF GetMonitorPhysicalSize(string monitorPnpDevId)
    //{
    //    byte[] edid = GetMonitorEdid(monitorPnpDevId);
    //    if (edid.Length < 23)
    //        return SizeF.Empty;

    //    return new SizeF(edid[21], edid[22]);
    //}

    //通过屏显示器理尺寸转换为显示器大小(inch)
    public static float MonitorScaler(SizeF moniPhySize)
    {
        double mDSize = Math.Sqrt(Math.Pow(moniPhySize.Width, 2) + Math.Pow(moniPhySize.Height, 2)) / 2.54d;
        return (float)Math.Round(mDSize, 1);
    }

    public static double MillimetersToPixelsWidth(double length) //length是毫米，1厘米=10毫米
    {
        //System.Windows.Forms.Panel p = new System.Windows.Forms.Panel();
        System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
        IntPtr hdc = g.GetHdc();
        int width = GetDeviceCaps(hdc, 4);     // HORZRES 
        int pixels = GetDeviceCaps(hdc, 8);     // BITSPIXEL
        g.ReleaseHdc(hdc);
        return (((double)pixels / (double)width) * (double)length);
    }



    public static double MmToWidth(double length) //length是毫米，1厘米=10毫米
    {
        return MmToPixelsWidth * length / ScreenScale;
    }

    public static double WidthToMm(double length)
    {
        return ScreenScale * length / MmToPixelsWidth;
    }

    public static double CmToWidth(double length) //length是毫米，1厘米=10毫米
    {
        return MmToPixelsWidth * length * 10d / ScreenScale;
    }

    public static double WidthToCm(double length)
    {
        return ScreenScale * length / (MmToPixelsWidth * 10d);
    }

    public static double InchToWidth(double length) //length是英寸，1英寸=2.54cm
    {
        return MmToPixelsWidth * length * 10d * 2.54d / ScreenScale;
    }

    public static double WidthToInch(double length) //length是英寸，1英寸=2.54cm
    {
        return ScreenScale * length / (MmToPixelsWidth * 10d * 2.54d);
    }

}
