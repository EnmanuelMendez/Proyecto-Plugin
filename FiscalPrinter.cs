using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace Plugin_ICGFront
{
    public class FiscalPrinter
    {
        private const byte Ack = 0x06;
        private const byte End = 0x03;

        private byte _seq = 0xD1;

        public FiscalPrinter(string portName, int baudRate)
        {
            SerialPort = new SerialPort(portName, baudRate)
            {
                DtrEnable = true
            };
        }

        private SerialPort SerialPort { get; }

        private void SendData(byte[] data)
        {
            var streamData = new MemoryStream();
            streamData.Write(data, 0, data.Length);

            var checkSum = GetCheckSum(data);
            streamData.Write(checkSum, 0, checkSum.Length);

            var bytes = streamData.ToArray();

            SerialPort.Write(bytes, 0, bytes.Length);
            SerialPort.Write(new[] { Ack }, 0, 1);

            Thread.Sleep(100);

            if (_seq >= 0xFE)
                _seq = 0x81;
            else
                _seq += 2;
        }

        private static byte[] IntToByteArray(int number)
        {
            var byteArray = BitConverter.GetBytes(number);

            if (BitConverter.IsLittleEndian) Array.Reverse(byteArray);

            return byteArray;
        }

        private static string ByteArrayToString(byte[] array)
        {
            return BitConverter.ToString(array).Replace("-", "");
        }

        private static int SumByteArray(byte[] array)
        {
            return array.Aggregate(0, (current, b) => current + b);
        }

        private static byte[] GetCheckSum(byte[] data)
        {
            var sum = SumByteArray(data);
            var length = IntToByteArray(sum);
            var hexRepresentation = ByteArrayToString(length).Substring(4, 4);
            return Encoding.ASCII.GetBytes(hexRepresentation);
        }

        private static byte[] StringToByteArray(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

        private static string CenterText(string text, int length)
        {
            return text.PadLeft((length - text.Length) / 2 + text.Length).PadRight(length);
        }

        public void InitConnection()
        {
            SerialPort.Open();
            SerialPort.DiscardOutBuffer();
            SerialPort.DiscardInBuffer();
        }

        public void DeInitConnection()
        {
            SerialPort.Close();
        }

        public void OpenNonFiscalReceipt()
        {
            byte[] openNonFiscalReceipt =
            {
                0x02, // Packet start
                _seq, // Sequence

                /* Command start */
                0x0E,
                0x01,
                0x1C,
                0x00,
                0x00,
                /* Command end */

                0x03 // Packet end
            };

            SendData(openNonFiscalReceipt);
        }

        public void CloseNonFiscalReceipt()
        {
            byte[] closeNonFiscalReceipt =
            {
                0x02, // Packet start
                _seq, // Sequence

                /* Command start */
                0x0E,
                0x06,
                0x1C,
                0x00,
                0x00,
                /* Command end */

                0x03 // Packet end
            };

            SendData(closeNonFiscalReceipt);
        }

        public void PaperCut()
        {
            byte[] cutPaper =
            {
                0x02, // Packet start
                _seq, // Sequence

                /* Command start */
                0x07,
                0x1B,
                0x02,
                0x1C,
                0x00,
                0x00,
                /* Command end */

                0x03 // Packet end
            };

            SendData(cutPaper);
        }

        public void PrintNonFiscalText(string text, bool tall = false, bool bold = false, bool inverted = false,
            bool wide = false, bool underline = false, bool code128 = false)
        {
            byte[] printNonFiscalText =
            {
                0x02, // Packet start
                _seq, // Sequence

                /* Command start */
                0x0E,
                0x1B,
                0x02,
                0x1C,
                0x00,
                0x00,
                0x1C
                /* Command end */
            };

            var memoryStream = new MemoryStream();
            memoryStream.Write(printNonFiscalText, 0, printNonFiscalText.Length);

            if (tall) memoryStream.Write(new byte[] { 0x1B, 0x1B, 0x08 }, 0, 3);

            if (bold) memoryStream.Write(new byte[] { 0x1B, 0x1B, 0x01 }, 0, 3);

            if (underline) memoryStream.Write(new byte[] { 0x1B, 0x1B, 0x02 }, 0, 3);

            if (inverted) memoryStream.Write(new byte[] { 0x1B, 0x1B, 0x10 }, 0, 3);

            if (wide) memoryStream.Write(new byte[] { 0x1B, 0x1B, 0x04 }, 0, 3);

            if (code128)
            {
                memoryStream.Write(new byte[] { 0x1B, 0x1B, 0x80, 0x04, 0x78, 0x00, 0x01, 0x00 }, 0, 8);
            }
            else
            {
                var maxLength = tall || wide ? 28 : 56;
                text = CenterText(text, maxLength).ToUpper();
            }

            var textToPrint = StringToByteArray(text);

            memoryStream.Write(textToPrint, 0, textToPrint.Length);
            memoryStream.Write(new[] { End }, 0, 1);

            var data = memoryStream.ToArray();

            SendData(data);
        }
    }
}