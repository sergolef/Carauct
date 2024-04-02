'use client'

import React, { useEffect, useState } from 'react'
import AuctionCard from './AuctionCard';
import { Auction, PageResult } from '@/types';
import AppPagination from '../components/AppPagination';

import {getData} from '../actions/auctionActions'; 
import Filters from './Filters';
import { useParamsStore } from '../hooks/useParamsStore';
import qs from 'query-string';
import { shallow } from 'zustand/shallow';

export default function Listings() {
    const [data, setData] = useState<PageResult<Auction>>();
    const params = useParamsStore(state => ({
        pageNumber: state.pageNumber,
        pageSize: state.pageSize,
        searchTerm: state.searchTerm
    }), shallow)

    const setParams = useParamsStore(state => state.setParams);
    const url = qs.stringifyUrl({url: '', query:params});

    function setPageNumber(pageNumber:number){
        setParams({pageNumber})
    }

    useEffect(() => {
        getData(url).then( data => {
            setData(data); 
            //setPageNumber(data.totalCount);
        })
        
    }, [url])

    if(!data) return <h3>Loading...</h3>

  return (
    <>
    <Filters></Filters>
     <div className='grid grid-cols-4 gap-6'>
        {
            data.results.map(auction =>(
                <AuctionCard auction={auction} key={auction.id}></AuctionCard>
            ))
        }
    </div>
    <div className='flex justify-center mt-4'>
        <AppPagination pageChanged={setPageNumber} currentPage={params.pageNumber} pageCount={data.pageCount}></AppPagination>
    </div>
    </>
   
  )
}
