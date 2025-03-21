using System;
using System.Runtime.InteropServices;

class Program
{
    // Импортируем необходимые функции из user32.dll
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

    // Константы для ChangeDisplaySettings
    private const int ENUM_CURRENT_SETTINGS = -1;
    private const int CDS_UPDATEREGISTRY = 0x01;
    private const int CDS_TEST = 0x02;
    private const int DISP_CHANGE_SUCCESSFUL = 0;
    private const int DISP_CHANGE_RESTART = 1;
    private const int DISP_CHANGE_FAILED = -1;

    // Структура DEVMODE
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct DEVMODE
    {
        public const int CCHDEVICENAME = 32;
        public const int CCHFORMNAME = 32;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        public string dmDeviceName;
        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;

        public int dmPositionX;
        public int dmPositionY;
        public int dmDisplayOrientation;
        public int dmDisplayFixedOutput;

        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
        public string dmFormName;
        public short dmLogPixels;
        public short dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;
        public int dmDisplayFlags;
        public int dmDisplayFrequency;
    }

    static void Main(string[] args)
    {
        // Укажите желаемое разрешение
        int width = 1280;
        int height = 720;

        // Изменение разрешения
        if (ChangeResolution(width, height))
        {
            Console.WriteLine($"Разрешение изменено на {width}x{height}.");
        }
        else
        {
            Console.WriteLine("Не удалось изменить разрешение.");
        }
    }

    private static bool ChangeResolution(int width, int height)
    {
        DEVMODE dm = GetDevMode();

        // Устанавливаем новые значения разрешения
        dm.dmPelsWidth = width;
        dm.dmPelsHeight = height;

        // Указываем, что мы изменяем ширину и высоту
        dm.dmFields = 0x80000 | 0x100000; // DM_PELSWIDTH | DM_PELSHEIGHT

        // Попробуйте изменить разрешение в тестовом режиме
        int result = ChangeDisplaySettings(ref dm, CDS_TEST);

        if (result == DISP_CHANGE_FAILED)
        {
            Console.WriteLine("Невозможно использовать указанное разрешение.");
            return false;
        }
        else
        {
            // Применить настройки
            result = ChangeDisplaySettings(ref dm, CDS_UPDATEREGISTRY);

            switch (result)
            {
                case DISP_CHANGE_SUCCESSFUL:
                    return true;
                case DISP_CHANGE_RESTART:
                    Console.WriteLine("Требуется перезагрузка системы.");
                    return false;
                default:
                    Console.WriteLine("Не удалось изменить разрешение. Код ошибки: " + result);
                    return false;
            }
        }
    }

    private static DEVMODE GetDevMode()
    {
        DEVMODE dm = new DEVMODE();
        dm.dmDeviceName = new string(new char[32]);
        dm.dmFormName = new string(new char[32]);
        dm.dmSize = (short)Marshal.SizeOf(dm);

        if (EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref dm))
        {
            return dm;
        }
        else
        {
            throw new Exception("Не удалось получить текущие настройки дисплея.");
        }
    }
}