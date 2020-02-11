using Newtonsoft.Json;
using RESTService.Managers;
using RESTService.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace RESTService.Controllers
{
    [MyAuthorize]
    public class ProductsController : ApiController
    {
        // GET
        /// <summary>
        /// List all products in the database
        /// </summary>
        /// <returns>List of products data in database</returns>
        public IEnumerable<Products> Get(string CountryContext="PLN")
        {
            using (IUMdbEntities entities = new IUMdbEntities())
            {
                List<Products> requestedProducts = entities.Products.ToList();
                Countries requestedCountry = entities.Countries.FirstOrDefault(country => country.CountryTag == CountryContext);

                foreach(Products product in requestedProducts)
                {
                    Prices priceForProduct = entities.Prices.FirstOrDefault(price =>
                        price.ProductId == product.Id && price.CountryId == requestedCountry.Id);
                    if (priceForProduct == null)
                        product.Price = decimal.MinusOne;
                    else
                        product.Price = priceForProduct.Price;
                }

                return requestedProducts;
            }
        }

        // GET id
        /// <summary>
        /// Return data of specific product.
        /// </summary>
        /// <param name="id">Id of the product</param>
        /// <returns>Specific product data</returns>
        public Products Get(int id, string CountryContext = "PLN")
        {
            using (IUMdbEntities entities = new IUMdbEntities())
            {
                Products searchedProduct = entities.Products.Find(id);

                if (searchedProduct == null)
                    return null;

                Countries requestedCountry = entities.Countries.FirstOrDefault(country => country.CountryTag == CountryContext);
                Prices priceForProduct = entities.Prices.FirstOrDefault(price =>
                    price.ProductId == searchedProduct.Id && price.CountryId == requestedCountry.Id);
                if (priceForProduct == null)
                    searchedProduct.Price = decimal.MinusOne;
                else
                    searchedProduct.Price = priceForProduct.Price;

                return searchedProduct;
            }
        }

        // POST
        /// <summary>
        /// Add new product. New products should be added with Quantity=0
        /// </summary>
        /// <param name="product">Product to add to database</param>
        public IHttpActionResult Post([FromBody]Products product, string CountryContext = "PLN")
        {
            System.Web.Http.Results.StatusCodeResult status;
            product.Quantity = 0;
            using (IUMdbEntities entities = new IUMdbEntities())
            {
                bool checkIfProductExist = entities.Products.Any(e => 
                    e.ManufacturerName == product.ManufacturerName &&
                    e.ModelName == product.ModelName);

                /*
                bool checkIfCountryContextIsSupported = entities.Countries.Any(e =>
                    e.CountryTag == CountryContext);
                */

                if (checkIfProductExist)
                {
                    status = new System.Web.Http.Results.StatusCodeResult(HttpStatusCode.Conflict, this);
                    return status;
                }

                entities.Products.Add(product);
                entities.SaveChanges();

                Countries requestedCountry = entities.Countries.FirstOrDefault(country => country.CountryTag == CountryContext);
                Prices price = new Prices()
                {
                    Price = product.Price.Value,
                    CountryId = requestedCountry.Id,
                    ProductId = product.Id
                };

                PricesController pc = new PricesController();
                pc.Post(price);
                
                status = new System.Web.Http.Results.StatusCodeResult(HttpStatusCode.Created, this);
                return status;
            }
        }

        // PUT
        /// <summary>
        /// Edit all availble product fields (exluding Quantiy)
        /// </summary>
        /// <param name="product">Updated product</param>
        public IHttpActionResult Put([FromBody]Products product, string CountryContext = "PLN")
        {
            System.Web.Http.Results.StatusCodeResult status;
            using (IUMdbEntities entities = new IUMdbEntities())
            {
                Products changedProduct = entities.Products.Find(product.Id);

                if (changedProduct == null)
                {
                    status = new System.Web.Http.Results.StatusCodeResult(HttpStatusCode.NotFound, this);
                    return status;
                }

                Countries requestedCountry = entities.Countries.FirstOrDefault(country => country.CountryTag == CountryContext);
                Prices changedPrice = entities.Prices.FirstOrDefault(price => price.ProductId == changedProduct.Id && price.CountryId == requestedCountry.Id);
                if (changedPrice == null)
                {
                    changedPrice = new Prices()
                    {
                        ProductId = changedProduct.Id,
                        CountryId = requestedCountry.Id
                    };
                }
                changedPrice.Price = product.Price.Value;
                entities.Prices.AddOrUpdate(changedPrice);
                entities.SaveChanges();

                product.Price = null;
                product.Quantity = changedProduct.Quantity;
                if (product.OriginCountry == null)
                    product.OriginCountry = changedProduct.OriginCountry;

                entities.Products.AddOrUpdate(product);
                entities.SaveChanges();

                status = new System.Web.Http.Results.StatusCodeResult(HttpStatusCode.OK, this);
                return status;
                
            }
        }

        // PUT id?quantityChange
        /// <summary>
        /// Change quantity of given product
        /// </summary>
        /// <param name="id">Id of the product</param>
        /// <param name="quantityChange">Change in quantity</param>
        public IHttpActionResult Put(int id, int quantityChange)
        {
            System.Web.Http.Results.StatusCodeResult status;
            using (IUMdbEntities entities = new IUMdbEntities())
            {
                Products changedProduct = entities.Products.Find(id);

                if(changedProduct == null)
                {
                    status = new System.Web.Http.Results.StatusCodeResult(HttpStatusCode.NotFound, this);
                    return status;
                }

                if ((changedProduct.Quantity + quantityChange) < 0)
                {
                    status = new System.Web.Http.Results.StatusCodeResult(HttpStatusCode.Conflict, this);
                    return status;
                }

                if (changedProduct != null)
                {
                    changedProduct.Quantity += quantityChange;
                    entities.Products.AddOrUpdate(changedProduct);
                    entities.SaveChanges();

                    status = new System.Web.Http.Results.StatusCodeResult(HttpStatusCode.OK, this);
                    return status;
                }
                else
                {
                    status = new System.Web.Http.Results.StatusCodeResult(HttpStatusCode.NotFound, this);
                    return status;
                }
            }
        }

        // DELETE id
        /// <summary>
        /// Removing a product
        /// </summary>
        /// <param name="id">Id of the product</param>
        public IHttpActionResult Delete(int id)
        {
            System.Web.Http.Results.StatusCodeResult status;
            using (IUMdbEntities entities = new IUMdbEntities())
            {
                Products deletedProduct = entities.Products.Find(id);

                if (deletedProduct == null)
                {
                    status = new System.Web.Http.Results.StatusCodeResult(HttpStatusCode.NotFound, this);
                    return status;
                }

                List<Prices> pricesOfDeletedProduct = entities.Prices.Where(price => price.ProductId == deletedProduct.Id).ToList();
                foreach(Prices price in pricesOfDeletedProduct)
                {
                    entities.Prices.Remove(price);
                }
                entities.SaveChanges();

                entities.Products.Remove(deletedProduct);
                entities.SaveChanges();

                status = new System.Web.Http.Results.StatusCodeResult(HttpStatusCode.OK, this);
                return status;
            }
        }

        [HttpPost]
        [Route("~/api/Products/SynchronizeOffline")]
        public IEnumerable<OfflineResponseModel> SynchronizeOffline([FromBody]List<OfflineRequestModel> offlineChanges, string CountryContext = "PLN")
        {
            List<HttpResponseMessage> results = new List<HttpResponseMessage>();
            TryToSynchronize(offlineChanges, results, CountryContext);

            List<OfflineRequestModel> repeatOfflineChanges = new List<OfflineRequestModel>();
            List<int> repeatIndexes = new List<int>();

            for(int i=0; i<results.Count(); i++)
            {
                if(!results[i].IsSuccessStatusCode)
                {
                    repeatOfflineChanges.Add(offlineChanges[i]);
                    repeatIndexes.Add(i);
                }
            }

            List<HttpResponseMessage> repeatResults = new List<HttpResponseMessage>();
            TryToSynchronize(repeatOfflineChanges, repeatResults, CountryContext);

            for(int i=0; i< repeatResults.Count(); i++)
            {
                results[repeatIndexes[i]] = repeatResults[i];
            }
            
            List<OfflineResponseModel> returnedResults = new List<OfflineResponseModel>();

            for(int i=0; i< results.Count(); i++)
            {
                var result = results[i];
                var offlineChange = offlineChanges[i];

                OfflineResponseModel response = new OfflineResponseModel()
                {
                    IsSuccessStatusCode = result.IsSuccessStatusCode,
                    ReasonPhrase = result.ReasonPhrase,
                    StatusCode = result.StatusCode,
                    changeType = offlineChange.changeType,
                    commentary = offlineChange.commentary
                };
                returnedResults.Add(response);
            }
            
            return returnedResults;
        }

        private void TryToSynchronize(List<OfflineRequestModel> offlineChanges, List<HttpResponseMessage> results, string CountryContext)
        {
            foreach (OfflineRequestModel offlineChange in offlineChanges)
            {
                Products syncProduct = offlineChange.data;

                if (offlineChange.changeType == OfflineChangeType.Create)
                {
                    int oldId = syncProduct.Id;

                    HttpResponseMessage status = Post(syncProduct, CountryContext)
                        .ExecuteAsync(System.Threading.CancellationToken.None).GetAwaiter().GetResult();

                    if (status.IsSuccessStatusCode)
                    {
                        int newId;
                        using (IUMdbEntities entities = new IUMdbEntities())
                        {
                            newId = entities.Products.FirstOrDefault(e => e.ManufacturerName == syncProduct.ManufacturerName &&
                                e.ModelName == syncProduct.ModelName).Id;
                        }

                        foreach (OfflineRequestModel offChn in offlineChanges)
                        {
                            Products currentProduct = offChn.data;
                            if (currentProduct.Id == oldId)
                                currentProduct.Id = newId;
                        }
                    }

                    results.Add(status);
                }
                else if (offlineChange.changeType == OfflineChangeType.Update)
                {
                    HttpResponseMessage status = Put(syncProduct, CountryContext)
                        .ExecuteAsync(System.Threading.CancellationToken.None).GetAwaiter().GetResult();

                    results.Add(status);
                }
                else if (offlineChange.changeType == OfflineChangeType.ChangeQuantity)
                {
                    int quantityChange = Int32.Parse(offlineChange.requestURL.Split('=')[1]);
                    HttpResponseMessage status = Put(syncProduct.Id, quantityChange)
                        .ExecuteAsync(System.Threading.CancellationToken.None).GetAwaiter().GetResult();

                    results.Add(status);
                }
                else if (offlineChange.changeType == OfflineChangeType.Delete)
                {
                    string token = Request.Headers.Authorization.Parameter;

                    JWTService service = new JWTService(DefaultSecretKey.key);
                    if (!service.IsTokenValid(token))
                    {
                        results.Add(new HttpResponseMessage(HttpStatusCode.Forbidden));
                        continue;
                    }

                    string tokenType; //username, password,
                    List<string> userRoles;
                    List<Claim> tokenClaims = service.GetTokenClaims(token).ToList();
                    userRoles = tokenClaims.FirstOrDefault(e => e.Type.Equals(MyClaimsTypes.Roles)).Value.Split(',').ToList();

                    if (!userRoles.Any(e => e == "Manager"))
                    {
                        results.Add(new HttpResponseMessage(HttpStatusCode.Forbidden));
                        continue;
                    }

                    HttpResponseMessage status = Delete(syncProduct.Id)
                        .ExecuteAsync(System.Threading.CancellationToken.None).GetAwaiter().GetResult();

                    results.Add(status);
                }
            }
        }
    }
}
