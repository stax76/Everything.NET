
using System;
using System.Runtime.InteropServices;

namespace EverythingNET
{
	public class DarkMode
	{
        [DllImport("uxtheme.dll", EntryPoint = "#135")]
        public static extern int SetPreferredAppMode(int appMode);

		[DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
		public static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(
            IntPtr hwnd, int dwAttribute, ref IntPtr pvAttribute, int cbAttribute);

		public static void BeforeWindowCreation()
		{
            try {
			    int APPMODE_ALLOWDARK = 1;
			    SetPreferredAppMode(APPMODE_ALLOWDARK);
            } catch (Exception) {}
		}

		public static void AfterWindowCreation(IntPtr hWnd)
		{
			try {
				SetWindowTheme(hWnd, "DarkMode_Explorer", null);
				IntPtr dark = new IntPtr(1);
				DwmSetWindowAttribute(hWnd, 20, ref dark, Environment.Is64BitProcess ? 8 : 4);
			} catch (Exception) { }
		}
	}
}
