
import React from 'react'
import { AiOutlineCar } from 'react-icons/ai'
import Search from './Search'
import Logo from './Logo'
import LoginButton from './LoginButton'
import { getCurrentUser } from '../actions/authActions'
import UserAction from './UserAction'

export default async function Navbar() {
  const user = await getCurrentUser();
  return (
    <header className='sticky top-0 z-50 flex justify-between bg-white p-5 items-center text-grey-800 shadow-md'>
       
        <Logo></Logo>
        <Search></Search>
        {user ? (
          <UserAction user={user}></UserAction>
        ) : (
          <LoginButton></LoginButton>
        )}
    </header>
  )
}
