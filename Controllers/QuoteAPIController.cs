using System;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuoteGeneratorAPI.Models;

namespace quoteGeneratorAPI.Controllers {
    // attribute is required for Web APIs
    [ApiController]  
    // disabling CORs for requests / responses of public Web API - eliminates CORs errors if this web API is used with a client side web app
    [DisableCors]

    public class QuoteAPIController : ControllerBase {

        [HttpGet]
        // the URL routing - Web APIs must have one
        [Route("quotes/{count}")]
        public ActionResult<List<Quote>> GetNumberOfQuotes(string count) {
        
            MyModel myModel = new MyModel();
            // gets the count fromm the method where the duplicates were removed
            // count is turned into a number where it is used in the url to display that amount
            myModel.getData();
            myModel.RandomQuotes(Convert.ToInt32(count));
            List<Quote> JSONdata = myModel.randomquotes;
            // test this out by hitting https://localhost:5001/quotes
            return JSONdata;
        }        
        // set to get instead of HttpPost
        [HttpGet]
        // the URL routing - Web APIs must have one
        [Route("quotes/")]
        public ActionResult<List<Quote>> GetAllData() {
            MyModel myModel = new MyModel();
            myModel.getData();
            List<Quote> JSONdata = myModel.datalist;
            // test this out by hitting https://localhost:5001/quotes
            return JSONdata;
        }        
    }
    
}
