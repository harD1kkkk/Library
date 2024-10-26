import React from "react";
import { useSelector } from "react-redux";
import { selectFavorites } from "../store/slices/favoritesSlice";
import Book from "../components/library/Book";

const Favorites = () => {
  const favorites = useSelector(selectFavorites);

  return (
    <div className="container mx-auto py-8">
      {favorites.length === 0 ? (
        <p className="text-lg font-bold text-center text-red-700">
          No favorites added yet.
        </p>
      ) : (
        <div className="favorite-list">
          {favorites.map((book) => (
            <Book key={book.id} book={book} isFavoriteBook={true} />
          ))}
        </div>
      )}
    </div>
  );
};

export default Favorites;
