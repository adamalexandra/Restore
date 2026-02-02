import { Box, Button, Checkbox, FormControlLabel, Paper, Step, StepLabel, Stepper, Typography } from "@mui/material";
import { AddressElement, PaymentElement, useElements, useStripe } from "@stripe/react-stripe-js";
import {useState, useEffect} from "react"
import Review from "./Review";
import { useFetchAddressQuery, useUpdateUserAddressMutation } from "../account/accountApi";
import type { Address } from "../../app/models/user";
import { useBasket } from "../../lib/hooks/useBasket";
import { currencyFormat } from "../../lib/util";
import { toast } from "react-toastify";
import type { ConfirmationToken, StripeAddressElementChangeEvent, StripePaymentElementChangeEvent } from "@stripe/stripe-js";
import { useNavigate } from "react-router-dom";
import { useCreateOrderMutation } from "../orders/orderApi";


const steps =['Address', 'Payment', 'Review'];

export default function CheckoutStepper() {
  const [activeStep, setActiveStep] = useState(0);
  const [createOrder]=useCreateOrderMutation();
  const {basket} = useBasket();
  const {data, isLoading} = useFetchAddressQuery();
  const {name, ...restAddress} = (data || {}) as Address;
  const[updateAddress]=useUpdateUserAddressMutation();
  const[saveAddressChecked, setSaveAddressChecked]=useState(false);
  const elements = useElements();
  const stripe = useStripe();
  const [addressComplete, setAddressComplete] = useState(false);
  const [paymentComplete, setPaymentComplete] = useState(false);
  const [paymentReady, setPaymentReady] = useState(false);
  const [submitting, setSubmitting] = useState (false);
  const {subtotal, deliveryFee, clearBasket} = useBasket();
  const navigate = useNavigate();
  const total = subtotal + deliveryFee;
  const [confirmationToken, setConfirmationToken] = useState<ConfirmationToken | null>(null);

  // Mark payment element as ready when elements exist
  useEffect(() => {
    if (elements && activeStep === 1) {
      setPaymentReady(true);
    }
  }, [elements, activeStep]);

  const handleNext = async () => {
    if (activeStep === 0) {
      if (saveAddressChecked && elements) {
        const address = await getStripeAddress();
        if (address) await updateAddress(address);
      }
      setActiveStep(step => step + 1);
    }
    if (activeStep === 2) {
      await confirmPayment();
    }
    if (activeStep === 1) {
      if (!elements || !stripe) {
        toast.error('Payment form not ready. Please wait a moment.');
        return;
      }
      if (!paymentReady) {
        toast.error('Payment element is still loading. Please wait.');
        return;
      }
      try {
        // Add delay to ensure element is fully initialized
        await new Promise(resolve => setTimeout(resolve, 200));
        
        // Verify payment element is still ready before submitting
        const paymentElement = elements?.getElement('payment');
        if (!paymentElement) {
          toast.error('Payment element is not available. Please refresh and try again.');
          return;
        }
        
        const result = await elements.submit();
        if (result.error) {
          toast.error('Payment validation error: ' + result.error.message);
          return;
        }

        const stripeResult = await stripe.createConfirmationToken({elements});
        if (stripeResult.error) {
          toast.error('Confirmation token error: ' + stripeResult.error.message);
          return;
        }
        setConfirmationToken(stripeResult.confirmationToken);
        setActiveStep(step => step + 1); // Move to review step
      } catch (error) {
        console.error('Payment submission error:', error);
        if (error instanceof Error) {
          toast.error('Payment form error: ' + error.message);
        } else {
          toast.error('An unexpected error occurred with the payment form');
        }
        return;
      }
    }
  }

  const confirmPayment = async () => {
    setSubmitting(true);
    try{
      if(!confirmationToken || !basket?.clientSecret) 
        throw new Error('Unable to process payment');

      const orderModel=await createOrderModel();
      const orderResult=await createOrder(orderModel);

      const paymentResult = await stripe?.confirmPayment({
        clientSecret: basket.clientSecret,
        redirect: 'if_required',
        confirmParams: {
          confirmation_token: confirmationToken.id
        }
      });

      if (paymentResult?.paymentIntent?.status === 'succeeded') {
        navigate('/checkout/success', {state: orderResult});
        clearBasket();
      } else if (paymentResult?.error) {
        throw new Error(paymentResult.error.message);
      } else {
        throw new Error ('Something went wrong');
      }
    } catch (error){
      if(error instanceof Error) {
        toast.error (error.message)
      }
      setActiveStep(step => step - 1);
    }finally {
      setSubmitting(false)
    }
  }

  const createOrderModel=async () =>{
    const shippingAddress=await getStripeAddress();
    const card = confirmationToken?.payment_method_preview.card;

    if (!shippingAddress || !card) throw new Error('Problem creating order');

    const paymentSummary = {
      last4: card.last4,
      brand: card.brand,
      exp_month: card.exp_month,
      exp_year: card.exp_year
    };

    return {shippingAddress, paymentSummary}
  }

  const getStripeAddress = async() => {
    const addressElement = elements?.getElement('address');
    if(!addressElement) return null;
    const {value: {name,address}} = await addressElement.getValue();
  
    if (name && address) return {...address,name}

    return null;
  }

  const handleBack = () => {
    setActiveStep(step => step -1);
  }

  const handleAddressChange = (event: StripeAddressElementChangeEvent) => {
    setAddressComplete(event.complete)
  }

  const handlePaymentChange = (event: StripePaymentElementChangeEvent) => {
    setPaymentComplete(event.complete);
    setPaymentReady(true); // Element is ready once we get an event
  }
  
  if (isLoading) return <Typography variant="h6">Loading checkout...</Typography>
  return (
    <Paper sx={{p: 3, borderRadious:3}}>
      <Stepper activeStep={activeStep}>
        {steps.map((label, index)=>{
          return(
            <Step key={index}>
              <StepLabel>{label}</StepLabel>
            </Step>
          )
        })}
      </Stepper>

      <Box sx={{mt: 2}}>
        <Box sx={{display: activeStep=== 0 ?'block' : 'none'}}>
          <AddressElement
            options={{
              mode:'shipping',
              defaultValues:{
                name: name,
                address: restAddress
              }
            }}
            onChange = {handleAddressChange}
          />
          <FormControlLabel
            sx={{display: 'flex', justifyContent:'end'}}
            control={<Checkbox 
              checked={saveAddressChecked}
              onChange={e=> setSaveAddressChecked(e.target.checked)}
            />}
            label='Save as default address'
          />
        </Box>
        <Box sx={{display: activeStep=== 1 ?'block' : 'none'}}>
          <PaymentElement onChange={handlePaymentChange}/>
        </Box>
        <Box sx={{display: activeStep=== 2 ?'block' : 'none'}}>
          <Review confirmationToken={confirmationToken}/>
        </Box>
      </Box>

      <Box display='flex' paddingTop={2} justifyContent='space-between'>
        <Button onClick={handleBack}>Back</Button>
        <Button
          onClick={handleNext}
          variant="contained"
          disabled={
            (activeStep === 0 && !addressComplete) ||
            (activeStep === 1 && (!paymentComplete || !paymentReady)) ||
            submitting
          }
        >
          {submitting ? 'Processing...' : (activeStep === steps.length - 1 ? `Pay ${currencyFormat(total)}`: 'Next')}
        </Button>
      </Box>
    </Paper>
  )
}