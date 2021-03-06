﻿using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using SignalR.Application.Common.Interfaces;
using SignalR.Domain.Enums;

namespace SignalR.Application.Reservation.IntegrationEvents
{
    public class ReservationCreatedConsumer : IConsumer<ToolBox.Contracts.Reservation.ReservationCreated>
    {
        private readonly IReservationService _reservationService;
        private readonly ILogger<ReservationCreatedConsumer> _logger;

        public ReservationCreatedConsumer(IReservationService reservationService, ILogger<ReservationCreatedConsumer> logger)
        {
            _reservationService = reservationService;
            _logger = logger;
        }
        
        public async Task Consume(ConsumeContext<ToolBox.Contracts.Reservation.ReservationCreated> context)
        {
            _logger.LogInformation("ReservationCreatedConsumer Called");
            await _reservationService.SendReservation(Type.Create, new Domain.Entities.Reservation
            {
                Id = context.Message.Id,
                UserId = context.Message.UserId,
                ResourceId = context.Message.ResourceId,
                From = context.Message.From,
                To = context.Message.To
            });
            await Task.CompletedTask;
        }
    }
}