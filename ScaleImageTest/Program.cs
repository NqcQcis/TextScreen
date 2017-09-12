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
        public static string path = @"D:/test/";
        [STAThread]
        public static void Main()
        {                                                                             //сохраняем и делаем обоями
            //img = ScaleImage(img, screenSize.Width, screenSize.Height);
            
            FileInfo fileinfo = new FileInfo(Program.path + "log.txt");
            if (fileinfo.Exists)
            {
                File.Delete(path + "log.txt");
            }
            Rectangle screenSize = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            string NameImage = screenSize.Width.ToString() + "x" + screenSize.Height.ToString() + ".jpg";
            NameImage = path + NameImage;
            string userprofile = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
            string HostName = Environment.MachineName;
            int FontSize = screenSize.Height / 78;
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
            img.Save(userprofile + "/wallpaper.jpg");
            path = (userprofile + "/wallpaper.jpg");
            SetWallpaper(path, 1, 0);
        }

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo
            (int uAction, int uParam, string lpvParam, int fuWinIni);

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

        static Image ScaleImage(Image source, int width, int height)
        {

            Image dest = new Bitmap(width, height);
            using (Graphics gr = Graphics.FromImage(dest))
            {
                gr.FillRectangle(Brushes.White, 0, 0, width, height);  // Очищаем экран
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                float srcwidth = source.Width;
                float srcheight = source.Height;
                float dstwidth = width;
                float dstheight = height;

                if (srcwidth <= dstwidth && srcheight <= dstheight)  // Исходное изображение меньше целевого
                {
                    int left = (width - source.Width) / 2;
                    int top = (height - source.Height) / 2;
                    gr.DrawImage(source, left, top, source.Width, source.Height);
                }
                else if (srcwidth / srcheight > dstwidth / dstheight)  // Пропорции исходного изображения более широкие
                {
                    float cy = srcheight / srcwidth * dstwidth;
                    float top = ((float)dstheight - cy) / 2.0f;
                    if (top < 1.0f) top = 0;
                    gr.DrawImage(source, 0, top, dstwidth, cy);
                }
                else  // Пропорции исходного изображения более узкие
                {
                    float cx = srcwidth / srcheight * dstheight;
                    float left = ((float)dstwidth - cx) / 2.0f;
                    if (left < 1.0f) left = 0;
                    gr.DrawImage(source, left, 0, cx, dstheight);
                }

                return dest;
            }
        }

        public static Image AddText(string CompanyText, string Path, float SizeFont)
        {
            //InstalledFontCollection fonts = new InstalledFontCollection();
            //FontFamily[] families = fonts.Families;
            //int count = 0;
            //for (int i = 0; i < fonts.Families.Length; i++)
            //{
            //    Log(fonts.Families[i].Name.ToString() + " № " + i);
            //    if (fonts.Families[i].Name.ToString().Equals("DINPro-Regular"))
            //    {
            //        count = i;

            //    }

            //    else if (fonts.Families[i].Name.ToString().Equals("Arial"))
            //    {
            //        count = i;
            //    }

            //    else
            //    {

            //    }
            //}
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
            System.IO.FileStream fs = new System.IO.FileStream(Path, System.IO.FileMode.Open);
            System.Drawing.Image img = System.Drawing.Image.FromStream(fs); 
            fs.Close();
            int otstup = img.Width / 6 * 4;

            RectangleF rectf = new RectangleF(otstup,0, img.Width - otstup, img.Height);

            Graphics g = Graphics.FromImage(img);

            g.SmoothingMode = SmoothingMode.Default;
            //g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            //g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            StringFormat format = new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near
            };
            g.DrawString(CompanyText, fnt, Brushes.Green, rectf, format);

            g.Flush();

            g.Save();
            //img.Save(path, ImageFormat.Jpeg);
            return img;
        }

        static void Log(string message)
        {
            File.AppendAllText(Program.path + "log.txt", message + "\n");
        }
    }
}
