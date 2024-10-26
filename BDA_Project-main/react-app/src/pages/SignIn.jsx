import "../styles/pages/Sign.scss";
import library from "../data/images/library.avif";
import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import InputSign from "../components/library/Catalog/InputSign";
import { useDispatch, useSelector } from "react-redux";
import { signInUser, resetSignIn } from "../store/slices/signinSlice";
import { setUserInfo } from "../store/slices/userSlice";
import { message } from "antd";

const SignIn = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const { success, error, user } = useSelector((state) => state.user);
  const [formData, setFormData] = useState({
    email: "",
    password: "",
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const resultAction = await dispatch(signInUser(formData));

      if (signInUser.fulfilled.match(resultAction)) {
        const { name, email } = resultAction.payload;
        dispatch(setUserInfo({ name, email }));
        message.success("You have signed in successfully!");
        navigate("/library");
      } else {
        message.error("Sign in failed. Please check your credentials.");
      }
    } catch (err) {
      message.error("An error occurred during sign-in.");
    }
  };

  useEffect(() => {
    if (user && success) {
      const { name, email } = user;
      dispatch(setUserInfo({ name, email }));
    }

    return () => {
      dispatch(resetSignIn());
    };
  }, [success, user, dispatch]);

  return (
    <div className="sign-in">
      <div className="bg-img">
        <img src={library} alt="Library Bg" className="bg-img__img" />
      </div>
      <div className="sign-in__content">
        <form onSubmit={handleSubmit} className="sign-in__form">
          <div className="mb-12">
            <h3 className="sign-in__title">Login</h3>
          </div>
          <InputSign
            label="Email"
            type="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
            placeholder="Enter email"
          />
          <InputSign
            label="Password"
            type="password"
            name="password"
            value={formData.password}
            onChange={handleChange}
            placeholder="Enter password"
          />
          {error && <p className="error">{error}</p>}
          {success && <p className="success">{success}</p>}
          <button type="submit" className="sign-in__submit">
            Login
          </button>
          <p className="sign-in__text">
            Don't have an account?{" "}
            <button
              type="button"
              onClick={() => navigate("/library/sign-up")}
              className="sign-in__btn"
            >
              Register here
            </button>
          </p>
        </form>
      </div>
    </div>
  );
};

export default SignIn;
