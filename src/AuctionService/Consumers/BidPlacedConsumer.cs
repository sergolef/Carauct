using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class BidPlacedConsumer : IConsumer<BidPlaced>
    {
        private readonly AuctionDbContext _auctionDbContext;

        public BidPlacedConsumer(AuctionDbContext auctionDbContext)
        {
            _auctionDbContext = auctionDbContext;
        }

        public async Task Consume(ConsumeContext<BidPlaced> context)
        {
            Console.WriteLine("--> Consuming auction bid placed");

            var auction = await _auctionDbContext.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId));

            // check if bid greater than previouse bid
            if (auction.CurrentHighBid == null
            || context.Message.BidStatus.Contains("Accepted")
            && auction.CurrentHighBid < context.Message.Amount)
            {
                auction.CurrentHighBid = context.Message.Amount;
                await _auctionDbContext.SaveChangesAsync();
            }

        }
    }
}