﻿using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.PaymentMethod
{
    public class AssignPaymentToDelivery
    {
        public class Command : IRequest
        {
            public Guid DeliveryId { get; set; }
            public Guid PaymentId { get; set; }
        }
        
        public class MyClass : IRequestHandler<Command, Unit>
        {
            private readonly DataContext _context;
            private readonly IUnitOfWork _unitOfWork;

            public MyClass(DataContext context, IUnitOfWork unitOfWork)
            {
                _context = context;
                _unitOfWork = unitOfWork;
            }
            
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var existingDelivery = await _context.DeliveryMethods.FindAsync(request.DeliveryId);
                var existingPayment = await _context.PaymentMethods.Include(p => p.DeliveryMethods).FirstOrDefaultAsync(p => p.Id == request.PaymentId);

                if (existingDelivery == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono metody dostawy dla podanego identyfikatora"});
                }
                
                if (existingPayment == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono metody płatności dla podanego identyfikatora"});
                }

                var deliveryIncluded = existingPayment.DeliveryMethods;
                deliveryIncluded.Add(existingDelivery);

                existingPayment.DeliveryMethods = deliveryIncluded;

                _context.PaymentMethods.Update(existingPayment);
                await _unitOfWork.CommitTransactionsAsync();
                return await Task.FromResult(Unit.Value);
            }
        }
    }
}