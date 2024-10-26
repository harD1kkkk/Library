import "../styles/components/ContactForm.scss";
import { message } from "antd";
import { Formik, Form, Field } from "formik";
import * as Yup from "yup";
import emailjs from "emailjs-com";

const ContactForm = () => {
  const sendEmail = (values) => {
    emailjs
      .send(
        "service_lgg8taa",
        "template_c6pbcim",
        {
          from_name: values.name,
          from_email: values.email,
          message: values.message,
        },
        "wBZ5V_gmujUfEBsVu"
      )
      .then(
        (response) => {
          message.success("Message sent successfully!");
        },
        (error) => {
          message.error("Failed to send the message.");
        }
      );
  };

  const validationSchema = Yup.object().shape({
    name: Yup.string()
      .min(2, "Name must be at least 2 characters")
      .max(50, "Name must not exceed 50 characters")
      .required("Name is required"),
    email: Yup.string()
      .email("Invalid email format")
      .required("Email is required"),
    message: Yup.string()
      .min(10, "Message must be at least 10 characters")
      .max(500, "Message must not exceed 500 characters")
      .required("Message is required"),
  });

  return (
    <Formik
      initialValues={{ name: "", email: "", message: "" }}
      validationSchema={validationSchema}
      onSubmit={(values, { resetForm }) => {
        sendEmail(values);
        resetForm();
      }}
    >
      {({ errors, touched }) => (
        <Form className="contact-form">
          <div className="mb-8">
            <label className="contact-form__label" htmlFor="name">
              Your Name
            </label>
            <Field
              type="text"
              id="name"
              name="name"
              placeholder="John Doe"
              className={`contact-form__field ${
                touched.name && errors.name ? "error" : ""
              }`}
            />
            {touched.name && errors.name && (
              <div className="error">{errors.name}</div>
            )}
          </div>

          <div className="mb-8">
            <label className="contact-form__label" htmlFor="email">
              Your Email
            </label>
            <Field
              type="email"
              id="email"
              name="email"
              placeholder="you@example.com"
              className={`contact-form__field ${
                touched.email && errors.email ? "error" : ""
              }`}
            />
            {touched.email && errors.email && (
              <div className="error">{errors.email}</div>
            )}
          </div>

          <div className="mb-8">
            <label className="contact-form__label" htmlFor="message">
              Message
            </label>
            <Field
              as="textarea"
              id="message"
              name="message"
              rows="4"
              placeholder="Your message here..."
              className={`contact-form__field ${
                touched.message && errors.message ? "error" : ""
              }`}
            />
            {touched.message && errors.message && (
              <div className="error">{errors.message}</div>
            )}
          </div>

          <button type="submit" className="contact-form__send-btn">
            Send Message
          </button>
        </Form>
      )}
    </Formik>
  );
};

export default ContactForm;
