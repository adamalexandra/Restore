import { Box, Container, CssBaseline, ThemeProvider } from "@mui/material";
import {createTheme} from "@mui/material/styles";
import Navbar from "./navbar";
import { Outlet, ScrollRestoration } from "react-router-dom";
import { useAppSelector } from "../store/store";

function App() {
  const {darkMode}= useAppSelector(state => state.ui);

  const lightTheme = createTheme({
    typography:{
      fontFamily: "'Manrope', sans-serif",
    },
    palette: {
      mode : 'light',
      primary:{
        main: '#c4948a',
        light: '#d7ada5',
        dark:'#a1756b',
        contrastText: '#ffffff',
      },
      secondary:{
        main: '#814043',
      },
      error:{
        main:'#a11132'
      },
      success:{
        main:'#1c7b73ff',
      },
      background:{
        default:'#fff4ef',
        paper: '#ffffff'
      },
      text:{
        primary:'#140f0e',
        secondary:'#643c34'

      },
    },
  });

  const darkTheme = createTheme({
    typography:{
      fontFamily: "'Manrope', sans-serif",
    },

    palette: {
      mode : 'dark',
      primary:{
        main: '#c4948a',
        light: '#d7ada5',
        dark:'#a1756b',
        contrastText: '#000000',
      },
      secondary:{
        main: '#d4bfbf', 
      },
       error:{
        main:'#a11132'
      },
      success:{
        main:'#1c7b73ff'
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
  


  return (
<ThemeProvider theme={darkMode ? darkTheme : lightTheme}>
  <ScrollRestoration />
  <CssBaseline/>
  <Navbar/>
  <Box
  sx={{
    minHeight: '100vh',
    background: darkMode? '#765953':'#fff4ef' ,
    py:6
  }}
  >
  
  
    <Container maxWidth='xl' sx=  {{mt: 8}}>
      <Outlet/>
    </Container>
  </Box>

</ThemeProvider>

  )
}

export default App;
