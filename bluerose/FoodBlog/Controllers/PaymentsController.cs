using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;
using Stripe.Checkout;
public class StripeOptions
{
    public string option { get; set; }
}
namespace server.Controllers
{
    
    [Route("create-session")]
    [ApiController]
    
    public class CheckoutApiController : Controller
    {
        [HttpPost]
        public ActionResult Create()
        {
            var domain = "http://localhost:5001";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                  "card",
                },
                ShippingAddressCollection = new SessionShippingAddressCollectionOptions
                {
                  AllowedCountries  = new List<string>
                  {
                      "NO",
                      "US"
                  }
                },
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                      UnitAmount = 30000,
                      Currency = "nok",
                      ProductData = new SessionLineItemPriceDataProductDataOptions
                      {
                        Name = "Bluerose Cook Book",
                        
                      },
                    },
                    Quantity = 1,
                  },
                  
                },
               
                Mode = "payment",
                SuccessUrl = "https://localhost:5001/Home/success",
                CancelUrl = "https://localhost:5001/Home/cancel",
            };
            var service = new SessionService();
            Session session = service.Create(options);
            return Json(new { id = session.Id });
        }
    }
}