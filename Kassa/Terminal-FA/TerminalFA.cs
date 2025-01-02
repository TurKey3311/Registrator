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
            MaterialMessageBox.Show("Запрос: " + BitConverter.ToString(command));
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
            // Определяем TAG 0x73 для команды GET_DATATIME
            byte tag = 0x73;
            byte length = 0;  // Нет входных параметров

            // Сформируем команду TLV (1 байт для TAG + 1 байт для LEN)
            byte[] command = new byte[1 + 1];
            command[0] = tag;
            command[1] = length;

            // Отправляем команду
            try
            {
                Port.Write(command, 0, command.Length);
                MaterialMessageBox.Show("Запрос: " + BitConverter.ToString(command));
            }
            catch (Exception ex)
            {
                // Обработка ошибок при отправке
                MaterialMessageBox.Show("Ошибка отправки команды: " + ex.Message);
                return string.Empty;
            }

            // Читаем ответ
            byte[] response = ReadResponse();
            MaterialMessageBox.Show("Ответ: " + BitConverter.ToString(response));
            if (response != null && response.Length >= 7) // 1 байт TAG + 1 байт LEN + 5 байт для DATETIME
            {
                // Проверяем первый байт (TAG) для правильности
                int tagResponse = response[0] << 8 | response[1]; // Переводим два байта в одно целое значение
                if (tagResponse == 30000) // Проверьте тег, который соответствует 30000
                {
                    // Проверяем длину
                    byte lengthResponse = response[2];
                    if (lengthResponse == 5)
                    {
                        // Извлекаем значение даты/времени (начиная с 3-го байта)
                        byte[] dateTimeBytes = new byte[5];
                        Array.Copy(response, 3, dateTimeBytes, 0, 5);

                        // Здесь можно преобразовать byte[] в строку или в нужный вам формат
                        // Например, преобразуем в строку, если даты/времена хранятся в определенном формате
                        // Исходный вид преобразования зависит от того, как представлены данные.
                        return BitConverter.ToString(dateTimeBytes); // Пример: Конвертируем в строку
                    }
                }
            }

            return string.Empty; // Возвращаем пустую строку, если ответ некорректен
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