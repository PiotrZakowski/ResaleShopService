using RESTService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RESTService.Controllers
{
    [MyAuthorize]
    public class PricesController : ApiController
    {
        // POST
        /// <summary>
        /// Add new price for a product.
        /// </summary>
        /// <param name="product">Price to add to database</param>
        public IHttpActionResult Post([FromBody]Prices price)
        {
            System.Web.Http.Results.StatusCodeResult status;

            using (IUMdbEntities entities = new IUMdbEntities())
            {
                bool checkIfPriceExist = entities.Prices.Any(e =>
                    e.ProductId == price.ProductId &&
                    e.CountryId == price.Id);

                if (checkIfPriceExist)
                {
                    status = new System.Web.Http.Results.StatusCodeResult(HttpStatusCode.Conflict, this);
                    return status;
                }

                entities.Prices.Add(price);
                entities.SaveChanges();

                status = new System.Web.Http.Results.StatusCodeResult(HttpStatusCode.Created, this);
                return status;
            }
        }
    }
}
