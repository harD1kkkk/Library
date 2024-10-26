import { createSlice } from "@reduxjs/toolkit";

const borrowBookSlice = createSlice({
  name: "borrowBook",
  initialState: {
    borrowedBooks: [],
  },
  reducers: {
    borrowBook: (state, action) => {
      if (!state.borrowedBooks.includes(action.payload)) {
        state.borrowedBooks.push(action.payload);
      }
    },
  },
});

export const { borrowBook } = borrowBookSlice.actions;
export const selectBorrowedBooks = (state) => state.borrowBook.borrowedBooks;
export default borrowBookSlice.reducer;
