using System;
namespace FabrieBank.DAL.Common.DTOs
{
    public class DTOTransfer
    {
        public int KaynakHesapIndex { get; set; }
        public int HedefHesapIndex { get; set; }
        public long KaynakHesapNo{ get; set; }
        public long HedefHesapNo { get; set; }
        public decimal Miktar { get; set; }
    }
}