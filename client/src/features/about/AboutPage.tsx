import { Box, Typography } from "@mui/material";

export default function AboutPage() {
return (
  <Box maxWidth="xl" mx="auto" px={4} position="relative">
    <Box
        display="flex"
        flexDirection="column"
        alignItems="center"
        justifyContent="center"
        position="relative"
        minHeight="80vh"
      >
       <img
          src="/images-candles/images/about-background.jpg"
          alt="factory image"
          style={{
            position: "absolute",
            inset: 0,
            width: "100%",
            height: "100%",
            objectFit: "cover",
            borderRadius: "16px",
            zIndex: 0,
          }}
        />
        
         <Box
          display="flex"
          flexDirection="column"
          p={8}
          alignItems="center"
          position="relative"
          borderRadius={4}
          sx={{
            backgroundColor: "#42212295",
            color: "white",
            maxWidth: "900px",
            textAlign: "center",
          }}
        >
          <Typography
            variant="h2"
            fontWeight="bold"
            sx={{my:3, textShadow: '2px 2px 4px #a1756b' }}
          >
            About e-CANDLE
          </Typography>
          <Typography variant="body1" align="center" mt={2}>
            At e-CANDLE, we believe that the smallest moments can create the
            biggest feelings.
          </Typography>
          <Typography variant="body1" align="center" mt={2}>
            Our journey began with a simple idea: to create candles that feel
            warm, clean, and truly comforting. What started as a passion quickly
            turned into a mission to bring more calm and intention into everyday
            life.
          </Typography>
      <Typography variant="body1" align="center" mt={2}>
            Each candle is carefully handcrafted using high-quality ingredients,
            designed to burn cleanly and fill your space with scents that feel
            like home. We focus on simplicity, quality, and creating products you
            can enjoy without worry.
          </Typography>
           <Typography variant="body1" align="center" mt={2}>
            Our candles are made for slow evenings, quiet mornings, and all the
            little moments in between.
          </Typography>
          <Typography variant="body1" align="center" mt={2}>
            Thank you for supporting our small business — it truly means the
            world to us.
            <br />
            — Our Team
          </Typography>
        </Box>
      </Box>
    </Box>
  );
}