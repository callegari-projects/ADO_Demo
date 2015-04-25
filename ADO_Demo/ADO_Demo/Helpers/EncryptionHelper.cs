using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Helpers
{
    public static class EncryptionHelper
    {
        public enum Format
        {
            AsciiNumerals,
            Base64String,
            UrlEncodedString,
            HtmlEncodedString
        }

        /// <summary>
        /// This encrypts a string into bytes using RijndaelManaged then returns them as a string representative of those bytes.
        /// </summary>
        /// <param name="sourceString">The string to be encrypted.</param>
        /// <param name="KeyIVBaseString">This string is used to generate the algorithm Key and IV</param>
        /// <returns>Returns a string representation of the encryption bytes where each byte takes up 3 spaces (with leading zeros) eg. "057"</returns>
        /// <remarks></remarks>
        public static string RijndaelEncrypt(string sourceString, string KeyIVBaseString, Format RijndaelFormat)
        {

            //Validation
            if (string.IsNullOrEmpty(KeyIVBaseString))
            {
                throw new ArgumentException("KeyIVBaseString is required for Encryption."); //Please pass a value for the Rijndael key before calling this method
            }

            //if (KeyIVBaseString.Length < 32)
            //{
            //    throw new ArgumentException("RijndaelBase does not meet the minimum required length of 32.");
            //}


            // RijndaelManaged will use a byte array for the IV and the Key, this converts a known string to the byte arrays
            byte[] companyIV = BuildIV(KeyIVBaseString);
            byte[] companyKey = BuildKey(KeyIVBaseString);

            byte[] encryptedSecretBytes = null;
            // Create an RijndaelManaged object with the specified key and IV. 
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {


                rijAlg.Key = companyKey;
                rijAlg.IV = companyIV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(sourceString);
                        }
                        encryptedSecretBytes = msEncrypt.ToArray();
                    }
                }

            }

            switch (RijndaelFormat)
            {
                case Format.AsciiNumerals:
                    //Convert Bytes to string of numerals
                    StringBuilder encryptionAsNumerals = new StringBuilder();
                    foreach (byte b in encryptedSecretBytes)
                    {
                        encryptionAsNumerals.Append(b.ToString("000"));
                        // add leading zeros if needed, so it's always 3 digits
                    }

                    //result is like: 077218215025056099228153045158070103158105
                    return encryptionAsNumerals.ToString();
                case Format.Base64String:
                    //result is like: lWklSb8hkOyP6VWDfZoxQ+NNqWNncptj34qwU3KEP5Y=
                    return Convert.ToBase64String(encryptedSecretBytes);
                case Format.UrlEncodedString:
                    return System.Web.HttpUtility.UrlEncode(Convert.ToBase64String(encryptedSecretBytes));
                case Format.HtmlEncodedString:
                    return System.Web.HttpUtility.HtmlEncode(Convert.ToBase64String(encryptedSecretBytes));
                default:
                    return "unimplemented RijndaelFormat";
            }



        }

        /// <summary>
        /// This decrypts a bytes into a string using RijndaelManaged.
        /// </summary>
        /// <param name="sourceEncryption">This should be a string representation of the bytes where every bytes takes up exactly 3 spaces.</param>
        /// <param name="KeyIVBaseString">This string is used to generate the algorithm Key and IV</param>
        /// <returns>Returns the plain text of the encrypted bytes</returns>
        /// <remarks></remarks>
        public static string RijndaelDecrypt(string sourceEncryption, string KeyIVBaseString, Format RijndaelFormat)
        {

            //Validation
            if (string.IsNullOrEmpty(KeyIVBaseString))
            {
                throw new ArgumentException("RijndaelBase is unassigned. Please assign a value for the Rijndael key before calling this method.");
            }
            //if (KeyIVBaseString.Length < 32)
            //{
            //    throw new ArgumentException("RijndaelBase does not meet the minimum required length of 32.");
            //}


            // RijndaelManaged will use a byte array for the IV and the Key, this converts a known string to the byte arrays
            byte[] companyIV = BuildIV(KeyIVBaseString);
            byte[] companyKey = BuildKey(KeyIVBaseString);

            byte[] encryptedSecretKeyBytesReturned = null;
            switch (RijndaelFormat)
            {
                case Format.AsciiNumerals:
                    //Convert string characters into true bytes
                    //result is like: 077218215025056099228153045158070103158105
                    ArrayList byteList = new ArrayList();
                    int adjustedLength = (sourceEncryption.Length / 3);
                    //since the 3 digit characters make one byte, and is 0 based
                    for (int i = 0; i < adjustedLength; i++)
                    {
                        byteList.Add(byte.Parse(sourceEncryption.Substring(i * 3, 3)));
                    }

                    encryptedSecretKeyBytesReturned = (byte[])byteList.ToArray(typeof(byte));
                    break;
                case Format.Base64String:
                    //result is like: lWklSb8hkOyP6VWDfZoxQ+NNqWNncptj34qwU3KEP5Y=
                    encryptedSecretKeyBytesReturned = Convert.FromBase64String(sourceEncryption);
                    break;
                case Format.UrlEncodedString:
                    encryptedSecretKeyBytesReturned = Convert.FromBase64String(System.Web.HttpUtility.UrlDecode(sourceEncryption));
                    break;
                case Format.HtmlEncodedString:
                    encryptedSecretKeyBytesReturned = Convert.FromBase64String(System.Web.HttpUtility.HtmlEncode(sourceEncryption));
                    break;
                default:
                    return "unimplemented RijndaelFormat";
            }




            //Convert string characters into true bytes
            //Dim adjustedLength As Integer = (sourceEncryption.Length / 3) - 1  'since the 3 digit characters make one byte, and is 0 based
            //Dim encryptedSecretKeyBytesReturned(adjustedLength) As Byte
            //For i = 0 To (adjustedLength)
            //    If i > 30 Then
            //        Dim B = "brk"
            //    End If
            //    encryptedSecretKeyBytesReturned(i) = Byte.Parse(sourceEncryption.Substring(i * 3, 3))
            //Next

            // Declare the string used to hold the decrypted text.
            string plaintextResult = null;

            // Create an RijndaelManaged object with the specified key and IV. 
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = companyKey;
                rijAlg.IV = companyIV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                //Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(encryptedSecretKeyBytesReturned))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            //// Read the decrypted bytes from the decrypting stream and place them in a string.
                            try
                            {
                                plaintextResult = srDecrypt.ReadToEnd();
                            }
                            catch (Exception)
                            {
                                plaintextResult = "";
                            }
                        }
                    }
                }
            }

            return plaintextResult;
        }

        private static byte[] BuildKey(string KeyIVBaseString)
        {
            if (KeyIVBaseString.Length < 32)
            {
                KeyIVBaseString = KeyIVBaseString + "~`Th1$isCr7pti(Text2S@yTru$tjEsUs=L!FE"; //DavB did this
            }
            return System.Text.Encoding.UTF8.GetBytes(KeyIVBaseString.Substring(0, 32));
            //32 length
        }

        private static byte[] BuildIV(string KeyIVBaseString)
        {
            if (KeyIVBaseString.Length < 32)
            {
                KeyIVBaseString = KeyIVBaseString + "`_Tru$tjEsUs=L!FE"; //DavB did this
            }
            return System.Text.Encoding.UTF8.GetBytes(KeyIVBaseString.Insert(7, "~").Substring(2, 16)); //vary the key to create variation in IV salt
            //16 length
        }

        // Above, the past, below, the future

        public static string EncryptRijndael(string text, string Key, string IV)
        {
            Validate(text, Key, IV);

            byte[] encryptedBytes = null;

            using (var algorithm = new System.Security.Cryptography.RijndaelManaged())
            {
                algorithm.Key = System.Text.Encoding.UTF8.GetBytes(Key.Substring(0, 32));
                algorithm.IV = System.Text.Encoding.UTF8.GetBytes(IV.Substring(0, 16));


                var encryptor = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV);
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    using (var cryptoStream = new System.Security.Cryptography.CryptoStream(memoryStream, encryptor, System.Security.Cryptography.CryptoStreamMode.Write))
                    {
                        using (var writer = new System.IO.StreamWriter(cryptoStream))
                        {
                            writer.Write(text);
                        }
                        encryptedBytes = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        public static string DecryptRijndael(string encryptedText, string Key, string IV)
        {
            Validate(encryptedText, Key, IV);

            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            using (var algorithm = new System.Security.Cryptography.RijndaelManaged())
            {
                algorithm.Key = System.Text.Encoding.UTF8.GetBytes(Key.Substring(0, 32));
                algorithm.IV = System.Text.Encoding.UTF8.GetBytes(IV.Substring(0, 16));


                var decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV);
                using (var memoryStream = new System.IO.MemoryStream(encryptedBytes))
                {
                    using (var cryptoStream = new System.Security.Cryptography.CryptoStream(memoryStream, decryptor, System.Security.Cryptography.CryptoStreamMode.Read))
                    {
                        using (var reader = new System.IO.StreamReader(cryptoStream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }

        private static bool Validate(string text, string Key, string IV)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Encryption failed because the text provided is empty.");
            }
            if (Key == null)
            {
                throw new ArgumentException("Encryption failed because the \"Key\" provided was missing.");
            }
            if (Key.Length != 32)
            {
                throw new ArgumentException("Encryption requires a \"Key\" of 32 bytes.");
            }
            if (IV == null)
            {
                throw new ArgumentException("Encryption failed because the \"IV\" provided was missing.");
            }
            if (IV.Length != 16)
            {
                throw new ArgumentException("Encryption requires an \"IV\" of 16 bytes.");
            }

            return true;
        }



    }
}
