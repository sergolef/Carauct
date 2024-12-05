using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BiddingService.Models;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BiddingService.Services
{
    public class CheckAuctionFinished : BackgroundService
    {
        IServiceProvider _service;
        ILogger _logger;

        public CheckAuctionFinished(ILogger<CheckAuctionFinished> logger, IServiceProvider service)
        {
            _logger = logger;
            _service = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting check for finished auctions");


            stoppingToken.Register(() => _logger.LogInformation("==> Auction check is stopping"));

            while(!stoppingToken.IsCancellationRequested)
            {
                await CheckAuction(stoppingToken);

                await Task.Delay(5000, stoppingToken);
            }

        }

        private async Task CheckAuction(CancellationToken stoppingToken)
        {
            var finisheedAuction = await DB.Find<Auction>()
                .Match(x => x.AuctionEnd <= DateTime.UtcNow)
                .Match(x => !x.Finished)
                .ExecuteAsync(stoppingToken);

            if(finisheedAuction.Count == 0) return;

            _logger.LogInformation("==> Found {count} auction that have completed", finisheedAuction.Count);
        
            using var scope = _service.CreateScope();

            var endpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            foreach( var auction in finisheedAuction)
            {
                auction.Finished = true;
                await auction.SaveAsync(null, stoppingToken);

                var winningBid = await DB.Find<Bid>()
                    .Match(x => x.AuctionId == auction.ID)
                    .Match(b => b.BidStatus == BidStatus.Accepted)
                    .Sort(x => x.Descending(s => s.Amount))
                    .ExecuteFirstAsync(stoppingToken);
                
                await endpoint.Publish(new AuctionFinished {
                    ItemSold = winningBid != null,
                    AuctionId = auction.ID,
                    Winner = winningBid?.Bidder,
                    Amount = winningBid?.Amount,
                    Seller = auction.Seller
                }, stoppingToken);


            }
        }
    }
}