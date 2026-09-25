import { useEffect, useState } from "react";
import { useUserProfileQuery, useUpdateUserProfileMutation, useGetMyOrdersQuery } from "../../features/account/accountApi";
import { TextField, Button, Box, Typography, CircularProgress, Card, CardContent, Stack, Grid2 } from "@mui/material";
import type { Order } from "../../app/models/order";

export default function ProfilePage() {
  const { data: profile, isLoading, isError, refetch } = useUserProfileQuery();
  const { data: orders = [] } = useGetMyOrdersQuery();
  const [updateProfile, { isLoading: isUpdating }] = useUpdateUserProfileMutation();

  const [form, setForm] = useState({
    name: "",
    email: "",
    address: {
      line1: "",
      city: "",
      postalCode: "",
      country: ""
    }
  });
  const [editMode, setEditMode] = useState(false);

  useEffect(() => {
    if (profile) {
      setForm({
        name: profile.name || profile.address?.name || "",
        email: profile.email ?? "",
        address: profile.address ?? {
          line1: "",
          city: "",
          postalCode: "",
          country: ""
        }
      });
    }
  }, [profile]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleAddressChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, address: { ...form.address, [e.target.name]: e.target.value } });
  };

  const handleSave = async () => {
    try {
      await updateProfile({
        name: form.name,
        address: form.address
      }).unwrap();

      setEditMode(false);
      refetch();
    } catch (error) {
      console.error(error);
    }
  };

  if (isLoading) return <CircularProgress sx={{ display: "block", mx: "auto", mt: 5 }} />;
  if (isError) return <Typography color="error" sx={{ mt: 5, textAlign: "center" }}>Failed to load profile.</Typography>;

  return (
    <Box maxWidth={1000} mx="auto" mt={3} px={2}>
      {/* Header */}
      <Box textAlign="center">
        <Typography variant="h3" fontWeight={600}>
          {form.name}
        </Typography>
        <Typography color="text.secondary">{form.email}</Typography>
      </Box>

      {/* Action Buttons */}
      <Box mb={3}>
        {editMode ? (
          <Button variant="contained" onClick={handleSave} disabled={isUpdating}>
            {isUpdating ? "Saving..." : "Save Changes"}
          </Button>
        ) : (
          <Button variant="outlined" color="secondary" onClick={() => setEditMode(true)}>
            Edit Profile
          </Button>
        )}
      </Box>

      <Grid2 container spacing={3}>
        {/* Personal Info */}
        <Grid2 size={{ xs: 12, md: 4 }}>
          <Card variant="outlined">
            <CardContent>
              <Typography variant="h6" gutterBottom>Personal Info</Typography>
              <TextField
                label="Name"
                name="name"
                value={form.name}
                onChange={handleChange}
                fullWidth
                margin="normal"
                disabled={!editMode}
              />
              <TextField
                label="Email"
                value={form.email}
                fullWidth
                margin="normal"
                disabled
              />
            </CardContent>
          </Card>
        </Grid2>

        {/* Shipping Address */}
        <Grid2 size={{ xs: 12, md: 4 }}>
          <Card variant="outlined">
            <CardContent>
              <Typography variant="h6" gutterBottom>Shipping Address</Typography>
              {["line1", "city", "postalCode", "country"].map((field) => (
                <TextField
                  key={field}
                  label={field.charAt(0).toUpperCase() + field.slice(1)}
                  name={field}
                  value={form.address[field as keyof typeof form.address]}
                  onChange={handleAddressChange}
                  fullWidth
                  margin="normal"
                  disabled={!editMode}
                />
              ))}
            </CardContent>
          </Card>
        </Grid2>

        {/* Recent Orders */}
        <Grid2 size={{ xs: 12, md: 4 }}>
          <Card variant="outlined">
            <CardContent>
              <Typography variant="h6" gutterBottom>Recent Orders</Typography>
              {orders.length === 0 ? (
                <Typography color="text.secondary">You have no orders yet.</Typography>
              ) : (
                <Stack spacing={1}>
                  {orders.slice(0, 3).map((order: Order) => (
                    <Box
                      key={order.id}
                      sx={{
                        border: 1,
                        borderColor: 'divider',
                        borderRadius: 2,
                        p: 1,
                        bgcolor: 'background.default'
                      }}
                    >
                      <Typography fontWeight={600} variant="body2">Order #{order.id}</Typography>
                      <Typography variant="caption" display="block" color="text.secondary">
                        Date: {new Date(order.orderDate).toLocaleDateString()}
                      </Typography>
                      <Typography variant="caption" display="block" color="text.secondary">
                        Total: €{(order.total / 100).toFixed(2)}
                      </Typography>
                    </Box>
                  ))}
                </Stack>
              )}
            </CardContent>
          </Card>
        </Grid2>
      </Grid2>
    </Box>
  );
}