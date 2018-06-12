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

namespace Template_ASID
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
        ///////////


        Timer timer1;

        public static int b1;
       

        public MainWindow()
        {
            InitializeComponent();

            timer1 = new Timer();
            timer1.Tick += new EventHandler(refreshValues);
            timer1.Interval = 100;

            B.Background = Brushes.Green;
            LampaH1.Visibility = Visibility.Visible;

            ///
            // Connect();
            ///
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
                    System.Windows.MessageBox.Show("Eroare de conexiune. Redeschidem aplicatia?", "Restart Confirmation",
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

        private void ButonulB(object sender, RoutedEventArgs e)
        {
            if (b1 == 0)
            {
                B.Background = Brushes.Red;
                b1 = 1;
 
            }
            else
            {
                B.Background = Brushes.Green;
                b1 = 0;
            }
        }

        private void refreshValues(object sender, EventArgs e)
        {
            Get_DI();
            Get_DO();

            if (b1 == 0)
            {
                resetDOBit(0);
            }
            if (b1 == 1)
            {
                setDOBit(0);
            }

            if (DIbits[0])
            {
                LampaH1.Visibility = Visibility.Visible;
            }
            if (!DIbits[0])
            {
                LampaH.Visibility = Visibility.Visible;
            }

        }


    }
}

