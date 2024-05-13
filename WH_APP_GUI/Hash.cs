using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WH_APP_GUI
{
    class Hash
    {
        public static string HashPassword(string password)
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

        public static bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            string hashedInputPassword = HashPassword(inputPassword);
            return hashedInputPassword == hashedPassword;
        }

        public static int GenerateRandomNumber(int min, int max)
        {
            max++;
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] randomNumber = new byte[4];
                rng.GetBytes(randomNumber);

                int generatedNumber = BitConverter.ToInt32(randomNumber, 0);

                return new Random(generatedNumber).Next(min, max);
            }
        }
        public static string GenerateRandomPassword()
        {
            int passwordLength = GenerateRandomNumber(6, 12);
            StringBuilder password = new StringBuilder();

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < passwordLength; i++)
                {
                    byte[] randomBytes = new byte[1];
                    rng.GetBytes(randomBytes);
                    byte b = randomBytes[0];

                    int type = b % 3;

                    switch (type)
                    {
                        case 0:
                            password.Append((char)(b % 26 + 65)); // Nagybetü
                            break;
                        case 1:
                            password.Append((char)(b % 26 + 97)); // Kisbetü
                            break;
                        case 2:
                            password.Append((char)(b % 10 + 48)); // Szám
                            break;
                    }
                }
            }
            return password.ToString();
        }
    }
}
