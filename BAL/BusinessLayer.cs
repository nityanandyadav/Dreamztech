using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL
{
    public class BusinessLayer
    {
        private readonly DataAccessLayer dataAccessLayer;

        public BusinessLayer()
        {
            dataAccessLayer = new DataAccessLayer();
        }

        public int? GetCustomerIdByEmail(string email)
        {
            try
            {
                return dataAccessLayer.GetCustomerIdByEmail(email);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public List<Product> GetProducts()
        {
            try
            {
                return dataAccessLayer.GetProducts();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void InsertOrder(int customerId, List<int> productIds, Dictionary<int, int> quantities)
        {
            try
            {
                 dataAccessLayer.InsertOrder(customerId, productIds, quantities);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
