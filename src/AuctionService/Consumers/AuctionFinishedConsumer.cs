using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
    {
        private readonly AuctionDbContext _context;
        private readonly AuctionDbContext _auctionDbContext;

        public AuctionFinishedConsumer(AuctionDbContext auctionDbContext)
        {
            _auctionDbContext = auctionDbContext;
        }

        public async Task Consume(ConsumeContext<AuctionFinished> context)
        {
            Console.WriteLine("--> Consuming auction finish"); 

            var auction = await _auctionDbContext.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId));

            if (context.Message.ItemSold)
            {
                auction.Winner = context.Message.Winner;
                auction.SoldAmaunt = context.Message.Amount;
            }

            auction.Status = auction.SoldAmaunt > auction.ReservePrice
             ? Entities.Status.Finished : Entities.Status.ReserveNotMet;

            await _auctionDbContext.SaveChangesAsync();
        }
    }
}