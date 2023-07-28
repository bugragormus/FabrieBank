using FabrieBank.BLL.Service;
using FabrieBank.DAL.Common.DTOs;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        [HttpGet]
        public IEnumerable<DTOAccountInfo> ReadListAccountInfo()
        {
            SAccountInfo sAccountInfo = new SAccountInfo();
            return sAccountInfo.ReadListAccountInfo(demoAccountInfo);
        }

        //[HttpGet]
        //public DTOAccountInfo ReadAccountInfo()
        //{
        //    SAccountInfo sAccountInfo = new SAccountInfo();
        //    return sAccountInfo.ReadAccountInfo(demoAccountInfo);
        //}

        //[HttpGet]
        //public bool InsertAccountInfo()
        //{
        //    SAccountInfo sAccountInfo = new SAccountInfo();
        //    return sAccountInfo.InsertAccountInfo(demoAccountInfo);
        //}

        //[HttpGet]
        //public bool UpdateAccountInfo()
        //{
        //    SAccountInfo sAccountInfo = new SAccountInfo();
        //    return sAccountInfo.UpdateAccountInfo(demoAccountInfo);
        //}

        //[HttpGet]
        //public bool DeleteAccountInfo()
        //{
        //    SAccountInfo sAccountInfo = new SAccountInfo();
        //    return sAccountInfo.DeleteAccountInfo(demoAccountInfo);
        //}
    }
}

