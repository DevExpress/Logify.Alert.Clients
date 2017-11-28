using System;

namespace DevExpress.Logify.Core.Internal {
    [CLSCompliant(false)]
    public class CRC32custom {
        uint[] tab;
        uint poly = 0;
        public CRC32custom() : this(0x04c11db7) { }
        public CRC32custom(uint poly) {
            this.poly = poly;
        }
        void Init() {
            if (tab != null) return;
            tab = new uint[256];
            for (uint i = 0; i < 256; i++) {
                uint t = i;
                for (int j = 0; j < 8; j++)
                    if ((t & 1) == 0)
                        t >>= 1;
                    else
                        t = (t >> 1) ^ poly;
                tab[i] = t;
            }
        }
        static CRC32custom _default;
        public static CRC32custom Default {
            get {
                if (_default == null) _default = new CRC32custom();
                return _default;
            }
        }
        public uint ComputeHash(string text) {
            return ComputeHash(System.Text.Encoding.UTF8.GetBytes(text));
        }
        public uint ComputeHash(byte[] data) {
            return ComputeHash(data, 0, data.Length);
        }
        public virtual uint ComputeHash(byte[] data, int start, int length) {
            return ComputeHash<byte[]>(data, start, length);
        }
        public virtual uint ComputeHash<T>(T data, int start, int length) where T : System.Collections.IList {
            Init();
            uint hash = 0xFFFFFFFF;
            for (int n = 0; n < length; n++) {
                byte b = (byte)data[n + start];
                hash = (hash << 8)
                    ^ tab[b ^ (hash >> 24)];
            }
            return ~hash;
        }
    }
}
