/*
 * Copyright 2016 Wouter Huysentruit
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Idento.Core.Cryptography
{
    public class Crypto
    {
        private const char PasswordHashingIterationCountSeparator = '.';
        private const int Pbkdf2SubkeyLength = 256 / 8; // 256 bits
        private const int SaltSize = 128 / 8; // 128 bits
        private static readonly int? FixedIterationCount = 10000;

        private enum PasswordHashingVersion : byte
        {
            Version1 = 1
        }

        public static byte[] GenerateRandomHash(int length)
        {
            if (length <= 0)
                throw new ArgumentException("Invalid length", nameof(length));

            var bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);

            return bytes;
        }

        public static string GenerateRandomToken(int length, string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            if (allowedChars == null)
                throw new ArgumentNullException(nameof(allowedChars));

            if (allowedChars.Length == 0)
                throw new ArgumentException("Should not be empty", nameof(allowedChars));

            var randomHash = GenerateRandomHash(length);
            var result = new StringBuilder();
            foreach (var value in randomHash)
                result.Append(allowedChars[value % allowedChars.Length]);

            return result.ToString();
        }

        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("password cannot be empty or null", nameof(password));

            var count = FixedIterationCount ?? GetIterationsFromYear(DateTime.Now.Year);
            var result = HashPassword(password, count);
            return EncodeIterations(count) + PasswordHashingIterationCountSeparator + result;
        }

        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword))
                throw new ArgumentException("hashedPassword cannot be empty or null", nameof(hashedPassword));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("password cannot be empty or null", nameof(password));

            var parts = hashedPassword.Split(PasswordHashingIterationCountSeparator);
            if (parts.Length != 2)
                return false;

            int count = DecodeIterations(parts[0]);
            if (count <= 0)
                return false;

            hashedPassword = parts[1];
            return VerifyHashedPassword(hashedPassword, password, count);
        }

        private static int GetIterationsFromYear(int year)
        {
            int startYear = 2000;
            int startCount = 1000;
            if (year > startYear)
            {
                var diff = (year - startYear) / 2;
                var mul = (int)Math.Pow(2, diff);
                var count = (long)startCount * (long)mul;
                return (int)Math.Min(count, int.MaxValue);
            }

            return startCount;
        }

        private static string EncodeIterations(int count)
        {
            return count.ToString("X");
        }

        private static int DecodeIterations(string prefix)
        {
            int val;
            return int.TryParse(prefix, NumberStyles.HexNumber, null, out val) ? val : -1;
        }

        private static string HashPassword(string password, int iterationCount)
        {
            byte[] salt;
            byte[] subkey;
            using (var deriveBytes = new Rfc2898DeriveBytes(password, SaltSize, iterationCount))
            {
                salt = deriveBytes.Salt;
                subkey = deriveBytes.GetBytes(Pbkdf2SubkeyLength);
            }

            var outputBytes = new byte[1 + SaltSize + Pbkdf2SubkeyLength];
            outputBytes[0] = (byte)PasswordHashingVersion.Version1;
            Buffer.BlockCopy(salt, 0, outputBytes, 1, SaltSize);
            Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SaltSize, Pbkdf2SubkeyLength);
            return Convert.ToBase64String(outputBytes);
        }

        private static bool VerifyHashedPassword(string hashedPassword, string password, int iterationCount)
        {
            var hashedPasswordBytes = Convert.FromBase64String(hashedPassword);
            var version = (PasswordHashingVersion)hashedPasswordBytes[0];

            if (version != PasswordHashingVersion.Version1 && hashedPasswordBytes.Length != (1 + SaltSize + Pbkdf2SubkeyLength))
                return false;

            var salt = new byte[SaltSize];
            Buffer.BlockCopy(hashedPasswordBytes, 1, salt, 0, SaltSize);
            var storedSubkey = new byte[Pbkdf2SubkeyLength];
            Buffer.BlockCopy(hashedPasswordBytes, 1 + SaltSize, storedSubkey, 0, Pbkdf2SubkeyLength);

            byte[] generatedSubkey;
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterationCount))
                generatedSubkey = deriveBytes.GetBytes(Pbkdf2SubkeyLength);

            return storedSubkey.SequenceEqual(generatedSubkey);
        }
    }
}