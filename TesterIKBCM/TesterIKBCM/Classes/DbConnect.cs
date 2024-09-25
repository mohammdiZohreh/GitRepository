using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace MainProject
{
    public static class DbConnect
    {
        //public static SqlConnection con = new SqlConnection(@"Data Source=.;Initial Catalog=EKS;Integrated Security=True;Pooling=False");
        public static SqlConnection con = new SqlConnection(@"Data Source=.;Initial Catalog=EKS;Integrated Security=True;Pooling=False");
        public static SqlCommand sql_command;

        static DbConnect (){

            //con.ConnectionString = "Data Source =.; Initial Catalog = EKS; Integrated Security = True; Pooling=False";
            //con.ConnectionString = GetConnectionStringByName("MyDb");
        }
        static string GetConnectionStringByName(string name)
        {
            // Assume failure.
            string returnValue = null;

            ConnectionStringSettingsCollection settings2 =
           ConfigurationManager.ConnectionStrings;
            // Look for the name in the connectionStrings section.
            ConnectionStringSettings settings =
                ConfigurationManager.ConnectionStrings[name];

            // If found, return the connection string.
            if (settings != null)
                returnValue = settings.ConnectionString;

            return returnValue;
        }
       


    }
}
