using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using HIDCtrl;
using Drivers;
using Mouse_Sender_Abs;
using System.ComponentModel;
using System.Diagnostics;
using System.Hooks;
using Gma.System.MouseKeyHook;
using KeyboardUtils;
using WindowsInput;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static App.MIDIReaderFile;
using System.Threading;

namespace App
{

    //create the HIDController object
    public partial class Form1 : Form
    {

        private HIDController HID = new HIDController();
        public static int tempo;
        public List<Note> composit;
        //private HIDController HID2 = new HIDController();
        private uint FTimeout = 5000;
        private KbUtils KUtils = new KbUtils();
        private string filePath;
        private const int keyoffset = 0;
        private static TextBox logger;
        private int speed;
        private static Thread mainThread;
        private Thread playerThread;
        public void SetText(String s) { tbLog.AppendText(s); }

        public Form1()
        {
            InitializeComponent();
            //ExitOnPlayHelper h = new ExitOnPlayHelper();
            //Thread myThread = new Thread(new ThreadStart(h.Subscribe));
            logger = tbLog;
            speed = 100;
            mainThread = Thread.CurrentThread;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            x = 16384;
            y = 16384;
            keys[0] = new Key(15976, 16237); keys[1] = new Key(16009, 16231); keys[2] = new Key(15979, 16243); keys[3] = new Key(15997, 16246); keys[4] = new Key(16015, 16243); keys[5] = new Key(15994, 16255); keys[6] = new Key(16033, 16246); keys[7] = new Key(16003, 16261); keys[8] = new Key(16021, 16264); keys[9] = new Key(16045, 16258); keys[10] = new Key(16027, 16273); keys[11] = new Key(16057, 16264); keys[12] = new Key(16039, 16279); keys[13] = new Key(16069, 16270); keys[14] = new Key(16054, 16285); keys[15] = new Key(16069, 16291); keys[16] = new Key(16093, 16285); keys[17] = new Key(16081, 16300); keys[18] = new Key(16111, 16288); keys[19] = new Key(16096, 16306); keys[20] = new Key(16114, 16315); keys[21] = new Key(16141, 16303); keys[22] = new Key(16132, 16321); keys[23] = new Key(16162, 16309); keys[24] = new Key(16153, 16327); keys[25] = new Key(16180, 16315); keys[26] = new Key(16174, 16333); keys[27] = new Key(16198, 16339); keys[28] = new Key(16225, 16324); keys[29] = new Key(16222, 16345); keys[30] = new Key(16249, 16330); keys[31] = new Key(16246, 16354); keys[32] = new Key(16273, 16354); keys[33] = new Key(16300, 16336); keys[34] = new Key(16300, 16360); keys[35] = new Key(16324, 16339); keys[36] = new Key(16330, 16366); keys[37] = new Key(16351, 16342); keys[38] = new Key(16360, 16366); keys[39] = new Key(16390, 16366); keys[40] = new Key(16405, 16339); keys[41] = new Key(16420, 16363); keys[42] = new Key(16432, 16339); keys[43] = new Key(16450, 16360); keys[44] = new Key(16480, 16360); keys[45] = new Key(16486, 16333); keys[46] = new Key(16507, 16354); keys[47] = new Key(16507, 16330); keys[48] = new Key(16534, 16351); keys[49] = new Key(16534, 16327); keys[50] = new Key(16555, 16342); keys[51] = new Key(16585, 16342); keys[52] = new Key(16576, 16315); keys[53] = new Key(16603, 16330); keys[54] = new Key(16597, 16312); keys[55] = new Key(16621, 16321); keys[56] = new Key(16645, 16318); keys[57] = new Key(16633, 16300); keys[58] = new Key(16657, 16309); keys[59] = new Key(16651, 16294); keys[60] = new Key(16675, 16303); keys[61] = new Key(16663, 16285); keys[62] = new Key(16696, 16297); keys[63] = new Key(16708, 16288); keys[64] = new Key(16693, 16273); keys[65] = new Key(16720, 16282); keys[66] = new Key(16705, 16267); keys[67] = new Key(16732, 16276); keys[68] = new Key(16738, 16267); keys[69] = new Key(16729, 16255); keys[70] = new Key(16750, 16261); keys[71] = new Key(16738, 16249); keys[72] = new Key(16759, 16255); keys[73] = new Key(16750, 16246); keys[74] = new Key(16765, 16249); keys[75] = new Key(16774, 16243); keys[76] = new Key(16765, 16234); keys[77] = new Key(16792, 16240); keys[78] = new Key(16768, 16228); keys[79] = new Key(16792, 16234); keys[80] = new Key(16807, 16231); keys[81] = new Key(16786, 16219); keys[82] = new Key(16810, 16225); keys[83] = new Key(16792, 16216); keys[84] = new Key(16810, 16219); keys[85] = new Key(16801, 16213); keys[86] = new Key(16819, 16216); keys[87] = new Key(16819, 16210);
            tbLog.AppendText("\r\n");
            //create the HIDController 
            HID.OnLog += new EventHandler<LogArgs>(Log);
            HID.VendorID = (ushort)DriversConst.TTC_VENDORID;                //the Tetherscript vendorid
            HID.ProductID = (ushort)DriversConst.TTC_PRODUCTID_MOUSEABS;     //the Tetherscript Virtual Mouse Absolute Driver productid
            HID.Connect();
            //hl = Process.GetProcessesByName("hl")[0];
            Subscribe();
            record = false;
            //Script();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            HID.Disconnect();
            Unsubscribe();
        }

