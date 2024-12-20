'use client'

import { Button, TextInput } from 'flowbite-react';
import React, { useEffect } from 'react'
import { FieldValue, FieldValues, useForm } from 'react-hook-form';
import Input from '../components/Input';
import DateInput from '../components/DateInput';
import { createAuction, updateAuction } from '../actions/auctionActions';
import {usePathname, useRouter} from 'next/navigation';
import toast from 'react-hot-toast';

type Props = {
    auction?: Auction
}

export default function AuctionForm({auction}:Props) {
    const router = useRouter();
    const pathname = usePathname();

    const {control, handleSubmit, setFocus, reset, 
        formState: {isSubmitting, isDirty, isValid, errors}} = useForm({
            mode: 'onTouched'
        });

    

    async function onSubmit(data: FieldValues){
        try{
            let id = '';
            let res;
            if(pathname === '/auctions/create'){
                res = await createAuction(data);
                id = res.id;
            }else{
                if(auction){
                    res = await updateAuction(data, auction.id);
                    id = auction.id;
                }
            }

            
            console.log(res);
            if(res.error){
                throw res.error;
            }
            router.push(`/auctions/details/${id}`);
        }catch(error: any){
            console.log(error);
            toast.error(error.status+ ' '+error.message);
        }
    }

    useEffect(() => {
        if(auction){
            const {make, model, color, mileage, year} = auction;
            reset({make, model, color, mileage, year});
        }
        setFocus('make');
    }, [setFocus])

  return (
    <form action="" className='flex flex-col mt-3' onSubmit={handleSubmit(onSubmit)}>
        <Input label='Make' name='make' control={control} rules={{required: 'Make is required'}}></Input>
        <Input label='Model' name='model' control={control} rules={{required: 'Model is required'}}></Input>
        <Input label='Color' name='color' control={control} rules={{required: 'Color is required'}}></Input>
       
       <div className='grid grid-cols-2 gap-3'>
        <Input label='Year' name='year' type='number' control={control} 
            rules={{required: 'Year is required'}}></Input>
        <Input label='Mileage' name='mileage' control={control} type='number'
            rules={{required: 'Mileage is required'}}></Input>
       </div>

        {pathname==='/auctions/create' && 
        <>
            <Input label='Image URL' name='imageUrl' control={control} rules={{required: 'Image url is required'}}></Input>

            <div className='grid grid-cols-2 gap-3'>
                <Input label='Reserved prise (enter 0 if no reserve)' name='reservePrice' type='number' control={control} 
                    rules={{required: 'Reserve price is required'}}></Input>
                <DateInput 
                    label='Auction end date/time' 
                    name='auctionEnd' 
                    control={control} 
                    dateFormat='dd MMMM yyyy h:mm a' 
                    showTimeSelect
                    rules={{required: 'Auction ends date is required'}}></DateInput>
            </div>
        </>}


        <div className='flex justify-between'>
            <Button outline color='grey'>Cancel</Button>
            <Button outline color='success'
                disabled={!isValid}
                type='submit'
             isProcessing={isSubmitting}>Submit</Button>
        </div>
    </form>
  )
}
