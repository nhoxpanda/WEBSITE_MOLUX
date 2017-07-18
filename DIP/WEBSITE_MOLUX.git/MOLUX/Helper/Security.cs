using System;
using System.Security.Cryptography;
using System.Text;

public static class Security
{
    /// <summary>
    /// Mã Hóa MD5
    /// </summary>
    public static string Encrypt(string key, string toEncrypt)
    {
        try
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            TripleDESCryptoServiceProvider tdes =
            new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        catch (Exception)
        {
            return "Lỗi";
        }
    }
    public static string Encrypt(string toEncrypt)
    {
        try
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes("rxZwQi8PwO9vGKVxGFTzuehApb0MpO92N/HOAMe0Ib7VkS6++gDtrFiotHWPzUjUklKa2hJjmG+6Sh"));
            TripleDESCryptoServiceProvider tdes =
            new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        catch (Exception)
        {
            return "Lỗi";
        }
    }
    /// <summary>
    /// Giải mã MD5
    /// </summary>

    public static string Decrypt(string key, string toDecrypt)
    {
        try
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
            toEncryptArray, 0, toEncryptArray.Length);
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        catch (Exception)
        {
            return "sai key";
        }
    }
    public static string Decrypt(string toDecrypt)
    {
        try
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes("rxZwQi8PwO9vGKVxGFTzuehApb0MpO92N/HOAMe0Ib7VkS6++gDtrFiotHWPzUjUklKa2hJjmG+6Sh"));

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
            toEncryptArray, 0, toEncryptArray.Length);
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        catch (Exception)
        {
            return "sai key";
        }
    }
    /// <summary>
    /// Không phải là mã hóa chỉ thay đổi giá trị ngẫu nhiên tránh việc đánh cắp dữ liệu thông qua ID
    /// sau khi public Không được thay đổi bất kỳ giá trị nào trong mãng key và lv => lỗi 404
    /// </summary>
    public static string DecryptID(string str)
    {   
        string[] key = { "M", "N", "B", "V", "C", "x", "Z", "L", "K", "J", "H", "G", "F", "D", "S", "A", "p", "O", "I", "U", "Y", "T", "R", "e", "W", "Q", "m", "n", "b", "v", "c", "X", "z", "l", "k", "j", "h", "g", "f", "d", "s", "a", "P", "o", "i", "u", "y", "t", "r", "E", "w", "q" };
        string[] vl = { "965", "825", "689", "656", "623", "545", "533", "523", "511", "400", "379", "368", "357", "346", "335", "324", "313", "296", "286", "276", "266", "256", "249", "239", "229", "219", "189", "179", "169", "159", "148", "139", "128", "00", "41", "70", "99", "88", "77", "66", "55", "44", "33", "22", "11", "0", "8", "7", "5", "4", "2", "1" };

        //ok
        ////string[] key = { "a", "B", "c", "D", "e", "i", "g", "h", "k", "L", "m", "N", "o", "p", "Q", "x", "y", "z", "s", "u", "t", "w", "C" };
        ////string[] vl = { "32", "11", "22", "33", "44", "55", "66", "77", "88", "99", "00", "40", "10", "21", "35", "56", "70", "2", "3", "5", "6", "8" };
        for (int i = 0; i < vl.Length; i++)
        {
            str = str.Replace(key[i], vl[i]);
        }
        return str;
    }
    public static string EncryptID(string str)
    {
        string[] key = { "M", "N", "B", "V", "C", "x", "Z", "L", "K", "J", "H", "G", "F", "D", "S", "A", "p", "O", "I", "U", "Y", "T", "R", "e", "W", "Q", "m", "n", "b", "v", "c", "X", "z", "l", "k", "j", "h", "g", "f", "d", "s", "a", "P", "o", "i", "u", "y", "t", "r", "E", "w", "q" };
        string[] vl = { "965", "825", "689", "656", "623", "545", "533", "523", "511", "400", "379", "368", "357", "346", "335", "324", "313", "296", "286", "276", "266", "256", "249", "239", "229", "219", "189", "179", "169", "159", "148", "139", "128", "00", "41", "70", "99", "88", "77", "66", "55", "44", "33", "22", "11", "0", "8", "7", "5", "4", "2", "1" };
        //ok
        ////string[] key = { "a", "B", "c", "D", "e", "i", "g", "h", "k", "L", "m", "N", "o", "p", "Q", "x", "y", "z", "s", "u", "t", "w", "C" };
        ////string[] vl = { "32", "11", "22", "33", "44", "55", "66", "77", "88", "99", "00", "40", "10", "21", "35", "56", "70", "2", "3", "5", "6", "8" };
        for (int i = 0; i < vl.Length; i++)
        {
            str = str.Replace(vl[i], key[i]);
        }
        return str;
    }
}