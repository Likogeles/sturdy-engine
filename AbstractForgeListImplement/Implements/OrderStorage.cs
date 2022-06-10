﻿using AbstractForgeContracts.BindingModels;
using AbstractForgeContracts.StoragesContracts;
using AbstractForgeContracts.ViewModels;
using AbstractForgeListImplement.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AbstractForgeListImplement.Implements
{
    public class OrderStorage : IOrderStorage
    {
        private readonly DataListSingleton source;
        
        public OrderStorage()
        {
            source = DataListSingleton.GetInstance();
        }
        
        public List<OrderViewModel> GetFullList()
        {
            var result = new List<OrderViewModel>();
            foreach (var order in source.Orders)
            {
                result.Add(CreateModel(order));
            }
            return result;
        }
        
        public List<OrderViewModel> GetFilteredList(OrderBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            var result = new List<OrderViewModel>();
            foreach (var order in source.Orders)
            {
                if (order.Id.Equals(model.Id))
                {
                    result.Add(CreateModel(order));
                }
            }
            return result.Where(rec => rec.Id.Equals(model.Id) || rec.DateCreate >= model.DateFrom && rec.DateCreate <= model.DateTo)
            .ToList();

        }
        
        public OrderViewModel GetElement(OrderBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            foreach (var order in source.Orders)
            {
                if (order.Id == model.Id)
                {
                    return CreateModel(order);
                }
            }
            return null;
        }
        
        public void Insert(OrderBindingModel model)
        {
            var tempOrder = new Order { Id = 1 };
            foreach (var order in source.Orders)
            {
                if (order.Id >= tempOrder.Id)
                {
                    tempOrder.Id = order.Id + 1;
                }
            }
            source.Orders.Add(CreateModel(model, tempOrder));
        }
        
        public void Update(OrderBindingModel model)
        {
            Order tempOrder = null;
            foreach (var order in source.Orders)
            {
                if (order.Id == model.Id)
                {
                    tempOrder = order;
                }
            }
            if (tempOrder == null)
            {
                throw new Exception("Заказ не найден");
            }
            CreateModel(model, tempOrder);
        }
        
        public void Delete(OrderBindingModel model)
        {
            for (int i = 0; i < source.Orders.Count; ++i)
            {
                if (source.Orders[i].Id == model.Id.Value)
                {
                    source.Orders.RemoveAt(i);
                    return;
                }
            }
            throw new Exception("Заказ не найден");
        }

        private static Order CreateModel(OrderBindingModel model, Order order)
        {
            order.ManufactureId = model.ManufactureId;
            order.Count = model.Count;
            order.Sum = model.Sum;
            order.Status = model.Status;
            order.DateCreate = model.DateCreate;
            order.DateImplement = model.DateImplement;
            return order;
        }

        private OrderViewModel CreateModel(Order order)
        {
            string manufactureName = null;
            foreach (var document in source.Manufactures)
            {
                if (document.Id == order.ManufactureId)
                {
                    manufactureName = document.ManufactureName;
                }
            }

            return new OrderViewModel
            {
                Id = order.Id,
                ManufactureId = order.ManufactureId,
                ManufactureName = manufactureName,
                Count = order.Count,
                Sum = order.Sum,
                Status = order.Status.ToString(),
                DateCreate = order.DateCreate,
                DateImplement = order.DateImplement,
            };
        }
    }
}
