using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADO_Demo.SQLHelpers;
using System.Data;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            GetAndSendEmail();

            Console.ReadLine();

        }


        public static void GetAndSendEmail()
        {

            DataTable Student = new DataTable();

            Student = SQLHelpers.GetTable("DemoConnection", "[dbo].[GetStudent_all]", null, true);


            if (Student.Rows.Count > 0)
            {

                string StudentId = null;
                string FirstName = null;
                string LastName = null;
                int Age = 0;
                string Gender= null;
                string Address = null;

                foreach (DataRow row in Student.Rows)
                {
                    StudentId = row["StudentId"].ToString();
                    FirstName = row["FirstName"].ToString();
                    LastName = row["LastName"].ToString();
                    Age = (int)row["Age"];
                    Gender = row["Gender"].ToString();
                    Address = row["Address"].ToString();

                    Console.WriteLine(FirstName + ' ' + LastName);

                }
            }

        }

    }
}
