using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

public class INI_file
{
    public string[] raw;
    int[] _headers;
    public List<Header> headers => _headers.Select(x => new Header(raw[x], GetPairs(x), x)).ToList();
    public INI_file(string[] data)
    {
        raw = data;
        LoadHeaders();
    }
    void LoadHeaders()
    {
        var list = new List<int>();
        for (int k = 0; k < raw.Length; k++)
        {
            var line = raw[k];
            if (string.IsNullOrEmpty(line)) continue;
            if (Regex.IsMatch(line, Header.RegexPattern))
            {
                list.Add(k);
            }
        }
        _headers = list.ToArray();
    }
    Pair[] GetPairs(int header)
    {
        var pairs = new List<Pair>();
        for (int k = header + 1; k < raw.Length; k++)
        {
            var line = raw[k];
            if (string.IsNullOrEmpty(line)) continue;
            if (Regex.IsMatch(line, Header.RegexPattern))
                break;
            if (Regex.IsMatch(line, Pair.RegexPattern))
                pairs.Add(new Pair(line, k));
        }
        return pairs.ToArray();
    }

    public struct Header
    {
        public const string RegexPattern = "^\\[[^\\[\\]]+\\]";
        public string name => raw.Substring(1, raw.IndexOf(']') - 1);
        public string raw;
        public Pair[] pairs;
        public int index;
        public Header(string raw, Pair[] pairs, int index)
        {
            this.raw = raw;
            this.pairs = pairs;
            this.index = index;
        }
    }

    public struct Pair
    {
        public const string RegexPattern = "^;{0,1}\\s*[^=\\[\\];\\r\\n\\s]+\\s*=\\s*([^=\\[\\];]+|(\".* \")|[\\r\\n]*)$";
        public string key => raw.Substring(0, raw.IndexOf('='));
        public string value => raw.Substring(raw.IndexOf('=') + 1);
        public string raw;
        public int index;
        public bool isCommented => raw.StartsWith(";");
        public Pair(string raw, int index)
        {
            this.raw = raw;
            this.index = index;
        }
    }
}