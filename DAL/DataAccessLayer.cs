using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DAL
{
    public class DataAccessLayer
    {
        private readonly string connectionString;

        public DataAccessLayer()
        {
            connectionString = ConfigurationManager.ConnectionStrings["constring"].ConnectionString;
        }
        public int? GetCustomerIdByEmail(string email)
        {
            int? customerId = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("ValidateEmailAndGetCustomerId", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Email", email);
                        SqlParameter outputParam = new SqlParameter("@CustomerId", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outputParam);

                        connection.Open();
                        command.ExecuteNonQuery();

                        if (outputParam.Value != DBNull.Value)
                        {
                            customerId = (int)outputParam.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
           

            return customerId;
        }
        public List<Product> GetProducts()
        {
            try
            {
                List<Product> products = new List<Product>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("GetProducts", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Product product = new Product
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price"))
                                };
                                products.Add(product);
                            }
                        }
                    }
                }

                return products;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void InsertOrder(int customerId, List<int> productIds, Dictionary<int, int> quantities)
        {
            try
            {
                string productIdsString = string.Join(",", productIds);
                string quantitiesString = string.Join(",", quantities.Select(kvp => $"{kvp.Key}:{kvp.Value}"));
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("InsertOrder", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CustomerId", customerId);
                        command.Parameters.AddWithValue("@ProductIds", productIdsString);
                        command.Parameters.AddWithValue("@Quantities", quantitiesString);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
