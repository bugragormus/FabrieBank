using FabrieBank.BLL.Service;
using FabrieBank.DAL.Common.DTOs;
using FabrieBank.Services;
using Microsoft.AspNetCore.Mvc;

namespace FabrieBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : Controller
	{
		public CurrencyController()
		{
        }

		public DTOCurrency demoCurrency = new DTOCurrency()
		{
			//DovizCins = "TRY"
		};

        [HttpGet("list")]
        public IEnumerable<DTOCurrency> ReadListCurrency()
		{
			SCurrency sCurrency = new SCurrency();
			return sCurrency.ReadListCurrency(demoCurrency);
		}

        [HttpGet]
        public DTOCurrency ReadCurrency()
        {
            SCurrency sCurrency = new SCurrency();
            return sCurrency.ReadCurrency(demoCurrency);
        }

        [HttpPost]
        public bool InsertCurrency()
        {
            SCurrency sCurrency = new SCurrency();
            return sCurrency.InsertCurrency(demoCurrency);
        }

        [HttpPut]
        public bool UpdateCurrency()
        {
            SCurrency sCurrency = new SCurrency();
            return sCurrency.UpdateCurrency(demoCurrency);
        }

        [HttpDelete]
        public bool DeleteCurrency()
        {
            SCurrency sCurrency = new SCurrency();
            return sCurrency.DeleteCurrency(demoCurrency);
        }
    }
}

