﻿using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Resource.Application.Common.Interfaces;

namespace Contracts.Resource
{
    public interface GetResource
    {
        public Guid Id { get; set; }
    }

    public class GetResourceConsumer : IConsumer<GetResource>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetResourceConsumer(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task Consume(ConsumeContext<GetResource> context)
        {
            try
            {
                var resource = await _dbContext.Resources.FirstOrDefaultAsync(r => r.Id == context.Message.Id);
                await context.RespondAsync<ResourceVm>(new
                {
                    Resource = resource
                });
            }
            catch (Exception e)
            {
                await context.RespondAsync<NotFound>(new
                {
                    Message = e.Message
                });
            }
        }
    }

    public interface ResourceVm
    {
        public global::Resource.Domain.Entities.Resource Resource { get; set; }
    }

    public interface NotFound
    {
        public string Message { get; set; }
    }
}