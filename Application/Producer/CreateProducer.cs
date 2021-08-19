﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Photo;
using Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Persistence.Context;

namespace Application.Producer
{
    public class CreateProducer
    {
        public class Command : IRequest
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public IFormFile Image { get; set; }
        }
        
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.Name).NotEmpty();
                RuleFor(p => p.Description).NotEmpty();
            }
        }
        
        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly DataContext _context;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IPhotoAccessor _photoAccessor;

            public Handler(DataContext context, IUnitOfWork unitOfWork, IPhotoAccessor photoAccessor)
            {
                _context = context;
                _unitOfWork = unitOfWork;
                _photoAccessor = photoAccessor;
            }
            
            
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                PhotoUploadResult uploadedImage;
                
                try
                {
                    uploadedImage = _photoAccessor.AddPhoto(request.Image);
                }
                catch (Exception e)
                {
                    throw new RestException(HttpStatusCode.BadRequest, new {info = e.Message});
                }
                
                var producer = new Domain.Models.Producer
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    Image = uploadedImage.Url,
                    Products = new List<Domain.Models.Product>()
                };

                await _context.Producers.AddAsync(producer, cancellationToken);
                await _unitOfWork.CommitTransactionsAsync();
                return await Task.FromResult(Unit.Value);
            }
        }
    }
}