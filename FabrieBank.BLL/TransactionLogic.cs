namespace FabrieBank.BLL;

public class TransactionLogic
{
    public bool Havale(long kaynakHesapNo, long hedefHesapNo, decimal miktar)
    {
        // Check if the source account exists
        if (!AccountExists(hedefHesapNo))
        {
            // Log the failed transfer
            LogFailedTransfer(kaynakHesapNo, hedefHesapNo, EnumTransactionType.WHETransfer, EnumTransactionStatus.Failed, miktar);
            Console.WriteLine("\nEFT");
            return false;
        }

        // Calculate the transaction fee for Havale
        decimal transactionFee = CalculateTransactionFee(EnumTransactionType.Havale);

        // Perform the transfer by calling a common method
        return PerformTransfer(kaynakHesapNo, hedefHesapNo, miktar, transactionFee);
    }

    public bool EFT(long kaynakHesapNo, long hedefHesapNo, decimal miktar)
    {
        // Check if the source account exists
        if (!AccountExists(hedefHesapNo))
        {
            // Log the failed transfer
            LogFailedTransfer(kaynakHesapNo, hedefHesapNo, EnumTransactionType.WHETransfer, EnumTransactionStatus.Failed, miktar);
            Console.WriteLine("\nEFT");
            return false;
        }

        // Calculate the transaction fee for EFT
        decimal transactionFee = CalculateTransactionFee(EnumTransactionType.EFT);

        // Perform the transfer by calling a common method
        return PerformTransfer(kaynakHesapNo, hedefHesapNo, miktar, transactionFee);
    }

    private decimal CalculateTransactionFee(EnumTransactionType transactionType)
    {
        // Use the TransactionFeeDB class or any other method to fetch the appropriate transaction fee from the database
        if (transactionType == EnumTransactionType.Havale)
        {
            // Calculate Havale transaction fee
            // ...
            return havaleTransactionFee;
        }
        else if (transactionType == EnumTransactionType.EFT)
        {
            // Calculate EFT transaction fee
            // ...
            return eftTransactionFee;
        }

        // Default value if the transaction type is not recognized
        return 0.00m;
    }

    private bool PerformTransfer(long kaynakHesapNo, long hedefHesapNo, decimal miktar, decimal transactionFee)
    {
        try
        {
            // ... (existing code)

            // Deduct the transaction fee from the transferred amount
            decimal totalAmount = miktar + transactionFee;

            // Check if the source account has sufficient balance
            if (kaynakBakiye < totalAmount)
            {
                // Log the failed transfer
                LogFailedTransfer(kaynakHesapNo, hedefHesapNo, EnumTransactionType.WHETransfer, EnumTransactionStatus.Failed, miktar);
                Console.WriteLine("\nYetersiz bakiye. Transfer gerçekleştirilemedi.");
                return false;
            }

            // Para transferi gerçekleştir
            // ... (existing code)

            return true;
        }
        catch (Exception ex)
        {
            // ... (existing code)
            return false;
        }
    }
}

