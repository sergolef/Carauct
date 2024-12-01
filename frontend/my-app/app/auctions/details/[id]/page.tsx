import { getDetailedViewData } from '@/app/actions/auctionActions'
import Heading from '@/app/components/Heading';
import React from 'react'
import CountdownTimer from '../../CountdownTimer';
import CarImage from '../../CarImage';
import DetailedSpecs from './DetailedSpecs';
import { getCurrentUser } from '@/app/actions/authActions';
import EditButton from './EditButton';
import DeleteButton from './DeleteButton';

export default async function Details({params}: {params:{id:string}}) {
  const data = await getDetailedViewData(params.id);
  const user = await getCurrentUser();

  return (
    <div>
      <div className='flex justify-between'>
        <div className="flex justify-between gap-3">
        <Heading title={`${data.make}`} subtitle=''></Heading>
          {user?.username === data.seller && 
          <>
          <EditButton id={data.id} />
          <DeleteButton id={data.id}/>
          </>
            
          }
        </div>
      

      <div className='flex gap-3'>
        <h3 className='text2xl font-semibold'>Time remaining:</h3>
        <CountdownTimer auctionEnd={data.auctionEnd} />
      </div>
    </div>

    <div className='grid grid-cols-2 gap-6 mt-3'>
      <div className='w-full bg-grey-200 aspect-h-10 aspect-w-16 rounded-lg overflow-hidden'>
        { data.imageUrl ? <CarImage imageUrl={data.imageUrl} ></CarImage> :  null }
      </div>

      <div className='border-2 rounded-lg bg-gray-100'>
      <Heading title='Bids' subtitle=""/>
      </div>

      <div className='mt-3 grid grid-cols-1 rounded-lg'>
        <DetailedSpecs auction={data}></DetailedSpecs>
      </div>
    </div>
    </div>
    
  )
}
