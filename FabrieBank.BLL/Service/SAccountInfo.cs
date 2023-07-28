using System;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;

namespace FabrieBank.BLL.Service
{
	public class SAccountInfo
	{
		public SAccountInfo()
		{
		}

        public List<DTOAccountInfo> ReadListAccountInfo(DTOAccountInfo accountInfo)
        {
            EAccountInfo eAccountInfo = new EAccountInfo();
            return eAccountInfo.ReadListAccountInfo(accountInfo);
        }

        public DTOAccountInfo ReadAccountInfo(DTOAccountInfo accountInfo)
        {
            EAccountInfo eAccountInfo = new EAccountInfo();
            return eAccountInfo.ReadAccountInfo(accountInfo);
        }

        public bool InsertAccountInfo(DTOAccountInfo accountInfo)
        {
            EAccountInfo eAccountInfo = new EAccountInfo();
            return eAccountInfo.InsertAccountInfo(accountInfo);
        }

        public bool UpdateAccountInfo(DTOAccountInfo accountInfo)
        {
            EAccountInfo eAccountInfo = new EAccountInfo();
            return eAccountInfo.UpdateAccountInfo(accountInfo);
        }

        public bool DeleteAccountInfo(DTOAccountInfo accountInfo)
        {
            EAccountInfo eAccountInfo = new EAccountInfo();
            return eAccountInfo.DeleteAccountInfo(accountInfo);
        }
    }
}

