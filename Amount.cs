namespace APBPurchaseHistoryTool
{
    internal class Amount
    {
        public Amount(int i, int v)
        {
            boxnumber = i;
            added = v;
        }
        public int boxnumber { get; set; }
        public int added { get; set; }
    }
}