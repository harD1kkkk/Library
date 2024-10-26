import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { feedbacks } from "../../data/dummy";

export const fetchFeedbacks = createAsyncThunk(
  "feedback/fetchFeedbacks",
  async (bookId) => {
    const filteredFeedbacks = feedbacks.filter(
      (feedback) => Number(feedback.bookId) === Number(bookId)
    );
    console.log(filteredFeedbacks);

    return filteredFeedbacks;
  }
);

export const submitFeedback = createAsyncThunk(
  "feedback/submitFeedback",
  async ({ bookId, feedback }, { rejectWithValue }) => {
    try {
      const response = await fetch(
        "https:/146.190.176.10/api/feedbacks/setFeedback",
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(bookId, feedback),
        }
      );
      const data = await response.json();
      if (!response.ok) {
        throw new Error(data.message || "Sending failed.");
      }
      return data;
    } catch (error) {
      return rejectWithValue(error.message);
    }
  }
);

const feedbackSlice = createSlice({
  name: "feedback",
  initialState: {
    feedbacks: [],
    status: "idle",
    error: null,
  },
  reducers: {},
  extraReducers(builder) {
    builder
      .addCase(fetchFeedbacks.pending, (state) => {
        state.status = "loading";
      })
      .addCase(fetchFeedbacks.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.feedbacks = action.payload;
      })
      .addCase(fetchFeedbacks.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message;
      })
      .addCase(submitFeedback.fulfilled, (state, action) => {
        state.feedbacks.push(action.payload);
      });
  },
});

export const selectFeedbackByBook = (state, bookId) =>
  state.feedback.feedbacks.filter((f) => f.bookId === bookId);

export default feedbackSlice.reducer;
