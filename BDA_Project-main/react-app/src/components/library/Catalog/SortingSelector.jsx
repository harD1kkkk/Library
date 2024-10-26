import { useState, useRef, useEffect } from "react";
import { IoIosArrowForward, IoIosArrowDown } from "react-icons/io";

const SortingSelector = ({ sortingNames, currentSort, setCurrentSort }) => {
  const [isOpen, setIsOpen] = useState(false);
  const selectorRef = useRef();

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (selectorRef.current && !selectorRef.current.contains(event.target)) {
        setIsOpen(false);
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, []);

  const handleSelectSort = (sortingName) => {
    setCurrentSort(sortingName);
    setIsOpen(false);
  };

  return (
    <div className="catalog__sort" ref={selectorRef}>
      <button className="catalog__sort-btn" onClick={() => setIsOpen(!isOpen)}>
        {currentSort || "Sort by"}
        <span className="catalog__sort-icon">
          {isOpen ? <IoIosArrowDown /> : <IoIosArrowForward />}
        </span>
      </button>

      {isOpen && (
        <div className="catalog__open-sort">
          {sortingNames.map((sortingName) => (
            <div
              key={sortingName}
              className={`catalog__open-sort-item ${
                currentSort === sortingName &&
                "bg-[#3b82f6] text-white rounded-b-lg"
              }`}
              onClick={() => handleSelectSort(sortingName)}
            >
              {sortingName}
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default SortingSelector;
