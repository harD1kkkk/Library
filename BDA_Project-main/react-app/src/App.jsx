import "./App.scss";
import { Outlet, ScrollRestoration } from "react-router-dom";
import NavBar from "./components/NavBar";
import Footer from "./components/Footer";

function App() {
  return (
    <div className="flex relative">
      <ScrollRestoration />
      <div className="flex flex-col min-h-screen w-full bg-main-bg">
        <NavBar />

        <div className="flex-grow pt-20">
          <Outlet />
        </div>

        <Footer />
      </div>
    </div>
  );
}

export default App;
