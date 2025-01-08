using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace KitCashProtocol
{
    public class TerminalFA : IDisposable
    {
        private static SerialPort Port { get; set; }
        private const int READ_TIMEOUT = 10;
        private byte[] TLV { get; set; }
        private ushort TLVPosition { get; set; }
        private TaxType DefaultTaxType { get; set; }

        public TerminalFA(string portName)
        {
            DefaultTaxType = TaxType.Unknown;
            TLV = new byte[1024];
            TLVPosition = 0;
            
                if (Port == null)
                {
                    Port = new SerialPort(portName);
                    Port.BaudRate = 115200;
                    Port.DataBits = 8;
                    Port.Parity = Parity.None;
                    Port.StopBits = StopBits.One;
                    Port.Open();
                }
            
        }

        // Метод закрытия соединения
        public void CloseConnection()
        {
            if (Port != null)
            {
                if (Port.IsOpen)
                {
                    Port.Close();
                    Port.Dispose();
                    Port = null; // Обнуление для дальнейшего предотвращения ошибок
                }
            }
        }

        // Реализация IDisposable
        public void Dispose()
        {
            CloseConnection(); // Закрываем соединение при освобождении ресурсов
        }

        public ErrorCode CancelDocument()
        {
            try
            {
                byte[] command = CommandGenerator.GetCommand(Command.CANCEL_DOCUMENT);
                Port.Write(command, 0, command.Length);
                byte[] response = ReadResponse();
                if (response[0] == 0x00)
                {
                    return ErrorCode.OK;
                }
                else
                {
                    return (ErrorCode)response[1];
                }
            }
            catch
            {
                return ErrorCode.UnknownError;
            }
        }

        public ErrorCode Initialize()
        {
            try
            {
                byte[] command = CommandGenerator.GetCommand(Command.REGISTRATION_PARAMETERS);
                Port.Write(command, 0, command.Length);
                byte[] response = ReadResponse();
                if (response[0] == 0x00)
                {
                    byte taxes = response[33];

                    if ((taxes & 1) != 0)
                    {
                        DefaultTaxType = TaxType.Common;
                        return ErrorCode.OK;
                    }
                    if ((taxes & 2) != 0)
                    {
                        DefaultTaxType = TaxType.Simplified;
                        return ErrorCode.OK;
                    }
                    if ((taxes & 4) != 0)
                    {
                        DefaultTaxType = TaxType.Simplified2;
                        return ErrorCode.OK;
                    }
                    if ((taxes & 8) != 0)
                    {
                        DefaultTaxType = TaxType.ENVD;
                        return ErrorCode.OK;
                    }
                    if ((taxes & 16) != 0)
                    {
                        DefaultTaxType = TaxType.ESN;
                        return ErrorCode.OK;
                    }
                    if ((taxes & 32) != 0)
                    {
                        DefaultTaxType = TaxType.Patent;
                        return ErrorCode.OK;
                    }

                    throw new Exception();
                }
                else
                {
                    return (ErrorCode)response[1];
                }
            }
            catch
            {
                return ErrorCode.UnknownError;
            }
        }

        public TerminalFAStatus GetStatus()
        {
            byte[] command = CommandGenerator.GetCommand(Command.GET_STATUS);
            Port.Write(command, 0, command.Length);
            byte[] response = ReadResponse();
            if (response != null)
            {
                TerminalFAStatus result;
                if (response[0] == 0x00)
                {
                    result = new TerminalFAStatus
                    {
                        Result = ErrorCode.OK,
                        FactoryNumber = Encoding.ASCII.GetString(response, 1, 12),
                        CurrentDateTime = new DateTime(response[13] + 2000, response[14], response[15], response[16], response[17], 0),
                        FatalErrors = (response[18] != 0),
                        PrinterStatus = (TerminalFAPrinterStatus)response[19]
                    };
                }
                else
                {
                    result = new TerminalFAStatus { Result = (ErrorCode)response[1] }; 
                }

                return result;
            }

            return null;
        }

        public FiscalStorageStatus GetFiscalStorageStatus()
        {
            byte[] command = CommandGenerator.GetCommand(Command.GET_FISCAL_STORAGE_STATUS);
            Port.Write(command, 0, command.Length);
            byte[] response = ReadResponse();
            if (response != null)
            {
                FiscalStorageStatus result;
                if (response[0] == 0x00)
                {
                    result = new FiscalStorageStatus
                    {
                        Result = ErrorCode.OK,
                        CurrentDocument = response[2],
                        SessionIsOpen = (response[4] != 0)
                    };
                }
                else
                {
                    result = new FiscalStorageStatus { Result = (ErrorCode)response[1] };
                }

                return result;
            }

            return null;
        }
        public string GetZN()
        {
            byte[] command = CommandGenerator.GetCommand(Command.GET_ZN);
            Port.Write(command, 0, command.Length);
            //MaterialMessageBox.Show("ЗН ККТ команда " + BitConverter.ToString(command)); // проверка команды
            byte[] response = ReadResponse();
            if (response != null)
            {
                if (response[0] == 0x00)
                {
                    return Encoding.GetEncoding(866).GetString(response.Skip(1).ToArray());
                }

                return string.Empty;
            }

            return string.Empty;
        }
        public string GetFN()
        {
            byte[] command = CommandGenerator.GetCommand(Command.GET_FN);
            Port.Write(command, 0, command.Length);
            //MaterialMessageBox.Show("ЗН ФН команда " + BitConverter.ToString(command)); // проверка команды
            byte[] response = ReadResponse();
            if (response != null)
            {
                if (response[0] == 0x00)
                {
                    return Encoding.GetEncoding(866).GetString(response.Skip(1).ToArray());
                }

                return string.Empty;
            }

            return string.Empty;
        }
        public string GetDATATIME()
        {
            byte[] command = CommandGenerator.GetCommand(Command.GET_DATATIME);

            // Отправляем команду
            try
            {
                Port.Write(command, 0, command.Length);
                //MaterialMessageBox.Show("Время и дата команда" + BitConverter.ToString(command)); // проверка команды
            }
            catch (Exception ex)
            {
                // Обработка ошибок при отправке
                MaterialMessageBox.Show("Ошибка отправки команды: " + ex.Message);
                return string.Empty;
            }

            byte[] response = ReadResponse();
            //MaterialMessageBox.Show("Время и дата ответ" + BitConverter.ToString(response)); // проверка команды
            if (response != null && response.Length >= 7) // Проверка длины ответа
            {

                // Проверяем длину
                byte lengthResponse = response[3];
                if (lengthResponse == 5)
                {
                    // Извлекаем значение даты/времени (начиная с 3-го байта)
                    byte year = response[5]; // Предполагаем, что год представлен как 00-99
                    byte month = response[6];
                    byte day = response[7];
                    byte hour = response[8];
                    byte minute = response[9];

                    // Создаем объект DateTime
                    DateTime dateTime;
                    dateTime = new DateTime(year, month, day, hour, minute, 0);
                    try
                    {
                        dateTime = new DateTime(year+2000, month, day, hour, minute, 0);

                        // Преобразуем DateTime в строку с нужным форматом
                        string dateTimeString = dateTime.ToString("dd.MM.yyyy HH:mm");
                        //MaterialMessageBox.Show("Дата и время: " + dateTimeString);

                        return dateTimeString; // Возвращаем строку с датой и временем
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        MaterialMessageBox.Show("Ошибка при создании DateTime: " + ex.Message);
                    }
                }
                else
                {
                    MaterialMessageBox.Show("Неверная длина данных: " + lengthResponse);
                }

            }
            else
            {
                MaterialMessageBox.Show("Недопустимый ответ или недостаточная длина: " + (response?.Length ?? 0));
            }

            return string.Empty; // Возвращаем пустую строку, если ответ некорректен
        }
        public void InputDATATIME()
        {
            try {
                DateTime now = DateTime.Now;
                byte[] data = { 48, 117, 5, 0, 25, 1, 1, 12, 00 };
                // Обновляем значения в массиве
                int year = now.Year - 2000;
                data[4] = (byte)year;   // Год
                data[5] = (byte)now.Month;  // Месяц
                data[6] = (byte)now.Day;    // День
                data[7] = (byte)now.Hour;   // Час
                data[8] = (byte)now.Minute;  // Минута

                byte[] command = CommandGenerator.GetCommand(Command.Input_DATATIME, data);
                Port.Write(command, 0, command.Length);
                //MaterialMessageBox.Show("Команда " + BitConverter.ToString(command)); // проверка команды

                MaterialMessageBox.Show("Время в ККТ и в ПК синхронизированы"); // проверка команды
                }
            catch (ArgumentOutOfRangeException ex) { MaterialMessageBox.Show("Не удалось ввести время. ОшибКа:" + ex.Message); }



            }
        public string GetVersConfig()
        {
            byte[] command = CommandGenerator.GetCommand(Command.GET_VERS_CONFIG);
            Port.Write(command, 0, command.Length);
            byte[] response = ReadResponse();
            if (response != null)
            {
                if (response[0] == 0x00)
                {
                    return Encoding.GetEncoding(866).GetString(response.Skip(1).ToArray());
                }

                return string.Empty;
            }

            return string.Empty;
        }
        public string GetModel()
        {
            byte[] command = CommandGenerator.GetCommand(Command.GET_MODEL);
            Port.Write(command, 0, command.Length);
            byte[] response = ReadResponse();
            if (response != null)
            {
                if (response[0] == 0x00)
                {
                    return Encoding.GetEncoding(866).GetString(response.Skip(1).ToArray());
                }

                return string.Empty;
            }

            return string.Empty;
        }

        public void Print(string text)
        {
            List<byte> data = new List<byte>();
            data.Add(0);
            data.Add(0);
            data.AddRange(Encoding.GetEncoding(866).GetBytes(text));
            byte[] command = CommandGenerator.GetCommand(Command.PRINT, data.ToArray());
            Port.Write(command, 0, command.Length);
        }

        public void Cut()
        {
            byte[] command = CommandGenerator.GetCommand(Command.CUT);
            Port.Write(command, 0, command.Length);
        }

        public ErrorCode OpenSession()
        {
            try
            {
                byte[] command = CommandGenerator.GetCommand(Command.BEGIN_OPEN_SESSION, new byte[] { 0x01 });
                Port.Write(command, 0, command.Length);
                byte[] response = ReadResponse();
                if (response[0] == 0x00)
                {
                    command = CommandGenerator.GetCommand(Command.OPEN_SESSION);
                    Port.Write(command, 0, command.Length);
                    response = ReadResponse();
                    if (response[0] == 0x00)
                    {
                        return ErrorCode.OK;
                    }
                    else
                    {
                        return (ErrorCode)response[1];
                    }
                }
                else
                {
                    return (ErrorCode)response[1];
                }
            }
            catch
            {
                return ErrorCode.UnknownError;
            }
        }

        public ErrorCode CloseSession()
        {
            try
            {
                byte[] command = CommandGenerator.GetCommand(Command.BEGIN_CLOSE_SESSION, new byte[] { 0x01 });
                Port.Write(command, 0, command.Length);
                byte[] response = ReadResponse();
                if (response[0] == 0x00)
                {
                    command = CommandGenerator.GetCommand(Command.CLOSE_SESSION);
                    Port.Write(command, 0, command.Length);
                    response = ReadResponse();
                    if (response[0] == 0x00)
                    {
                        return ErrorCode.OK;
                    }
                    else
                    {
                        return (ErrorCode)response[1];
                    }
                }
                else
                {
                    return (ErrorCode)response[1];
                }
            }
            catch
            {
                return ErrorCode.UnknownError;
            }
        }

        public ErrorCode Check(string provider, string providerPhone, double sum, double commission)
        {
            try
            {
                if (DefaultTaxType == TaxType.Unknown) throw new Exception();

                byte[] command = CommandGenerator.GetCommand(Command.BEGIN_CHECK);
                Port.Write(command, 0, command.Length);
                byte[] response = ReadResponse();
                if (response[0] == 0x00)
                {
                    TLVPosition = 0;
                    SetSubject(string.Format("ПОСТАВЩИК: {0}", provider), sum - commission);
                    command = CommandGenerator.GetCommand(Command.CHECK_POSITION, TLV.Take(TLVPosition).ToArray());
                    Port.Write(command, 0, command.Length);
                    response = ReadResponse();
                    if (response[0] == 0x00)
                    {
                        TLVPosition = 0;
                        SetSubject("АГЕНТСКОЕ ВОЗНАГРАЖДЕНИЕ", commission);
                        command = CommandGenerator.GetCommand(Command.CHECK_POSITION, TLV.Take(TLVPosition).ToArray());
                        Port.Write(command, 0, command.Length);
                        response = ReadResponse();
                        if (response[0] == 0x00)
                        {
                            TLVPosition = 0;
                            SetParameterString(1073, "7-800-5555-630");
                            SetParameterString(1044, "Перевод денежных средств");
                            SetParameterString(1074, "7(495)967-02-20");
                            SetParameterString(1026, "ООО \"КИБЕРПЛАТ\"");
                            SetParameterString(1016, "7731220815");
                            SetParameterString(1005, "121108,г.Москва,ул.Герасима Курина,д.4.корп.3,пом.1");
                            SetParameterString(1075, "7(495)967-02-20");
                            SetParameterString(1171, providerPhone);
                            command = CommandGenerator.GetCommand(Command.AGENT_DATA, TLV.Take(TLVPosition).ToArray());
                            Port.Write(command, 0, command.Length);
                            response = ReadResponse();
                            if (response[0] == 0x00)
                            {
                                TLVPosition = 0;
                                SetParameterInt8(1055, (byte)DefaultTaxType);
                                SetParameterDV(1031, ToPrice(sum));
                                SetParameterDV(1081, ToPrice(0));
                                SetParameterDV(1215, ToPrice(0));
                                SetParameterDV(1216, ToPrice(0));
                                SetParameterDV(1217, ToPrice(0));
                                command = CommandGenerator.GetCommand(Command.PAYMENT_DATA, TLV.Take(TLVPosition).ToArray());
                                Port.Write(command, 0, command.Length);
                                response = ReadResponse();
                                if (response[0] == 0x00)
                                {
                                    List<byte> data = new List<byte>();
                                    data.Add(0x01);
                                    DoubleValue dv = ToPrice5(sum);
                                    for (int i = 0; i < dv.Size; i++) data.Add(dv.Value[i]);
                                    data.AddRange(Encoding.GetEncoding(866).GetBytes("Пиво телки и угар"));
                                    command = CommandGenerator.GetCommand(Command.CHECK, data.ToArray());
                                    Port.Write(command, 0, command.Length);
                                    response = ReadResponse();
                                    if (response[0] == 0x00)
                                    {
                                        return ErrorCode.OK;
                                    }
                                    else
                                    {
                                        return (ErrorCode)response[1];
                                    }
                                }
                                else
                                {
                                    return (ErrorCode)response[1];
                                }
                            }
                            else
                            {
                                return (ErrorCode)response[1];
                            }
                        }
                        else
                        {
                            return (ErrorCode)response[1];
                        }
                    }
                    else
                    {
                        return (ErrorCode)response[1];
                    }
                }
                else
                {
                    return (ErrorCode)response[1];
                }
            }
            catch
            {
                return ErrorCode.UnknownError;
            }
        }

        //public virtual void Dispose()
        //{
        //    if (Port != null) Port.Dispose();
        //}

        private byte[] ReadResponse()
        {
            DateTime startTime = DateTime.Now;

            while (DateTime.Now.Subtract(startTime) < TimeSpan.FromSeconds(READ_TIMEOUT))
            {
                Thread.Sleep(250);
                try
                {
                    if (Port.BytesToRead != 0)
                    {
                        byte[] response = new byte[Port.BytesToRead];
                        Port.Read(response, 0, response.Length);
                        if (ResponseParser.IsValid(response))
                        {
                            return ResponseParser.GetData(response);
                        }
                    }
                }
                catch
                {
                    // empty
                }
            }

            return null;
        }

        private void SetSubject(string subjectName, double price)
        {
            ushort code, length;
            DoubleValue dv = ToPrice(price);
            DoubleValue dv2 = ToCount(1);
            code = 1059;
            length = 20;
            length += (ushort)(2 + subjectName.Length + dv.Size + dv2.Size);
            TLV[TLVPosition++] = (byte)code;
            TLV[TLVPosition++] = (byte)(code >> 8);
            TLV[TLVPosition++] = (byte)length;
            TLV[TLVPosition++] = (byte)(length >> 8);

            code = 1030;
            TLV[TLVPosition++] = (byte)code;
            TLV[TLVPosition++] = (byte)(code >> 8);
            byte[] buffer = Encoding.GetEncoding(866).GetBytes(subjectName);
            length = (ushort)buffer.Length;
            TLV[TLVPosition++] = (byte)length;
            TLV[TLVPosition++] = (byte)(length >> 8);
            for (ushort i = 0; i < length; i++) TLV[TLVPosition++] = buffer[i];

            code = 1079;
	        TLV[TLVPosition++] = (byte)code;
	        TLV[TLVPosition++] = (byte)(code >> 8);
	        length = dv.Size;
	        TLV[TLVPosition++] = (byte)length;
	        TLV[TLVPosition++] = (byte)(length >> 8);
	        for(ushort i = 0; i < length; i++) TLV[TLVPosition++] = dv.Value[i];

            code = 1023;
            TLV[TLVPosition++] = (byte)code;
            TLV[TLVPosition++] = (byte)(code >> 8);
            length = dv2.Size;
            TLV[TLVPosition++] = (byte)length;
            TLV[TLVPosition++] = (byte)(length >> 8);
            for (ushort i = 0; i < length; i++) TLV[TLVPosition++] = dv2.Value[i];

            code = 1199;
            TLV[TLVPosition++] = (byte)code;
            TLV[TLVPosition++] = (byte)(code >> 8);
            TLV[TLVPosition++] = 1;
            TLV[TLVPosition++] = 0;
            TLV[TLVPosition++] = 6;

            code = 1214;
            TLV[TLVPosition++] = (byte)code;
            TLV[TLVPosition++] = (byte)(code >> 8);
            TLV[TLVPosition++] = 1;
            TLV[TLVPosition++] = 0;
            TLV[TLVPosition++] = 4;
        }

        private void SetParameterString(ushort code, string value)
        {
            byte[] biffer = Encoding.GetEncoding(866).GetBytes(value);
            TLV[TLVPosition++] = (byte)code;
            TLV[TLVPosition++] = (byte)(code >> 8);
            ushort length = (ushort)biffer.Length;
            TLV[TLVPosition++] = (byte)length;
            TLV[TLVPosition++] = (byte)(length >> 8);
            for (ushort i = 0; i < length; i++) TLV[TLVPosition++] = biffer[i];
        }

        private void SetParameterInt8(ushort code, byte value)
        {
            TLV[TLVPosition++] = (byte)code;
            TLV[TLVPosition++] = (byte)(code >> 8);
            TLV[TLVPosition++] = 1;
            TLV[TLVPosition++] = 0;
            TLV[TLVPosition++] = value;
        }

        private void SetParameterDV(ushort code, DoubleValue dv)
        {
            TLV[TLVPosition++] = (byte)code;
            TLV[TLVPosition++] = (byte)(code >> 8);
            ushort length = dv.Size;
            TLV[TLVPosition++] = (byte)length;
            TLV[TLVPosition++] = (byte)(length >> 8);
            for (ushort i = 0; i < length; i++) TLV[TLVPosition++] = dv.Value[i];
        }

        private DoubleValue ToPrice(double value)
        {
            DoubleValue dv = new DoubleValue();
	        int number = (int)Math.Truncate(value * 100);

	        int step = 0;
	        byte vl;

	        do
	        {
                vl = (byte)(number >> step * 8);
                dv.Value[step] = vl;
		        step++;
	        }
            while (vl != 0);
            dv.Size = (byte)(step - 1);

            return dv;
        }

        private DoubleValue ToPrice5(double value)
        {
            DoubleValue dv = new DoubleValue();
	        dv.Size = 5;
	        int number = (int)Math.Truncate(value * 100);
	        byte vl;

	        for(int i = 0; i < dv.Size; i++)
	        {
		        vl = (byte)(number >> i * 8);
                dv.Value[i] = vl;
	        }

            return dv;
        }

        private DoubleValue ToCount(double value)
        {
            DoubleValue dv = new DoubleValue();
	        byte position = 0;
	        int number = (int)Math.Truncate(value);
	        while(value - number > 0.000001)
	        {
		        position++;
		        value *= 10;
		        number = (int)Math.Truncate(value);
	        }

	        int step = 0;
	        dv.Value[step++] = position;
	        byte vl;
	        do
	        {
		        vl = (byte)(number >> (step - 1) * 8);
                dv.Value[step] = vl;
		        step++;
	        }
            while (vl != 0);
            dv.Size = (byte)(step - 1);

            return dv;
        }
    }
}