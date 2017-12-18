using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandMessenger;
using CommandMessenger.Transport.Serial;
using System.Management;

namespace Thruster_Test
{
    enum Command
    {
        Start, Stop, MotorValue, Time, Thrust, Voltage, Current, Rpm, Throttle, Tare,
    };

    class Bridge
    {
        private SerialTransport _serialTransport;
        private CmdMessenger _cmdMessenger;
        public Action<uint> TimeData;
        public Action<double> ThrustData;
        public Action<double> VoltageData;
        public Action<double> CurrentData;
        public Action<double> RpmData;
        public Action<double> ThrottleData;

        Random r = new Random();

        public bool Initialize()
        {
            System.Diagnostics.Debug.AutoFlush = true;
            // Find Arduino COM Port
            string comPort = AutodetectArduinoPort();
            if (comPort != null)
            {
                try
                {
                    // Create Serial Port object
                    _serialTransport = new SerialTransport
                    {
                        CurrentSerialSettings = { PortName = comPort, BaudRate = 115200, DtrEnable = true } // object initializer //DtrEnable = false 
                    };

                    // Initialize the command messenger with the Serial Port transport layer
                    _cmdMessenger = new CmdMessenger(_serialTransport, BoardType.Bit16);

                    // Attach the callbacks to the Command Messenger
                    AttachCommandCallBacks();

                    // Start listening
                    _cmdMessenger.Connect();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void SendStart()
        {
            SendCommand cmd = new SendCommand((int)Command.Start);
            _cmdMessenger.SendCommand(cmd);
        }

        public void SendStop()
        {
            try
            {
                SendCommand cmd = new SendCommand((int)Command.Stop);
                _cmdMessenger.SendCommand(cmd);
            }
            catch
            {
                Console.WriteLine("Error sending");
            }
        }

        public void SendMotorSpeed(int speed)
        {
            try
            {
                SendCommand cmd = new SendCommand((int)Command.MotorValue);
                cmd.AddBinArgument(speed);
                _cmdMessenger.SendCommand(cmd);
            }
            catch
            {
                Console.WriteLine("Error sending");
            }
        }

        public void SendTare()
        {
            try
            {
                SendCommand cmd = new SendCommand((int)Command.Tare);
                _cmdMessenger.SendCommand(cmd);
            }catch{
                Console.WriteLine("Error sending");
            }
        }

        public void Exit()
        {
            try
            {
                _serialTransport.Disconnect();
                _cmdMessenger.Dispose();
                _serialTransport.Dispose();
            }
            catch
            {
            }
        }

        private string AutodetectArduinoPort()
        {
            ManagementScope connectionScope = new ManagementScope();
            SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

            try
            {
                foreach (ManagementObject item in searcher.Get())
                {
                    string desc = item["Description"].ToString();
                    string deviceId = item["DeviceID"].ToString();

                    if (desc.Contains("Arduino"))
                    {
                        return deviceId;
                    }
                }
            }
            catch (ManagementException e)
            {
                /* Do Nothing */
            }

            return null;
        }

        private void AttachCommandCallBacks()
        {
            _cmdMessenger.Attach(OnUnknownCommand);
            _cmdMessenger.Attach((int)Command.Time, OnTimeIn);
            _cmdMessenger.Attach((int)Command.Thrust, OnThrustDataIn);
            _cmdMessenger.Attach((int)Command.Voltage, OnVoltageIn);
            _cmdMessenger.Attach((int)Command.Current, OnCurrentIn);
            _cmdMessenger.Attach((int)Command.Rpm, OnRpmIn);
            _cmdMessenger.Attach((int)Command.Throttle, OnThrottleIn);
        }

        void OnUnknownCommand(ReceivedCommand arguments)
        {
            Console.WriteLine("Command without attached callback received");
        }

        void OnTimeIn(ReceivedCommand arguments)
        {
            UInt32 time = arguments.ReadBinUInt32Arg();
            TimeData(time);
        }

        void OnThrustDataIn(ReceivedCommand arguments)
        {
            float data = arguments.ReadBinFloatArg();
            ThrustData((double)data);
        }

        void OnVoltageIn(ReceivedCommand arguments)
        {
            float data = arguments.ReadBinFloatArg();
            VoltageData((double)data);
        }

        void OnCurrentIn(ReceivedCommand arguments)
        {
            float data = arguments.ReadBinFloatArg();
            CurrentData((double)data);
        }

        void OnRpmIn(ReceivedCommand arguments)
        {
            float data = arguments.ReadBinFloatArg();
            RpmData((double)data);
        }

        void OnThrottleIn(ReceivedCommand arguments)
        {
            Int16 data = arguments.ReadBinInt16Arg();
            ThrottleData((double)data);
        }
    }
}
