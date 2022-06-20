using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using QuoteGeneratorAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;


namespace QuoteGeneratorAPI.Controllers {

    public class QuoteAdminController : Controller {

        private IWebHostEnvironment environment;
        public QuoteAdminController(IWebHostEnvironment env) {
            environment = env;
        }
        public IActionResult Index() {
            // construct model
            MyModel myModel = new MyModel();
            // getting the quotes by calling the method when the page loads
            myModel.getQuotes();
            // setting feedback to be a TempData since using a redirect
            ViewData["feedback"] = TempData["feedback"];
            ViewData["deletefeedback"] = TempData["deletefeedback"];
            return View(myModel);
            // return View();
        }

        [HttpPost]
        public IActionResult AddQuote(MyModel myModel,  IFormFile selectedFile) {
            
            if (!ModelState.IsValid) return RedirectToAction("index");

            MyModel imageUploader = new MyModel(environment, "uploads");
            // if selectedfile is null catch it and dispaly the error instead else do the rest of the code
            if(selectedFile == null){
                ViewData["feedback"] = "Sorry no file was selected please try again:)";
            }else{
                // getting a filename withouth the extetions in order to add a number in between later
                string filename = Path.GetFileNameWithoutExtension(selectedFile.FileName);
                // creating a fullpath to use later
                string fullpath = environment.WebRootPath + "/" + "uploads" + "/";
                // string with filextension
                string FileExtension = Path.GetExtension(selectedFile.FileName);
                
                // checks if duplicate to add a number to it
                if(System.IO.File.Exists(fullpath + filename + FileExtension)) {
                    
                    // if the length is greater than 100 it goes in adn subtracts the length of the image to be less so than it is able to go into the other while loop and add number to the name to make it unique
                    while(filename.Length + myModel.number.ToString().Length + FileExtension.Length > 100) {
                        
                        // cutting down the lengh of the string if it reaches 100 only for the image that is 100 charcters is long but gets my number added to it when is duplicated another test case
                        filename = filename.Remove(filename.Length - 50);
                         
                    }
                    
                    while(System.IO.File.Exists(fullpath + filename + myModel.number + FileExtension)) {
                            // increamenting the number if more than one duplicate image is added
                            myModel.number++;
                    }
                    // putting everything in the end
                    
                    filename = filename + myModel.number + FileExtension;

                }else{
                    filename = filename + FileExtension;
                }


                int result = imageUploader.uploadImage(selectedFile, filename);
                Console.WriteLine("upload result: " + result);
                // myModel.filename = filename;


                // uses the values from the model to check what feedback to giveback to the user
                if (result == 5){
                    myModel.image = filename;
                    myModel.SaveQuote();
                    TempData["feedbacksuccess"] = "The upload was successful!";
                    myModel.getQuotes();
                    // return View("index", myModel);
                    return RedirectToAction("index");
                }else if (result == 4) {
                    ViewData["feedback"] = "Sorry error with saving occured";
                }else if (result == 3){
                    ViewData["feedback"] = "Sorry error with name length occured, file name is too long";
                }else if (result == 2){
                    ViewData["feedback"] = "Sorry error with size occured, file is too big";
                }else if (result == 1){
                    ViewData["feedback"] = "Sorry error with type occured, can not have any other type than jpeg/png/gif";
                }else if(result == 0){
                    ViewData["feedback"] = "Sorry no file was selected please try again:)";
                }
            }
    
            myModel.getQuotes();
            return View("index", myModel);

        }

        [HttpPost]
        public IActionResult Delete(MyModel myModel) {
            // lets the user know that the quote was deleted succesfully
            TempData["deletefeedback"] = "Quote was deleted succesfully!";
            myModel.deleteImage(environment, "uploads");
            myModel.DeleteQuote();
            myModel.getQuotes();
            // return View("index",myModel);
            return RedirectToAction("index");
        }

    }
}
