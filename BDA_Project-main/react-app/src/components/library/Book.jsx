import "../../styles/components/Book.scss";
import React from "react";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import {
  toggleFavorite,
  selectFavorites,
} from "../../store/slices/favoritesSlice";
import { FaHeart, FaTimes } from "react-icons/fa";

const Book = ({ book, isFavoriteBook }) => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const favorites = useSelector(selectFavorites);

  const isFavorite = favorites.some((favBook) => favBook.id === book.id);

  const openBook = (bookItem) => {
    navigate(`/library/${bookItem.id}`);
  };

  const handleToggleFavorite = (e) => {
    e.stopPropagation();
    dispatch(toggleFavorite(book));
  };

  const handleRemoveFavorite = (book) => dispatch(toggleFavorite(book));

  return (
    <div className="book" onClick={() => openBook(book)}>
      <div className="book__img-container">
        <img src={book.image} alt={`${book.image}`} className="book__img" />
      </div>

      <div className="p-5 bg-white">
        <h2 className="book__title">{book.title}</h2>
        <p className="text-gray-700 mb-1">
          <span>Author:</span> {book.author}
        </p>
        <p className="text-gray-700 mb-3">
          <span>Genre:</span> {book.genre}
        </p>
        <p className="text-gray-600 text-sm line-clamp-3">{book.description}</p>

        {isFavoriteBook ? (
          <button
            className="book__favorite-btn"
            onClick={(e) => {
              e.stopPropagation();
              handleRemoveFavorite(book);
            }}
          >
            <FaTimes size={20} />
          </button>
        ) : (
          <button
            className={`book__cancel-favorite ${isFavorite ? "text-red-500" : "text-gray-500"
              }`}
            onClick={handleToggleFavorite}
          >
            <FaHeart size={20} />
          </button>
        )}
      </div>
    </div>
  );
};

export default Book;
