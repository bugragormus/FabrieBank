using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public DTOAccountInfo demoAccountInfo = new DTOAccountInfo()
        {
            //DovizCins = "TRY"
        };

        [HttpGet]
        public IEnumerable<DTOAccountInfo> ReadListAccountInfo()
        {
            SAccountInfo sAccountInfo = new SAccountInfo();
            return sAccountInfo.ReadListAccountInfo(demoAccountInfo);
        }
    }
}

