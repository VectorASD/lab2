using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringOfStudentProgress.Models {
    internal class NormalizeStr {
        public static string NormalizeTitleStr(string input) {
            int len = input.Length, index = 0;
            var src = input.ToCharArray();
            byte skip = 0;
            for (int i = 0; i < len; i++) {
                char ch = src[i];
                switch (ch) {
                case '\u0020': // Не спрашивайте, откуда я содрал все возможные пустые глифы ;'-}
                case '\u00A0':
                case '\u1680':
                case '\u2000':
                case '\u2001':
                case '\u2002':
                case '\u2003':
                case '\u2004':
                case '\u2005':
                case '\u2006':
                case '\u2007':
                case '\u2008':
                case '\u2009':
                case '\u200A':
                case '\u202F':
                case '\u205F':
                case '\u3000':
                case '\u2028':
                case '\u2029':
                case '\u0009':
                case '\u000A':
                case '\u000B':
                case '\u000C':
                case '\u000D':
                case '\u0085':
                    if (skip == 0) continue;
                    src[index++] = ' ';
                    skip = 0;
                    continue;
                default:
                    if (skip == 0) {
                        if (ch >= 'a' && ch <= 'z') ch -= ' ';
                        else if (ch >= 'а' && ch <= 'я') ch -= ' ';
                        skip = 1;
                    } else {
                        if (ch >= 'A' && ch <= 'Z') ch += ' ';
                        else if (ch >= 'А' && ch <= 'Я') ch += ' ';
                        skip = 2;
                    }
                    src[index++] = ch;
                    continue;
                }
            }
            if (index > 0 && skip == 0) index -= 1;
            return new string(src, 0, index);
        }
    }
}
