using CRM_TTV.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading;
using System.Security.Cryptography;
using System.Collections;

namespace CRM_TTV.Helper
{
    public static class Common
    {
        private static readonly char[] chars = new char[] { ',', '(', ')', ' ', '+', '=', '>', '<', '&' };

        #region Orthers
        private static readonly string[] VietNamChar = new string[]
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };
        public static string RemoveUnicode(string str)
        {
            //Thay thế và lọc dấu từng char      
            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                    str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
            }
            return str;
        }

        public static string CutdownDescription(string str, int numberWords)
        {
            if(str != null && str != "")
            {
                char[] characters = new char[] { ' ' };
                string[] splitStr = str.Split(characters);
                if (numberWords > splitStr.Length)
                {
                    return str;
                }
                else
                {
                    var tempDes = "";
                    for (var i = 0; i < numberWords; i++)
                    {
                        tempDes += splitStr[i] + " ";
                    }
                    return tempDes;
                }
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region Render View
        /// <summary>
        /// Render Razor View to String
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static String RenderRazorViewToString(ControllerContext controllerContext, String viewName, Object model)
        {
            controllerContext.Controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var ViewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var ViewContext = new ViewContext(controllerContext, ViewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, sw);
                ViewResult.View.Render(ViewContext, sw);
                ViewResult.ViewEngine.ReleaseView(controllerContext, ViewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        #endregion

        public static string ConvertFileSize(double filesize)
        {
            string[] sizes = { "Byte", "KB", "MB", "GB" };
            int order = 0;
            while (filesize >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                filesize = filesize / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return String.Format("{0:0.##} {1}", filesize, sizes[order]);
        }


        public static string MakeUrlSlug(string strInput)
        {
            try
            {
                for (int i = 1; i < VietNamChar.Length; i++)
                {
                    for (int j = 0; j < VietNamChar[i].Length; j++)
                    {
                        strInput = strInput.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
                    }
                }
                string KyTuDacBiet = "~!@#$%^&*+=|_«»\\/'\",.?/:;`<>(){}[]–";
                for (byte i = 0; i < KyTuDacBiet.Length; i++)
                {
                    strInput = strInput.Replace(KyTuDacBiet.Substring(i, 1), "");
                }
                strInput = strInput.Trim().Replace(" ", "-");
                //lặp cho đến khi khoog còn chuỗi -- thì thoát khỏi lặp
                for (int i = 0; i < 1000; i++)
                {
                    if (strInput.Contains("--"))
                        strInput = strInput.Replace("--", "-");
                    else
                        break;
                }
                return strInput.Replace("-–-","-").ToLower();
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        static public string MaHoa(string inputString)
        {
            CspParameters _cpsParameter;
            RSACryptoServiceProvider RSAProvider;
            _cpsParameter = new CspParameters();
            _cpsParameter.Flags = CspProviderFlags.UseMachineKeyStore;
            RSAProvider = new RSACryptoServiceProvider(1024, _cpsParameter);

            // TODO: Add Proper Exception Handlers
            CspParameters CSPParam = new CspParameters();
            CSPParam.Flags = CspProviderFlags.UseMachineKeyStore;

            RSACryptoServiceProvider rsaCryptoServiceProvider;
            //if (System.Web.HttpContext.Current == null) // WinForm
            rsaCryptoServiceProvider = new RSACryptoServiceProvider();
            //else // WebForm - Uses Machine store for keys
            //rsaCryptoServiceProvider = new RSACryptoServiceProvider(CSPParam);

            rsaCryptoServiceProvider.FromXmlString("<RSAKeyValue><Modulus>rxZwQi8PwO9vGKVxGFTzuehApb0MpO92N/HOAMe0Ib7VkS6++gDtrFiotHWPzUjUklKa2hJjmG+6Sh74c+iwJpU7dQGRxvoXYuF+m9r4lyGzXTrRP4Wt16SmbF8Pm6jaw9JPu1Xy+8sVBxYq8B5jyI5aaZ7aKvSBuJGLMtv/wcE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
            byte[] bytes = Encoding.UTF32.GetBytes(inputString);
            // The hash function in use by the .NET RSACryptoServiceProvider here is SHA1
            // int maxLength = ( keySize ) - 2 - ( 2 * SHA1.Create().ComputeHash( rawBytes ).Length );
            int dataLength = bytes.Length;
            int iterations = dataLength / 86;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i <= iterations; i++)
            {
                byte[] tempBytes = new byte[(dataLength - 86 * i > 86) ? 86 : dataLength - 86 * i];
                Buffer.BlockCopy(bytes, 86 * i, tempBytes, 0, tempBytes.Length);
                byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt(tempBytes, true);
                // Be aware the RSACryptoServiceProvider reverses the order of encrypted bytes after encryption and before decryption.
                // If you do not require compatibility with Microsoft Cryptographic API (CAPI) and/or other vendors.
                // Comment out the next line and the corresponding one in the DecryptString function.
                Array.Reverse(encryptedBytes);
                // Why convert to base 64?
                // Because it is the largest power-of-two base printable using only ASCII characters
                stringBuilder.Append(Convert.ToBase64String(encryptedBytes));
            }
            return stringBuilder.ToString();
        }

        static public string GiaiMa(this string inputString)
        {
            // TODO: Add Proper Exception Handlers
            CspParameters CSPParam = new CspParameters();
            CSPParam.Flags = CspProviderFlags.UseMachineKeyStore;

            RSACryptoServiceProvider rsaCryptoServiceProvider;
            //if (System.Web.HttpContext.Current == null) // WinForm
            rsaCryptoServiceProvider = new RSACryptoServiceProvider();
            //else // WebForm - Uses Machine store for keys
            //rsaCryptoServiceProvider = new RSACryptoServiceProvider(CSPParam);

            rsaCryptoServiceProvider.FromXmlString("<RSAKeyValue><Modulus>rxZwQi8PwO9vGKVxGFTzuehApb0MpO92N/HOAMe0Ib7VkS6++gDtrFiotHWPzUjUklKa2hJjmG+6Sh74c+iwJpU7dQGRxvoXYuF+m9r4lyGzXTrRP4Wt16SmbF8Pm6jaw9JPu1Xy+8sVBxYq8B5jyI5aaZ7aKvSBuJGLMtv/wcE=</Modulus><Exponent>AQAB</Exponent><P>5nR8EplxlG0uPVGorn8OkMXZ9TF7BPa5wZs1vL4JPsxZv8D+UjufUsGrHOQmZRxvFe4J/1/iZI/6m+nHOcFk1w==</P><Q>wn7R12szMYoIMFN8UEXcEmamO7PSELqhV+qe9a/7N6G1pKG1xU3AZpkfW0E/GJZGl7pA9UQNQZTxS/LSv0AjJw==</Q><DP>inrSl4aXBp6422X3W6vDv+D0AO+Twb7Ujm9K0jjLa232PFCnQhjLuznfLcQ3Aikc42ufnFIsw0r1R70p1x3MDw==</DP><DQ>lYaKLOLtaJiF0yFb4RrUJhFkm2GTjejtQXnO23N/3zUjQH5SEG3GDRqLUMzIhU6C1wMKDYVT66dmGs2D2CSm4Q==</DQ><InverseQ>eXW6RmvwuAoo52IAnv9dBq+ixrZqhDKyFRYusjuUpFggPw7A4OknUNwJtCHeQecOCmKNTo0T+AmGfq530XnDqg==</InverseQ><D>RTclocRhAfClhqTAlNHgl/nMtLiLqxhPL8aTnZNVDpIWc5J7RPHhA2T5LH3dH1ZPUpj9RoBGhxiEGJEtvwSZvb76txmEXaUlou0ZZveeJe7O+crWT70dn06Qz+Ua7F6uwpVCQr7VmTEY4qXFowvrdH8Haz/2uHM+FFpv/1idD9E=</D></RSAKeyValue>");
            int iterations = inputString.Length / 172;
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < iterations; i++)
            {
                byte[] encryptedBytes = Convert.FromBase64String(inputString.Substring(172 * i, 172));
                // Be aware the RSACryptoServiceProvider reverses the order of encrypted bytes after encryption and before decryption.
                // If you do not require compatibility with Microsoft Cryptographic API (CAPI) and/or other vendors.
                // Comment out the next line and the corresponding one in the EncryptString function.
                Array.Reverse(encryptedBytes);
                arrayList.AddRange(rsaCryptoServiceProvider.Decrypt(encryptedBytes, true));
            }
            return Encoding.UTF32.GetString(arrayList.ToArray(Type.GetType("System.Byte")) as byte[]);
        }

    }
}