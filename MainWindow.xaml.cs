using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace APBPurchaseHistoryTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string result;
        private string jmbResult;
        private DateTime convertDate = new DateTime(2018, 07, 09, 19, 00, 00);
        private List<PurchaseHistoryEntry> history;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GotFocus(object sender, RoutedEventArgs e)
        {

            TextBox tb = sender as TextBox;
            if (tb.Text == "Paste purchase history here")
            {
                tb.Text = "";
                ResultsBlock.Text = "Press Calculate to get the results";
                JmbBlock.Text = "Press Calculate to get the results";

            }
        }

        private void Generate(object sender, RoutedEventArgs e)
        {
            try
            {
                history = new List<PurchaseHistoryEntry>();
                result = "";
                jmbResult = "";
                int totalG1C = 0;
                double totalMoney = 0;
                string[] temp;
                if (HistoryBox.Text.Trim().Contains("\r\n"))
                {
                    temp = HistoryBox.Text.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.None);
                }
                else
                {
                    if (HistoryBox.Text.Trim().Contains("\r"))
                    {
                        temp = HistoryBox.Text.Trim().Split(new string[] { "\r" }, StringSplitOptions.None);
                    }
                    else
                    {
                        if (HistoryBox.Text.Trim().Contains("\n"))
                        {
                            temp = HistoryBox.Text.Trim().Split(new string[] { "\n" }, StringSplitOptions.None);
                        }
                        else
                        {
                            temp = null;
                        }

                    }

                }

                foreach (string line in temp)
                {
                    string[] temp2 = line.Split(new string[] { "\t" }, StringSplitOptions.None);
                    history.Add(new PurchaseHistoryEntry(temp2[0], temp2[1], temp2[2], temp2[3]));
                }
                foreach (PurchaseHistoryEntry entry in history)
                {
                    totalG1C += entry.Price;
                    totalMoney += G1CtoUSD(entry);
                }
                result += "You have spent " + totalG1C + " G1C\r\nWhich comes to a total of $ " + totalMoney.ToString("0.##") + "\r\n---Details---\r\n";
                List<PurchaseHistoryEntry> jmbs = new List<PurchaseHistoryEntry>();
                foreach (PurchaseHistoryEntry entry in history)
                {
                    if (entry.Item.StartsWith("Joker Mystery Box"))
                    {
                        jmbs.Add(entry);
                    }
                }

                int jmbtotal = 0;
                double jmbmoney = 0;
                List<Amount> amount = new List<Amount>();
                List<int> jmblist = new List<int>();
                foreach (PurchaseHistoryEntry entry in jmbs)
                {
                    jmbtotal += entry.Price;
                    jmbmoney += G1CtoUSD(entry);
                    string text = entry.Item.Remove(0, 17);
                    text = text.Trim();
                    string[] text2 = text.Split(new string[] { " " }, StringSplitOptions.None);
                    if (!Int32.TryParse(text2[0], out int i))
                    {
                        i = 0;
                    }
                    if (!jmblist.Contains(i))
                    {
                        jmblist.Add(i);
                    }
                }
                jmbResult += "You have spent " + jmbtotal + " G1C on Joker Boxes\r\nWhich comes to $ " + jmbmoney.ToString("0.##") + "\r\n---Details---\r\n";
                jmblist.Sort();
                foreach (int i in jmblist)
                {
                    amount.Add(new Amount(i, 0));
                }

                foreach (PurchaseHistoryEntry entry in jmbs)
                {
                    string text = entry.Item.Remove(0, 17);
                    text = text.Trim();
                    string[] text2 = text.Split(new string[] { " " }, StringSplitOptions.None);
                    foreach (Amount am in amount)
                    {
                        if (!Int32.TryParse(text2[0], out int j))
                        {
                            j = 0;
                        }
                        if (am.boxnumber == j)
                        {
                            if (!(text2.Length > 1))
                            {
                                am.added++;
                            }
                            else
                            {
                                int toAdd = 0;
                                text2 = text2.Where((item, index) => index != 0).ToArray();
                                foreach (string s in text2)
                                {
                                    string t = GetNumbers(s);
                                    if (!Int32.TryParse(t, out int i))
                                    {
                                        i = 0;
                                    }
                                    toAdd += i;
                                }
                                am.added += toAdd;
                            }
                        }
                    }
                }
                foreach (Amount a in amount)
                {
                    jmbResult += "Joker Box " + a.boxnumber + ": " + a.added + " boxes\r\n";
                }


                List<int> yearlist = new List<int>();
                foreach (PurchaseHistoryEntry entry in history)
                {
                    if (!yearlist.Contains(entry.Date.Year))
                    {
                        yearlist.Add(entry.Date.Year);
                    }
                }
                List<List<PurchaseHistoryEntry>> perYear = new List<List<PurchaseHistoryEntry>>();
                foreach (int year in yearlist)
                {
                    List<PurchaseHistoryEntry> list = new List<PurchaseHistoryEntry>();

                    foreach (PurchaseHistoryEntry entry in history)
                    {
                        if (entry.Date.Year == year)
                        {
                            list.Add(entry);
                        }
                    }
                    perYear.Add(list);
                }

                List<Total> spending = new List<Total>();
                foreach (List<PurchaseHistoryEntry> li in perYear)
                {
                    double moneyYear = 0.0;
                    int g1cYear = 0;
                    int year = 0;
                    foreach (PurchaseHistoryEntry entry in li)
                    {
                        g1cYear += entry.Price;
                        moneyYear += G1CtoUSD(entry);
                        if (year != entry.Date.Year)
                        {
                            year = entry.Date.Year;
                        }
                    }
                    Total total = new Total(year, g1cYear, moneyYear);
                    spending.Add(total);

                }
                spending.Reverse();
                foreach (Total t in spending)
                {
                    result += "In " + t.year + " you spent " + t.credits + " G1C ($ " + t.money.ToString("0.##") + ")\r\n";
                }


                TotalLabel.Text = "You spent " + totalG1C + " G1C, which is $ " + totalMoney.ToString("0.##");
                ResultsBlock.Text = result;
                JmbBlock.Text = jmbResult;

            }
            catch (Exception)
            {
                TotalLabel.Text = "Invalid data provided, please only use the purchase history";
            }
        }

        private double G1CtoUSD(PurchaseHistoryEntry e)
        {

            if (e.Date > convertDate)
            {
                return e.Price * 0.01;
            }
            else
            {
                return e.Price * 0.0125;
            }
        }

        private static string GetNumbers(string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }
    }
}
