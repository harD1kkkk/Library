import "../styles/pages/Sign.scss";
import library from "../data/images/library.avif";
import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import InputSign from "../components/library/Catalog/InputSign";
import { useDispatch, useSelector } from "react-redux";
import { signUpUser, resetSignUp } from "../store/slices/signupSlice";
import { signUpInputs } from "../data/dummy";
import { message } from "antd";

const SignUp = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const { success, error } = useSelector((state) => state.signup);
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    password: "",
    confirmPassword: "",
  });
  const [passwordError, setPasswordError] = useState("");

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
    setPasswordError("");
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (formData.password !== formData.confirmPassword) {
      setPasswordError("Passwords do not match.");
      return;
    }
    dispatch(signUpUser(formData));
  };

  useEffect(() => {
    if (success) {
      setFormData({ name: "", email: "", password: "", confirmPassword: "" });
      message.success("The user registered successfully!");
      setTimeout(() => {
        navigate("/library/sign-in");
      }, 3000);
    }
    return () => {
      dispatch(resetSignUp());
    };
  }, [success, dispatch, navigate]);

  return (
    <div className="sign-up">
      <div className="bg-img">
        <img src={library} alt="Library Bg" className="bg-img__img" />
      </div>
      <div className="sign-up__content">
        <form onSubmit={handleSubmit} className="sign-up__form">
          <div className="mb-12">
            <h3 className="sign-up__title">Register</h3>
          </div>
          {signUpInputs.map((input, index) => (
            <InputSign
              key={index}
              label={input.label}
              type={input.type}
              name={input.name}
              value={formData[input.name]}
              onChange={handleChange}
              placeholder={input.placeholder}
            />
          ))}
          {passwordError && <p className="error">{passwordError}</p>}
          {error && <p className="error">{error}</p>}
          {success && <p className="success">{success}</p>}
          <button type="submit" className="sign-up__submit">
            Register
          </button>
          <p className="sign-up__text">
            Already have an account?{" "}
            <button
              type="button"
              onClick={() => navigate("/library/sign-in")}
              className="sign-up__btn"
            >
              Login here
            </button>
          </p>
        </form>
      </div>
    </div>
  );
};

export default SignUp;
