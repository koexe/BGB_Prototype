using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class CSVReader
{
    public static Dictionary<int, WeaponInfo> ReadWeaponCSV(TextAsset _csv)
    {
        Dictionary<int, WeaponInfo> t_dic = new Dictionary<int, WeaponInfo>();

        if (_csv == null)
        {
            Debug.LogError("CSVReader: TextAsset is null");
            return t_dic;
        }

        string[] t_lines = _csv.text.Split('\n');

        // 첫 줄은 헤더 → 1부터 시작
        for (int i = 1; i < t_lines.Length; i++)
        {
            string t_line = t_lines[i].Trim();

            if (string.IsNullOrEmpty(t_line))
                continue;

            string[] t_cols = t_line.Split(',');

            WeaponInfo t_data = new WeaponInfo();
            t_data.weaponCode = int.Parse(t_cols[0]);
            t_data.weaponName = t_cols[1];
            t_data.atk = int.Parse(t_cols[2]);
            t_data.ats = int.Parse(t_cols[3]);
            t_data.reload = float.Parse(t_cols[4], CultureInfo.InvariantCulture);
            t_data.mag = int.Parse(t_cols[5]);
            t_data.crit = float.Parse(t_cols[6], CultureInfo.InvariantCulture);
            t_data.critMag = float.Parse(t_cols[7], CultureInfo.InvariantCulture);

            t_dic.Add(t_data.weaponCode, t_data);
        }

        return t_dic;
    }
}
