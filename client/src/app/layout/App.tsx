import { useEffect, useState } from "react";
import type { Product } from "../models/product";
import Catalog from "../../features/catalog/Catalog";
import { Box, Container, createTheme, CssBaseline, ThemeProvider } from "@mui/material";
import Navbar from "./navbar";

function App() {
  const [products, setProducts] = useState<Product[]>([]);
  const [darkMode,setDarkMode]= useState(false);

  const lightTheme = createTheme({
    palette: {
      mode : 'light',
      primary:{
        main: '#c4948a ',
        light: '#d7ada5',
        dark:'#a1756b',
        contrastText: '#ffffff',
      },
      secondary:{
        main: '#814043', 
      },
      background:{
        default:'#fff4ef',
        paper: '#ffffff'
      },
      text:{
        primary:'#140f0e',
        secondary:'#3b2c29'
      },
    },
  });
  

  const darkTheme = createTheme({
    palette: {
      mode : 'dark',
      primary:{
        main: '#c4948a ',
        light: '#d7ada5',
        dark:'#a1756b',
        contrastText: '#000000',
      },
      secondary:{
        main: '#d4bfbf', 
      },
      background:{
        default:'#1a1414',
        paper:'#241c1c',
      },
      text:{
        primary:'#f2e7e7',
        secondary:'#c5b1b1',
      },
    },
  });
  
      
const toggleDarkMode = () => {
  setDarkMode(!darkMode);
};

  useEffect(() => {
    fetch("https://localhost:5001/api/products") //get database from api
      .then((response) => response.json())
      .then((data) => setProducts(data))}, []);

  return (
<ThemeProvider theme={darkMode ? darkTheme : lightTheme}>
  <CssBaseline/>
  <Navbar toggleDarkMode={toggleDarkMode} darkMode={darkMode}/>
  <Box
  sx={{
    minHeight: '100vh',
    background: darkMode? '#765953':'#fff4ef' ,
    py:6
  }}
  >
  
  
    <Container maxWidth='xl' sx=  {{mt: 8}}>
      <Catalog products={products} />
    </Container>
  </Box>

</ThemeProvider>

  )
}

export default App;
