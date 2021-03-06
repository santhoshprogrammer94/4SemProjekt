﻿using System.Threading;
using System.Threading.Tasks;
using Auth.Application.Common.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToolBox.Contracts.Auth;

namespace Auth.Application.AuthUser.Commands.UpdateAuthUser
{
    public class UpdateAuthUserConsumer : IConsumer<ToolBox.Contracts.Auth.UpdateAuthUser>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IHashService _hashService;
        private readonly ILogger<UpdateAuthUserConsumer> _logger;

        public UpdateAuthUserConsumer(IApplicationDbContext dbContext, IHashService hashService,
            ILogger<UpdateAuthUserConsumer> logger)
        {
            _dbContext = dbContext;
            _hashService = hashService;
            _logger = logger;
        }
        
        public async Task Consume(ConsumeContext<ToolBox.Contracts.Auth.UpdateAuthUser> context)
        {
            _logger.LogInformation("UpdateAuthUserConsumer Called");
            
            var userToUpdate = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == context.Message.Id);
            var salt = _hashService.GenerateSalt();

            if (context.Message.Username != null) userToUpdate.UserName = context.Message.Username;
            if (context.Message.Email != null) userToUpdate.Email = context.Message.Email;
            if (context.Message.Password != null)
            {
                userToUpdate.PasswordSalt = salt;
                userToUpdate.PasswordHash = _hashService.GenerateHash(context.Message.Password, salt);
            }

            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            await context.Publish<AuthUserUpdated>(new
            {
                userToUpdate.Id
            });
        }
    }
}