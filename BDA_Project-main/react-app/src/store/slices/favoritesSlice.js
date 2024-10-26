import { createSlice } from "@reduxjs/toolkit";

const favoritesSlice = createSlice({
  name: "favorites",
  initialState: {
    favorites: JSON.parse(localStorage.getItem("favorites")) || [],
  },
  reducers: {
    toggleFavorite: (state, action) => {
      const bookId = action.payload.id;
      const existingIndex = state.favorites.findIndex(
        (book) => book.id === bookId
      );

      if (existingIndex >= 0) state.favorites.splice(existingIndex, 1);
      else state.favorites.push(action.payload);

      localStorage.setItem("favorites", JSON.stringify(state.favorites));
    },
  },
});

export const { toggleFavorite } = favoritesSlice.actions;
export const selectFavorites = (state) => state.favorites.favorites;
export default favoritesSlice.reducer;
