using System;

namespace APBPurchaseHistoryTool
{
    internal class PurchaseHistoryEntry
    {
        public PurchaseHistoryEntry(string v1, string v2, string v3, string v4)
        {
            Item = v1;
            if (v2.Contains("G1C"))
            {
                v2 = v2.Substring(0, v2.Length - 4);
                v2 = v2.Replace(",", "");
                if (!Int32.TryParse(v2, out int i))
                {
                    i = 0;
                };
                Price = i;
            }
            else
            {
                v2 = v2.Replace("$", "");
                double temp = float.Parse(v2);
                temp *= 0.8;
                Price = Convert.ToInt32(temp);
            }
            Date = DateTime.ParseExact(v3, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture); ;
            Payment = v4;
            gift = v1.EndsWith(" ");
        }
        public string Item { get; set; }
        public int Price { get; set; }
        public DateTime Date { get; set; }
        public string Payment { get; set; }
        public bool gift { get; set; }
    }
}