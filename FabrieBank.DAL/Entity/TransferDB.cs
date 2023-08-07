namespace FabrieBank.DAL.Entity
{
    public class TransferDB
    {
        private DataAccessLayer dataAccessLayer;

        public TransferDB()
        {
            dataAccessLayer = new DataAccessLayer();
        }

        //public bool EFT(long sourceAccountNo, long targetAccountNo, decimal amount)
        //{
        //    return dataAccessLayer.EFT(sourceAccountNo, targetAccountNo, amount);
        //}
    }
}