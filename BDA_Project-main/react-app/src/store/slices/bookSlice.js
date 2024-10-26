import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";

export const fetchBooksItems = createAsyncThunk(
  "books/fetchBooks",
  async () => {
    const response = await fetch("https://146.190.176.10/api/books/allBooks");

    if (!response.ok || response === undefined || response === null)
      throw new Error("Failed to fetch books");
    const data = await response.json();
    return data;
  }
);

export const addBook = createAsyncThunk("books/addBook", async (newBook) => {
  const response = await fetch("https://146.190.176.10/api/books/addBook", {
    method: "POST",
    body: newBook,
  });

  if (!response.ok) {
    throw new Error("Failed to add book: " + response.statusText);
  }
  return await response.json();
});

export const addFeedback = createAsyncThunk(
  "books/addFeedback",
  async (feedbackData) => {
    const response = await fetch("/api/feedback", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(feedbackData),
    });

    if (!response.ok) throw new Error("Failed to add feedback");

    return await response.json();
  }
);

const initialState = {
  books: [],
  status: "idle",
  error: null,
};

const bookSlice = createSlice({
  name: "books",
  initialState,
  reducers: {
    deleteBook(state, action) {
      state.books = state.books.filter((book) => book.id !== action.payload);
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchBooksItems.pending, (state) => {
        state.status = "loading";
      })
      .addCase(fetchBooksItems.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.books = action.payload;
      })
      .addCase(fetchBooksItems.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message;
      })
      .addCase(addBook.fulfilled, (state, action) => {
        state.books.push(action.payload);
      })
      .addCase(addBook.rejected, (state, action) => {
        state.error = action.error.message;
      });
  },
});

export const selectAllBooks = (state) => state.books.books;

export const { deleteBook } = bookSlice.actions;
export default bookSlice.reducer;
