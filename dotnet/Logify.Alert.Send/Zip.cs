using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
#if !DXWINDOW
using DevExpress.Utils.Zip.Internal;
#endif

using System.IO.Compression;
#if FORCE_USE_NATIVE_ENCODING || DXWINDOW
using DXEncoding = System.Text.Encoding;
#endif

#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Utils.Zip {
#endif
    #region Crc32CheckSumCalculator
    [CLSCompliant(false)]
    public class Crc32CheckSumCalculator : ICheckSumCalculator<uint> {
        static Crc32CheckSumCalculator instance;
        public static Crc32CheckSumCalculator Instance {
            get {
                if (instance == null)
                    instance = new Crc32CheckSumCalculator();
                return instance;
            }
        }

        #region CheckSumCalculator<uint> Members
        public uint InitialCheckSumValue { get { return unchecked(0xFFFFFFFF); } }
        public uint UpdateCheckSum(uint value, byte[] buffer, int offset, int count) {
            return Crc32CheckSum.Update(value, buffer, offset, count);
        }
        public uint GetFinalCheckSum(uint value) {
            return unchecked(value ^ 0xFFFFFFFF);
        }
        #endregion
    }
    #endregion
    #region ByteCountCheckSumCalculator
    public class ByteCountCheckSumCalculator : ICheckSumCalculator<int> {
        static ByteCountCheckSumCalculator instance;
        public static ByteCountCheckSumCalculator Instance {
            get {
                if (instance == null)
                    instance = new ByteCountCheckSumCalculator();
                return instance;
            }
        }

        #region CheckSumCalculator<uint> Members
        public int InitialCheckSumValue { get { return 0; } }
        public int UpdateCheckSum(int value, byte[] buffer, int offset, int count) {
            return value + count;
        }
        public int GetFinalCheckSum(int value) {
            return value;
        }
        #endregion
    }
    #endregion

    #region Crc32CheckSum
    [CLSCompliant(false)]
    public static class Crc32CheckSum {
        static uint[] table = CreateTable();
        static uint[] CreateTable() {
            uint[] result = new uint[256];
            for (uint n = 0; n < 256; n++) {
                uint c = n;
                for (int k = 0; k < 8; k++) {
                    if ((c & 1) != 0)
                        c = 0xedb88320 ^ (c >> 1);
                    else
                        c = c >> 1;
                }
                result[n] = c;
            }
            return result;
        }

        public static uint[] Table { get { return table; } }

        public static uint Update(uint checkSum, byte[] buffer, int offset, int count) {
            for (int i = offset, end = offset + count; i < end; i++)
                checkSum = (checkSum >> 8) ^ table[(checkSum ^ buffer[i]) & 0xFF];
            return checkSum;
        }
        public static uint Calculate(uint checkSum, byte ch) {
            return (checkSum >> 8) ^ table[(checkSum ^ ch) & 0xFF];
        }
        public static uint Calculate(string str) {
            byte[] buffer = Encoding.ASCII.GetBytes(str);
            return Update(0xffffffff, buffer, 0, buffer.Length) ^ 0xffffffff;
        }
        public static uint Calculate(Stream stream) {
            long streamPos = stream.Position;
            const int bufferSize = 0x32000;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            uint crc = 0xffffffff;
            do {
                bytesRead = stream.Read(buffer, 0, bufferSize);
                crc = Update(crc, buffer, 0, bytesRead); // ^ 0xffffffff;
            }
            while (bytesRead == bufferSize);
            stream.Position = streamPos;
            return ~crc;
        }
    }
    #endregion
    #region Crc32Stream
    [CLSCompliant(false)]
    public class Crc32Stream : CheckSumStream<uint> {
        public Crc32Stream(Stream stream)
            : base(stream, Crc32CheckSumCalculator.Instance) {
        }
    }
    #endregion

    #region ByteCountStream
    public class ByteCountStream : CheckSumStream<int> {
        public ByteCountStream(Stream stream)
            : base(stream, ByteCountCheckSumCalculator.Instance) {
        }
    }
    #endregion
    #region InternalZipArchive
    public class InternalZipArchiveCore : IDisposable {
        readonly Stream zipStream;
        readonly BinaryWriter writer;
        readonly List<CentralDirectoryEntry> centralDirectory;
        readonly bool requireDisposeForStream;

        public InternalZipArchiveCore(string zipFileName)
            : this(new FileStream(zipFileName, FileMode.Create, FileAccess.Write)) {
            requireDisposeForStream = true;
        }
        public InternalZipArchiveCore(Stream stream) {
            zipStream = stream;
            writer = new BinaryWriter(zipStream);
            centralDirectory = new List<CentralDirectoryEntry>();
        }

        internal protected List<CentralDirectoryEntry> CentralDirectory { get { return centralDirectory; } }
        BinaryWriter Writer { get { return writer; } }
        internal protected Stream ZipStream { get { return zipStream; } }
        Encoding UTF8Encoding { get { return Encoding.UTF8; } }
        void WriteCentralDirectory() {
            long centralDirectoryStartPosition = ZipStream.Position;
            for (int i = 0; i < CentralDirectory.Count; i++) {
                CentralDirectoryEntry dirEntry = CentralDirectory[i];
                Writer.Write((uint)ZipSignatures.FileEntryRecord);
                Writer.Write((short)0x14);	// version made by
                Writer.Write((short)0x14);	// version needed to extract
                Writer.Write(dirEntry.GeneralPurposeFlag);	// general purpose bit flag - compression method = fast
                Writer.Write((short)dirEntry.CompressionMethod);		// compression method = deflate
                Writer.Write(dirEntry.MsDosDateTime);	// file time and data
                Writer.Write(dirEntry.Crc32);			// crc-32
                Writer.Write(dirEntry.CompressedSize);
                Writer.Write(dirEntry.UncompressedSize);
                bool useUtf = (dirEntry.GeneralPurposeFlag & (short)ZipFlags.EFS) != 0;
                byte[] fileNameBytes = ConvertToByteArray(dirEntry.FileName, useUtf);
                byte[] commentBytes = ConvertToByteArray(dirEntry.Comment, useUtf);
                Writer.Write((short)fileNameBytes.Length);
                if (dirEntry.ExtraFields != null) {
                    Writer.Write(dirEntry.ExtraFields.CalculateSize(ExtraFieldType.CentralDirectoryEntry));
                } else
                    Writer.Write((short)0);		// extra field length
                Writer.Write((short)commentBytes.Length);		// file comment length
                Writer.Write((short)0);		// disk number start 
                Writer.Write((short)0);		// internal file attributes
                Writer.Write(dirEntry.FileAttributes);		// external file attributes
                Writer.Write(dirEntry.RelativeOffset);		// relative offset of local header
                Writer.Write(fileNameBytes);
                if (dirEntry.ExtraFields != null)
                    dirEntry.ExtraFields.Write(writer, ExtraFieldType.CentralDirectoryEntry);
                Writer.Write(commentBytes);
            }
            long centralDirectoryEndPosition = ZipStream.Position;
            Writer.Write((uint)ZipSignatures.EndOfCentralDirSignature);
            Writer.Write((short)0);			// number of this disk
            Writer.Write((short)0);			// number of this disk with the start of the central directory
            Writer.Write((short)CentralDirectory.Count);			// total number of entries in the central dir on this disk
            Writer.Write((short)CentralDirectory.Count);			// total number of entries in the central dir
            Writer.Write((uint)(centralDirectoryEndPosition - centralDirectoryStartPosition));			// size of the central directory
            Writer.Write((uint)centralDirectoryStartPosition);	// offset of start of central directory
            Writer.Write((short)0);
        }

        byte[] ConvertToByteArray(string value, bool useUtf) {
            if (String.IsNullOrEmpty(value))
                return new byte[] { };
            if (useUtf)
                return UTF8Encoding.GetBytes(value);
            return GetDefaultEncoding().GetBytes(value);
        }
        protected internal virtual Stream CreateDeflateStream(Stream stream) {
            return new DeflateStream(stream, CompressionMode.Compress, true);
        }
        internal CompressionMethod GetDeflateStreamCompressionMethod() {
            return CompressionMethod.Deflate; // compression method = deflate
        }
        public void Add(string fileName) {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                Add(Path.GetFileName(fileName), File.GetLastWriteTime(fileName), stream);
        }
        public void AddCompressed(string name, DateTime fileTime, CompressedStream compressedStream) {
            Stream stream = compressedStream.Stream;
            ICompressionStrategy compressionStrategy = new UseCompressedStreamCompressionStrategy(compressedStream);
            name = name.Replace('\\', '/');
            CentralDirectoryEntry dirEntry = new CentralDirectoryEntry();
            dirEntry.MsDosDateTime = (int)ZipDateTimeHelper.ToMsDosDateTime(fileTime);
            dirEntry.Crc32 = compressionStrategy.Crc32;
            dirEntry.CompressedSize = (int)(stream.Length - stream.Position);
            dirEntry.FileName = name;
            dirEntry.UncompressedSize = compressedStream.UncompressedSize;
            dirEntry.RelativeOffset = (int)ZipStream.Position;
            dirEntry.GeneralPurposeFlag = CalculateGeneralPuropseFlag(dirEntry.FileName, compressionStrategy);
            WriteDirectoryEntry(dirEntry, compressionStrategy);
            compressionStrategy.Compress(stream, ZipStream, null);

            CentralDirectory.Add(dirEntry);
            ZipStream.Position = ZipStream.Length;
        }
        short CalculateGeneralPuropseFlag(string fileName, ICompressionStrategy compressionStrategy) {
            Encoding defaultEncoding = GetDefaultEncoding();
            bool canCodeToASCII = (UTF8Encoding == defaultEncoding) ? false : ZipEncodingHelper.CanCodeToEncoding(defaultEncoding, fileName);
            short generalPurposeFlag = (short)ZipFlags.ImplodeUse8kSlidingDictionary;
#pragma warning disable 675
            generalPurposeFlag |= compressionStrategy.GetGeneralPurposeBitFlag();
#pragma warning restore 675
            if(!canCodeToASCII)
                generalPurposeFlag |= (short)ZipFlags.EFS;
            return generalPurposeFlag;
        }
        protected virtual Encoding GetDefaultEncoding() {
            return Encoding.ASCII;
        }
        protected virtual CentralDirectoryEntry WriteFile(string name, DateTime fileTime, Stream stream, ICompressionStrategy compressionStrategy, IZipComplexOperationProgress progress) {
            System.Diagnostics.Debug.Assert(compressionStrategy != null);
            name = name.Replace('\\', '/');
            CentralDirectoryEntry dirEntry = new CentralDirectoryEntry();
            dirEntry.ExtraFields = CreateExtraFieldCollection();
            dirEntry.MsDosDateTime = (int)ZipDateTimeHelper.ToMsDosDateTime(fileTime);
            dirEntry.FileName = name;
            long streamPosition = 0;
            try {
                streamPosition = stream.Position;
            } catch (NotSupportedException) {

            }
            dirEntry.UncompressedSize = (int)(stream.Length - streamPosition);
            dirEntry.RelativeOffset = (int)ZipStream.Position;
            dirEntry.GeneralPurposeFlag = CalculateGeneralPuropseFlag(dirEntry.FileName, compressionStrategy);
            long crc32Position = WriteDirectoryEntry(dirEntry, compressionStrategy);
            long compressedDataStartPosition = ZipStream.Position;
            compressionStrategy.Compress(stream, ZipStream, progress);
            dirEntry.Crc32 = compressionStrategy.Crc32;
            dirEntry.CompressedSize = (int)(ZipStream.Position - compressedDataStartPosition);
            ZipStream.Position = crc32Position;
            Writer.Write(dirEntry.Crc32);
            Writer.Write(dirEntry.CompressedSize);
            CentralDirectory.Add(dirEntry);
            ZipStream.Position = ZipStream.Length;
            return dirEntry;
        }
        protected virtual IZipExtraFieldCollection CreateExtraFieldCollection() {
            return null;
        }
        public void Add(string name, DateTime fileTime, Stream stream) {
            DeflateCompressionStrategy compressionStrategy = new DeflateCompressionStrategy();
            WriteFile(name, fileTime, stream, compressionStrategy, null);
        }
        long WriteDirectoryEntry(CentralDirectoryEntry dirEntry, ICompressionStrategy compressionStrategy) {
            Writer.Write((uint)ZipSignatures.FileRecord);
            Writer.Write((short)0x14); // version needed to extract
            Writer.Write(dirEntry.GeneralPurposeFlag); // general purpose bit flag - compression method = fast
            dirEntry.CompressionMethod = compressionStrategy.CompressionMethod;
            Writer.Write((short)dirEntry.CompressionMethod);
            Writer.Write(dirEntry.MsDosDateTime); // file time and data
            long crc32Position = ZipStream.Position;
            Writer.Write(dirEntry.Crc32); // crc-32////?? dirEntry.crc or compressionStrategy.crc ???,

            Writer.Write(dirEntry.CompressedSize); // CompressedSize
            Writer.Write(dirEntry.UncompressedSize);
            bool useUtf = (dirEntry.GeneralPurposeFlag & (short)ZipFlags.EFS) != 0;
            byte[] fileNameBytes = ConvertToByteArray(dirEntry.FileName, useUtf);
            Writer.Write((short)fileNameBytes.Length);
            compressionStrategy.PrepareExtraFields(dirEntry.ExtraFields);
            if (dirEntry.ExtraFields != null)
                Writer.Write((short)dirEntry.ExtraFields.CalculateSize(ExtraFieldType.LocalHeader));//extra bytes
            else
                Writer.Write((short)0);//extra bytes
            Writer.Write(fileNameBytes);
            if (dirEntry.ExtraFields != null)
                dirEntry.ExtraFields.Write(Writer, ExtraFieldType.LocalHeader);
            return crc32Position;
        }
        public void Add(string fileName, DateTime fileTime, string content) {
            Add(fileName, fileTime, UTF8Encoding.GetBytes(content));
        }
        public void Add(string fileName, DateTime fileTime, byte[] content) {
            using (MemoryStream stream = new MemoryStream(content, false))
                Add(fileName, fileTime, stream);
        }
        public void Add(string fileName, DateTime fileTime, byte[] content, int index, int count) {
            using (MemoryStream stream = new MemoryStream(content, index, count, false))
                Add(fileName, fileTime, stream);
        }

        #region IDisposable Members
        void IDisposable.Dispose() {
            WriteCentralDirectory();
            if (zipStream != null) {
                if (requireDisposeForStream)
                    zipStream.Dispose();
                else
                    zipStream.Flush();
            }
        }
        #endregion
    }
    #endregion
    public class InternalZipFileCollection : List<InternalZipFile> {
    }
    public class InternalZipArchive : InternalZipArchiveCore {
        public static bool IsZipFileSignature(int value) {
            return (ZipSignatures)value == ZipSignatures.FileRecord;
        }
        public static InternalZipFileCollection Open(Stream stream) {
            return Open(stream, Encoding.Default);
        }
        public static InternalZipFileCollection Open(Stream stream, Encoding fileNameEncoding) {
            InternalZipFileParser zipFileParser = new InternalZipFileParser();
            zipFileParser.Parse(stream, fileNameEncoding);
            return zipFileParser.Records;
        }

        public InternalZipArchive(string zipFileName)
            : base(zipFileName) {
        }
        public InternalZipArchive(Stream stream)
            : base(stream) {
        }
    }

    #region CompressedStream
    public class CompressedStream {
        int crc32;
        int uncompressedSize;
        Stream stream;

        public int Crc32 { get { return crc32; } set { crc32 = value; } }
        public int UncompressedSize { get { return uncompressedSize; } set { uncompressedSize = value; } }
        public Stream Stream { get { return stream; } set { stream = value; } }
    }
    #endregion

    #region ZipFlags
    [Flags]
    public enum ZipFlags {
        Encrypted = 0x0001,
        ImplodeUse8kSlidingDictionary = 0x0002,
        ImplodeUse4kSlidingDictionary = 0x0000,
        ImplodeUse3ShannonFanoTrees = 0x0004,
        ImplodeUse2ShannonFanoTrees = 0x0000,
        DeflateNormalCompression = 0x0000,
        DeflateMaximumCompression = 0x0002,
        DeflateFastCompression = 0x0004,
        DeflateSuperFastCompression = 0x0007,
        LZMAEOSIndicatesEndOfStream = 0x0001,
        UseDataFromDataDescriptor = 0x0008,
        //Reserved1 = 0x0010,
        ArchiveContainsCompressedPatchedData = 0x0020,
        StrongEncryption = 0x0040,
        Unused1 = 0x0080,
        Unused2 = 0x0100,
        Unused3 = 0x0200,
        Unused4 = 0x0400,
        EFS = 0x0800,
        //PkWareReserved1 = 0x1000,
        LocalHeaderDataMasked = 0x2000,
        //PkWareReserved2 = 0x3000,
        //PkWareReserved3 = 0x4000,
    }
    #endregion
    #region InternalZipFile
    public class InternalZipFile {
        #region Fields
        Encoding fileNameEncoding = Encoding.Default;
        Stream internalFileDataStream;//for backward compatibility
        #endregion

        public InternalZipFile() {
            FileLastModificationTime = DateTime.Now;
        }

        #region Properties
        protected Int16 FileNameLength { get; set; }
        protected Int16 LocalHeaderExtraFieldLength { get; set; }
        protected Int32 Crc32 { get; set; }
        protected ZipFlags GeneralPurposeBitFlag { get; set; }
        public long UncompressedSize { get; protected internal set; }
        public long CompressedSize { get; protected internal set; }
        protected StreamProxy ContentRawDataStreamProxy { get; private set; }
        protected CompressionMethod CompressionMethod { get; set; }
        public Stream FileDataStream {
            get {
                if (internalFileDataStream == null) {
                    this.internalFileDataStream = CreateDecompressionStream(ContentRawDataStreamProxy);
                }
                return internalFileDataStream;
            }
        }
        public string FileName { get; protected set; }
        public Encoding DefaultEncoding {
            get { return fileNameEncoding; }
            set {
                if (value == null)
                    value = Encoding.Default;
                fileNameEncoding = value;
            }
        }
        public DateTime FileLastModificationTime { get; set; }
        public bool IsEncrypted { get; private set; }
        public byte CheckByte { get; private set; }
        protected Int16 VersionToExtract { get; set; }
        #endregion

        protected internal virtual void ReadLocalHeader(BinaryReader reader) {
            VersionToExtract = reader.ReadInt16();
            GeneralPurposeBitFlag = (ZipFlags)reader.ReadInt16();
            IsEncrypted = (GeneralPurposeBitFlag & ZipFlags.Encrypted) == ZipFlags.Encrypted;
            CompressionMethod = (CompressionMethod)reader.ReadInt16();
            int dosLastModificationTimeValue = reader.ReadInt32();
            try {
                FileLastModificationTime = ZipDateTimeHelper.FromMsDos(dosLastModificationTimeValue);
            }
            catch (ArgumentOutOfRangeException) {
                FileLastModificationTime = DateTime.MinValue;
            }
            Crc32 = reader.ReadInt32();
            if (IsEncrypted) {
                if ((GeneralPurposeBitFlag & ZipFlags.UseDataFromDataDescriptor) != 0)
                    CheckByte = (byte)((dosLastModificationTimeValue >> 8) & 0xff);
                else
                    CheckByte = (byte)((Crc32 >> 24) & 0xff);
            }

            CompressedSize = reader.ReadUInt32();
            UncompressedSize = reader.ReadUInt32();
            FileNameLength = reader.ReadInt16();
            LocalHeaderExtraFieldLength = reader.ReadInt16();
            FileName = ReadString(reader, FileNameLength);
            ReadLocalHeaderExtraFields(reader, LocalHeaderExtraFieldLength);//skip extra fields
            Stream baseStream = reader.BaseStream;
            long dataStreamPosition = reader.BaseStream.Position;
            baseStream.Seek(CompressedSize, SeekOrigin.Current);
            if ((GeneralPurposeBitFlag & ZipFlags.UseDataFromDataDescriptor) != 0) {
                SeekToDataDescriptorData(reader, CompressedSize);
                Crc32 = reader.ReadInt32();
                CompressedSize = reader.ReadUInt32();
                UncompressedSize = reader.ReadUInt32();                
            }
            long actualDataStreamLength = (CompressedSize == 0) ? -1 : CompressedSize;
            ContentRawDataStreamProxy = new StreamProxy(baseStream, dataStreamPosition, actualDataStreamLength, CompressionMethod != CompressionMethod.Store);
        }
        protected virtual void ReadLocalHeaderExtraFields(BinaryReader reader, short extraFieldLength) {
            InternalZipExtraFieldFactory fieldFactory = CreateInternalZipExtraFieldFactory();
            ZipExtraFieldComposition extraFields = ZipExtraFieldComposition.Read(reader, extraFieldLength, fieldFactory);
            extraFields.Apply(this);
        }
        protected virtual InternalZipExtraFieldFactory CreateInternalZipExtraFieldFactory() {
            return InternalZipExtraFieldFactoryInstance.Instance;
        }
        protected string ReadString(BinaryReader reader, int count) {
            byte[] bytes = reader.ReadBytes(count);
            Encoding actualEncoding = GetActualEncoding();
            return actualEncoding.GetString(bytes, 0, bytes.Length);
        }
        Encoding GetActualEncoding() {
            bool useUtf = (GeneralPurposeBitFlag & ZipFlags.EFS) == ZipFlags.EFS;
            return (useUtf) ? Encoding.UTF8 : DefaultEncoding;
        }
        protected internal virtual void SeekToDataDescriptorData(BinaryReader reader, long compressedSize) {
            Stream baseStream = reader.BaseStream;
            if (compressedSize != 0) {
                ZipSignatures signature = (ZipSignatures)reader.ReadInt32();
                if (signature != ZipSignatures.DataDescriptorRecord)
                    baseStream.Seek(-sizeof(Int32), SeekOrigin.Current);
            } else {
                byte[] pattern = new byte[] { (byte)'P', (byte)'K', 0x07, 0x08 };
                byte[] bytes = new byte[7]; // pattern.Length * 2 - 1                
                long beginFilePosition = baseStream.Position;
                baseStream.Read(bytes, 0, 7);
                for (; ; ) {
                    int index = SearchForPattern(bytes, pattern);
                    if (index >= 0) {
                        long positionToContinue = baseStream.Position;
                        baseStream.Seek(index - 3, SeekOrigin.Current);
                        long calculatedCompressedSize = baseStream.Position - beginFilePosition - 4;
                        long possibleDataDescriptorPosition = baseStream.Position;//skip signature
                        reader.ReadInt32();//skip crc
                        Int32 readedCompressedSize = reader.ReadInt32();
                        if (calculatedCompressedSize == readedCompressedSize) {
                            baseStream.Seek(possibleDataDescriptorPosition, SeekOrigin.Begin);
                            break;
                        }
                        baseStream.Seek(positionToContinue, SeekOrigin.Begin);
                    }
                    bytes[0] = bytes[4];
                    bytes[1] = bytes[5];
                    bytes[2] = bytes[6];
                    if (baseStream.Read(bytes, 3, 4) == 0)
                        break;
                }
            }
        }
        // Brute search, may be replaced with Boyer-Moore search
        protected internal int SearchForPattern(byte[] bytes, byte[] pattern) {
            int bufferLength = bytes.Length;
            int patternLength = pattern.Length;
            if (bufferLength < patternLength)
                return -1;

            int patternIndex = 0;

            //int count = bufferLength - patternLength;
            for (int i = 0; i < bufferLength; i++) {
                if (bytes[i] == pattern[patternIndex]) {
                    patternIndex++;
                    if (patternIndex >= patternLength)
                        return i - patternLength + 1;
                } else {
                    if (patternIndex > 0)
                        i--;
                    patternIndex = 0;
                }
            }

            return -1;
        }

        internal virtual Stream CreateDecompressionStream(StreamProxy streamProxy) {
            if (IsEncrypted)
                return null;
            IDecompressionStrategy decompressionStrategy = CreateDecompressionStrategy();
            return decompressionStrategy.Decompress(streamProxy.CreateRawStream());
        }
        public virtual Stream CreateDecompressionStream() {
            if (IsEncrypted)
                return null;
            return CreateDecompressionStream(ContentRawDataStreamProxy);
        }
        protected virtual IDecompressionStrategy CreateDecompressionStrategy() {
            if (UncompressedSize == 0)
                return new ZeroLengthContentDecompressionStrategy();
            if (CompressionMethod == CompressionMethod.Deflate)
                return new DeflateDecompressionStrategy();
            return new NoCompressionDecompressionStrategy();
        }
    }
    #endregion
    #region StreamProxy
    public class StreamProxy {
        public static StreamProxy Create(Stream stream) {
            return new StreamProxy(stream, stream.Position, stream.Length, false);
        }
        public static StreamProxy Create(Stream stream, long position) {
            return new StreamProxy(stream, position, stream.Length, false);
        }

        readonly Stream baseStream;
        readonly long startPositionInBaseStream;
        readonly long length;
        bool isPackedStream = false;
        public StreamProxy(Stream baseStream, long startPositionInBaseStream, long length, bool isPackedStream) {
            //Guard.ArgumentNotNull(baseStream, "baseStream");
            this.isPackedStream = isPackedStream;
            this.baseStream = baseStream;
            this.startPositionInBaseStream = startPositionInBaseStream;
            this.length = length;
        }
        public Stream BaseStream { get { return baseStream; } }
        public long StartPositionInBaseStream { get { return startPositionInBaseStream; } }
        public long Length { get { return length; } }
        public bool IsPackedStream { get { return isPackedStream; } }

        public Stream CreateRawStream() {
            return new FixedOffsetSequentialReadOnlyStream(BaseStream, StartPositionInBaseStream, Length, IsPackedStream);
        }
    }
    #endregion
}
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Utils.Zip.Internal {
#endif
    #region FixedOffsetSequentialReadOnlyStream
    public class FixedOffsetSequentialReadOnlyStream : Stream {
        #region Fields
        readonly Stream baseStream;
        readonly long basePosition;
        readonly long length;
        long position;
        bool isPackedStream;
        #endregion

        public FixedOffsetSequentialReadOnlyStream(Stream baseStream, long length)
            : this(baseStream, baseStream.Position, length, false) {
        }
        public FixedOffsetSequentialReadOnlyStream(Stream baseStream, long basePosition, long length, bool isPackedStream) {
            this.baseStream = baseStream;
            this.basePosition = basePosition;
            this.length = length;
            this.isPackedStream = isPackedStream;
        }

        #region Properties
        public Stream BaseStream { get { return baseStream; } }
        public override bool CanRead { get { return BaseStream.CanRead; } }
        public override bool CanSeek { get { return BaseStream.CanSeek; } }
        public override bool CanWrite { get { return false; } }

        public override long Length {
            get {
                if (length < 0)
                    throw new NotSupportedException();
                return length;
            }
        }

        public override long Position {
            get { return position; }
            set {
                throw new NotSupportedException();
            }
        }
        #endregion

        protected internal virtual void ValidateBaseStreamPosition() {
            if (BaseStream.Position != basePosition + position)
                BaseStream.Seek(basePosition + position - BaseStream.Position, SeekOrigin.Current);
        }

        public override int Read(byte[] buffer, int offset, int count) {
            ValidateBaseStreamPosition();
            if (length >= 0 && !this.isPackedStream) {
                if (this.position + count > length)
                    count = (int)(length - this.position);
            }
            int actualByteCount = ReadFromBaseStream(buffer, offset, count);
            this.position += actualByteCount;
            return actualByteCount;
        }
        protected virtual int ReadFromBaseStream(byte[] buffer, int offset, int count) {
            return BaseStream.Read(buffer, offset, count);
        }
        protected void IncrementPosition(long offset) {
            this.position += offset;
        }
        public override long Seek(long offset, SeekOrigin origin) {
            if (!CanSeek)
                throw new NotSupportedException();
            if (origin < SeekOrigin.Begin || origin > SeekOrigin.End)
                throw new ArgumentException();
            if (origin == SeekOrigin.Begin)
                this.position = offset;
            else if (origin == SeekOrigin.Current)
                this.position = this.position + offset;
            else if (origin == SeekOrigin.End)
                this.position = Length - offset;
            if (this.position < 0)
                this.position = 0;
            else if (this.position > Length)
                this.position = Length;
            ValidateBaseStreamPosition();
            return position;
        }

        public override void SetLength(long value) {
            throw new NotSupportedException();
        }
        public override void Write(byte[] buffer, int offset, int count) {
            throw new NotSupportedException();
        }
        public override void Flush() {
            throw new NotSupportedException();
        }
    }
    #endregion
    public class CentralDirectoryEntry {
        public int Crc32 { get; set; }
        public int CompressedSize { get; set; }
        public int UncompressedSize { get; set; }
        public string FileName { get; set; }
        public int MsDosDateTime { get; set; }
        public int RelativeOffset { get; set; }
        public int FileAttributes { get; set; }
        public string Comment { get; set; }
        public IZipExtraFieldCollection ExtraFields { get; set; }
        public short GeneralPurposeFlag { get; set; }
        public CompressionMethod CompressionMethod { get; set; }
    }
    #region CompressionMethod
    public enum CompressionMethod {
        Store = 0,
        Shrunke = 1,
        Reduce1 = 2,
        Reduce2 = 3,
        Reduce3 = 4,
        Reduce4 = 5,
        Implode = 6,
        TokenizingCompression = 7,
        Deflate = 8,
        Deflate64 = 9,
        PkWareImplode = 10,
        //PkWareReserved1 = 11,
        BZip2 = 12,
        //PkWareReserved = 13,
        LZMA = 14,
        //PkWareReserved2 = 15,
        //PkWareReserved3 = 16,
        //PkWareReserved4 = 17,
        IbmTerse = 18,
        LZ77 = 19,
        PPMd11 = 98,
        AESEncryption = 99
    }
    #endregion
    #region ZipSignatures
    public enum ZipSignatures {
        FileRecord = 0x04034b50, // "PK\x03\x04"
        DataDescriptorRecord = 0x08074b50, // "PK\x07\x08"
        MultiVolumeArchiveRecord = 0x08074b50, // "PK\x07\x08"
        ArchiveExtraDataRecord = 0x08064b50, // "PK\x06\x08"
        FileEntryRecord = 0x02014b50, // "PK\x01\x02"
        DigitalSignatureRecord = 0x05054b50, // "PK\x05\x05"
        EndOfCentralDirSignature = 0x06054b50, // "PK\x05\x06"
    }
    #endregion


    public abstract class InternalZipFileParserCore<T> where T : InternalZipFile, new() {
        readonly IList<T> records;
        readonly Dictionary<string, T> zipDictionary;
        public InternalZipFileParserCore() {
            this.records = CreateRecords();
            this.zipDictionary = new Dictionary<string, T>();
        }

        protected IList<T> InnerRecords { get { return records; } }
        protected Dictionary<string, T> ZipDictionary { get { return zipDictionary; } }

        public virtual void Parse(Stream stream) {
            Parse(stream, Encoding.Default);
        }
        public void Parse(Stream stream, Encoding fileNameEncoding) {
            BinaryReader reader = new BinaryReader(stream);
            for (; ; ) {
                try {
                    ZipSignatures signature = (ZipSignatures)reader.ReadInt32();
                    bool isProcessed = ProcessZipRecord(reader, signature, fileNameEncoding);
                    if (!isProcessed)
                        break;
                } catch (EndOfStreamException) {
                    break;
                }
            }
        }
        void Add(T item) {
            this.records.Add(item);
            this.zipDictionary.Add(item.FileName, item);
        }
        bool ProcessZipRecord(BinaryReader reader, ZipSignatures signature, Encoding fileNameEncoding) {
            bool processed = true;
            switch (signature) {
                case ZipSignatures.FileRecord:
                    T record = ProcessZipFile(reader, fileNameEncoding);
                    Add(record);
                    break;
                case ZipSignatures.FileEntryRecord:
                    PorcessFileEntryRecord(reader, fileNameEncoding);
                    break;
                default:
                    processed = false;
                    break;
            }
            return processed;
        }
        protected virtual T ProcessZipFile(BinaryReader reader, Encoding fileNameEncoding) {
            T zipFile = CreateZipFileInstance();
            zipFile.DefaultEncoding = fileNameEncoding;
            zipFile.ReadLocalHeader(reader);
            return zipFile;
        }
        protected T FindRecordByName(string name) {
            if (zipDictionary.ContainsKey(name))
                return zipDictionary[name];
            return null;
        }
        protected virtual void PorcessFileEntryRecord(BinaryReader reader, Encoding fileNameEncoding) {
            //do nothing
        }
        protected virtual T CreateZipFileInstance() {
            return new T();
        }
        protected abstract IList<T> CreateRecords();
    }
    public class InternalZipFileParser : InternalZipFileParserCore<InternalZipFile> {
        public InternalZipFileCollection Records { get { return (InternalZipFileCollection)InnerRecords; } }

        protected override IList<InternalZipFile> CreateRecords() {
            return new InternalZipFileCollection();
        }
    }
    public interface ICompressionStrategy {
        void Compress(Stream sourceStream, Stream targetStream, IZipComplexOperationProgress progress);
        CompressionMethod CompressionMethod { get; }
        int Crc32 { get; }
        Int16 GetGeneralPurposeBitFlag();
        void PrepareExtraFields(IZipExtraFieldCollection extraFields);
    }
    public interface IDecompressionStrategy {
        Stream Decompress(Stream stream);
    }
    class UseCompressedStreamCompressionStrategy : ICompressionStrategy {
        CompressedStream stream;
        public UseCompressedStreamCompressionStrategy(CompressedStream stream) {
            this.stream = stream;
        }

        public CompressionMethod CompressionMethod { get { return CompressionMethod.Deflate; } }
        public int Crc32 { get { return (int)this.stream.Crc32; } }

        public void Compress(Stream sourceStream, Stream targetStream, IZipComplexOperationProgress progress) {
            CopyProgressHandler progressHandler = null;
            if (progress != null) {
                long totalLength = sourceStream.Length - sourceStream.Position;
                ZipCopyStreamOperationProgress operationProgress = new ZipCopyStreamOperationProgress(totalLength);
                progress.AddOperationProgress(progress);
                progressHandler = operationProgress.CopyHandler;
            }
            StreamUtils.CopyStream(sourceStream, targetStream, progressHandler);
        }
        public short GetGeneralPurposeBitFlag() {
            return 0;
        }
        public void PrepareExtraFields(IZipExtraFieldCollection extraFields) {
            //do nothing
        }
    }
    public class DeflateDecompressionStrategy : IDecompressionStrategy {
        public Stream Decompress(Stream rawStream) {
            return new DeflateStream(rawStream, CompressionMode.Decompress, true);
        }
    }
    public class NoCompressionDecompressionStrategy : IDecompressionStrategy {
        public Stream Decompress(Stream stream) {
            return stream;
        }
    }
    public class ZeroLengthContentDecompressionStrategy : IDecompressionStrategy {
        public Stream Decompress(Stream stream) {
            return new MemoryStream();
        }
    }
    public class StoreCompressionStrategy : ICompressionStrategy {
        int crc32 = 0;

        public CompressionMethod CompressionMethod { get { return CompressionMethod.Store; } }
        public int Crc32 { get { return crc32; } }

        public void Compress(Stream sourceStream, Stream targetStream, IZipComplexOperationProgress progress) {
            CopyProgressHandler progressHandler = null;
            if (progress != null) {
                long sourceStreamLength = 0;
                try {
                    sourceStreamLength = sourceStream.Position;
                } catch (NotSupportedException) { }
                long totalLength = sourceStream.Length - sourceStreamLength;
                ZipCopyStreamOperationProgress operationProgress = new ZipCopyStreamOperationProgress(totalLength);
                progress.AddOperationProgress(operationProgress);
                progressHandler = operationProgress.CopyHandler;
            }
            Crc32Stream crc32Stream = new Crc32Stream(sourceStream);
            StreamUtils.CopyStream(crc32Stream, targetStream, progressHandler);
            this.crc32 = (int)crc32Stream.ReadCheckSum;
        }
        public short GetGeneralPurposeBitFlag() {
            return 0;
        }
        public void PrepareExtraFields(IZipExtraFieldCollection extraFields) {
            //do nothing
        }
    }
    public class DeflateCompressionStrategy : ICompressionStrategy {
        int crc32 = 0;

        public CompressionMethod CompressionMethod { get { return CompressionMethod.Deflate; } }
        public int Crc32 { get { return crc32; } }

        public void Compress(Stream sourceStream, Stream targetStream, IZipComplexOperationProgress progress) {
            CopyProgressHandler progressHandler = null;
            if (progress != null) {
                long sourceStreamLength = 0;
                try {
                    sourceStreamLength = sourceStream.Position;
                } catch (NotSupportedException) { }
                long totalLength = sourceStream.Length - sourceStreamLength;
                ZipCopyStreamOperationProgress operationProgress = new ZipCopyStreamOperationProgress(totalLength);
                progress.AddOperationProgress(operationProgress);
                progressHandler = operationProgress.CopyHandler;
            }
            Crc32Stream crc32Stream = new Crc32Stream(sourceStream);
            using (Stream compressStream = CreateDeflateStream(targetStream)) {
                StreamUtils.CopyStream(crc32Stream, compressStream, progressHandler);
            }
            this.crc32 = (int)crc32Stream.ReadCheckSum;
        }
        Stream CreateDeflateStream(Stream stream) {
            return new DeflateStream(stream, CompressionMode.Compress, true);
        }
        public short GetGeneralPurposeBitFlag() {
            return 0;
        }
        public void PrepareExtraFields(IZipExtraFieldCollection extraFields) {
            //do nothing
        }
    }
    public static class StreamUtils {
        public static void CopyStream(Stream sourceStream, Stream targetStream) {
            CopyStream(sourceStream, targetStream, null);
        }
        public static void CopyStream(Stream sourceStream, Stream targetStream, CopyProgressHandler copyDelegate) {
            const int bufferSize = 32768;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            bool canContinue = true;
            do {
                bytesRead = sourceStream.Read(buffer, 0, bufferSize);
                targetStream.Write(buffer, 0, bytesRead);
                if (copyDelegate != null)
                    canContinue = copyDelegate(bytesRead);
            }
            while (bytesRead == bufferSize && canContinue);
        }
        public static void MakeReadingPass(Stream sourceStream, CopyProgressHandler copyDelegate) {
            const int bufferSize = 32768;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            bool canContinue = true;
            do {
                bytesRead = sourceStream.Read(buffer, 0, bufferSize);
                if (copyDelegate != null)
                    canContinue = copyDelegate(bytesRead);
            }
            while (bytesRead == bufferSize && canContinue);
        }
    }
    public static class ZipEncodingHelper {
        public static bool CanCodeToASCII(string sourceString) {
            return CanCodeToEncoding(Encoding.ASCII, sourceString);
        }
        public static bool CanCodeToEncoding(Encoding encoding, string sourceString) {
            byte[] bytes = encoding.GetBytes(sourceString);
            string reconvertedString = encoding.GetString(bytes, 0, bytes.Length);
            return reconvertedString == sourceString;
        }
    }
    public static class ZipDateTimeHelper {
        public static int ToMsDosDateTime(DateTime dateTime) {
            int num = 0;
            num |= (int)((dateTime.Second / 2) & 0x1f);
            num |= (int)((dateTime.Minute & 0x3f) << 5);
            num |= (int)((dateTime.Hour & 0x1f) << 11);
            num |= (int)((dateTime.Day & 0x1f) << 0x10);
            num |= (int)((dateTime.Month & 15) << 0x15);
            return (num | ((int)(((dateTime.Year - 0x7bc) & 0x7f) << 0x19)));
        }
        public static DateTime FromMsDos(int data) {
            uint udata = (uint)data;
            int second = (int)((udata & 0x1f) * 2);
            int minute = (int)((udata >> 5) & 0x3f);
            int hour = (int)((udata >> 11) & 0x1f);
            int day = (int)((udata >> 0x10) & 0x1f);
            int month = (int)((udata >> 0x15) & 15);
            int yaar = (int)((udata >> 0x19) + 0x7bc);
            return new DateTime(yaar, month, day, hour, minute, second);
        }
    }
    public delegate bool CopyProgressHandler(int bytesCopied);
    public interface IZipOperationProgress {
        double CurrentProgress { get; }
        double Weight { get; }
        bool IsStopped { get; }

        event EventHandler NotifyProgress;
        void Stop();
    }
    public interface IZipComplexOperationProgress : IZipOperationProgress {
        void AddOperationProgress(IZipOperationProgress progressItem);
    }
    public class ZipCopyStreamOperationProgress : IZipOperationProgress {
        double weight;
        long totalSize;
        long totalBytesCopied;
        double currentProgress;
        bool isStopped;

        public ZipCopyStreamOperationProgress(long totalSize)
            : this(totalSize, 1) {
        }
        public ZipCopyStreamOperationProgress(long totalSize, double weight) {
            this.weight = weight;
            this.totalSize = totalSize;
        }
        public double CurrentProgress { get { return currentProgress; } }
        public double Weight { get { return weight; } }
        public long TotalSize { get { return totalSize; } }
        public bool IsStopped { get { return isStopped; } }

        public event EventHandler NotifyProgress;

        public bool CopyHandler(int bytesCopied) {
            if (this.totalSize <= 0)
                return !isStopped;
            this.totalBytesCopied += bytesCopied;
            this.currentProgress = (1.0 * this.totalBytesCopied) / this.totalSize;
            if (NotifyProgress != null)
                NotifyProgress(this, EventArgs.Empty);
            return !isStopped;
        }
        public void Stop() {
            this.isStopped = true;
        }
    }

    #region ExtraFields
    public interface IZipExtraField {
        ExtraFieldType Type { get; }
        short Id { get; }
        short ContentSize { get; }

        void AssignRawData(BinaryReader reader);
        void Apply(InternalZipFile zipFile);
        void Write(BinaryWriter writer);
    }
    #region ZipExtraField
    public abstract class ZipExtraField : IZipExtraField {
        public abstract short Id { get; }
        public abstract ExtraFieldType Type { get; }
        public abstract short ContentSize { get; }
        public abstract void Apply(InternalZipFile zipFile);
        public abstract void AssignRawData(BinaryReader reader);
        public abstract void Write(BinaryWriter writer);
    }
    #endregion

    #region ExtraFieldType
    [Flags]
    public enum ExtraFieldType { LocalHeader = 1, CentralDirectoryEntry = 2, Both = LocalHeader | CentralDirectoryEntry }
    #endregion
    public interface IZipExtraFieldCollection {
        short CalculateSize(ExtraFieldType fieldType);
        void Write(BinaryWriter writer, ExtraFieldType fieldType);
        void Add(IZipExtraField field);
    }
    public interface IZipExtraFieldFactory {
        IZipExtraField Create(int headerId);
    }
    public class FactorySingleton<T> where T : class, new() {
        static T instance;
        public static T Instance {
            get {
                if (instance == null)
                    instance = new T();
                return instance;
            }
        }
    }
    public class InternalZipExtraFieldFactoryInstance : FactorySingleton<InternalZipExtraFieldFactory> { }
    public class InternalZipExtraFieldFactory : IZipExtraFieldFactory {
        public virtual IZipExtraField Create(int headerId) {
            if (headerId == Zip64ExtraField.HeaderId)
                return new Zip64ExtraField();
            return null;
        }
    }
    public class ZipExtraFieldComposition : IZipExtraFieldCollection {
        const int HeaderInfoSize = 4;//headerId + size fields
        public static ZipExtraFieldComposition Read(BinaryReader reader, long extraFieldsLength, IZipExtraFieldFactory headerFactory) {
            ZipExtraFieldComposition result = new ZipExtraFieldComposition();
            long basePosition = reader.BaseStream.Position;
            long endExtraFieldPosition = basePosition + extraFieldsLength;
            while (reader.BaseStream.Position < endExtraFieldPosition) {
                short headerId = (short)reader.ReadUInt16();
                ushort size = reader.ReadUInt16();
                long contentFieldStartPosition = reader.BaseStream.Position;
                long endFieldPosition = contentFieldStartPosition + size;
                IZipExtraField field = headerFactory.Create(headerId);
                FixedOffsetSequentialReadOnlyStream dataStream = new FixedOffsetSequentialReadOnlyStream(reader.BaseStream, size);
                if (field != null) {
                    BinaryReader dataStreamReader = new BinaryReader(dataStream);
                    field.AssignRawData(dataStreamReader);
                    result.Add(field);
                }
                long bytesToSkip = endFieldPosition - reader.BaseStream.Position;
                System.Diagnostics.Debug.Assert(bytesToSkip >= 0);
                SkipUnusedBytes(dataStream, bytesToSkip);
            }
            return result;
        }
        static void SkipUnusedBytes(FixedOffsetSequentialReadOnlyStream dataStream, long bytesToSkip) {
            if (dataStream.CanSeek) {
                dataStream.Seek(0, SeekOrigin.End);
                return;
            }
            for (long i = 0; i < bytesToSkip; i++)
                dataStream.ReadByte();
        }

        List<IZipExtraField> fields = new List<IZipExtraField>();
        public List<IZipExtraField> Fields { get { return fields; } }

        public void Apply(InternalZipFile zipFile) {
            int count = this.fields.Count;
            for (int i = 0; i < count; i++)
                this.fields[i].Apply(zipFile);
        }
        public void Add(IZipExtraField field) {
            this.fields.Add(field);
        }
        public void Write(BinaryWriter writer, ExtraFieldType type) {
            int count = Fields.Count;
            for (int i = 0; i < count; i++) {
                IZipExtraField field = Fields[i];
                if ((field.Type & type) == 0)
                    continue;
                writer.Write(field.Id);
                writer.Write(field.ContentSize);
                field.Write(writer);
            }
        }
        public short CalculateSize(ExtraFieldType type) {
            short size = 0;
            int count = Fields.Count;
            for (int i = 0; i < count; i++) {
                IZipExtraField field = Fields[i];
                if ((field.Type & type) == 0)
                    continue;
                short totalFieldSize = (short)(field.ContentSize + HeaderInfoSize);
                size += totalFieldSize;
            }
            return size;
        }
    }

    public class Zip64ExtraField : ZipExtraField {
        public const int HeaderId = 0x1;

        public override short Id { get { return HeaderId; } }
        public override ExtraFieldType Type { get { return ExtraFieldType.Both; } }
        public override short ContentSize { get { return 30; } }

        public long UncompressedSize { get; set; }
        public long CompressedSize { get; set; }
        
        public override void AssignRawData(BinaryReader reader) {
            UncompressedSize = reader.ReadInt64();
            CompressedSize = reader.ReadInt64();
        }
        public override void Apply(InternalZipFile zipFile) {
            zipFile.CompressedSize = CompressedSize;
            zipFile.UncompressedSize = UncompressedSize;
        }
        public override void Write(BinaryWriter writer) {
            //TODO: implement
        }
    }
    #endregion
}
