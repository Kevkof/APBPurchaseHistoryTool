namespace APBPurchaseHistoryTool
{
    internal class Total
    {

        public Total(int x, int y, double z)
        {
            year = x;
            credits = y;
            money = z;
        }

        public int year { get; set; }
        public int credits { get; set; }
        public double money { get; set; }
    }
}