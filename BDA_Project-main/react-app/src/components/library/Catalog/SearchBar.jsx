import React, { useEffect, useState } from "react";
import { useStateContext } from "../../../contexts/ContextProvider";
import { FaArrowRightLong } from "react-icons/fa6";

function SearchBar() {
  const [inputValue, setInputValue] = useState("");
  const { setSearchTerm } = useStateContext();
  const [isScrolling, setIsScrolling] = useState(false);

  useEffect(() => {
    const handleScroll = () => {
      setIsScrolling(true);
    };

    const handleScrollEnd = () => {
      setIsScrolling(false);
    };

    window.addEventListener("scroll", handleScroll);

    let scrollTimer;
    window.addEventListener("scroll", () => {
      clearTimeout(scrollTimer);
      scrollTimer = setTimeout(handleScrollEnd, 100);
    });

    return () => {
      window.removeEventListener("scroll", handleScroll);
      clearTimeout(scrollTimer);
    };
  }, []);

  useEffect(() => {
    if (!isScrolling) {
      const timer = setTimeout(() => {
        setSearchTerm("");
        setInputValue("");
      }, 5000);

      return () => clearTimeout(timer);
    }
  }, [inputValue, isScrolling, setSearchTerm]);

  const handleSearch = () => {
    setSearchTerm(inputValue);
    setInputValue("");
  };

  const handleKeyDown = (e) => {
    if (e.key === "Enter") {
      handleSearch();
    }
  };

  const handleChange = (e) => {
    setInputValue(e.target.value);
    setSearchTerm(e.target.value);
  };

  return (
    <div className="catalog__search-bar">
      <input
        type="text"
        placeholder="Search..."
        onChange={handleChange}
        className="catalog__search-input"
        onKeyDown={handleKeyDown}
        value={inputValue}
      />
      <button onClick={handleSearch} className="catalog__search-btn-enter">
        <FaArrowRightLong />
      </button>
    </div>
  );
}

export default SearchBar;
