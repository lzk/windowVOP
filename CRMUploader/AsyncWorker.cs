using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Net;
using System.Collections.ObjectModel;
using System.ComponentModel;
using VOP;

namespace CRMUploader
{
    public enum EnumCmdResult : int
    {
        _ACK = 0,
        _CMD_invalid = 1,
        _Parameter_invalid = 2,
        _Do_not_support_this_function = 3,
        _Printer_busy = 4,
        _Printer_error = 5,
        _Set_parameter_error = 6,
        _Get_parameter_error = 7,
        _Printer_is_Sleeping = 8,
        _SW_USB_OPEN_FAIL = 11,
        _SW_USB_ERROR_OTHER = 12,
        _SW_USB_WRITE_TIMEOUT = 13,
        _SW_USB_READ_TIMEOUT = 14,
        _SW_USB_DATA_FORMAT_ERROR = 15,
        _SW_NET_DLL_LOAD_FAIL = 21,
        _SW_NET_DATA_FORMAT_ERROR = 22,
        _SW_UNKNOWN_PORT = 31,
        _SW_INVALID_PARAMETER = 32,
        _SW_INVALID_RETURN_VALUE = 33,
    }

    class AsyncWorker
    {
        public AsyncWorker()
        {
        }

        public UserCenterInfoRecord GetUserCenterInfo(string printerName)
        {
            UserCenterInfoRecord rec = new UserCenterInfoRecord();

            StringBuilder _2ndSerialNO = new StringBuilder(128);
            StringBuilder _serialNO4AIO = new StringBuilder(128);
            uint totalCount = 0;

            int result = dll.GetUserCenterInfo(printerName, _2ndSerialNO, ref totalCount, _serialNO4AIO);

            rec.PrinterName = printerName;
            rec.TotalCounter = totalCount;
            rec.SecondSerialNO = _2ndSerialNO.ToString();
            rec.SerialNO4AIO = _serialNO4AIO.ToString();

            rec.CmdResult = (EnumCmdResult)result;

            return rec;
        }

    }

    public class BaseRecord : INotifyPropertyChanged
    {
        protected string printerName;
        protected EnumCmdResult cmdResult;

        public string PrinterName
        {
            get { return this.printerName; }
            set
            {
                this.printerName = value;
                OnPropertyChanged("PrinterName");
            }
        }

        public EnumCmdResult CmdResult
        {
            get { return this.cmdResult; }
            set
            {
                this.cmdResult = value;
                OnPropertyChanged("CmdResult");
            }
        }

        public BaseRecord()
        {
            printerName = "";
        }

        public BaseRecord(string printerName)
        {
            this.printerName = printerName;
            cmdResult = EnumCmdResult._CMD_invalid;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public class UserCenterInfoRecord : BaseRecord
    {
        private uint    _totalCounter;
        private string  _2ndSerialNO;
        private string  _serialNO4AIO;

        public uint TotalCounter
        {
            get { return this._totalCounter; }
            set
            {
                this._totalCounter = value;
                OnPropertyChanged("TotalCounter");
            }
        }

        public string SecondSerialNO
        {
            get { return this._2ndSerialNO; }
            set
            {
                this._2ndSerialNO = value;
                OnPropertyChanged("SecondSerialNO");
            }
        }

        public string SerialNO4AIO
        {
            get { return this._serialNO4AIO; }
            set
            {
                this._serialNO4AIO = value;
                OnPropertyChanged("SerialNO4AIO");
            }
        }

        public UserCenterInfoRecord()
        {
            printerName = "";
            _totalCounter = 0;
            _2ndSerialNO = "";
            _serialNO4AIO = "";
       
            cmdResult = EnumCmdResult._CMD_invalid;
        }

        public UserCenterInfoRecord(string printerName, uint _totalCounter, string _2ndSerialNO, string _serialNO4AIO)
        {
            this.printerName = printerName;
            this._totalCounter = _totalCounter;
            this._2ndSerialNO = _2ndSerialNO;
            this._serialNO4AIO = _serialNO4AIO;

            cmdResult = EnumCmdResult._CMD_invalid;
        }

    }
}
