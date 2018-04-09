using System;
namespace NbboService
{
    public class NBBO
    {
        public NBBO()
        {
        }

        private string security;
        private double nbboBid;
        private long nbboBidSize;
        private double nbboAsk;
        private long nbboAskSize;

        public string Security { get => security; set => security = value; }
        public double NbboBid { get => nbboBid; set => nbboBid = value; }
        public long NbboBidSize { get => nbboBidSize; set => nbboBidSize = value; }
        public double NbboAsk { get => nbboAsk; set => nbboAsk = value; }
        public long NbboAskSize { get => nbboAskSize; set => nbboAskSize = value; }
    }
}
