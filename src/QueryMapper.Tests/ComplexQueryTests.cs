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
            public List<OrderDto> Orders { get; set; }
        }

        public class OrderDto
        {
            public int OrderId { get; set; }
            public DateTime OrderDate { get; set; }
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
            public ICollection<OrderEntity> Orders { get; set; }
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
                .ForMember(d => d.Orders, d => d.MapFrom(s => s.Orders));
            Mapper.CreateMap<CustomerEntity, CustomerDto>()
                .ForMember(d => d.CustomerId, d => d.MapFrom(s => s.Id))
                .ForMember(d => d.CustomerName, d => d.MapFrom(s => s.Name))
                .ForMember(d => d.Orders, d => d.MapFrom(s => s.Orders.ToList()));
            Mapper.CreateMap<OrderDto, OrderEntity>()
                .ForMember(d => d.Id, d => d.MapFrom(s => s.OrderId))
                .ForMember(d => d.OrderDate, d => d.MapFrom(s => s.OrderDate));
            Mapper.CreateMap<OrderEntity, OrderDto>()
                .ForMember(d => d.OrderId, d => d.MapFrom(s => s.Id))
                .ForMember(d => d.OrderDate, d => d.MapFrom(s => s.OrderDate));

            var orderDtos = new List<OrderDto>
            {
                new OrderDto
                {
                    OrderId = 1,
                    OrderDate = DateTime.Now.AddDays(-10)
                },
                new OrderDto
                {
                    OrderId = 2,
                    OrderDate = DateTime.Now.AddDays(-10)
                },
                new OrderDto
                {
                    OrderId = 3,
                    OrderDate = DateTime.Now.AddDays(-10)
                },
                new OrderDto
                {
                    OrderId = 4,
                    OrderDate = DateTime.Now.AddDays(-10)
                }
            };

            var customerDtos = new List<CustomerDto>
            {
                new CustomerDto
                {
                    CustomerId = 1,
                    CustomerName = "CustomerOne",
                    Orders = orderDtos
                },
                new CustomerDto
                {
                    CustomerId = 2,
                    CustomerName = "CustomerTwo",
                    Orders = orderDtos
                },
                new CustomerDto
                {
                    CustomerId = 3,
                    CustomerName = "CustomerThree",
                    Orders = new List<OrderDto>()
                }
            };

            var query = _customers.AsQueryable().AsMappedQuery(
                new DelegatedMapper<CustomerEntity, CustomerDto>(t => Mapper.Map<CustomerDto>(t)))
                .Where(q => q.Orders.Any());

            var result = query.ToArray();
        }
    }
}
