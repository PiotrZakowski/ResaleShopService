using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace RESTService.Managers
{
    public class CryptoService
    {
        private int SaltSize { get; set; }

        private int HashSize { get; set; }

        private int HashIter { get; set; } //Min. 1000

        public CryptoService()
        {
            this.SaltSize = 16;

            this.HashSize = 20;

            this.HashIter = 10000;
        }

        public CryptoService(int saltSize, int hashSize, int hashIter)
        {
            this.SaltSize = saltSize;

            this.HashSize = hashSize;

            this.HashIter = hashIter;
        }

        public string GetHashedString(string stringToBeHashed)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[this.SaltSize]);

            var pbkdf2 = new Rfc2898DeriveBytes(stringToBeHashed, salt, this.HashIter);
            byte[] hash = pbkdf2.GetBytes(this.HashSize);

            byte[] hashBytes = new byte[this.SaltSize + this.HashSize];
            Array.Copy(salt, 0, hashBytes, 0, this.SaltSize);
            Array.Copy(hash, 0, hashBytes, this.SaltSize, this.HashSize);

            string hashedString = Convert.ToBase64String(hashBytes);
            return hashedString;
        }

        public bool CompareStringToHash(string str, string hashedStr)
        {
            byte[] hashBytes = Convert.FromBase64String(hashedStr);
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            var pbkdf2 = new Rfc2898DeriveBytes(str, salt, this.HashIter);
            byte[] hash = pbkdf2.GetBytes(this.HashSize);

            bool result = true;
            for (int i = 0; i < this.HashSize; i++)
                if (hashBytes[i + this.SaltSize] != hash[i])
                {
                    result = false;
                    break;
                }

            return result;
        }
    }
}