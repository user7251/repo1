using System;
using System.Collections.Generic;
namespace GitHub.User7251
{
    public class StringTools
    {
        /// <summary>
        /// Splits s into substrings and returns them in an IList.  Tries to do a split after a delimiter and maximize substring length.
        /// </summary>
        public static IList<string> Split(string s, char delimiter, int minSubstringLen, int maxSubstringLen)
        {
            if (maxSubstringLen < minSubstringLen || minSubstringLen < 1) throw new ArgumentException();
            var r = new List<string>();
            if (s == null || s.Length == 0) return r;
            if (s.Length <= maxSubstringLen)
            {
                r.Add(s);
                return r;
            }
            int i1 = 0; // index of first char of substring
            while (true)
            {
                int charsRemaining = s.Length - i1;
                if (charsRemaining <= maxSubstringLen)
                {
                    r.Add(s.Substring(i1));
                    return r;
                }
                int i2Max = i1 + maxSubstringLen - 1;
                int i2 = s.LastIndexOf(delimiter, startIndex: i2Max); // index of last char of substring
                if (i2 < i1 + minSubstringLen - 1) i2 = i2Max;
                int len = i2 - i1 + 1;
                r.Add(s.Substring(i1, len));
                i1 = i2 + 1;
            }
        }

        public static string RemoveIllegalFileNameChars(string s)
        {
            if (s != null) foreach (char c in ILLEGAL_FILE_NAME_CHARS) s = s.Replace(c, ' ');
            return s;
        }

        private const string ILLEGAL_FILE_NAME_CHARS = @"/\:*?""<>|";

        /// from http://stackoverflow.com/questions/472906/converting-a-string-to-byte-array
        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// from http://stackoverflow.com/questions/472906/converting-a-string-to-byte-array
        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}