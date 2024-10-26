import "../../../styles/components/Catalog.scss";
import AddBook from "./AddBook";
import SearchBar from "./SearchBar";
import SortingSelector from "./SortingSelector";
import { useMediaQuery } from "react-responsive";

const Catalog = ({ sortingMap, setCurrentSort, currentSort }) => {
  const sortingNames = Object.keys(sortingMap);
  const isLargerThan514px = useMediaQuery({ query: "(min-width: 514px)" });

  return (
    <div className="catalog">
      {isLargerThan514px ? (
        <>
          {/* Add new Book form */}
          <div>
            <AddBook />
          </div>

          {/* Search bar and Sorting */}
          <div className="flex items-center gap-4">
            {/* Search bar */}
            <SearchBar />

            {/* Books sorting */}
            <SortingSelector
              sortingNames={sortingNames}
              currentSort={currentSort}
              setCurrentSort={setCurrentSort}
            />
          </div>
        </>
      ) : (
        <div className="grid justify-center items-center gap-2">
          <div className="flex items-center justify-center gap-4">
            {/* Add new Book form */}
            <div>
              <AddBook />
            </div>

            {/* Books sorting */}
            <SortingSelector
              sortingNames={sortingNames}
              currentSort={currentSort}
              setCurrentSort={setCurrentSort}
            />
          </div>

          {/* Search bar */}
          <div className="px-4">
            <SearchBar />
          </div>
        </div>
      )}
    </div>
  );
};

export default Catalog;
