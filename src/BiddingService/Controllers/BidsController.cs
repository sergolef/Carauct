using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BiddingService.DTOs;
using BiddingService.Models;
using BiddingService.Services;
using Contracts;
using DnsClient.Internal;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Entities;
using ZstdSharp.Unsafe;

namespace BiddingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BidsController : ControllerBase
    {
        IMapper _mapper;
        IPublishEndpoint _endpoint;
        private readonly GrpcAuctionClient _grpcClient;

        public BidsController(IMapper mapper, IPublishEndpoint endpoint, GrpcAuctionClient grpcClient)
        {
            _mapper = mapper;
            _endpoint = endpoint;
            _grpcClient = grpcClient;
        }

        //private readonly IMapper _mapper;
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BidDto>> PlaceBid(string auctionId, int amount){
            var auction = await DB.Find<Auction>().OneAsync(auctionId);

            if (auction == null){
                //TODO: check with auction service if that has auction
                auction = _grpcClient.GetAuction(auctionId);

                if(auction == null) return BadRequest("Cannot accept bids on this auction at this time");
            }

            if(auction.Seller == User.Identity.Name){
                return BadRequest("You cannot bid on your own auction");
            }

            var bid = new Bid
            {
                Amount = amount,
                AuctionId = auctionId,
                Bidder = User.Identity.Name
            };
            
            
            if(auction.AuctionEnd < DateTime.UtcNow)
            {
                bid.BidStatus = BidStatus.Finished;
            }
            else
            {
                var highBid = await DB.Find<Bid>()
                    .Match(a => a.AuctionId == auctionId)
                    .Sort(b => b.Descending(x => x.Amount))
                    .ExecuteFirstAsync();
                

                if (highBid != null && amount > highBid.Amount || highBid == null)
                {
                    bid.BidStatus = amount > auction.ReservePrice ? BidStatus.Accepted : BidStatus.AcceptedBelowReserve;
                }

                if(highBid!= null && bid.Amount <= highBid.Amount)
                {
                    bid.BidStatus = BidStatus.TooLow;
                }
            }

            

            await DB.SaveAsync(bid);

            await _endpoint.Publish(_mapper.Map<BidPlaced>(bid));

            return Ok(_mapper.Map<BidDto>(bid));
        }

        [HttpGet("{auctionId}")]
        public async Task<ActionResult<List<BidDto>>> GetBidForAuction(string auctionId)
        {
            var bids = await DB.Find<Bid>()
                .Match(a => a.AuctionId == auctionId)
                .Sort(b => b.Descending(c => c.BidTime))
                .ExecuteAsync();
                
            return bids.Select(_mapper.Map<BidDto>).ToList();
        }
    }
}