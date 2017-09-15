using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ScaleImageTest
{
    class Program
    {
        public static string path = Settings1.Default.path;

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo
            (int uAction, int uParam, string lpvParam, int fuWinIni);
        [STAThread]
        public static void Main()
        {   

            FileInfo fileinfo = new FileInfo(Program.path + "log.txt");
            if (fileinfo.Exists)
            {
                File.Delete(path + "log.txt");
            }
            Rectangle screenSize = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            string NameImage =path + screenSize.Width.ToString() + "x" + screenSize.Height.ToString() + ".bmp";
            string userprofile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string HostName = Environment.MachineName;
            int FontSize = screenSize.Height / 90;

            FileStream CompanyText;
            try
            {
                CompanyText = new FileStream(Program.path + "CompanyText.txt", FileMode.Open);
            }
            catch (FileNotFoundException exc)
            {
                Log("FileNotFoundException" + exc.Message);
                return;
            }
            catch (IndexOutOfRangeException exc)
            {
                Log("IndexOutOfRangeException" + exc.Message);
                return;
            }

            byte[] array = new byte[CompanyText.Length];
            CompanyText.Read(array, 0, array.Length);
            string text = System.Text.Encoding.UTF8.GetString(array);
            


            Image img = AddText(text + "\n\n\n\n" + "Имя компьютера: " + HostName + "\n", NameImage, FontSize);
            img.Save(userprofile + "/wallpaper.bmp");
            path = (userprofile + "/wallpaper.bmp");
            SetWallpaper(path, 1, 0);
        }


        public static void SetWallpaper (string path, int style, int tile)  // Вносим в раздел реестра информацию о расположении новых обоев и сразу же применяем настройки
        {
            try
            {
                RegistryKey CurrentUser = Registry.CurrentUser;
                RegistryKey cuControlPanel = CurrentUser.OpenSubKey("Control Panel");
                RegistryKey cuDesktop = cuControlPanel.OpenSubKey("Desktop", true);
                cuDesktop.SetValue("WallPaper", path);
                cuDesktop.SetValue("TileWallpaper", tile);
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            }
            catch(Exception ex)
            {
                Log("Ошибка при изменении реестра: " + ex.Message);
                return;
            }
        }

        public static Image AddText(string CompanyText, string Path, float SizeFont)
        {
            PrivateFontCollection pfc = new PrivateFontCollection();
            try
            {
                pfc.AddFontFile(Program.path + "DINPro-Regular.ttf");
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            Font fnt = new Font(pfc.Families[0], SizeFont, FontStyle.Regular);
            FileStream fs = new FileStream(Path, FileMode.Open);
            Image img = Image.FromStream(fs); 
            fs.Close();
            int otstup = img.Width / 10 * 6;

            RectangleF rectf = new RectangleF(otstup,0, img.Width - otstup, img.Height);

            Graphics g = Graphics.FromImage(img);
            Color brushColor = Color.FromArgb(255 / 100 * 50, 0, 0, 0);
            SolidBrush brush = new SolidBrush(brushColor);
            g.FillRectangle(brush, rectf);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            StringFormat format = new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near
            };
            g.DrawString(CompanyText, fnt, Brushes.White, rectf, format);

            g.Flush();

            g.Save();
            return img;
        }

        static void Log(string message)
        {
            File.AppendAllText(Program.path + "log.txt", message + "\n");
        }

        public static String ParseTag(string text)
        {
            string testArg = "<i>";
            int lenght = text.Length;
            for (int count = 0; count < text.Length; count++)
            {
            }
            

        }
    }
}
