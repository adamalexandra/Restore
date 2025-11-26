import { useParams } from "react-router-dom";
import Grid from "@mui/material/Grid2";
import { Button, Divider, Table, TableBody, TableCell, TableContainer, TableRow, TextField, Typography } from "@mui/material";
import { useFetchProductDetailsQuery } from "./catalogApi";

export default function ProductDetail() {
  const {id} = useParams();
 
  const {data: product, isLoading} = useFetchProductDetailsQuery(id? +id: 0) //convert id(string) into number

  if (!product || isLoading) return <div>Loading...</div>

  const productDetails =[
    {label: 'Name', value: product.name},
    {label: 'Description', value: product.description},
    {label: 'Price', value: `€${product.price.toFixed(2)}`},
    {label: 'Brand', value: product.brand},
    {label: 'Type', value: product.type},
    {label: 'Quantity in Stock', value: product.quantityInStock},
  ]

  return (
    <Grid container spacing ={6} maxWidth="lg" sx={{marginTop: 'auto'}}>
      <Grid size={6}>
        <img src={product?.pictureUrl} alt={product.name} style={{width: '100%'}}/>
      </Grid>

      <Grid size={6}>
        <Typography variant="h3">{product?.name}</Typography>
        <Divider sx={{mb:2}}/>
        <Typography variant="h4" color='secondary'>€{(product.price).toFixed(2)}</Typography>
        <TableContainer>
          <Table sx={{
            '& td':{fontSize:'1rem'}
          }}>
            <TableBody>
                {productDetails.map((detail,index) => (
              <TableRow key={index}>
                <TableCell sx={{fontWeight:'bold'}}>{detail.label}</TableCell>
                <TableCell>{detail.value}</TableCell>
              </TableRow>
                ))}


            </TableBody>
          </Table>
        </TableContainer>
        <Grid container spacing={2} marginTop={3}>
          <Grid size={6}>
            <TextField
              variant='outlined'
              type="number"
              label="Quantity in Cart"
              fullWidth
              defaultValue={1}
              />
              </Grid>
          <Grid size={6}>
            <Button
            
            sx ={{height:'55px', fontWeight: 650}}
            color="primary" 
            size="large" 
            variant="contained" 
            fullWidth>
              Add to Cart
            </Button>
          </Grid>
        </Grid>
      </Grid>
    </Grid>
  )
}