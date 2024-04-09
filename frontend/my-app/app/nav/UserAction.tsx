'use client'

import { Button } from 'flowbite-react/lib/esm/components/Button'
import { Dropdown, DropdownDivider } from 'flowbite-react/lib/esm/components/Dropdown'
import { User } from 'next-auth'
import { signOut } from 'next-auth/react'
import Link from 'next/link'
import React from 'react'
import { AiFillCar, AiFillTrophy, AiOutlineLogout } from 'react-icons/ai'
import { HiCog, HiUser } from 'react-icons/hi'

type Props = {
    user: Partial<User>
}

export default function UserAction({user}: Props) {
  return (
    <Dropdown label={`Welcome ${user.name}`}>
      <Dropdown.Item icon={HiUser}><Link href='/'>
        My Auctions
        </Link></Dropdown.Item>

        <Dropdown.Item icon={AiFillTrophy}><Link href='/'>
        Auctions won
        </Link></Dropdown.Item>

        <Dropdown.Item icon={AiFillCar}><Link href='/'>
        Sell my car
        </Link></Dropdown.Item>

        <Dropdown.Item icon={HiCog}><Link href='/session'>
        Session (dev only)
        </Link></Dropdown.Item>
        <DropdownDivider></DropdownDivider>

      <Dropdown.Item icon={AiOutlineLogout} onClick={() => signOut({callbackUrl: '/'})}>
        Sign out
      </Dropdown.Item>
    </Dropdown>
  )
}
