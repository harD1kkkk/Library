import "../styles/components/NavBar.scss";
import logo from "../data/images/bda_logo.png";
import { useState } from "react";
import { NavLink } from "react-router-dom";
import { FaBars, FaTimes } from "react-icons/fa";
import UserInfo from "./library/UserInfo";
import { useSelector } from "react-redux";
import { selectUser } from "../store/slices/userSlice";

const links = [
  { name: "Library", link: "/library" },
  { name: "Favorite", link: "/favorite" },
  { name: "About Us", link: "/about-us" },
  { name: "Contact Us", link: "/contact-us" },
];

const NavBar = () => {
  const [open, setOpen] = useState(false);
  const user = useSelector(selectUser);

  const toggleMenu = () => setOpen(!open);

  return (
    <div className="navBar">
      <div className="navBar__content">
        <div className="flex">
          <NavLink to="/">
            <img src={logo} alt="logo" className="navBar__logo" />
          </NavLink>
        </div>

        <div onClick={toggleMenu} className="navBar__toggle">
          {open ? <FaTimes /> : <FaBars />}
        </div>

        <ul className={`navBar__links ${open ? "top-12" : "top-[-490px]"}`}>
          {links.map(({ name, link }) => (
            <li key={name} className="navBar__item">
              <NavLink
                to={link}
                className="navBar__link"
                onClick={() => setOpen(false)}
              >
                {name}
              </NavLink>
            </li>
          ))}

          {user?.name && user?.email ? (
            <UserInfo />
          ) : (
            <NavLink to="/library/sign-in">
              <button className="signBtn" onClick={() => setOpen(false)}>
                Sign in
              </button>
            </NavLink>
          )}
        </ul>
      </div>
    </div>
  );
};

export default NavBar;
