import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";

const initialState = {
  loading: false,
  error: null,
  success: null,
};

export const signUpUser = createAsyncThunk(
  "signup/signUpUser",
  async (formData, { rejectWithValue }) => {
    try {
      const response = await fetch("https:/146.190.176.10/api/users/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(formData),
      });
      const data = await response.json();
      if (!response.ok) {
        throw new Error(data.message || "Registration failed.");
      }
      return data;
    } catch (error) {
      return rejectWithValue(error.message);
    }
  }
);

const signupSlice = createSlice({
  name: "signup",
  initialState,
  reducers: {
    resetSignUp(state) {
      state.loading = false;
      state.error = null;
      state.success = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(signUpUser.pending, (state) => {
        state.loading = true;
        state.error = null;
        state.success = null;
      })
      .addCase(signUpUser.fulfilled, (state) => {
        state.loading = false;
        state.success = "Registration successful! Redirecting to login page...";
      })
      .addCase(signUpUser.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      });
  },
});

export const { resetSignUp } = signupSlice.actions;
export default signupSlice.reducer;
