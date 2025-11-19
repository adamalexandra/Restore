import { AppBar, IconButton, Toolbar, Typography } from "@mui/material";
import { DarkMode, LightMode } from "@mui/icons-material";

type props={
  toggleDarkMode: () => void;
  darkMode: boolean;
}
export default function Navbar({darkMode,toggleDarkMode}:props) {
  
  return (
   <AppBar position="fixed">
    <Toolbar>
      <Typography variant="h6">e-CANDLE</Typography>
      <IconButton onClick={toggleDarkMode}>
        {darkMode ? <DarkMode /> : <LightMode sx={{ color: '#f2e7e7' }} />}
      </IconButton>
    </Toolbar>
   </AppBar>
  )
}