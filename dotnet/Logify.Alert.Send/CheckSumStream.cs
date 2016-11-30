using System.IO;

#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Utils.Zip {
#endif
    public interface ICheckSumCalculator<T> {
        T InitialCheckSumValue { get; }
        T UpdateCheckSum(T value, byte[] buffer, int offset, int count);
        T GetFinalCheckSum(T value);
    }

    public class CheckSumStream<T> : Stream {
        readonly Stream stream;
        readonly ICheckSumCalculator<T> checkSumCalculator;
        T readCheckSum;
        T writeCheckSum;

        public CheckSumStream(Stream stream, ICheckSumCalculator<T> checkSumCalculator) {
            this.stream = stream;
            this.checkSumCalculator = checkSumCalculator;
            ResetCheckSum();
        }

        #region Properties
        public Stream Stream { get { return stream; } }
        public override bool CanRead { get { return Stream.CanRead; } }
        public override bool CanSeek { get { return Stream.CanSeek; } }
        public override bool CanWrite { get { return Stream.CanWrite; } }
        public override long Length { get { return Stream.Length; } }
        public override long Position { get { return Stream.Position; } set { Stream.Position = value; } }
        public T ReadCheckSum { get { return checkSumCalculator.GetFinalCheckSum(readCheckSum); } }
        public T WriteCheckSum { get { return checkSumCalculator.GetFinalCheckSum(writeCheckSum); } }
        #endregion

        public void ResetCheckSum() {
            readCheckSum = checkSumCalculator.InitialCheckSumValue;
            writeCheckSum = checkSumCalculator.InitialCheckSumValue;
        }

        public override void Flush() {
            Stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin) {
            return Stream.Seek(offset, origin);
        }

        public override void SetLength(long value) {
            Stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count) {
            count = Stream.Read(buffer, offset, count);
            this.readCheckSum = checkSumCalculator.UpdateCheckSum(this.readCheckSum, buffer, offset, count);
            return count;
        }

        public override void Write(byte[] buffer, int offset, int count) {
            Stream.Write(buffer, offset, count);
            this.writeCheckSum = checkSumCalculator.UpdateCheckSum(this.writeCheckSum, buffer, offset, count);
        }
    }
}
