using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Kashefi.Security
{
    public class ReverseEngineeringProtector
    {
        private byte[] m_key = new byte[16];
        
        public void CreateKey(string key)
        {
            MD5 md5hash = MD5.Create();

            m_key = md5hash.ComputeHash(Encoding.UTF8.GetBytes(key));
        }

        public byte[] Encrypt(string content)
        {
            
            RC2CryptoServiceProvider RC2ServiceProvider = new RC2CryptoServiceProvider();
            ICryptoTransform RC2Encryptor = RC2ServiceProvider.CreateEncryptor(m_key, m_key);

            MemoryStream MS = new MemoryStream();
            CryptoStream CS = new CryptoStream(MS, RC2Encryptor, CryptoStreamMode.Write);

            StreamWriter SW = new StreamWriter(CS);
            SW.WriteLine(content);
            SW.Close();

            CS.Close();

            return MS.ToArray();
        }

        public string Dycrypt(byte[] content)
        {
            RC2CryptoServiceProvider RC2ServiceProvider = new RC2CryptoServiceProvider();
            ICryptoTransform RC2Dycryptor = RC2ServiceProvider.CreateDecryptor(m_key, m_key);

            MemoryStream MS = new MemoryStream(content);
            CryptoStream CS = new CryptoStream(MS, RC2Dycryptor, CryptoStreamMode.Read);

            StreamReader SR = new StreamReader(CS);
            string output = SR.ReadToEnd();
            
            SR.Close();
            CS.Close();
            MS.Close();

            return output;
        }

    }

}
