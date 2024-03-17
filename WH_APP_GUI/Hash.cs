using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WH_APP_GUI
{
    internal class Hash
    {
        static string HashPassword(string password)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password);

            byte[] result = sha256.ComputeHash(bytes);

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                stringBuilder.Append(result[i].ToString("x2")); 
                //Az x2 az egy formázási opció amit a ToString metódusnak adhatsz át byte típusú adatok konvertálásához stringé hexadecimális formában és ez JÓ
            }

            return stringBuilder.ToString();
        }

        static bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            string hashedInputPassword = HashPassword(inputPassword);
            return hashedInputPassword == hashedPassword;
        }
    }
}
