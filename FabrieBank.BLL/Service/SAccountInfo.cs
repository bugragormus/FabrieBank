using System;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.Entity;

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
    }
}

