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

        [HttpGet]
		public IEnumerable<DTOCurrency> ReadListCurrency()
		{
			SCurrency sCurrency = new SCurrency();
			return sCurrency.ReadListCurrency(demoCurrency);
		}
	}
}

