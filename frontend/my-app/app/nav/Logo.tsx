'use client'

import React from 'react'
import { AiOutlineCar } from 'react-icons/ai'
import { useParamsStore } from '../hooks/useParamsStore'
import { useRouter } from 'next/navigation'
import { usePathname } from 'next/navigation'

export default function Logo() {
    const router = useRouter();
    const pathname = usePathname();
    
    const reset = useParamsStore(state => state.reset);


    function doReset(){
      if(pathname !== '/'){
        router.push('/');
      }
    }

  return (
    <div onClick={reset} className='cursor-pointer flex items-center gap-2 text-2xl font-semibold text-red-500'>
            <AiOutlineCar size={34}></AiOutlineCar>
            <div>Cars Auction</div>
        </div>
  )
}
