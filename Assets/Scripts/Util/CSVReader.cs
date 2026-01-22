using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
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
            t_data.atk = float.Parse(t_cols[2]);
            t_data.ats = float.Parse(t_cols[3]);
            t_data.reload = float.Parse(t_cols[4], CultureInfo.InvariantCulture);
            t_data.mag = float.Parse(t_cols[5]);
            t_data.crit = float.Parse(t_cols[6], CultureInfo.InvariantCulture);
            t_data.critMag = float.Parse(t_cols[7], CultureInfo.InvariantCulture);

            t_dic.Add(t_data.weaponCode, t_data);
        }

        return t_dic;
    }

    public static Dictionary<PerkType, Dictionary<int, PerkInfo>> ReadPerkCSV(TextAsset _csv)
    {
        Dictionary<PerkType, Dictionary<int, PerkInfo>> t_dic = new Dictionary<PerkType, Dictionary<int, PerkInfo>>();

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
            string[] t_cols = t_line.Split(',');
            if (string.IsNullOrEmpty(t_cols[0]))
                continue;



            PerkInfo t_info = new PerkInfo();

            t_info.perkCode = int.Parse(t_cols[0]);
            t_info.perkName = t_cols[1];
            t_info.perkDescription = t_cols[2];
            t_info.perkStat = ParseStatBlocks(t_cols[3]);
            t_info.perkType = Enum.Parse<PerkType>(t_cols[4]);

            if (!t_dic.ContainsKey(t_info.perkType))
                t_dic.Add(t_info.perkType, new Dictionary<int, PerkInfo>());
            t_dic[t_info.perkType].Add(t_info.perkCode, t_info);
        }

        return t_dic;
    }


    static StatBlock[] ParseStatBlocks(string _effect)
    {
        if (string.IsNullOrEmpty(_effect))
            return Array.Empty<StatBlock>();

        string[] t_tokens = _effect.Split('/');
        List<StatBlock> t_list = new List<StatBlock>();

        foreach (string t in t_tokens)
        {
            string t_str = t.Trim();

            // + / - 판단
            bool t_isMinus = t_str.Contains("-");
            StatSign t_sign = t_str.Contains("%")
                ? StatSign.Percentage
                : StatSign.Constant;

            // StatType 문자열 추출 (숫자/기호 제거 전)
            string t_statName = ExtractStatName(t_str);

            StatType t_type = Enum.Parse<StatType>(t_statName);

            // 숫자만 추출
            string t_valueStr = t_str
                .Replace(t_statName, "")
                .Replace("+", "")
                .Replace("-", "")
                .Replace("%", "")
                .Replace("c", ""); // Mag+1c
            float t_value = 1f;
            if (!string.IsNullOrEmpty(t_valueStr))
                t_value = float.Parse(t_valueStr, CultureInfo.InvariantCulture);

            if (t_isMinus)
                t_value *= -1f;

            t_list.Add(new StatBlock(t_type, t_value, t_sign));
        }

        return t_list.ToArray();
    }

    static string ExtractStatName(string _str)
    {
        // 숫자/기호 나오기 전까지가 StatType 이름
        int t_idx = 0;
        while (t_idx < _str.Length && char.IsLetter(_str[t_idx]))
            t_idx++;

        return _str.Substring(0, t_idx);
    }

}
