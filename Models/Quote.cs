using System;
using MySql.Data.MySqlClient;

namespace QuoteGeneratorAPI.Models {
// ------------------------------------------------------- get/set methods
    public class Quote {
        // database connectivity variables
        private MySqlConnection dbConnection;
        private MySqlCommand dbCommand;

        public int quoteID { get; set; }
        public string author {get; set;}
        public string quote {get; set;}
        public string permalink {get; set;}
        public string image {get; set;}

    // // -------------------------------------------------------- public methods
    public int create() {
        try {   
                // when the create is called inserts data to the database when the create is called
                // open connection
                dbConnection = new MySqlConnection(Connection.CONNECTION_STRING);
                dbConnection.Open();

                string sqlString = "INSERT INTO tblQuotes " +
                "(author,quote,permalink,image) VALUES " +
                "(?author,?quote,?permalink,?image)";
                // Populate Command Object
                dbCommand = new MySqlCommand(sqlString,dbConnection);
                dbCommand.Parameters.AddWithValue("?author", author);
                dbCommand.Parameters.AddWithValue("?quote", quote);
                dbCommand.Parameters.AddWithValue("?permalink", permalink);
                dbCommand.Parameters.AddWithValue("?image", image);
                dbCommand.ExecuteNonQuery();

                dbCommand.Parameters.Clear();

                sqlString = "SELECT @@identity";
                dbCommand.CommandText = sqlString;
                quoteID = Convert.ToInt32(dbCommand.ExecuteScalar());

            } finally {
                dbConnection.Close();
            }

            return quoteID;
        }

        public void delete() {
            
            // when the method is called goes into the database and deltes quotes based on the quoteID
            try {   
                // open connection
                dbConnection = new MySqlConnection(Connection.CONNECTION_STRING);
                dbConnection.Open();
                string sqlString = "DELETE FROM tblQuotes WHERE quoteID = ?quoteID";
                // Populate Command Object
                dbCommand = new MySqlCommand(sqlString,dbConnection);
                dbCommand.Parameters.AddWithValue("?quoteID", quoteID);
                dbCommand.ExecuteNonQuery();
                
            } catch (Exception e) {
                Console.WriteLine(">>> An error has occured with deleting Quotes");
                Console.WriteLine(">>> " + e.Message);
            } finally {
                dbConnection.Close();
            }
        }
    }

}
