'use client'

import React, { useState } from 'react'
import { FaSearch } from 'react-icons/fa'
import { useParamsStore } from '../hooks/useParamsStore'
import { TbSettingsSearch } from 'react-icons/tb';
import { useRouter } from 'next/navigation';
import { usePathname } from 'next/navigation';

export default function Search() {
    const router = useRouter();
    const pathname = usePathname();
    const setParams = useParamsStore( state => state.setParams);
    const setSearchValue = useParamsStore(state => state.setSearchValue);
    const searchValue = useParamsStore(state => state.searchValue);

    function onChange(event: any){
        setSearchValue(event.target.value);
    }

    function search(){
        if(pathname!=='/') router.push('/');
        setParams({searchTerm: searchValue});
    }



  return (
    <div className='flex w-[50%] items-center border-2 rounded-full py-2 shadow-sm'>
        <input type="text" 
        onKeyDown={(e:any) => {
            if(e.key == 'Enter') search();
        }}
        onChange={onChange}
        placeholder='Search'
        value={searchValue}
        className='
            flex-grow
            pl-5
            bg-transparent
            focus:outline-none
            border-transparent
            focus:border-transparent
            focus:ring-0
            text-sm
            text-grey-600
        '/>

        <button onClick={search}>
            <FaSearch size={34} className='bg-red-400 text-white rounded-full p-2 cursor-pointer mx-2'></FaSearch>
        </button>
    </div>
  )
}
