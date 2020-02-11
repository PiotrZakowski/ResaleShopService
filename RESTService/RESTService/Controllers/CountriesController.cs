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
    public class CountriesController : ApiController
    {
        // GET
        /// <summary>
        /// List all supported countries currencies
        /// </summary>
        /// <returns>List all supported countries currencies</returns>
        public IEnumerable<Countries> Get()
        {
            using (IUMdbEntities entities = new IUMdbEntities())
            {
                List<Countries> supportedCountries = entities.Countries.ToList();
                return supportedCountries;
            }
        }
    }
}
