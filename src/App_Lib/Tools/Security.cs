using System;
using System.Collections.Generic;
using System.Linq;

using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace src.App_Lib.Tools
{
    public class Security
    {
        static readonly char[] padding = { '=' };
        public static string Encrypt(string clearText, string EncryptionKey)
        {
            try
            {
                byte[] clearBytes = Encoding.UTF8.GetBytes(clearText);

                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, Encoding.UTF8.GetBytes(EncryptionKey));
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray()).TrimEnd(padding).Replace('+', '-').Replace('/', '_');
                    }
                }
                return clearText;
            }
            catch
            {

            }
            return "";
        }

        public static string Decrypt(string cipherText, string EncryptionKey)
        {
            string incoming = cipherText.Replace('_', '/').Replace('-', '+');

            switch (cipherText.Length % 4)
            {
                case 2: incoming += "=="; break;
                case 3: incoming += "="; break;
            }
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(incoming);

                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, Encoding.UTF8.GetBytes(EncryptionKey));
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }
            catch
            {

            }
            return "";
        }



    }

}