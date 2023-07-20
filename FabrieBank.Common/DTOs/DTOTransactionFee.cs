﻿using FabrieBank.Common.Enums;

namespace FabrieBank.Common.DTOs
{
    public class DTOTransactionFee
    {
        public EnumTransactionFeeType FeeType { get; set; }
        public decimal Amount { get; set; }
    }
}
