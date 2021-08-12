﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.PaymentMethod
{
    public class CreatePaymentMethod
    {
        public class Command : IRequest
        {
            public string Name { get; set; }
            public string ImagePath { get; set; }
            public double Price { get; set; }
        }
        
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.Name).NotEmpty();
                RuleFor(p => p.ImagePath).NotEmpty();
                RuleFor(p => p.Price).NotEmpty().GreaterThanOrEqualTo(0);
            }
        }
        
        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly DataContext _context;
            private readonly IUnitOfWork _unitOfWork;

            public Handler(DataContext context, IUnitOfWork unitOfWork)
            {
                _context = context;
                _unitOfWork = unitOfWork;
            }
            
            
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var paymentMethod = new Domain.Models.PaymentMethod
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Price = request.Price,
                    ImagePath = request.ImagePath,
                    Orders = new List<Domain.Models.Order>()
                };

                await _context.PaymentMethods.AddAsync(paymentMethod, cancellationToken);
                await _unitOfWork.CommitTransactionsAsync();
                return await Task.FromResult(Unit.Value);
            }
        }
    }
}