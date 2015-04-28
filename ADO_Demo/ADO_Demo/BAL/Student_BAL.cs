using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ADO_Demo.BAL
{
    public class Student_BAL
    {

        public static List<Student>  GetUserAll()
        {
            
            DataTable dt = new DataTable();

            dt = SQLHelpers.SQLHelpers.GetTable("DemoConnection", "[dbo].[GetStudent_all]", null, true);
            

            List<Student> studentList = new List<Student>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Student student = new Student();

                student.StudentId = dt.Rows[i]["StudentId"].ToString();
                student.FirstName = dt.Rows[i]["FirstName"].ToString();
                student.LastName = dt.Rows[i]["LastName"].ToString();
                student.Age = (int)dt.Rows[i]["Age"];
                student.Gender = dt.Rows[i]["Gender"].ToString();
                student.Batch = dt.Rows[i]["Batch"].ToString();
                student.Address = dt.Rows[i]["Address"].ToString();
                student.Class = dt.Rows[i]["Class"].ToString();
                student.School = dt.Rows[i]["School"].ToString();
                student.Domicile = dt.Rows[i]["Domicile"].ToString();

                studentList.Add(student);
            } 

            return studentList;

        }

    }
}