import { Box, Typography } from "@mui/material";
import {AccessTime, EmailOutlined, PhoneOutlined, PlaceOutlined} from '@mui/icons-material';

export default function ContactPage() {
  return (
    <Box maxWidth="xl" mx="auto" px={4} position="relative">
      <Box
        display="flex"
        flexDirection="column"
        alignItems="center"
        justifyContent="center"
        position="relative"
        minHeight="85vh"
      >
        <img
          src="/images-candles/images/contact-background.jpg"
          alt="candles image"
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
        {/* Content */}
        <Box
          position="relative"
          zIndex={2}
          p={8}
          borderRadius={4}
          sx={{
            color: "white",
            textAlign: "center",
            maxWidth: "800px",
            backgroundColor: "#645455b2",
          }}
        >
          <Typography
          color = "primary"
            variant="h2"
            fontWeight="bold"
            sx={{ mb: 4, textShadow: "2px 2px 7px #564747c6" }}
          >
            Get in Touch
          </Typography>

          <Typography variant="body1" >
            We love hearing from our customers. Whether you have a question
            about our candles, a special request, or need help with an order,
            feel free to reach out.
          </Typography>

          <Typography variant="h5" sx={{ mt: 4, fontWeight: "bold" }}>
            <PlaceOutlined /> Address
          </Typography>
          <Typography variant="body1" >
            123 Street, Building 4<br />
            Athens, Greece
          </Typography>

          <Typography variant="h5" sx={{ mt: 3, fontWeight: "bold" }}>
            <PhoneOutlined /> Phone
          </Typography>
          <Typography variant="body1" >
            +30 234 5678901
          </Typography>

          <Typography variant="h5" sx={{ mt: 3, fontWeight: "bold" }}>
            <EmailOutlined /> Email
          </Typography>
          <Typography variant="body1" >
             support@e-candle.com
          </Typography>

          <Typography variant="h5" sx={{ mt: 3, fontWeight: "bold" }}>
            <AccessTime  /> Working Hours
          </Typography>
          <Typography variant="body1">
            Monday - Friday: 9:00 AM – 6:00 PM<br />
            Saturday: 10:00 AM – 4:00 PM
          </Typography>
        </Box>
      </Box>
    </Box>
  );
}