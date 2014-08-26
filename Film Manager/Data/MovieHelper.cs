using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Film_Manager.Data
{
    class MovieHelper
    {
        public bool UseFilter { get; set; }
        public bool ReplaceUmlauts { get; set; }
        public bool ReplaceCodec { get; set; }
        public bool ReplaceYear { get; set; }
        public bool ReplaceQualitySetting { get; set; }
        public bool ReplaceGroup { get; set; }
        public bool ReplaceLanguage { get; set; }

        private Dictionary<string, string> umlauts;
        public Dictionary<string, string> Umlauts
        {
            get
            {
                if (umlauts == null)
                {
                    umlauts = new Dictionary<string, string>();
                    umlauts.Add("Ä", "ae");
                    umlauts.Add("Ö", "oe");
                    umlauts.Add("Ü", "ue");
                    umlauts.Add("ß", "ss");
                }
                return umlauts;
            }
        }

        private Dictionary<string, string> codec;
        public Dictionary<string, string> Codec
        {
            get
            {
                if (codec == null)
                {
                    codec = new Dictionary<string, string>();
                    codec.Add("DIVX", null);
                    codec.Add("XVID", null);
                    codec.Add("MPEG", null);
                    codec.Add("AC3", null);
                    codec.Add("DTS", null);
                    codec.Add("H.264", null);
                    codec.Add("X264", null);
                }
                return codec;
            }
        }

        private Dictionary<string, string> releaseyear;
        public Dictionary<string, string> ReleaseYear
        {
            get
            {
                if (releaseyear == null)
                {
                    releaseyear = new Dictionary<string, string>();
                    for (int i = 1970; i < DateTime.Now.Year + 1; i++)
                    {
                        releaseyear.Add(i.ToString(), null);
                    }
                }
                return releaseyear;
            }
        }

        private Dictionary<string, string> qualitysettings;
        public Dictionary<string, string> QualitySettings
        {
            get
            {
                if (qualitysettings == null)
                {
                    qualitysettings = new Dictionary<string, string>();
                    qualitysettings.Add("BLURAY", null);
                    qualitysettings.Add("BD", null);
                    qualitysettings.Add("TS", null);
                    qualitysettings.Add("DVDRIP", null);
                    qualitysettings.Add("DL", null);
                    qualitysettings.Add("MD", null);
                    qualitysettings.Add("RETAIL", null);
                    qualitysettings.Add("360P", null);
                    qualitysettings.Add("480P", null);
                    qualitysettings.Add("720P", null);
                    qualitysettings.Add("1080P", null);
                    qualitysettings.Add("MPEG-4", null);
                    qualitysettings.Add("HD", null);
                    qualitysettings.Add("AVC", null);
                    qualitysettings.Add("REMUX", null);
                    qualitysettings.Add("UNTOUCHED", null);
                }
                return qualitysettings;
            }
        }

        private Dictionary<string, string> releasegroup;
        public Dictionary<string, string> ReleaseGroup
        {
            get
            {
                if (releasegroup == null)
                {
                    releasegroup = new Dictionary<string, string>();
                    releasegroup.Add("-AOE", null);
                    releasegroup.Add("-CIS", null);
                    releasegroup.Add("-HDC", null);
                    releasegroup.Add("-ENCOUNTERS", null);
                    releasegroup.Add("-RHD", null);
                    releasegroup.Add("-LEETHD", null);
                    releasegroup.Add("-HADE", null);
                }
                return releasegroup;
            }
        }

        private Dictionary<string, string> languages;
        public Dictionary<string, string> Languages
        {
            get
            {
                if (languages == null)
                {
                    languages = new Dictionary<string, string>();
                    languages.Add("DEUTSCH", null);
                    languages.Add("GERMAN", null);
                    languages.Add("ENGLISCH", null);
                    languages.Add("ENGLISH", null);
                }
                return languages;
            }
        }

        public MovieHelper()
        {

        }

        public string GetName(string filename)
        {
            return filename;
            if (!filename.Contains(".") && !(filename.Contains(" ")))
            {
                return filename;
            }
            string result = filename.ToUpper().Replace(".", " ");
            if (ReplaceUmlauts)
            {
                ReplaceStringFromDirectory(ref result, Umlauts);
            }
            if (ReplaceCodec)
            {
                ReplaceStringFromDirectory(ref result, Codec);
            }
            if (ReplaceYear)
            {
                ReplaceStringFromDirectory(ref result, ReleaseYear);
            }
            if (ReplaceQualitySetting)
            {
                ReplaceStringFromDirectory(ref result, QualitySettings);
            }
            if (ReplaceGroup)
            {
                ReplaceStringFromDirectory(ref result, ReleaseGroup);
                if (result.Contains("-")) {
                    result = result.Split('-')[0];
                }
            }
            if (ReplaceLanguage)
            {
                ReplaceStringFromDirectory(ref result, Languages);
            }
            return result.Trim();
        }

        private void ReplaceStringFromDirectory(ref string s, Dictionary<string, string> dict)
        {
            foreach (KeyValuePair<string, string> str in dict)
            {
                s = s.Replace(str.Key, str.Value);
            }
        }
    }
}
