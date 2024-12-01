import { Label, TextInput } from 'flowbite-react'
import React, { useState } from 'react'
import { UseControllerProps, useController } from 'react-hook-form'
import 'react-datepicker/dist/react-datepicker.css'
import DatePicker, { ReactDatePickerProps } from 'react-datepicker'

type Props = {
    label: string
    type?: string
    showLabel?: boolean
} & UseControllerProps & Partial<ReactDatePickerProps>

export default function DateInput(props: Props) {
    const {fieldState, field} = useController({...props, defaultValue: ''})
    const [startDate, setStartDate] = useState(new Date());

  return (
    <div className='block'>
       <DatePicker
            {...props} 
            {...field}
            placeholderText={props.label}
            selected={field.value} onChange={(date) => field.onChange(date)}
            className={`
                rounded-lg w-[100%] flex flex-col
                ${fieldState.error ? 'bg-red-50 border-red-500 text-red-900' 
                : (!fieldState.invalid && fieldState.isDirty) 
                ? 'bg-green-50 border-green-500 text-green-900':''}
            `}
      ></DatePicker>
       
       {fieldState.error && (
        <div className='text-red-500 text-sm'>{fieldState.error.message}</div>
       )}
                
    </div>
  )
}

