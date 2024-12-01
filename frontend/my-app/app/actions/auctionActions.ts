'use server'

import { Auction, PageResult } from "@/types";
import { getTokenWorkaround } from "./authActions";
import { fetchWrapper } from "@/lib/fetchWrapper";
import { FieldValues } from "react-hook-form";
import { revalidatePath } from "next/cache";

export async function getData(urlparams:string): Promise<PageResult<Auction>> {
   return await fetchWrapper.get(`search/${urlparams}`);
}

export async function updateActionTest(){
    const data = {
        mileage: Math.floor(Math.random() * 100000) + 1

    }

    return await fetchWrapper.put('auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c', data);

    // const token = await getTokenWorkaround();

    // const res = await fetch('http://localhost:6001/auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c', {
    //     method: 'PUT',
    //     headers:{
    //         'Content-type': 'application/json',
    //         'Authorization': 'Bearer '+ token?.access_token
    //     },
    //     body: JSON.stringify(data)
    // })

    // if(!res.ok) return {status: res.status, message: res.statusText};

    // return res.statusText;
}

export async function createAuction(data: FieldValues){
    return await fetchWrapper.post('auctions', data);
}

export async function getDetailedViewData(id: string): Promise<Auction>{
    return await fetchWrapper.get(`auctions/${id}`);
}

export async function updateAuction(data: FieldValues, id: string){
    const res = await fetchWrapper.put(`auctions/${id}`, data);
    revalidatePath(`/auctions/${id}`);
    return res;
}

export async function deleteAuction(id: string) {
    return await fetchWrapper.del(`auctions/${id}`, {});
}
