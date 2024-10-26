import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";

const initialState = {
  loading: false,
  error: null,
  success: null,
};

export const signInUser = createAsyncThunk(
  "signin/signInUser",
  async (formData, { rejectWithValue }) => {
    try {
      const response = await fetch("https:/146.190.176.10/api/users/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(formData),
      });
      const data = await response.json();
      if (!response.ok) {
        throw new Error(data.message || "Login failed.");
      }
      return data;
    } catch (error) {
      return rejectWithValue(error.message);
    }
  }
);

const signinSlice = createSlice({
  name: "signin",
  initialState,
  reducers: {
    resetSignIn(state) {
      state.loading = false;
      state.error = null;
      state.success = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(signInUser.pending, (state) => {
        state.loading = true;
        state.error = null;
        state.success = null;
      })
      .addCase(signInUser.fulfilled, (state) => {
        state.loading = false;
        state.success = "Login successful!";
      })
      .addCase(signInUser.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      });
  },
});

export const { resetSignIn } = signinSlice.actions;
export default signinSlice.reducer;
