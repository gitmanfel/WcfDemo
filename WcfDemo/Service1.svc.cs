using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Configuration;

namespace WcfDemo
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" dans le code, le fichier svc et le fichier de configuration.
    // REMARQUE : pour lancer le client test WCF afin de tester ce service, sélectionnez Service1.svc ou Service1.svc.cs dans l'Explorateur de solutions et démarrez le débogage.
    public class Service1 : IService1
    {
       

        public static String ConnectionString = ConfigurationManager.ConnectionStrings["MyDbConn"].ConnectionString;


        public List<student> GetStudents()
        {
            List<student> studentList = new List<student>();
            DataTable resourceTable = new DataTable();
            SqlDataReader resultReader = null;
            SqlConnection connection = new SqlConnection(ConnectionString);
            SqlCommand command = new SqlCommand("GetStudent", connection);
            command.CommandType = CommandType.StoredProcedure;
            try
            {
                connection.Open();
                resultReader = command.ExecuteReader();
                resourceTable.Load(resultReader);
                resultReader.Close();
                connection.Close();
                studentList = (from DataRow dr in resourceTable.Rows
                               select new Student()
                               {
                                   StudentId = Convert.ToInt64(dr["StudentId"]),
                                   FirstName = dr["FirstName"].ToString(),
                                   LastName = dr["LastName"].ToString(),
                                   RegisterNo = dr["RegisterNo"].ToString(),
                                   Department = dr["Department"].ToString()
                               }).ToList();
            }
            catch (Exception exception)
            {
                if (resultReader != null || connection.State == ConnectionState.Open)
                {
                    resultReader.Close();
                    connection.Close();
                }
                throw new FaultException<exceptionmessage>(new ExceptionMessage(exception.Message));
            }
            return studentList;
        }

        public void AddStudents(Student student)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("AddStudent", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@StudentId", student.StudentId));
                    command.Parameters.Add(new SqlParameter("@FirstName", student.FirstName));
                    command.Parameters.Add(new SqlParameter("@LastName", student.LastName));
                    command.Parameters.Add(new SqlParameter("@RegisterNo", student.RegisterNo));
                    command.Parameters.Add(new SqlParameter("@Department", student.Department));
                    object result = command.ExecuteScalar();
                    connection.Close();
                }
            }
            catch (SqlException exception)
            {
                throw new FaultException<exceptionmessage>(new ExceptionMessage(exception.Message));
            }
        }

        public void DeleteStudent(long StudentId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("DeleteStudent", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@StudentId", StudentId));
                    int result = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (SqlException exception)
            {
                throw new FaultException<exceptionmessage>(new ExceptionMessage(exception.Message));
            }
        }
    }
}
