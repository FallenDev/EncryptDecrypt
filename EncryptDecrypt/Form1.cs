using System.Security.Cryptography;
using System.Text;

namespace EncryptDecrypt
{
    public partial class Form1 : Form
    {
        private string Input => textBox1.Text;
        private static byte[]? _key;
        private static byte[]? _tag;
        private static byte[]? _textBytes;
        private static byte[]? _cipher;
        private static byte[]? _nonce;

        public Form1()
        {
            InitializeComponent();
            PlayerLoggedInGenerateKey();
        }

        // Generate Key
        private void PlayerLoggedInGenerateKey()
        {
            _key = new byte[32];
            RandomNumberGenerator.Fill(_key);
        }


        // Encrypt
        private void button1_Click(object sender, EventArgs e)
        {
            if (Input.Length <= 0) return;

            //var encrypt = Encrypt(Input, _key);
            _textBytes = Encoding.UTF8.GetBytes(Input);
            var encrypt = EncryptRc4(_key, _textBytes);
            //Console.Write($"Cipher Length: {encrypt.cipher.Length}\n");
            //Console.Write($"Nonce Length: {encrypt.nonce.Length}\n");
            //Console.Write($"Tag Length: {encrypt.tag.Length}\n");
            Console.Write($"Cipher: {encrypt.Length}\n");
        }

        private static (byte[] cipher, byte[] nonce, byte[] tag) Encrypt(string data, byte[] key)
        {
            using var aesEncrypt = new AesGcm(key);
            _nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
            RandomNumberGenerator.Fill(_nonce);

            _tag = new byte[AesGcm.TagByteSizes.MaxSize];
            _textBytes = Encoding.UTF8.GetBytes(data);
            _cipher = new byte[_textBytes.Length];

            aesEncrypt.Encrypt(_nonce, _textBytes, _cipher, _tag);
            return (_cipher, _nonce, _tag);
        }

        private static byte[] EncryptRc4(IReadOnlyList<byte> pwd, IReadOnlyList<byte> data)
        {
            int a, i, j;
            int tmp;

            var key = new int[256];
            var box = new int[256];
            var cipher = new byte[data.Count];

            for (i = 0; i < 256; i++)
            {
                key[i] = pwd[i % pwd.Count];
                box[i] = i;
            }

            for (j = i = 0; i < 256; i++)
            {
                j = (j + box[i] + key[i]) % 256;
                tmp = box[i];
                box[i] = box[j];
                box[j] = tmp;
            }

            for (a = j = i = 0; i < data.Count; i++)
            {
                a++;
                a %= 256;
                j += box[a];
                j %= 256;
                tmp = box[a];
                box[a] = box[j];
                box[j] = tmp;
                var k = box[((box[a] + box[j]) % 256)];
                cipher[i] = (byte)(data[i] ^ k);
            }

            _cipher = cipher;

            return cipher;
        }


        // Decrypt
        private void button2_Click(object sender, EventArgs e)
        {
            //if (_tag == null || _cipher == null || _nonce == null || _textBytes == null) return;

            //var decrypt = Decrypt(_cipher, _nonce, _tag, _key);
            var decrypt = DecryptRc4(_key, _cipher);
            Console.Write(decrypt.Equals(Input) ? $"Decryption Succeeded: {decrypt}\n" : $"Decryption was not successful: {decrypt}\n");
        }

        private static string Decrypt(byte[] cipher, byte[] nonce, byte[] tag, byte[]? key)
        {
            using var aes = new AesGcm(key);
            var textBytes = new byte[cipher.Length];
            
            aes.Decrypt(nonce, cipher, tag, textBytes);
            return Encoding.UTF8.GetString(textBytes);
        }

        private static string DecryptRc4(IReadOnlyList<byte> pwd, IReadOnlyList<byte> data)
        {
            var textBytes = EncryptRc4(pwd, data);

            return Encoding.UTF8.GetString(textBytes);
        }
    }
}