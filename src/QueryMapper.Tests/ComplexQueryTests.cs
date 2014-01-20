using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace QueryMapper.Tests
{
    [TestClass]
    public class ComplexQueryTests
    {
        public class CustomerDto
        {
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public int OrderCount { get; set; }
        }

        public class OrderEntity
        {
            public int Id { get; set; }
            public DateTime OrderDate { get; set; }
        }

        public class CustomerEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<OrderEntity> Orders { get; set; }
        }

        private List<OrderEntity> _orders;
        private List<CustomerEntity> _customers;

        [TestInitialize]
        public void Initialize()
        {
            _orders = new List<OrderEntity>
            {
                new OrderEntity{
                    Id = 1, 
                    OrderDate = DateTime.Now.AddDays(-10)
                },
                new OrderEntity{
                    Id = 2, 
                    OrderDate = DateTime.Now.AddDays(-10)
                },
                new OrderEntity{
                    Id = 1, 
                    OrderDate = DateTime.Now.AddDays(-10)
                },
                new OrderEntity{
                    Id = 1, 
                    OrderDate = DateTime.Now.AddDays(-10)
                },
            };

            _customers = new List<CustomerEntity>
            {
                new CustomerEntity
                {
                    Id = 1,
                    Name = "CustomerOne",
                    Orders = _orders
                },
                new CustomerEntity
                {
                    Id = 2,
                    Name = "CustomerTwo",
                    Orders = _orders
                },
                new CustomerEntity
                {
                    Id = 3,
                    Name = "CustomerThree",
                    Orders = new List<OrderEntity>()
                }
            };
        }

        [TestMethod]
        public void TestQueryMapperWhenCheckingForChildElements()
        {
            Mapper.CreateMap<CustomerDto, CustomerEntity>()
                .ForMember(d => d.Id, d => d.MapFrom(s => s.CustomerId))
                .ForMember(d => d.Name, d => d.MapFrom(s => s.CustomerName))
                .ForMember(d => d.Orders, d => d.Ignore());

            var customerDtos = new List<CustomerDto>
            {
                new CustomerDto
                {
                    CustomerId = 1,
                    CustomerName = "CustomerOne",
                    OrderCount = 4
                },
                new CustomerDto
                {
                    CustomerId = 2,
                    CustomerName = "CustomerTwo",
                    OrderCount = 4
                },
                new CustomerDto
                {
                    CustomerId = 3,
                    CustomerName = "CustomerThree",
                    OrderCount = 0
                }
            };

            var query = customerDtos.AsQueryable().AsMappedQuery(
                new DelegatedMapper<CustomerDto, CustomerEntity>(t => Mapper.Map<CustomerEntity>(t)))
                .Where(q => q.Orders.Any());

            var result = query.ToArray();
        }
    }
}
