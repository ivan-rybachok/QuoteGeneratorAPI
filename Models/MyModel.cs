using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace QuoteGeneratorAPI.Models {

    public class MyModel {

        // class constants for different errors while uploading
        // public const int ERROR_NO_FILE = 0;
        public const int ERROR_TYPE = 1;
        public const int ERROR_SIZE = 2;
        public const int ERROR_NAME_LENGTH = 3;
        public const int ERROR_SAVE = 4;
        public const int SUCCESS = 5;

        // database connectivity variables
        private MySqlConnection dbConnection;
        private MySqlCommand dbCommand;
        private MySqlDataReader dbReader;

        private const int UPLOADLIMIT = 4194304;

        // needed for getting path to web app's location
        private string targetFolder;
        // path to the upload folder
        private string fullPath;
        // list for the dropdown
        private List<SelectListItem> _quotes;
        // lists for the WebAPI
        private List<Quote> _datalist;
        private List<Quote> _randomquotes;

        // increment number to add to the duplicate image name 
        public int number = 0;

        // ------------------------------------------------------- get/set methods    
        [Key]
        public int quoteID { get; set; }
        [Required]
        [MaxLength(100)]
        [Display(Name="Author")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage="Please fill in the textField")]
        public string author {get; set;}
        [Required]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage="Please fill in the textField")]
        [Display(Name="Quote")]
        public string quote {get; set;}
        [Url]
        public string permalink {get; set;}
        public string image {get; set;}

        public MyModel() {
            // initialization
            _quotes =  new List<SelectListItem>();
            _datalist = new List<Quote>();
            _randomquotes = new List<Quote>();
            // construct DB objects for use
            dbConnection = new MySqlConnection(Connection.CONNECTION_STRING);
            dbCommand = new MySqlCommand("", dbConnection); 
        }

        // // -------------------------------------------------- get/sets
        public List<SelectListItem> quotes { 
            get {
                return _quotes;
            }
        }

        public List<Quote> datalist {
            get {
                return _datalist;
            }
        }

        public List<Quote> randomquotes {
            get {
                return _randomquotes;
            }
        }
        
        public void SaveQuote(){
            // creating a new quote obect and setting fields
            Quote quoteObjectSave = new Quote();
            // values that were submitted by the user are on the right
            //getting values from the quote model and overwriting them
            quoteObjectSave.author = author;
            quoteObjectSave.quote = quote;
            quoteObjectSave.permalink = permalink;
            quoteObjectSave.image = image;
            // calls the create method
            quoteObjectSave.create();

        }
        public void DeleteQuote(){
            // bring a quoteID over to create a new quote obect delete and setting fields in order to know what quote info to delete
            Quote quoteObjectDelete = new Quote();
            quoteObjectDelete.quoteID = quoteID;
            // calls the delete method
            quoteObjectDelete.delete();
        }

        public void deleteImage(IWebHostEnvironment environment, string myTargetFolder) {
            

            try {
                // deletes image base on what quoteID it got
                dbConnection = new MySqlConnection(Connection.CONNECTION_STRING);
                dbConnection.Open();
                string sqlString = "SELECT image FROM tblQuotes WHERE quoteID = ?quoteID";
                dbCommand = new MySqlCommand(sqlString,dbConnection);
                dbCommand.Parameters.Clear();
                Console.WriteLine(quoteID);
                dbCommand.Parameters.AddWithValue("?quoteID", quoteID);
                string imagetodelete = dbCommand.ExecuteScalar().ToString();
                string nameofthefile = Path.GetFileName(imagetodelete);
                // gets the path if the file exists goes to the root
                if (File.Exists(environment.WebRootPath + "/" + myTargetFolder + "/" + nameofthefile)) {
                    File.Delete(environment.WebRootPath + "/" + myTargetFolder + "/" + nameofthefile);
                }

            } catch (Exception e) {
                Console.WriteLine(">>> An error has occured with Image");
                Console.WriteLine(">>> " + e.Message);
            } finally {
                dbConnection.Close();
            }

        }

        // ------------------------------------------------- public methods
        public void getQuotes() {
            try {
                // clear out quotes list for fresh population below
                // _quotes.Clear();
                dbConnection.Open();
                // gets all of the quotes from the database and their id and populates the list
                dbCommand.CommandText = "SELECT * FROM tblQuotes";
                dbReader = dbCommand.ExecuteReader();
                while (dbReader.Read()) {
                    SelectListItem item = new SelectListItem();
                    if (Convert.ToString(dbReader["quote"]).Length > 100) {
                        item.Text = Convert.ToString(dbReader["quote"]).Substring(0, 100) + "...";
                    }
                    else {
                        item.Text = Convert.ToString(dbReader["quote"]);
                    }
                    item.Value = Convert.ToString(dbReader["quoteID"]); 
                    // add object to list
                    _quotes.Add(item);
                }
                dbReader.Close();
            } finally {
                dbConnection.Close();
            }
        }

        public void getData() {
            try {
                // gets all fo the data from the database and adds it to the dataist that is being used for the WebAPI
                _datalist.Clear();
        
                dbConnection.Open();
              
                dbCommand.CommandText = "SELECT * FROM tblQuotes";
                
                dbReader = dbCommand.ExecuteReader();
                while (dbReader.Read()) {
                    Quote list = new Quote();
                    list.quoteID = Convert.ToInt32(dbReader["quoteID"]);
                    list.author = Convert.ToString(dbReader["author"]);
                    list.quote = Convert.ToString(dbReader["quote"]);
                    list.permalink = Convert.ToString(dbReader["permalink"]);
                    list.image = Convert.ToString(dbReader["image"]);
                    // add object to list
                    _datalist.Add(list);
                }
                dbReader.Close();

            } catch (Exception e) {
                Console.WriteLine(">>> An error has occurred with get Data");
                Console.WriteLine(">>> " + e.Message);
            } finally {
                dbConnection.Close();
            }
        }


        public void RandomQuotes(int count){
            // method for checking for duplictes 
            List<Quote> datalistcopy = new List<Quote>(_datalist);

            for (int n=0; n<count; n++) {
            // randomly pick index of quote in JSON data
                // if statement for the error check if quotenumber is out of range goes to the else statement and returns all of the quotes that are in the database.
                if(count < 39) {
                    Random rnd = new Random();
                    int index = rnd.Next(0,datalistcopy.Count - 1);       
                    // add quote to quotes array
                    _randomquotes.Add(datalistcopy[index]);
                    // remove quotes to avoid duplicates
                    datalistcopy.RemoveAt(index);
                }else{
                    _randomquotes = datalist;
                   
                }
                
            }
        
        }

        public MyModel(IWebHostEnvironment env, string myTargetFolder) {
            // initialization
            targetFolder = myTargetFolder;   
            // check to see if web app's root folder has an "uploads" folder - if not create it
            fullPath = env.WebRootPath + "/" + targetFolder + "/";
            Console.WriteLine(fullPath);
            if (!Directory.Exists(fullPath)) {
                Directory.CreateDirectory(fullPath);
            }
        }

        public int uploadImage(IFormFile file, string filename) {
            // error check 1 : no file is found
            // if (file != null) {
                // error check 2 : check file type
                string contentType = file.ContentType;
                if ((contentType == "image/png") || (contentType == "image/jpeg") || (contentType == "image/gif")) {
                    // error check 3 : check file size is below limit
                    long size = file.Length;
                    if ((size > 0) && (size < UPLOADLIMIT)) {
                        // error check 4 : check to make sure the filename is under 100 characters
                        if (filename.Length <= 100) {
                            // safe to save the file to the server!
                            FileStream stream = new FileStream((fullPath + filename), FileMode.Create);
                            try {
                                // copy the IFormFile object to the stream which writes it to the File System
                                file.CopyTo(stream);
                                stream.Close();
                                return MyModel.SUCCESS;
                            } catch {
                                stream.Close();
                                return MyModel.ERROR_SAVE;
                            }
                        } else {
                            return MyModel.ERROR_NAME_LENGTH;
                        }
                    } else {
                        return MyModel.ERROR_SIZE;
                    }
                } else {
                    return MyModel.ERROR_TYPE;
                
            // } else {
                // return MyModel.ERROR_NO_FILE;
            }
        }

    }
}