        public void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {

                openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;

                }
            }
            tbLog.Text = "Zagryjayou midi, jdi";
            MIDIReaderFile.richTextBox1 = tbLog;
            MIDIReaderFile.openMIDIFile(filePath);
            composit = MIDIReaderFile.notes;
            tbLog.Text = (MIDIReaderFile.HeaderMIDIStruct.lengthSection + " - len. ");
            tbLog.AppendText(MIDIReaderFile.HeaderMIDIStruct.settingTime + " - timeSettings. ");
            tbLog.AppendText(composit.Count + " - notes count. ");
            tbLog.Text += MIDIReaderFile.pizda ? "Pizda. (proizowla owibka pri s4itivanii) " : "Done. ";
        }

        struct Key
        {

            public UInt16 x;
            public UInt16 y;
            public Key(UInt16 x, UInt16 y)
            {
                this.x = x;
                this.y = y;
            }
        }

        private Key[] keys = new Key[88];


        public void button2_Click(object sender, EventArgs e)
        {
            playerThread = new Thread(new ThreadStart(Play));
            playerThread.Start();
        }

        public void Play ()
        {
            if (composit == null || composit.Count == 0) return;
            Sleep(5000);
            Send_Data_To_MouseAbs(16384, 16384);
            Sleep(5000);
            int time = 0;
            int emptycount = 0;
            int accordLength = 0;
            while (composit.Count != 0)
            {
                int notescount = 0;
                while (composit.Count != 0 && composit[0].StartTime == time)
                {
                    if (composit[0].note >= 0 && composit[0].note <= 87)
                    {
                        notescount++;
                        accordLength += 24;
                        Key k = keys[composit[0].note];
                        Send_Data_To_MouseAbs(k.x, k.y);
                        Sleep(12);
                        Click();
                        Sleep(12);
                        emptycount = 0;
                    }
                    composit.RemoveAt(0);
                }
                if (notescount == 0) emptycount++;
                if (emptycount >= 5)
                {
                    Sleep(Math.Max(3000 / (int)(HeaderMIDIStruct.settingTime * speed / 100) - accordLength, 0));
                    accordLength = Math.Max(0, accordLength - 3000 / (int)(HeaderMIDIStruct.settingTime * speed / 100));
                    emptycount = 0;
                }
                time++;
            }
        }

        public void Log(object s, LogArgs e)
        {
            tbLog.AppendText(e.Msg + "\r\n");
        }

        public void btnSend_Click(object sender, EventArgs e)
        {
            tbLog.AppendText("Sending mouse abs coords and button states...\r\n");
            Send_Data_To_MouseAbs();
            tmrRelease.Enabled = true; //this will release the mouse buttons if they are pressed
        }

        //here we send data to the mouse
        void Send_Data_To_MouseAbs()
        {
            SetFeatureMouseAbs MouseAbsData = new SetFeatureMouseAbs();
            MouseAbsData.ReportID = 1;
            MouseAbsData.CommandCode = 2;
            byte btns = 0;
            if (false) { btns = 1; };
            if (false) { btns = (byte)(btns | (1 << 1)); }
            if (false) { btns = (byte)(btns | (1 << 2)); }
            MouseAbsData.Buttons = btns;  //button states are represented by the 3 least significant bits
            MouseAbsData.X = x;
            MouseAbsData.Y = y;
            //convert struct to buffer
            byte[] buf = getBytesSFJ(MouseAbsData, Marshal.SizeOf(MouseAbsData));
            //send filled buffer to driver
            HID.SendData(buf, (uint)Marshal.SizeOf(MouseAbsData));
        }

        public void Send_Data_To_MouseAbs(UInt16 x, UInt16 y)
        {
            SetFeatureMouseAbs MouseAbsData = new SetFeatureMouseAbs();
            MouseAbsData.ReportID = 1;
            MouseAbsData.CommandCode = 2;
            byte btns = 0;
            if (false) { btns = 1; };
            if (false) { btns = (byte)(btns | (1 << 1)); }
            if (false) { btns = (byte)(btns | (1 << 2)); }
            MouseAbsData.Buttons = btns;  //button states are represented by the 3 least significant bits
            this.x = x;
            this.y = y;
            MouseAbsData.X = x;
            MouseAbsData.Y = y;
            //convert struct to buffer
            byte[] buf = getBytesSFJ(MouseAbsData, Marshal.SizeOf(MouseAbsData));
            //send filled buffer to driver
            HID.SendData(buf, (uint)Marshal.SizeOf(MouseAbsData));
        }

        //for converting a struct to byte array
        public byte[] getBytesSFJ(SetFeatureMouseAbs sfj, int size)
        {
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(sfj, ptr, false);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        private void tmrRelease_Tick(object sender, EventArgs e)
        {
            tmrRelease.Enabled = false;
            tbLog.AppendText("Releasing buttons...\r\n");
            //cbLeft.Checked = false;
            //cbRight.Checked = false;
            //cbMiddle.Checked = false;
            Send_Data_To_MouseAbs();
        }

        public void Sleep(int t)
        {
            System.Threading.Thread.Sleep(t);
        }

        private void Script()
        {
            new Script(this);
            Send_Data_To_MouseAbs();
        }

        void Send(Byte Modifier, Byte Padding, Byte Key0, Byte Key1, Byte Key2, Byte Key3, Byte Key4, Byte Key5)
        {
            SetFeatureKeyboard KeyboardData = new SetFeatureKeyboard();
            KeyboardData.ReportID = 1;
            KeyboardData.CommandCode = 2;
            KeyboardData.Timeout = FTimeout / 5; //5 because we count in blocks of 5 in the driver
            KeyboardData.Modifier = Modifier;
            //padding should always be zero.
            KeyboardData.Padding = Padding;
            KeyboardData.Key0 = Key0;
            KeyboardData.Key1 = Key1;
            KeyboardData.Key2 = Key2;
            KeyboardData.Key3 = Key3;
            KeyboardData.Key4 = Key4;
            KeyboardData.Key5 = Key5;
            //convert struct to buffer
            byte[] buf = getBytesSFJ(KeyboardData, Marshal.SizeOf(KeyboardData));
            //send filled buffer to driver
        }

        public byte[] getBytesSFJ(SetFeatureKeyboard sfj, int size)
        {
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(sfj, ptr, false);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        const UInt32 WM_KEYDOWN = 0x0100;
        const int VK_E = 0x74;

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);
        InputSimulator iss = new InputSimulator();
        new public void Click()
        {
            Sleep(1);
            iss.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.VK_E);
            iss.Mouse.LeftButtonClick();
        }

        private IKeyboardMouseEvents m_GlobalHook;

        public void Subscribe()
        {
            // Note: for the application hook, use the Hook.AppEvents() instead
            m_GlobalHook = Hook.GlobalEvents();

            //m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
            m_GlobalHook.KeyPress += GlobalHookKeyPress;
        }
        bool record;
        private void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
        {
            if (record)
            {
                switch (e.KeyChar)
                {
                    case '8':
                        y -= 3;
                        break;
                    case '2':
                        y += 3;
                        break;
                    case '6':
                        x += 3;
                        break;
                    case '4':
                        x -= 3;
                        break;
                    case 'g':
                        AddKey();
                        break;
                    case 'f':
                        x = 16384;
                        y = 16384;
                        break;
                    case 'q':
                        Send_Data_To_MouseAbs(15976, 16237);
                        x = 16384;
                        y = 16384;
                        Send_Data_To_MouseAbs();
                        Click();
                        break;
                }
                Send_Data_To_MouseAbs(x, y);
            }
            else
            {
                switch (e.KeyChar)
                {
                    case 'x':
                        if (playerThread != null)
                        {
                            if (playerThread.ThreadState != System.Threading.ThreadState.Suspended)
                            {
                                tbLog.Text = "Pauza.";
                                playerThread.Suspend();
                            }
                            else
                            {
                                playerThread.Resume();
                                tbLog.Text = "Igraem.";
                            }
                        }
                        break;
                }
            }
        }

        private void AddKey()
        {
            tbLog.AppendText("keys[" + i + keyoffset + "] = new Key(" + x + "," + y + ");");
            i++;
        }

        public void Unsubscribe()
        {
            //m_GlobalHook.MouseDownExt -= GlobalHookMouseDownExt;
            m_GlobalHook.KeyPress -= GlobalHookKeyPress;

            //It is recommened to dispose it
            m_GlobalHook.Dispose();
        }

        int i = 0;
        UInt16 x = 0;
        UInt16 y = 0;

        private void txtBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46))
            {
                e.Handled = true;
                return;
            }

            if (e.KeyChar == 46)
            {
                if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                    e.Handled = true;
            }
        }

        private void txtBox1_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(txtBox1.Text, out speed);
            tbLog.Text = speed.ToString();
        }
    }



    #region https://github.com/Vadimatorik/midi_to_k1986be92qi

    public class MIDIReaderFile
    {
        public static List<Note> notes;
        public static TextBox richTextBox1;
        public BinaryReader BinaryReaderMIDIFile;   // Создаем поток. На его основе будем работать с MIDI файлом.
        public MIDIReaderFile(Stream input) // В конструкторе инициализируем байтовый поток на основе открытого потока.
        {
            BinaryReaderMIDIFile = new BinaryReader(input); // Открываем поток для чтения по байтам на основе открытого потока файла.
        }

        public UInt32 ReadUInt32BigEndian() // Считываем 4 байта в формате "от старшего к младшему" и располагаем их в переменной.
        {
            if (pizda) return 0;
            UInt32 bufferData = 0;  // Начальное значени = 0.
            for (int IndexByte = 3; IndexByte >= 0; IndexByte--)    // Счетчик от старшего к младшему.
                bufferData |= (UInt32)((UInt32)BinaryReaderMIDIFile.ReadByte()) << 8 * IndexByte;   // Располагаем значения. 
            return bufferData;
        }

        public UInt16 ReadUInt16BigEndian() // Считываем 2 байта в формате "от старшего к младшему" и располагаем их в переменной.
        {
            UInt16 bufferData = 0;  // Начальное значени = 0.
            for (int IndexByte = 1; IndexByte >= 0; IndexByte--)    // Счетчик от старшего к младшему.
                bufferData |= (UInt16)((UInt16)BinaryReaderMIDIFile.ReadByte() << 8 * IndexByte);   // Располагаем значения. 
            return bufferData;
        }

        public string ReadStringOf4byte()   // Получаем из файла строку в 4 элемента.
        {
            return Encoding.Default.GetString(BinaryReaderMIDIFile.ReadBytes(4));   // Достаем 4 байта и преобразовываем их в стоку из 4-х символов.
        }

        public byte ReadByte()  // Считываем 1 байт.
        {
            try
            {
                pizda = false;
                return BinaryReaderMIDIFile.ReadByte();
            }
            catch (IOException ex)
            {
                richTextBox1.Text = ex.Message;
                pizda = true;
                return 0;
            }
        }

        public byte[] ReadBytes(int count)  // Считываем count байт.
        {
            return BinaryReaderMIDIFile.ReadBytes(count);
        }

        // Назначение: разбор главной структуры MIDI файла.
        // Параметры: Открытый FileStream поток.
        // Возвращаемой значение - заполненная структура типа MIDIheaderStruct.
        public MIDIheaderStruct CopyHeaderOfMIDIFile(MIDIReaderFile MIDIFile)
        {
            MIDIheaderStruct ST = new MIDIheaderStruct(); // Создаем пустую структуру заголовка файла.
            ST.nameSection = MIDIFile.ReadStringOf4byte(); // Копируем имя раздела. 
            ST.lengthSection = MIDIFile.ReadUInt32BigEndian(); // Считываем 4 байта длины блока. Должно в итоге быть 0x6
            ST.mode = MIDIFile.ReadUInt16BigEndian(); // Считываем 2 байта режима MIDI. Должно быть 0, 1 или 2.
            ST.channels = MIDIFile.ReadUInt16BigEndian(); // Считываем 2 байта количество каналов в MIDI файле. 
            ST.settingTime = MIDIFile.ReadUInt16BigEndian(); // Считываем 2 байта параметров тактирования.
            return ST; // Возвращаем заполненную структуру.
        }

        public struct MIDIheaderStruct
        {
            public string nameSection; // Имя раздела. Должно быть "MThd".
            public UInt32 lengthSection; // Длинна блока, 4 байта. Должно быть 0x6;
            public UInt16 mode; // Режим MIDI файла: 0, 1 или 2. 
            public UInt16 channels; // Количество каналов. 
            public UInt16 settingTime;  // Параметры тактирования.
        }

        public static MIDIheaderStruct HeaderMIDIStruct;

        public static bool openMIDIFile(string pathToFile)
        {
            FileStream fileStream = new FileStream(pathToFile, FileMode.Open, FileAccess.Read);  // Открываем файл только для чтения.
            MIDIReaderFile MIDIFile = new MIDIReaderFile(fileStream);                            // Собственный поток для работы с MIDI файлом со спец. функциями. На основе байтового потока открытого файла.
            HeaderMIDIStruct = MIDIFile.CopyHeaderOfMIDIFile(MIDIFile);                  // Считываем заголовок.
            MIDIMTrkStruct[] MTrkStruct = new MIDIMTrkStruct[HeaderMIDIStruct.channels];         // Определяем массив для MTrkStruct.
            richTextBox1.Text += "Количество блоков: " + HeaderMIDIStruct.channels.ToString() + "\n"; // Количество каналов.
            richTextBox1.Text += "Параметры времени: " + HeaderMIDIStruct.settingTime.ToString() + "\n";
            richTextBox1.Text += "Формат MIDI: " + HeaderMIDIStruct.mode.ToString() + "\n";
            for (int loop = 0; loop < HeaderMIDIStruct.channels; loop++)
                MTrkStruct[loop] = MIDIFile.CopyMIDIMTrkSection(MIDIFile);                                // Читаем блоки MIDI файла.
            MIDIFile.outData(MIDIFile.СreateNotesArray(MTrkStruct, HeaderMIDIStruct.channels));                    // Получаем список нота/длительность.
            return true;
        }

        public struct MIDIMTrkStruct
        {
            public string nameSection; // Имя раздела. Должно быть "MTrk".
            public UInt32 lengthSection; // Длинна блока, 4 байта.
            public ArrayList arrayNoteStruct; // Динамический массив с нотами и их изменениями.
        }

        // Назначение: хранить события нажатия/отпускания клавиши или смены ее громкости.
        public struct noteStruct
        {
            public byte roomNotes;                    // Номер ноты.
            public UInt32 noteTime;                     // Длительность ноты время обсалютное. 
            public byte dynamicsNote;                 // Динамика взятия/отпускания ноты.
            public byte channelNote;                  // Канал ноты.
            public bool flagNote;                     // Взятие ноты (true) или отпускание ноты (false).    
        }



        // Назначение: копирование блока MTrk (блок с событиями) из MIDI файла.
        // Параметры: поток для чтения MIDI файла.
        // Возвращает: структуру блока с массивом структур событий.
        public static bool pizda { private set; get; }
        public MIDIMTrkStruct CopyMIDIMTrkSection(MIDIReaderFile MIDIFile)
        {
            MIDIMTrkStruct ST = new MIDIMTrkStruct(); // Создаем пустую структуру блока MIDI файла. 
            ST.arrayNoteStruct = new ArrayList(); // Создаем в структуре блока динамический массив структур событий клавиш.
            noteStruct bufferSTNote = new noteStruct(); // Создаем запись о новой ноте (буферная структура, будем класть ее в arrayNoteStruct).
            ST.nameSection = MIDIFile.ReadStringOf4byte(); // Копируем имя раздела. 
            ST.lengthSection = MIDIFile.ReadUInt32BigEndian(); // 4 байта длинны всего блока.
            UInt32 LoopIndex = ST.lengthSection; // Копируем колличество оставшихся ячеек. Будем считывать события, пока счетчик не будет = 0.
            UInt32 realTime = 0; // Реальное время внутри блока.
            while (LoopIndex != 0 && !pizda) // Пока не считаем все события.
            {
                // Время описывается плавающим числом байт. Конечный байт не имеет 8-го разрядка справа (самого старшего).
                byte loopСount = 0; // Колличество считанных байт.
                byte buffer; // Сюда кладем считанное значение.
                UInt32 bufferTime = 0; // Считанное время помещаем сюда.
                do
                {
                    buffer = MIDIFile.ReadByte(); // Читаем значение.
                    if (pizda) break;
                    loopСount++; // Показываем, что считали байт.
                    bufferTime <<= 7; // Сдвигаем на 7 байт влево существующее значенеи времени (Т.к. 1 старший байт не используется).
                    bufferTime |= (byte)(buffer & (0x7F)); // На сдвинутый участок накладываем существующее время.
                } while ((buffer & (1 << 7)) != 0); // Выходим, как только прочитаем последний байт времени (старший бит = 0).
                //if (pizda) break;
                realTime += bufferTime; // Получаем реальное время.

                buffer = MIDIFile.ReadByte(); loopСount++; // Считываем статус-байт, показываем, что считали байт. 
                // Если у нас мета-события, то...
                if (buffer == 0xFF)
                {
                    buffer = MIDIFile.ReadByte(); // Считываем номер мета-события.
                    buffer = MIDIFile.ReadByte(); // Считываем длину.
                    loopСount += 2;
                    for (int loop = 0; loop < buffer; loop++)
                        MIDIFile.ReadByte();
                    LoopIndex = LoopIndex - loopСount - buffer; // Отнимаем от счетчика длинну считанного.   
                }

                // Если не мета-событие, то смотрим, является ли событие событием первого уровня.
                else switch ((byte)buffer & 0xF0) // Смотрим по старшым 4-м байтам.
                    {
                        // Перебираем события первого уровня.

                        case 0x80: // Снять клавишу.
                            bufferSTNote.channelNote = (byte)(buffer & 0x0F); // Копируем номер канала.
                            bufferSTNote.flagNote = false; // Мы отпускаем клавишу.
                            bufferSTNote.roomNotes = MIDIFile.ReadByte(); // Копируем номер ноты.
                            bufferSTNote.dynamicsNote = MIDIFile.ReadByte(); // Копируем динамику ноты.
                            bufferSTNote.noteTime = realTime; // Присваеваем реальное время ноты.
                            ST.arrayNoteStruct.Add(bufferSTNote); // Сохраняем новую структуру.
                            LoopIndex = LoopIndex - loopСount - 2; // Отнимаем прочитанное. 
                            break;
                        case 0x90:   // Нажать клавишу.
                            bufferSTNote.channelNote = (byte)(buffer & 0x0F); // Копируем номер канала.
                            bufferSTNote.flagNote = true; // Мы нажимаем.
                            bufferSTNote.roomNotes = MIDIFile.ReadByte(); // Копируем номер ноты.
                            bufferSTNote.dynamicsNote = MIDIFile.ReadByte(); // Копируем динамику ноты.
                            bufferSTNote.noteTime = realTime; // Присваеваем реальное время ноты.
                            ST.arrayNoteStruct.Add(bufferSTNote); // Сохраняем новую структуру.
                            LoopIndex = LoopIndex - loopСount - 2; // Отнимаем прочитанное. 
                            break;
                        case 0xA0:  // Сменить силу нажатия клавишы. 
                            bufferSTNote.channelNote = (byte)(buffer & 0x0F); // Копируем номер канала.
                            bufferSTNote.flagNote = true; // Мы нажимаем.
                            bufferSTNote.roomNotes = MIDIFile.ReadByte(); // Копируем номер ноты.
                            bufferSTNote.dynamicsNote = MIDIFile.ReadByte(); // Копируем НОВУЮ динамику ноты.
                            bufferSTNote.noteTime = realTime; // Присваеваем реальное время ноты.
                            ST.arrayNoteStruct.Add(bufferSTNote); // Сохраняем новую структуру.
                            LoopIndex = LoopIndex - loopСount - 2; // Отнимаем прочитанное.     
                            break;
                        // Если 2-х байтовая комманда.
                        case 0xB0:
                            byte buffer2level = MIDIFile.ReadByte(); // Читаем саму команду.
                            switch (buffer2level) // Смотрим команды второго уровня.
                            {
                                default: // Для определения новых комманд (не описаных).
                                    MIDIFile.ReadByte(); // Считываем параметр какой-то неизвестной функции.
                                    LoopIndex = LoopIndex - loopСount - 2; // Отнимаем прочитанное. 
                                    break;
                            }
                            break;

                        // В случае попадания их просто нужно считать.
                        case 0xC0:   // Просто считываем байт номера.
                            MIDIFile.ReadByte(); // Считываем номер программы.
                            LoopIndex = LoopIndex - loopСount - 1; // Отнимаем прочитанное. 
                            break;

                        case 0xD0:   // Сила канала.
                            MIDIFile.ReadByte(); // Считываем номер программы.
                            LoopIndex = LoopIndex - loopСount - 1; // Отнимаем прочитанное. 
                            break;

                        case 0xE0:  // Вращения звуковысотного колеса.
                            MIDIFile.ReadBytes(2); // Считываем номер программы.
                            LoopIndex = LoopIndex - loopСount - 2; // Отнимаем прочитанное. 
                            break;
                    }
            }
            return ST; // Возвращаем заполненную структуру.
        }

        public ArrayList СreateNotesArray(MIDIMTrkStruct[] arrayST, int arrayCount)
        {
            ArrayList arrayChannelNote = new ArrayList(); // Массив каналов.

            for (int indexBlock = 0; indexBlock < arrayCount; indexBlock++) // Проходим по всем блокам MIDI.
            {
                for (int eventArray = 0; eventArray < arrayST[indexBlock].arrayNoteStruct.Count; eventArray++) // Пробегаемся по всем событиям массива каждого канала.
                {
                    noteStruct bufferNoteST = (noteStruct)arrayST[indexBlock].arrayNoteStruct[eventArray]; // Достаем событие ноты.
                    if (bufferNoteST.flagNote == true) // Если нажимают ноту.
                    {
                        notesComposition.Add(new Note((int)(bufferNoteST.noteTime), bufferNoteST.roomNotes - 20));
                    }
                }
            }
            return arrayChannelNote;
        }

        #endregion

        public List<Note> notesComposition = new List<Note>();

        public struct Note
        {
            public int StartTime;
            public int note;

            public Note(int s, int n)
            {
                StartTime = s;
                note = n;
            }
        }

        public void outData(ArrayList Data)
        {

            int maxTime = 0;
            for (int i = 0; i < notesComposition.Count; i++)
            {
                if (maxTime < notesComposition[i].StartTime) maxTime = notesComposition[i].StartTime;
                //richTextBox1.Text += notesComposition[i].note + "_" + notesComposition[i].StartTime + " ";
                for (int j = i; j < notesComposition.Count-i; j++)
                {
                    Note buffer = notesComposition[j];
                    if (notesComposition[i].StartTime > notesComposition[j].StartTime)
                    {
                        notesComposition[j] = notesComposition[i];
                        notesComposition[i] = buffer;
                    }
                }
            }
            

            for (int i = 0; i < notesComposition.Count; i++)
            {
                //richTextBox1.Text += notesComposition[i].note + "_" + notesComposition[i].StartTime + " ";
            }
            notes = notesComposition;
            //AliasGenerator gen = new AliasGenerator(maxTime, notesComposition);
            //gen.Generate();
            //richTextBox1.Text = "  ";
        }

    }
}
