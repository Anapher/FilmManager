using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Film_Manager.Data
{
   class MovieHelper
    {
        public static string GetName(string name, bool UseFilter)
        {
            if (!UseFilter)
                return name;
            if (name.Contains("."))
            {
                name = name.Replace(".", " ");
            }

            List<string> Years = new List<string>();
            for (int i = 1970; i < DateTime.Now.Year + 1; i++)
            {
                Years.Add(i.ToString());
            }


            string[] sSplit = name.Split(' ');
            for (int i = 0; sSplit.Length < i; i++)
            {
                if (i == 0) //Das erste wird übersprungen, weil das auf jeden Fall zum Filmtitel gehört
                    continue;
                if (Years.Contains(sSplit[i])) //Wenn das aktuelle Item ein Jahr ist...
                {
                    return string.Join(" ", sSplit.Take(i)); //...nehmen wir alle Items bis zu dem aktuellen Index, fügen dazwischen ein Leerzeichen ein und geben das zurück
                }
            }
            return name;
        }
    }
}
