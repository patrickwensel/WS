using System;

namespace WS.Mobile.WorkOrders.Backend
{
    public static class ByteArrayExtensions
    {
        public static byte[] ToMD5Hash(this byte[] array)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            var hash = md5.ComputeHash(array);
            
            return hash;
        }

        public static string ToBase64(this byte[] array)
        {
            return Convert.ToBase64String(array);
        }
    }
}

