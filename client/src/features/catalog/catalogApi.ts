import { createApi } from "@reduxjs/toolkit/query/react";
import type { Product } from "../../app/models/product";
import { baceQueryWithErrorHandling } from "../../app/api/baceApi";

export const catalogApi = createApi({
  reducerPath: 'catalogApi',
  baseQuery: baceQueryWithErrorHandling, 
  endpoints: (builder) => ({
    fetchProducts: builder.query<Product[], void>({
      query:()=> ({url: 'products'})

    }),
    fetchProductDetails: builder.query<Product, number>({
      query :(productId)=> `products/${productId}`
    })
  })
});

export const {useFetchProductDetailsQuery, useFetchProductsQuery} = catalogApi;