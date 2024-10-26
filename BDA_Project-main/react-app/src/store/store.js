import { configureStore } from "@reduxjs/toolkit";
import bookReducer from "./slices/bookSlice";
import feedbackReducer from "./slices/feedbackSlice";
import favoritesReducer from "./slices/favoritesSlice";
import signupReducer from "./slices/signupSlice";
import signinReducer from "./slices/signinSlice";
import borrowBookReducer from "./slices/borrowBookSlice";
import userReducer from "./slices/userSlice";

const store = configureStore({
  reducer: {
    books: bookReducer,
    feedback: feedbackReducer,
    favorites: favoritesReducer,
    signup: signupReducer,
    signin: signinReducer,
    borrowBook: borrowBookReducer,
    user: userReducer,
  },
});

export default store;
