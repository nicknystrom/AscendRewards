using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007
{
    public static class A1Translator
    {
        private static readonly Regex _re = new Regex("^(?<col>[A-Z]+)(?<row>[0-9]*)$", RegexOptions.Compiled);
        private static readonly Dictionary<string, uint> _cache = new Dictionary<string, uint>();
        private static int _maxLength = 0;

        private static IEnumerable<char> GetChars()
        {
            for (char l = 'A'; l <= 'Z'; ++l)
                yield return l;
        }

        private static IEnumerable<string> GetStrings(string b, int length)
        {
            foreach (char c in GetChars())
            {
                if (length == 1)
                    yield return b + c;
                else
                    foreach (string s in GetStrings(b + c, length - 1))
                        yield return s;
            }
        }

        private static string Parse(string r, string group)
        {
            Match m = _re.Match(r.ToUpper());

            if (m.Success)
                return m.Groups[group].Value;
            else
            {
                string err = string.Format("{0} is not a valid R1 reference", r);
#if DEBUG
                throw new ArgumentException(err);
#else
                Trace.WriteLine(err);
#endif
            }

#if !DEBUG
            return "";
#endif
        }

        public static uint GetRowIndex(string r)
        {
            string l = Parse(r, "row");

            return uint.Parse(l);
        }

        public static uint GetCellIndex(string r)
        {
            string l = Parse(r, "col");

            if (_maxLength < l.Length)
            {
                uint start = 0;

                if (_maxLength > 0)
                    start = _cache[new string('Z', _maxLength)];

                while (_maxLength < l.Length)
                {
                    int n = _maxLength + 1;

                    foreach (string s in GetStrings("", n))
                        _cache[s] = ++start;

                    _maxLength = n;
                }
            }

            return _cache[l];

        }
    }
}

