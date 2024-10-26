import React, { useEffect, useState } from "react";
import CatalogSettings from "../components/library/Catalog/Catalog.jsx";
import Book from "../components/library/Book.jsx";
import { useDispatch, useSelector } from "react-redux";
import { fetchBooksItems, selectAllBooks } from "../store/slices/bookSlice.js";
import { useStateContext } from "../contexts/ContextProvider.js";

const sortingMap = {
  Title: (a, b) => a.title.localeCompare(b.title),
  Author: (a, b) => a.author.localeCompare(b.author),
  Genre: (a, b) => a.genre.localeCompare(b.genre),
};

const Library = () => {
  const dispatch = useDispatch();
  const books = useSelector(selectAllBooks);
  const bookStatus = useSelector((state) => state.books.status);
  const error = useSelector((state) => state.books.error);

  useEffect(() => {
    if (bookStatus === "idle") {
      dispatch(fetchBooksItems());
    }
  }, [bookStatus, dispatch]);

  const [currentSort, setCurrentSort] = useState("Title");
  const { searchTerm, setSearchTerm } = useStateContext();

  const filteredAndSortedBooks = () => {
    return books
      .filter((book) => {
        return (
          book.title.toLowerCase().includes(searchTerm) ||
          book.author.toLowerCase().includes(searchTerm) ||
          book.genre.toLowerCase().includes(searchTerm)
        );
      })
      .sort(sortingMap[currentSort]);
  };

  const renderBooks = () => {
    return (
      <div className="book-list">
        {filteredAndSortedBooks().map((book) => (
          <Book book={book} isFavoriteBook={false} />
        ))}
      </div>
    );
  };

  if (bookStatus === "loading")
    return <div className="text-lg text-center mt-5">Loading...</div>;

  if (bookStatus === "failed")
    return (
      <div className="w-full text-center text-red-600 text-lg mt-10">
        Error: {error}
      </div>
    );

  return (
    <>
      <div className="container mx-auto py-8 sm:px-0 px-5">
        <CatalogSettings
          sortingMap={sortingMap}
          setCurrentSort={setCurrentSort}
          setSearchTerm={setSearchTerm}
          currentSort={currentSort}
        />
        {renderBooks()}
      </div>
    </>
  );
};

export default Library;
