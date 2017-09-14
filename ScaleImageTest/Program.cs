using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace ScaleImageTest
{
    class Program
    {
        public static string path = @"\\mw.irk\disk\app\wallpaper\";

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
            Image img = AddText("НАША МИССИЯ: \n" +
"Мы заботимся о Вашем здоровье, доставляя Вам природную Байкальскую воду. \n" +
"\n" +
"НАШИ ЦЕННОСТИ: \n" +
"1.Открытость компании.Мы проводим политику открытости всех  производственных процессов для Покупателей  нашей продукции.Мы готовы отвечать на любые вопросы наших Покупателей и рады видеть их на нашем производстве. \n" +
"2.Ответственность.Мы обеспечиваем стабильный рост компании  силами наших людей, наделенных достаточными полномочиями и полностью принимающими ответственность на себя за свои результаты.Мы доводим до получения результата принятые нами решения.В нашем понимании Ответственность - это полноценное осознавание сотрудником компании последствий своего выбора.\n" +
"3.Работа в команде.Работая как единая команда, мы стремимся быть профессионалами, способными развиваться и вносить свой вклад в достижение целей нашей компании.Мы уважаем своих коллег, гарантируя им нашу полную поддержку.Мы стремимся помогать им и ценить их каждодневный вклад в общее дело всей компании.\n" +
"4.Объективность.Мы принимаем решения  в соответствии со стратегией и политикой нашей  компании, на основании проверенных фактов.Отсутствие достоверных фактов требует от нас дополнительных усилий по их выявлению.\n" +
"5.Поиск нового.В нашей повседневной работе, и в проектах мы ведем постоянный поиск новых решений, поощряя его и нацеливая на повышение надежности и результативности наших бизнес процессов.\n" +
"6.Лучшие  условия труда.Мы непрестанно стремимся улучшать рабочую обстановку, обеспечивая наилучшие и наиболее комфортные условия труда, образцовый порядок и чистоту.Это позволит нам работать в благоприятной, творческой и безопасной атмосфере.\n" +
"7.Эффективность и Производительность.Мы стремимся добиваться наших тактических и стратегических целей с упорством и энтузиазмом, постоянно помня об успехе, которого мы хотим достичь общими усилиями.Мы должны целесообразно и вдумчиво подходить к планированию и использованию операционных и стратегических ресурсов.\n" +
"\n" +
"\n" +
"Имя компьютера: " + HostName + 


"", NameImage, FontSize);
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
    }
}
