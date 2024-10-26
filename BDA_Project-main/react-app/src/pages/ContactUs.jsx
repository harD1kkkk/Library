import { ContactForm } from "../components/";
import "../styles/pages/ContactUs.scss";

const contactDetails = [
  { label: "Phone", value: "+38 (032) 123-4567" },
  { label: "Email", value: "info@bda-library.com" },
  { label: "Address", value: "BDA Library, Central St., Drohobych, Ukraine" },
];

const ContactUs = () => {
  return (
    <div className="contact-us">
      <div className="contact-us__background"></div>
      <div className="contact-us__circle-1"></div>
      <div className="contact-us__circle-2"></div>

      <div className="contact-us__container">
        <h1 className="contact-us__container__title">Contact Us</h1>

        <div className="contact-us__container__flex-container">
          {" "}
          <div className="contact-us__container__get-in-touch">
            <h2 className="contact-us__container__get-in-touch__heading">
              Get in Touch
            </h2>
            <div className="contact-us__container__get-in-touch__details">
              {contactDetails.map((detail, index) => (
                <p key={index}>
                  <strong className="contact-us__container__get-in-touch__details__label">
                    {detail.label}:
                  </strong>{" "}
                  {detail.value}
                </p>
              ))}
            </div>
          </div>
          <div className="contact-us__container__form">
            <ContactForm />
          </div>
        </div>

        <div className="contact-us__container__divider">
          <div className="contact-us__container__divider__line"></div>
        </div>
      </div>
    </div>
  );
};

export default ContactUs;
