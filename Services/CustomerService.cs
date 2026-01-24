using System;
using System.Collections.Generic;
using WarehouseManagement.Models;
using WarehouseManagement.Repositories;

using Newtonsoft.Json;

namespace WarehouseManagement.Services
{
    public class CustomerService
    {
        private readonly CustomerRepository _customerRepo;
        private readonly LogRepository _logRepo;

        public CustomerService()
        {
            _customerRepo = new CustomerRepository();
            _logRepo = new LogRepository();
        }

        public List<Customer> GetAllCustomers()
        {
            return _customerRepo.GetAllCustomers();
        }

        public Customer GetCustomerById(int id)
        {
            return _customerRepo.GetCustomerById(id);
        }

        public bool AddCustomer(Customer customer)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customer.CustomerName))
                    throw new ArgumentException("Tên khách hàng không được để trống");

                if (_customerRepo.AddCustomer(customer) > 0)
                {
                    _logRepo.LogAction("ADD_CUSTOMER", $"Thêm khách hàng: {customer.CustomerName}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm khách hàng: " + ex.Message);
            }
        }

        public bool UpdateCustomer(Customer customer)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customer.CustomerName))
                    throw new ArgumentException("Tên khách hàng không được để trống");

                var oldCustomer = _customerRepo.GetCustomerById(customer.CustomerID);
                if (oldCustomer == null) throw new ArgumentException("Khách hàng không tồn tại");

                var beforeData = new
                {
                    CustomerID = oldCustomer.CustomerID,
                    CustomerName = oldCustomer.CustomerName,
                    Phone = oldCustomer.Phone,
                    Email = oldCustomer.Email,
                    Address = oldCustomer.Address
                };

                if (_customerRepo.UpdateCustomer(customer))
                {
                    _logRepo.LogAction("UPDATE_CUSTOMER", 
                        $"Cập nhật khách hàng ID {customer.CustomerID}",
                        JsonConvert.SerializeObject(beforeData));
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật khách hàng: " + ex.Message);
            }
        }

        public bool DeleteCustomer(int id)
        {
            try
            {
                var oldCustomer = _customerRepo.GetCustomerById(id);
                if (oldCustomer != null)
                {
                    var beforeData = new
                    {
                        CustomerID = oldCustomer.CustomerID,
                        CustomerName = oldCustomer.CustomerName,
                        Phone = oldCustomer.Phone,
                        Email = oldCustomer.Email,
                        Address = oldCustomer.Address
                    };

                    if (_customerRepo.SoftDeleteCustomer(id))
                    {
                        _logRepo.LogAction("DELETE_CUSTOMER", 
                            $"Xóa (ẩn) khách hàng ID {id}",
                            JsonConvert.SerializeObject(beforeData));
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa khách hàng: " + ex.Message);
            }
        }
    }
}
