using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FabrieBank.BLL.Service;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.DAL.Entity;
using FabrieBank.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FabrieBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        public CustomerController()
        {
        }

        public DTOCustomer demoCustomer = new DTOCustomer()
        {
            //DovizCins = "TRY"
        };

        [HttpGet("list")]
        public IEnumerable<DTOCustomer> ReadListCustomer()
        {
            SCustomer sCustomer  = new SCustomer();
            return sCustomer.ReadListCustomer(demoCustomer);
        }

        [HttpGet]
        public DTOCustomer ReadCustomer()
        {
            SCustomer sCustomer = new SCustomer();
            return sCustomer.ReadCustomer(demoCustomer);
        }

        [HttpPost]
        public bool InsertCustomer()
        {
            SCustomer sCustomer = new SCustomer();
            return sCustomer.InsertCustomer(demoCustomer);
        }

        [HttpPut]
        public bool UpdateCustomer()
        {
            SCustomer sCustomer = new SCustomer();
            return sCustomer.UpdateCustomer(demoCustomer);
        }

        [HttpDelete]
        public bool DeleteCustomer()
        {
            SCustomer sCustomer = new SCustomer();
            return sCustomer.DeleteCustomer(demoCustomer);
        }
    }
}