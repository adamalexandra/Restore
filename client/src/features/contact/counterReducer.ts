import { createSlice } from "@reduxjs/toolkit"

// Define a type for the slice state
export type CounterState = {
  data: number
}

// Define the initial state using that type
const initialState: CounterState = {
  data: 42
}

// Redux Toolkit slice
export const counterSlise=createSlice({
  name:'counter',
  initialState,
  reducers:{
    increment:(state,action)=>{
      state.data +=action.payload //mutating state 
    },
    decrement:(state,action)=>{
      state.data -= action.payload //mutating state
    }
  }
})
// Export actions
export const{increment,decrement}=counterSlise.actions;


//
export function incrementLegacy(amount=1){
  return{
    type:'increment',
    payload: amount
  }
}

export function decrementLegacy(amount=1){
  return{
    type:'decrement',
    payload: amount
  }
}

//
export default function counterReducer(state = initialState, action:{type:string, payload:number}) {
  switch (action.type) {
    case 'increment':
      return{
        ...state,
        data: state.data +  action.payload
      }
      
  
    case 'decrement':
      return {
        ...state,
        data: state.data - action.payload
      }
      default:
        return state;

  }
}