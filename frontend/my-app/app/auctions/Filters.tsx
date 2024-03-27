import { Button, ButtonGroup } from 'flowbite-react';
import React from 'react'

const pageSizeButtons = [4, 8, 12];
type Props = {
    pageSize: number;
    setPageSize: (size:number) => void
}
export default function Filters({pageSize, setPageSize}: Props) {
  return (
    <div className='flex justify-between items-center mb-4'>
        <div>
            <span className='uppercase text-sm text-green-500 mr-2'>Page Size</span>
            <ButtonGroup>
                {pageSizeButtons.map( (value, i) => (
                    <Button key={i}
                     onClick={() => setPageSize(value)}
                     color={`${pageSize === value ? 'red': 'grey'}`}
                     className='focus:ring-0'
                    >
                        {value}

                    </Button>
                ))}
            </ButtonGroup>
        </div>
    </div>
  )
}
