using FabrieBank.BLL.Service;
using FabrieBank.DAL.Common.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FabrieBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountInfoController : Controller
    {
        public AccountInfoController()
        {
        }

        public DTOAccountInfo demoAccountInfo = new DTOAccountInfo();

        [HttpGet("list")]
        public IEnumerable<DTOAccountInfo> ReadListAccountInfo()
        {
            SAccountInfo sAccountInfo = new SAccountInfo();
            return sAccountInfo.ReadListAccountInfo(demoAccountInfo);
        }

        [HttpGet]
        public DTOAccountInfo ReadAccountInfo(DTOAccountInfo accountInfo)
        {
            SAccountInfo sAccountInfo = new SAccountInfo();
            return sAccountInfo.ReadAccountInfo(accountInfo);
        }

        [HttpPost]
        public bool InsertAccountInfo()
        {
            SAccountInfo sAccountInfo = new SAccountInfo();
            return sAccountInfo.InsertAccountInfo(demoAccountInfo);
        }

        [HttpPut]
        public bool UpdateAccountInfo()
        {
            SAccountInfo sAccountInfo = new SAccountInfo();
            return sAccountInfo.UpdateAccountInfo(demoAccountInfo);
        }

        [HttpDelete]
        public bool DeleteAccountInfo()
        {
            SAccountInfo sAccountInfo = new SAccountInfo();
            return sAccountInfo.DeleteAccountInfo(demoAccountInfo);
        }
    }
}