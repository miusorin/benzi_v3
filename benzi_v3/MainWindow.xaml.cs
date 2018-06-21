using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using Modbus;
using System.IO;
using System.IO.Ports;
using Modbus.Device;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media.Animation;

namespace benzi_v3
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        //Declarare parametri DAQ
        int[] DI2convert = { 0 };
        public bool[] DObits;
        public BitArray DIbits;


        public byte SHJ_digital_slaveID = 10;

        public ushort SHJ_digital_inputReg = 100;
        public ushort SHJ_digital_outputReg = 102;
        public SerialPort port;
        public IModbusSerialMaster modbus_master;
        
        Timer timer1;

        public static bool info = false;
        public static int button0;
        public static int button1;
        public static int button2;
        public static int button3;
        public static int button4;
        public static int button5;
        public static int button6;
        public static int button7;
        public static int button8;
        
        public object LampaH1 { get; set; }
        public object LampaH2 { get; set; }
        public object LampaH3 { get; set; }
        public object LampaH4 { get; set; }


        public MainWindow()
        {

            InitializeComponent();

            timer1 = new Timer();
            timer1.Tick += new EventHandler(refreshValues);
            timer1.Interval = 100;

           

            ///
            // Connect(); //Uncomment when compile for ASID!
            ///
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {
            try
            {
                port = new SerialPort("COM1");
                port.BaudRate = 115200;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.WriteTimeout = 500;
                port.ReadTimeout = 500;
                port.Handshake = Handshake.None;
                port.Open();

                ushort INIT_OUTPUT = 65535;

                modbus_master = ModbusSerialMaster.CreateRtu(port);
                modbus_master.WriteSingleRegister(SHJ_digital_slaveID, SHJ_digital_outputReg, INIT_OUTPUT); //== all outputs are set LOW MAX = 65535

                timer1.Start();
            }
            catch
            {
                MessageBoxResult result =
                    System.Windows.MessageBox.Show("Eroare de conexiune. Redeschidem aplicatia?", "Confirmare restart",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    System.Windows.Forms.Application.Restart();
                    Close();
                }
                if (result == MessageBoxResult.No)
                {

                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e) //MUST be MANUALLY linked
        {
            MessageBoxResult result =
                System.Windows.MessageBox.Show("Iesire din aplicatie?", "Confirmare iesire",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                string shortcutName = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.Programs),
                "\\Menu\\", "Menu", ".appref-ms");
                ProcessStartInfo openMenu = new ProcessStartInfo(shortcutName);
                openMenu.WindowStyle = ProcessWindowStyle.Maximized;
                Process.Start(openMenu);
                e.Cancel = false;
            }
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        //read Digital Outputs
        public int Get_DO()
        {
            return 0;
            ///
         //  return (int) modbus_master.ReadHoldingRegisters(SHJ_digital_slaveID, SHJ_digital_outputReg, 1)[0];
            ///
        }

        //read Digital Inputs
        public int Get_DI()
        {
            int DI = modbus_master.ReadHoldingRegisters(SHJ_digital_slaveID, SHJ_digital_inputReg, 1)[0];
            DI2convert[0] = DI;

            DIbits = new BitArray(DI2convert);
            DIbits.Not();

            return DI;
        }

        private void setDOBit(int bit)
        {
            int DO = Get_DO();

            int[] bits = Utils.decimalToBits(DO);

            bits[15 - bit] = 0;

            int newDO = Utils.bitsToDecimal(bits);
            ///
           // modbus_master.WriteSingleRegister(SHJ_digital_slaveID, SHJ_digital_outputReg, (ushort)(newDO));
            ///
        }

        private void resetDOBit(int bit)
        {
            int DO = Get_DO();

            int[] bits = Utils.decimalToBits(DO);

            bits[15 - bit] = 1;

            int newDO = Utils.bitsToDecimal(bits);
            ///
         //  modbus_master.WriteSingleRegister(SHJ_digital_slaveID, SHJ_digital_outputReg, (ushort)(newDO));
            ///
        }

        public void AnimationControl() { }

        private void button_Click(object sender, RoutedEventArgs e)//HomeButtonClick
        {
            this.Close();
        }

        private void INFO_button_Click(object sender, RoutedEventArgs e)
        {
            if (!info)
            {
                var _infoWindow = new benzi_v3.InfoWindow();
                _infoWindow.Show();
                //_infoWindow.blurEffect.Radius = 10;

                info = true;

            }
        }

        private void S1_button_TouchDown(object sender, TouchEventArgs e)
        {
            if (button1 == 0)
            {
                button1 = 1;
            }           
        }

        private void S2_button_TouchDown(object sender, TouchEventArgs e)
        {
            if (button2 == 0)
            {
                button2 = 1;
            }
        }

        private void S3_button_TouchDown(object sender, TouchEventArgs e)
        {
            if (button3 == 0)
            {
                button3 = 1;
            }
        }

        private void S0_button_TouchDown(object sender, TouchEventArgs e)
        {
            Get_DO();
            //modbus_master_serial.WriteSingleRegister(SHJ_digital_slaveID, SHJ_digital_outputReg, (ushort)(DO - 2 ^ 0));
        }

        private void S0_button_TouchUp(object sender, TouchEventArgs e)
        {
            Get_DO();
            //modbus_master_serial.WriteSingleRegister(SHJ_digital_slaveID, SHJ_digital_outputReg, (ushort)(DO - 2 ^ 0));
        }

       

        private void S1_button_TouchUp(object sender, TouchEventArgs e)
        {
            Get_DO();
            //modbus_master_serial.WriteSingleRegister(SHJ_digital_slaveID, SHJ_digital_outputReg, (ushort)(DO - 2 ^ 1));
        }

       
        private void S2_button_TouchUp(object sender, TouchEventArgs e)
        {
            Get_DO();
            //modbus_master_serial.WriteSingleRegister(SHJ_digital_slaveID, SHJ_digital_outputReg, (ushort)(DO - 2 ^ 2));
        }

        

        private void S3_button_TouchUp(object sender, TouchEventArgs e)
        {
            Get_DO();
            //modbus_master_serial.WriteSingleRegister(SHJ_digital_slaveID, SHJ_digital_outputReg, (ushort)(DO - 2 ^ 3));
        }

        private void S4_button_TouchDown(object sender, TouchEventArgs e)
        {
            Get_DO();
            //modbus_master_serial.WriteSingleRegister(SHJ_digital_slaveID, SHJ_digital_outputReg, (ushort)(DO - 2 ^ 4));
        }

        private void S4_button_TouchUp(object sender, TouchEventArgs e)
        {
            Get_DO();
            //modbus_master_serial.WriteSingleRegister(SHJ_digital_slaveID, SHJ_digital_outputReg, (ushort)(DO - 2 ^ 4));
        }

        private void S5_button_TouchDown(object sender, TouchEventArgs e)
        {
            Random r = new Random();
            int rInt = r.Next(0, 100);
            Get_DO();
            //modbus_master_serial.WriteSingleRegister(SHJ_digital_slaveID, SHJ_digital_outputReg, (ushort)(DO - 2 ^ 5));

        }

        private void S5_button_TouchUp(object sender, TouchEventArgs e)
        {
            Get_DO();
            //modbus_master_serial.WriteSingleRegister(SHJ_digital_slaveID, SHJ_digital_outputReg, (ushort)(DO - 2 ^ 5));
        }

        private void S6_button_TouchDown(object sender, TouchEventArgs e)
        {
            Get_DO();
            //modbus_master_serial.WriteSingleRegister(SHJ_digital_slaveID, SHJ_digital_outputReg, (ushort)(DO - 2 ^ 6));
        }

        private void S6_button_TouchUp(object sender, TouchEventArgs e)
        {
            Get_DO();
            //modbus_master_serial.WriteSingleRegister(SHJ_digital_slaveID, SHJ_digital_outputReg, (ushort)(DO - 2 ^ 6));
        }

        private void S7_button_TouchDown(object sender, TouchEventArgs e)
        {
            Get_DO();
            //modbus_master_serial.WriteSingleRegister(SHJ_digital_slaveID, SHJ_digital_outputReg, (ushort)(DO - 2 ^ 7));
        }

        private void S7_button_TouchUp(object sender, TouchEventArgs e)
        {
            Get_DO();
            //modbus_master_serial.WriteSingleRegister(SHJ_digital_slaveID, SHJ_digital_outputReg, (ushort)(DO - 2 ^ 7));
        }

        private void S8_button_TouchDown(object sender, TouchEventArgs e)
        {
            Get_DO();
            //modbus_master_serial.WriteSingleRegister(SHJ_digital_slaveID, SHJ_digital_outputReg, (ushort)(DO - 2 ^ 8));
        }

        private void S8_button_TouchUp(object sender, TouchEventArgs e)
        {
            Get_DO();
            //modbus_master_serial.WriteSingleRegister(SHJ_digital_slaveID, SHJ_digital_outputReg, (ushort)(DO - 2 ^ 8));
        }




        public void CreateAPath()
        {

            // Create a blue and a black Brush

            SolidColorBrush blueBrush = new SolidColorBrush();

            blueBrush.Color = Colors.Blue;

            SolidColorBrush blackBrush = new SolidColorBrush();

            blackBrush.Color = Colors.Black;
        }

        private void refreshValues(object sender, EventArgs e)
        {
            Get_DI();
            Get_DO();

            if (button1 == 0)
            {
                resetDOBit(0);
            }
            if (button1 == 1)
            {
                setDOBit(0);
            }

            if (button2 == 0)
            {
                resetDOBit(0);
            }
            if (button2 == 1)
            {
                setDOBit(0);
            }

            if (button3 == 0)
            {
                resetDOBit(0);
            }
            if (button3 == 1)
            {
                setDOBit(0);
            }

            if (DIbits[0])
            {
                LampaH1 = Brushes.Red;
            }
            if (!DIbits[0])
            {
                LampaH1 = Brushes.Green;
            }

        }


    }
}

