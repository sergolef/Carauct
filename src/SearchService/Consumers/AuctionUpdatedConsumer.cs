using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers
{
    public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
    {
        private readonly IMapper _mapper;

        public AuctionUpdatedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<AuctionUpdated> context)
        {
            Console.WriteLine("--> Consuming auction updated: "+ context.Message.Id);

            var item = _mapper.Map<Item>(context.Message);

            

            var result = await DB.Update<Item>()
                .Match(e => e.ID == context.Message.Id)
                .ModifyOnly(b => new {
                    b.Mileage,
                    b.Color,
                    b.Make,
                    b.Model,
                    b.Year
                }, item).ExecuteAsync();

                if(!result.IsAcknowledged)
                {
                    throw new MessageException(typeof(AuctionUpdated), "Problem of updating mongodb");
                } 
        }
    }
}