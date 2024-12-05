using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Entities;

namespace BiddingService.Models
{
    public class Bid : Entity
    {
        public string AuctionId { get; set; }

        public string Bidder { get; set; }

        public int Amount { get; set; } 

        public DateTime BidTime { get; set; }   = DateTime.UtcNow;

        public BidStatus BidStatus { get; set; }

    }
}