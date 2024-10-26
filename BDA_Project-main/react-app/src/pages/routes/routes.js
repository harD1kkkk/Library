import { createBrowserRouter } from "react-router-dom";

import App from "../../App";
import {
  Home,
  Library,
  BookPage,
  Favorite,
  AboutUs,
  ContactUs,
  SignIn,
  SignUp,
} from "../";

const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    errorElement: <h1>Page Not Found</h1>,
    children: [
      {
        index: true,
        component: <Home />,
      },
      {
        path: "/library",
        element: <Library />,
      },
      {
        path: "/library/:id",
        element: <BookPage />,
      },
      {
        path: "/favorite",
        element: <Favorite />,
      },
      {
        path: "/about-us",
        element: <AboutUs />,
      },
      {
        path: "/contact-us",
        element: <ContactUs />,
      },
      {
        path: "/library/sign-in",
        element: <SignIn />,
      },
      {
        path: "/library/sign-up",
        element: <SignUp />,
      },
    ],
  },
]);

export default router;
